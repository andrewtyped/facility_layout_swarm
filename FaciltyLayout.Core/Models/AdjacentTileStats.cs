using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct AdjacentTileStats
    {
        public int Number { get; set; }
        public bool OpenCorner { get; set; }
        public bool RelevantTiles { get; set; }
        public bool ContigTiles { get; set; }
    }
}
