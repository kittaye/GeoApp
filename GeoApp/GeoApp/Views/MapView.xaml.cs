using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapView : ContentPage {

        public MapView()
        {
            InitializeComponent();
            Map pageMap = viewModel.InitialiseMap();
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(pageMap);
            Content = stack;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshMap();
        }


    }
}