using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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

        private bool _UseAppleMaps;
        public bool UseAppleMaps
        {
            get { return _UseAppleMaps; }
            set
            {
                _UseAppleMaps = value;
                HandleMapSwitchChanged();
            }
        }

        private void HandleMapSwitchChanged()
        {
            Application.Current.Properties["UseAppleMaps"] = UseAppleMaps;
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
                Application.Current.Properties["UserID"] = IDEntry;
            }
            else
            {
                Application.Current.Properties["UserID"] = "Default";
            }
        }
    }
}
