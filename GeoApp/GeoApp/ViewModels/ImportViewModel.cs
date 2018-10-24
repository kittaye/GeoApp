using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
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
                    FileData fileData = await CrossFilePicker.Current.PickFile();
                    if (fileData == null)
                        return; // user canceled file picking

                    string fileName = fileData.FileName;
                    string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                    Debug.WriteLine("File name chosen: " + fileName);
                    Debug.WriteLine("File data: " + contents);
                } catch (Exception ex) {
                    Debug.WriteLine("Exception choosing file: " + ex.ToString());
                }
            });
        }
    }
}
