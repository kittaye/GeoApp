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
        public string Type { get; set; }
        public Properties Properties { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class Properties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public Dictionary<string, object> MetadataFields { get; set; }
        public string Name { get; set; }
        public string TypeIconPath { get; set; }
        public DateTime Date { get; set; }
    }

    public class Geometry {
        public DataType Type { get; set; }
        public List<object> Coordinates { get; set; }
        public List<Point> XamarinCoordinates { get; set; }
    }

    public class RootObject
    {
        public List<Feature> Features { get; set; }
    }
}
