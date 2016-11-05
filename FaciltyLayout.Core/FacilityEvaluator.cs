using FaciltyLayout.Core.Models;
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
        internal IReadOnlyDictionary<int,Position> CentroidCalculator(FacilityStats facilityStats, int[,] facilityMatrix)
        {
            var centroids = new Dictionary<int, Position>();

            for (var k = 1; k <= facilityStats.DepartmentCount; k++)
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
                centroids[k] = new Position(rowSums / facilityStats.DepartmentSizes[k],
                                            columnSums / facilityStats.DepartmentSizes[k]);
            }

            return centroids;
        }

        public double VolumeDistanceCostProduct(FacilityStats facilityStats, int[,] facilityMatrix)
        {
            double product = 0.0;
            var centroids = CentroidCalculator(facilityStats, facilityMatrix);

            for(var i = 1; i <= facilityStats.DepartmentCount; i++)
            {
                for(var j = 1; j <= facilityStats.DepartmentCount; j++)
                {
                    product += (Math.Abs(centroids[j].Row - centroids[i].Row) + Math.Abs(centroids[j].Column - centroids[i].Column))
                        * facilityStats.VolumeMatrix[i, j]
                        * facilityStats.CostMatrix[i, j];
                }
            }

            return product;

        }
    }
}
