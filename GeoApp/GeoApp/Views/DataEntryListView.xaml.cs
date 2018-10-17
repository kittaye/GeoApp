using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DataEntryListView : ContentPage
	{
		public DataEntryListView()
		{
			InitializeComponent ();
            loadingList.IsVisible = false;
            loadingList.Color = Color.White;
            loadingList.Margin = new Thickness(0, 10, 0, 0);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if(App.LocationManager.CurrentLocations == null) {
                loadingList.IsRunning = true;
                loadingList.IsVisible = true;
                App.LocationManager.CurrentLocations = await App.LocationManager.GetLocationsAsync();
                loadingList.IsRunning = false;
                loadingList.IsVisible = false;
            }

            listView.ItemsSource = App.LocationManager.CurrentLocations;
        }
    }
}