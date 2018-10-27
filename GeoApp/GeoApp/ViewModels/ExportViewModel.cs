using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    class ExportViewModel {
        public ICommand ButtonClickCommand { set; get; }

        public ExportViewModel() {
            if (!CrossShare.IsSupported)
                return;

            ButtonClickCommand = new Command(async () => {
                await CrossShare.Current.Share(new ShareMessage {
                    Text = App.LocationManager.ExportLocationsToJson(),
                    Title = "Share"
                });
            });
        }
    }
}
