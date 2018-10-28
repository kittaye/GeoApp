using System.Diagnostics;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;
using System;

namespace GeoApp
{
    public class MapViewModel
    {
        CustomMap map;
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
            AddGeometry();


            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-27.4784849, 153.0262424), Distance.FromMiles(0.5)));

            return map;

        }

        public void AddGeometry()
        {

            List<Feature> features = new List<Feature>();
            features = App.LocationManager.CurrentLocations;
            Debug.Write("ENTRY TIMME!!!!!!!!");
            map.Pins.Clear();
            foreach (var feature in features)
            {
                Debug.Write("I am Feature");
                if(feature.geometry.type == "Point"){
                    CreatePin(feature.properties.name, feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude);
                }
                if (feature.geometry.type == "Line")
                {
                    List<Position> pos = new List<Position> { new Position(feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude), new Position(feature.properties.xamarincoordinates[1].Latitude, feature.properties.xamarincoordinates[1].Longitude) };
                    map.ShapeCoordinates.Add(pos);
                }
                    if (feature.geometry.type == "Polygon")
                {
                    //List<Position> posi = new List<Position> {
                    //    new Position(feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude),
                    //    new Position(feature.properties.xamarincoordinates[1].Latitude, feature.properties.xamarincoordinates[1].Longitude),
                    //    new Position(feature.properties.xamarincoordinates[2].Latitude, feature.properties.xamarincoordinates[2].Longitude),
                    //    new Position(feature.properties.xamarincoordinates[3].Latitude, feature.properties.xamarincoordinates[3].Longitude),
                    //    new Position(feature.properties.xamarincoordinates[4].Latitude, feature.properties.xamarincoordinates[4].Longitude), };
                    List<Position> posi = new List<Position> { new Position(39.934633, 116.399921), new Position(39.929709, 116.400208), new Position(39.929792, 116.405994), new Position(39.934689, 116.405526) };
                    map.ShapeCoordinates.Add(posi);
                    Debug.Write("I am Polygon {feature.properties.name}");
                }


            }


            //List<Position> pos = new List<Position> { new Position(39.939889, 116.423493), new Position(39.930622, 116.423924), new Position(39.930733, 116.441135), new Position(39.939944, 116.44056) };
            //List<Position> posi = new List<Position> { new Position(39.934633, 116.399921), new Position(39.929709, 116.400208), new Position(39.929792, 116.405994), new Position(39.934689, 116.405526) };


        }

        internal void RefreshMap()
        {
            Debug.Write("Here");
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
