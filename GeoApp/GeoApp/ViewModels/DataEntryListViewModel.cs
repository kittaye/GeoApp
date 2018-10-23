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
    class DataEntryListViewModel {
        public ICommand ButtonClickedCommand { set; get; }

        public DataEntryListViewModel() {
            ButtonClickedCommand = new Command(async () => {
                await HomePage.Instance.ShowDetailFormOptions();
            });
        }
    }
}
