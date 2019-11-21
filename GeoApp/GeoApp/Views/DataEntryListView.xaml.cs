using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataEntryListView : ContentPage
    {

        public DataEntryListView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing() // make data refresh on android - maybe with willappear
        {
            base.OnAppearing();

            if (((DataEntryListViewModel)BindingContext).RefreshListCommand.CanExecute(null))
            {
                ((DataEntryListViewModel)BindingContext).RefreshListCommand.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            //loadingList.IsRunning = false;
            //loadingList.IsVisible = false;
            base.OnDisappearing();
        }
    }
}