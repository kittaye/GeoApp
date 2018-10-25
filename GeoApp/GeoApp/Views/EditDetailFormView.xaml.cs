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
    public partial class EditDetailFormView : ContentPage
    {
        public EditDetailFormView(Feature data) {
            InitializeComponent();
            ((EditDetailFormViewModel)BindingContext).EntryType = data.Geometry.Type.ToString();
            ((EditDetailFormViewModel)BindingContext).EntryID = data.Properties.Id;

            foreach (var item in data.Properties.XamarinCoordinates) {
                ((EditDetailFormViewModel)BindingContext).GeolocationPoints.Add(item);
            }

            foreach (var item in data.Properties.MetadataFields) {
                ((EditDetailFormViewModel)BindingContext).MetadataEntries.Add(new MetadataEntry(item.Key, item.Value.ToString(), Keyboard.Default));
            }

            Title = $"Editing {data.Properties.Name}";

            nameEntry.Text = data.properties.name;
            dateEntry.Date = DateTime.Parse(data.properties.date);

            // fill in geo-location data
            //geolocationListView.ItemsSource = data.Properties.XamarinCoordinates;
            // assign metadatefileds as itemsource
            //listView.ItemsSource = data.Properties.MetadataFields;

            if (data.geometry.type.ToString() == "LineString" || data.geometry.type.ToString() == "Polygon") {
                addPointBtn.Text = $"Add to {data.geometry.type.ToString()}";
                addPointBtn.IsVisible = true;
            } else {
                addPointBtn.IsVisible = false;
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }
    }
}
