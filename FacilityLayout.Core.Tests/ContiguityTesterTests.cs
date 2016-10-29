using FaciltyLayout.Core;
using FaciltyLayout.Core.Models;
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

        [Test]
        public void Detects_If_Adjacent_Tiles_Contain_Same_Department()
        {
            int[] myDeptSizes = new int[] { 0, 5, 4 };

            var adjTilesContainSameDept = _contiguityTester.AdjacentTilesContainSameDepartment(1, 3, 2, _contiguousFacility, myDeptSizes);
            Assert.IsTrue(adjTilesContainSameDept);
        }

        [Test]
        public void Detects_If_Adjacent_Tiles_Do_Not_Contain_Same_Department()
        {
            int[] myDeptSizes = new int[] { 0, 5, 4 };

            var adjTilesContainSameDept = _contiguityTester.AdjacentTilesContainSameDepartment(2, 2, 3, _contiguousFacility, myDeptSizes);
            Assert.IsFalse(adjTilesContainSameDept);
        }

        [Test]
        public void Corner_Counts_As_Adjacent_Tile()
        {
            int[] myDeptSizes = new int[] { 0, 5, 4 };

            var adjTilesContainSameDept = _contiguityTester.AdjacentTilesContainSameDepartment(2, 2, 2, _contiguousFacility, myDeptSizes);
            Assert.IsTrue(adjTilesContainSameDept);
        }


        [TestCase(3,2,1)]
        [TestCase(2,3,2)]
        [TestCase(2,1,3)]
        public void Counts_Number_Of_Similar_Tiles_Adjacent_To_A_Tile(int testRow, int testColumn, int expectedNumSimilarTiles)
        {
            Termites[] termites = new Termites[1]
            {
                new Termites()
                {
                    RowPos = testRow,
                    ColumnPos = testColumn,
                }
            };

            var numSimilarTiles = _contiguityTester.CountAdjacentTilesOfSameDepartment(termites[0].Position, _contiguousFacility);

            Assert.AreEqual(expectedNumSimilarTiles, numSimilarTiles);
        }
    }
}
