using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Xamarin.Essentials;
using PCLStorage;

namespace GeoApp
{
    public class ExportViewModel : ViewModelBase
    {

        public ICommand ExportButtonClickCommand { set; get; }

        public ICommand ShareButtonClickCommand { set; get; }

        public ICommand BackupButtonClickCommand { set; get; }

        private string _EmailEntry;
        public string EmailEntry
        {
            get { return _EmailEntry; }
            set
            {
                _EmailEntry = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// View-model constructor for the export page.
        /// </summary>
        public ExportViewModel()
        {

            // If export button clicked
            ExportButtonClickCommand = new Command(async () =>
            {

                // Checks validity of email
                if (string.IsNullOrWhiteSpace(EmailEntry) == false)
                {
                    try
                    {
                        string JSONfile = App.FeaturesManager.ExportFeaturesToJson();
                        string email_Address = EmailEntry;


                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("GroundsmanQUTApp@gmail.com");

                        // Reciever's entered email address
                        mail.To.Add(email_Address);

                        mail.Subject = "Groundsman Feature List Export";
                        mail.Body = "Attached is your requested GeoJSON dataset.";

                        // Add the Json attachment int othe email
                        mail.Attachments.Add(Attachment.CreateAttachmentFromString(JSONfile, string.Format("My Features - {0}-{1}-{2}.geojson", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)));

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("geoapplicationqut@gmail.com", "Geoapplication123");
                        SmtpServer.EnableSsl = true;

                        // Some security bypasses (if you remove this, email won't be sent and get rejected)
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        };

                        // Send the mail
                        SmtpServer.Send(mail);

                        // Confirmation alert
                        await HomePage.Instance.DisplayAlert("Status", "Email sent successfully", "OK");
                    }

                    catch (Exception)
                    {
                        await HomePage.Instance.DisplayAlert("Status", "Unable to send email. Make sure IMAP is enabled in your email address settings!", "OK");
                    }
                }

                else
                {
                    await HomePage.Instance.DisplayAlert("Invalid Email", "Email Address cannot be empty.", "OK");
                }

            });


            // If share button clicked
            if (!CrossShare.IsSupported)
                return;

            ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
            ShareButtonClickCommand = new Command(async () =>
            {
                IFile featuresFile = await App.FeaturesManager.GetEmbeddedFile();
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Features Export",
                    File = new ShareFile(featuresFile.Path, "text/plain")
                });
            });

            BackupButtonClickCommand = new Command(async () =>
            {
                IFile featuresFile = await App.FeaturesManager.GetEmbeddedFile();
                string textFile = await featuresFile.ReadAllTextAsync();
                await Clipboard.SetTextAsync(textFile);

                await HomePage.Instance.DisplayAlert("Features Copied", "Paste in any text field", "OK");
            });
        }
    }
}
