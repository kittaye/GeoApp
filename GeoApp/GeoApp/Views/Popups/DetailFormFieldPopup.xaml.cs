using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;

namespace GeoApp.Views.Popups {
  
    public partial class DetailFormFieldPopup : PopupPage {
        private TaskCompletionSource<MetadataEntry> tcs;

        public static async Task<MetadataEntry> GetResultAsync() {
            DetailFormFieldPopup popup = new DetailFormFieldPopup();

            var result = await popup._GetResultAsync();
            return result;
        }

        private async Task<MetadataEntry> _GetResultAsync() {
            tcs = new TaskCompletionSource<MetadataEntry>();

            await this.Navigation.PushPopupAsync(this);

            return await tcs.Task;
        }

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

            entryTitle.Focus();
        }

        private async void OnAdd(object sender, EventArgs e) {
            var loadingPage = new LoadingPopupPage();

            if (string.IsNullOrWhiteSpace(entryTitle.Text) || entryTitle.Text.Trim().Contains(" ")) {
                await HomePage.Instance.DisplayAlert("Alert", "Title must not have spaces!", "OK");
                return;
            }

            await Navigation.PushPopupAsync(loadingPage);
            await Task.Delay(500);
            await Navigation.RemovePopupPageAsync(loadingPage);
            await Navigation.PushPopupAsync(new DetailFormFieldSuccessPopup());

            if (string.IsNullOrEmpty(entryTitle.Text) == false && picker.SelectedItem != null) {
                MetadataEntry mle = new MetadataEntry();

                try {
                    mle.LabelTitle = entryTitle.Text.Trim();
                    if (picker.SelectedItem.ToString() != "String") {
                        mle.EntryType = Keyboard.Numeric;
                    } else {
                        mle.EntryType = Keyboard.Default;
                    }
                } catch (Exception exception) {
                    throw exception;
                }

                tcs.TrySetResult(mle);
            }

            CloseAllPopup();
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
            if(tcs.Task.Status != TaskStatus.RanToCompletion) {
                tcs.TrySetResult(null);
            }
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}
