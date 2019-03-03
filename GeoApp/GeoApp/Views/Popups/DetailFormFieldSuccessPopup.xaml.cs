using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace GeoApp.Views.Popups {

    public partial class DetailFormFieldSuccessPopup : PopupPage {
        public DetailFormFieldSuccessPopup() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();

            HidePopup();
        }

        private async void HidePopup() {
            await Task.Delay(6000);
            await PopupNavigation.Instance.RemovePageAsync(this);
        }
    }
}