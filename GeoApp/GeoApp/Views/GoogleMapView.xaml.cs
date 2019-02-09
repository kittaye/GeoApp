using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp {
    public partial class GoogleMapView : ContentPage {

        public GoogleMapView() {
            Task.Run(async () => { await InitGoogleMaps(); });
        }

        /// <summary>
        /// Initialises Maps
        /// </summary>
        /// <returns></returns>
        private async Task InitGoogleMaps() {
            try {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (status != PermissionStatus.Granted) {
                    // If the user accepts the permission get the resulting value and check the if the key exists
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    if (results.ContainsKey(Permission.Location)) {
                        status = results[Permission.Location];
                    }
                }
                // Make sure maps is initalised after permission is granted and invoked on the mainthread
                if (status == PermissionStatus.Granted) {
                    Device.BeginInvokeOnMainThread(() => {
                        InitializeComponent();
                        myMap.UiSettings.MyLocationButtonEnabled = true;
                        myMap.UiSettings.ZoomControlsEnabled = false;
                    });
                } else {
                    // I don't think this is ideal.
                    await InitGoogleMaps();
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        protected override void OnAppearing() {
            base.OnAppearing();

            if (viewModel.RefreashGeoDataCommand.CanExecute(null)) {
                viewModel.RefreashGeoDataCommand.Execute(null);
                viewModel.LocationBtnClickedCommand.Execute(null);
            }
        }
    }
}
