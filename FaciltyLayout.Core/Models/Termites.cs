using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class Termites
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

        /// <summary>
        /// Returns the next position of the termite given its current position 
        /// and horizontal/vertical directions
        /// </summary>
        public Position NextPosition
        {
            get
            {
                //Yes, this is right. Think about it. Rows top to bottom, columns left to right
                return new Position(RowPos + VertDirection, ColumnPos + HorizDirection); 
            }
        }
    }
}
