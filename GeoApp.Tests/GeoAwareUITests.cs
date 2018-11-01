using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace GeoApp.Tests {
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class GeoAwareUITests {
        IApp app;
        Platform platform;

        public GeoAwareUITests(Platform platform) {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest() {
            app = AppInitializer.StartApp(platform);
        }

        /// <summary>
        /// Tests tabbed navigation menu
        /// </summary>
        [Test]
        public void TestTabbedNavigation() {
            AppResult[] results = app.Query(c => c.Class("BottomNavigationitemView"));

            app.TapCoordinates(results[1].Rect.X, results[1].Rect.Y); // map view
            app.TapCoordinates(results[2].Rect.X, results[2].Rect.Y); // import
            app.TapCoordinates(results[3].Rect.X, results[3].Rect.Y); // export
            app.TapCoordinates(results[0].Rect.X, results[0].Rect.Y); // data entries
        }

        /// <summary>
        /// Remove after unit testing
        /// </summary>
        [Test]
        public void Repl() {
            app.Repl();
        }
    }
}
