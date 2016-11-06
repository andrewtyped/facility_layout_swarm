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
        public GridSize LayoutArea { get; private set; }

        public int[,] Facility { get; private set; }

        public bool[,] LockedTiles { get; private set; }

        //For testing
        internal FacilityLayoutModel(int[,] facility)
        {
            Facility = facility;
            LayoutArea = new GridSize(facility.GetLength(0), facility.GetLength(1));
        }
        public FacilityLayoutModel(FacilityStats facilityStats)
        {
            this.facilityStats = facilityStats;
            LayoutArea = SetLayoutArea();
        }

        private GridSize SetLayoutArea()
        {
            /*
            var facilitySize = facilityStats.FacilitySize;

            var layoutRows = (int)Round(Sqrt(2 * Pow(facilitySize.Rows, 2)));
            var layoutColumns = (int)Round(Sqrt(2 * Pow(facilitySize.Columns, 2)));

            return new GridSize(layoutRows, layoutColumns);
            */
            return new GridSize(facilityStats.FacilitySize.Rows + 1, facilityStats.FacilitySize.Columns + 1);
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

        public void ReduceLayoutArea(IList<Termites> termites, int gravStart, ref int loopCounter)
        {
            UnlockTiles();

            var currentRows = LayoutArea.Rows;
            var currentColumns = LayoutArea.Columns;
            LayoutArea = new GridSize(LayoutArea.Rows - 1, LayoutArea.Columns - 1);

            var i = 0;

            for (int x = 0; x < currentRows; x++)
            {
                if (IsTileAssigned(x, currentColumns - 1))
                {
                    termites[i].TakeTile(this, new Position(x, currentColumns - 1));
                    i++;
                }
            }

            for (int y = 0; y < currentColumns - 1; y++)
            {
                if (IsTileAssigned(currentRows - 1, y))
                {
                    termites[i].TakeTile(this, new Position(currentRows - 1, y));
                    i++;
                }
            }

            Parallel.ForEach(termites, (termite) =>
            {
                termite.Move(this);
            });

            bool TotalContig = false;
            bool ContigIndicator = false;
            int n = 0;
            int Phase1Decay = 10;
            var contiguityTester = new ContiguityTester();

            for(int refreshcounter = 0;  !TotalContig; refreshcounter++)
            {
                if (currentRows - 1 == facilityStats.FacilitySize.Rows && currentColumns - 1 == facilityStats.FacilitySize.Columns)
                    break;

                if(refreshcounter % 400 == 0) //HACK: Magic number
                {
                    UnlockTiles();

                    for(int y = 0; y <= 50; y++) //HACK: Magic number
                    {
                        foreach(var termite in termites)
                        {
                            termite.MoveTile(this, this.facilityStats, new ContiguityTester());
                            //TODO: Old code refreshed tile UI here
                        }
                    }
                }

                foreach (var termite in termites)
                {
                    termite.MoveTile(this, this.facilityStats, new ContiguityTester());
                    //TODO: Old code refreshed tile UI here
                }

                loopCounter++;

                TotalContig = contiguityTester.AllDepartmentsAreContiguous(this);

                if(n < termites.Count)
                {
                    if(loopCounter % Phase1Decay == 0 && loopCounter >= gravStart - Round((double)gravStart / 4.0, 0)) //HACK: Magic number
                    {
                        termites[n] = termites[n].ChangeType<GreedyTermite>();
                        n++;
                    }
                }

                if(loopCounter >= gravStart + Round(gravStart /2.0,0))
                {
                    for(int a = 1; a <= facilityStats.DepartmentCount; a++)
                    {
                        ContigIndicator = contiguityTester.DepartmentIsContiguous(a, this);

                        if (!ContigIndicator)
                            TotalContig = false;
                        else
                            LockDeptTiles(a);
                    }
                }
            }

            
        }

        public void LockDeptTiles(int department)
        {
            Parallel.For(0, LayoutArea.Rows, (i) =>
             {
                 for (int j = 0; j < LayoutArea.Columns; j++)
                 {
                     if (Facility[i, j] == department)
                         LockedTiles[i, j] = true;
                 }
             });
        }

        public void UnlockTiles()
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

        public bool IsTileMarked(int row, int column)
        {
            return Facility[row, column] < 0;
        }
        
        public bool IsTileMarked(int row, int column, int department)
        {
            return Facility[row, column] == -1 * department;
        }

        /// <summary>
        /// Negates the id stored in the facility at the specified row and column. Useful for checking contiguity
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void MarkTile(int row, int column)
        {
            Facility[row, column] = -1 * Facility[row, column];
        }

        /// <summary>
        /// Returns the id stored in the facility at the specified row and column to a non-negative integer
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void UnmarkTile(int row, int column)
        {
            Facility[row, column] = Abs(Facility[row, column]);
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
        public bool IsPositionValid(Position position)
        {
            return position.Row >= 0 && position.Row < LayoutArea.Rows &&
                position.Column >= 0 && position.Column < LayoutArea.Columns &&
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
        private void OnTilePlaced(Position position, int department)
        {
            TilePlaced?.Invoke(this, new TileEventArgs(position, department));
        }

        private void OnTileRemoved(Position position)
        {
            TileRemoved?.Invoke(this, new TileEventArgs(position, 0));
        }
    }
}
