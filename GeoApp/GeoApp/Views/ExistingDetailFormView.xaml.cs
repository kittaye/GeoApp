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
            BindingContext = new DetailFormViewModel(data);

            Title = $"View {data.geometry.type}";
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }
    }
}