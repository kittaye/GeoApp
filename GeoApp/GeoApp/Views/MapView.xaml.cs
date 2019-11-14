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
        }


        private void CleanFeaturesOnMap()
        {
            map.MapElements.Clear();
            map.Pins.Clear();
        }


        public void DrawAllGeoDataOnTheMap()
        {
            // Clean all the data on the map first
            //CleanFeaturesOnMap();
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
                        Address = "The city with a boardwalk",
                        Type = PinType.Place,
                        Position = new Position(points[0].Latitude, points[0].Longitude)
                    };
                    map.Pins.Add(pin);
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
                    map.MapElements.Add(polyline);
                }
                else if (feature.geometry.type.Equals("Polygon"))
                {
                    //GoogleMapManager.DrawPolygon(Polygons, feature.properties.name, points);
                    Debug.WriteLine("--------- {0}", feature.properties.name );

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
                    map.MapElements.Add(polygon);
                }

                //}
            });
        }


        void OnButtonClicked(object sender, EventArgs e)
        {
            Pin boardwalkPin = new Pin
            {
                Position = new Position(36.9641949, -122.0177232),
                Label = "Boardwalk",
                Address = "Santa Cruz",
                Type = PinType.Place
            };
            boardwalkPin.MarkerClicked += OnMarkerClickedAsync;

            Pin wharfPin = new Pin
            {
                Position = new Position(36.9571571, -122.0173544),
                Label = "Wharf",
                Address = "Santa Cruz",
                Type = PinType.Place
            };
            wharfPin.InfoWindowClicked += OnInfoWindowClickedAsync;

            map.Pins.Add(boardwalkPin);
            map.Pins.Add(wharfPin);
        }

        async void OnMarkerClickedAsync(object sender, PinClickedEventArgs e)
        {
            e.HideInfoWindow = true;
            string pinName = ((Pin)sender).Label;
            await DisplayAlert("Pin Clicked", $"{pinName} was clicked.", "Ok");
        }

        async void OnInfoWindowClickedAsync(object sender, PinClickedEventArgs e)
        {
            string pinName = ((Pin)sender).Label;
            await DisplayAlert("Info Window Clicked", $"The info window was clicked for {pinName}.", "Ok");
        }

        protected override void OnAppearing()
        {
            CleanFeaturesOnMap();
            DrawAllGeoDataOnTheMap();
        }

        
    }
}