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
        internal IReadOnlyDictionary<int,DoublePosition> CentroidCalculator(FacilityStats facilityStats, FacilityLayoutModel facility)
        {
            var centroids = new Dictionary<int, DoublePosition>();

            for (var k = 1; k <= facilityStats.DepartmentCount; k++)
            {
                var rowSums = 0;
                var columnSums = 0;

                for(var i = 0; i < facility.LayoutArea.Rows; i++)
                {
                    for(var j = 0; j < facility.LayoutArea.Columns; j++)
                    {
                        if(facility.GetTile(i,j) == k)
                        {
                            rowSums += i;
                            columnSums += j;
                        }
                    }
                }

                centroids[k] = new DoublePosition((double)rowSums / facilityStats.DepartmentSizes[k],
                                            (double)columnSums / facilityStats.DepartmentSizes[k]);
            }

            return centroids;
        }

        public double VolumeDistanceCostProduct(FacilityStats facilityStats, FacilityLayoutModel facility)
        {
            double product = 0.0;
            var centroids = CentroidCalculator(facilityStats, facility);

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
