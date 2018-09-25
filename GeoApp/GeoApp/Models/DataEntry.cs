using System;
using System.Collections.Generic;
using System.Text;

namespace GeoApp
{
    public enum DataType { Point, Line, Polygon };

    class DataEntry
    {
        public string Name { get; set; }
        public DataType Type { get; set; }

        public DataEntry() {
        }
    }
}
