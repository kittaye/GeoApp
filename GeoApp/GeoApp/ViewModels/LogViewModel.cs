using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Plugin.Share;
using System.IO;

namespace GeoApp
{
    public class LogViewModel : ViewModelBase
    {
        private CancellationTokenSource cts;
        private double lat;
        private double lon;
        private double alt;
        public ICommand StartButtonClickCommand { set; get; }
        public ICommand ClearButtonClickCommand { set; get; }
        public ICommand ExportButtonClickCommand { set; get; }
        private const string LOG_FILENAME = "log.csv";

        public bool isLogging;

        private string _textEntry;

        public string TextEntry
        {
            get { return _textEntry; }
            set
            {
                _textEntry = value;
                OnPropertyChanged();
            }
        }

        private int _intervalEntry;

        public int IntervalEntry
        {
            get { return _intervalEntry; }
            set
            {
                _intervalEntry = value;
                OnPropertyChanged();
            }
        }

        public LogViewModel()
        {

            StartButtonClickCommand = new Command(() =>
            {
                if (isLogging)
                {
                    StopUpdate();
                }
                else
                {
                    if (IntervalEntry > 0)
                    {
                        StartUpdate();
                    }
                    else
                    {
                        IntervalEntry = 1;
                        StartUpdate();
                    }
                }
            });

            ClearButtonClickCommand = new Command(() =>
           {
               TextEntry = "";
           });

            ExportButtonClickCommand = new Command(() =>
            {
                ExportLog();
            });

            async void ExportLog()
            {
                if (!CrossShare.IsSupported)
                    return;
                File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, LOG_FILENAME), TextEntry);
                ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Logfile",
                    File = new ShareFile(Path.Combine(FileSystem.AppDataDirectory, LOG_FILENAME), "text/csv")
                });
            }
            ExportButtonClickCommand = new Command(ExportLog);
        }

        public void StartUpdate()
        {
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
            var ignore = UpdaterAsync(cts.Token);
            isLogging = true;
        }

        public void StopUpdate()
        {
            if (cts != null) cts.Cancel();
            cts = null;
            isLogging = false;
        }

        public async Task UpdaterAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await GetGeoLocation();
                string newEntry = string.Format("{0}, {1}, {2}, {3} \n", DateTime.Now, lat, lon, alt);
                TextEntry += newEntry;
                await Task.Delay(IntervalEntry * 999, ct);
            }
        }

        private async Task GetGeoLocation()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (status == PermissionStatus.Granted)
                {
                    // Gets current location of device (MORE ACCURATE, but slower)
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    var location = await Geolocation.GetLocationAsync(request);
                    if (location != null)
                    {
                        lat = location.Latitude;
                        lon = location.Longitude;
                        alt = location.Altitude ?? 0;
                    }
                }
                else
                {
                    await HomePage.Instance.DisplayAlert("Permissions Error", "Location permissions for Groundsman must be enabled to utilise this feature.", "Ok");
                }
            }
            catch (Exception)
            {
                await HomePage.Instance.DisplayAlert("Geolocation Error", "Location services must be enabled to utilise this feature", "Ok");
                throw new Exception();
            }
        }
    }
}
