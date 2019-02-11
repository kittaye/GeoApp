using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;

/// <summary>
/// Plugin provides base functionality for Plugins for Xamarin to gain access to the application's main Activity.
/// This file exposes an Android "Application" that registers for Activity changes.
/// https://github.com/jamesmontemagno/CurrentActivityPlugin/tree/6458fd91e83c6b56bbfda2c80b5cefcdbae2fe0a
/// </summary>
namespace GeoApp.Droid {
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks {

        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer) {
        }

        public override void OnCreate() {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate() {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState) {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity) {
        }

        public void OnActivityPaused(Activity activity) {
        }

        public void OnActivityResumed(Activity activity) {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) {
        }

        public void OnActivityStarted(Activity activity) {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity) {
        }
    }
}