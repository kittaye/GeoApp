using System.Linq;
using System.Reflection;
using GeoApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using System;
using CoreGraphics;
using GeoApp.Styles;
using GeoApp;

[assembly: ExportRenderer(typeof(DataEntryListView), typeof(ExtendedPageRenderer))]
[assembly: ExportRenderer(typeof(ExportView), typeof(ExtendedPageRenderer))]
[assembly: ExportRenderer(typeof(ImportView), typeof(ExtendedPageRenderer))]
[assembly: ExportRenderer(typeof(LogView), typeof(ExtendedPageRenderer))]
[assembly: ExportRenderer(typeof(ExistingDetailFormView), typeof(ExtendedPageRenderer))]
namespace GeoApp.iOS
{
    public class ExtendedPageRenderer : PageRenderer
    {
        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        private bool _pageWasShiftedUp;
        private double _activeViewBottom;
        private bool _isKeyboardShown;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);


            NavigationController.NavigationBar.PrefersLargeTitles = true;
            NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Automatic;


            if (Element is ContentPage page)
            {
                if (page.Content is ScrollView)
                    return;

                RegisterForKeyboardNotifications();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            UnregisterForKeyboardNotifications();
        }

        void RegisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
        }

        void UnregisterForKeyboardNotifications()
        {
            _isKeyboardShown = false;
            if (_keyboardShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }

            if (_keyboardHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }

        protected virtual void OnKeyboardShow(NSNotification notification)
        {
            if (!IsViewLoaded || _isKeyboardShown)
                return;

            _isKeyboardShown = true;
            var activeView = View.FindFirstResponder();

            if (activeView == null)
                return;

            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

            if (!isOverlapping)
                return;

            if (isOverlapping)
            {
                _activeViewBottom = activeView.GetViewRelativeBottom(View);
                ShiftPageUp(keyboardFrame.Height, _activeViewBottom);
            }
        }

        private void OnKeyboardHide(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            _isKeyboardShown = false;
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);

            if (_pageWasShiftedUp)
            {
                ShiftPageDown(keyboardFrame.Height, _activeViewBottom);
            }
        }

        private void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom)
        {

            if (Element.AutomationId != "import_icon")
            {
                var pageFrame = Element.Bounds;
                var newY = pageFrame.Y + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

                Element.LayoutTo(new Rectangle(pageFrame.X, newY,
                    pageFrame.Width, pageFrame.Height));

                _pageWasShiftedUp = true;
            }


        }

        private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;

            var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            Element.LayoutTo(new Rectangle(pageFrame.X, newY,
                pageFrame.Width, pageFrame.Height));

            _pageWasShiftedUp = false;
        }

        private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
        {
            return (pageHeight - activeViewBottom) - keyboardHeight;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (NavigationController != null)
            {
                NavigationController.NavigationBar.TintColor = UIColor.FromRGB(76, 175, 80);
            }
            if (!(this.Element is ContentPage contentPage) || NavigationController == null)
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

                if (!(field.GetValue(nativeItem) is ToolbarItem info))
                    return;

                //if (info.Priority == 1)
                //    newLeftButtons.Add(nativeItem);
                //else
                    newRightButtons.Add(nativeItem);
            });

            leftNativeButtons.ForEach(nativeItem =>
            {
                newLeftButtons.Add(nativeItem);
            });

            navigationItem.RightBarButtonItems = newRightButtons.ToArray();
            navigationItem.LeftBarButtonItems = newLeftButtons.ToArray();
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
			Console.WriteLine($"TraitCollectionDidChange: {TraitCollection.UserInterfaceStyle} != {previousTraitCollection.UserInterfaceStyle}");

			if (TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
			{
				SetAppTheme();
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

    public static class ViewExtensions
    {
        /// <summary>
        /// Find the first responder in the <paramref name="view"/>'s subview hierarchy
        /// </summary>
        /// <param name="view">
        /// A <see cref="UIView"/>
        /// </param>
        /// <returns>
        /// A <see cref="UIView"/> that is the first responder or null if there is no first responder
        /// </returns>
        public static UIView FindFirstResponder(this UIView view)
        {
            if (view.IsFirstResponder)
            {
                return view;
            }
            foreach (UIView subView in view.Subviews)
            {
                var firstResponder = subView.FindFirstResponder();
                if (firstResponder != null)
                    return firstResponder;
            }
            return null;
        }

        /// <summary>
        /// Returns the new view Bottom (Y + Height) coordinates relative to the rootView
        /// </summary>
        /// <returns>The view relative bottom.</returns>
        /// <param name="view">View.</param>
        /// <param name="rootView">Root view.</param>
        public static double GetViewRelativeBottom(this UIView view, UIView rootView)
        {
            var viewRelativeCoordinates = rootView.ConvertPointFromView(view.Frame.Location, view);
            var activeViewRoundedY = Math.Round(viewRelativeCoordinates.Y, 2);

            return activeViewRoundedY + view.Frame.Height;
        }

        /// <summary>
        /// Determines if the UIView is overlapped by the keyboard
        /// </summary>
        /// <returns><c>true</c> if is keyboard overlapping the specified activeView rootView keyboardFrame; otherwise, <c>false</c>.</returns>
        /// <param name="activeView">Active view.</param>
        /// <param name="rootView">Root view.</param>
        /// <param name="keyboardFrame">Keyboard frame.</param>
        public static bool IsKeyboardOverlapping(this UIView activeView, UIView rootView, CGRect keyboardFrame)
        {
            var activeViewBottom = activeView.GetViewRelativeBottom(rootView);
            var pageHeight = rootView.Frame.Height;
            var keyboardHeight = keyboardFrame.Height;

            var isOverlapping = activeViewBottom >= (pageHeight - keyboardHeight);

            return isOverlapping;
        }

    }

}
