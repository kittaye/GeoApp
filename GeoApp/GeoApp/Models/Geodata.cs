using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

//Geodata.cs defines the models for geoJSON data to deserialize into.
namespace GeoApp
{
    public enum DataType { Point, LineString, Polygon }; //Change Line to LineString? Also potential for Multipart Gemoetries: MultiPoint, MultiLineString and MultiPolygon.

    public class Feature
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
        public List<Feature> features { get; set; }
    }

    public class ExportModel {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }
}
