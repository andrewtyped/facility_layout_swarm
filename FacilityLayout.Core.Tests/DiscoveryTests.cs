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

namespace FacilityLayout.Core.Tests
{
    [TestFixture]
    public class DiscoveryTests
    {
        private Form1 _facilityLayoutForm;
        private string pathToDataFile;

        [SetUp]
        public void SetUp()
        {
            pathToDataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SampleConfigFiles", "Kraft15_data.txt");
            _facilityLayoutForm = new Form1();
            _facilityLayoutForm.Configure_App(pathToDataFile);
        }

        [Test]
        public void Configure_App_Sets_Num_Departments()
        {
            Assert.AreEqual(15, _facilityLayoutForm.myNumDepartments);
        }

        [Test]
        public void Configure_App_Sets_Facility_Row_And_Column_Numbers()
        {
            Assert.AreEqual(15, _facilityLayoutForm.myDeptRowsColumns[0]); //rows
            Assert.AreEqual(15, _facilityLayoutForm.myDeptRowsColumns[1]); //columns
        }

        [Test]
        public void Configure_App_Sets_Department_Sizes()
        {
            Assert.AreEqual(15, _facilityLayoutForm.myDeptSizes[1], "Dept 1 - unexpected size");
            Assert.AreEqual(10, _facilityLayoutForm.myDeptSizes[2], "Dept 2 - unexpected size");
            Assert.AreEqual(9, _facilityLayoutForm.myDeptSizes[3], "Dept 3 - unexpected size");
            Assert.AreEqual(7, _facilityLayoutForm.myDeptSizes[4], "Dept 4 - unexpected size");
            Assert.AreEqual(9, _facilityLayoutForm.myDeptSizes[5], "Dept 5 - unexpected size");
            Assert.AreEqual(25, _facilityLayoutForm.myDeptSizes[6], "Dept 6 - unexpected size");
            Assert.AreEqual(25, _facilityLayoutForm.myDeptSizes[7], "Dept 7 - unexpected size");
            Assert.AreEqual(15, _facilityLayoutForm.myDeptSizes[8], "Dept 8 - unexpected size");
            Assert.AreEqual(10, _facilityLayoutForm.myDeptSizes[9], "Dept 9 - unexpected size");
            Assert.AreEqual(25, _facilityLayoutForm.myDeptSizes[10], "Dept 10 - unexpected size");
            Assert.AreEqual(10, _facilityLayoutForm.myDeptSizes[11], "Dept 11 - unexpected size");
            Assert.AreEqual(15, _facilityLayoutForm.myDeptSizes[12], "Dept 12 - unexpected size");
            Assert.AreEqual(6, _facilityLayoutForm.myDeptSizes[13], "Dept 13 - unexpected size");
            Assert.AreEqual(19, _facilityLayoutForm.myDeptSizes[14], "Dept 14 - unexpected size");
            Assert.AreEqual(25, _facilityLayoutForm.myDeptSizes[15], "Dept 15 - unexpected size");
        }

        [Test]
        public void Configure_App_Sets_Fixed_Department_Flags()
        {
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[1], "Dept 1 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[2], "Dept 2 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[3], "Dept 3 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[4], "Dept 4 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[5], "Dept 5 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[6], "Dept 6 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[7], "Dept 7 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[8], "Dept 8 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[9], "Dept 9 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[10], "Dept 10 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[11], "Dept 11 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[12], "Dept 12 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[13], "Dept 13 - unexpected fixed indicator");
            Assert.IsFalse(_facilityLayoutForm.myFixedDeptIndicator[14], "Dept 14 - unexpected fixed indicator");
            Assert.IsTrue(_facilityLayoutForm.myFixedDeptIndicator[15], "Dept 15 - unexpected fixed indicator");
        }

        [Test]
        public void Configure_App_Sets_Fixed_Department_Locations()
        {
            //Badly implemented - fixed dept location stores the upper left corner, width, and height of a single dept
            Assert.AreEqual(1, _facilityLayoutForm.myFixedDeptLocations[0, 0]); //row
            Assert.AreEqual(1, _facilityLayoutForm.myFixedDeptLocations[1, 0]); //column
            Assert.AreEqual(5, _facilityLayoutForm.myFixedDeptLocations[2, 0]); //width
            Assert.AreEqual(5, _facilityLayoutForm.myFixedDeptLocations[3, 0]); //height
            Assert.AreEqual(4, _facilityLayoutForm.myFixedDeptLocations.GetLength(0));
            Assert.AreEqual(1, _facilityLayoutForm.myFixedDeptLocations.GetLength(1));
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
                    Assert.AreEqual(expectedMatrix[i, j], _facilityLayoutForm.myVolumeMatrix[i, j], $"mismatch at index [{i},{j}]");
                }
            }
        }


        [TearDown]
        public void TearDown()
        {
            _facilityLayoutForm?.Dispose();
        }

    }
}
