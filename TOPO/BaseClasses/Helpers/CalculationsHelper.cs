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
        public static double CalculateCanopiesBargeLength(ObservableCollection<CCanopiesInfo> CanopiesList)
        {
            double len = 0;
            foreach (CCanopiesInfo canopy in CanopiesList)
            {
                if (canopy.Left)
                {
                    CCanopiesInfo previousCanopy = CanopiesHelper.GetPreviousNeighboringCanopyLeft(canopy, CanopiesList);
                    if (previousCanopy == null)
                        len += canopy.WidthLeft; // (canopy.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X) / Math.Cos(Math.Abs(Roof_Pitch_rad);

                    CCanopiesInfo nextCanopy = CanopiesHelper.GetNextNeighboringCanopyLeft(canopy, CanopiesList);
                    if (nextCanopy == null)
                        // TO Ondrej, ako sirku canopy by sme mali uvazovat to co je v komentari (cely vyraz)
                        len += canopy.WidthLeft; // canopy.WidthLeft + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X;
                    // Horizontalny rozmer canopy este treba previest na sikmu dlzku takze cely vyraz podelit Math.Cos(Math.Abs(Roof_Pitch_rad) 

                    else
                        len += Math.Abs(nextCanopy.WidthLeft - canopy.WidthLeft); // TO Ondrej, ako sirku canopy by sme mali uvazovat to co je v komentari (cely vyraz)
                }

                if (canopy.Right)
                {
                    CCanopiesInfo previousCanopy = CanopiesHelper.GetPreviousNeighboringCanopyRight(canopy, CanopiesList);
                    if (previousCanopy == null)
                        // TO Ondrej, ako sirku canopy by sme mali uvazovat to co je v komentari (cely vyraz)
                        // Horizontalny rozmer canopy este treba previest na sikmu dlzku takze cely vyraz podelit Math.Cos(Math.Abs(Roof_Pitch_rad) 
                        len += canopy.WidthRight; // canopyNext.WidthRight + canopyOverhangOffset_x - column_crsc_z_plus_temp - roofEdgeOverhang_X

                    CCanopiesInfo nextCanopy = CanopiesHelper.GetNextNeighboringCanopyRight(canopy, CanopiesList);
                    if (nextCanopy == null)
                        len += canopy.WidthRight; // TO Ondrej, ako sirku canopy by sme mali uvazovat to co je v komentari (cely vyraz)
                    // Horizontalny rozmer canopy este treba previest na sikmu dlzku takze cely vyraz podelit Math.Cos(Math.Abs(Roof_Pitch_rad) 
                    else
                        len += Math.Abs(nextCanopy.WidthRight - canopy.WidthRight); // TO Ondrej, ako sirku canopy by sme mali uvazovat to co je v komentari (cely vyraz)
                }
            }
            return len;
        }
    }
}