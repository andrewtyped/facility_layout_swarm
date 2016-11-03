using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class Termites
    {
        //Share rand between all termites to avoid bias due to creatig too many
        //randoms in a small time frame
        protected static Random rand = new Random();

        /// <summary>
        /// How far up/down should I go each turn
        /// </summary>
        public int VertDirection { get; set; } 
        /// <summary>
        /// How far left/right should I go each turn?
        /// </summary>
        public int HorizDirection { get; set; }
        /// <summary>
        /// What column am I in?
        /// </summary>
        public int ColumnPos { get; set; }
        /// <summary>
        /// What row am I in?
        /// </summary>
        public int RowPos { get; set; }
        /// <summary>
        /// Do I have a tile ?
        /// </summary>
        public bool HasTile { get; set; }
        /// <summary>
        /// What kind of tile do I have
        /// </summary>
        public int TileDept { get; set; }

        /// <summary>
        /// In what order should I look around me for empty spaces or tiles of 
        /// the same department as I am holding?
        /// </summary>
        public IReadOnlyList<Position> TileSearchOrder { get; private set; }
        public IReadOnlyList<Position> EmptyTileSearchOrder { get; private set; }

        public Position Position
        {
            get
            {
                return new Position(RowPos, ColumnPos);
            }
        }

        /// <summary>
        /// Returns the next position of the termite given its current position 
        /// and horizontal/vertical directions
        /// </summary>
        public Position NextPosition
        {
            get
            {
                //Yes, this is right. Think about it. Rows top to bottom, columns left to right
                return new Position(RowPos + VertDirection, ColumnPos + HorizDirection); 
            }
        }

        public Termites()
        {
            //The range 5 to 9 appears to be magic for the application. Not having the termite
            //check every surrounding tile keeps the tiles from being too "sticky" - not having
            //enough mobility to overcome local optima that prevent departments from joining
            //contiguously. On the other hand, not checking enough tiles makes the tiles move
            //too freely, and they never form reliable clusters.
            var rn = rand.Next(5, 10);
            TileSearchOrder = RelativeTiles.ShufflePositions().Take(rn).ToList();
            EmptyTileSearchOrder = RelativeTiles.ShufflePositions().Take(rn).ToList();
        }

        public void Move(FacilityLayoutModel facility, int? maxRow = null, int? maxColumn = null)
        {
            if (!IsDirectionValid(facility, maxRow, maxColumn))
                ChooseNewDirection(facility, maxRow, maxColumn);

            RowPos = NextPosition.Row;
            ColumnPos = NextPosition.Column;
        }

        public void ChooseNewDirection(FacilityLayoutModel facility, int? maxRow = null, int? maxColumn = null)
        {
            do
            {
                HorizDirection = rand.Next(0, 5) - 2;
                VertDirection = rand.Next(0, 5) - 2;
            } while (!IsDirectionValid(facility, maxRow, maxColumn));
        }

        private bool IsDirectionValid(FacilityLayoutModel facility, int? maxRow = null, int? maxColumn = null)
        {
            return !(HorizDirection == 0 && VertDirection == 0) &&
                facility.IsPositionValid(NextPosition, maxRow, maxColumn);
        }

        public void DropTile(FacilityLayoutModel facilityLayoutModel)
        {
            facilityLayoutModel.SetTile(Position, TileDept);
            HasTile = false;
            TileDept = 0;
        }

        public void DropTile(FacilityLayoutModel facilityLayoutModel, int row, int column)
        {
            facilityLayoutModel.SetTile(row, column, TileDept);
            HasTile = false;
            TileDept = 0;
        }

        public virtual void FindDropPoint(FacilityLayoutModel facilityLayoutModel, FacilityStats facilityStats, int rows, int columns)
        {

        }

        public void TakeTile(FacilityLayoutModel facilityLayoutModel)
        {
            HasTile = true;
            TileDept = facilityLayoutModel.GetTile(Position);
            facilityLayoutModel.SetTileEmpty(Position);
        }

        public T ChangeType<T>() where T:Termites, new()
        {
            var newTermite = new T();
            newTermite.RowPos = RowPos;
            newTermite.ColumnPos = ColumnPos;
            newTermite.HorizDirection = HorizDirection;
            newTermite.VertDirection = VertDirection;
            newTermite.HasTile = HasTile;
            newTermite.TileDept = TileDept;

            return newTermite;
        }
    }
}
