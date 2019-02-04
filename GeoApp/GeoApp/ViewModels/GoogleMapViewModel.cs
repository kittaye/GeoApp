﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GeoApp
{
    public class GoogleMapViewModel: ViewModelBase
    {

        public ObservableCollection<Pin> Pins { get; set; }
        public ObservableCollection<Polygon> Polygons { get; set; }
        public ObservableCollection<Polyline> Polylines { get; set; }

        // Shape Options for the Shape Picker
        private List<string> shape_options = new List<string>
        {
            "All", "Point","Line","Polygon"
        };

        public List<string> Shape_options
        {
            get { return shape_options; }
            set
            {
                shape_options = value;
                OnPropertyChanged();
            }
        }

        // Filter Currently selected item
        private string shape_filter = "All";

        public string Shape_filter
        {
            get { return shape_filter; }
            set
            {
                shape_filter = value;
                OnPropertyChanged();
            }
        }

        // Initialize the map position to Brisbane City at the beginning
        private MapSpan _region = MapSpan.FromCenterAndRadius(
                new Position(-27.47004901089882, 153.021072),
                Distance.FromKilometers(2)
            );

        // Variable for changing the map position
        public MapSpan Region
        {
            get => _region;
            set
            {
                _region = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreashGeoDataCommand { set; get; }
        public ICommand LocationBtnClickedCommand { set; get; }

        public GoogleMapViewModel()
        {
            RefreashGeoDataCommand = new Command( () => DrawGeoDataOnTheMap());

            LocationBtnClickedCommand = new Command(async () => await RedirectMap() );

        }

        public void DrawGeoDataOnTheMap() 
        {
            // Clean all the data on the map first
            CleanFeaturesOnMap();
            // Using CurrentFeature to draw the geodata on the map
            App.FeaturesManager.CurrentFeatures.ForEach((Feature feature) =>
            {
                var points = feature.properties.xamarincoordinates;

                if ( feature.geometry.type.Equals("Point") && (shape_filter.Equals("Point") || shape_filter.Equals("All") ) )
                {
                    GoogleMapManager.DropPins(Pins, feature.properties.name, points);
                }
                else if (feature.geometry.type.Equals("Line") && (shape_filter.Equals("Line") || shape_filter.Equals("All")) )
                {
                    GoogleMapManager.DrawLine(Polylines, points);
                }
                else if (feature.geometry.type.Equals("Polygon") && (shape_filter.Equals("Polygon") || shape_filter.Equals("All")))
                {
                    GoogleMapManager.DrawPolygon(Polygons, points);
                }
            });
        }

        // use it when you need to implement any function need to click the map
        public Command<MapClickedEventArgs> MapClickedCommand = new Command<MapClickedEventArgs>( async (args) =>
        {
            await Application.Current.MainPage.DisplayAlert("Coordinate", $" Latitude {args.Point.Latitude} Longtitude {args.Point.Longitude}", "Okay");
        });

        public async Task RedirectMap()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Region = MapSpan.FromCenterAndRadius(
                        new Position(location.Latitude, location.Longitude),
                        Distance.FromKilometers(2)
                    );
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                throw fnsEx;
            }
            catch (PermissionException pEx)
            {
                throw pEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CleanFeaturesOnMap() 
        {
            Pins.Clear();
            Polygons.Clear();
            Polylines.Clear();
        }

    }
}
