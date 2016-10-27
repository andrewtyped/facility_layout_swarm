using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct Department
    {
        public int Id { get; }
        public int Area { get; }
        public bool IsLocationFixed { get; }
        public Position? TopLeft { get; }
        public Position? BottomRight { get; }

        //Provide array to fixed positions for legacy code. 
        public int[,] FixedPositions
        {
            get
            {
                //Always set second index to 0 for legacy purposes
                var pos = new int[4, 1];
                pos[0, 0] = TopLeft?.Row ?? -1;
                pos[1, 0] = TopLeft?.Column ?? -1;
                pos[2, 0] = BottomRight?.Row ?? -1;
                pos[3, 0] = BottomRight?.Column ?? -1;

                return pos;
            }
        }

        public Department(int id, int area, bool isLocationFixed, Position? topLeft = null, Position? bottomRight = null)
        {
            Id = id;
            Area = area;
            IsLocationFixed = isLocationFixed;
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public override bool Equals(object obj)
        {
            return obj is Department && this == (Department)obj;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Department department1, Department department2)
        {
            return department1.Id == department2.Id;
        }

        public static bool operator !=(Department department1, Department department2)
        {
            return department1.Id != department2.Id;
        }
    }
}
