using GeoApp.Data;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoApp {
    public partial class App : Application {

        public static FeaturesManager FeaturesManager { get; private set; }

        public App() {
            // Checks for location permission on start of application
            Task.Run(async () => { await CheckLocationPermission(); });

            InitializeComponent();

            // Uncomment this to clear your set user ID.
            //Application.Current.Properties.Clear();

            // If the user ID hasn't been set yet, prompt the user to create one upon app launch.
            if (Application.Current.Properties.ContainsKey("UserID") == false) {
                LoadUserIdInputPage();
            } else {
                LoadMainPage();
            }
        }

        private async Task CheckLocationPermission() {
            try {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (status != PermissionStatus.Granted) {
                    // If the user accepts the permission get the resulting value and check the if the key exists
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    if (results.ContainsKey(Permission.Location)) {
                        status = results[Permission.Location];
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        private void LoadUserIdInputPage() {
            LoadMainPage();
            MainPage.Navigation.PushModalAsync(new IDFormView());
        }

        private void LoadMainPage() {
            FeaturesManager = new FeaturesManager(new FileService());
            MainPage = new NavigationPage(HomePage.Instance) { BarBackgroundColor = Color.Default, BarTextColor = Color.Default };
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
