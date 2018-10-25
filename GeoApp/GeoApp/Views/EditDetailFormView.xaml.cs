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
            ((DetailFormViewModel)BindingContext).EntryType = data.Geometry.Type.ToString();
            ((DetailFormViewModel)BindingContext).EntryID = data.Properties.Id;

            Title = $"Editing {data.Properties.Name}";

            nameEntry.Text = data.Properties.Name;
            dateEntry.Date = data.Properties.Date;

            // fill in geo-location data
            geolocationListView.ItemsSource = data.Geometry.XamarinCoordinates;
            // assign metadatefileds as itemsource
            listView.ItemsSource = data.Properties.MetadataFields;

            if (data.Geometry.Type.ToString() == "LineString" || data.Geometry.Type.ToString() == "Polygon") {
                addPointBtn.Text = $"Add to {data.Geometry.Type.ToString()}";
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
