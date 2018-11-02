using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace GeoApp.Tests {
    [TestFixture]
    public class MapViewModelTests {
        MapViewModel mapvm;

        [SetUp]
        public void SetUp() {
            mapvm = new MapViewModel();
            mapvm.InitialiseMap();
        }

        /// <summary>
        /// Test that a pin is created and contains the correct information
        /// </summary>
        [Test]
        public void CreatePinTest() {
            mapvm.CreatePin("Test", 100, 100, 1);
            Assert.AreEqual(mapvm.GetPins().Count, 1);
            Assert.AreEqual(mapvm.GetPins()[0].Label, "Test");
            Assert.AreEqual(mapvm.GetPins()[0].Position, new Position(100, 100));
        }
    }
}
