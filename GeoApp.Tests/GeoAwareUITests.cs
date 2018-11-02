using System;
using System.Diagnostics;
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
        public void TestAddPointDataEntry() {
            DataEntryNav(0); // select point option

            app.Tap("nameEntry_Container"); // tap name entry field
            app.EnterText("TEST"); // input test ot the field
            app.Back(); // close keyboard
            // Verify that we're in New Point page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, "New Point");
            // tap the save entry icon (top right)
            app.TapCoordinates((app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.X,
                (app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.Y);

            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...(this is a hack)

            // Verify entry has been saved
            // Note that app.Query stores an array of AppResult[] since theres only one entry we use index of 0
            // index of 1 is the data type of the data entry
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "TEST");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Point");
        }

        [Test]
        public void TestAddLineDataEntry() {
            DataEntryNav(1); // select line option

            app.Tap("nameEntry_Container"); // tap name entry field
            app.EnterText("LineTest"); // input test ot the field
            app.Back(); // close keyboard
            // Verify that we're in New Line page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, "New Line");
            // tap the save entry icon (top right)
            app.TapCoordinates((app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.X,
                (app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.Y);

            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...(this is a hack)

            // Verify entry has been saved
            // Note that app.Query stores an array of AppResult[] since theres only one entry we use index of 0
            // index of 1 is the data type of the data entry
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "LineTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Line");
        }

        [Test]
        public void TestAddPolyDataEntry() {
            DataEntryNav(2); // select polygon option

            app.Tap("nameEntry_Container"); // tap name entry field
            app.EnterText("PolyTest"); // input test ot the field
            app.Back(); // close keyboard
            // Verify that we're in New Point page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, "New Polygon");
            // tap the save entry icon (top right)
            app.TapCoordinates((app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.X,
                (app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.Y);

            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...(this is a hack)

            // Verify entry has been saved
            // Note that app.Query stores an array of AppResult[] since theres only one entry we use index of 0
            // index of 1 is the data type of the data entry
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "PolyTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Polygon");
        }

        /// <summary>
        /// Remove after unit testing
        /// </summary>
        [Test]
        public void Repl() {
            app.Repl();
        }


        /// <summary>
        /// Helper function used to navigate to different entry types on the dialog box
        /// Point, Line, Polygon
        /// </summary>
        /// <param name="type"> 0 = Point , 1 = Line , 2 = Polygon</param>
        private void DataEntryNav(int type) {
            TestNavToAddDataEntryDialogBox();

            // Perform query on the current page to identify the dialogoptions
            var dialogOptions = app.Query("text1"); 
            // Select Point data type
            app.TapCoordinates(dialogOptions[type].Rect.CenterX, dialogOptions[type].Rect.CenterY);
        }
    }
}
