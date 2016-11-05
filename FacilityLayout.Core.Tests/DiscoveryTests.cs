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
            facilityStats = _facilityLayoutForm.FacilityStats;
            facilityLayout = new FacilityLayoutModel(facilityStats);
            facilityLayout.InitializeDepartmentTiles();

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
