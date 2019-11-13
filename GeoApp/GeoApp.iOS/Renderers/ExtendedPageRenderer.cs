using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using System;
using CoreGraphics;
using GeoApp.Styles;
using GeoApp.iOS.Renderers;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ExtendedPageRenderer))]

namespace GeoApp.iOS.Renderers
{

    public class ExtendedPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null)
            {
                NavigationController.NavigationBar.TintColor = UIColor.FromRGB(76, 175, 80);
                if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                {
                    NavigationController.NavigationBar.PrefersLargeTitles = true;

                    NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Automatic;
                }
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (!(this.Element is ContentPage contentPage) || NavigationController == null)
                return;

            var navigationItem = this.NavigationController.TopViewController.NavigationItem;
            navigationItem.LeftBarButtonItems = null;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetAppTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {ex.Message}");
            }
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                if (TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
                {
                    SetAppTheme();
                }

            }
        }

        void SetAppTheme()
        {
            if (TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
            {
                if (App.AppTheme == "dark")
                    return;

                Xamarin.Forms.Application.Current.Resources = new DarkTheme();

                App.AppTheme = "dark";
            }
            else
            {
                if (App.AppTheme != "dark")
                    return;
                Xamarin.Forms.Application.Current.Resources = new LightTheme();
                App.AppTheme = "light";
            }
        }
    }

}
