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

        public ICommand ButtonClickedCommand { set; get; }
        public ICommand ItemTappedCommand { set; get; }
        public ICommand RefreshListCommand { set; get; }
        public ICommand EditEntryCommand { get; set; }

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
            IsRefreshing = true;
            Device.BeginInvokeOnMainThread(async () => {
                App.FeaturesManager.CurrentFeatures = await Task.Run(() => App.FeaturesManager.GetFeaturesAsync());
                EntryListSource = App.FeaturesManager.CurrentFeatures;
            });
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
