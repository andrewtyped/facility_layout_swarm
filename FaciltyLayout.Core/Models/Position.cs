using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct Position
    {
        public int Row { get; }
        public int Column { get; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is Position && this == (Position)obj;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() ^ Column.GetHashCode();
        }

        public static bool operator == (Position pos1, Position pos2)
        {
            return pos1.Row == pos2.Row && pos1.Column == pos2.Column;
        }

        public static bool operator != (Position pos1, Position pos2)
        {
            return pos1.Row != pos2.Row || pos1.Column == pos2.Column;
        }
    }
}
