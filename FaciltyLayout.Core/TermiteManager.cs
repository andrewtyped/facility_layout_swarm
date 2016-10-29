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

        public bool[,] OwnedTiles { get; private set; }

        public event EventHandler<TermiteActionEventArgs> TermiteRemovedTile;

        public TermiteManager(FacilityLayoutModel facilityLayout)
        {
            if (facilityLayout == null)
                throw new ArgumentNullException(nameof(facilityLayout));

            this.facilityLayout = facilityLayout;
        }

        public List<Termites> ReleaseTheTermites(int numTermites, int typeRatio)
        {
            var termites = new List<Termites>();

            OwnedTiles = new bool[facilityLayout.LayoutArea.Rows, facilityLayout.LayoutArea.Columns];

            for(int i = 0; i < numTermites; i++)
            {
                var termite = BuildTermite(typeRatio);
                termites.Add(termite);
            }

            return termites;
        }

        private Termites BuildTermite(int typeRatio)
        {
            var termite = new Termites();
            SetTermitePosition(termite);
            SetTermiteType(termite, typeRatio);
            SetTermiteDirection(termite);
            TakeInitialTile(termite);

            return termite;
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

        private void SetTermiteType(Termites termite, int typeRatio)
        {
            var number = rand.Next(0, typeRatio);
            termite.TermiteType = number < typeRatio - 1 ? 0 : 1;
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

        private void TakeInitialTile(Termites termite)
        {
            var department = facilityLayout.GetTile(termite.Position);

            if(department != 0 
                && OwnedTiles[termite.RowPos,termite.ColumnPos] == false)
            {
                termite.HasTile = true;
                OwnedTiles[termite.RowPos, termite.ColumnPos] = true;
                termite.TileDept = department;
                facilityLayout.SetTileEmpty(termite.Position); //TODO: This is not an appropriate place for this state change. move it into the termite model
                OnTermiteRemovedTile(termite);
            }
        }

        private void OnTermiteRemovedTile(Termites termite)
        {
            TermiteRemovedTile?.Invoke(this, new TermiteActionEventArgs(termite));
        }
    }
}
