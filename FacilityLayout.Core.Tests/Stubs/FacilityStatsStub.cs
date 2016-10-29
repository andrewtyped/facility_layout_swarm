using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityLayout.Core.Tests.Stubs
{
    public static class FacilityStatsStub
    {
        public static FacilityStats Get()
        {
            var departmentCount = 15;
            var facilitySize = new GridSize(15, 15);

            var departments = new List<Department>()
            {
                new Department(0,0,false),
                new Department(1,15,false),
                new Department(2,10,false),
                new Department(3,9,false),
                new Department(4,7,false),
                new Department(5,9,false),
                new Department(6,25,false),
                new Department(7,25,false),
                new Department(8,15,false),
                new Department(9,10,false),
                new Department(10,25,false),
                new Department(11,10,false),
                new Department(12,15,false),
                new Department(13,6,false),
                new Department(14,19,false),
                new Department(15,25,true, new Position(1,1), new Position(5,5))
            };

            var volumeMatrix = new int[16, 16]
            {
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },   // filler
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,240 }, //dept1
                { 0,240,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, //dept2
                { 0,0,0,0,1200,0,0,0,0,0,0,0,0,0,0,0 }, //dept3
                { 0,0,0,0,0,0,0,0,0,0,1200,0,0,0,0,0 }, //dept4
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,600,0 }, //dept5
                { 0,0,0,0,0,0,0,0,480,0,0,0,0,0,0,0 }, //dept6
                { 0,0,0,0,0,0,0,0,480,0,0,0,0,0,0,0 }, //dept7
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,120 }, //dept8
                { 0,0,0,0,0,0,0,0,0,0,600,0,0,0,0,0 }, //dept9
                { 0,0,0,0,0,0,0,0,0,0,0,0,600,0,0,0 }, //dept10
                { 0,0,0,0,0,0,0,480,0,0,0,0,0,0,0,0 }, //dept11
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,600 }, //dept12
                { 0,0,0,0,0,0,0,480,0,0,0,0,0,0,0,0 }, //dept13
                { 0,0,0,0,0,0,0,0,0,0,0,0,600,0,0,0 }, //dept14
                { 0,0,10,50,0,25,40,0,0,25,0,40,0,20,0,0 } //dept15
            };

            var max = int.MaxValue;

            var costMatrix = new double[16, 16]
                {
                    { max, max,max,max,max,max,max,max,max,max,max,max,max,max,max,max },//filler
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept1
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept2
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept3
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept4
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept5
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept6
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept7
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept8
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept9
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept10
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept11
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept12
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept13
                    { max,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5 },//dept14
                    { max,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5,.5 } //dept15
                };

            var facilityStats = new FacilityStats(departmentCount, facilitySize, departments, volumeMatrix, costMatrix);

            return facilityStats;
        }
    }
}
