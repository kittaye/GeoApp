using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp
{
    /// <summary>
    /// View-model for the page that shows the list of data entries.
    /// </summary>
    public class DataEntryListViewModel : ViewModelBase
    {
        // Static flag that determines whether the features list should be updated or not.
        public static bool isDirty = true;

        public ICommand ButtonClickedCommand { set; get; }
        public ICommand IDClickedCommand { set; get; }
        public ICommand ItemTappedCommand { set; get; }
        public ICommand RefreshListCommand { set; get; }
        public ICommand EditEntryCommand { get; set; }
        public ICommand DeleteEntryCommand { get; set; }

        private bool _isBusy = false;

        private int _featureCount;
        public int FeatureCount{
            get { return _featureCount; }
            set
            {
                _featureCount = value;
                OnPropertyChanged();
            }
        }


        public class VeggieModel
        {
            public string Name { get; set; }
            public string Comment { get; set; }
            public bool IsReallyAVeggie { get; set; }
            public string Image { get; set; }
            public VeggieModel()
            {
            }
        }

        public class GroupedVeggieModel : ObservableCollection<VeggieModel>
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
        }


        private List<Feature> _entryListSource;
        public List<Feature> EntryListSource
        {
            get { return _entryListSource; }
            set
            {
                _entryListSource = value;
                OnPropertyChanged();
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// View-model constructor.
        /// </summary>
        public DataEntryListViewModel()
        {
            ButtonClickedCommand = new Command(async () => await ExecuteButtonClickedCommand());
            IDClickedCommand = new Command(() => IDTappedCommand());
            ItemTappedCommand = new Command<Feature>(async (data) => await ExecuteItemTappedCommand(data));
            RefreshListCommand = new Command(() => ExecuteRefreshListCommand());
            EditEntryCommand = new Command<Feature>((feature) => EditFeatureEntry(feature));
            DeleteEntryCommand = new Command<Feature>(async (feature) => await DeleteFeatureEntry(feature));
        }

        /// <summary>
        /// Opens the ExistingDetailFormView page showing more detail about the feature the user tapped on in the list.
        /// </summary>
        /// <param name="data">Feature tapped on.</param>
        /// <returns></returns>
        private async Task ExecuteItemTappedCommand(Feature data)
        {
            if (_isBusy) return;
            _isBusy = true;

            await HomePage.Instance.ShowExistingDetailFormPage(data);

            _isBusy = false;
        }

        private void IDTappedCommand()
        {
            if (_isBusy) return;
            _isBusy = true;

            HomePage.Instance.ShowProfileSettingsPage();

            _isBusy = false;
        }

        /// <summary>
        /// Opens up the dialog box where the user can select between Point, Line, and Polygon feature types to add.
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteButtonClickedCommand()
        {
            if (_isBusy) return;
            _isBusy = true;

            await HomePage.Instance.ShowDetailFormOptions();

            _isBusy = false;
        }

        /// <summary>
        /// Refreshes the list of current locations by re-reading the embedded file contents.
        /// </summary>
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
                    EntryListSource = App.FeaturesManager.CurrentFeatures;
                    FeatureCount = EntryListSource.Count;
                });
            }

            IsRefreshing = false;
        }

        /// <summary>
        /// Displays the edit page for the selected feature.
        /// </summary>
        /// <param name="feature">Feature to edit.</param>
        private void EditFeatureEntry(Feature feature)
        {
            if (_isBusy) return;
            _isBusy = true;

            HomePage.Instance.ShowEditDetailFormPage(feature);

            _isBusy = false;
        }

        private async Task DeleteFeatureEntry(Feature feature)
        {
            if (_isBusy) return;
            _isBusy = true;

            bool yesResponse = await HomePage.Instance.DisplayAlert("Delete Feature", "Are you sure you want to delete this feature?", "Yes", "No");
            if (yesResponse)
            {
                await App.FeaturesManager.DeleteFeatureAsync(feature.properties.id);
            }
            ExecuteRefreshListCommand();

            _isBusy = false;
        }
    }
}