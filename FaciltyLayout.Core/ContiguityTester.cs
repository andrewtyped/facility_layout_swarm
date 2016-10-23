using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core
{
    public class ContiguityTester
    {
        public bool AllDepartmentsAreContiguous(int[,] facility)
        {
            var startPoints = new Dictionary<int, Tuple<int, int>>();

            var rows = facility.GetLength(0);
            var columns = facility.GetLength(1);

            for(var row = 0; row < rows; row++)
            {
                for(var column = 0; column < columns; column++)
                {
                    var currentDepartment = facility[row, column];

                    if (currentDepartment != 0)
                    {
                        //Contiguity for each department will be tested starting with the 
                        //last occurence of the department in the facility
                        startPoints[currentDepartment] = new Tuple<int, int>(row, column);

                        //Negate the value of the tile to show it has not been evaluated yet
                        facility[row, column] = -1 * currentDepartment;
                    }
                }
            }

            foreach(var department in startPoints.Keys)
            {
                var startRow = startPoints[department].Item1;
                var startColumn = startPoints[department].Item2;
                ContigHelper(startRow, startColumn, department, rows, columns, facility);
            }

            var isContiguous = AllTilesEvaluated(rows, columns, facility);

            return isContiguous;
        }

        public bool DepartmentIsContiguous(int department, int[,] facility)
        {
            var startRow = 0;
            var startColumn = 0;

            var rows = facility.GetLength(0);
            var columns = facility.GetLength(1);

            // Loop through all the facility tiles, negating the value of any
            // tiles belonging to the dept under test
            for (var row = 0; row < rows; row++)
            {
                for(var column = 0; column < columns; column++)
                {
                    if(facility[row,column] == department)
                    {
                        // The start point for the contiguity algorithm will be 
                        //the last occurence of the department on the tile grid
                        startRow = row;
                        startColumn = column;
                        facility[row, column] = -1 * facility[row, column];
                    }
                }
            }

            ContigHelper(startRow, startColumn, department, rows, columns, facility);

            var isContiguous = AllTilesEvaluated(rows, columns, facility);

            return isContiguous;
        }

        public void ContigHelper(int row, int column, int department, int rows, int columns, int[,] facility)
        {
            //terminate if we're outside the bounds of the facility
            if (row < 0 || rows <= row || column < 0 || columns <= column)
                return;
            //terminate if the tile does not belong to our department, or if the tile
            //has already been evalutated
            else if (facility[row, column] != -1 * department)
                return;
            else
            {
                //return the tile to a positive value
                facility[row, column] = -1 * facility[row, column];

                //evaluate the tiles to the left, right, bottom, and top of this tile.
                ContigHelper(row - 1, column, department, rows, columns, facility);
                ContigHelper(row + 1, column, department, rows, columns, facility);
                ContigHelper(row, column - 1, department, rows, columns, facility);
                ContigHelper(row, column + 1, department, rows, columns, facility);
            }
        }
        
        private bool AllTilesEvaluated(int rows, int columns, int[,] facility)
        {
            var allTilesEvaluated = true;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    //the presence of a negative tile indicates it wasn't evaluated by ContigHelper,
                    //and is therefore not part of a contiguous block of departments. Reset the tile
                    //and flag the facility as noncontiguous
                    if (facility[row, column] < 0)
                    {
                        facility[row, column] = -1 * facility[row, column];
                        allTilesEvaluated = false;
                    }
                }
            }

            return allTilesEvaluated;
        }
    }
}
