using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class FacilityStats
    {
        public int DepartmentCount { get; }
        public GridSize FacilitySize { get; }
        public IEnumerable<Department> Departments {get;}
        public int[,] VolumeMatrix { get; }
        public double[,] CostMatrix { get; }

        private int[] _departmentSizes;

        //Provide conversion to array for legacy code
        public int[] DepartmentSizes
        {
            get
            {
                if (_departmentSizes == null)
                    _departmentSizes = Departments
                        .OrderBy(d => d.Id)
                        .Select(d => d.Area)
                        .ToArray();

                return _departmentSizes;
            }
        }

        private bool[] _isDepartmentLocationFixed;

        //Provide conversion to array for legacy code
        public bool[] IsDepartmentLocationFixed
        {
            get
            {
                if (_isDepartmentLocationFixed == null)
                    _isDepartmentLocationFixed = Departments
                        .OrderBy(d => d.Id)
                        .Select(d => d.IsLocationFixed)
                        .ToArray();

                return _isDepartmentLocationFixed;
            }
        }

        private int[,] _fixedDepartmentLocations;

        public int[,] FixedDepartmentLocations
        {
            get
            {
                if (_fixedDepartmentLocations == null)
                    _fixedDepartmentLocations = Departments
                            .Single(d => d.IsLocationFixed)
                            .FixedPositions;

                return _fixedDepartmentLocations;
            }
        }

        private int[,] weightedVolumeMatrix;
        /// <summary>
        /// Uses the Volume and Department size matrices to establish
        /// weighted relationships between departments. 
        /// </summary>
        public int[,] WeightedVolumeMatrix
        {
            get
            {
                if(weightedVolumeMatrix == null)
                {
                    weightedVolumeMatrix = new int[DepartmentCount + 1, DepartmentCount + 1];

                    for(int i = 1; i <= DepartmentCount; i++)
                    {
                        for(int j = 1; j <= DepartmentCount; j++)
                        {
                            weightedVolumeMatrix[i, j] = (int)Math.Round((Math.Pow(VolumeMatrix[i, j],2)) / ((DepartmentSizes[j] + DepartmentSizes[i]) / 2.0),0);
                        }
                    }
                }

                return weightedVolumeMatrix;
            }
        }

        private FlowStats[] flows;

        public FlowStats[] Flows
        {
            get
            {
                if(flows == null)
                {
                    flows = CalculateDepartmentFlowStats();
                }

                return flows;
            }
        }


        public FacilityStats(int departmentCount, GridSize facilitySize, IEnumerable<Department> departments, int[,] volumeMatrix, double[,] costMatrix)
        {
            DepartmentCount = departmentCount;
            FacilitySize = facilitySize;
            Departments = departments;
            VolumeMatrix = volumeMatrix;
            CostMatrix = costMatrix;
        }

        private FlowStats[] CalculateDepartmentFlowStats()
        {
            var flowStats = new FlowStats[DepartmentCount + 1];

            for(var i = 1; i <= DepartmentCount; i++)
            {
                flowStats[i].Flows = new int[DepartmentCount + 1];

                for(var j = 1; j <= DepartmentCount; j++)
                {
                    //The roundabout method of getting to an int here seems to match VB most closely
                    var flow = (int)(Math.Round(WeightedVolumeMatrix[j, i] / CostMatrix[j, i], 0)
                        + Math.Round(WeightedVolumeMatrix[i, j] / CostMatrix[i, j],0));

                    if(flow > 0)
                    {
                        flowStats[i].Flows[j] = flow;
                        flowStats[i].NumRelations++;
                        flowStats[i].FlowSum += flow;
                    }
                }
            }

            return flowStats;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Basic Facility Stats");
            sb.AppendLine($"Department Count: {DepartmentCount}");
            sb.AppendLine($"Rows: {FacilitySize.Rows}");
            sb.AppendLine($"Columns: {FacilitySize.Columns}");
            sb.AppendLine("Department Sizes:");

            foreach(var department in Departments)
            {
                sb.AppendLine($"{department.Id},{department.Area}");
            }

            sb.AppendLine("Fixed Departments:");

            foreach(var department in Departments)
            {
                if (department.IsLocationFixed)
                    sb.AppendLine($"{department.Id}");
            }

            return sb.ToString();
        }


    }
}
