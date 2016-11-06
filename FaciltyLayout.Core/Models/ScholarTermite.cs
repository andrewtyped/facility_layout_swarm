using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class ScholarTermite : Termites
    {
        public override void FindDropPoint(FacilityLayoutModel facilityLayout, FacilityStats facilityStats)
        {
            bool ClosestFound = false;
            var rn = rand.Next(0, 100);
            //Check adjacent spaces for equivalent tiles

            foreach (var tileSearchOrder in TileSearchOrder)
            {
                var adjacentTileSearchColumn = ColumnPos - tileSearchOrder.Column;
                var adjacentTileSearchRow = RowPos - tileSearchOrder.Row;
                //must not look outside of facility field boundaries
                if (0 <= adjacentTileSearchColumn && adjacentTileSearchColumn < facilityLayout.LayoutArea.Columns)
                {
                    if (0 <= adjacentTileSearchRow && adjacentTileSearchRow < facilityLayout.LayoutArea.Rows)
                    {
                        if (facilityLayout.IsTileAssigned(adjacentTileSearchRow, adjacentTileSearchColumn))
                        {
                            if (rn <= 100 * (facilityStats.Flows[TileDept].Flows[facilityLayout.GetTile(adjacentTileSearchRow,
                            adjacentTileSearchColumn)]) / (facilityStats.Flows[TileDept].FlowSum))
                            {
                                foreach (var emptyTileSearchOrder in EmptyTileSearchOrder)
                                {
                                    var emptyTileSearchColumn = adjacentTileSearchColumn - emptyTileSearchOrder.Column;
                                    var emptyTileSearchRow = adjacentTileSearchRow - emptyTileSearchOrder.Row;

                                    if (0 <= emptyTileSearchColumn && emptyTileSearchColumn < facilityLayout.LayoutArea.Columns)
                                    {
                                        if (0 <= emptyTileSearchRow && emptyTileSearchRow < facilityLayout.LayoutArea.Rows)
                                        {
                                            if (facilityLayout.IsTileAssigned(emptyTileSearchRow, emptyTileSearchColumn) == false)
                                            {
                                                ClosestFound = true;
                                                ColumnPos = emptyTileSearchColumn;
                                                RowPos = emptyTileSearchRow;
                                                facilityLayout.SetTile(emptyTileSearchRow, emptyTileSearchColumn, TileDept);
                                                HasTile = false;
                                                TileDept = 0;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (ClosestFound)
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}