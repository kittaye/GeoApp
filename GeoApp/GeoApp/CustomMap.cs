using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace GeoApp
{
    public class CustomMap : Map
    {
        public List<Pin> CustomPins { get; set; }
        public Position Position { get; set; }
    }
}
