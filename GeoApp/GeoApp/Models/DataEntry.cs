using System;
using System.Collections.Generic;
using System.Text;

namespace GeoApp
{
    public enum DataType { Point, Line, Polygon };

    public class Properties
    {
        public int id { get; set; }
        public string name { get; set; }
        public string typeIcon { get; set; }
    }

    public class Geometry
    {
        public DataType type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class RootObject
    {
        public string type { get; set; }
        public Feature[] features { get; set; }

    }
}