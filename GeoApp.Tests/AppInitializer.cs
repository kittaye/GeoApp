using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace GeoApp.Tests {
    public class AppInitializer {
        public static IApp StartApp(Platform platform) {
            if (platform == Platform.Android) {
                IApp app = ConfigureApp
                    .Android
                    .ApkFile("../../../GeoApp/GeoApp.Android/bin/Release/com.CompanyAware.GeoApp.apk")
                    .StartApp();

                return app;
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}