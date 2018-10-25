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
    class DataEntryListViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ButtonClickedCommand { set; get; }
        public ICommand ItemTappedCommand { set; get; }
        public ICommand RefreshListCommand { set; get; }

        public ICommand EditEntryCommand { get; set; }

        private List<Feature> entryListSource;
        public List<Feature> EntryListSource {
            get { return entryListSource; }
            set {
                entryListSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EntryListSource"));
            }
        }

        private bool isRefreshing;
        public bool IsRefreshing {
            get { return isRefreshing; }
            set {
                isRefreshing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
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

        protected virtual void OnPropertyChanged(string propertyName) {
            var changed = PropertyChanged;
            if (changed != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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
