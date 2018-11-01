using System.Diagnostics;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;
using System;

namespace GeoApp {
    /// <summary>
    /// ViewModel for maps page
    /// </summary>
    public class MapViewModel {
        private CustomMap map;

        public MapViewModel() {
#if __IOS__
            InitialiseMap();
#endif

        }

        /// <summary>
        /// Create map and set starting position
        /// </summary>
        /// <returns></returns>
        public Map InitialiseMap() {
            map = new CustomMap() {
#if __IOS__
                IsShowingUser = true,
#else
#if __ANDROID__
                IsShowingUser = false,
#endif
#endif
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-27.4784849, 153.0262424), Distance.FromMiles(2)));
            return map;
        }

        /// <summary>
        /// Adds overlays to the map, with different methods for points, lines and perimeters. Currently only points activated
        /// </summary>
        public void AddGeometry() {
            List<Feature> features = App.FeaturesManager.CurrentFeatures;
            map.Pins.Clear();

            foreach (var feature in features)
            {
                if(feature.geometry.type == "Point") //Use pins for points
                { 
                    CreatePin(feature.properties.name, feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude, feature.properties.xamarincoordinates[0].Altitude);
                }
                if (feature.geometry.type == "Line") //
                {
                    //List<Position> pos = new List<Position> { new Position(feature.properties.xamarincoordinates[0].Latitude, feature.properties.xamarincoordinates[0].Longitude), new Position(feature.properties.xamarincoordinates[1].Latitude, feature.properties.xamarincoordinates[1].Longitude) };
                    //map.ShapeCoordinates.Add(pos);
                }
                if (feature.geometry.type == "Polygon") //Use overlay shapes for perimeters
            {
                    //List<Position> posi = new List<Position> { new Position(39.934633, 116.399921), new Position(39.929709, 116.400208), new Position(39.929792, 116.405994), new Position(39.934689, 116.405526) };
                    //map.ShapeCoordinates.Add(posi);
                }
            }
        }

        /// <summary>
        /// Call geometry whenever map comes into view, clearing and readding all data points
        /// </summary>
        public void RefreshMap() {
            AddGeometry();
        }

        /// <summary>
        /// Create pins for point representation on the map
        /// </summary>
        /// <param name="name">point name</param>
        /// <param name="lat">point latitude</param>
        /// <param name="alt">point altitude</param>
        /// /// <param name="lon">point longitude</param>
        public void CreatePin(string name, double lat, double lon, double alt)
        {
            var position = new Position(lat, lon); // Latitude, Longitude
            var pin = new Pin {
                Type = PinType.Place,
                Position = position,
                Label = name,
                Address = String.Format("{0}, {1}, {2}", lat, lon, alt)
            };
            map.Pins.Add(pin);
        }

        /// <summary>
        /// Method to return the list of pins on the map
        /// </summary 
        /// <returns>List of Pins</returns>
        public IList<Pin> GetPins()
        {
            return map.Pins;
        }
    }
}
