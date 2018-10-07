using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    class DataEntryListViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ButtonClickedCommand { set; get; }

        public DataEntryListViewModel() {
            ButtonClickedCommand = new Command(async () => {

                await MainPage.Instance.ShowDetailFormOptions();
                //MainPage.Instance.ShowDetailFormPage();
            });
        }

    }
}
