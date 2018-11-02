using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp {
    public partial class HomePage : TabbedPage {
        private static HomePage instance;
        public static HomePage Instance {
            get {
                if (instance == null) {
                    instance = new HomePage();
                }
                return instance;
            }
        }

        public HomePage() {
            InitializeComponent();
        }

        /// <summary>
        /// Asynchronously adds DetailFormView to the top of the navigation stack.
        /// </summary>
        /// <param name="type">Data entry type</param>
        private void ShowNewDetailFormPage(string type) {
            Navigation.PushAsync(new ModifyDetailFormView(type));
        }

        public void ShowEditDetailFormPage(Feature entryToEdit) {
            Navigation.PushAsync(new ModifyDetailFormView(entryToEdit));
        }

        public async Task ShowExistingDetailFormPage(Feature data) {
            await Navigation.PushAsync(new ExistingDetailFormView(data));
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
