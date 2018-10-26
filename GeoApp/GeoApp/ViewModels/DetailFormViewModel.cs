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
using Newtonsoft.Json.Linq;

namespace GeoApp {
    // View-model for the page that shows a data entry's details as a form.
    class DetailFormViewModel : ViewModelBase {

        public ICommand GetLocationCommand { get; set; }
        public ICommand AddPointCommand { get; set; }
        public ICommand DeletePointCommand { get; set; }
        public ICommand AddMetadataFieldCommand { get; set; }
        public ICommand DeleteMetadataFieldCommand { get; set; }
        public ICommand OnSaveUpdatedCommand { get; set; }
        public ICommand DeleteEntryCommand { get; set; }

        // Popup used for creating new metadata fields.
        private DetailFormFieldPopup _detailFormPopup;

        // Property binding to determine if the delete button for metadata fields is visible.
        public bool ShowPointDeleteBtn { get { return _numPointFields > 1; } }

        public ObservableCollection<MetadataEntry> MetadataEntries { get; set; }
        public ObservableCollection<Point> GeolocationPoints { get; set; }

        private string _dateEntry;
        public string DateEntry {
            get { return _dateEntry; }
            set {
                _dateEntry = value;
                OnPropertyChanged();
            }
        }

        private string _nameEntry;
        public string NameEntry {
            get { return _nameEntry; }
            set {
                _nameEntry = value;
                OnPropertyChanged();
            }
        }

        private string _entryType;
        public string EntryType {
            get { return _entryType; }
            set {
                _entryType = value;
                OnPropertyChanged();
            }
        }

        private int _entryID;
        public int EntryID {
            get { return _entryID; }
            set {
                _entryID = value;
                OnPropertyChanged();
            }
        }

        private bool _loadingIconActive;
        public bool LoadingIconActive {
            get { return _loadingIconActive; }
            set {
                _loadingIconActive = value;
                OnPropertyChanged();
            }
        }

        private bool _geolocationEntryEnabled;
        public bool GeolocationEntryEnabled {
            get { return _geolocationEntryEnabled; }
            set {
                _geolocationEntryEnabled = value;
                OnPropertyChanged();
            }
        }

        private int _numPointFields;
        public int NumPointFields {
            get { return _numPointFields; }
            set {
                _numPointFields = value;
                OnPropertyChanged();
                OnPropertyChanged("ShowPointDeleteBtn");
            }
        }

        private bool _addMetadataFieldsBtnEnabled;
        public bool AddMetadataFieldsBtnEnabled {
            get { return _addMetadataFieldsBtnEnabled; }
            set {
                _addMetadataFieldsBtnEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// View-model Constructor
        /// </summary>
        public DetailFormViewModel() {
            // Initialise fields.
            {
                DateEntry = DateTime.Now.ToShortDateString();

                _detailFormPopup = new DetailFormFieldPopup();
                MetadataEntries = new ObservableCollection<MetadataEntry>();
                GeolocationPoints = new ObservableCollection<Point>();

                AddMetadataFieldsBtnEnabled = true;
                GeolocationEntryEnabled = true;
                LoadingIconActive = false;

                NumPointFields = 0;
            }

            // Add one geolocation point to the list of points as there must be at least one.
            AddPoint();

            // Initialise command bindings.
            {
                GetLocationCommand = new Command<Point>(async (point) => { await GetGeoLocation(point); });

                AddMetadataFieldCommand = new Command(async () => { await AddMetadataField(); });
                DeleteMetadataFieldCommand = new Command<MetadataEntry>((item) => DeleteMetadataField(item));

                AddPointCommand = new Command(() => AddPoint());
                DeletePointCommand = new Command<Point>((item) => DeletePoint(item));

                DeleteEntryCommand = new Command(async () => await DeleteEntry());

                OnSaveUpdatedCommand = new Command(() => OnSaveUpdateActivated());
            }
        }

        /// <summary>
        /// Queries the current device's location coordinates
        /// </summary>
        /// <param name="point">Point to set GPS data to.</param>
        private async Task GetGeoLocation(Point point) {
            try {
                // Disable interaction with entries to prevent errors.
                GeolocationEntryEnabled = false;
                LoadingIconActive = true;

                // Gets current location of device (MORE ACCURATE, but slower)
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                // Re-enable interaction.
                LoadingIconActive = false;
                GeolocationEntryEnabled = true;

                if (location != null) {
                    point.Latitude = location.Latitude;
                    point.Longitude = location.Longitude;

                    if (location.Altitude == null) {
                        point.Altitude = 0;
                    } else {
                        point.Altitude = (double)location.Altitude;
                    }

                }
            } catch (FeatureNotSupportedException fnsEx) {
                throw fnsEx;
            } catch (PermissionException pEx) {
                throw pEx;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Adds a new geolocation point to the list for line or polygon data types.
        /// </summary>
        /// <returns></returns>
        private void AddPoint() {
            GeolocationPoints.Add(new Point(0, 0, 0));
            NumPointFields++;
        }

        /// <summary>
        /// Deletes a geolocation point from the list.
        /// </summary>
        /// <param name="item">Item to delete</param>
        private void DeletePoint(Point item) {
            GeolocationPoints.Remove(item);
            NumPointFields--;
        }

        /// <summary>
        /// Adds a new metadata field to the list.
        /// </summary>
        /// <returns></returns>
        private async Task AddMetadataField() {
            // Waits for the user to fill in a popup form describing the metadata field.
            var result = await DetailFormFieldPopup.GetResultAsync();

            if (result != null) {
                MetadataEntries.Add(result);

                // Maximum of 5 metadata entries.
                if (MetadataEntries.Count == 5) {
                    AddMetadataFieldsBtnEnabled = false;
                }
            }
        }

        /// <summary>
        /// Deletes a metadata entry from the list.
        /// </summary>
        /// <param name="item">Item to delete.</param>
        private void DeleteMetadataField(MetadataEntry item) {
            MetadataEntries.Remove(item);
            if (MetadataEntries.Count < 5) {
                AddMetadataFieldsBtnEnabled = true;
            }
        }

        private async Task DeleteEntry() {
            bool yesResponse = await HomePage.Instance.DisplayAlert("Delete Entry", "Are you sure you want to delete this entry?", "Yes", "No");
            if (yesResponse) {
                await App.LocationManager.DeleteLocationAsync(EntryID);
                await HomePage.Instance.Navigation.PopAsync();
            }
        }

        async void OnSaveUpdateActivated() {
            // Do validation checks here.
            if (string.IsNullOrEmpty(NameEntry)) {
                await HomePage.Instance.DisplayAlert("Alert", "Location name must not be empty.", "OK");
                return;
            }

            foreach (var item in MetadataEntries) {
                item.LabelTitle = item.LabelTitle.Trim();
                if(item.LabelTitle.Contains(" ")) {
                    await HomePage.Instance.DisplayAlert("Alert", "Metadata labels must not have spaces.", "OK");
                    return;
                }
            }

            if(EntryType == "Polygon") {
                if(GeolocationPoints.Count < 4) {
                    await HomePage.Instance.DisplayAlert("Alert", "A polygon must have at least 4 points.", "OK");
                    return;
                }

                if(GeolocationPoints[0].Latitude != GeolocationPoints[GeolocationPoints.Count - 1].Latitude
                    || GeolocationPoints[0].Longitude != GeolocationPoints[GeolocationPoints.Count - 1].Longitude) {
                    await HomePage.Instance.DisplayAlert("Alert", "The first and last points of a polygon must match.", "OK");
                    return;
                }
            } else if (EntryType == "Line") {
                if (GeolocationPoints.Count < 2) {
                    await HomePage.Instance.DisplayAlert("Alert", "A line must have at least 2 points.", "OK");
                    return;
                }
            }

            // Create the feature object based on the view-model data of the entry.
            Feature feature = new Feature();
            {
                feature.type = "Feature";
                feature.properties = new Properties();
                feature.properties.name = NameEntry;
                feature.properties.date = DateTime.Parse(DateEntry).ToShortDateString();
                feature.properties.metadatafields = new Dictionary<string, object>();
                foreach (var metadataField in MetadataEntries) {
                    feature.properties.metadatafields.Add(metadataField.LabelTitle, metadataField.LabelData);
                }

                feature.geometry = new Geometry();
                feature.geometry.type = EntryType;
                if (EntryType == "Point") {
                    feature.geometry.coordinates = new List<object>() {
                        GeolocationPoints[0].Latitude,
                        GeolocationPoints[0].Longitude,
                        GeolocationPoints[0].Altitude };
                } else if (EntryType == "Line") {
                    feature.geometry.coordinates = new List<object>(GeolocationPoints.Count);
                    for (int i = 0; i < GeolocationPoints.Count; i++) {
                        feature.geometry.coordinates.Add(new JArray(new double[3] {
                            GeolocationPoints[i].Latitude,
                            GeolocationPoints[i].Longitude,
                            GeolocationPoints[i].Altitude }));
                    }
                } else if(EntryType == "Polygon") {
                    // This specific method of structuring points means that users will not
                    // be able to create multiple shapes in one polygon (whereas true GEOJSON allows that).
                    // This doesn't matter since our app interface can't allow for it anyway.
                    feature.geometry.coordinates = new List<object>(GeolocationPoints.Count);
                    List<object> innerPoints = new List<object>(GeolocationPoints.Count);
                    for (int i = 0; i < GeolocationPoints.Count; i++) {
                        innerPoints.Add(new JArray(new double[3] {
                            GeolocationPoints[i].Latitude,
                            GeolocationPoints[i].Longitude,
                            GeolocationPoints[i].Altitude }));
                    }
                    feature.geometry.coordinates.Add(innerPoints);
                }
            }

            if (EntryID == 0) {
                // Save the feature and go back to the entry list page.
                await App.LocationManager.SaveLocationAsync(feature);

            } else {
                feature.properties.id = EntryID;
                await App.LocationManager.EditSaveLocationAsync(feature);
            }

            await HomePage.Instance.Navigation.PopAsync();
        }
    }
}
