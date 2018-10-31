using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Tests viewmodel constructor for adding new entries
        /// The minimum number of points necessary for the chosen entry type
        /// </summary>
        [Test]
        public void TestNewEntries() {
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

    }
}
