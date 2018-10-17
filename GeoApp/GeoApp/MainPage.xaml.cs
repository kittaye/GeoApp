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

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e) {
            var item = e.SelectedItem as MasterViewItem;
            if (item != null) {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType)) {
                    BarBackgroundColor = Color.FromHex("#202225")
                };
                masterView.listView.SelectedItem = null;
                IsPresented = false;
            }
        }


        //----- Pages to Navigate ----//

        /// <summary>
        /// Asynchronously adds DetailFormView to the top of the navigation stack.
        /// </summary>
        /// <param name="type">Data entry type</param>
        private void ShowNewDetailFormPage(string type) {
            Detail.Navigation.PushAsync(new NewDetailFormView(type));
        }

        private void ShowExistingDetailFormPage(string type) {
            Detail.Navigation.PushAsync(new DetailFormView(type));
        }


        /// <summary>
        /// Displays a pop-up user interface to navigate to different data entry types
        /// </summary>
        /// <returns></returns>
        public async Task ShowDetailFormOptions() {
            var action = await DisplayActionSheet("Select a Data Type", "Cancel", null, "Point", "Line", "Polygon");
            switch (action) {
                case "Point":
                    ShowNewDetailFormPage("Point");
                    break;
                case "Line":
                    ShowNewDetailFormPage("Line");
                    break;
                case "Polygon":
                    ShowNewDetailFormPage("Polygon");
                    break;
                default:
                    break;
            }
        }
    }
}
