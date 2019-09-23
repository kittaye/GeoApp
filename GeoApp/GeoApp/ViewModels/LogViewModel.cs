using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Plugin.Share;
using PCLStorage;

namespace GeoApp
{
    public class LogViewModel : ViewModelBase
    {
        private CancellationTokenSource cts;
        private double lat= 0;
        private double lon = 0;
        private double alt = 0;
        public ICommand StartButtonClickCommand { set; get; }
        public ICommand ClearButtonClickCommand { set; get; }
        public ICommand ExportButtonClickCommand { set; get; }

        public bool isLogging = false;

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
                IFile featuresFile = await App.FeaturesManager.GetLogFile();
                if (!CrossShare.IsSupported)
                    return;
                await featuresFile.WriteAllTextAsync(TextEntry);
                ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Logfile",
                    File = new ShareFile(featuresFile.Path, "text/csv")
                });
            }
            ExportButtonClickCommand = new Command(ExportLog);
        }

        public void StartUpdate()
        {
            Console.WriteLine("Start");
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
            var ignore = UpdaterAsync(cts.Token);
            isLogging = true;
        }

        public void StopUpdate()
        {
            Console.WriteLine("Stop");
            if (cts != null) cts.Cancel();
            cts = null;
            isLogging = false;
        }

        public async Task UpdaterAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.WriteLine("Log");
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
