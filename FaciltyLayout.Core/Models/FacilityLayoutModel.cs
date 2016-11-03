using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace FaciltyLayout.Core.Models
{
    public class FacilityLayoutModel
    {
        private readonly Random random = new Random();
        private readonly FacilityStats facilityStats;
        public GridSize LayoutArea { get; }

        public int[,] Facility { get; private set; }

        public bool[,] LockedTiles { get; private set; }

        public FacilityLayoutModel(FacilityStats facilityStats)
        {
            this.facilityStats = facilityStats;
            LayoutArea = SetLayoutArea();
        }

        private GridSize SetLayoutArea()
        {
            var facilitySize = facilityStats.FacilitySize;

            var layoutRows = (int)Round(Sqrt(2 * Pow(facilitySize.Rows, 2)));
            var layoutColumns = (int)Round(Sqrt(2 * Pow(facilitySize.Columns, 2)));

            return new GridSize(layoutRows, layoutColumns);
        }

        public void InitializeDepartmentTiles()
        {
            Facility = new int[LayoutArea.Rows, LayoutArea.Columns];
            LockedTiles = new bool[LayoutArea.Rows, LayoutArea.Columns];
            PlaceTilesForFixedDepartments();
            PlaceTilesForLooseDepartments();
        }

        private void PlaceTilesForFixedDepartments()
        {
            foreach(var department in facilityStats.Departments.Where(d => d.IsLocationFixed))
            {
                for (int i = (int)department.TopLeft?.Row; i <= department.BottomRight?.Row; i++)
                {
                    for (int j = (int)department.TopLeft?.Column; j <= department.BottomRight?.Column; j++)
                    {
                        Facility[i - 1, j - 1] = department.Id;
                    }
                }
            }
        }

        private void PlaceTilesForLooseDepartments()
        {
            foreach(var department in facilityStats.Departments.Where(d => !d.IsLocationFixed))
            {
                PlaceTilesForLooseDepartment(department);
            }
        }

        private void PlaceTilesForLooseDepartment(Department department)
        {
            for (var tilesToPlace = department.Area; tilesToPlace > 0; tilesToPlace--)
            {
                Position desiredPostion;

                do
                {
                    desiredPostion = new Position(random.Next(0, LayoutArea.Rows), random.Next(LayoutArea.Columns));
                } while (IsTileAssigned(desiredPostion));

                Facility[desiredPostion.Row, desiredPostion.Column] = department.Id;
            }
        }

        public void LockDeptTiles(int department, int? rows = null, int? columns = null)
        {
            int irows = rows ?? LayoutArea.Rows;
            int icolumns = columns ?? LayoutArea.Columns;

            Parallel.For(0, irows, (i) =>
             {
                 for (int j = 0; j < icolumns; j++)
                 {
                     if (Facility[i, j] == department)
                         LockedTiles[i, j] = true;
                 }
             });
        }

        public void UnlockTiles(int? rows = null, int? columns = null)
        {
            LockedTiles = new bool[LayoutArea.Rows, LayoutArea.Columns];
        }

        public int GetTile(int row, int column)
        {
            return Facility[row, column];
        }

        public int GetTile(Position position)
        {
            return Facility[position.Row, position.Column];
        }

        public void SetTile(int row, int column, int department)
        {
            Facility[row, column] = department;
            LockedTiles[row, column] = false;
            OnTilePlaced(new Position(row, column), department);
        }

        public void SetTile(Position position, int department)
        {
            Facility[position.Row, position.Column] = department;
            LockedTiles[position.Row, position.Column] = false;
            OnTilePlaced(position, department);
        }

        public void SetTileEmpty(int row, int column)
        {
            Facility[row, column] = 0;
            LockedTiles[row, column] = true;
            OnTileRemoved(new Position(row, column));
        }

        public void SetTileEmpty(Position position)
        {
            Facility[position.Row, position.Column] = 0;
            LockedTiles[position.Row, position.Column] = true;
            OnTileRemoved(position);
        }

        /// <summary>
        /// Returns true if the position is on the board and not over a fixed tile,
        /// false otherwise.
        /// </summary>
        public bool IsPositionValid(Position position, int? maxRow = null, int? maxColumn = null)
        {
            return position.Row >= 0 && position.Row < (maxRow ?? LayoutArea.Rows) &&
                position.Column >= 0 && position.Column < (maxColumn ?? LayoutArea.Columns) &&
                IsTileFixed(position) == false;
        }

        public bool IsTileFixed(int row, int column)
        {
            var departmentId = Facility[row, column];
            var department = facilityStats.GetDepartment(departmentId);
            return department.IsLocationFixed;
        }

        public bool IsTileFixed(Position position)
        {
            var departmentId = Facility[position.Row, position.Column];
            var department = facilityStats.GetDepartment(departmentId);
            return department.IsLocationFixed;
        }

        public bool IsTileAssigned(int row, int column)
        { 
            return Facility[row, column] != 0;
        }

        public bool IsTileAssigned(Position position)
        {
            return Facility[position.Row, position.Column] != 0;
        }

        public bool IsTileLocked(Position position)
        {
            return LockedTiles[position.Row, position.Column];
        }

        public event EventHandler<TileEventArgs> TilePlaced;
        public event EventHandler<TileEventArgs> TileRemoved;
        public void OnTilePlaced(Position position, int department)
        {
            TilePlaced?.Invoke(this, new TileEventArgs(position, department));
        }

        public void OnTileRemoved(Position position)
        {
            TileRemoved?.Invoke(this, new TileEventArgs(position, 0));
        }
    }
}
