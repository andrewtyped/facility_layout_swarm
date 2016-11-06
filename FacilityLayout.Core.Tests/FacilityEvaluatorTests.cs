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
    public class FacilityEvaluatorTests
    {
        private FacilityEvaluator _facilityEvaluator;
        private int[,] _facilityMatrix;
        private int[,] _volumeMatrix;
        private double[,] _costMatrix;
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

            _volumeMatrix = new int[4, 4]
            {
                {0, 0, 0, 0 },
                {0, 0, 0, 10 },
                {0, 5, 0, 20 },
                {0, 10, 0, 0 },
            };

            _costMatrix = new double[4, 4]
            {
                {0, 0, 0, 0 },
                {0, 1, 1, 0.5 },
                {0, 1, 1, 0.5 },
                {0, 0.5, 0.5, 0.5 },
            };

            _numDepartments = 3;
            _deptSizes = new int[4] { 0, 3, 3, 3 };
            _facilityEvaluator = new FacilityEvaluator();
        }

        [Test]
        public void CentroidCalculator_Calculates_Centers_Of_Departments()
        {
            var facilityStats = new FacilityStats(
                3,
                new GridSize(3, 3),
                new List<Department>()
                {
                    new Department(0,0,false),
                    new Department(1,3,false),
                    new Department(2,3,false),
                    new Department(3,3,false),
                },
                _volumeMatrix,
                _costMatrix);

            var centroids = _facilityEvaluator.CentroidCalculator(facilityStats, new FacilityLayoutModel(_facilityMatrix));

            Assert.AreEqual(0, centroids[1].Row);
            Assert.AreEqual(2, centroids[1].Column);
            Assert.AreEqual(1, centroids[2].Row);
            Assert.AreEqual(2, centroids[2].Column);
            Assert.AreEqual(2, centroids[3].Row);
            Assert.AreEqual(2, centroids[3].Column);
        }

        [Test]
        public void VDCProduct_Calculates_Volume_Distance_Cost_Product_Of_Facility()
        {
            var facilityStats = new FacilityStats(
                3,
                new GridSize(3, 3),
                new List<Department>()
                {
                    new Department(0,0,false),
                    new Department(1,3,false),
                    new Department(2,3,false),
                    new Department(3,3,false),
                },
                _volumeMatrix,
                _costMatrix);

            double product = _facilityEvaluator.VolumeDistanceCostProduct(facilityStats, new FacilityLayoutModel(_facilityMatrix));

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
