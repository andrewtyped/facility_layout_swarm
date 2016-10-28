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
            var departmentCount = 15;
            var facilitySize = new GridSize(15, 15);

            var departments = new List<Department>()
            {
                new Department(0,0,false),
                new Department(1,15,false),
                new Department(2,10,false),
                new Department(3,9,false),
                new Department(4,7,false),
                new Department(5,9,false),
                new Department(6,25,false),
                new Department(7,25,false),
                new Department(8,15,false),
                new Department(9,10,false),
                new Department(10,25,false),
                new Department(11,10,false),
                new Department(12,15,false),
                new Department(13,6,false),
                new Department(14,19,false),
                new Department(15,25,true, new Position(1,1), new Position(5,5))
            };

            var volumeMatrix = new int[16, 16]
            {
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },   // filler
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,240 }, //dept1
                { 0,240,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, //dept2
                { 0,0,0,0,1200,0,0,0,0,0,0,0,0,0,0,0 }, //dept3
                { 0,0,0,0,0,0,0,0,0,0,1200,0,0,0,0,0 }, //dept4
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,600,0 }, //dept5
                { 0,0,0,0,0,0,0,0,480,0,0,0,0,0,0,0 }, //dept6
                { 0,0,0,0,0,0,0,0,480,0,0,0,0,0,0,0 }, //dept7
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,120 }, //dept8
                { 0,0,0,0,0,0,0,0,0,0,600,0,0,0,0,0 }, //dept9
                { 0,0,0,0,0,0,0,0,0,0,0,0,600,0,0,0 }, //dept10
                { 0,0,0,0,0,0,0,480,0,0,0,0,0,0,0,0 }, //dept11
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,600 }, //dept12
                { 0,0,0,0,0,0,0,480,0,0,0,0,0,0,0,0 }, //dept13
                { 0,0,0,0,0,0,0,0,0,0,0,0,600,0,0,0 }, //dept14
                { 0,0,10,50,0,25,40,0,0,25,0,40,0,20,0,0 } //dept15
            };

            var max = int.MaxValue;

            var costMatrix = new double[16, 16]
                {
                    { max, max,max,max,max,max,max,max,max,max,max,max,max,max,max,max },//filler
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept1
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept2
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept3
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept4
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept5
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept6
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept7
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept8
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept9
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept10
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept11
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept12
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept13
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept14
                    { max,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5 } //dept15
                };

            facilityStats = new FacilityStats(departmentCount, facilitySize, departments, volumeMatrix, costMatrix);
            facilityLayout = new FacilityLayoutModel(facilityStats);
            facilityLayout.InitializeDepartmentTiles();
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
