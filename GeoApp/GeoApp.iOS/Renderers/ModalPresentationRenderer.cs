using GeoApp;
using GeoApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(IDFormView), typeof(ModalPresentationRenderer))]
namespace GeoApp.iOS.Renderers
{
    public class ModalPresentationRenderer : PageRenderer
    {
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            parent.ModalInPresentation |= UIDevice.CurrentDevice.CheckSystemVersion(12, 0);
            base.WillMoveToParentViewController(parent);
        }
    }
}
