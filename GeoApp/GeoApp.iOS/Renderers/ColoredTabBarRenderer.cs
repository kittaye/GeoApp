using GeoApp;
using GeoApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(IDFormView), typeof(ModalPageRenderer))]
namespace GeoApp.iOS.Renderers
{
    public class ModalPageRenderer : PageRenderer
    {
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            parent.ModalInPresentation |= UIDevice.CurrentDevice.CheckSystemVersion(12, 0);
            base.WillMoveToParentViewController(parent);
        }
    }
}
