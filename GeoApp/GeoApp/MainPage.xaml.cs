using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp {
    public partial class MainPage : MasterDetailPage {
        private static MainPage instance;
        public static MainPage Instance {
            get {
                if(instance == null) {
                    instance = new MainPage();
                }
                return instance;
            }
        }

        private MainPage() {
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

        public void ShowDetailFormPage() {
            Detail.Navigation.PushAsync(new DetailFormView());
        }
    }
}
