using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Essentials;

using Debug = System.Diagnostics.Debug;
using Plugin.Permissions;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;

namespace GeoApp.Droid {
    [Activity(Label = "GeoAware", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
        protected override void OnCreate(Bundle bundle) {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle); // initialise xamarin essentials
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle);
            Xamarin.FormsGoogleMapsBindings.Init();
            Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) => {
                if (!string.IsNullOrWhiteSpace(e.View.AutomationId)) {
                    e.NativeView.ContentDescription = e.View.AutomationId;
                }
            };

            LoadApplication(new App());
        }

        protected override void OnResume() {
            base.OnResume();

            Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
                SetSupportActionBar(toolbar);
        }

        /// <summary>
        /// Handle runtime permissions
        /// https://docs.microsoft.com/en-us/xamarin/essentials/get-started?context=xamarin%2Fxamarin-forms&tabs=windows%2Candroid
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            // Check if the selected toolbar button's id equals the back button id.
            if (item.ItemId == Android.Resource.Id.Home) {
                // If so, override it so it always takes the user straight back to the main page.
                HomePage.Instance.Navigation.PopToRootAsync();
                return false;
            } else {
                return base.OnOptionsItemSelected(item);
            }
        }
    }
}

