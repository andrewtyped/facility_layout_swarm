using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class GridEventArgs : EventArgs
    {
        public GridSize GridSize { get; }

        public GridEventArgs(GridSize gridSize)
        {
            GridSize = gridSize;
        }
    }
}
