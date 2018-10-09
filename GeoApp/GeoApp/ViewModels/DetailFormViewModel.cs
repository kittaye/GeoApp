﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using GeoApp.Popups;

namespace GeoApp {
    class DetailFormViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand GetLocationCommand { get; set; }
        public ICommand AddPointsCommand { get; set; }
        public ICommand AddFieldsCommand { get; set; }

        private DetailFormFieldPopup _detailFormPopup;

        private string _dateEntry;
        private bool _isAddBtnVisible;
        private bool[] _metadataEntriesVisible = new bool[5];
        private string _addBtnTxt;
        private int[] _gridRow = new int[2];
        private string[] _geoEntry = new string[3];

        public bool MetadataEntry1Visible {
            get { return _metadataEntriesVisible[0]; }
            set {
                _metadataEntriesVisible[0] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetadataEntry1Visible"));
            }
        }

        public bool MetadataEntry2Visible {
            get { return _metadataEntriesVisible[1]; }
            set {
                _metadataEntriesVisible[1] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetadataEntry2Visible"));
            }
        }

        public bool MetadataEntry3Visible {
            get { return _metadataEntriesVisible[2]; }
            set {
                _metadataEntriesVisible[2] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetadataEntry3Visible"));
            }
        }

        public bool MetadataEntry4Visible {
            get { return _metadataEntriesVisible[3]; }
            set {
                _metadataEntriesVisible[3] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetadataEntry4Visible"));
            }
        }

        public bool MetadataEntry5Visible {
            get { return _metadataEntriesVisible[4]; }
            set {
                _metadataEntriesVisible[4] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetadataEntry5Visible"));
            }
        }

        public string DateEntry {
            get { return _dateEntry; }
            set {
                _dateEntry = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DateEntry"));
            }
        }

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

        public string AddPointBtnTxt {
            get { return _addBtnTxt; }
            set {
                _addBtnTxt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddFieldGridRow"));
            }
        }

        public bool AddBtnIsVisble {
            get { return _isAddBtnVisible; }
            set {
                _isAddBtnVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddBtnIsVisble"));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public DetailFormViewModel(string type) {
            _detailFormPopup = new DetailFormFieldPopup();

            // UI Changes based on selected type
            if (type == "Line" || type == "Polygon") {
                AddPointBtnTxt = $"Add to {type}";
                AddBtnIsVisble = true;
            } else {
                AddBtnIsVisble = false;
            }

            DateEntry = DateTime.Now.ToShortDateString();
            GetLocationCommand = new Command(async () =>  {
               await GetGeoLocation();
            });

            AddFieldsCommand = new Command(async () => {
               await AddFields();
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

        private async Task ViewPoints() {
            
        }

        /// <summary>
        /// Adds point data to line
        /// </summary>
        /// <returns></returns>
        private async Task AddPoint() {
            
        }

        private async Task AddFields() {
            await PopupNavigation.Instance.PushAsync(_detailFormPopup);
        }
    }
}
