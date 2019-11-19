using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapView : ContentPage
    {
        public MapView()
        {
            InitializeComponent();
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-27.47004901089882, 153.021072), Distance.FromMiles(1.0)));

            customMap.Position = new Position(-27.47004901089882, 153.021072);

            Pin pin = new Pin
            {
                Type = PinType.Place,
                Position = new Position(37.79752, -122.40183),
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA",
            };

            customMap.CustomPins = new List<Pin> { pin };
        }

        private void CleanFeaturesOnMap()
        {
            customMap.MapElements.Clear();
            customMap.Pins.Clear();
            customMap.CustomPins.Clear();
        }

        public void DrawAllGeoDataOnTheMap()
        {
            // Using CurrentFeature to draw the geodata on the map
            App.FeatureStore.CurrentFeatures.ForEach((Feature feature) =>
            {
                var points = feature.properties.xamarincoordinates;

                // One day before the feature, so it works for showing all feature
                DateTime beforeDate = DateTime.Parse(feature.properties.date).AddDays(-1);

                //if (Date_filter.Equals("Today"))
                //    beforeDate = DateTime.Today.AddDays(-1);
                //else if (Date_filter.Equals("Last 7 days"))
                //    beforeDate = DateTime.Now.AddDays(-7);
                //else if (Date_filter.Equals("Last month"))
                //    beforeDate = DateTime.Now.AddDays(-30);

                // feature is earily than before date
                //if (DateTime.Compare(beforeDate, DateTime.Parse(feature.properties.date)) < 0)
                //{
                if (feature.geometry.type.Equals("Point"))
                {
                    Pin pin = new Pin
                    {
                        Label = feature.properties.name,
                        Address = string.Format("{0}, {1}", points[0].Latitude, points[0].Longitude),
                        Type = PinType.Place,
                        Position = new Position(points[0].Latitude, points[0].Longitude)
                    };
                    customMap.Pins.Add(pin);
                    customMap.CustomPins.Add(pin);
                }
                else if (feature.geometry.type.Equals("Line"))
                {
                    Polyline polyline = new Polyline
                    {
                        StrokeColor = Color.Blue,
                        StrokeWidth = 12,
                    };
                    points.ForEach((Point point) =>
                    {
                        polyline.Geopath.Add(new Position(point.Latitude, point.Longitude));
                    });
                    customMap.MapElements.Add(polyline);
                    

                    Pin pin = new Pin
                    {
                        Label = feature.properties.name,
                        Address = string.Format("{0}, {1}", points[0].Latitude, points[0].Longitude),
                        Type = PinType.Place,
                        Position = new Position(points[0].Latitude, points[0].Longitude)
                    };
                    //customMap.Pins.Add(pin);
                }
                else if (feature.geometry.type.Equals("Polygon"))
                {
                    Polygon polygon = new Polygon
                    {
                        StrokeWidth = 8,
                        StrokeColor = Color.FromHex("#1BA1E2"),
                        FillColor = Color.FromHex("#881BA1E2"),
                    };
                    points.ForEach((Point point) =>
                    {
                        polygon.Geopath.Add(new Position(point.Latitude, point.Longitude));
                    });
                    customMap.MapElements.Add(polygon);
                    Pin pin = new Pin
                    {
                        Label = feature.properties.name,
                        Address = string.Format("{0}, {1}", points[0].Latitude, points[0].Longitude),
                        Type = PinType.Place,
                        Position = new Position(points[0].Latitude, points[0].Longitude)
                    };
                    //customMap.Pins.Add(pin);

                }
                //}
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CleanFeaturesOnMap();
            DrawAllGeoDataOnTheMap();

        }
    }
}