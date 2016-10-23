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
    public class ContiguityTesterTests
    {
        private ContiguityTester _contiguityTester;
        private int[,] _contiguousFacility;
        private int[,] _nonContiguousFacility;
    
        [SetUp]
        public void SetUp()
        {
            _contiguousFacility = new int[4, 4]
            {
                {2, 2, 0 ,0 },
                {2, 2, 1 ,0 },
                {0, 1, 1 ,1 },
                {0, 1, 0 ,0 },
            };

            _nonContiguousFacility = new int[4, 4]
            {
                {0, 2, 0 ,0 },
                {0, 2, 1 ,0 },
                {0, 1, 1 ,1 },
                {0, 2, 0 ,0 },
            };

            _contiguityTester = new ContiguityTester();
        }

        [Test]
        public void Detects_Contiguity_Of_One_Department()
        {
            Assert.IsTrue(_contiguityTester.DepartmentIsContiguous(1, _contiguousFacility));
        }

        [Test]
        public void Detects_NonContiguity_Of_One_Department()
        {
            Assert.IsFalse(_contiguityTester.DepartmentIsContiguous(2, _nonContiguousFacility));
        }

        [Test]
        public void Detects_Contiguity_Of_All_Departments()
        {
            Assert.IsTrue(_contiguityTester.AllDepartmentsAreContiguous(_contiguousFacility));
        }

        [Test]
        public void Detects_Contiguity_Of_Any_Department()
        {
            Assert.IsFalse(_contiguityTester.AllDepartmentsAreContiguous(_nonContiguousFacility));
        }
    }
}
