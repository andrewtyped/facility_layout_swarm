using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityLayout.Core.Tests.Stubs
{
    public static class FacilityLayoutModelStub
    {
        public static FacilityLayoutModel Get()
        {
            var stats = FacilityStatsStub.Get();
            var facilityLayoutModel = new FacilityLayoutModel(stats);
            facilityLayoutModel.InitializeDepartmentTiles();

            return facilityLayoutModel;
        }
    }
}
