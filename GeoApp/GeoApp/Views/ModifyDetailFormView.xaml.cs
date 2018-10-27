using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModifyDetailFormView : ContentPage {

        /// <summary>
        /// Detail form constructor for when a new entry is being added.
        /// </summary>
        /// <param name="type">The geoJSON geometry type being added.</param>
        public ModifyDetailFormView(string type) {
            InitializeComponent();
            ((DetailFormViewModel)BindingContext).EntryType = type;
            ((DetailFormViewModel)BindingContext).GeolocationPoints.Add(new Point(0, 0, 0));

            Title = $"New {type}";

            DetermineAddPointBtnVisability(type);
        }

        /// <summary>
        /// Detail form constructor for when an existing entry is being edited.
        /// </summary>
        /// <param name="data">The entry's data as represented by a feature object.</param>
        public ModifyDetailFormView(Feature data) {
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

            DetermineAddPointBtnVisability(data.geometry.type);
        }

        /// <summary>
        /// Determines whether or not the "add to {type}" button is visible.
        /// </summary>
        /// <param name="type">The type of the entry.</param>
        private void DetermineAddPointBtnVisability(string type) {
            if (type == "Line" || type == "Polygon") {
                addPointBtn.Text = $"Add to {type}";
                addPointBtn.IsVisible = true;
            } else {
                addPointBtn.IsVisible = false;
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }
    }
}
