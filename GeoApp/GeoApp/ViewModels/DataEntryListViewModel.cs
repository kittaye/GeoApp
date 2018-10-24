using System;
using System.Collections.Generic;
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

        public DataEntryListViewModel() {
            ButtonClickedCommand = new Command(async () => {
                await HomePage.Instance.ShowDetailFormOptions();
            });

            ItemTappedCommand = new Command<Feature> (async (data) => {
                await HomePage.Instance.ShowExistingDetailFormPage(data);
            });
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            var changed = PropertyChanged;
            if (changed != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
