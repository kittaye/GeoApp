using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace GeoApp.Data
{
    public class FeatureStore
    {

        public List<Feature> CurrentFeatures { get; set; } = new List<Feature>();
        private string DATA_PATH = FileSystem.AppDataDirectory + "/";
        private const string EMBEDDED_FILENAME = "locations.json";
        private const string LOG_FILENAME = "log.csv";

        public Task<List<Feature>> GetFeaturesAsync()
        {
            return Task.Run(async () =>
            {
                string featuresFile = GetFeaturesFile();
                Debug.WriteLine("{0}", featuresFile);
                var rootobject = JsonConvert.DeserializeObject<RootObject>(featuresFile);
                if (rootobject == null)
                {
                    Debug.WriteLine("\n\n::::::::::::::::::::::DESERIALIZATION FAILED");
                    throw new Exception();
                }

                rootobject.type = "FeatureCollection";

                foreach (var feature in rootobject.features)
                {
                    await TryParseFeature(feature);
                }

                return rootobject.features;
            });
        }

        private async Task<bool> TryParseFeature(Feature feature)
        {

            // Ensure the feature has valid GeoJSON fields supplied.
            if (feature == null || feature.type == null || feature.geometry == null || feature.geometry.type == null || feature.geometry.coordinates == null)
            {
                await HomePage.Instance.DisplayAlert("Invalid File", "Ensure your file only contains data in valid GeoJSON format.", "OK");
                return false;
            }

            // Immediately convert LineStrings to Line for use in the rest of the codebase. 
            // This will be converted back to LineString before serialization back to json.
            if (feature.geometry.type == "LineString")
            {
                feature.geometry.type = "Line";
            }

            if (string.IsNullOrWhiteSpace(feature.properties.name))
            {
                feature.properties.name = "Unnamed " + feature.geometry.type;
            }

            // If author ID hasn't been set on the feature, default it to the user's ID.
            if (string.IsNullOrWhiteSpace(feature.properties.authorId))
            {
                if (App.Current.Properties.ContainsKey("UserID"))
                {
                    feature.properties.authorId = App.Current.Properties["UserID"] as string;
                }
                else
                {
                    feature.properties.authorId = string.Empty;
                }
            }

            // If the date field is missing or invalid, convert it into DateTime.Now.
            DateTime dummy;
            if (feature.properties.date == null || DateTime.TryParse(feature.properties.date, out dummy) == false)
            {
                Debug.WriteLine($"\n\n::::::::::::::::::::::BAD DATE VALUE: {feature.properties.date}, DEFAULTING TO DateTime.Now");
                feature.properties.date = DateTime.Now.ToShortDateString();
            }

            // Determine the icon used for each feature based on it's geometry type.
            if (feature.geometry.type == "Point")
            {
                feature.properties.typeIconPath = "point_icon.png";
            }
            else if (feature.geometry.type == "Line")
            {
                feature.properties.typeIconPath = "line_icon.png";
            }
            else if (feature.geometry.type == "Polygon")
            {
                feature.properties.typeIconPath = "area_icon.png";
            }
            else
            {
                await HomePage.Instance.DisplayAlert("Import Error", "Groundsman currently only supports feature types of Point, Line, and Polygon.", "OK");
                return false;
            }

            // Initialise xamarin coordinates list.
            feature.properties.xamarincoordinates = new List<Point>();

            // Properly deserialize the list of coordinates into an app-use-specific list of Points (XamarinCoordinates).
            {
                object[] trueCoords;

                if (feature.geometry.type == "Point")
                {
                    trueCoords = feature.geometry.coordinates.ToArray();
                    feature.properties.xamarincoordinates.Add(JsonCoordToXamarinPoint(trueCoords));

                }
                else if (feature.geometry.type == "Line")
                {
                    // Iterates the root coordinates (List<object>),
                    // then casts each element in the list to a Jarray which contain the actual coordinates.
                    for (int i = 0; i < feature.geometry.coordinates.Count; i++)
                    {
                        trueCoords = ((JArray)feature.geometry.coordinates[i]).ToObject<object[]>();
                        feature.properties.xamarincoordinates.Add(JsonCoordToXamarinPoint(trueCoords));
                    }
                }
                else if (feature.geometry.type == "Polygon")
                {
                    // Iterates the root coordinates (List<object>), and casts each element in the list to a Jarray, 
                    // then casts each Jarray's element to another Jarray which contain the actual coordinates.
                    for (int i = 0; i < feature.geometry.coordinates.Count; i++)
                    {
                        for (int j = 0; j < ((JArray)feature.geometry.coordinates[i]).Count; j++)
                        {
                            trueCoords = ((JArray)(((JArray)feature.geometry.coordinates[i])[j])).ToObject<object[]>();
                            feature.properties.xamarincoordinates.Add(JsonCoordToXamarinPoint(trueCoords));
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private Point JsonCoordToXamarinPoint(object[] coords)
        {
            double longitude = (double)coords[0];
            double latitude = (double)coords[1];
            double altitude = (coords.Length == 3) ? (double)coords[2] : 0.0;

            Point point = new Point(latitude, longitude, altitude);
            AppConstants.RoundGPSPosition(point);
            return point;
        }

        public async Task<bool> DeleteFeatureAsync(int featureID)
        {
            Feature featureToDelete = App.FeatureStore.CurrentFeatures.Find((feature) => (feature.properties.id == featureID));
            bool deleteSuccessful = App.FeatureStore.CurrentFeatures.Remove(featureToDelete);

            if (deleteSuccessful)
            {
                await SaveCurrentFeaturesToEmbeddedFile();
                return true;
            }
            else
            {
                Debug.WriteLine($"\n\n:::::::::::::::::::::::FAILED TO DELETE FEATURE OF ID: {featureID}");
                return false;
            }
        }

        public async Task<bool> SaveFeatureAsync(Feature feature)
        {
            // If this is a newly added feature, generate an ID and add it immediately.
            if (feature.properties.id == AppConstants.NEW_ENTRY_ID)
            {
                TryGetUniqueFeatureID(feature);
                App.FeatureStore.CurrentFeatures.Add(feature);
            }
            else
            {
                // Otherwise we are saving over an existing feature, so override its contents without changing ID.
                int indexToEdit = -1;
                for (int i = 0; i < App.FeatureStore.CurrentFeatures.Count; i++)
                {
                    if (App.FeatureStore.CurrentFeatures[i].properties.id == feature.properties.id)
                    {
                        indexToEdit = i;
                        break;
                    }
                }

                if (indexToEdit != -1)
                {
                    App.FeatureStore.CurrentFeatures[indexToEdit] = feature;
                }
                else
                {
                    Debug.WriteLine($"\n\n::::::::::::::::::::::FAILED TO SAVE EDIT FOR FEATURE WITH ID: {feature.properties.id}");
                    return false;
                }
            }

            await SaveCurrentFeaturesToEmbeddedFile();
            return true;
        }

        public async Task SaveAllCurrentFeaturesAsync()
        {
            await SaveCurrentFeaturesToEmbeddedFile();
        }

        /// <summary>
        /// Formats the list of current features into valid geojson, then writes it to the embedded file.
        /// </summary>
        /// <returns></returns>
        private async Task SaveCurrentFeaturesToEmbeddedFile()
        {
            var objToSave = FormatCurrentFeaturesIntoGeoJSON();

            // Save the rootobject to file.
            var json = JsonConvert.SerializeObject(objToSave);
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, EMBEDDED_FILENAME), json);

            // Mark the features list as dirty so it can refresh.
            DataEntryListViewModel.isDirty = true;
        }

        /// <summary>
        /// Takes the current list of features and prepare the contents into a valid geoJSON serializable structure.
        /// </summary>
        /// <returns></returns>
        private RootObject FormatCurrentFeaturesIntoGeoJSON()
        {
            var rootobject = new RootObject();
            rootobject.type = "FeatureCollection";
            rootobject.features = App.FeatureStore.CurrentFeatures;

            foreach (var feature in rootobject.features)
            {
                // Convert Lines back into LineStrings for valid geojson.
                if (feature.geometry.type == "Line")
                {
                    feature.geometry.type = "LineString";
                }
            }

            // Mark the features list as being modified, since the feature types had to be converted to LineStrings for export.
            // The dirty flag will make sure the line features in the list are refreshed back to "Line" types next time that page is viewed.
            DataEntryListViewModel.isDirty = true;

            return rootobject;
        }

        public async Task DeleteAllFeatures()
        {
            App.FeatureStore.CurrentFeatures.Clear();
            await SaveCurrentFeaturesToEmbeddedFile();
        }

        /// <summary>
        /// Imports features from the contents of a file.
        /// </summary>
        /// <param name="fileContents">The string of geojson to import from.</param>
        /// <returns></returns>
        public async Task<bool> ImportFeaturesAsync(string fileContents)
        {
            try
            {
                // Ensure file contents are structured in a valid GeoJSON format.
                var importedRootObject = JsonConvert.DeserializeObject<RootObject>(fileContents);
                if (importedRootObject == null)
                {
                    await HomePage.Instance.DisplayAlert("Invalid File Contents", "Ensure your file only contains data in valid GeoJSON format.", "OK");
                    return false;
                }

                // Loop through all imported features and make sure they are valid.
                foreach (var importedFeature in importedRootObject.features)
                {
                    bool parseResult = await TryParseFeature(importedFeature);

                    if (parseResult == false)
                    {
                        return false;
                    }
                }

                // Loop through all imported features one by one, ensuring there are no ID clashes.
                foreach (var importedFeature in importedRootObject.features)
                {
                    TryGetUniqueFeatureID(importedFeature);
                }

                // Finally, add all the imported features to the current features list.
                App.FeatureStore.CurrentFeatures.AddRange(importedRootObject.features);

                await SaveCurrentFeaturesToEmbeddedFile();
                await HomePage.Instance.DisplayAlert("File Import", "File imported successfully. New features have been added to your features list.", "OK");
                return true;
            }
            catch (Exception ex)
            {
                await HomePage.Instance.DisplayAlert("Invalid File Contents", "Ensure your file only contains data in valid GeoJSON format.", "OK");
                Debug.WriteLine(ex);
                return false;
            }
        }

        public string GetLogFile()
        {
            // Attempt to open the embedded file on the device. 
            // If it exists return it, else create a new embedded file from a json source file.
            if (File.Exists(DATA_PATH + LOG_FILENAME))
            {
                return File.ReadAllText(DATA_PATH + LOG_FILENAME);
            }
            else
            {
                return "";
            }
        }

        public string GetEmbeddedFile()
        {
            if (File.Exists(DATA_PATH + EMBEDDED_FILENAME))
            {
                return File.ReadAllText(DATA_PATH + EMBEDDED_FILENAME);
            }
            else
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                Stream stream = assembly.GetManifestResourceStream("GeoApp.locationsAutoGenerated.json");
                string text = "";
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
                Debug.WriteLine("\n\n::::::::::::::::::::::No File");
                return text;
            }
        }

        public string GetFeaturesFile()
        {
            if (File.Exists(Path.Combine(FileSystem.AppDataDirectory, EMBEDDED_FILENAME)))
            {
                return File.ReadAllText(Path.Combine(FileSystem.AppDataDirectory, EMBEDDED_FILENAME));
            }
            else
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                Stream stream = assembly.GetManifestResourceStream("GeoApp.locationsAutoGenerated.json");
                string text = "";
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
                Debug.WriteLine("\n\n::::::::::::::::::::::No File");
                return text;
            }
        }

        /// <summary>
        /// Exports the current list of features by serializing to geojson.
        /// </summary>
        /// <returns></returns>
        public string ExportFeaturesToJson()
        {
            try
            {
                var rootobject = FormatCurrentFeaturesIntoGeoJSON();
                var json = JsonConvert.SerializeObject(rootobject, Formatting.Indented);

                // String cleaning
                if (json.StartsWith("[")) json = json.Substring(1);
                if (json.EndsWith("]")) json = json.TrimEnd(']');
                return json;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Imports features from a specified filepath.
        /// </summary>
        /// <param name="path">path to file.</param>
        /// <returns></returns>
        public async Task<bool> ImportFeaturesFromFile(string path)
        {
            try
            {
                string text = File.ReadAllText(path);
                bool resultStatus = await ImportFeaturesAsync(text);
                return resultStatus;
            }
            catch (Exception ex)
            {
                await HomePage.Instance.DisplayAlert("Import Error", "An unknown error occured when trying to process this file.", "OK");
                Debug.WriteLine(ex);
                return false;
            }
        }



        /// <summary>
        /// If necessary, creates a new ID that is unique to all current features stored.
        /// </summary>
        /// <returns>The original ID if no clashes were found, else a new unique ID.</returns>
        public static void TryGetUniqueFeatureID(Feature featureToCheck)
        {
            bool validID = false;

            while (validID == false)
            {
                validID = true;

                if (featureToCheck.properties.id == AppConstants.NEW_ENTRY_ID)
                {
                    validID = false;
                }
                else
                {
                    foreach (var feature in App.FeatureStore.CurrentFeatures)
                    {
                        if (featureToCheck.properties.id == feature.properties.id && featureToCheck != feature)
                        {
                            validID = false;
                            break;
                        }
                    }
                }

                if (validID == false)
                {
                    featureToCheck.properties.id = DateTime.Now.GetHashCode();
                }
            }
        }
    }
}
