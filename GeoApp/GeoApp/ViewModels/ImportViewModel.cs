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
                            await App.LocationManager.ImportLocationsAsync(contents);
                        }
                    } else {
                        // What's happening here...

                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage)) {
                            await HomePage.Instance.DisplayAlert("File", "Need that file", "OK");
                        }

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
