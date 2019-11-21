using Xamarin.Forms;

namespace GeoApp
{
    public class ProfileViewModel : ViewModelBase
    {
        private string _IDEntry;
        public string IDEntry
        {
            get { return _IDEntry; }
            set
            {
                _IDEntry = value;
                HandleTextChanged();
            }
        }

        public ProfileViewModel()
        {
            if (Application.Current.Properties.ContainsKey("UserID") == true)
            {
                IDEntry = Application.Current.Properties["UserID"] as string;
            }
        }

        private void HandleTextChanged()
        {
            if (string.IsNullOrWhiteSpace(IDEntry) == false)
            {
                if (IDEntry.Length <= 30)
                {
                    Application.Current.Properties["UserID"] = IDEntry;
                }
                else
                {
                    Application.Current.Properties["UserID"] = IDEntry.Substring(0, 30);
                }

            }
            else
            {
                Application.Current.Properties["UserID"] = "Default";
            }
        }
    }
}
