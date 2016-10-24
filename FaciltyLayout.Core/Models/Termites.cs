using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public struct Termites
    {
        /// <summary>
        /// How far up/down should I go each turn
        /// </summary>
        public int VertDirection { get; set; } 
        /// <summary>
        /// How far left/right should I go each turn?
        /// </summary>
        public int HorizDirection { get; set; }
        /// <summary>
        /// What column am I in?
        /// </summary>
        public int ColumnPos { get; set; }
        /// <summary>
        /// What row am I in?
        /// </summary>
        public int RowPos { get; set; }
        /// <summary>
        /// Do I have a tile ?
        /// </summary>
        public bool HasTile { get; set; }
        /// <summary>
        /// Will I only pick up a certain kind of tile?
        /// </summary>
        public bool SpecificTile { get; set; }
        /// <summary>
        /// What is the only kind of tile I'll pick up (optional)
        /// </summary>
        public int WhatSpecificTile { get; set; }
        /// <summary>
        /// What kind of tile do I have
        /// </summary>
        public int TileDept { get; set; }
        /// <summary>
        /// What type of termite am I?
        /// </summary>
        public int TermiteType { get; set; }

        public Position Position
        {
            get
            {
                return new Position(RowPos, ColumnPos);
            }
        }
    }
}
