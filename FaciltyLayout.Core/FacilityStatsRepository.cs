using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core
{
    public class FacilityStatsRepository
    {
        private const string COMMENT_MARKER = @"//";
        private const char DELIMITER = ',';

        private readonly string pathToDataFile;
        public FacilityStatsRepository(string pathToDataFile)
        {
            if (pathToDataFile == null)
                throw new ArgumentNullException(nameof(pathToDataFile));

            if (!File.Exists(pathToDataFile))
                throw new FileNotFoundException($"Invalid path: [{pathToDataFile}]");

            this.pathToDataFile = pathToDataFile;
        }

        public FacilityStats Load()
        {
            int departmentCount;
            GridSize facilitySize;
            IEnumerable<Department> departments;
            int[,] volumeMatrix;
            double[,] costMatrix;

            using (var reader = new StreamReader(pathToDataFile))
            {
                departmentCount = GetDepartmentCount(reader);
                facilitySize = GetFacilitySize(reader);
                departments = GetDepartments(reader, departmentCount);
                volumeMatrix = GetVolumeMatrix(reader, departmentCount);
                costMatrix = GetCostMatrix(reader, departmentCount);
            }

            return new FacilityStats(departmentCount, facilitySize, departments, volumeMatrix, costMatrix);
        }

        private int GetDepartmentCount(StreamReader reader)
        {
            var line = ReadUntilNonComment(reader);
            return int.Parse(line);
        }

        private GridSize GetFacilitySize(StreamReader reader)
        {
            var line = ReadUntilNonComment(reader);
            var items = line.Split(DELIMITER);
            var rows = int.Parse(items[0]);
            var columns = int.Parse(items[1]);

            var size = new GridSize(rows,columns);

            return size;
        }

        private IEnumerable<Department> GetDepartments(StreamReader reader, int departmentCount)
        {
            var departments = new List<Department>()
            {
                new Department(0,0,false) //filler, for legacy purposes
            };

            for(int i = 1; i <= departmentCount; i++)
            {
                var line = ReadUntilNonComment(reader);
                var items = line.Split(DELIMITER);

                var departmentArea = int.Parse(items[0]);
                var isLocationFixed = items[1].Trim() != "0";

                var topLeft =   isLocationFixed ? new Position(int.Parse(items[2]), int.Parse(items[3])) : (Position?)null;
                var bottomRight = isLocationFixed ? new Position(int.Parse(items[4]), int.Parse(items[5])) : (Position?)null;

                var department = new Department(i, departmentArea, isLocationFixed, topLeft, bottomRight);
                departments.Add(department);
            }

            return departments;
        }

        private int[,] GetVolumeMatrix(StreamReader reader, int departmentCount)
        {
            var matrix = new int[departmentCount + 1, departmentCount + 1];

            for(var i = 1; i <= departmentCount; i++)
            {
                var line = ReadUntilNonComment(reader);
                var items = line.Split(DELIMITER);

                for(var j = 1; j <= departmentCount; j++)
                {
                    matrix[i, j] = int.Parse(items[j - 1]);
                }
            }

            return matrix;
        }

        private double[,] GetCostMatrix(StreamReader reader, int departmentCount)
        {
            var matrix = new double[departmentCount + 1, departmentCount + 1];

            //initialize filler values that don't correspond to departments
            for(var i = 0; i <= departmentCount; i++)
            {
                matrix[i, 0] = int.MaxValue;
                matrix[0, i] = int.MaxValue;
            }

            for (var i = 1; i <= departmentCount; i++)
            {
                var line = ReadUntilNonComment(reader);
                var items = line.Split(DELIMITER);

                for (var j = 1; j <= departmentCount; j++)
                {
                    matrix[i, j] = double.Parse(items[j - 1]);
                }
            }

            return matrix;
        }

        private string ReadUntilNonComment(StreamReader reader)
        {
            string line = "";

            do
            {
                line = reader.ReadLine();
            } while (line.Trim().StartsWith(COMMENT_MARKER) && !reader.EndOfStream);

            return TrimComment(line);
        }

        private string TrimComment(string str)
        {
            var commentIndex = str.IndexOf(COMMENT_MARKER);

            return commentIndex > -1 ?
                str.Substring(0, commentIndex).Trim() :
                str.Trim();
        }
    }
}
