using System.Diagnostics;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;

namespace GeoApp
{
    public class MapViewModel
    {
        Map map;
        public List<Pin> CustomPins { get; set; }
        public MapViewModel()
        {
            InitialiseMap();
        }

        public Map InitialiseMap()
        {
            map = new Map(MapSpan.FromCenterAndRadius(new Position(-27.4784849, 153.0262424), Distance.FromMiles(5)))
            {
                
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            return map;

        }

        internal void RefreshMap()
        {
            map.Pins.Clear();
            AddPins();
            Debug.Write("Here");
        }

        public void AddPins()
        {
            List<Feature> features = new List<Feature>();
            features = App.LocationManager.CurrentLocations;

            foreach (var feature in features)
            {
                foreach (var point in feature.properties.xamarincoordinates)
                {
                    CreatePin(feature.properties.name, point.Latitude, point.Longitude);

                }
            }
        }


        public void CreatePin(string name, double lat, double lon)
        {
            var position = new Position(lat, lon); // Latitude, Longitude
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
