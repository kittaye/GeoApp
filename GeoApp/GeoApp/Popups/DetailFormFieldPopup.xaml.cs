using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;

namespace GeoApp.Popups {
  
    public partial class DetailFormFieldPopup : PopupPage {
        public DetailFormFieldPopup() {
            InitializeComponent();
        }

        protected override void OnAppearingAnimationBegin() {
            base.OnAppearingAnimationBegin();

            if (!IsAnimationEnabled) {
                AddButton.Scale = CloseButton.Scale = 1;
                AddButton.Opacity = CloseButton.Opacity = 1;
                return;
            }

            AddButton.Scale = CloseButton.Scale =  0.3;
            AddButton.Opacity = CloseButton.Opacity = 0;
        }

        protected override async Task OnAppearingAnimationEndAsync() {
            if (!IsAnimationEnabled)
                return;

            await Task.WhenAll(
                AddButton.ScaleTo(1),
                AddButton.FadeTo(1),
                CloseButton.ScaleTo(1),
                CloseButton.FadeTo(1));
        }

        private async void OnAdd(object sender, EventArgs e) {
            var loadingPage = new LoadingPopupPage();
            await Navigation.PushPopupAsync(loadingPage);
            await Task.Delay(500);
            await Navigation.RemovePopupPageAsync(loadingPage);
            await Navigation.PushPopupAsync(new DetailFormFieldSuccessPopup());
        }

        private void OnCloseButtonTapped(object sender, EventArgs e) {
            CloseAllPopup();
        }

        private async void OnClose(object sender, EventArgs e) {
            await PopupNavigation.Instance.PopAsync();
        }

        protected override bool OnBackgroundClicked() {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup() {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}
