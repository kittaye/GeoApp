using System;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;

namespace GeoApp
{
    public class MapViewModel
    {
        Map map;
        public MapViewModel()
        {
            InitialiseMap();
        }

        public Map InitialiseMap()
        {
            map = new Map(MapSpan.FromCenterAndRadius(new Position(-27.4784849, 153.0262424), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            return map;

        }



        public void Locc()
        {
            List<Feature> features = new List<Feature>();
            features = App.LocationManager.CurrentLocations;

            foreach (var feature in features)
            {
                foreach (var point in feature.properties.xamarincoordinates)
                {
                    addPin(feature.properties.name, point.Latitude, point.Longitude);

                }
            }
        }



        public void addPin(string name, double lat, double longt)
        {
            var position = new Position(lat, longt); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = name,
                Address = "custom detail info"
            };
            map.Pins.Add(pin);
        }



    }
}
