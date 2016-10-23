using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct DepartmentStats
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Area { get; set; }
        public int Up { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Right { get; set; }
    }
}
