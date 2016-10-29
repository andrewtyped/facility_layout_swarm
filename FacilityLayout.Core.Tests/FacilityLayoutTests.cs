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
            Assert.AreEqual(21, facilityLayout.Facility.GetLength(0));
            Assert.AreEqual(21, facilityLayout.Facility.GetLength(1));
        }
        
        [Test]
        public void InitializeDepartmentTiles_Places_All_Required_Department_Tiles()
        {
            var actualDepartmentTiles = new Dictionary<int, int>();
            var facility = facilityLayout.Facility;

            for (var i = 0; i < facility.GetLength(0); i++)
            {
                for (var j = 0; j < facility.GetLength(1); j++)
                {
                    var department = facility[i, j];
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
            var facility = facilityLayout.Facility;

            for (var i = 0; i < facility.GetLength(0); i++)
            {
                for (var j = 0; j < facility.GetLength(1); j++)
                {
                    var department = facility[i, j];
                    var assigned = department != 0;

                    Assert.AreEqual(assigned, facilityLayout.IsTileAssigned(i, j));
                    Assert.AreEqual(assigned, facilityLayout.IsTileAssigned(new Position(i,j)));
                }
            }
        }
        
        [Test]
        public void Generate_Facility_Swarm_Marks_Fixed_Tiles()
        {
            var facility = facilityLayout.Facility;

            for (var i = 0; i < facility.GetLength(0); i++)
            {
                for (var j = 0; j < facility.GetLength(1); j++)
                {
                    var departmentId = facility[i, j];
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
                        Assert.AreEqual(department.Id, facilityLayout.Facility[i - 1, j - 1]);
                    }
                }
            }
        }
    }
}
