using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core
{
    public class FacilityEvaluator
    {
        /// <summary>
        /// Calculates the current (x,y) center point for each department in the facility.
        /// </summary>
        /// <returns>
        ///     2-d array showing the center-x coord for each dept in the [,0] position, and the center-y
        ///     coord for each dept in the [,1] position. The [n,]th row denotes the nth department.
        /// </returns>
        internal double[,] CentroidCalculator(int numDepartments, int[,] facilityMatrix, int[] deptSizes)
        {
            double[,] centroids = new double[numDepartments + 1, 2];

            for (var k = 1; k <= numDepartments; k++)
            {
                var rowSums = 0;
                var columnSums = 0;

                for(var i = 0; i < facilityMatrix.GetLength(0); i++)
                {
                    for(var j = 0; j < facilityMatrix.GetLength(1); j++)
                    {
                        if(facilityMatrix[i,j] == k)
                        {
                            rowSums += i;
                            columnSums += j;
                        }
                    }
                }

                //TODO: This is always truncating the expression to an int. Is that right?
                centroids[k, 0] = rowSums / deptSizes[k];
                centroids[k, 1] = columnSums / deptSizes[k];
            }

            return centroids;
        }

        public double VolumeDistanceCostProduct(int numDepartments, int[,] facilityMatrix, int[,] volumeMatrix, double[,] costMatrix, int[] deptSizes)
        {
            double product = 0.0;
            var centroids = CentroidCalculator(numDepartments, facilityMatrix, deptSizes);

            for(var i = 1; i <= numDepartments; i++)
            {
                for(var j = 1; j <= numDepartments; j++)
                {
                    product += (Math.Abs(centroids[j, 0] - centroids[i, 0]) + Math.Abs(centroids[j, 1] - centroids[i, 1]))
                        * volumeMatrix[i, j]
                        * costMatrix[i, j];
                }
            }

            return product;

        }
    }
}
