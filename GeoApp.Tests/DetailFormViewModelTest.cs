using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GeoApp.Tests {
    [TestFixture]
    public class DetailFormViewModelTest {

        //DetailFormViewModel viewModel;
        DetailFormViewModel viewModelPoint;
        DetailFormViewModel viewModelLine;
        DetailFormViewModel viewModelPoly;

        [SetUp]
        public void SetUp() {
            viewModelPoint = new DetailFormViewModel("Point");
            viewModelLine = new DetailFormViewModel("Line");
            viewModelPoly = new DetailFormViewModel("Polygon");
        }

        /// <summary>
        /// Tests the minimum number of points necessary for the chosen entry type
        /// </summary>
        [Test]
        public void TestNewEntriesNumDefaultPoints() {
            Assert.AreEqual(viewModelPoint.GeolocationPoints.Count, 1);
            Assert.AreEqual(viewModelLine.GeolocationPoints.Count, 2);
            Assert.AreEqual(viewModelPoly.GeolocationPoints.Count, 4);
        }

        /// <summary>
        /// Tests adding geolocation points in the line and polygon viewmodels
        /// </summary>
        [Test]
        public void TestAddPoint() {
            viewModelLine.GeolocationPoints.Add(new Point(10, 10, 10));
            viewModelPoly.GeolocationPoints.Add(new Point(10, 10, 10));
            // 2 as the 'Point' entry type starts
            Assert.AreEqual(viewModelLine.GeolocationPoints.Count, 3);
            Assert.AreEqual(viewModelPoly.GeolocationPoints.Count, 5);
        }

        /// <summary>
        /// Tests deleting geolocation points in line and polygon viewmodels
        /// </summary>
        [Test]
        public void TestDeletePoint() {
            var testPoint = new Point(10, 10, 10);

            viewModelLine.GeolocationPoints.Add(testPoint);
            viewModelLine.GeolocationPoints.Add(testPoint);
            viewModelPoly.GeolocationPoints.Add(testPoint);

            viewModelLine.GeolocationPoints.Remove(testPoint);
            viewModelPoly.GeolocationPoints.Remove(testPoint);

            Assert.AreEqual(viewModelLine.GeolocationPoints.Count, 3);
            Assert.AreEqual(viewModelPoly.GeolocationPoints.Count, 4);
        }

        /// <summary>
        /// Tests adding metadatafield 
        /// </summary>
        [Test]
        public void TestAddMetadataField() {
            MetadataEntry item = new MetadataEntry("Test", "mytest", Keyboard.Default);

            viewModelPoint.MetadataEntries.Add(item);
            viewModelPoint.MetadataEntries.Add(item);
            viewModelLine.MetadataEntries.Add(item);
            viewModelPoly.MetadataEntries.Add(item);
            viewModelPoly.MetadataEntries.Add(item);
            viewModelPoly.MetadataEntries.Add(item);

            Assert.AreEqual(viewModelPoint.MetadataEntries.Count, 2);
            Assert.AreEqual(viewModelLine.MetadataEntries.Count, 1);
            Assert.AreEqual(viewModelPoly.MetadataEntries.Count, 3);
        }


        /// <summary>
        /// Tests deletion of metadata entry from the list
        /// </summary>
        [Test]
        public void TestDeleteMetaDataField() {
            MetadataEntry item = new MetadataEntry("Test", "mytest", Keyboard.Default);

            viewModelPoint.MetadataEntries.Add(item);
            viewModelPoint.MetadataEntries.Add(item);
            viewModelLine.MetadataEntries.Add(item);
            viewModelPoly.MetadataEntries.Add(item);
            viewModelPoly.MetadataEntries.Add(item);
            viewModelPoly.MetadataEntries.Add(item);

            viewModelPoly.MetadataEntries.Remove(item);
            viewModelPoly.MetadataEntries.Remove(item);
            viewModelLine.MetadataEntries.Remove(item);
            viewModelPoint.MetadataEntries.Remove(item);

            Assert.AreEqual(viewModelPoint.MetadataEntries.Count, 1);
            Assert.AreEqual(viewModelLine.MetadataEntries.Count, 0);
            Assert.AreEqual(viewModelPoly.MetadataEntries.Count, 1);
        }
    }
}
