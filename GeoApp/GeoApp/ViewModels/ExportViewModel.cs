using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

/// Mine
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Plugin.FilePicker;
using Xamarin.Forms.PlatformConfiguration;
using Plugin.FilePicker.Abstractions;


namespace GeoApp {
    public class ExportViewModel: ViewModelBase {

        public ICommand ButtonClickCommand { set; get; }


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

            ButtonClickCommand = new Command(async () => {


                if (string.IsNullOrWhiteSpace(EmailEntry) == false)
                {
                    try
                    {

                        //JSON_attachment = App.FeaturesManager.ExportFeaturesToJson();

                        string testString = App.FeaturesManager.ExportFeaturesToJson();

                        string email_Address = EmailEntry;


                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("GeoApp@gmail.com");

                        mail.To.Add(email_Address);

                        mail.Subject = "Your requested Geo Application CSV dataset";
                        mail.Body = "The following attachment is your request dataset contained into a GEO JSON format";


                        // Attaching attachment files into mails
                        mail.Attachments.Add(Attachment.CreateAttachmentFromString(testString, "GeoAware.json"));
                        // mail.Attachments.Add(JSON_attachment);


                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("geoapplicationqut@gmail.com", "Geoapplication123");
                        SmtpServer.EnableSsl = true;

                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        };

                        SmtpServer.Send(mail);

                        await HomePage.Instance.DisplayAlert("Status", "Email sent successfully", "OK");
                    }

                    catch (Exception mailNotSent)
                    {
                        await HomePage.Instance.DisplayAlert("Status", "Unable to send email. Error :" + mailNotSent, "OK");
                    }
                }

                else
                {
                    await HomePage.Instance.DisplayAlert("Invalid ID", "The ID cannot be empty.", "OK");
                }

               
            });


        }
    }
}
