using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct GridSize
    {
        public int Rows { get; }
        public int Columns { get; }

        public GridSize(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        //For compatibility with legacy system
        public int[] ToArray()
        {
            return new int[2] { Rows, Columns };
        }

        public override bool Equals(object obj)
        {
            return obj is GridSize && this == (GridSize)obj;
        }

        public override int GetHashCode()
        {
            return Rows.GetHashCode() ^ Columns.GetHashCode();
        }

        public static bool operator ==(GridSize size1, GridSize size2)
        {
            return size1.Rows == size2.Rows && size1.Columns == size2.Columns;
        }

        public static bool operator !=(GridSize size1, GridSize size2)
        {
            return size1.Rows != size2.Rows || size1.Columns != size2.Columns;
        }
    }
}
