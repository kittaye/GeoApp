using GeoApp.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoApp {
    public partial class App : Application {

        public static FeaturesManager FeaturesManager { get; private set; }

        public App() {
            InitializeComponent();
            FeaturesManager = new FeaturesManager(new FileService());
            MainPage = new NavigationPage(HomePage.Instance) { BarBackgroundColor = Color.Default, BarTextColor = Color.Default };

            // Uncomment this to clear your set user ID.
            //Application.Current.Properties.Clear();

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
