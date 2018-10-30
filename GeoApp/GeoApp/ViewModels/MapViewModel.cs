using System.Diagnostics;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;
using System;

namespace GeoApp
{
    public class MapViewModel
    {
        private CustomMap map;
        public List<Pin> CustomPins { get; set; }

        public MapViewModel()
        {
            InitialiseMap();
        }

        public Map InitialiseMap()
        {
            map = new CustomMap()
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-27.4784849, 153.0262424), Distance.FromMiles(2)));
            return map;
        }

        public void AddGeometry()
        {
            List<Feature> features = App.LocationManager.CurrentLocations;
            map.Pins.Clear();

            //List<Position> posi = new List<Position> {
            //            new Position(39.934633, 116.399921),
            //            new Position(39.929709, 116.400208),
            //            new Position(39.929792, 116.405994),
            //            new Position(39.934689, 116.405526)};
            //        map.ShapeCoordinates.Add(posi);
            //List<Position> posil = new List<Position> {
            //            new Position(49.934633, 116.399921),
            //            new Position(49.929709, 116.400208),
            //            new Position(49.929792, 116.405994)};
            //    map.ShapeCoordinates.Add(posi);
            //    map.ShapeCoordinates.Add(posil);
            //List<Position> postest = new List<Position> {
            //    new Position(features[0].properties.xamarincoordinates[0].Latitude, features[0].properties.xamarincoordinates[0].Longitude),
            //    new Position(features[0].properties.xamarincoordinates[1].Latitude, features[0].properties.xamarincoordinates[1].Longitude),
            //    new Position(features[0].properties.xamarincoordinates[2].Latitude, features[0].properties.xamarincoordinates[2].Longitude),
            //    new Position(features[0].properties.xamarincoordinates[3].Latitude, features[0].properties.xamarincoordinates[3].Longitude),
            //    new Position(features[0].properties.xamarincoordinates[4].Latitude, features[0].properties.xamarincoordinates[4].Longitude)};
            //map.ShapeCoordinates.Add(postest);

            foreach (var feature in features)
            {
                if(feature.geometry.type == "Point"){
                    CreatePin(feature.properties.name, feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude);
                }
                if (feature.geometry.type == "Line")
                {
                    //List<Position> pos = new List<Position> { new Position(feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude), new Position(feature.properties.xamarincoordinates[1].Latitude, feature.properties.xamarincoordinates[1].Longitude) };
                    //map.ShapeCoordinates.Add(pos);
                }
                    if (feature.geometry.type == "Polygon")
                {
                    List<Position> posi = new List<Position> { new Position(39.934633, 116.399921), new Position(39.929709, 116.400208), new Position(39.929792, 116.405994), new Position(39.934689, 116.405526) };
                    map.ShapeCoordinates.Add(posi);
                }
            }
        }

        public void RefreshMap()
        {
            Debug.Write("Refreshing\n");
            AddGeometry();
        }

        public void CreatePin(string name, double lat, double lon)
        {
            var position = new Position(lat, lon); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = name,
                Address = String.Format("{0}, {1}", lat, lon)
            };
            map.Pins.Add(pin);
        }
    }
}
