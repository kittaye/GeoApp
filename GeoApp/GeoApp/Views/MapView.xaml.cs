using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapView : ContentPage {

        public MapView() {
            InitializeComponent();
            var stack = new StackLayout { Spacing = 0 };
            Map pageMap = viewModel.InitialiseMap();
            stack.Children.Add(pageMap);
            Content = stack;
        }

        protected override void OnAppearing() {
#if __IOS__
            viewModel.RefreshMap();
            base.OnAppearing();
#endif
        }
    }
}