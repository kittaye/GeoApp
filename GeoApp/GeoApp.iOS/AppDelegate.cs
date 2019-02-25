using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Foundation;
using UIKit;
using KeyboardOverlap.Forms.Plugin.iOSUnified;

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
            KeyboardOverlapRenderer.Init();
            var shouldPerformAdditionalDelegateHandling = true;

            // Get possible shortcut item
            if (options != null)
            {
                LaunchedShortcutItem = options[UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
                shouldPerformAdditionalDelegateHandling = (LaunchedShortcutItem == null);
            }

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

        public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

        public bool HandleShortcutItem(UIApplicationShortcutItem shortcutItem)
        {
            var handled = false;

            // Anything to process?
            if (shortcutItem == null) return false;

            // Take action based on the shortcut type
            switch (shortcutItem.Type)
            {
                case ShortcutIdentifier.First:
                    Console.WriteLine("First shortcut selected");
                    HomePage.Instance.ShowDetailFormOptions();
                    handled = true;
                    break;
            }

            // Return results
            return handled;
        }

        public override void OnActivated(UIApplication application)
        {
            // Handle any shortcut item being selected
            HandleShortcutItem(LaunchedShortcutItem);

            // Clear shortcut after it's been handled
            LaunchedShortcutItem = null;
        }

        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            // Perform action
            completionHandler(HandleShortcutItem(shortcutItem));
        }


    }
}
