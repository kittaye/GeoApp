using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PCLStorage;
using System.Threading.Tasks;

namespace GeoApp {
    public class FileService : IDataService {
        public FileService() {
        }

        public async Task<bool> DeleteLocationAsync(int id) {
            foreach (var feature in App.LocationManager.CurrentLocations) {
                if (feature.Properties.Id == id) {
                    App.LocationManager.CurrentLocations.Remove(feature);

                    RootObject rootobject = new RootObject();
                    rootobject.Features = App.LocationManager.CurrentLocations;
                    var json = JsonConvert.SerializeObject(rootobject);

                    IFolder rootFolder = FileSystem.Current.LocalStorage;
                    IFile locations = await GetLocationsFile();
                    await locations.WriteAllTextAsync(json);
                    return true;
                }
            }
            return false;
        }

        public Task<List<Feature>> RefreshDataAsync() {
            return Task.Run(async () => {
                List<Feature> features = new List<Feature>();

                IFile locations = await GetLocationsFile();

                //Debug.WriteLine(await locations.ReadAllTextAsync());
                var rootobject = JsonConvert.DeserializeObject<RootObject>(await locations.ReadAllTextAsync());
                features = rootobject.Features;

                // Determine the icon used for each feature based on it's geometry type.
                // Also properly deserialize the list of coordinates into an app-use-specific list of Points.
                foreach (var feature in features) {
                    feature.Properties.XamarinCoordinates = new List<Point>();
                    if (feature.Properties.MetadataFields == null || feature.Properties.MetadataFields.Count == 0) {
                        feature.Properties.MetadataFields = new Dictionary<string, object>();
                    }

                    switch (Enum.Parse(typeof(DataType), feature.Geometry.Type)) {
                        case DataType.Point:
                            feature.Properties.TypeIconPath = "point_icon.png";
                            break;
                        case DataType.LineString:
                            feature.Properties.TypeIconPath = "line_icon.png";
                            break;
                        case DataType.Polygon:
                            feature.Properties.TypeIconPath = "area_icon.png";
                            break;
                        default:
                            Debug.WriteLine($"::::::::::::::::::::: UNSUPPORTED DATATYPE: {feature.Geometry.Type}");
                            feature.Properties.TypeIconPath = "point_icon.png";
                            break;
                    }

                    if (feature.Geometry.Type == "Point") {
                        feature.Properties.XamarinCoordinates.Add(new Point(
                            (double)feature.Geometry.Coordinates[0],
                            (double)feature.Geometry.Coordinates[1],
                            (double)feature.Geometry.Coordinates[2]));
                    } else {
                        for (int i = 0; i < feature.Geometry.Coordinates.Count; i++) {
                            feature.Properties.XamarinCoordinates.Add(new Point(
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.Geometry.Coordinates[i]))[0]),
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.Geometry.Coordinates[i]))[1]),
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.Geometry.Coordinates[i]))[2])));
                        }
                    }
                }
                return features;
            });
        }

        public Task EditSaveLocationAsync(Feature location) {
            return Task.Run(async () => {
                int indexToEdit = -1;
                for (int i = 0; i < App.LocationManager.CurrentLocations.Count; i++) {
                    if (App.LocationManager.CurrentLocations[i].Properties.Id == location.Properties.Id) {
                        indexToEdit = i;
                        break;
                    }
                }

                if(indexToEdit != -1) {
                    App.LocationManager.CurrentLocations[indexToEdit] = location;
                    //App.LocationManager.CurrentLocations[indexToEdit].Properties.Name = location.Properties.Name;
                    //App.LocationManager.CurrentLocations[indexToEdit].Geometry.XamarinCoordinates = location.Geometry.XamarinCoordinates;
                }

                RootObject rootobject = new RootObject();
                rootobject.Features = App.LocationManager.CurrentLocations;

                var json = JsonConvert.SerializeObject(rootobject);
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFile locations = await GetLocationsFile();
                await locations.WriteAllTextAsync(json);
            });
        }

        public Task SaveLocationAsync(Feature location)
        {
            return Task.Run(async () =>
            {
                List<Feature> existingLocations = await App.LocationManager.GetLocationsAsync();

                RootObject rootobject = new RootObject();
                rootobject.Features = existingLocations;

                location.Properties.Id = DateTime.Now.Millisecond.GetHashCode();
                rootobject.Features.Add(location);

                var json = JsonConvert.SerializeObject(rootobject);
                App.LocationManager.CurrentLocations = rootobject.Features;

                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFile locations = await GetLocationsFile();
                await locations.WriteAllTextAsync(json);
            });
        }

        public async Task<IFile> GetLocationsFile() {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            Debug.Write(rootFolder);
            rootFolder.Path.Replace("/../Library", " ");

            ExistenceCheckResult result = await rootFolder.CheckExistsAsync("locations7.json");
            if (result != ExistenceCheckResult.FileExists) {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations7.json", CreationCollisionOption.ReplaceExisting);

                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                Stream stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
                string json;
                using (var reader = new System.IO.StreamReader(stream)) {
                    json = reader.ReadToEnd();
                }

                await locationsFile.WriteAllTextAsync(json);
                return locationsFile;

            } else {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations7.json", CreationCollisionOption.OpenIfExists);
                return locationsFile;
            }
        }

        public void AddLocationsFromFile(string path) {
            Debug.WriteLine("HERE 2222222222!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            List<Feature> features = new List<Feature>();
            String text = File.ReadAllText(path);
            var rootobject = JsonConvert.DeserializeObject<RootObject>(text);
            features = rootobject.Features;

            foreach (var feature in features) {
                SaveLocationAsync(feature);
            }
        }

        public async Task ImportLocationsAsync(string fileContents) {
            // add feature list range to current locations
            try {
                List<Feature> features = new List<Feature>();
                var importedRootObject = JsonConvert.DeserializeObject<RootObject>(fileContents);
                features = importedRootObject.Features;
                App.LocationManager.CurrentLocations = await App.LocationManager.GetLocationsAsync();

                App.LocationManager.CurrentLocations.AddRange(importedRootObject.Features);
                importedRootObject.Features = App.LocationManager.CurrentLocations;

                var json = JsonConvert.SerializeObject(importedRootObject);

                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFile locations = await GetLocationsFile();
                await locations.WriteAllTextAsync(json);
            } catch (Exception ex) {
                await HomePage.Instance.DisplayAlert("Invalid File Contents!", "Please make sure your GeoJSON is formatted correctly.", "OK");
                Debug.WriteLine(ex);
                throw ex;
            }

        }
    }
}
