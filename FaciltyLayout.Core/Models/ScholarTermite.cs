using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class ScholarTermite : Termites
    {
        public override void FindDropPoint(FacilityLayoutModel facilityLayoutModel, FacilityStats facilityStats, int rows, int columns)
        {
            bool ClosestFound = false;
            var rn = rand.Next(0, 100);
            //Check adjacent spaces for equivalent tiles

            foreach (var tileSearchOrder in TileSearchOrder)
            {
                var adjacentTileSearchColumn = ColumnPos - tileSearchOrder.Column;
                var adjacentTileSearchRow = RowPos - tileSearchOrder.Row;
                //must not look outside of facility field boundaries
                if (0 <= adjacentTileSearchColumn && adjacentTileSearchColumn < columns)
                {
                    if (0 <= adjacentTileSearchRow && adjacentTileSearchRow < rows)
                    {
                        if (facilityLayoutModel.IsTileAssigned(adjacentTileSearchRow, adjacentTileSearchColumn))
                        {
                            if (rn <= 100 * (facilityStats.Flows[TileDept].Flows[facilityLayoutModel.Facility[adjacentTileSearchRow,
                            adjacentTileSearchColumn]]) / (facilityStats.Flows[TileDept].FlowSum))
                            {
                                foreach (var emptyTileSearchOrder in EmptyTileSearchOrder)
                                {
                                    var emptyTileSearchColumn = adjacentTileSearchColumn - emptyTileSearchOrder.Column;
                                    var emptyTileSearchRow = adjacentTileSearchRow - emptyTileSearchOrder.Row;

                                    if (0 <= emptyTileSearchColumn && emptyTileSearchColumn < columns)
                                    {
                                        if (0 <= emptyTileSearchRow && emptyTileSearchRow < rows)
                                        {
                                            if (facilityLayoutModel.IsTileAssigned(emptyTileSearchRow, emptyTileSearchColumn) == false)
                                            {
                                                ClosestFound = true;
                                                ColumnPos = emptyTileSearchColumn;
                                                RowPos = emptyTileSearchRow;
                                                facilityLayoutModel.SetTile(emptyTileSearchRow, emptyTileSearchColumn, TileDept);
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