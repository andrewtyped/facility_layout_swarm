using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class FacilityInitializedEventArgs : EventArgs
    {
        public GridSize LayoutArea { get; }
        public int DepartmentCount { get; }

        public int[,] Facility { get; }

        public FacilityInitializedEventArgs(FacilityLayoutModel facilityLayoutModel, FacilityStats facilityStats)
        {
            LayoutArea = facilityLayoutModel.LayoutArea;
            DepartmentCount = facilityStats.DepartmentCount;

            Facility = new int[LayoutArea.Rows, LayoutArea.Columns];

            for(int i = 0; i < LayoutArea.Rows; i++)
            {
                for(int j = 0; j < LayoutArea.Columns; j++)
                {
                    Facility[i, j] = facilityLayoutModel.Facility[i, j];
                }
            }
        }
    }
}
