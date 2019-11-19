using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaseClasses.Helpers
{
    public static class DisplayOptionsHelper
    {
        public static DisplayOptionsFootingPad2D GetDefault()
        {
            DisplayOptionsFootingPad2D opts = new DisplayOptionsFootingPad2D();
            opts.bDrawFootingPad = true;
            opts.FootingPadColor = Brushes.Black;
            opts.FootingPadThickness = 2;

            opts.bDrawColumnOutline = true;
            opts.ColumnOutlineColor = Brushes.Tomato;
            opts.ColumnOutlineBehindColor = Brushes.Tomato;
            opts.ColumnOutlineAboveColor = Brushes.Turquoise;
            opts.ColumnOutlineThickness = 0.7;

            opts.bDrawAnchors = true;

            opts.bDrawBasePlate = true;
            opts.BasePlateColor = Brushes.Brown;
            opts.BasePlateThickness = 0.7;

            opts.bDrawScrews = true;

            opts.bDrawPerimeter = true;
            opts.PerimeterColor = Brushes.DarkOrange;
            opts.PerimeterThickness = 0.8;

            opts.bDrawReinforcement = true;
            opts.ReinforcementFillColor = Brushes.LightGray;
            opts.ReinforcementStrokeColor = Brushes.Black;
            opts.ReinforcementThickness = 1;

            opts.bDrawReinforcementInSlab = true;
            opts.ReinforcementInSlabColor = Brushes.BlueViolet;
            //opts.ReinforcementInSlabThickness = 1;

            opts.bDrawDPC_DPM = true;
            opts.DPC_DPMColor = Brushes.Green;
            opts.DPC_DPMThickness = 0.7;

            opts.bDrawDimensions = true;
            opts.DimensionsLinesColor = Brushes.DarkGreen;
            opts.DimensionsTextColor = Brushes.DarkGreen;
            opts.DimensionsThickness = 1;

            opts.bDrawNotes = true;
            opts.NotesArrowFillColor = Brushes.Black;
            opts.NotesArrowStrokeColor = Brushes.Black;
            opts.NotesThickness = 1;
            return opts;  
        }
        public static DisplayOptionsFootingPad2D GetDefaultForExport()
        {
            DisplayOptionsFootingPad2D opts = new DisplayOptionsFootingPad2D();
            opts.bDrawFootingPad = true;
            opts.FootingPadColor = Brushes.Black;
            opts.FootingPadThickness = 2;

            opts.bDrawColumnOutline = true;
            opts.ColumnOutlineColor = Brushes.Black;
            opts.ColumnOutlineBehindColor = Brushes.Black;
            opts.ColumnOutlineAboveColor = Brushes.Black;
            opts.ColumnOutlineThickness = 0.7;

            opts.bDrawAnchors = true;

            opts.bDrawBasePlate = true;
            opts.BasePlateColor = Brushes.Black;
            opts.BasePlateThickness = 0.7;

            opts.bDrawScrews = true;

            opts.bDrawPerimeter = true;
            opts.PerimeterColor = Brushes.Black;
            opts.PerimeterThickness = 0.8;

            opts.bDrawReinforcement = true;
            opts.ReinforcementFillColor = Brushes.Black;
            opts.ReinforcementStrokeColor = Brushes.Black;
            opts.ReinforcementThickness = 1;

            opts.bDrawReinforcementInSlab = true;
            opts.ReinforcementInSlabColor = Brushes.Black;
            //opts.ReinforcementInSlabThickness = 1;

            opts.bDrawDPC_DPM = true;
            opts.DPC_DPMColor = Brushes.Black;
            opts.DPC_DPMThickness = 0.7;

            opts.bDrawDimensions = true;
            opts.DimensionsLinesColor = Brushes.Black;
            opts.DimensionsTextColor = Brushes.Black;
            opts.DimensionsThickness = 1;

            opts.bDrawNotes = true;
            opts.NotesArrowFillColor = Brushes.Black;
            opts.NotesArrowStrokeColor = Brushes.Black;
            opts.NotesThickness = 1;
            return opts;
        }
    }
}
