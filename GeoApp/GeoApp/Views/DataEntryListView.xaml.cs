using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataEntryListView : ContentPage {
        private bool isFetchingData;

        public DataEntryListView() {
            InitializeComponent();
            isFetchingData = false;
        }

        protected async override void OnAppearing() {
            base.OnAppearing();

            if (App.LocationManager.CurrentLocations == null && isFetchingData == false) {
                isFetchingData = true;
                loadingList.IsRunning = true;
                loadingList.IsVisible = true;
                App.LocationManager.CurrentLocations = await Task.Run(() => App.LocationManager.GetLocationsAsync());
                loadingList.IsRunning = false;
                loadingList.IsVisible = false;
            }

            listView.ItemsSource = App.LocationManager.CurrentLocations;
        }

        protected override void OnDisappearing() {
            loadingList.IsRunning = false;
            loadingList.IsVisible = false;
            base.OnDisappearing();
        }

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            listView.SelectedItem = null;
            Feature _data = (Feature)e.Item;
            await Navigation.PushAsync(new ExistingDetailFormView(_data));
        }
    }
}