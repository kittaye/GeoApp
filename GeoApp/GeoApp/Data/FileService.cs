using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PCLStorage;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GeoApp {
    public class FileService : IDataService {
        public FileService() {
        }

        public async Task<bool> DeleteLocationAsync(int id) {
            foreach (var feature in App.LocationManager.CurrentLocations) {
                if (feature.properties.id == id) {
                    App.LocationManager.CurrentLocations.Remove(feature);

                    RootObject rootobject = new RootObject();
                    rootobject.features = App.LocationManager.CurrentLocations;
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

                var rootobject = JsonConvert.DeserializeObject<RootObject>(await locations.ReadAllTextAsync());
                features = rootobject.features;

                // Determine the icon used for each feature based on it's geometry type.
                // Also properly deserialize the list of coordinates into an app-use-specific list of Points.
                foreach (var feature in features) {
                    feature.properties.xamarincoordinates = new List<Point>();
                    if (feature.properties.metadatafields == null || feature.properties.metadatafields.Count == 0) {
                        feature.properties.metadatafields = new Dictionary<string, object>();
                    }

                    Console.WriteLine(feature.properties.date);

                    switch (Enum.Parse(typeof(DataType), feature.geometry.type)) {
                        case DataType.Point:
                            feature.properties.typeIconPath = "point_icon.png";
                            break;
                        case DataType.LineString:
                            feature.properties.typeIconPath = "line_icon.png";
                            break;
                        case DataType.Polygon:
                            feature.properties.typeIconPath = "area_icon.png";
                            break;
                        default:
                            Debug.WriteLine($"::::::::::::::::::::: UNSUPPORTED DATATYPE: {feature.geometry.type}");
                            feature.properties.typeIconPath = "point_icon.png";
                            break;
                    }

                    if (feature.geometry.type == "Point") {
                        feature.properties.xamarincoordinates.Add(new Point(
                            (double)feature.geometry.coordinates[0],
                            (double)feature.geometry.coordinates[1],
                            (double)feature.geometry.coordinates[2]));
                    } else {
                        for (int i = 0; i < feature.geometry.coordinates.Count; i++) {
                            feature.properties.xamarincoordinates.Add(new Point(
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.geometry.coordinates[i]))[0]),
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.geometry.coordinates[i]))[1]),
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.geometry.coordinates[i]))[2])));
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
                    if (App.LocationManager.CurrentLocations[i].properties.id == location.properties.id) {
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
                rootobject.features = App.LocationManager.CurrentLocations;

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
                rootobject.features = existingLocations;

                location.properties.id = DateTime.Now.Millisecond.GetHashCode();
                rootobject.features.Add(location);

                var json = JsonConvert.SerializeObject(rootobject);
                App.LocationManager.CurrentLocations = rootobject.features;

                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFile locations = await GetLocationsFile();
                await locations.WriteAllTextAsync(json);
            });
        }

        public async Task<IFile> GetLocationsFile() {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            Debug.Write(rootFolder);
            rootFolder.Path.Replace("/../Library", " ");

            ExistenceCheckResult result = await rootFolder.CheckExistsAsync("locations9.json");
            if (result != ExistenceCheckResult.FileExists) {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations9.json", CreationCollisionOption.ReplaceExisting);

                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                Stream stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
                string json;
                using (var reader = new System.IO.StreamReader(stream)) {
                    json = reader.ReadToEnd();
                }

                await locationsFile.WriteAllTextAsync(json);
                return locationsFile;

            } else {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations9.json", CreationCollisionOption.OpenIfExists);
                return locationsFile;
            }
        }

        public void AddLocationsFromFile(string path) {
            Debug.WriteLine("HERE 2222222222!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            List<Feature> features = new List<Feature>();
            String text = File.ReadAllText(path);
            var rootobject = JsonConvert.DeserializeObject<RootObject>(text);
            features = rootobject.features;

            foreach (var feature in features) {
                SaveLocationAsync(feature);
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

        public async Task<string> ExportLocationsAsync() {
            try {
                var storedLocations = await App.LocationManager.GetLocationsAsync();

                // export object model that matches geojson standard
                List<ExportModel> exportObject = new List<ExportModel> {
                    new ExportModel{ type = "FeatureCollection", features = storedLocations }
                };

                var json = JsonConvert.SerializeObject(exportObject);

                return json;
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                throw ex;
            }
        }
    }
}
