using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GeoApp
{
    public class GoogleMapViewModel
    {
        public ObservableCollection<Pin> Pins { get; set; }

        public Command<MapClickedEventArgs> MapClickedCommand =>
            new Command<MapClickedEventArgs>(args =>
            {
                Pins.Add(new Pin
                {
                    Label = $" Pin {Pins.Count}",
                    Position = args.Point
                });
            });

        public Command<MyLocationButtonClickedEventArgs> LocationBtnClickedCommand =>
            new Command<MyLocationButtonClickedEventArgs>(args =>
            {
                Application.Current.MainPage.DisplayAlert("Hello", "Message", "Okay");
            });
    }
}
