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
                if(instance == null) {
                    instance = new HomePage();
                }
                return instance;
            }
        }

        public HomePage() {
            InitializeComponent();
            var navigationPage = new NavigationPage(new DataEntryListView());
            navigationPage.Icon = "appIcon";
            navigationPage.Title = "Test";
            Children.Add(new MasterView());
            Children.Add(new GeoApp.HomeView());
            Children.Add(new GeoApp.DataEntryListView());
        }
    }
}
