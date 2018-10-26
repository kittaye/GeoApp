using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PCLStorage;
using System.Threading.Tasks;


namespace GeoApp {
    public class FileService : IDataService {

        public FileService() { }

        public async Task<bool> DeleteLocationAsync(int id) {
            foreach (var feature in App.LocationManager.CurrentLocations) {
                if (feature.properties.id == id) {
                    App.LocationManager.CurrentLocations.Remove(feature);

                    RootObject rootobject = new RootObject();
                    rootobject.features = App.LocationManager.CurrentLocations;

                    await SaveToFile(rootobject);
                    return true;
                }
            }
            return false;
        }

        public Task<List<Feature>> RefreshDataAsync() {
            return Task.Run(async () => {
                IFile locations = await GetLocationsFile();

                var rootobject = JsonConvert.DeserializeObject<RootObject>(await locations.ReadAllTextAsync());
                rootobject.type = "FeatureCollection";

                foreach (var feature in rootobject.features) {
                    // Initialise xamarin coordinates list and metadata fields list.
                    feature.properties.xamarincoordinates = new List<Point>();
                    if (feature.properties.metadatafields == null || feature.properties.metadatafields.Count == 0) {
                        feature.properties.metadatafields = new Dictionary<string, object>();
                    }

                    // Immediately convert LineStrings to Line for display purposes. 
                    // This will be converted back to LineString before serialization to json.
                    if(feature.geometry.type == "LineString") {
                        feature.geometry.type = "Line";
                    }

                    // Determine the icon used for each feature based on it's geometry type.
                    if (feature.geometry.type == "Point") {
                        feature.properties.typeIconPath = "point_icon.png";
                    } else if (feature.geometry.type == "Line") {
                        feature.properties.typeIconPath = "line_icon.png";
                    } else if (feature.geometry.type == "Polygon") {
                        feature.properties.typeIconPath = "area_icon.png";
                    } else {
                        Debug.WriteLine($":::::::::::::::::::::::::::::INVALID TYPE: {feature.geometry.type}");
                    }

                    // Properly deserialize the list of coordinates into an app-use-specific list of Points (XamarinCoordinates).
                    {
                        object[] trueCoords;

                        if (feature.geometry.type == "Point") {
                            trueCoords = feature.geometry.coordinates.ToArray();
                            feature.properties.xamarincoordinates.Add(JsonCoordToXamarinPoint(trueCoords));

                        } else if (feature.geometry.type == "Line") {
                            // Iterates the root coordinates (List<object>),
                            // then casts each element in the list to a Jarray which contain the actual coordinates.
                            for (int i = 0; i < feature.geometry.coordinates.Count; i++) {
                                trueCoords = ((JArray)feature.geometry.coordinates[i]).ToObject<object[]>();
                                feature.properties.xamarincoordinates.Add(JsonCoordToXamarinPoint(trueCoords));
                            }
                        } else if (feature.geometry.type == "Polygon") {
                            // Iterates the root coordinates (List<object>), and casts each element in the list to a Jarray, 
                            // then casts each Jarray's element to another Jarray which contain the actual coordinates.
                            for (int i = 0; i < feature.geometry.coordinates.Count; i++) {
                                for (int j = 0; j < ((JArray)feature.geometry.coordinates[i]).Count; j++) {
                                    trueCoords = ((JArray)(((JArray)feature.geometry.coordinates[i])[j])).ToObject<object[]>();
                                    feature.properties.xamarincoordinates.Add(JsonCoordToXamarinPoint(trueCoords));
                                }
                            }
                        } else {
                            Debug.WriteLine($":::::::::::::::::::::::::::::INVALID TYPE WHEN PARSING POINTS: {feature.geometry.type}");
                        }
                    }
                }
                return rootobject.features;
            });
        }

        private Point JsonCoordToXamarinPoint(object[] coords) {
            double latitude = (double)coords[0];
            double longitude = (double)coords[1];
            double altitude = (coords.Length == 3) ? (double)coords[2] : 0.0;
            return new Point(latitude, longitude, altitude);
        }

        public Task EditSaveLocationAsync(Feature location) {
            return Task.Run(async () => {
                int indexToEdit = -1;
                for (int i = 0; i < App.LocationManager.CurrentLocations.Count; i++) {
                    if (App.LocationManager.CurrentLocations[i].properties.id == location.properties.id) {
                        indexToEdit = i;
                        break;
                    }
                }

                if(indexToEdit != -1) {
                    App.LocationManager.CurrentLocations[indexToEdit] = location;
                }

                RootObject rootobject = new RootObject();
                rootobject.features = App.LocationManager.CurrentLocations;

                await SaveToFile(rootobject);
            });
        }

        public Task SaveLocationAsync(Feature location)
        {
            return Task.Run(async () =>
            {
                List<Feature> existingLocations = await App.LocationManager.GetLocationsAsync();

                RootObject rootobject = new RootObject();
                rootobject.features = existingLocations;

                location.properties.id = DateTime.Now.Millisecond.GetHashCode();
                rootobject.features.Add(location);

                App.LocationManager.CurrentLocations = rootobject.features;

                await SaveToFile(rootobject);
            });
        }

        public async Task<IFile> GetLocationsFile() {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            Debug.Write(rootFolder);
            rootFolder.Path.Replace("/../Library", " ");

            ExistenceCheckResult result = await rootFolder.CheckExistsAsync("locations10.json");
            if (result != ExistenceCheckResult.FileExists) {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations10.json", CreationCollisionOption.ReplaceExisting);

                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                //Stream stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
                Stream stream = assembly.GetManifestResourceStream("GeoApp.locationsAutoGenerated.json");
                string json;
                using (var reader = new System.IO.StreamReader(stream)) {
                    json = reader.ReadToEnd();
                }

                await locationsFile.WriteAllTextAsync(json);
                return locationsFile;

            } else {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations10.json", CreationCollisionOption.OpenIfExists);
                return locationsFile;
            }
        }

        private async Task SaveToFile(RootObject objToSave) {
            foreach (var feature in objToSave.features) {
                if(feature.geometry.type == "Line") {
                    feature.geometry.type = "LineString";
                }
            }

            var json = JsonConvert.SerializeObject(objToSave);
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile locations = await GetLocationsFile();
            await locations.WriteAllTextAsync(json);
        }

        public async Task AddLocationsFromFile(string path) {
            try
            {
                List<Feature> features = new List<Feature>();
                String text = File.ReadAllText(path);
                var importedRootObject = JsonConvert.DeserializeObject<RootObject>(text);
                features = importedRootObject.features;

                App.LocationManager.CurrentLocations = await App.LocationManager.GetLocationsAsync();

                App.LocationManager.CurrentLocations.AddRange(importedRootObject.features);
                importedRootObject.features = App.LocationManager.CurrentLocations;

                await SaveToFile(importedRootObject);
            }
            catch (Exception ex)
            {
                await HomePage.Instance.DisplayAlert("Invalid File Contents!", "Please make sure your GeoJSON is formatted correctly.", "OK");
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        public async Task ImportLocationsAsync(string fileContents) {
            // add feature list range to current locations
            try {
                List<Feature> features = new List<Feature>();
                var importedRootObject = JsonConvert.DeserializeObject<RootObject>(fileContents);
                features = importedRootObject.features;
                App.LocationManager.CurrentLocations = await App.LocationManager.GetLocationsAsync();

                App.LocationManager.CurrentLocations.AddRange(importedRootObject.features);
                importedRootObject.features = App.LocationManager.CurrentLocations;

                await SaveToFile(importedRootObject);
            } catch (Exception ex) {
                await HomePage.Instance.DisplayAlert("Invalid File Contents!", "Please make sure your GeoJSON is formatted correctly.", "OK");
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        public async Task<string> ExportLocationsAsync() {
            try {
                var storedLocations = await App.LocationManager.GetLocationsAsync();

                var rootobject = new RootObject();
                rootobject.features = storedLocations;
                rootobject.type = "FeatureCollection";
                foreach (var feature in rootobject.features) {
                    if (feature.geometry.type == "Line") {
                        feature.geometry.type = "LineString";
                    }
                }
                var json = JsonConvert.SerializeObject(rootobject);
                // string cleaning
                if (json.StartsWith("[")) json = json.Substring(1);
                
                if (json.EndsWith("]")) json = json.TrimEnd(']');

                return json;
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                throw ex;
            }
        }
    }
}
