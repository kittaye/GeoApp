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
    public class ExportViewModel {
        public ICommand ButtonClickCommand { set; get; }

        /// <summary>
        /// View-model constructor for the export page.
        /// </summary>
        public ExportViewModel() {
            if (!CrossShare.IsSupported)
                return;

            ButtonClickCommand = new Command(async () => {
                await CrossShare.Current.Share(new ShareMessage {
                    Text = App.FeaturesManager.ExportFeaturesToJson(),
                    Title = "Share"
                });
            });
        }
    }
}
