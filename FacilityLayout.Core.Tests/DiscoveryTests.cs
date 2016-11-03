using FaciltyLayout.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApplication1;
using System.Reflection;
using System.Windows.Forms;

namespace FacilityLayout.Core.Tests
{
    [TestFixture]
    public class DiscoveryTests
    {
        private Form1 _facilityLayoutForm;
        private string pathToDataFile;
        private FacilityStats facilityStats;
        private FacilityLayoutModel facilityLayout;

        [SetUp]
        public void SetUp()
        {
            pathToDataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SampleConfigFiles", "Kraft15_data.txt");
            _facilityLayoutForm = new Form1();
            _facilityLayoutForm.Configure_App(pathToDataFile);
            _facilityLayoutForm.FacilityLayoutModel = _facilityLayoutForm.GenerateFacilitySwarm(_facilityLayoutForm.FacilityStats);
            _facilityLayoutForm.myFacilityMatrix = _facilityLayoutForm.FacilityLayoutModel.Facility;
            facilityStats = _facilityLayoutForm.FacilityStats;
            facilityLayout = _facilityLayoutForm.FacilityLayoutModel;

            _facilityLayoutForm.Tile = new Label[facilityLayout.LayoutArea.Rows, facilityLayout.LayoutArea.Columns];

            for (int i = 0; i < facilityLayout.LayoutArea.Rows; i++)
            {
                for (int j = 0; j < facilityLayout.LayoutArea.Columns; j++)
                {
                    _facilityLayoutForm.Tile[i, j] = new Label();
                }
            }
        }

        [Test]
        public void Configure_App_Sets_Num_Departments()
        {
            Assert.AreEqual(15, facilityStats.DepartmentCount);
        }

        [Test]
        public void Configure_App_Sets_Facility_Row_And_Column_Numbers()
        {
            Assert.AreEqual(15, facilityStats.FacilitySize.Rows); //rows
            Assert.AreEqual(15, facilityStats.FacilitySize.Columns); //columns
        }

        [Test]
        public void Configure_App_Sets_Department_Sizes()
        {
            Assert.AreEqual(15, facilityStats.DepartmentSizes[1], "Dept 1 - unexpected size");
            Assert.AreEqual(10, facilityStats.DepartmentSizes[2], "Dept 2 - unexpected size");
            Assert.AreEqual(9, facilityStats.DepartmentSizes[3], "Dept 3 - unexpected size");
            Assert.AreEqual(7, facilityStats.DepartmentSizes[4], "Dept 4 - unexpected size");
            Assert.AreEqual(9, facilityStats.DepartmentSizes[5], "Dept 5 - unexpected size");
            Assert.AreEqual(25, facilityStats.DepartmentSizes[6], "Dept 6 - unexpected size");
            Assert.AreEqual(25, facilityStats.DepartmentSizes[7], "Dept 7 - unexpected size");
            Assert.AreEqual(15, facilityStats.DepartmentSizes[8], "Dept 8 - unexpected size");
            Assert.AreEqual(10, facilityStats.DepartmentSizes[9], "Dept 9 - unexpected size");
            Assert.AreEqual(25, facilityStats.DepartmentSizes[10], "Dept 10 - unexpected size");
            Assert.AreEqual(10, facilityStats.DepartmentSizes[11], "Dept 11 - unexpected size");
            Assert.AreEqual(15, facilityStats.DepartmentSizes[12], "Dept 12 - unexpected size");
            Assert.AreEqual(6, facilityStats.DepartmentSizes[13], "Dept 13 - unexpected size");
            Assert.AreEqual(19, facilityStats.DepartmentSizes[14], "Dept 14 - unexpected size");
            Assert.AreEqual(25, facilityStats.DepartmentSizes[15], "Dept 15 - unexpected size");
        }

        [Test]
        public void Configure_App_Sets_Volume_Matrix()
        {
            var expectedMatrix = new int[16, 16]
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

            for(var i = 0; i < expectedMatrix.GetLength(0); i++)
            {
                for(var j = 0; j < expectedMatrix.GetLength(1); j++)
                {
                    Assert.AreEqual(expectedMatrix[i, j], facilityStats.VolumeMatrix[i, j], $"mismatch at index [{i},{j}]");
                }
            }
        }

        [Test]
        public void Configure_App_Sets_Cost_Matrix()
        {
            var max = int.MaxValue;
            var expectedCostMatrix = new double[16, 16]
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

            for (var i = 0; i < expectedCostMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < expectedCostMatrix.GetLength(1); j++)
                {
                    Assert.AreEqual(expectedCostMatrix[i, j], facilityStats.CostMatrix[i, j], $"mismatch at index [{i},{j}]");
                }
            }
        }

        [Test]
        public void Configure_App_Sets_Flows()
        {
            //"FlowStats" are a struct which describe a department's relationship with all other departments.
            var flowStats = new FlowStats[]
                {
                    new FlowStats(), //filler
                    new FlowStats()
                    {
                        Flows = new int[16] { 0,0,4608,0,0,0,0,0,0,0,0,0,0,0,0,5760 }, //formula = volume / cost
                        FlowSum = 4608 + 5760,
                        NumRelations = 2
                    },
                    new FlowStats()
                    {
                        Flows = new int[16] { 0, 4608, 0,0,0,0,0,0,0,0,0,0,0,0,0,12 }, //formula = volume / cost
                        FlowSum = 4608 + 12,
                        NumRelations = 2
                    }
                };

            for (var i = 0; i < flowStats.Length; i++)
            {
                var expFlow = flowStats[i];
                var actualFlow = facilityStats.Flows[i];

                CollectionAssert.AreEqual(expFlow.Flows, actualFlow.Flows, $"Flows mismatched at index [{i}]");
                Assert.AreEqual(expFlow.FlowSum, actualFlow.FlowSum, $"Flowsum mismatched at index[{i}]");
                Assert.AreEqual(expFlow.NumRelations, actualFlow.NumRelations, $"NumRelations mismatched at index [{i}]");
            }
        }

        [Test]
        public void Generate_Facility_Swarm_Sets_Size_Of_Algortihm_Space()
        {
            Assert.AreEqual(21, _facilityLayoutForm.myFacilityMatrix.GetLength(0));
            Assert.AreEqual(21, _facilityLayoutForm.myFacilityMatrix.GetLength(1));
        }

        [Test]
        public void Generate_Facility_Swarm_Places_All_Required_Department_Tiles()
        {
            var actualDepartmentTiles = new Dictionary<int, int>();
            var facility = _facilityLayoutForm.myFacilityMatrix;

            for(var i = 0; i < facility.GetLength(0); i++)
            {
                for(var j = 0; j < facility.GetLength(1); j++)
                {
                    var department = facility[i, j];
                    int currentCount;
                    actualDepartmentTiles.TryGetValue(department, out currentCount);
                    actualDepartmentTiles[department] = currentCount + 1;
                }
            }

            foreach(var department in _facilityLayoutForm.FacilityStats.Departments)
            {
                //0th filler department has no expected area
                if(department.Id != 0)
                    Assert.AreEqual(department.Area, actualDepartmentTiles[department.Id]);
            }
        }

        [Test]
        public void Generate_Facility_Swarm_Marks_Assigned_Tiles()
        {
            var facility = _facilityLayoutForm.myFacilityMatrix;

            for (var i = 0; i < facility.GetLength(0); i++)
            {
                for (var j = 0; j < facility.GetLength(1); j++)
                {
                    var department = facility[i, j];
                    var assigned = department != 0;

                    Assert.AreEqual(assigned, _facilityLayoutForm.FacilityLayoutModel.IsTileAssigned(i, j));
                }
            }
        }

        [Test]
        public void Generate_Facility_Swarm_Marks_Fixed_Tiles()
        {
            var facility = _facilityLayoutForm.myFacilityMatrix;
            var facilityStats = _facilityLayoutForm.FacilityStats;

            for (var i = 0; i < facility.GetLength(0); i++)
            {
                for (var j = 0; j < facility.GetLength(1); j++)
                {
                    var departmentId = facility[i, j];
                    var department = facilityStats.GetDepartment(departmentId);

                    Assert.AreEqual(department.IsLocationFixed, _facilityLayoutForm.FacilityLayoutModel.IsTileFixed(i, j));
                }
            }
        }

        [Test]
        public void Generate_Facility_Swarm_Respects_Fixed_Department_Locations()
        {
            var facility = _facilityLayoutForm.myFacilityMatrix;
            var fixedDeptLocations = _facilityLayoutForm.FacilityStats.FixedDepartmentLocations;
            var fixedDepartmentId = _facilityLayoutForm.FacilityStats.Departments.Single(d => d.IsLocationFixed).Id;

            for(int i = fixedDeptLocations[0,0]; i < fixedDeptLocations[2,0]; i++)
            {
                for(int j = fixedDeptLocations[1,0]; j < fixedDeptLocations[3,0]; j++)
                {
                    Assert.AreEqual(fixedDepartmentId, facility[i - 1, j - 1]);
                }
            }
        }

        [Test]
        public void Release_The_Termites_Places_Termites()
        {
            _facilityLayoutForm.ReleaseTheTermites(100);

            var numTermitesAtPositions = new Dictionary<Position, int>();

            foreach(var termite in _facilityLayoutForm.myTermites)
            {
                int currentCount;
                numTermitesAtPositions.TryGetValue(termite.Position, out currentCount);
                numTermitesAtPositions[termite.Position] = currentCount + 1;
            }

            var avgNumTermitesPerPosition = numTermitesAtPositions.Values.Average();

            Assert.That(avgNumTermitesPerPosition < 1.25); //A magic number, but since we're only using 100 termites
                                                           //and the grid is 15 x 15, doubling on a tile should be rare.
        }

        [Test]
        public void Release_The_Termites_Sets_Termite_Direction()
        {
            _facilityLayoutForm.ReleaseTheTermites(100);

            foreach(var termite in _facilityLayoutForm.myTermites)
            {
                Assert.That(termite.RowPos != 0 || termite.ColumnPos != 0);
            }
        }

        [Test]
        public void Release_The_Termites_Does_Not_Allow_Termite_Placement_On_Fixed_Departments()
        {
            _facilityLayoutForm.ReleaseTheTermites(100);

            var fixedDepartmentIds = facilityStats.Departments.Where(d => d.IsLocationFixed).Select(d => d.Id);

            foreach (var termite in _facilityLayoutForm.myTermites)
            {
                Assert.IsFalse(fixedDepartmentIds.Contains(facilityLayout.GetTile(termite.Position)));
            }
        }

        [Test]
        public void Release_The_Termites_Allows_Termites_To_Pick_Up_The_Tile_They_Are_Placed_On()
        {
            var initialFacility = new int[facilityLayout.LayoutArea.Rows, facilityLayout.LayoutArea.Columns];

            for (int i = 0; i < facilityLayout.LayoutArea.Rows; i++)
            {
                for (int j = 0; j < facilityLayout.LayoutArea.Columns; j++)
                {
                    initialFacility[i, j] = facilityLayout.GetTile(i, j);
                }
            }

            _facilityLayoutForm.ReleaseTheTermites(100);

            var tilesAlreadyTaken = new Dictionary<Position, bool>();

            foreach(var termite in _facilityLayoutForm.myTermites)
            {
                bool tileAlreadyTaken;
                tilesAlreadyTaken.TryGetValue(termite.Position, out tileAlreadyTaken);

                if (!tileAlreadyTaken)
                {
                    var expectedDepartment = initialFacility[termite.RowPos, termite.ColumnPos];

                    Assert.AreEqual(expectedDepartment, termite.TileDept, $"unexpected termite tile at position [{termite.RowPos},{termite.ColumnPos}]");
                    Assert.AreEqual(expectedDepartment != 0, termite.HasTile, $"unexpected termite HasTile at position [{termite.RowPos},{termite.ColumnPos}]");

                    tilesAlreadyTaken[termite.Position] = true;
                }
                else
                {
                    Assert.AreEqual(0, termite.TileDept);
                    Assert.IsFalse(termite.HasTile);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < facilityLayout.LayoutArea.Rows; i++)
            {
                for (int j = 0; j < facilityLayout.LayoutArea.Columns; j++)
                {
                    _facilityLayoutForm.Tile[i, j]?.Dispose();
                }
            }

            _facilityLayoutForm?.Dispose();
        }

    }
}
