using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaseClasses.Helpers
{
    public static class CalculationsHelper
    {
        public static double CalculateCanopiesBargeLength(ObservableCollection<CCanopiesInfo> CanopiesList, float canopyOverhangOffset_x, float roofEdgeOverhang_X, float column_crsc_z_plus, float Roof_Pitch_rad)
        {
            double len = 0;
            foreach (CCanopiesInfo canopy in CanopiesList)
            {
                if (canopy.Left)
                {
                    CCanopiesInfo previousCanopy = CanopiesHelper.GetPreviousNeighboringCanopyLeft(canopy, CanopiesList);
                    if (previousCanopy == null)
                        len += (canopy.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus - roofEdgeOverhang_X) / Math.Cos(Math.Abs(Roof_Pitch_rad));

                    CCanopiesInfo nextCanopy = CanopiesHelper.GetNextNeighboringCanopyLeft(canopy, CanopiesList);
                    if (nextCanopy == null)
                        len += (canopy.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus - roofEdgeOverhang_X) / Math.Cos(Math.Abs(Roof_Pitch_rad));
                    else
                        len += Math.Abs(nextCanopy.WidthLeft - canopy.WidthLeft) / Math.Cos(Math.Abs(Roof_Pitch_rad));
                }

                if (canopy.Right)
                {
                    CCanopiesInfo previousCanopy = CanopiesHelper.GetPreviousNeighboringCanopyRight(canopy, CanopiesList);
                    if (previousCanopy == null)
                        len += (canopy.WidthRight + canopyOverhangOffset_x - column_crsc_z_plus - roofEdgeOverhang_X) / Math.Cos(Math.Abs(Roof_Pitch_rad));

                    CCanopiesInfo nextCanopy = CanopiesHelper.GetNextNeighboringCanopyRight(canopy, CanopiesList);
                    if (nextCanopy == null)
                        len += (canopy.WidthRight + canopyOverhangOffset_x - column_crsc_z_plus - roofEdgeOverhang_X) / Math.Cos(Math.Abs(Roof_Pitch_rad));
                    else
                        len += Math.Abs(nextCanopy.WidthRight - canopy.WidthRight) / Math.Cos(Math.Abs(Roof_Pitch_rad));
                }
            }
            return len;
        }
    }
}