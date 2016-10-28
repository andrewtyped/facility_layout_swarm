using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct FlowStats
    {
        public int[] Flows { get; set; }
        public int FlowSum { get; set; }
        public int NumRelations { get; set; }
    }
}
