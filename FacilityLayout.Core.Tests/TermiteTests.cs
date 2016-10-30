using FacilityLayout.Core.Tests.Stubs;
using FaciltyLayout.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityLayout.Core.Tests
{
    [TestFixture]
    public class TermiteTests
    {
        private FacilityLayoutModel facilityLayout;

        [SetUp]
        public void SetUp()
        {
            facilityLayout = FacilityLayoutModelStub.Get();
        }

        [Test]
        public void Termite_Can_Change_Position()
        {
            var termite = new Termites();
            termite.ColumnPos = 12;
            termite.RowPos = 13;
            termite.HorizDirection = 1;
            termite.VertDirection = -2;

            termite.Move(facilityLayout);
            Assert.AreEqual(11, termite.RowPos);
            Assert.AreEqual(13, termite.ColumnPos);
        }

        [Test]
        public void Termite_Wont_Move_Out_Of_Facility_Bounds()
        {
            var termite = new Termites();
            termite.ColumnPos = 20;
            termite.RowPos = 20;
            termite.HorizDirection = 1;
            termite.VertDirection = 1;

            termite.Move(facilityLayout);
            Assert.AreNotEqual(21, termite.RowPos);
            Assert.AreNotEqual(21, termite.ColumnPos);
        }

        [Test]
        public void Termite_Wont_Move_Over_Fixed_Tiles()
        {
            var termite = new Termites();
            termite.ColumnPos = 5;
            termite.RowPos = 4;
            termite.HorizDirection = -1;
            termite.VertDirection = -1;

            Assert.IsTrue(facilityLayout.IsTileFixed(termite.NextPosition));
            termite.Move(facilityLayout);
            Assert.IsFalse(facilityLayout.IsTileFixed(termite.Position));
        }

        [Test]
        public void Termite_Wont_Stay_In_Same_Position()
        {
            var termite = new Termites();
            termite.ColumnPos = 5;
            termite.RowPos = 5;

            var currentPosition = termite.Position;
            termite.Move(facilityLayout);
            Assert.AreNotEqual(currentPosition, termite.Position);
        }

        [Test]
        public void Termites_Respect_AdHoc_Limits_On_Max_Row_And_Colum_Position()
        {
            var termite = new Termites();
            termite.ColumnPos = 20;
            termite.RowPos = 20;
            termite.HorizDirection = 1;
            termite.VertDirection = 1;

            termite.Move(facilityLayout, 19, 19);
            Assert.AreNotEqual(21, termite.RowPos);
            Assert.AreNotEqual(21, termite.ColumnPos);
        }

        [Test]
        public void Termites_Have_A_Random_Order_In_Which_They_Search_Surrounding_Tiles_For_A_Place_To_Set_The_Tile_Theyre_Holding()
        {
            //The value is the aggregate of the index at which the keyed position
            //appears in each termite's search order.
            var positionIndexSums = new ConcurrentDictionary<Position, int>();

            Parallel.For(0, 500000, (i) =>
             {
                 var termite = new Termites();
                 var searchOrder = termite.TileSearchOrder.ToList();

                 for (int j = 1; j <= 9; j++)
                 {
                     positionIndexSums.AddOrUpdate(searchOrder[j - 1], 0, (pos, currentSum) => currentSum + j);
                 }

                 Assert.AreEqual(9, searchOrder.Count);
             });

            Assert.AreEqual(9, positionIndexSums.Count);
            CollectionAssert.AreEquivalent(RelativeTiles.Positions,positionIndexSums.Keys);

            var idealAverage = 2500000.00;
            var average = positionIndexSums.Values.Average();

            foreach(var indexSum in positionIndexSums.Values)
            {
                //Over time, Each position should appear at each possible index
                //in the termites' search orders, leading to a balanced index.
                Assert.LessOrEqual(Math.Abs(idealAverage - average), 5.0);
            }
        }
    }
}
