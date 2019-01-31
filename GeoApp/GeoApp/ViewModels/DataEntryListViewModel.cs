using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    /// <summary>
    /// View-model for the page that shows the list of data entries.
    /// </summary>
    public class DataEntryListViewModel : ViewModelBase {
        // Static flag that determines whether the features list should be updated or not.
        public static bool isDirty = true;

        public ICommand ButtonClickedCommand { set; get; }
        public ICommand ItemTappedCommand { set; get; }
        public ICommand RefreshListCommand { set; get; }
        public ICommand EditEntryCommand { get; set; }

        private bool firstRefreshOccured = false;

        private List<Feature> _entryListSource;
        public List<Feature> EntryListSource {
            get { return _entryListSource; }
            set {
                _entryListSource = value;
                OnPropertyChanged();
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing {
            get { return _isRefreshing; }
            set {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// View-model constructor.
        /// </summary>
        public DataEntryListViewModel() {
            ButtonClickedCommand = new Command(async () => {
                await HomePage.Instance.ShowDetailFormOptions();
            });

            ItemTappedCommand = new Command<Feature> (async (data) => {
                await HomePage.Instance.ShowExistingDetailFormPage(data);
            });

            RefreshListCommand = new Command(() => {
                ExecuteRefreshListCommand();
            });

            EditEntryCommand = new Command<Feature>((feature) => EditFeatureEntry(feature));
        }

        /// <summary>
        /// Refreshes the list of current locations by re-reading the embedded file contents.
        /// </summary>
        private void ExecuteRefreshListCommand() {
            // Only update the list if it has changed as indicated by the dirty flag.
            if (isDirty) {
                isDirty = false;

                Device.BeginInvokeOnMainThread(async () => {
                    // Do a full re-read of the embedded file to get the most current list of features.
                    App.FeaturesManager.CurrentFeatures = await Task.Run(() => App.FeaturesManager.GetFeaturesAsync());

                    // This double-checks that the source embedded file doesn't start off with feature IDs that are already clashing.
                    // All other operations within the app (adding/editing/deleting/importing) have ID clash logic implemented already.
                    // TODO: When we publish the app, we should remove this slow code and clear out the source data file to avoid this issue entirely.
                    // Meaning the user starts with a blank list of features when they first download the app (which is reasonable).
                    if (firstRefreshOccured == false) {
                        firstRefreshOccured = true;

                        foreach (var feature in App.FeaturesManager.CurrentFeatures) {
                            FileService.TryGetUniqueFeatureID(feature);
                        }

                        await App.FeaturesManager.SaveAllCurrentFeaturesAsync();

                        // After saving the new clash-free IDs, we need to get the features again to fix the LineString save conversion.
                        App.FeaturesManager.CurrentFeatures = await Task.Run(() => App.FeaturesManager.GetFeaturesAsync());
                    }

                    EntryListSource = App.FeaturesManager.CurrentFeatures;
                });
            }

            IsRefreshing = false;
        }

        /// <summary>
        /// Displays the edit page for the selected feature.
        /// </summary>
        /// <param name="feature">Feature to edit.</param>
        private void EditFeatureEntry(Feature feature) {
            HomePage.Instance.ShowEditDetailFormPage(feature);
        }
    }
}
