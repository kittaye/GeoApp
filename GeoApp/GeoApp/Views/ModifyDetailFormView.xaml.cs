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
            this.BindingContext = new DetailFormViewModel(type);

            Title = $"New {type}";

            DetermineAddPointBtnVisability(type);
        }

        /// <summary>
        /// Detail form constructor for when an existing entry is being edited.
        /// </summary>
        /// <param name="data">The entry's data as represented by a feature object.</param>
        public ModifyDetailFormView(Feature data) {
            InitializeComponent();
            this.BindingContext = new DetailFormViewModel(data);

            Title = $"Editing {data.properties.name}";

            DetermineAddPointBtnVisability(data.geometry.type);
        }

        /// <summary>
        /// Determines whether or not the "add to {type}" button is visible.
        /// </summary>
        /// <param name="type">The type of the entry.</param>
        private void DetermineAddPointBtnVisability(string type) {
            if (type == "Line" || type == "Polygon") {
                addPointBtn.Image = "add_icon_blue";
                addPointBtn.IsVisible = true;
            } else {
                addPointBtn.IsVisible = false;
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            geolocationListView.SelectedItem = null;
        }

        // Android button spam fix: force all opened pages to go back to main page.
        protected override bool OnBackButtonPressed() {
            HomePage.Instance.Navigation.PopToRootAsync();
            return true;
        }

    }
}
