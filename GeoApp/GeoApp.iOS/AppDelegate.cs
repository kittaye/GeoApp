using Foundation;
using UIKit;

namespace GeoApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

        App mainForms;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            //KeyboardOverlapRenderer.Init();
            var shouldPerformAdditionalDelegateHandling = true;

            // Get possible shortcut item
            if (options != null)
            {
                LaunchedShortcutItem = options[UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
                shouldPerformAdditionalDelegateHandling = (LaunchedShortcutItem == null);
            }
            UIColor tintColor = UIColor.FromRGB(76, 175, 80);
            UINavigationBar.Appearance.TintColor = tintColor;
            UINavigationBar.Appearance.Translucent = true;

            Rg.Plugins.Popup.Popup.Init();
            mainForms = new App();
            LoadApplication(new App());
            
            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            //add confirmation alert
            {
                App.FeatureStore.ImportFeaturesFromFile(url.Path);
            }
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
                    HomePage.Instance.ShowNewDetailFormPage("Point");
                    handled = true;
                    break;
                case ShortcutIdentifier.Second:
                    HomePage.Instance.ShowNewDetailFormPage("Line");
                    handled = true;
                    break;
                case ShortcutIdentifier.Third:
                    HomePage.Instance.ShowNewDetailFormPage("Polygon");
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
