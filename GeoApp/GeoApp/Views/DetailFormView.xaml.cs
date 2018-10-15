using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailFormView : ContentPage
    {
        public DetailFormView(string type)
        {
            InitializeComponent();
            Title = $"New {type}";

            if (type == "Line" || type == "Polygon")
            {
                addPointBtn.Text = $"Add to {type}";
                addPointBtn.IsVisible = true;
            }
            else
            {
                addPointBtn.IsVisible = false;
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }

        async void OnSaveUpdateActivated(object sender, EventArgs e)
        {
            var location = (Feature)BindingContext;

            if (location.Properties.Name == null)
            {
                await DisplayAlert("Alert", "Location name cannot be empty!", "OK");
            }
            else if (location.Properties.Name.Trim() == "")
            {
                await DisplayAlert("Alert", "Location name cannot be empty!", "OK");
            }
            else
            {
                await App.LocationManager.SaveLocationAsync(location);
                await Navigation.PopAsync();

                //async void OnSaveUpdateActivated(object sender, EventArgs e)
                //{
                //    var location = (Properties)BindingContext;

                //    if (location.name == null)
                //    {
                //        await DisplayAlert("Alert", "Location name cannot be empty!", "OK");
                //    }
                //    else if (location.name.Trim() == "")
                //    {
                //        await DisplayAlert("Alert", "Location name cannot be empty!", "OK");
                //    }
                //    else
                //    {
                //        await App.LocationManager.SaveLocationAsync(location);
                //        await Navigation.PopAsync();

                //    }
            }
        }
    }
}