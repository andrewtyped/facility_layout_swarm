using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class FacilityLayoutSolution
    {
        public double VolumeDistanceCostProduct { get; }
        public TimeSpan RunTime { get; }
        public IReadOnlyDictionary<int, Position> DepartmentCentroids { get; }

        public int[] FinalLayout { get; }

        public FacilityLayoutSolution(double volumeDistanceCostProduct, TimeSpan runTime, IReadOnlyDictionary<int,Position> departmentCentroids, int[] finalLayout)
        {
            VolumeDistanceCostProduct = volumeDistanceCostProduct;
            RunTime = runTime;
            DepartmentCentroids = departmentCentroids;
            FinalLayout = finalLayout;
        }
    }
}
