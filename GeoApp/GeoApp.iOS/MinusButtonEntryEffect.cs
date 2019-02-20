using GeoApp.iOS;
using UIKit;
using Xamarin.Forms;
using System;

[assembly: ResolutionGroupName("GeoLads")]
[assembly: ExportEffect(typeof(MinusButtonEntryEffect), "MinusButtonEntryEffect")]
namespace GeoApp.iOS
{
    public class MinusButtonEntryEffect : PlatformEffect<UIView, UITextField>
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control == null) return;
                var element = Element as Entry;
                if (element == null) return;

                UIBarButtonItem button = new UIBarButtonItem("-", UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    var position = Control.SelectedTextRange.Start;
                    var idx = (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, position);
                    element.Text = element.Text.Insert(idx, "-");
                });
                UIToolbar toolbar = new UIToolbar()
                {
                    Items = new[] { button }
                };
                Control.InputAccessoryView = toolbar;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: {0}", ex.Message);
            }

        }

        protected override void OnDetached()
        {
            //Control.InputAccessoryView = null;
        }
    }
}
