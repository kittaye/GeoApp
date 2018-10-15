using System;
using System.Collections.Generic;
using System.Text;

namespace GeoApp
{
    public enum DataType { Point, Line, Polygon };

    public class Properties
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeIcon { get; set; }
    }

    public class Point {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public Point(double lat, double lng, double alt) {
            this.Latitude = lat;
            this.Longitude = lng;
            this.Altitude = alt;
        }
    }

    public class Geometry
    {
        public DataType Type { get; set; }
        public List<double> Coordinates { get; set; }
    }

    public class Feature
    {
        public string Type { get; set; }
        public Properties Properties { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class RootObject
    {
        public string Type { get; set; }
        public Feature[] Features { get; set; }
    }
}