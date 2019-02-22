using System.Linq;
using System.Reflection;
using GeoApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ExtendedPageRenderer))]
namespace GeoApp.iOS
{
    public class ExtendedPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var contentPage = this.Element as ContentPage;
            if (contentPage == null || NavigationController == null)
                return;

            var itemsInfo = contentPage.ToolbarItems;

            var navigationItem = this.NavigationController.TopViewController.NavigationItem;
            var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
            var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();

            var newLeftButtons = new UIBarButtonItem[] { }.ToList();
            var newRightButtons = new UIBarButtonItem[] { }.ToList();

            rightNativeButtons.ForEach(nativeItem =>
            {
                // [Hack] Get Xamarin private field "item"
                var field = nativeItem.GetType().GetField("_item", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    return;

                var info = field.GetValue(nativeItem) as ToolbarItem;
                if (info == null)
                    return;

                if (info.Priority == 1)
                    newLeftButtons.Add(nativeItem);
                else
                    newRightButtons.Add(nativeItem);
            });

            leftNativeButtons.ForEach(nativeItem =>
            {
                newLeftButtons.Add(nativeItem);
            });

            navigationItem.RightBarButtonItems = newRightButtons.ToArray();
            navigationItem.LeftBarButtonItems = newLeftButtons.ToArray();
        }
    }

}
