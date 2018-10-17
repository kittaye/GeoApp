using GeoApp.Data;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoApp {
    public partial class App : Application {

        public static LocationItemManager LocationManager { get; private set; }

        public App() {
            InitializeComponent();
            if (Device.Android == Device.RuntimePlatform){
                MainPage = GeoApp.MainPage.Instance;
            }
            else{
                MainPage = new GeoApp.HomePage ();
            }

            LocationManager = new LocationItemManager(new FileService());
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
