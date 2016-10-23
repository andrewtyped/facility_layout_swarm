using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApplication1;

namespace FacilityLayout.Core.Tests
{
    [TestFixture]
    public class DiscoveryTests
    {
        private Form1 _facilityLayoutForm;

        [SetUp]
        public void SetUp()
        {
            _facilityLayoutForm = new Form1();
        }

        [TearDown]
        public void TearDown()
        {
            _facilityLayoutForm?.Dispose();
        }

    }
}
