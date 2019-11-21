using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IDFormView : ContentPage
    {
        public IDFormView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
        }

        // Stop the user from leaving the ID Entry page.
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}