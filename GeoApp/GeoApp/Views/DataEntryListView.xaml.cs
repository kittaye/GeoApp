using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GeoApp.DataEntryListViewModel;

namespace GeoApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataEntryListView : ContentPage
    {
        private List<Feature> GeoList;
        private int FeatureCount;
        private ObservableCollection<GroupedVeggieModel> grouped { get; set; }
        public DataEntryListView()
        {
            InitializeComponent();
            var vm = BindingContext as DataEntryListViewModel;
            ExecuteRefreshListCommand();
            grouped = new ObservableCollection<GroupedVeggieModel>();
            var veggieGroup = new GroupedVeggieModel() { LongName = "vegetables", ShortName = "v" };
            var fruitGroup = new GroupedVeggieModel() { LongName = "fruit", ShortName = "f" };


            veggieGroup.Add(new VeggieModel() { Name = GeoList[0].properties.name, IsReallyAVeggie = true, Comment = "try ants on a log" });
            veggieGroup.Add(new VeggieModel() { Name = "tomato", IsReallyAVeggie = false, Comment = "pairs well with basil" });
            veggieGroup.Add(new VeggieModel() { Name = "zucchini", IsReallyAVeggie = true, Comment = "zucchini bread > bannana bread" });
            veggieGroup.Add(new VeggieModel() { Name = "peas", IsReallyAVeggie = true, Comment = "like peas in a pod" });
            fruitGroup.Add(new VeggieModel() { Name = "banana", IsReallyAVeggie = false, Comment = "available in chip form factor" });
            fruitGroup.Add(new VeggieModel() { Name = "strawberry", IsReallyAVeggie = false, Comment = "spring plant" });
            fruitGroup.Add(new VeggieModel() { Name = "cherry", IsReallyAVeggie = false, Comment = "topper for icecream" });
            grouped.Add(veggieGroup); grouped.Add(fruitGroup);
            listView.ItemsSource = grouped;
        }

        protected override void OnAppearing()
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

        public void ExecuteRefreshListCommand()
        {
            // Only update the list if it has changed as indicated by the dirty flag.
            if (isDirty)
            {
                isDirty = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    // Do a full re-read of the embedded file to get the most current list of features.
                    App.FeaturesManager.CurrentFeatures = await Task.Run(() => App.FeaturesManager.GetFeaturesAsync());
                    GeoList = App.FeaturesManager.CurrentFeatures;
                    FeatureCount = GeoList.Count;
                    Console.WriteLine(GeoList.Count);
                    foreach (Feature listItem in GeoList)
                    {
                        Console.WriteLine(listItem.properties.name);
                    }
                });
            }

            //IsRefreshing = false;
        }
    }
}