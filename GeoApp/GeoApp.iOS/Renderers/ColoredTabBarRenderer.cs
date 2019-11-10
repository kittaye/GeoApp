﻿using KickassUI.Spotify.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(ColoredTabBarRenderer))]
namespace KickassUI.Spotify.iOS.Renderers
{
    public class ColoredTabBarRenderer : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // Set our basic tab bar colors.
            UITabBar.Appearance.BarTintColor = UIColor.Clear;
            UITabBar.Appearance.TintColor = UIColor.Clear;
        }

        public override void ViewWillAppear(bool animated)
        {
            if (TabBar?.Items == null)
                return;

            // Go through our elements and change the icons
            if (Element is TabbedPage tabs)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                    UpdateTabBarItem(TabBar.Items[i], tabs.Children[i].IconImageSource.ToString());
            }

            base.ViewWillAppear(animated);
        }

        private void UpdateTabBarItem(UITabBarItem item, string icon)
        {
            if (item == null || icon == null)
                return;

            // Set the font for the title.
            item.SetTitleTextAttributes(new UITextAttributes() { TextColor = Color.Gray.ToUIColor() }, UIControlState.Normal);
            item.SetTitleTextAttributes(new UITextAttributes() { TextColor = Color.Black.ToUIColor() }, UIControlState.Selected);

            // Moves the titles up just a bit.
            item.TitlePositionAdjustment = new UIOffset(0, -2);
        }
    }
}
