using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class TermiteActionEventArgs
    {
        public Position Position { get; }
        public int DepartmentId { get; }
        public TermiteActionEventArgs(Termites termite)
        {
            Position = termite.Position;
            DepartmentId = termite.TileDept;
        }
    }
}
