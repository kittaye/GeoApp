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
                        string[] res = this.GetType().Assembly.GetManifestResourceNames();
                        var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                        Stream stream = assembly.GetManifestResourceStream(GAStorage);


                        //The App craches without this check
                        if (stream == null) {
                            CreateFile();
                            stream = assembly.GetManifestResourceStream(GAStorage);
                            

                        }
                        using (var reader = new System.IO.StreamReader(stream)) {
                            json = reader.ReadToEnd();
                        }

                    } else {
                        json = File.ReadAllText(GAStorage);
                    }

                    if (string.IsNullOrEmpty(json) == false) {
                        var rootobject = JsonConvert.DeserializeObject<RootObject>(json);
                        features = rootobject.Features.ToList();

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
                rootobject.Features = existingLocations.ToArray<Feature>();
                var json = JsonConvert.SerializeObject(rootobject);

                File.WriteAllText(GAStorage, json);
                hasBeenUpdated = true;
            });
        }

        //Method to load contents of locations.json to GALocations
        public void CreateFile()
        {
            Debug.WriteLine("####################################### 3 Save");
            var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
            var stream = assembly.GetManifestResourceStream("GeoApp.locations.json");
            string json;


            //File.WriteAllText(GAStorage, "Write this text into a file");



            using (var reader = new System.IO.StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            //System.IO.File.Create(GAStorage);
            var text = File.ReadAllText(GAStorage);
            Console.WriteLine(text);
            Console.WriteLine("==============");

            Debug.WriteLine(json);

            Console.WriteLine("==============");
            File.WriteAllText(GAStorage, json);
            text = File.ReadAllText(GAStorage);
            Console.WriteLine(text);
            Console.WriteLine("==============");
        }

    }
}
