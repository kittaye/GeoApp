using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Share;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportView : ContentPage {
        public ExportView() {
            InitializeComponent();

            ButtonShare.Clicked += delegate {
                CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage {
                    Text = "my clipboard",
                    Title = "Share"
                });
            };
        }
    }
}