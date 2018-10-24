using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PCLStorage;
using System.Threading.Tasks;

namespace GeoApp {
    public class FileService : IDataService {
        public FileService() {
        }

        public Task DeleteLocationAsync(string Name) {
            throw new NotImplementedException();
        }

        public Task DeleteLocationAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<List<Feature>> RefreshDataAsync() {
            return Task.Run(async () => {
                List<Feature> features = new List<Feature>();

                IFile locations = await GetLocationsFile();

                var rootobject = JsonConvert.DeserializeObject<RootObject>(await locations.ReadAllTextAsync());
                features = rootobject.Features;

                // Determine the icon used for each feature based on it's geometry type.
                // Also properly deserialize the list of coordinates into an app-use-specific list of Points.
                foreach (var feature in features) {
                    feature.Geometry.XamarinCoordinates = new List<Point>();

                    switch (feature.Geometry.Type) {
                        case DataType.Point:
                            feature.Properties.TypeIconPath = "point_icon.png";
                            break;
                        case DataType.Line:
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

                    if (feature.Geometry.Type == DataType.Point) {
                        feature.Geometry.XamarinCoordinates.Add(new Point(
                            (double)feature.Geometry.Coordinates[0],
                            (double)feature.Geometry.Coordinates[1],
                            (double)feature.Geometry.Coordinates[2]));
                    } else {
                        for (int i = 0; i < feature.Geometry.Coordinates.Count; i++) {
                            feature.Geometry.XamarinCoordinates.Add(new Point(
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.Geometry.Coordinates[i]))[0]),
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.Geometry.Coordinates[i]))[1]),
                                (double)(((Newtonsoft.Json.Linq.JArray)(feature.Geometry.Coordinates[i]))[2])));
                        }
                    }
                }
                return features;
            });
        }

        public Task SaveLocationAsync(Feature location)
        {
            return Task.Run(async () =>
            {
                List<Feature> existingLocations = await App.LocationManager.GetLocationsAsync();

                bool isEdit = false;
                for (int i = 0; i < existingLocations.Count; i++) {
                    if(existingLocations[i].Properties.Id == location.Properties.Id) {
                        existingLocations[i] = location;
                        isEdit = true;
                        break;
                    }
                }

                RootObject rootobject = new RootObject();
                rootobject.Features = existingLocations;

                if (isEdit == false) {
                    location.Properties.Id = DateTime.Now.Millisecond.GetHashCode();
                    rootobject.Features.Add(location);
                }

                var json = JsonConvert.SerializeObject(rootobject);

                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFile locations = await GetLocationsFile();
                await locations.WriteAllTextAsync(json);
            });
        }

        public async Task<IFile> GetLocationsFile()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            ExistenceCheckResult result = await rootFolder.CheckExistsAsync("locations3.json");
            if(result != ExistenceCheckResult.FileExists) {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations3.json", CreationCollisionOption.ReplaceExisting);

                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                Stream stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
                string json;
                using (var reader = new System.IO.StreamReader(stream)) {
                    json = reader.ReadToEnd();
                }

                await locationsFile.WriteAllTextAsync(json);
                return locationsFile;

            } else {
                IFile locationsFile = await rootFolder.CreateFileAsync("locations3.json", CreationCollisionOption.OpenIfExists);
                return locationsFile;
            }
        }
    }
}
