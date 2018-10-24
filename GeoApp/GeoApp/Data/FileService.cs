using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GeoApp {
    public class FileService : IDataService {
        bool hasBeenUpdated = false;
        bool fileEmpty = true;
        string GAStorage = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GALocations.txt");
        //string fileName = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "GALocations.txt");

        public FileService() {
        }

        public Task DeleteLocationAsync(string Name) {
            throw new NotImplementedException();
        }

        public Task DeleteLocationAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<List<Feature>> RefreshDataAsync() {

            return Task.Run(() => {

                // Exception handling for file service
                try {
                    List<Feature> features = new List<Feature>();

                    string json;

                    if (hasBeenUpdated == false) {
                        if (fileEmpty) {
                            CreateFile();
                            fileEmpty = false;

                        }

                        using (var reader = new System.IO.StreamReader(GAStorage)) {
                            json = reader.ReadToEnd();
                        }

                    } else {
                        json = File.ReadAllText(GAStorage);
                    }

                    if (string.IsNullOrEmpty(json) == false) {
                        var rootobject = JsonConvert.DeserializeObject<RootObject>(json);
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

                            if(feature.Geometry.Type == DataType.Point) {
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
                    }

                    return features;
                } catch (Exception e) {
                    Debug.WriteLine(e);
                    throw e;
                }
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

                if (isEdit == false) {
                    location.Properties.Id = DateTime.Now.Millisecond.GetHashCode();
                }

                RootObject rootobject = new RootObject();
                rootobject.Features = existingLocations;
                var json = JsonConvert.SerializeObject(rootobject);

                File.WriteAllText(GAStorage, json);
                hasBeenUpdated = true;
            });
        }

        //Method to load contents of locations.json to GALocations
        public void CreateFile()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
            var stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
            string json;
            using (var reader = new System.IO.StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            File.WriteAllText(GAStorage, json);
        }

    }
}
