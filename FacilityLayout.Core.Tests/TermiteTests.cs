﻿using FacilityLayout.Core.Tests.Stubs;
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
    }
}
