using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapView : ContentPage {
        Map map;
        public MapView()
        {
            InitializeComponent();

            map = new Map(
            MapSpan.FromCenterAndRadius(new Position(-27.4784849, 153.0262424), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;

        }



        public void Locc()
        {
            List<Feature> features = new List<Feature>();
            features = App.LocationManager.CurrentLocations;

            foreach (var feature in features)
            {
                foreach (var point in feature.properties.xamarincoordinates)
                {
                    addPin(feature.properties.name, point.Latitude, point.Longitude);
                    
                }
            }
        }



        public void addPin(string name, double lat, double longt){
            var position = new Position(lat, longt); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = name,
                Address = "custom detail info"
            };
            map.Pins.Add(pin);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Locc();
        }

    }
}