using System;
using CoreGraphics;
using GeoApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(TranslucentNavigationPageRenderer))]
namespace GeoApp.iOS
{
    public class TranslucentNavigationPageRenderer : NavigationRenderer
    {
        public bool viewAdded = false;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Some basic navigationbar styling.
            NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White,
                Font = UIFont.FromName("CircularSpotifyTxt-Med", 16)
            };

            // Remove background colors and such to form a completely transparent bar.
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.TintColor = UIColor.White;
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationBar.Translucent = true;

            if (NavigationBar != null && !viewAdded)
            {
                var effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
                var visualEffectView = new UIVisualEffectView(effect);

                var bounds = NavigationBar.Bounds;
                bounds.Offset(0, -10);
                bounds = bounds.Inset(0, -10);

                visualEffectView.Frame = bounds;
                visualEffectView.Tag = 74619;

                NavigationBar.AddSubview(visualEffectView);
                NavigationBar.SendSubviewToBack(visualEffectView);

                viewAdded = true;
            }
        }
    }
}