using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core
{
    public class ContiguityTester
    {
        /// <summary>
        /// Represents the relative positions of tiles directly to the left, right, top, and bottom
        /// of a given tile.
        /// </summary>
        private readonly List<Position> _adjacencyTestPositions = new List<Position>()
        {
            new Position(-1,0),
            new Position(0,-1),
            new Position(0,1),
            new Position(1,0),
            new Position(-1,-1),
            new Position(-1,1),
            new Position(1,-1),
            new Position(1,1)
        };

        public bool AdjacentTilesContainSameDepartment(int dept, int row, int column, FacilityLayoutModel facility, int[] deptSizes)
        {
            var adjTilesContainSameDept = false;

            foreach(var testPoint in _adjacencyTestPositions)
            {
                var testRow = row - testPoint.Row;
                var testColumn = column - testPoint.Column;

                if (0 <= testColumn && testColumn < facility.LayoutArea.Columns)
                    if (0 <= testRow && testRow < facility.LayoutArea.Rows)
                    {
                        if (facility.GetTile(testRow,testColumn) == dept)
                            adjTilesContainSameDept = true;
                        else if (deptSizes[dept] == 1)
                            adjTilesContainSameDept = true;
                    }
            }

            return adjTilesContainSameDept;
        }

        /// <summary>
        /// Returns the number of adjacent tiles that are identical to the tile at the test position. 
        /// A high value makes the tile less likely to move.
        /// </summary>
        public int CountAdjacentTilesOfSameDepartment(Position testPos, FacilityLayoutModel facility)
        {
            var numberAdjTiles = 0;

            foreach(var adjPoint in _adjacencyTestPositions)
            { 
                var testRow = testPos.Row - adjPoint.Row;
                var testColumn = testPos.Column - adjPoint.Column;

                if (0 <= testColumn && testColumn < facility.LayoutArea.Columns)
                    if (0 <= testRow && testRow < facility.LayoutArea.Rows)
                    {
                        if (facility.GetTile(testRow, testColumn) == facility.GetTile(testPos.Row, testPos.Column))
                            numberAdjTiles++;
                    }
            }

            return numberAdjTiles;
        }

        public bool AllDepartmentsAreContiguous(FacilityLayoutModel facility)
        {
            var startPoints = new Dictionary<int, Tuple<int, int>>();

            for(var row = 0; row < facility.LayoutArea.Rows; row++)
            {
                for(var column = 0; column < facility.LayoutArea.Columns; column++)
                {
                    var currentDepartment = facility.GetTile(row,column);

                    if (currentDepartment != 0)
                    {
                        //Contiguity for each department will be tested starting with the 
                        //last occurence of the department in the facility
                        startPoints[currentDepartment] = new Tuple<int, int>(row, column);

                        //Negate the value of the tile to show it has not been evaluated yet
                        facility.MarkTile(row, column);
                    }
                }
            }

            foreach(var department in startPoints.Keys)
            {
                var startRow = startPoints[department].Item1;
                var startColumn = startPoints[department].Item2;
                ContigHelper(startRow, startColumn, department, facility);
            }

            var isContiguous = AllTilesEvaluated(facility);

            return isContiguous;
        }

        public bool DepartmentIsContiguous(int department, FacilityLayoutModel facility)
        {
            var startRow = 0;
            var startColumn = 0;


            // Loop through all the facility tiles, negating the value of any
            // tiles belonging to the dept under test
            for (var row = 0; row < facility.LayoutArea.Rows; row++)
            {
                for(var column = 0; column < facility.LayoutArea.Columns; column++)
                {
                    if(facility.GetTile(row,column) == department)
                    {
                        // The start point for the contiguity algorithm will be 
                        //the last occurence of the department on the tile grid
                        startRow = row;
                        startColumn = column;
                        facility.MarkTile(row, column);
                    }
                }
            }

            ContigHelper(startRow, startColumn, department, facility);

            var isContiguous = AllTilesEvaluated(facility);

            return isContiguous;
        }

        private void ContigHelper(int row, int column, int department, FacilityLayoutModel facility)
        {
            //terminate if we're outside the bounds of the facility
            if (row < 0 || facility.LayoutArea.Rows <= row || column < 0 || facility.LayoutArea.Columns <= column)
                return;
            //terminate if the tile does not belong to our department, or if the tile
            //has already been evalutated
            else if (!facility.IsTileMarked(row,column,department))
                return;
            else
            {
                //return the tile to a positive value
                facility.UnmarkTile(row, column);

                //evaluate the tiles to the left, right, bottom, and top of this tile.
                ContigHelper(row - 1, column, department, facility);
                ContigHelper(row + 1, column, department, facility);
                ContigHelper(row, column - 1, department, facility);
                ContigHelper(row, column + 1, department, facility);
            }
        }
        
        private bool AllTilesEvaluated(FacilityLayoutModel facility)
        {
            var allTilesEvaluated = true;

            for (var row = 0; row < facility.LayoutArea.Rows; row++)
            {
                for (var column = 0; column < facility.LayoutArea.Columns; column++)
                {
                    //the presence of a negative tile indicates it wasn't evaluated by ContigHelper,
                    //and is therefore not part of a contiguous block of departments. Reset the tile
                    //and flag the facility as noncontiguous
                    if (facility.IsTileMarked(row,column))
                    {
                        facility.UnmarkTile(row, column);
                        allTilesEvaluated = false;
                    }
                }
            }

            return allTilesEvaluated;
        }
    }
}
