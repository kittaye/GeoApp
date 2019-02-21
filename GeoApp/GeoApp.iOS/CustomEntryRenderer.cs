using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;
using UIKit;

[assembly: ExportRenderer(typeof(Entry), typeof(GeoApp.CustomEntryRenderer))]
namespace GeoApp

{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            // Check for only Numeric keyboard

            if (Element == null)
            {
                return;
            }

            if (this.Element.Keyboard == Keyboard.Numeric)
            {
                 this.AddNegDoneButton();
            }

            if (this.Element.Keyboard == Keyboard.Default || this.Element.Keyboard == Keyboard.Text)
            {
                this.AddDoneButton();
            }
        }
        /// <summary>
        /// Add toolbar with Done button
        /// </summary>
        /// 
        /// 

        protected void AddDoneButton()
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            toolbar.Translucent = true;

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                this.Control.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton

                };
            this.Control.InputAccessoryView = toolbar;
        }

        protected void AddNegDoneButton()
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            toolbar.Translucent = true;

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                this.Control.ResignFirstResponder();
            });
            var negButton = new UIBarButtonItem("-", UIBarButtonItemStyle.Plain, delegate {
                this.Control.InsertText("-");
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace),
                negButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton

                };
            this.Control.InputAccessoryView = toolbar;
        }
    }
}
