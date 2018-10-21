using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    class DataEntryListViewModel {
        public ICommand ButtonClickedCommand { set; get; }
        public ICommand ButtonSaveCommand { set; get; }

        public DataEntryListViewModel() {
            ButtonClickedCommand = new Command(async () => {
                await HomePage.Instance.ShowDetailFormOptions();
            });
            ButtonSaveCommand = new Command<Feature>((item) => OnSaveUpdateActivated(item));
        }

        async void OnSaveUpdateActivated(Feature e)
        {
            Debug.WriteLine("HELLO:::::::::::::              Saving now--------------------------------------------------------------");
            await App.LocationManager.SaveLocationAsync(e);

        }
    }
}
