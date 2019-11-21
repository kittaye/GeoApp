using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileView : ContentPage
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        async void OnDismissButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        async void OnViewCellTapped(object sender, EventArgs e)
        {
            bool yesResponse = await HomePage.Instance.DisplayAlert("Reset User Data", "This will permanently erase all saved features. Do you wish to continue?", "Yes", "No");
            if (yesResponse)
            {
                await App.FeatureStore.DeleteAllFeatures();
                await HomePage.Instance.DisplayAlert("Reset User Data", "Your user data has been erased.", "Ok");
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        } 
    }
}