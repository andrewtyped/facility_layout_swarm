using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core
{
    public class TermiteManager
    {
        private Random rand = new Random();
        private readonly FacilityLayoutModel facilityLayout;

        public event EventHandler<TileEventArgs> TermiteRemovedTile;

        public TermiteManager(FacilityLayoutModel facilityLayout)
        {
            if (facilityLayout == null)
                throw new ArgumentNullException(nameof(facilityLayout));

            this.facilityLayout = facilityLayout;
        }

        public List<Termites> ReleaseTheTermites(int numTermites, int typeRatio)
        {
            var termites = new List<Termites>();

            for(int i = 0; i < numTermites; i++)
            {
                var termite = BuildTermite(typeRatio);
                termites.Add(termite);
            }

            return termites;
        }

        private Termites BuildTermite(int typeRatio)
        {
            var termite = SetTermiteType(typeRatio);
            SetTermitePosition(termite);
            SetTermiteDirection(termite);

            return termite;
        }

        private Termites SetTermiteType(int typeRatio)
        {
            var number = rand.Next(0, typeRatio);

            if (number < typeRatio - 1)
                return new GreedyTermite();
            else
                return new ScholarTermite();
        }

        private void SetTermitePosition(Termites termite)
        {
            int row, column;

            do
            {
                row = rand.Next(0, facilityLayout.LayoutArea.Rows);
                column = rand.Next(0, facilityLayout.LayoutArea.Columns);
            } while (facilityLayout.IsTileFixed(row, column));

            termite.RowPos = row;
            termite.ColumnPos = column;
        }

        private void SetTermiteDirection(Termites termite)
        {
            int horizontalDir, verticalDir;

            do
            {
                horizontalDir = rand.Next(0, 5) - 2;
                verticalDir = rand.Next(0, 5) - 2;
            } while (horizontalDir == 0 && verticalDir == 0);

            termite.HorizDirection = horizontalDir;
            termite.VertDirection = verticalDir;
        }
    }
}
