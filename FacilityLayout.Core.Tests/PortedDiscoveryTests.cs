using FaciltyLayout.Core;
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
    public class PortedDiscoveryTests
    {
        private FacilityEvaluator _portedFunctions;

        [SetUp]
        public void SetUp()
        {
            _portedFunctions = new FacilityEvaluator();
        }

    }
}
