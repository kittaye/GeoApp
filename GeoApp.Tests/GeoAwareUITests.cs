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
            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Maps"); // used to capture the current page
            app.TapCoordinates(res[2].Rect.X, res[2].Rect.Y); // import
            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Import");
            app.TapCoordinates(res[3].Rect.X, res[3].Rect.Y); // export
            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...
            Assert.AreEqual((app.Query(x => x.Id("largeLabel")))[0].Text, "Export");
            app.TapCoordinates(res[0].Rect.X, res[0].Rect.Y); // data entries
            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...
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
        /// Tests adding point data entry
        /// </summary>
        [Test]
        public void TestAddPointDataEntry() {
            AddDataEntry(0, "PointTest");

            // Verify entry has been saved
            // Note that app.Query stores an array of AppResult[] since theres only one entry we use index of 0
            // index of 1 is the data type of the data entry
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "PointTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Point");
        }

        /// <summary>
        /// Tests adding line data entry
        /// </summary>
        [Test]
        public void TestAddLineDataEntry() {
            AddDataEntry(1,"LineTest");

            // Verify entry has been saved
            // Note that app.Query stores an array of AppResult[] since theres only one entry we use index of 0
            // index of 1 is the data type of the data entry
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "LineTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Line");
        }

        /// <summary>
        /// Tests adding polygon data entry
        /// </summary>
        [Test]
        public void TestAddPolyDataEntry() {
            AddDataEntry(2, "PolyTest");

            // Verify entry has been saved
            // Note that app.Query stores an array of AppResult[] since theres only one entry we use index of 0
            // index of 1 is the data type of the data entry
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "PolyTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Polygon");
        }

        /// <summary>
        /// Tests adding multiple different types of data entries
        /// </summary>
        [Test]
        public void TestMultiAddDateEntry() {
            AddDataEntry(0, "MyPointTest");
            AddDataEntry(1, "MyLineTest");
            AddDataEntry(2, "MyPolyTest");

            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "MyPointTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Point");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[2].Text, "MyLineTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[3].Text, "Line");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[4].Text, "MyPolyTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[5].Text, "Polygon");
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
        private void DataEntryDialogNav(int type) {
            TestNavToAddDataEntryDialogBox();

            // Perform query on the current page to identify the dialogoptions
            var dialogOptions = app.Query("text1"); 
            // Select Point data type
            app.TapCoordinates(dialogOptions[type].Rect.CenterX, dialogOptions[type].Rect.CenterY);
        }

        /// <summary>
        /// Helper function to create a data entry
        /// </summary>
        /// <param name="type"> 0 = Point , 1 = Line , 2 = Polygon </param>
        /// <param name="title"> Name given to the data entry </param>
        private void AddDataEntry(int type, string title) {
            string header;

            DataEntryDialogNav(type); // select point option

            if (type == 0) {
                header = "Point";
            } else if (type == 1) {
                header = "Line";
            } else {
                header = "Polygon";
            }

            app.Tap("nameEntry_Container"); // tap name entry field
            app.EnterText($"{title}"); // input test ot the field
            app.Back(); // close keyboard
            // Verify that we're in New Point page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, $"New {header}");
            // tap the save entry icon (top right)
            app.TapCoordinates((app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.X,
                (app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.Y);

            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...
        }
    }
}
