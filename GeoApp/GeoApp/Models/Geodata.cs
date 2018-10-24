using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GeoApp
{
    public enum DataType { Point, Line, Polygon }; //Change Line to LineString? Also potential for Multipart Gemoetries: MultiPoint, MultiLineString and MultiPolygon.

    public class Feature
    {
        public DataType Type { get; set; }
        public Properties Properties { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class Properties
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
       // public string TypeIcon { get; set; }
    }

    public class Geometry
    {
        public DataType Type { get; set; }
        public List<double> Coordinates { get; set; } //Longtitute, Latitude, Elevation
    }



    public class RootObject
    {
        public Feature[] Features { get; set; }
    }
}