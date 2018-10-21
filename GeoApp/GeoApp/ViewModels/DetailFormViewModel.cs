using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using GeoApp.Views.Popups;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GeoApp
{
    class DetailFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand GetLocationCommand { get; set; }
        public ICommand AddPointCommand { get; set; }
        public ICommand DeletePointCommand { get; set; }
        public ICommand AddMetadataFieldCommand { get; set; }
        public ICommand DeleteMetadataFieldCommand { get; set; }
        public ICommand ButtonSaveCommand { get; set; }

        private DetailFormFieldPopup _detailFormPopup;

        private string _dateEntry;
        private bool _addMetadataFieldsBtnEnabled;
        private int _numPointFields;
        private bool _loadingIconActive;
        private bool _geolocationEntryEnabled;

        public bool ShowPointDeleteBtn { get { return _numPointFields > 1; } }

        public ObservableCollection<MetadataEntry> MetadataEntries { get; set; }
        public ObservableCollection<Point> GeolocationPoints { get; set; }

        public string DateEntry {
            get { return _dateEntry; }
            set {
                _dateEntry = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DateEntry"));
            }
        }

        public bool LoadingIconActive {
            get { return _loadingIconActive; }
            set {
                _loadingIconActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LoadingIconActive"));
            }
        }

        public bool GeolocationEntryEnabled {
            get { return _geolocationEntryEnabled; }
            set {
                _geolocationEntryEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GeolocationEntryEnabled"));
            }
        }

        public int NumPointFields {
            get { return _numPointFields; }
            set {
                _numPointFields = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumPointFields"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowPointDeleteBtn"));
            }
        }

        public bool AddMetadataFieldsBtnEnabled {
            get { return _addMetadataFieldsBtnEnabled; }
            set {
                _addMetadataFieldsBtnEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddMetadataFieldsBtnEnabled"));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DetailFormViewModel()
        {
            _detailFormPopup = new DetailFormFieldPopup();

            AddMetadataFieldsBtnEnabled = true;
            GeolocationEntryEnabled = true;
            LoadingIconActive = false;
            NumPointFields = 0;

            MetadataEntries = new ObservableCollection<MetadataEntry>();
            GeolocationPoints = new ObservableCollection<Point>();
            AddPoint();

            DateEntry = DateTime.Now.ToShortDateString();
            GetLocationCommand = new Command<Point>(async (point) =>
            {
                await GetGeoLocation(point);
            });

            AddMetadataFieldCommand = new Command(async () =>
            {
                await AddMetadataField();
            });

            AddPointCommand = new Command(() => AddPoint());
            DeleteMetadataFieldCommand = new Command<MetadataEntry>((item) => DeleteMetadataField(item));
            DeletePointCommand = new Command<Point>((item) => DeletePoint(item));

        }

        /// <summary>
        /// Queries the current device's location coordinates
        /// </summary>
        private async Task GetGeoLocation(Point point)
        {
            try
            {
                // Gets last known location of device (LESS ACCURATE, but faster)
                //var location = await Geolocation.GetLastKnownLocationAsync();
                GeolocationEntryEnabled = false;
                LoadingIconActive = true;

                // Gets current location of device (MORE ACCURATE, but slower)
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                LoadingIconActive = false;
                GeolocationEntryEnabled = true;

                if (location != null)
                {
                    point.Latitude = location.Latitude;
                    point.Longitude = location.Longitude;
                    point.Altitude = (double)location.Altitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                throw fnsEx;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                throw pEx;
            }
            catch (Exception ex)
            {
                // Unable to get location
                throw ex;
            }
        }

        /// <summary>
        /// Adds point data to line
        /// </summary>
        /// <returns></returns>
        private void AddPoint()
        {
            GeolocationPoints.Add(new Point(0, 0, 0));
            NumPointFields++;
        }

        private void DeletePoint(Point item)
        {
            GeolocationPoints.Remove(item);
            NumPointFields--;
        }

        private async Task AddMetadataField()
        {
            var result = await DetailFormFieldPopup.GetResultAsync();

            if (result != null)
            {
                MetadataEntries.Add(result);
                if (MetadataEntries.Count == 5)
                {
                    AddMetadataFieldsBtnEnabled = false;
                }
            }
        }

        private void DeleteMetadataField(MetadataEntry item)
        {
            MetadataEntries.Remove(item);
            if (MetadataEntries.Count < 5)
            {
                AddMetadataFieldsBtnEnabled = true;
            }
        }


    }
}