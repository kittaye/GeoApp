﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp
{
    public class ProfileViewModel : ViewModelBase
    {

        public ICommand IDSubmitCommand { get; set; }
        public ICommand ResetDataCommand { get; set; }

        private string prevID;

        private string _IDEntry;
        public string IDEntry
        {
            get { return _IDEntry; }
            set
            {
                _IDEntry = value;
                OnPropertyChanged();
            }
        }

        public ProfileViewModel()
        {
            if (Application.Current.Properties.ContainsKey("UserID") == true)
            {
                IDEntry = Application.Current.Properties["UserID"] as string;
            }

            IDSubmitCommand = new Command(async () => await SubmitIDEntry());
            ResetDataCommand = new Command(async () => await ResetData());
        }

        /// <summary>
        /// Submits the inputted ID entry from the user. If valid, the ID will be saved and the user continues to the main page.
        /// </summary>
        /// <returns>True if the submission was successful.</returns>
        private async Task<bool> SubmitIDEntry()
        {
            // Make a copy of the feature list to iterate and modify
            var featureList = App.FeatureStore.CurrentFeatures.ToList();

            if (string.IsNullOrWhiteSpace(IDEntry) == false)
            {
                // Edits the UserID of all the features that belong to the previous ID set on the device 
                if (Application.Current.Properties.ContainsKey("UserID") == true)
                {
                    prevID = Application.Current.Properties["UserID"] as string;

                    foreach (var feature in featureList)
                    {
                        if (feature.properties.authorId == prevID)
                        {
                            feature.properties.authorId = IDEntry;
                            await App.FeatureStore.SaveFeatureAsync(feature);
                        }
                    }
                }
                Application.Current.Properties["UserID"] = IDEntry;

                await Application.Current.SavePropertiesAsync();
                await HomePage.Instance.DisplayAlert("User ID", "Your user ID has been updated.", "OK");
                return true;
            }
            else
            {
                await HomePage.Instance.DisplayAlert("Invalid ID", "Your user ID cannot be empty.", "OK");
                return false;
            }


        }

        private async Task ResetData()
        {
            bool yesResponse = await HomePage.Instance.DisplayAlert("Reset User Data", "This will permanently erase all saved features. Do you wish to continue?", "Yes", "No");
            if (yesResponse)
            {
                await App.FeatureStore.DeleteAllFeatures();
                await HomePage.Instance.DisplayAlert("Reset User Data", "Your user data has been erased.", "Ok");
            }
        }

    }
}