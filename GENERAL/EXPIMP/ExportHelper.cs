using BaseClasses;
using BaseClasses.Helpers;
using FEM_CALC_BASE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EXPIMP
{
    public static class ExportHelper
    {
        public static RenderTargetBitmap SaveViewPortContentAsImage(Viewport3D viewPort)
        {

            // Scale dimensions from 96 dpi to 600 dpi.
            double scale = 300 / 96;
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(scale * viewPort.ActualWidth),
                                                            (int)(scale * viewPort.ActualHeight),
                                                            scale * 96,
                                                            scale * 96, System.Windows.Media.PixelFormats.Default);
            viewPort.InvalidateVisual();
            bmp.Render(viewPort);
            bmp.Freeze();
            SaveBitmapImage(bmp, "ViewPort.png");
            return bmp;

        }

        public static void SaveBitmapImage(RenderTargetBitmap bmp, string fileName)
        {
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create(fileName))
            {
                png.Save(stm);
            }
        }

        public static RenderTargetBitmap GetRenderedCanvas(Canvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);
            return rtb;



            //// Scale dimensions from 96 dpi to 600 dpi.
            //double scale = 300 / 96;
            //RenderTargetBitmap bmp = new RenderTargetBitmap((int)(scale * canvas.ActualWidth),
            //                                                (int)(scale * canvas.ActualHeight),
            //                                                scale * 96,
            //                                                scale * 96, System.Windows.Media.PixelFormats.Default);
            //canvas.InvalidateVisual();
            //bmp.Render(canvas);
            //bmp.Freeze();
            //return bmp;
        }

        public static Stream GetCanvasStream(Canvas canvas)
        {
            //RenderTargetBitmap bmp = GetRenderedCanvas(canvas);
            RenderTargetBitmap bmp = RenderVisual(canvas);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            MemoryStream stm = new MemoryStream();
            png.Save(stm);
            stm.Position = 0;
            
            //using (Stream stm2 = File.Create("canvas"+canvas.Uid+".png"))
            //{
            //    png.Save(stm2);
            //}

            return stm;
        }
        private static RenderTargetBitmap RenderVisual(UIElement elt)
        {            
            Size size = new Size(elt.RenderSize.Width, elt.RenderSize.Height);
            elt.Measure(size);
            elt.Arrange(new Rect(size));
            elt.UpdateLayout();

            var bitmap = new RenderTargetBitmap(
                (int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);

            bitmap.Render(elt);
            return bitmap;

            //PresentationSource source = PresentationSource.FromVisual(elt);
            //RenderTargetBitmap rtb = new RenderTargetBitmap((int)elt.RenderSize.Width,
            //      (int)elt.RenderSize.Height, 96, 96, PixelFormats.Default);

            //VisualBrush sourceBrush = new VisualBrush(elt);
            //DrawingVisual drawingVisual = new DrawingVisual();
            //DrawingContext drawingContext = drawingVisual.RenderOpen();
            //using (drawingContext)
            //{
            //    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0),
            //          new Point(elt.RenderSize.Width, elt.RenderSize.Height)));
            //}
            //rtb.Render(drawingVisual);

            //return rtb;
        }

        // lcomb - Kombinacia ktorej vysledky chceme zobrazit
        public static List<Canvas> GetIFCanvases(bool UseCRSCGeometricalAxes, CLoadCombination lcomb, CMember member, List<CMemberInternalForcesInLoadCombinations> listMemberLoadForces, List<CMemberDeflectionsInLoadCombinations> listMemberDeflections)
        {
            float fCanvasHeight = 180; // Size of Canvas // Same size of of diagrams ???
            float fCanvasWidth = 720;  // Size of Canvas

            float modelMarginLeft_x = 10;
            float modelMarginRight_x = 10;
            float modelMarginTop_y = 10;
            float modelMarginBottom_y = 10;            
            float modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;

            designBucklingLengthFactors[] sBucklingLengthFactors;
            designMomentValuesForCb[] sMomentValuesforCb; // Nepouziva sa

            basicInternalForces[] sBIF_x;
            basicDeflections[] sBDef_x;

            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;
            float[] arrPointsCoordX = new float[iNumberOfDesignSections];
            float fMemberLength_xMax = member.FLength;

            for (int i = 0; i < iNumberOfDesignSections; i++) // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)
                arrPointsCoordX[i] = ((float)i / (float)iNumberOfSegments) * fMemberLength_xMax; // Int must be converted to the float to get decimal numbers

            // TODO - nastavi sa sada vnutornych sil ktora sa ma pre dany prut zobrazit (podla vybraneho pruta a load combination)
            // Zmena 22.02.20019 - Potrebujeme pracovat s LoadCombinations, pretoze BFENet moze vracat vysledky v Load Cases aj Load Combinations
            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(UseCRSCGeometricalAxes, member, lcomb, listMemberLoadForces, iNumberOfDesignSections, out sBucklingLengthFactors, out sMomentValuesforCb, out sBIF_x);
            CMemberResultsManager.SetMemberDeflectionsInLoadCombination(UseCRSCGeometricalAxes, member, lcomb, listMemberDeflections, iNumberOfDesignSections, out sBDef_x);

            // Internal Forces
            float[] fArr_AxialForceValuesN;
            float[] fArr_ShearForceValuesVx;
            float[] fArr_ShearForceValuesVy;
            float[] fArr_TorsionMomentValuesT;
            float[] fArr_BendingMomentValuesMx;
            float[] fArr_BendingMomentValuesMy;

            // Deflections
            float[] fArr_DeflectionValuesDeltax;
            float[] fArr_DeflectionValuesDeltay;

            float fUnitConversionFactor_IF = 1e-3f; // N to kN, Nm to kNm
            float fUnitConversionFactor_Def = 1e+3f; // m to mm
            TransformIFStructureOnMemberToFloatArrays(UseCRSCGeometricalAxes, // // TODO - toto budem musiet nejako elegantne vyriesit LCS vs PCS pruta, problem sa tiahne uz od zadavaneho zatazenie, vypoctu vn. sil az do posudkov
            fUnitConversionFactor_IF,
            fUnitConversionFactor_Def,
            sBIF_x,
            sBDef_x,
            iNumberOfDesignSections,
            out fArr_AxialForceValuesN, out fArr_ShearForceValuesVx, out fArr_ShearForceValuesVy, out fArr_TorsionMomentValuesT, out fArr_BendingMomentValuesMx,
            out fArr_BendingMomentValuesMy, out fArr_DeflectionValuesDeltax, out fArr_DeflectionValuesDeltay);
            
            // Clear canvases
            Canvas Canvas_AxialForceDiagram = new Canvas();            
            Canvas Canvas_ShearForceDiagramVx = new Canvas();
            Canvas Canvas_ShearForceDiagramVy = new Canvas();
            Canvas Canvas_TorsionMomentDiagram = new Canvas();
            Canvas Canvas_BendingMomentDiagramMx = new Canvas();
            Canvas Canvas_BendingMomentDiagramMy = new Canvas();
            Canvas Canvas_DeflectionDiagramDeltax = new Canvas();
            Canvas Canvas_DeflectionDiagramDeltay = new Canvas();

            Canvas_AxialForceDiagram.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_AxialForceDiagram.ToolTip = "Axial Force N [kN]";
            Canvas_ShearForceDiagramVx.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_ShearForceDiagramVx.ToolTip = "Shear Force Vx [kN]";
            Canvas_ShearForceDiagramVy.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_ShearForceDiagramVy.ToolTip = "Shear Force Vy [kN]";
            Canvas_TorsionMomentDiagram.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_TorsionMomentDiagram.ToolTip = "Torsion Moment";
            Canvas_BendingMomentDiagramMx.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_BendingMomentDiagramMx.ToolTip = "Bending Moment Mx [kNm]";
            Canvas_BendingMomentDiagramMy.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_BendingMomentDiagramMy.ToolTip = "Bending Moment My [kNm]";
            Canvas_DeflectionDiagramDeltax.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_DeflectionDiagramDeltax.ToolTip = "Deflection Diagram Delta X";
            Canvas_DeflectionDiagramDeltay.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
            Canvas_DeflectionDiagramDeltay.ToolTip = "Deflection Diagram Delta Y";
            
            Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
            Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);

            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltax);
            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltay);

            // TODO
            // Vysledky by mali byt v N a Nm (pocitame v zakladnych jednotkach SI), pre zobrazenie prekonvertovat na kN a kNm, pripadne pridat nastavenie jednotiek do GUI

            // Draw y values

            // TODO Ondrej - skusal som vykreslovat diagram ako polygon ale zatial neuspesne, zase je tu "rozpor" v tom na akej urovni prepocitat hodnoty do zobrazovacich jednotiek
            // Chcelo by to nejako pekne zjednotit s vykreslovanim FrameInternalForces, ale tu je to zlozitejsie lebo som navymyslal rozne pozicie a orientaciu osi x podla toho ake su hodnoty

            //List<Point> listAxialForceValuesN = AddFirstAndLastDiagramPoint(fArr_AxialForceValuesN, member, 40, arrPointsCoordX, -1, 0.01, 1);
            //Drawing2D.DrawYValuesPolygonInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, listAxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);

            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);

            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltax);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltay);

            // Draw values description
            int iNumberOfDecimalPlaces = 2;
            Drawing2D.DrawTexts(true, true, ConversionsHelper.ConvertArrayFloatToString(fArr_AxialForceValuesN, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_AxialForceDiagram);
            Drawing2D.DrawTexts(true, true, ConversionsHelper.ConvertArrayFloatToString(fArr_ShearForceValuesVx, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawTexts(true, true, ConversionsHelper.ConvertArrayFloatToString(fArr_ShearForceValuesVy, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_TorsionMomentValuesT, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_BendingMomentValuesMx, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_BendingMomentValuesMy, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_BendingMomentDiagramMy);

            Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_DeflectionValuesDeltax, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_DeflectionDiagramDeltax);
            Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_DeflectionValuesDeltay, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_DeflectionDiagramDeltay);

            List<Canvas> canvases = new List<Canvas>();
            if (fArr_AxialForceValuesN.Any(n => n > 0.001)) canvases.Add(Canvas_AxialForceDiagram);
            if (fArr_ShearForceValuesVx.Any(n => n > 0.001)) canvases.Add(Canvas_ShearForceDiagramVx);
            if (fArr_ShearForceValuesVy .Any(n => n > 0.001)) canvases.Add(Canvas_ShearForceDiagramVy);
            if (fArr_TorsionMomentValuesT.Any(n => n > 0.001)) canvases.Add(Canvas_TorsionMomentDiagram);
            if (fArr_BendingMomentValuesMx.Any(n => n > 0.001)) canvases.Add(Canvas_BendingMomentDiagramMx);
            if (fArr_BendingMomentValuesMy .Any(n => n > 0.001)) canvases.Add(Canvas_BendingMomentDiagramMy);
            if (fArr_DeflectionValuesDeltax.Any(n => n > 0.001)) canvases.Add(Canvas_DeflectionDiagramDeltax);
            if (fArr_DeflectionValuesDeltay.Any(n => n > 0.001)) canvases.Add(Canvas_DeflectionDiagramDeltay);

            return canvases;
        }


        public static void TransformIFStructureOnMemberToFloatArrays(
            bool bUseResultsForGeometricalCRSCAxis, // Use cross-section geometrical axis IF or principal axis IF
            float fUnitConversionFactor_IF,
            float fUnitConversionFactor_Def,
            basicInternalForces[] sBIF_x,
            basicDeflections[] sBDef_x,
            int iNumberOfDesignSections,
            out float[] fArr_AxialForceValuesN,
            out float[] fArr_ShearForceValuesVx,
            out float[] fArr_ShearForceValuesVy,
            out float[] fArr_TorsionMomentValuesT,
            out float[] fArr_BendingMomentValuesMx,
            out float[] fArr_BendingMomentValuesMy,
            out float[] fArr_DeflectionValuesDeltax,
            out float[] fArr_DeflectionValuesDeltay
            )
        {
            fArr_AxialForceValuesN = new float[iNumberOfDesignSections];
            fArr_ShearForceValuesVx = new float[iNumberOfDesignSections];
            fArr_ShearForceValuesVy = new float[iNumberOfDesignSections];
            fArr_TorsionMomentValuesT = new float[iNumberOfDesignSections];
            fArr_BendingMomentValuesMx = new float[iNumberOfDesignSections];
            fArr_BendingMomentValuesMy = new float[iNumberOfDesignSections];

            fArr_DeflectionValuesDeltax = new float[iNumberOfDesignSections];
            fArr_DeflectionValuesDeltay = new float[iNumberOfDesignSections];

            for (int i = 0; i < iNumberOfDesignSections; i++)
            {
                // TODO indexy pre cross-section principal axes vs indexy pre local axes
                fArr_AxialForceValuesN[i] = sBIF_x[i].fN * fUnitConversionFactor_IF;
                fArr_TorsionMomentValuesT[i] = sBIF_x[i].fT * fUnitConversionFactor_IF;

                if (bUseResultsForGeometricalCRSCAxis)
                {
                    fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yy * fUnitConversionFactor_IF;
                    fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zz * fUnitConversionFactor_IF;
                    fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yy * fUnitConversionFactor_IF;
                    fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zz * fUnitConversionFactor_IF;

                    fArr_DeflectionValuesDeltax[i] = sBDef_x[i].fDelta_yy * fUnitConversionFactor_Def;
                    fArr_DeflectionValuesDeltay[i] = sBDef_x[i].fDelta_zz * fUnitConversionFactor_Def;
                }
                else
                {
                    fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yu * fUnitConversionFactor_IF;
                    fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zv * fUnitConversionFactor_IF;
                    fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yu * fUnitConversionFactor_IF;
                    fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zv * fUnitConversionFactor_IF;

                    fArr_DeflectionValuesDeltax[i] = sBDef_x[i].fDelta_yu * fUnitConversionFactor_Def;
                    fArr_DeflectionValuesDeltay[i] = sBDef_x[i].fDelta_zv * fUnitConversionFactor_Def;
                }
            }
        }


    }
}
