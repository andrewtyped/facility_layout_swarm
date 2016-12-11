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
        public int Id { get; }
        public double VolumeDistanceCostProduct { get; }
        public TimeSpan RunTime { get; }
        public IReadOnlyDictionary<int, DoublePosition> DepartmentCentroids { get; }
        public GridSize FacilitySize { get; }

        public int[] FinalLayout { get; }

        public FacilityLayoutSolution()
        {
            DepartmentCentroids = new ReadOnlyDictionary<int, DoublePosition>(new Dictionary<int, DoublePosition>());
            FinalLayout = new int[1];
        }

        public FacilityLayoutSolution(int id, double volumeDistanceCostProduct, TimeSpan runTime, IReadOnlyDictionary<int,DoublePosition> departmentCentroids, int[] finalLayout, GridSize facilitySize)
        {
            Id = id;
            VolumeDistanceCostProduct = volumeDistanceCostProduct;
            RunTime = runTime;
            DepartmentCentroids = departmentCentroids;
            FinalLayout = finalLayout;
            FacilitySize = facilitySize;
        }
    }
}
