using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct DoublePosition
    {
        public double Row { get; }
        public double Column { get; }

        public DoublePosition(double row, double column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is DoublePosition && this == (DoublePosition)obj;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() ^ Column.GetHashCode();
        }

        public static bool operator ==(DoublePosition pos1, DoublePosition pos2)
        {
            return pos1.Row == pos2.Row && pos1.Column == pos2.Column;
        }

        public static bool operator !=(DoublePosition pos1, DoublePosition pos2)
        {
            return pos1.Row != pos2.Row || pos1.Column != pos2.Column;
        }
    }
}
