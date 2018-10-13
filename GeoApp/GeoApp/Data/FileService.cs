using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GeoApp
{
    public class FileService : IDataService
    {
        bool hasBeenUpdated = false;
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "locations.json");

        public FileService()
        {
        }

        public Task DeleteLocationAsync(string Name)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLocationAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Properties>> RefreshDataAsync()
        {
            return Task.Run(() => {
                Properties[] locations = { };
                string json;

                if (hasBeenUpdated == false)
                {
                    string[] res = this.GetType().Assembly.GetManifestResourceNames();
                    var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;
                    Stream stream = assembly.GetManifestResourceStream("GeoApp.locations.json");

                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                else
                {
                    json = File.ReadAllText(fileName);
                }

                if (json != null)
                {
                    var rootobject = JsonConvert.DeserializeObject<RootObject>(json);
                    Debug.WriteLine("HELLO:::::::::::::              {0}", rootobject);
                    locations = null;
                //    locations = rootobject.features;
                //    Debug.WriteLine("HELLO:::::::::::::              {0}", locations);
                }
                return null;
                return locations.ToList();
            });
        }

        public Task SaveLocationAsync(Properties location)
        {
            return Task.Run(() =>
            {
                List<Properties> locations = App.LocationManager.CurrentLocations;

            //    if (locations.Id == null)
            //    {
            //        locations.Id = DateTime.Now.GetHashCode();
            //        locations.Add(item);
            //    }

                Properties[] locationsArr = locations.ToArray<Properties>();
                RootObject rootobject = new RootObject();
            //    rootobject.features[].properties = locationsArr;
                var json = JsonConvert.SerializeObject(rootobject);

                File.WriteAllText(fileName, json);
                hasBeenUpdated = true;
            });
        }
    }
}
