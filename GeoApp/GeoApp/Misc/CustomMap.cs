using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace GeoApp
{
    public class CustomMap : Map
    {
        public List<List<Position>> ShapeCoordinates { get; set; }

        public CustomMap()
        {
            ShapeCoordinates = new List<List<Position>>();
        }
    }
}
