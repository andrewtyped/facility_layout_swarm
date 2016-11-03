using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class TileEventArgs
    {
        public Position Position { get; }
        public int DepartmentId { get; }
        public TileEventArgs(Position position, int departmentId)
        {
            Position = position;
            DepartmentId = departmentId;
        }
    }
}
