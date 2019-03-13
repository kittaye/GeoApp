using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GeoApp
{
    public class GoogleMapViewModel : ViewModelBase
    {

        // Variables
        public ObservableCollection<Pin> Pins { get; set; }
        public ObservableCollection<Polygon> Polygons { get; set; }
        public ObservableCollection<Polyline> Polylines { get; set; }

        // Related to Shape Filter
        public List<string> Shape_options
        {
            get { return new List<string> { "All", "Point", "Line", "Polygon" }; }

            set
            {
                Shape_options = value;
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

        // Related to Shape Filter
        public List<string> Date_options
        {
            get { return new List<string> { "All", "Today", "Last 7 days", "Last month" }; }

            set
            {
                Date_options = value;
                OnPropertyChanged();
            }
        }

        // Filter Currently selected item
        private string date_filter = "All";

        public string Date_filter
        {
            get { return date_filter; }
            set
            {
                date_filter = value;
                OnPropertyChanged();
            }
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

        public ICommand RefreashGeoDataCommand { set; get; }
        public ICommand LocationBtnClickedCommand { set; get; }

        public GoogleMapViewModel()
        {
            RefreashGeoDataCommand = new Command(() =>
            {
                DrawAllGeoDataOnTheMap();
            });

            LocationBtnClickedCommand = new Command(async () => await RedirectMap());
        }

        public void DrawAllGeoDataOnTheMap()
        {
            // Clean all the data on the map first
            CleanFeaturesOnMap();
            // Using CurrentFeature to draw the geodata on the map
            App.FeaturesManager.CurrentFeatures.ForEach((Feature feature) =>
            {
                var points = feature.properties.xamarincoordinates;

                // One day before the feature, so it works for showing all feature
                DateTime beforeDate = DateTime.Parse(feature.properties.date).AddDays(-1);

                if (Date_filter.Equals("Today"))
                    beforeDate = DateTime.Today.AddDays(-1);
                else if (Date_filter.Equals("Last 7 days"))
                    beforeDate = DateTime.Now.AddDays(-7);
                else if (Date_filter.Equals("Last month"))
                    beforeDate = DateTime.Now.AddDays(-30);

                // feature is earily than before date
                if (DateTime.Compare(beforeDate, DateTime.Parse(feature.properties.date)) < 0)
                {
                    if (feature.geometry.type.Equals("Point") && (shape_filter.Equals("Point") || shape_filter.Equals("All")))
                    {
                        GoogleMapManager.DropPins(Pins, feature.properties.name, points);
                    }
                    else if (feature.geometry.type.Equals("Line") && (shape_filter.Equals("Line") || shape_filter.Equals("All")))
                    {
                        GoogleMapManager.DrawLine(Polylines, feature.properties.name, points);
                    }
                    else if (feature.geometry.type.Equals("Polygon") && (shape_filter.Equals("Polygon") || shape_filter.Equals("All")))
                    {
                        GoogleMapManager.DrawPolygon(Polygons, feature.properties.name, points);
                    }

                }
            });
        }

        // use it when you need to implement any function need to click the map

        public Command<MapClickedEventArgs> MapClickedCommand = new Command<MapClickedEventArgs>(async (args) =>
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
                //throw fnsEx;
            }
            catch (PermissionException pEx)
            {
                //throw pEx;
            }
            catch (Exception ex)
            {
                //throw ex;
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
