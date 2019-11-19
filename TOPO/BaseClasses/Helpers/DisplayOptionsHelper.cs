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
            opts.ColumnOutlineBehindLineStyle = DashStyles.Dash;
            opts.ColumnOutlineThickness = 0.7;

            opts.bDrawBasePlate = true;
            opts.BasePlateColor = Brushes.Brown;
            opts.BasePlateThickness = 0.7;

            opts.bDrawScrews = true;

            opts.bDrawHoles = true;
            opts.bHoleColor = Brushes.Black;
            opts.HoleLineThickness = 0.5;

            opts.bDrawHoleCentreSymbols = true;
            opts.bHoleCenterSymbolColor = Brushes.Red;
            opts.HoleCenterSymbolLineThickness = 0.5;

            opts.bDrawAnchors = true;
            opts.AnchorStrokeColor = Brushes.Green;
            opts.AnchorLineThickness = 0.5;

            opts.bDrawWashers = true;
            opts.WasherStrokeColor = Brushes.DarkOliveGreen;
            opts.WasherLineThickness = 0.5;

            opts.bDrawNuts = true;
            opts.NutStrokeColor = Brushes.Blue;
            opts.NutLineThickness = 0.5;

            opts.bDrawPerimeter = true;
            opts.PerimeterColor = Brushes.DarkOrange;
            opts.PerimeterLineStyle = DashStyles.Dash;
            opts.PerimeterThickness = 0.8;

            opts.bDrawReinforcement = true;
            opts.ReinforcementInSectionFillColor = Brushes.LightGray;
            opts.ReinforcementInSectionStrokeColor = Brushes.Black;
            opts.ReinforcementInSectionThickness = 0.5;

            opts.ReinforcementInWiewColorTop = Brushes.Coral;
            opts.ReinforcementInViewThicknessTop = 2;

            opts.ReinforcementInWiewColorBottom = Brushes.Purple;
            opts.ReinforcementInViewThicknessBottom = 2;

            opts.ReinforcementInWiewColorStarter = Brushes.LightGreen;
            opts.ReinforcementInViewThicknessStarter = 2;

            opts.bDrawReinforcementInSlab = true;
            opts.ReinforcementInSlabColor = Brushes.BlueViolet;
            opts.ReinforcementInSlabLineStyle = DashStyles.Dash;
            opts.ReinforcementInSlabThickness = 1;

            opts.bDrawDPC_DPM = true;
            opts.DPC_DPMColor = Brushes.Green;
            opts.DPC_DPMLineStyle = DashStyles.Dash;
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
            opts.ColumnOutlineBehindLineStyle = DashStyles.Dash;
            opts.ColumnOutlineThickness = 0.7;

            opts.bDrawBasePlate = true;
            opts.BasePlateColor = Brushes.Black;
            opts.BasePlateThickness = 0.7;

            opts.bDrawScrews = true;

            opts.bDrawHoles = true;
            opts.bHoleColor = Brushes.Black;
            opts.HoleLineThickness = 0.7;

            opts.bDrawHoleCentreSymbols = true;
            opts.bHoleCenterSymbolColor = Brushes.Black;
            opts.HoleCenterSymbolLineThickness = 0.7;

            opts.bDrawAnchors = true;
            opts.AnchorStrokeColor = Brushes.Black;
            opts.AnchorLineThickness = 0.7;

            opts.bDrawWashers = true;
            opts.WasherStrokeColor = Brushes.Black;
            opts.WasherLineThickness = 0.7;

            opts.bDrawNuts = true;
            opts.NutStrokeColor = Brushes.Black;
            opts.NutLineThickness = 0.7;

            opts.bDrawPerimeter = true;
            opts.PerimeterColor = Brushes.Black;
            opts.PerimeterLineStyle = DashStyles.Dash;
            opts.PerimeterThickness = 0.8;

            opts.bDrawReinforcement = true;
            opts.ReinforcementInSectionFillColor = Brushes.Black;
            opts.ReinforcementInSectionStrokeColor = Brushes.Black;
            opts.ReinforcementInSectionThickness = 1;

            opts.ReinforcementInWiewColorTop = Brushes.Black;
            opts.ReinforcementInViewThicknessTop = 2;

            opts.ReinforcementInWiewColorBottom = Brushes.Black;
            opts.ReinforcementInViewThicknessBottom = 2;

            opts.ReinforcementInWiewColorStarter = Brushes.Black;
            opts.ReinforcementInViewThicknessStarter = 2;

            opts.bDrawReinforcementInSlab = true;
            opts.ReinforcementInSlabColor = Brushes.Black;
            opts.ReinforcementInSlabLineStyle = DashStyles.Dash;
            opts.ReinforcementInSlabThickness = 1;

            opts.bDrawDPC_DPM = true;
            opts.DPC_DPMColor = Brushes.Black;
            opts.DPC_DPMLineStyle = DashStyles.Dash;
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
