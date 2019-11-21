using Newtonsoft.Json;
using System.Collections.Generic;

//Geodata.cs defines the models for geoJSON data to serialize into and from.
namespace GeoApp
{
    public class Feature
    {
        public string Type { get; set; }
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }

    public class Properties
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string MetadataStringValue { get; set; }
        public int MetadataIntegerValue { get; set; }
        public float MetadataFloatValue { get; set; }
        [JsonIgnore]
        public List<Point> Xamarincoordinates { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string TypeIconPath { get; set; }
        public string Date { get; set; }
    }

    public class Geometry {
        public string Type { get; set; }
        public List<object> Coordinates { get; set; }
    }

    public class RootObject
    {
        public string Type { get; set; }
        public List<Feature> Features { get; set; }
    }
}
