using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

//Geodata.cs defines the models for geoJSON data to serialize into and from.
namespace GeoApp
{
    public class Feature
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public int id { get; set; }
        public Dictionary<string, object> metadatafields { get; set; }
        [JsonIgnore]
        public List<Point> xamarincoordinates { get; set; }
        public string name { get; set; }
        [JsonIgnore]
        public string typeIconPath { get; set; }
        public string date { get; set; }
    }

    public class Geometry {
        public string type { get; set; }
        public List<object> coordinates { get; set; }
    }

    public class RootObject
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }
}
