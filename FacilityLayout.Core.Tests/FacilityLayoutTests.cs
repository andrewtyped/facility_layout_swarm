using FacilityLayout.Core.Tests.Stubs;
using FaciltyLayout.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityLayout.Core.Tests
{
    [TestFixture]
    public class FacilityLayoutTests
    {
        private FacilityStats facilityStats;
        private FacilityLayoutModel facilityLayout;

        [SetUp]
        public void SetUp()
        {
            facilityStats = FacilityStatsStub.Get();
            facilityLayout = FacilityLayoutModelStub.Get();
        }

        [Test]
        public void InitializeDepartmentTiles_Sets_Size_Of_Algortihm_Space()
        {
            Assert.AreEqual(facilityLayout.LayoutArea.Rows, facilityLayout.Facility.GetLength(0));
            Assert.AreEqual(facilityLayout.LayoutArea.Columns, facilityLayout.Facility.GetLength(1));
        }
        
        [Test]
        public void InitializeDepartmentTiles_Places_All_Required_Department_Tiles()
        {
            var actualDepartmentTiles = new Dictionary<int, int>();

            for (var i = 0; i < facilityLayout.LayoutArea.Rows; i++)
            {
                for (var j = 0; j < facilityLayout.LayoutArea.Columns; j++)
                {
                    var department = facilityLayout.GetTile(i,j);
                    int currentCount;
                    actualDepartmentTiles.TryGetValue(department, out currentCount);
                    actualDepartmentTiles[department] = currentCount + 1;
                }
            }

            foreach (var department in facilityStats.Departments)
            {
                //0th filler department has no expected area
                if (department.Id != 0)
                    Assert.AreEqual(department.Area, actualDepartmentTiles[department.Id], $"Mismatch for department [{department.Id}]");
            }
        }
        
        [Test]
        public void Generate_Facility_Swarm_Marks_Assigned_Tiles()
        {
            for (var i = 0; i < facilityLayout.LayoutArea.Rows; i++)
            {
                for (var j = 0; j < facilityLayout.LayoutArea.Columns; j++)
                {
                    var department = facilityLayout.GetTile(i, j);
                    var assigned = department != 0;

                    Assert.AreEqual(assigned, facilityLayout.IsTileAssigned(i, j));
                    Assert.AreEqual(assigned, facilityLayout.IsTileAssigned(new Position(i,j)));
                }
            }
        }
        
        [Test]
        public void Generate_Facility_Swarm_Marks_Fixed_Tiles()
        {
            for (var i = 0; i < facilityLayout.LayoutArea.Rows; i++)
            {
                for (var j = 0; j < facilityLayout.LayoutArea.Columns; j++)
                {
                    var departmentId = facilityLayout.GetTile(i, j);
                    var department = facilityStats.GetDepartment(departmentId);

                    Assert.AreEqual(department.IsLocationFixed, facilityLayout.IsTileFixed(i, j));
                    Assert.AreEqual(department.IsLocationFixed, facilityLayout.IsTileFixed(new Position(i,j)));
                }
            }
        }
        
        [Test]
        public void InitializeDepartmentTiles_Respects_Fixed_Department_Locations()
        {
            foreach (var department in facilityStats.Departments.Where(d => d.IsLocationFixed))
            {
                for (int i = (int)department.TopLeft?.Row; i < department.BottomRight?.Row; i++)
                {
                    for (int j = (int)department.TopLeft?.Column; j < department.BottomRight?.Column; j++)
                    {
                        Assert.AreEqual(department.Id, facilityLayout.GetTile(i - 1, j - 1));
                    }
                }
            }
        }

        [TestCase(-1,1, false)]
        [TestCase(12, -1, false)]
        [TestCase(22,1,false)]
        [TestCase(1,22,false)]
        [TestCase(1,1,false)] //fixed dept
        [TestCase(8,8,true)] //normal tile
        public void IsPositionLegal_Validates_Positions_In_The_Layout_Area(int row, int column, bool expectedIsValid)
        {
            Assert.AreEqual(expectedIsValid, facilityLayout.IsPositionValid(new Position(row, column)));
        }
    }
}
