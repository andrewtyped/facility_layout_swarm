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
    public class FacilityEvaluatorTests
    {
        private FacilityEvaluator _facilityEvaluator;
        private int[,] _facilityMatrix;
        private int[] _deptSizes;
        private int _numDepartments;

        [SetUp]
        public void SetUp()
        {
            _facilityMatrix = new int[4, 4] //extra space comes from the space larger than the facility itself on which the algorithm executes
            {
                {0, 1, 1, 1 },
                {0, 2, 2, 2 },
                {0, 3, 3, 3 },
                {0, 0, 0, 0 }
            };

            _numDepartments = 3;
            _deptSizes = new int[4] { 0, 3, 3, 3 };
            _facilityEvaluator = new FacilityEvaluator();
        }

        [Test]
        public void CentroidCalculator_Calculates_Centers_Of_Departments()
        {
            double[,] centroids = _facilityEvaluator.CentroidCalculator(_numDepartments, _facilityMatrix, _deptSizes);

            Assert.AreEqual(0, centroids[1, 0]);
            Assert.AreEqual(2, centroids[1, 1]);
            Assert.AreEqual(1, centroids[2, 0]);
            Assert.AreEqual(2, centroids[2, 1]);
            Assert.AreEqual(2, centroids[3, 0]);
            Assert.AreEqual(2, centroids[3, 1]);
        }

        [Test]
        public void VDCProduct_Calculates_Volume_Distance_Cost_Product_Of_Facility()
        {
            int[,] volumeMatrix = new int[4, 4]
            {
                {0, 0, 0, 0 },
                {0, 0, 0, 10 },
                {0, 5, 0, 20 },
                {0, 10, 0, 0 },
            };

            double[,] costMatrix = new double[4, 4]
            {
                {0, 0, 0, 0 },
                {0, 1, 1, 0.5 },
                {0, 1, 1, 0.5 },
                {0, 0.5, 0.5, 0.5 },
            };

            double product = _facilityEvaluator.VolumeDistanceCostProduct(_numDepartments, _facilityMatrix, volumeMatrix, costMatrix, _deptSizes);

            //VDC formula
            //sum(
            //  (abs(distanceX1 - distanceX2) + abs(distanceY1 - distanceY2)) 
            //  * volume(dept1,dept2) 
            //  * cost(dept1,dept2)
            //) for all dept combinations
            Assert.AreEqual(35, product);
        }
    }
}
