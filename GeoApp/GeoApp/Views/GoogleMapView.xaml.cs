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

            // If refreash GeoDataCommand is available to Execute
            if ( viewModel.RefreashGeoDataCommand.CanExecute(null))
            {
                // Refreash GeoData
                viewModel.RefreashGeoDataCommand.Execute(null);
                viewModel.LocationBtnClickedCommand.Execute(null);
            }
        }
    }
}
