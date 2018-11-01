using GeoApp.Data;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoApp {
    public partial class App : Application {

        public static FeaturesManager FeaturesManager { get; private set; }

        public App() {
            InitializeComponent();
            FeaturesManager = new FeaturesManager(new FileService());
            MainPage = new NavigationPage(HomePage.Instance) { BarBackgroundColor = Color.Default, BarTextColor = Color.Default};
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
