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
            TapUpperRightButton();
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
            AddDataEntry(1, "LineTest");

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
        public void TestMultiAddDataEntry() {
            AddDataEntry(0, "MyPointTest");
            AddDataEntry(1, "MyLineTest");
            AddDataEntry(2, "MyPolyTest");

            // Verify entries has been saved
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "MyPointTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Point");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[2].Text, "MyLineTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[3].Text, "Line");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[4].Text, "MyPolyTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[5].Text, "Polygon");
        }

        /// <summary>
        /// Tests viewing a recently crated data entry
        /// </summary>
        [Test]
        public void TestViewDataEntry() {
            AddDataEntry(0, "MyViewTest");

            app.TapCoordinates((app.Query(c => c.Class("FormsTextView")))[0].Rect.X, (app.Query(c => c.Class("FormsTextView")))[0].Rect.Y);

            // Verify that we're in "View" page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, $"View Point");
        }

        /// <summary>
        /// Tests deleting data entry
        /// </summary>
        [Test]
        public void TestDeleteDataEntry() {
            TestViewDataEntry();

            AppResult[] res = app.Query(x => x.Class("ActionMenuItemView"));

            app.TapCoordinates(res[0].Rect.X, res[0].Rect.Y); // tap the delete data entry icon (top right)
            Assert.AreEqual((app.Query(x => x.Id("alertTitle")))[0].Text, "Delete Data Entry"); // check if the add button displays an alertbox

            // Perform query on the current page to identify the dialogoptions
            var dialogBtns = app.Query("button1");
            // Select Point data type
            app.TapCoordinates(dialogBtns[0].Rect.CenterX, dialogBtns[0].Rect.CenterY);

            // Verify that there are no more data entries
            Assert.AreEqual(app.Query(c => c.Class("FormsTextView")).Length, 0);
        }

        /// <summary>
        /// Tests editing a created data entry 
        /// </summary>
        [Test]
        public void TestEditDataEntry() {
            AddDataEntry(0, "MyEditTest");

            AppResult[] res = app.Query(x => x.Class("AppCompatButton"));
            app.TapCoordinates(res[0].Rect.CenterX, res[0].Rect.CenterY);
            app.TapCoordinates(0, 0);

            app.Tap("nameEntry_Container"); // tap name entry container
            app.ClearText(); // clear text input
            app.EnterText("NewEditTest"); // input new title
            app.Back(); // close keyboard

            TapUpperRightButton();

            // Verify edit has been made
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[0].Text, "NewEditTest");
            Assert.AreEqual((app.Query(c => c.Class("FormsTextView")))[1].Text, "Point");
        }

        /// <summary>
        /// Tests dialog box displaying when trying to save a data entry with no set name
        /// </summary>
        [Test]
        public void TestDenySaveDialog() {
            DataEntryDialogNav(0); // select point option
            TapUpperRightButton();
            Assert.AreEqual((app.Query(x => x.Id("alertTitle")))[0].Text, "Alert");
        }

        [Test]
        public void TestAddMetadataField() {
            DataEntryDialogNav(0); // select point option
            FillNameEntryField("MetadataTest");
            AddMetadataField("TestMetadata", 0); // select string
            Assert.AreEqual(app.Query("MetadataLabel")[0].Text, "TestMetadata");
        }

        [Test]
        public void TestBadAddMetadataField() {
            DataEntryDialogNav(0); // select point option
            FillNameEntryField("BadMetadataTest");
            AddMetadataField("Bad TestMetadata", 0); // select string
            Assert.AreEqual(app.Query("message")[0].Text, "Title must not have spaces!");
        }

        /// <summary>
        /// Helper function used to tap the menu item on the top right corner
        /// </summary>
        private void TapUpperRightButton() {
            AppResult[] save = app.Query(x => x.Class("ActionMenuItemView"));

            app.TapCoordinates(save[0].Rect.X, save[0].Rect.Y); // tap the save data entry icon (top right)
            app.TapCoordinates(0, 0);
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
        /// Helper function that creates a metadatafield
        /// </summary>
        /// <param name="fieldTitle">Text input for field title</param>
        /// <param name="option"> 0 = string, 1 = integer, 2 = float</param>
        private void AddMetadataField(string fieldTitle, int option) {
            // tap add field button
            app.TapCoordinates((app.Query(c => c.Class("AppCompatButton")))[0].Rect.CenterX, (app.Query(c => c.Class("AppCompatButton")))[0].Rect.CenterY);
            app.TapCoordinates(0, 0);

            // fill meta data
            app.Tap("entryTitle_Container");
            app.EnterText($"{fieldTitle}");
            app.Back();
            app.Tap("picker_Container");
            // index 0 = string, 1 = integer, 2 = float
            app.TapCoordinates((app.Query("text1"))[option].Rect.CenterX, (app.Query("text1"))[option].Rect.CenterY); // select String

            app.Tap("AddButtonMeta_Container");
            app.TapCoordinates(0, 0);
        }

        /// <summary>
        /// Helper function that fillers the name entry field of data entry
        /// </summary>
        /// <param name="text"></param>
        private void FillNameEntryField(string text) {
            app.Tap("nameEntry_Container"); // tap name entry field
            app.EnterText($"{text}"); // input test ot the field
            app.Back(); // close keyboard
        }

        /// <summary>
        /// Helper function to create a data entry
        /// </summary>
        /// <param name="type"> 0 = Point , 1 = Line , 2 = Polygon </param>
        /// <param name="title"> Name given to the data entry </param>
        private void AddDataEntry(int type, string title) {
            string header;

            DataEntryDialogNav(type); // select point option

            app.Tap("nameEntry_Container"); // tap name entry field
            app.EnterText($"{title}"); // input test ot the field
            app.Back(); // close keyboard

            if (type == 0) {
                header = "Point";
            } else if (type == 1) {
                header = "Line";
            } else {
                header = "Polygon";
            }

            // Verify that we're in New Point page
            Assert.AreEqual(app.Query(c => c.Class("AppCompatTextView"))[0].Text, $"New {header}");
            // tap the save entry icon (top right)
            app.TapCoordinates((app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.X,
                (app.Query(x => x.Class("ActionMenuItemView")))[0].Rect.Y);

            app.TapCoordinates(0, 0); // tap somewhere on the screen to update the view hierarchy...
        }
    }
}
