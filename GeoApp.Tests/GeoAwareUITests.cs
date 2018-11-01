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

        [Test]
        public void WelcomeTextIsDisplayed() {
            app.Repl();
        }
    }
}
