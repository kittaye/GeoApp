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
            NavigationPage.SetHasNavigationBar(this, true);
            NavigationPage.SetHasBackButton(this, true);
        }

        async void OnDismissButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
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