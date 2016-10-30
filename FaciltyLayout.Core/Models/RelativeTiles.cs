using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public static class RelativeTiles
    {
        private static object locker = new Object();
        private static Random rand = new Random();

        public static IEnumerable<Position> Positions = new List<Position>
        {
            new Position(-1, 0),
            new Position(0, -1),
            new Position(0, 1),
            new Position(1, 0),
            new Position(-1, -1),
            new Position(-1, 1),
            new Position(1, -1),
            new Position(1, 1),
            new Position(0, 0)
        };

        /// <summary>
        /// Returns a randomized list of relative positions for all
        /// points in a 3x3 grid
        /// </summary>
        public static IEnumerable<Position> ShufflePositions()
        {
            var result = Positions.ToList();

            for(var i = 0; i < result.Count; i++)
            {
                lock (locker)
                {
                    Swap(result, i, rand.Next(i, result.Count));
                }
            }

            return result;
        }

        private static void Swap(List<Position> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
