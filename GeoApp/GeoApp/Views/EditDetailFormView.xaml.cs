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
            ((DetailFormViewModel)BindingContext).EntryType = data.geometry.type.ToString();
            ((DetailFormViewModel)BindingContext).EntryID = data.properties.id;

            foreach (var item in data.properties.xamarincoordinates) {
                ((DetailFormViewModel)BindingContext).GeolocationPoints.Add(item);
            }

            foreach (var item in data.properties.metadatafields) {
                ((DetailFormViewModel)BindingContext).MetadataEntries.Add(new MetadataEntry(item.Key, item.Value?.ToString(), Keyboard.Default));
            }

            Title = $"Editing {data.properties.name}";

            nameEntry.Text = data.properties.name;
            dateEntry.Date = DateTime.Parse(data.properties.date);

            if (data.geometry.type == "Line" || data.geometry.type == "Polygon") {
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
