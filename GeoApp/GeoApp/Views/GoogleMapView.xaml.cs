using Xamarin.Forms;

namespace GeoApp
{
    public partial class GoogleMapView : ContentPage
    {
        public GoogleMapView()
        {
            InitializeComponent();
            myMap.UiSettings.MyLocationButtonEnabled = true;
            myMap.UiSettings.ZoomControlsEnabled = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
