using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Foundation;
using UIKit;

namespace GeoApp.iOS {
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        App mainForms;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();

            Xamarin.FormsGoogleMaps.Init("AIzaSyDaeurrZExaOrUGhn5Q9_g447PSC7DOfHM");
            Xamarin.FormsGoogleMapsBindings.Init();

            Rg.Plugins.Popup.Popup.Init();
            mainForms = new App();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Debug.WriteLine("HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            App.FeaturesManager.ImportFeaturesFromFile(url.Path);
            return true;
        }
    }
}
