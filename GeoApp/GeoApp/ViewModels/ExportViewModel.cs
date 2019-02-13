using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

/// Export libs
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace GeoApp {
    public class ExportViewModel: ViewModelBase {

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
        public ExportViewModel() {

            // If export button clicked
            ExportButtonClickCommand = new Command(async () => {

                // Checks validity of email
                if (string.IsNullOrWhiteSpace(EmailEntry) == false)
                {
                    try
                    {
                        string JSONfile = App.FeaturesManager.ExportFeaturesToJson();
                        string email_Address = EmailEntry;


                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("GeoApp@gmail.com");

                        // Reciever's entered email address
                        mail.To.Add(email_Address);

                        mail.Subject = "Your requested Geo Application CSV dataset";
                        mail.Body = "The following attachment is your request dataset contained into a GEO JSON format";

                        // Add the Json attachment int othe email
                        mail.Attachments.Add(Attachment.CreateAttachmentFromString(JSONfile, "GeoAware.json"));


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

                    catch (Exception mailNotSent)
                    {
                        await HomePage.Instance.DisplayAlert("Status", "Unable to send email. Error :" + mailNotSent, "OK");
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

            ShareButtonClickCommand = new Command(async () => {
                await CrossShare.Current.Share(new ShareMessage
                {
                    Text = App.FeaturesManager.ExportFeaturesToJson(),
                    Title = "My Features"
                });

            });

            BackupButtonClickCommand = new Command(async () =>
            {
                string name = string.Format("My Features - {0}-{1}-{2} {3}-{4}-{5}.json", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                var filename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), name);
                try
                {
                    System.IO.File.WriteAllText(filename, App.FeaturesManager.ExportFeaturesToJson());
                }
                catch
                {
                    await HomePage.Instance.DisplayAlert("Backup Failure", "Backup unable to complete. Try removing previous backup files.", "OK");

                }
                await HomePage.Instance.DisplayAlert("Backup Success", "File now saved in app documents. on iOS, this document can be found in the Groundsman folder in your Files app or through iTunes file sharing. On Android, this document can be accessed through your file explorer.", "OK");
            });


        }
    }
}
