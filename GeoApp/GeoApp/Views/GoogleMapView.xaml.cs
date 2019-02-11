﻿using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp {
    public partial class GoogleMapView : ContentPage {

        public GoogleMapView()
        {
            InitializeComponent();
            myMap.UiSettings.MyLocationButtonEnabled = true;
            myMap.UiSettings.ZoomControlsEnabled = false;
        }

        /// <summary>
        /// Initialises Maps
        /// </summary>
        /// <returns></returns>
        private async Task InitGoogleMaps() {
            try {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

            if ( viewModel.RefreashGeoDataCommand.CanExecute(null))
            {
                viewModel.RefreashGeoDataCommand.Execute(null);
                viewModel.LocationBtnClickedCommand.Execute(null);
            }
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            // Maps must be initialised 
            if (locationPermissionEnabled == true) {
                if (viewModel.RefreashGeoDataCommand.CanExecute(null)) {
                    viewModel.RefreashGeoDataCommand.Execute(null);
                    viewModel.LocationBtnClickedCommand.Execute(null);
                }
            } else {
                HomePage.Instance.DisplayAlert("Permission", "Location permission must be enabled to utilise the map feature", "Ok");
            }
        }
    }
}

        private bool locationPermissionEnabled;

        public GoogleMapView() {
            locationPermissionEnabled = false;
            Task.Run(async () => { await InitGoogleMaps(); });
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
                        locationPermissionEnabled = true;
                    });
                } else {
                    Device.BeginInvokeOnMainThread(() => {
                        HomePage.Instance.DisplayAlert("Permission", "Location permission must be enabled to utilise some features in the applcation", "Ok");
                        locationPermissionEnabled = false;
                    });
                }
            } catch (Exception ex) {
                throw ex;