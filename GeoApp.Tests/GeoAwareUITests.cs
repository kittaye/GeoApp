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
            AppResult[] res = app.Query(x => x.Class("BottomNavigationitemView"));

            app.TapCoordinates(res[1].Rect.X, res[1].Rect.Y); // map view
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Maps"); // used to capture the current page
            app.TapCoordinates(res[2].Rect.X, res[2].Rect.Y); // import
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Import");
            app.TapCoordinates(res[3].Rect.X, res[3].Rect.Y); // export
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Export");
            app.TapCoordinates(res[0].Rect.X, res[0].Rect.Y); // data entries
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Data Entries");
        }

        /// <summary>
        /// Tests navigation to dialog box for selecting a data type for detail form view
        /// </summary>
        [Test]
        public void TestNavToAddDataEntryDialogBox() {
            AppResult[] res = app.Query(x => x.Class("ActionMenuItemView"));

            app.TapCoordinates(res[0].Rect.X, res[0].Rect.Y); // tap the add data entry icon (top right)
            Assert.AreEqual((app.Query(x => x.Id("alertTitle")))[0].Text, "Select a Data Type"); // check if the add button displays an alertbox
        }


        /// <summary>
        /// Tests navigation to detail form view (using POINT as an example)
        /// </summary>
        [Test]
        public void TestNavtoDetailFormView() {

            TestNavToAddDataEntryDialogBox();

            //perform query on the current page to identify the dialogoptions
            var dialogOptions = app.Query("text1"); // 0 = Point , 1 = Line , 2 = Polygon
            //select Point data type
            app.TapCoordinates(dialogOptions[0].Rect.CenterX, dialogOptions[0].Rect.CenterY);
            //Verify that we're in New Point page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, "New Point");
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
