using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExistingDetailFormView : ContentPage
    {
        public ExistingDetailFormView(Feature data)
        {
            InitializeComponent();
            BindingContext = new DetailFormViewModel(data);

            Title = $"View {data.Geometry.Type}";
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            geolocationListView.SelectedItem = null;
        }

        // Android button spam fix: force all opened pages to go back to main page.
        protected override bool OnBackButtonPressed()
        {
            HomePage.Instance.Navigation.PopToRootAsync();
            return true;
        }
    }
}