using Rg.Plugins.Popup.Pages;

namespace GeoApp.Views.Popups {

    public partial class LoadingPopupPage : PopupPage {
        public LoadingPopupPage() {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed() {
            return true;
        }
    }
}