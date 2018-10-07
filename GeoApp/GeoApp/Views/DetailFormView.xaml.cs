using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailFormView : ContentPage {
        public DetailFormView(string type) {
            InitializeComponent();
            Title = $"New { type }";
            BindingContext = new DetailFormViewModel(type);


            // Add more elements to the page depending on the 
        }
    }
}