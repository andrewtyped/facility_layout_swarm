using FacilityLayout.Core.Tests.Stubs;
using FaciltyLayout.Core;
using FaciltyLayout.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityLayout.Core.Tests
{
    [TestFixture]
    class TermiteManagerTests
    {
        private FacilityStats facilityStats;
        private FacilityLayoutModel facilityLayout;
        private TermiteManager termiteManager;

        [SetUp]
        public void SetUp()
        {
            facilityStats = FacilityStatsStub.Get();
            facilityLayout = FacilityLayoutModelStub.Get();
            termiteManager = new TermiteManager(facilityLayout);
        }

        [Test]
        public void Release_The_Termites_Places_Termites()
        {
            var termites = termiteManager.ReleaseTheTermites(100, 3);

            var numTermitesAtPositions = new Dictionary<Position, int>();

            foreach (var termite in termites)
            {
                int currentCount;
                numTermitesAtPositions.TryGetValue(termite.Position, out currentCount);
                numTermitesAtPositions[termite.Position] = currentCount + 1;
            }

            var avgNumTermitesPerPosition = numTermitesAtPositions.Values.Average();

            Assert.Less(avgNumTermitesPerPosition, 1.25); //A magic number, but since we're only using 100 termites
                                                           //and the grid is 15 x 15, doubling on a tile should be rare.
        }

        [Test]
        public void Release_The_Termites_Sets_Termite_Direction()
        {
            var termites = termiteManager.ReleaseTheTermites(100, 3);

            foreach (var termite in termites)
            {
                Assert.That(termite.RowPos != 0 || termite.ColumnPos != 0);
            }
        }

        [Test]
        public void Release_The_Termites_Does_Not_Allow_Termite_Placement_On_Fixed_Departments()
        {
            var termites = termiteManager.ReleaseTheTermites(100, 3);

            var fixedDepartmentIds = facilityStats.Departments.Where(d => d.IsLocationFixed).Select(d => d.Id);

            foreach (var termite in termites)
            {
                Assert.IsFalse(fixedDepartmentIds.Contains(facilityLayout.GetTile(termite.Position)));
            }
        }
    }
}
