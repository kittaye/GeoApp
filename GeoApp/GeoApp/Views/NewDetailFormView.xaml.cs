using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewDetailFormView : ContentPage
    {
        public NewDetailFormView(string type)
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
            Debug.WriteLine("HELLO:::::::::::::              {0},{1}", BindingContext, "i");
            
            var feature = (Feature)BindingContext;

            if (feature.Properties.Name == null)
            {
                await DisplayAlert("Alert", "Location name cannot be empty!", "OK");
            }
            else if (feature.Properties.Name.Trim() == "")
            {
                await DisplayAlert("Alert", "Location name cannot be empty!", "OK");
            }
            else
            {
                await App.LocationManager.SaveLocationAsync(feature);
                await Navigation.PopAsync();
            }
        }
    }
}
