using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataEntryListView : ContentPage {

        public DataEntryListView() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();

            if (((DataEntryListViewModel)BindingContext).RefreshListCommand.CanExecute(null)) {
                ((DataEntryListViewModel)BindingContext).RefreshListCommand.Execute(null);
            }
        }

        protected override void OnDisappearing() {
            loadingList.IsRunning = false;
            loadingList.IsVisible = false;
            base.OnDisappearing();
        }
    }
}