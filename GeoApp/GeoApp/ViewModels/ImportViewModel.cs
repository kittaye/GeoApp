using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    class ImportViewModel {
        public ICommand ButtonClickCommand { set; get; }

        public ImportViewModel() {
            ButtonClickCommand = new Command(async () => {
                try {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                    if (status != PermissionStatus.Granted) {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage)) {
                            await HomePage.Instance.DisplayAlert("File", "Need that file", "OK");
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                        if (results.ContainsKey(Permission.Storage)) {
                            status = results[Permission.Storage];
                        }
                    }

                    if (status == PermissionStatus.Granted) {
                        FileData fileData = await CrossFilePicker.Current.PickFile();
                        if (fileData == null)
                            return; // user canceled file picking

                        string fileName = fileData.FileName;
                        string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray); // TODO: NEED TO PROPERLY EXTRACT DATA FROM FILE

                        Debug.WriteLine("File name chosen: " + fileName);
                        Debug.WriteLine("File data: " + contents);
                    }
                } catch (Exception ex) {
                    Debug.WriteLine("Exception choosing file: " + ex.ToString());
                }
            });
        }
    }
}
