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
    // View-model for the page that shows the list of data entries.
    class DataEntryListViewModel : ViewModelBase {

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

            EditEntryCommand = new Command<Feature>((feature) => EditEntry(feature));
        }

        private void ExecuteRefreshListCommand() {
            IsRefreshing = true;
            Device.BeginInvokeOnMainThread(async () => {
                App.LocationManager.CurrentLocations = await Task.Run(() => App.LocationManager.GetLocationsAsync());
                EntryListSource = App.LocationManager.CurrentLocations;
            });

            IsRefreshing = false;
        }

        private void EditEntry(Feature feature) {
            HomePage.Instance.ShowEditDetailFormPage(feature);
        }
    }
}
