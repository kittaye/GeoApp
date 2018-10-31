using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoApp.Tests {
    [TestFixture]
    public class DetailFormViewModelTest {

        DetailFormViewModel viewModel;
        [SetUp]
        public void SetUp() {
            viewModel = new DetailFormViewModel("Point");
        }

        [Test]
        public void TestMethod() {
            // TODO: Add your test code here
            
            Assert.Pass("Your first passing test");
        }
    }
}
