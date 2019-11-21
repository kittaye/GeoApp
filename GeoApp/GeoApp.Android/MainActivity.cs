using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Xamarin.Essentials;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace GeoApp.Droid
{
    [Activity(Label = "Groundsman",
        //Icon = "@mipmap/ic_launcher",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Platform.Init(this, savedInstanceState); // initialise xamarin essentials
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.View.AutomationId))
                {
                    e.NativeView.ContentDescription = e.View.AutomationId;
                }
            };

            LoadApplication(new App());
        }

        protected override void OnResume()
        {
            base.OnResume();

            //Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //if (toolbar != null)
            //    SetSupportActionBar(toolbar);
        }

        /// <summary>
        /// Handle runtime permissions
        /// https://docs.microsoft.com/en-us/xamarin/essentials/get-started?context=xamarin%2Fxamarin-forms&tabs=windows%2Candroid
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode,
   string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult
                  (requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Check if the selected toolbar button's id equals the back button id.
            if (item.ItemId == Android.Resource.Id.Home)
            {
                // If so, override it so it always takes the user straight back to the main page.
                HomePage.Instance.Navigation.PopToRootAsync();
                return false;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}

