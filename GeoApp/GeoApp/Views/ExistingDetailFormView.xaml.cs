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
            ((DetailFormViewModel)BindingContext).EntryID = data.properties.id;

            Title = $"View {data.geometry.type}";
            itemName.Text = data.properties.name;
            dateEntry.Text = data.properties.date;

            geolocationListView.ItemsSource = new List<Point>(data.properties.xamarincoordinates);
            listView.ItemsSource = new Dictionary<string, object>(data.properties.metadatafields);
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }
    }
}