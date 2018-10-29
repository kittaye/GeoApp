using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GeoApp {
    /// <summary>
    /// Defines the model used by listviews to present geolocational data.
    /// </summary>
    public class Point : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _latitude;
        private double _longitude;
        private double _altitude;

        public double Latitude {
            get { return _latitude; }
            set { _latitude = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude")); }
        }

        public double Longitude {
            get { return _longitude; }
            set { _longitude = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude")); }
        }

        public double Altitude {
            get { return _altitude; }
            set { _altitude = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Altitude")); }
        }

        public Point(double lat, double lng, double alt) {
            this.Latitude = lat;
            this.Longitude = lng;
            this.Altitude = alt;
        }
    }
}