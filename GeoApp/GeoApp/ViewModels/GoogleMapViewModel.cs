using System;
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
            RefreashGeoDataCommand = new Command( () => {
                DrawAllGeoDataOnTheMap();
            });

            LocationBtnClickedCommand = new Command(async () => await RedirectMap() );
        }

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

    }
}
