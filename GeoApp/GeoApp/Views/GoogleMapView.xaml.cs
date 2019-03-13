using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp
{
    public partial class GoogleMapView : ContentPage
    {

        private bool locationPermissionEnabled;

        public GoogleMapView()
        {
            locationPermissionEnabled = false;
            InitGoogleMaps();
        }

        private async Task GetPermission()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Need location", "Gunna need that location", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                status = results[Permission.Location];
            }
            if (status == PermissionStatus.Granted)
            {
                locationPermissionEnabled = true;
            }
            else if (status != PermissionStatus.Unknown)
            {
                locationPermissionEnabled = false;
            }
        }



        /// <summary>
        /// Initialises Maps
        /// </summary>
        /// <returns></returns>
        private void InitGoogleMaps()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                InitializeComponent();
                myMap.UiSettings.MyLocationButtonEnabled = false;
            });
        }

        protected override async void OnAppearing()
        {

            base.OnAppearing();

            // Maps must be initialised
            //if (locationPermissionEnabled == false)
            //{
            await GetPermission();
            //}

            if (locationPermissionEnabled == true)
            {
                myMap.UiSettings.MyLocationButtonEnabled = true;
                // Do a full re-read of the embedded file to get the most current list of features.
                App.FeaturesManager.CurrentFeatures = await Task.Run(() => App.FeaturesManager.GetFeaturesAsync());
                // Redraw maps
                if (viewModel.RefreashGeoDataCommand.CanExecute(null))
                {
                    viewModel.RefreashGeoDataCommand.Execute(null);
                    viewModel.LocationBtnClickedCommand.Execute(null);
                }
            }
            else
            {
                myMap.UiSettings.MyLocationButtonEnabled = false;
                await HomePage.Instance.DisplayAlert("Location Permissions", "Location permission are required to utilise the map feature. Enable location permissions for Groundsman in your device settings to continue.", "Ok");
            }
        }
    }
}

