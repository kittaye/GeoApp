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
    /// <summary>
    /// View-model for the page that shows a data entry's details in a form.
    /// </summary>
    public class DetailFormViewModel : ViewModelBase {

        public ICommand GetFeatureCommand { get; set; }
        public ICommand AddPointCommand { get; set; }
        public ICommand DeletePointCommand { get; set; }
        public ICommand AddMetadataFieldCommand { get; set; }
        public ICommand DeleteMetadataFieldCommand { get; set; }
        public ICommand OnSaveUpdatedCommand { get; set; }
        public ICommand DeleteEntryCommand { get; set; }

        // Property binding to determine if the delete button for metadata fields is visible, which is based on the type of this entry.
        public bool ShowPointDeleteBtn { get { return _numPointFields > minPoints; } }
        private int minPoints = 0;

        // A reference to this entry's ID.
        private int thisEntryID;

        // A reference to this entry's type of structure.
        private string thisEntryType;

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
        /// View-model constructor for adding new entries.
        /// </summary>
        public DetailFormViewModel(string entryType) {
            thisEntryType = entryType;
            thisEntryID = AppConstants.NEW_ENTRY_ID;

            DateEntry = DateTime.Now.ToShortDateString();

            // Add the minimum number of points necessary for the chosen type.
            {
                if (entryType == "Point") {
                    minPoints = 1;
                } else if (entryType == "Line") {
                    minPoints = 2;
                } else if (entryType == "Polygon") {
                    minPoints = 4;
                } else {
                    Debug.WriteLine($"\n\n:::::::::::::::::::::UNSUPPORTED ENTRY TYPE: {entryType}");
                }
                GeolocationPoints = new ObservableCollection<Point>();
                for (int i = 0; i < minPoints; i++) {
                    AddPoint();
                }
            }

            MetadataEntries = new ObservableCollection<MetadataEntry>();

            AddMetadataFieldsBtnEnabled = true;
            GeolocationEntryEnabled = true;
            LoadingIconActive = false;

            InitCommandBindings();
        }

        /// <summary>
        /// View-model constructor for viewing/editing existing entries.
        /// </summary>
        public DetailFormViewModel(Feature data) {
            thisEntryType = data.geometry.type;
            thisEntryID = data.properties.id;

            NameEntry = data.properties.name;
            DateEntry = DateTime.Parse(data.properties.date).ToShortDateString();

            GeolocationPoints = new ObservableCollection<Point>(data.properties.xamarincoordinates);
            MetadataEntries = new ObservableCollection<MetadataEntry>();
            foreach (var item in data.properties.metadatafields) {
                MetadataEntries.Add(new MetadataEntry(item.Key, item.Value?.ToString(), Keyboard.Default));
            }

            AddMetadataFieldsBtnEnabled = true;
            GeolocationEntryEnabled = true;
            LoadingIconActive = false;

            InitCommandBindings();
        }

        /// <summary>
        /// Initialise command bindings.
        /// </summary>
        private void InitCommandBindings() {
            GetFeatureCommand = new Command<Point>(async (point) => { await GetGeoLocation(point); });

            AddMetadataFieldCommand = new Command(async () => { await AddMetadataField(); });
            DeleteMetadataFieldCommand = new Command<MetadataEntry>((item) => DeleteMetadataField(item));

            AddPointCommand = new Command(() => AddPoint());
            DeletePointCommand = new Command<Point>((item) => DeletePoint(item));

            DeleteEntryCommand = new Command(async () => await DeleteFeatureEntry());

            OnSaveUpdatedCommand = new Command(() => OnSaveUpdateActivated());
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
                    point.Altitude = location.Altitude ?? 0.0;
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

        /// <summary>
        /// Deletes the entire feature from the list of current features and saves changes to the embedded file.
        /// </summary>
        /// <returns></returns>
        private async Task DeleteFeatureEntry() {
            bool yesResponse = await HomePage.Instance.DisplayAlert("Delete Data Entry", "Are you sure you want to delete this entry?", "Yes", "No");
            if (yesResponse) {
                await App.FeaturesManager.DeleteFeatureAsync(thisEntryID);
                await HomePage.Instance.Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Saves a new or edited feature to the embedded file.
        /// </summary>
        async void OnSaveUpdateActivated() {
            // Ensure geolocation points are only accurate up to the specified digit precision.
            foreach (var point in GeolocationPoints) {
                AppConstants.RoundGPSPosition(point);
            }

            // Do validation checks here.
            if (await FeatureEntryIsValid() == false) {
                return;
            }

            Feature featureToSave = CreateFeatureFromInput();

            await App.FeaturesManager.SaveFeatureAsync(featureToSave);
            await HomePage.Instance.Navigation.PopAsync();
        }

        /// <summary>
        /// Creates a feature object based on the view-model data of this feature entry.
        /// </summary>
        /// <returns>A feature object formed from input values</returns>
        private Feature CreateFeatureFromInput() {
            Feature feature = new Feature();

            feature.type = "Feature";
            feature.properties = new Properties();
            feature.properties.name = NameEntry;
            feature.properties.date = DateTime.Parse(DateEntry).ToShortDateString();
            feature.properties.metadatafields = new Dictionary<string, object>();
            // A new entry will have an ID of NEW_ENTRY_ID as assigned from the constructor,
            // otherwise an ID will already be set for editing entries.
            feature.properties.id = thisEntryID;
            foreach (var metadataField in MetadataEntries) {
                feature.properties.metadatafields.Add(metadataField.LabelTitle, metadataField.LabelData);
            }

            feature.geometry = new Geometry();
            feature.geometry.type = thisEntryType;
            // Converts our xamarin coordinate data back into a valid geojson structure.
            {
                if (thisEntryType == "Point") {
                    feature.geometry.coordinates = new List<object>() {
                        GeolocationPoints[0].Latitude,
                        GeolocationPoints[0].Longitude,
                        GeolocationPoints[0].Altitude };
                } else if (thisEntryType == "Line") {
                    feature.geometry.coordinates = new List<object>(GeolocationPoints.Count);
                    for (int i = 0; i < GeolocationPoints.Count; i++) {
                        feature.geometry.coordinates.Add(new JArray(new double[3] {
                            GeolocationPoints[i].Latitude,
                            GeolocationPoints[i].Longitude,
                            GeolocationPoints[i].Altitude }));
                    }
                } else if (thisEntryType == "Polygon") {
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
            return feature;
        }

        /// <summary>
        /// Performs validation checks on the data in the form.
        /// </summary>
        /// <returns>True if the form contains valid data.</returns>
        private async Task<bool> FeatureEntryIsValid() {
            /// Begin validation checks.
            if (string.IsNullOrEmpty(NameEntry)) {
                await HomePage.Instance.DisplayAlert("Alert", "Feature name must not be empty.", "OK");
                return false;
            }

            foreach (var item in MetadataEntries) {
                item.LabelTitle = item.LabelTitle.Trim();
                if (item.LabelTitle.Contains(" ")) {
                    await HomePage.Instance.DisplayAlert("Alert", "Metadata labels must not have spaces.", "OK");
                    return false;
                }
            }

            if (thisEntryType == "Polygon") {
                if (GeolocationPoints.Count < 4) {
                    await HomePage.Instance.DisplayAlert("Alert", "A polygon structure must have at least 4 geolocational points.", "OK");
                    return false;
                }

                // Check if first and last points of the polygon have the same lat/long values.
                {
                    double firstLatitude = GeolocationPoints[0].Latitude;
                    double lastLatitude = GeolocationPoints[GeolocationPoints.Count - 1].Latitude;
                    double firstLongitude = GeolocationPoints[0].Longitude;
                    double lastLongitude = GeolocationPoints[GeolocationPoints.Count - 1].Longitude;

                    if (firstLatitude != lastLatitude || firstLongitude != lastLongitude) {
                        await HomePage.Instance.DisplayAlert("Alert", "The first and last points of a polygon must match.", "OK");
                        return false;
                    }
                }
            } else if (thisEntryType == "Line") {
                if (GeolocationPoints.Count < 2) {
                    await HomePage.Instance.DisplayAlert("Alert", "A line structure must have at least 2 geolocational points.", "OK");
                    return false;
                }
            } else if (thisEntryType == "Point") {
                if (GeolocationPoints.Count != 1) {
                    await HomePage.Instance.DisplayAlert("Alert", "A point structure must only have 1 geolocational point.", "OK");
                    return false;
                }
            }

            return true;
        }
    }
}
