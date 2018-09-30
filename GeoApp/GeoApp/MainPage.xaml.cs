using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp {
    public partial class MainPage : MasterDetailPage {
        public MainPage() {
            InitializeComponent();
            masterView.listView.ItemSelected += OnItemSelected;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e) {
            var item = e.SelectedItem as MasterViewItem;
            if (item != null) {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType)) {
                    BarBackgroundColor = Color.FromHex("#202225")
                };
                masterView.listView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
