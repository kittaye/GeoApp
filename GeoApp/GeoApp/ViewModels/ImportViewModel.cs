using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    public class ImportViewModel {
        public ICommand ButtonClickCommand { set; get; }

        /// <summary>
        /// View-model constructor for the import page.
        /// Based on https://github.com/jamesmontemagno/PermissionsPlugin
        /// </summary>
        public ImportViewModel() {
            ButtonClickCommand = new Command(async () => {
                try {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

                    // If permissions allowed, prompt the user to pick a file.
                    if (status == PermissionStatus.Granted) {
                        FileData fileData = await CrossFilePicker.Current.PickFile();

                        // If the user didn't cancel, import the contents of the file they selected.
                        if (fileData != null) {
                            string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
                            await App.FeaturesManager.ImportFeaturesAsync(contents);
                        }
                    } else {

                        // Display storage permission popup if permission is not be established, display alert if the user declines 
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage)) {
                            await HomePage.Instance.DisplayAlert("File", "You need to enable storage permissions to import.", "OK");
                        }

                        // If the user accepts the permission get the resulting value and check the if the key exists
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                        if (results.ContainsKey(Permission.Storage)) {
                            status = results[Permission.Storage];
                        }
                    }
                } catch (Exception ex) {
                    Debug.WriteLine($"\n\n::::::::::::::::::::::Exception choosing file: {ex.ToString()}");
                    throw ex;
                }
            });
        }
    }
}
