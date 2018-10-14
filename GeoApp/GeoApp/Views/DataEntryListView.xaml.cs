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
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if(App.LocationManager.CurrentLocations == null) {
                App.LocationManager.CurrentLocations = await App.LocationManager.GetLocationsAsync();
            }

            listView.ItemsSource = App.LocationManager.CurrentLocations;
        }
    }
}