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
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GALocations.txt");

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
                    Feature[] features = { };

                    string json;

                    if (hasBeenUpdated == false) {
                        string[] res = this.GetType().Assembly.GetManifestResourceNames();
                        var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                        Stream stream = assembly.GetManifestResourceStream(fileName);
                        Debug.WriteLine("HELLO:::::::::::::              {0}", stream);

                        if (stream == null) {
                            stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
                        }
                        Debug.WriteLine("HELLO2:::::::::::::              {0}", stream);
                        using (var reader = new System.IO.StreamReader(stream)) {


                            json = reader.ReadToEnd();
                        }
                        Debug.WriteLine("HELLO3");

                    } else {
                        json = File.ReadAllText(fileName);
                    }

                    if (json != null) {
                        var rootobject = JsonConvert.DeserializeObject<RootObject>(json);
                        Debug.WriteLine("HELLO:::::::::::::              {0}", rootobject);
                        //locations = null;
                        features = rootobject.Features;
                        //locations[0] = rootobject;
                        Debug.WriteLine("HELLO:::::::::::::              {0},{1}", rootobject.Features[0].Properties.Name, rootobject.Features[1].Properties.Name);
                    }

                    // Determine the icon used for each feature based on it's geometry type.
                    foreach (var feature in features) {
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
                    }

                    return features.ToList();
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
                rootobject.Features = existingLocations.ToArray<Feature>();
                var json = JsonConvert.SerializeObject(rootobject);

                File.WriteAllText(fileName, json);
                hasBeenUpdated = true;
            });
        }
    }
}
