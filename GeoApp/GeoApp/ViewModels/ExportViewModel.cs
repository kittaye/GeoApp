using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    class ExportViewModel {
        public ICommand ButtonClickCommand { set; get; }

        public ExportViewModel() {

            ButtonClickCommand = new Command(() => {
                CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage {
                    Text = "my clipboard",
                    Title = "Share"
                });
            });
        }
    }


}
