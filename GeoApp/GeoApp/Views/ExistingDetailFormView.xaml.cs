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
    public partial class ExistingDetailFormView : ContentPage {
        public ExistingDetailFormView(Feature data) {
            InitializeComponent();
            Title = "test";

            // auto fill fields...
            foreach (var item in data.Geometry.Coordinates) {
                Debug.WriteLine(item);
            }
 

        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }
    }
}