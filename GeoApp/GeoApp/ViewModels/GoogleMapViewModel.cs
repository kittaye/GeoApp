using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GeoApp
{
    public class GoogleMapViewModel: ViewModelBase
    {
        public ObservableCollection<Pin> Pins { get; set; }
        public ObservableCollection<Polygon> Polygons { get; set; }
        public ObservableCollection<Polyline> Polylines { get; set; }

        public GoogleMapViewModel()
        {
            DrawAllGeoDataOnTheMap();
        }

        // Initialize the map position to Brisbane City at the beginning
        private MapSpan _region = MapSpan.FromCenterAndRadius(
                new Position(-27.47004901089882, 153.021072),
                Distance.FromKilometers(2)
            );

        public MapSpan Region
        {
            get => _region;
            set
            {
                _region = value;
                OnPropertyChanged();
            }
        }

        public Command<MyLocationButtonClickedEventArgs> LocationBtnClickedCommand =>
            new Command<MyLocationButtonClickedEventArgs>( args =>
            {
                GoogleMapManager.UpdateRegionToUserLocation(Region);
                DrawAllGeoDataOnTheMap();
            });
            
        public void DrawAllGeoDataOnTheMap() 
        {
            // Using CurrentFeature to draw the geodata on the map
            App.FeaturesManager.CurrentFeatures.ForEach((Feature feature) =>
            {
                var points = feature.properties.xamarincoordinates;
                switch (feature.geometry.type)
                {
                    case "Point":
                        GoogleMapManager.DropPins(Pins, feature.properties.name, points);
                        break;
                    case "Line":
                        GoogleMapManager.DrawLine(Polylines, points);
                        break;
                    case "Polygon":
                        GoogleMapManager.DrawPolygon(Polygons, points);
                        break;
                }
            });
        }

        // use it when you need to implement any function need to click the map

        public Command<MapClickedEventArgs> MapClickedCommand =>
        new Command<MapClickedEventArgs>(args =>
        {
            Application.Current.MainPage.DisplayAlert("Coordinate", $" Latitude {args.Point.Latitude} Longtitude {args.Point.Longitude}", "Okay");
        });
    }
}
