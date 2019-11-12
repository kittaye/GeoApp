using GeoApp.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoApp {
    public partial class App : Application {

        public static FeatureStore FeatureStore { get; private set; }
        public static string AppTheme { get; set; }

        public App() {
            InitializeComponent();
            FeatureStore = new FeatureStore();
            MainPage = new NavigationPage(HomePage.Instance);

            // If the user ID hasn't been set yet, prompt the user to create one upon app launch.
            if (Application.Current.Properties.ContainsKey("UserID") == false) {
                MainPage.Navigation.PushModalAsync(new IDFormView());
            }

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
