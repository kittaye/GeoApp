using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoApp {
    class DetailFormViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand GetLocationCommand { get; set; }

        private string[] _geoEntry = new string[3];

        public string LatEntry {
            get { return _geoEntry[0]; }
            set {
                _geoEntry[0] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LatEntry"));
            }
        }

        public string LongEntry {
            get { return _geoEntry[1]; }
            set {
                _geoEntry[1] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LongEntry"));
            }
        }

        public string AltEntry {
            get { return _geoEntry[2]; }
            set {
                _geoEntry[2] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AltEntry"));
            }
        }

        public DetailFormViewModel(string type) {
            GetLocationCommand = new Command(async () =>  {
               await GetGeoLocation();
            });
        }

        /// <summary>
        /// Queries the current device's location coordinates
        /// </summary>
        private async Task GetGeoLocation() {
            try {
                LatEntry = string.Empty;
                LongEntry = string.Empty;
                AltEntry = string.Empty;

                // Gets last known location of device (LESS ACCURATE, but faster)
                var location = await Geolocation.GetLastKnownLocationAsync();

                // Gets current location of device (MORE ACCURATE, but slower)
                //var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                //var location = await Geolocation.GetLocationAsync(request);

                if (location != null) {
                    LatEntry = $"{location.Latitude}";
                    LongEntry = $"{location.Longitude}";
                    AltEntry = $"{location.Altitude}";

                }
            } catch (FeatureNotSupportedException fnsEx) {
                // Handle not supported on device exception
                throw fnsEx;
            } catch (PermissionException pEx) {
                // Handle permission exception
                throw pEx;
            } catch (Exception ex) {
                // Unable to get location
                throw ex;
            }
        }
    }
}
