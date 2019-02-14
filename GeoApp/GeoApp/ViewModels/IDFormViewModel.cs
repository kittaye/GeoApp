using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    public class IDFormViewModel : ViewModelBase {

        public ICommand IDSubmitCommand { get; set; }

        private string _IDEntry;
        public string IDEntry {
            get { return _IDEntry; }
            set {
                _IDEntry = value;
                OnPropertyChanged();
            }
        }

        public IDFormViewModel() {
            IDSubmitCommand = new Command(async() => await SubmitIDEntry());
        }

        /// <summary>
        /// Submits the inputted ID entry from the user. If valid, the ID will be saved and the user continues to the main page.
        /// </summary>
        /// <returns>True if the submission was successful.</returns>
        private async Task<bool> SubmitIDEntry() {
            if (string.IsNullOrWhiteSpace(IDEntry) == false) {
                Application.Current.Properties["UserID"] = IDEntry;
                await Application.Current.SavePropertiesAsync();

                await HomePage.Instance.Navigation.PopModalAsync();
                return true;
            } else {
                await HomePage.Instance.DisplayAlert("Invalid ID", "The ID cannot be empty.", "OK");
                return false;
            }
        }
    }
}
