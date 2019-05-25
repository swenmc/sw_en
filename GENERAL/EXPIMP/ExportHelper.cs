using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
using FEM_CALC_BASE;
using MATH;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static List<Canvas> GetIFCanvases(bool IsULS, bool UseCRSCGeometricalAxes, CLoadCombination lcomb, CMember member, List<CMemberInternalForcesInLoadCombinations> listMemberLoadForces, List<CMemberDeflectionsInLoadCombinations> listMemberDeflections)
        {
            List<Canvas> canvases = new List<Canvas>();
            if (lcomb == null || member == null || listMemberLoadForces == null || listMemberDeflections == null) return canvases;
            
            //Original Size - 620:160
            float fCanvasHeight = 160; // Size of Canvas // Same size of of diagrams ???
            float fCanvasWidth = 620;  // Size of Canvas
            
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
            

            //TOTO
            //toto nastavenie dole tu nema byt. Bude nutne prerobit vykreslovanie v canvasoch. Zvacsit okraje napravo a dole. Mozno potom aj roztiahnut canvas v dokumente na max sirku 720.
            fCanvasHeight = 150; 
            fCanvasWidth = 600;
            float modelMarginLeft_x = 10;
            float modelMarginRight_x = 10;
            float modelMarginTop_y = 10;
            float modelMarginBottom_y = 10;
            float modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;
            int iNumberOfDecimalPlaces = 2;

            double limitForce = 1e-3; // 0.001 kN
            double limitMoment = 1e-3; // 0.001 kNm
            double limitDeflection = 1e-2; // 0.01 mm
            
            if (IsULS)
            {
                Canvas Canvas_AxialForceDiagram = new Canvas();
                Canvas Canvas_ShearForceDiagramVx = new Canvas();
                Canvas Canvas_ShearForceDiagramVy = new Canvas();
                Canvas Canvas_TorsionMomentDiagram = new Canvas();
                Canvas Canvas_BendingMomentDiagramMx = new Canvas();
                Canvas Canvas_BendingMomentDiagramMy = new Canvas();


                Canvas_AxialForceDiagram.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_AxialForceDiagram.Name = "AxialForce_N";
                Canvas_AxialForceDiagram.ToolTip = "Axial Force N [kN]";
                Canvas_ShearForceDiagramVx.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_ShearForceDiagramVx.Name = "ShearForce_Vx";
                Canvas_ShearForceDiagramVx.ToolTip = "Shear Force Vx [kN]";
                Canvas_ShearForceDiagramVy.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_ShearForceDiagramVy.Name = "ShearForce_Vy";
                Canvas_ShearForceDiagramVy.ToolTip = "Shear Force Vy [kN]";
                Canvas_TorsionMomentDiagram.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_TorsionMomentDiagram.Name = "TorsionMoment_T";
                Canvas_TorsionMomentDiagram.ToolTip = "Torsion Moment T [kNm]";
                Canvas_BendingMomentDiagramMx.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_BendingMomentDiagramMx.Name = "BendingMoment_Mx";
                Canvas_BendingMomentDiagramMx.ToolTip = "Bending Moment Mx [kNm]";
                Canvas_BendingMomentDiagramMy.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_BendingMomentDiagramMy.Name = "BendingMoment_My";
                Canvas_BendingMomentDiagramMy.ToolTip = "Bending Moment My [kNm]";

                Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
                Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
                Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

                Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
                Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
                Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);



                // TODO
                // Vysledky by mali byt v N a Nm (pocitame v zakladnych jednotkach SI), pre zobrazenie prekonvertovat na kN a kNm, pripadne pridat nastavenie jednotiek do GUI

                // Draw y values

                // TODO Ondrej - skusal som vykreslovat diagram ako polygon ale zatial neuspesne, zase je tu "rozpor" v tom na akej urovni prepocitat hodnoty do zobrazovacich jednotiek
                // Chcelo by to nejako pekne zjednotit s vykreslovanim FrameInternalForces, ale tu je to zlozitejsie lebo som navymyslal rozne pozicie a orientaciu osi +/-y podla toho ake su minimalne a maximalne hodnoty a ci sa jedna o sily alebo ohybove momenty

                //List<Point> listAxialForceValuesN = AddFirstAndLastDiagramPoint(fArr_AxialForceValuesN, member, 40, arrPointsCoordX, -1, 0.01, 1);
                //Drawing2D.DrawYValuesPolygonInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, listAxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);

                Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
                Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
                Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

                Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
                Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
                Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);


                // Draw values description
                
                Drawing2D.DrawTexts(true, true, ConversionsHelper.ConvertArrayFloatToString(fArr_AxialForceValuesN, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_AxialForceDiagram);
                Drawing2D.DrawTexts(true, true, ConversionsHelper.ConvertArrayFloatToString(fArr_ShearForceValuesVx, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_ShearForceDiagramVx);
                Drawing2D.DrawTexts(true, true, ConversionsHelper.ConvertArrayFloatToString(fArr_ShearForceValuesVy, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_ShearForceDiagramVy);

                Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_TorsionMomentValuesT, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_TorsionMomentDiagram);
                Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_BendingMomentValuesMx, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_BendingMomentDiagramMx);
                Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_BendingMomentValuesMy, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_BendingMomentDiagramMy);

                if (fArr_AxialForceValuesN.Any(n => Math.Abs(n) > limitForce)) canvases.Add(Canvas_AxialForceDiagram);
                if (fArr_ShearForceValuesVx.Any(n => Math.Abs(n) > limitForce)) canvases.Add(Canvas_ShearForceDiagramVx);
                if (fArr_ShearForceValuesVy.Any(n => Math.Abs(n) > limitForce)) canvases.Add(Canvas_ShearForceDiagramVy);
                if (fArr_TorsionMomentValuesT.Any(n => Math.Abs(n) > limitMoment)) canvases.Add(Canvas_TorsionMomentDiagram);
                if (fArr_BendingMomentValuesMx.Any(n => Math.Abs(n) > limitMoment)) canvases.Add(Canvas_BendingMomentDiagramMx);
                if (fArr_BendingMomentValuesMy.Any(n => Math.Abs(n) > limitMoment)) canvases.Add(Canvas_BendingMomentDiagramMy);

            }
            else if (!IsULS)
            {
                Canvas Canvas_DeflectionDiagramDeltax = new Canvas();
                Canvas Canvas_DeflectionDiagramDeltay = new Canvas();
                Canvas_DeflectionDiagramDeltax.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_DeflectionDiagramDeltax.Name = "LocalDeflection_Delta_x";
                Canvas_DeflectionDiagramDeltax.ToolTip = "Local Deflection δx [mm]";
                Canvas_DeflectionDiagramDeltay.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                Canvas_DeflectionDiagramDeltay.Name = "LocalDeflection_Delta_y";
                Canvas_DeflectionDiagramDeltay.ToolTip = "Local Deflection δy [mm]";
                Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltax);
                Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltay);

                Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltax);
                Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltay);

                Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_DeflectionValuesDeltax, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_DeflectionDiagramDeltax);
                Drawing2D.DrawTexts(false, true, ConversionsHelper.ConvertArrayFloatToString(fArr_DeflectionValuesDeltay, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_DeflectionDiagramDeltay);
                if (fArr_DeflectionValuesDeltax.Any(n => Math.Abs(n) > limitDeflection)) canvases.Add(Canvas_DeflectionDiagramDeltax);
                if (fArr_DeflectionValuesDeltay.Any(n => Math.Abs(n) > limitDeflection)) canvases.Add(Canvas_DeflectionDiagramDeltay);
            }
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

        public static List<Canvas> GetFrameInternalForcesCanvases(bool isULS, List<CFrame> frames, CMember member, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> ListMemberDeflectionsInLoadCombinations, bool UseCRSCGeometricalAxes)
        {
            if (frames == null || member == null || lcomb == null || ListMemberInternalForcesInLoadCombinations == null || ListMemberDeflectionsInLoadCombinations == null) return new List<Canvas>();

            CFrame model = null;
            for (int i = 0; i < frames.Count; i++)
            {
                if (Array.Exists(frames[i].m_arrMembers, mem => mem.ID == member.ID)) { model = frames[i]; break; }
            }
            //if(model == null) frames.FirstOrDefault(); //if not found frame for member - get first frame
            if (model == null) throw new Exception($"Frame not found for member: {member?.ID}");
            if(isULS) return GetFrameInternalForcesCanvasesULS(model, lcomb, ListMemberInternalForcesInLoadCombinations, ListMemberDeflectionsInLoadCombinations, UseCRSCGeometricalAxes);
            else return GetFrameInternalForcesCanvasesSLS(model, lcomb, ListMemberInternalForcesInLoadCombinations, ListMemberDeflectionsInLoadCombinations, UseCRSCGeometricalAxes);

        }

        private static List<Canvas> GetFrameInternalForcesCanvasesULS(CFrame model, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> ListMemberDeflectionsInLoadCombinations, bool UseCRSCGeometricalAxes)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////
            //double internalForceScale_user = 2;
            int NumberOfDecimalPlaces = 2;
            int FontSize = 12;
            float fCanvasWidth = (float)720; // Size of Canvas  990/580
            float fCanvasHeight = (float)422; // Size of Canvas
            int scale_unit = 1; // m

            List<Point> modelNodesCoordinatesInGCS = new List<Point>();

            for (int i = 0; i < model.m_arrNodes.Length; i++) // Naplnime pole bodov s globanymi suradnicami modelu
            {
                modelNodesCoordinatesInGCS.Add(new Point(model.m_arrNodes[i].X, model.m_arrNodes[i].Z));
            }

            double dTempMax_X;
            double dTempMin_X;
            double dTempMax_Y;
            double dTempMin_Y;
            Drawing2D.CalculateModelLimits(modelNodesCoordinatesInGCS, out dTempMax_X, out dTempMin_X, out dTempMax_Y, out dTempMin_Y);

            float fModel_Length_x_real = (float)(dTempMax_X - dTempMin_X);
            float fModel_Length_y_real = (float)(dTempMax_Y - dTempMin_Y);
            float fModel_Length_x_page;
            float fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            float fReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginTop_y;
            float fmodelBottomPosition_y;

            Drawing2D.CalculateBasicValue(
            fModel_Length_x_real,
            fModel_Length_y_real,
            0.6f, // zoom ratio 0-1 (zoom of 2D view), zobrazime model vo velkosti 50% z canvas aby bol dostatok priestoru pre vykreslenie vn sil
            scale_unit,
            fCanvasWidth,
            fCanvasHeight,
            out fModel_Length_x_page,
            out fModel_Length_y_page,
            out dFactor_x,
            out dFactor_y,
            out fReal_Model_Zoom_Factor,
            out fmodelMarginLeft_x,
            out fmodelMarginTop_y,
            out fmodelBottomPosition_y
            );

            float fmodelMarginBottom_y = fCanvasHeight - fmodelMarginTop_y - fModel_Length_y_page;
            int factorSwitchYAxis = -1;

            List<Canvas> canvases = new List<Canvas>();

            for (int IFtypeIndex = 0; IFtypeIndex <= 5; IFtypeIndex++)
            {
                Canvas DiagramCanvas = new Canvas();
                DiagramCanvas.RenderSize = new Size(fCanvasWidth, fCanvasHeight);

                // Urcenie specifickeho mena canvasu pre jednoznacnu identifikaciu
                switch (IFtypeIndex)
                {
                    case 0:
                        DiagramCanvas.Name = "AxialForce_N";
                        break;
                    case 1:
                        DiagramCanvas.Name = "ShearForce_Vx";
                        break;
                    case 2:
                        DiagramCanvas.Name = "ShearForce_Vy";
                        break;
                    case 3:
                        DiagramCanvas.Name = "TorsionMoment_T";
                        break;
                    case 4:
                        DiagramCanvas.Name = "BendingMoment_Mx";
                        break;
                    case 5:
                        DiagramCanvas.Name = "BendingMoment_My";
                        break;
                    case 6:
                        DiagramCanvas.Name = "LocalDeflection_Delta_x";
                        break;
                    case 7:
                        DiagramCanvas.Name = "LocalDeflection_Delta_y";
                        break;
                    default:
                        DiagramCanvas.Name = "";
                        break;
                }

                string IFTypeUnit = "";
                //"N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz"
                if (IFtypeIndex <= 2) IFTypeUnit = "kN";
                else if (IFtypeIndex <= 6) IFTypeUnit = "kNm";
                else IFTypeUnit = "mm";

                // Internal forces / Deformation - default unit scale factor
                float fUnitFactor = 1;
                float fUnitFactor_IF = 0.001f; // N to kN or Nm to kNm
                float fUnitFactor_Def = 1000f; // m to mm

                if (IFtypeIndex <= 6)
                    fUnitFactor = fUnitFactor_IF;  // Forces and moments
                else
                    fUnitFactor = fUnitFactor_Def; // Deformations (Displacement / Deflection, Rotation)

                bool IncludeResults = false;

                double bestScaleRatio = GetBestScaleRatio(model, ListMemberInternalForcesInLoadCombinations, ListMemberDeflectionsInLoadCombinations,
                        model.m_arrMembers, lcomb, IFtypeIndex, fUnitFactor, fReal_Model_Zoom_Factor, UseCRSCGeometricalAxes);

                // Draw each member in the model and selected internal force diagram
                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    // Calculate Member Rotation angle (clockwise)
                    double rotAngle_radians = Math.Atan(((dTempMax_Y + factorSwitchYAxis * model.m_arrMembers[i].NodeEnd.Z) - (dTempMax_Y + factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z)) / (model.m_arrMembers[i].NodeEnd.X - model.m_arrMembers[i].NodeStart.X));
                    double rotAngle_degrees = Geom2D.RadiansToDegrees(rotAngle_radians);
                    
                    // Get list of points from Dictionary, if not exist then calculate
                    List<Point> listMemberInternalForcePoints = GetMemberInternalForcePoints(model, ListMemberInternalForcesInLoadCombinations, ListMemberDeflectionsInLoadCombinations,
                        model.m_arrMembers[i], lcomb, IFtypeIndex, fUnitFactor, bestScaleRatio, fReal_Model_Zoom_Factor, UseCRSCGeometricalAxes);

                    double translationOffset_x = fmodelMarginLeft_x + fReal_Model_Zoom_Factor * model.m_arrMembers[i].NodeStart.X;
                    double translationOffset_y = fmodelBottomPosition_y + fReal_Model_Zoom_Factor * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z;

                    RotateTransform rotateTransform = new RotateTransform(rotAngle_degrees, 0, 0); // + clockwise, - counter-clockwise
                    TranslateTransform translateTransform = new TranslateTransform(translationOffset_x, translationOffset_y);
                    TransformGroup transformGroup_RandT = new TransformGroup();
                    transformGroup_RandT.Children.Add(rotateTransform);
                    transformGroup_RandT.Children.Add(translateTransform);

                    List<Point> points = new List<Point>();
                    foreach (Point p in listMemberInternalForcePoints)
                        points.Add(transformGroup_RandT.Transform(p));

                    // Analyse diagram - find minimum and maximum value (find local extremes ???)
                    // store index of extreme values

                    double dMinValue = Double.PositiveInfinity;
                    double dMaxValue = Double.NegativeInfinity;

                    int iIndexMinValue = 0;
                    int iIndexMaxValue = 0;

                    if (ListMemberInternalForcesInLoadCombinations == null) continue; // TODO - Sem by sa to uz nemalo ani dostat ak prut nema vysledky

                    int iNumberOfDesignSections = 11;
                    designBucklingLengthFactors[] sBucklingLengthFactors;
                    designMomentValuesForCb[] sMomentValuesforCb;
                    basicInternalForces[] sBIF_x;
                    basicDeflections[] sBDef_x;

                    CMemberResultsManager.SetMemberInternalForcesInLoadCombination(
                        UseCRSCGeometricalAxes,
                        model.m_arrMembers[i],
                        lcomb,
                        ListMemberInternalForcesInLoadCombinations,
                        iNumberOfDesignSections,
                        out sBucklingLengthFactors,
                        out sMomentValuesforCb,
                        out sBIF_x);

                    CMemberResultsManager.SetMemberDeflectionsInLoadCombination(
                        UseCRSCGeometricalAxes,
                        model.m_arrMembers[i],
                        lcomb,
                        ListMemberDeflectionsInLoadCombinations,
                        iNumberOfDesignSections,
                        out sBDef_x);

                    for (int c = 0; c < sBIF_x.Length; c++)
                    {
                        float IF_Value = GetInternalForcesValue(IFtypeIndex, sBIF_x[c], sBDef_x[c]);
                        if (IF_Value < dMinValue)
                        {
                            dMinValue = IF_Value;
                            iIndexMinValue = c;
                        }
                        if (IF_Value > dMaxValue)
                        {
                            dMaxValue = IF_Value;
                            iIndexMaxValue = c;
                        }
                    }

                    // TODO Ondrej - Tieto limity "0" by mali byt rovnake ako limity pre diagramy samostatnych prutov podla typu IF (IFtypeIndex)
                    // aby sa nestalo ze tam bude diagram pre ram ale nie pre prut

                    if (!MathF.d_equal(dMinValue, 0) || !MathF.d_equal(dMaxValue, 0)) IncludeResults = true;

                    for (int c = 0; c < sBIF_x.Length; c++)
                    {
                        if(c != 0 && c != (sBIF_x.Length - 1) && c != iIndexMinValue && c != iIndexMaxValue) continue;
                        float IF_Value = GetInternalForcesValue(IFtypeIndex ,sBIF_x[c], sBDef_x[c]);

                        // Ignore and do not display zero value label
                        if (MathF.d_equal(IF_Value, 0))
                            continue;

                        string txt = (fUnitFactor * IF_Value).ToString($"F{NumberOfDecimalPlaces}");
                        txt += " " + IFTypeUnit;
                        //string txt = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * IF_Value, 2))) + " " + vm.IFTypeUnit;
                        Drawing2D.DrawText(txt, points[c + 1].X, points[c + 1].Y, 0, FontSize, Brushes.SlateGray, DiagramCanvas);
                    }
                    
                    Drawing2D.DrawPolygon(points, Brushes.LightSlateGray, Brushes.SlateGray, PenLineCap.Flat, PenLineCap.Flat, 1, 0.3, DiagramCanvas);
                    
                    // Draw Member on the Internal forces polygon
                    DrawMember(model, DiagramCanvas, i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees, fmodelMarginLeft_x, fmodelBottomPosition_y, Brushes.Black, 1);
                }

                if (IncludeResults)
                {
                    DiagramCanvas.ToolTip = GetInternalForceText(IFtypeIndex);
                    canvases.Add(DiagramCanvas);
                }
            }

            return canvases;
        }

        private static List<Canvas> GetFrameInternalForcesCanvasesSLS(CFrame model, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> ListMemberDeflectionsInLoadCombinations, bool UseCRSCGeometricalAxes)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////
            //double internalForceScale_user = 2;
            int NumberOfDecimalPlaces = 2;
            int FontSize = 12;
            float fCanvasWidth = (float)720; // Size of Canvas  990/580
            float fCanvasHeight = (float)422; // Size of Canvas
            int scale_unit = 1; // m

            List<Point> modelNodesCoordinatesInGCS = new List<Point>();

            for (int i = 0; i < model.m_arrNodes.Length; i++) // Naplnime pole bodov s globanymi suradnicami modelu
            {
                modelNodesCoordinatesInGCS.Add(new Point(model.m_arrNodes[i].X, model.m_arrNodes[i].Z));
            }

            double dTempMax_X;
            double dTempMin_X;
            double dTempMax_Y;
            double dTempMin_Y;
            Drawing2D.CalculateModelLimits(modelNodesCoordinatesInGCS, out dTempMax_X, out dTempMin_X, out dTempMax_Y, out dTempMin_Y);

            float fModel_Length_x_real = (float)(dTempMax_X - dTempMin_X);
            float fModel_Length_y_real = (float)(dTempMax_Y - dTempMin_Y);
            float fModel_Length_x_page;
            float fModel_Length_y_page;
            double dFactor_x;
            double dFactor_y;
            float fReal_Model_Zoom_Factor;
            float fmodelMarginLeft_x;
            float fmodelMarginTop_y;
            float fmodelBottomPosition_y;

            Drawing2D.CalculateBasicValue(
            fModel_Length_x_real,
            fModel_Length_y_real,
            0.6f, // zoom ratio 0-1 (zoom of 2D view), zobrazime model vo velkosti 50% z canvas aby bol dostatok priestoru pre vykreslenie vn sil
            scale_unit,
            fCanvasWidth,
            fCanvasHeight,
            out fModel_Length_x_page,
            out fModel_Length_y_page,
            out dFactor_x,
            out dFactor_y,
            out fReal_Model_Zoom_Factor,
            out fmodelMarginLeft_x,
            out fmodelMarginTop_y,
            out fmodelBottomPosition_y
            );

            float fmodelMarginBottom_y = fCanvasHeight - fmodelMarginTop_y - fModel_Length_y_page;
            int factorSwitchYAxis = -1;

            List<Canvas> canvases = new List<Canvas>();

            for (int IFtypeIndex = 6; IFtypeIndex <= 7; IFtypeIndex++)
            {
                Canvas DiagramCanvas = new Canvas();
                DiagramCanvas.RenderSize = new Size(fCanvasWidth, fCanvasHeight);

                // Urcenie specifickeho mena canvasu pre jednoznacnu identifikaciu
                switch (IFtypeIndex)
                {
                    case 0:
                        DiagramCanvas.Name = "AxialForce_N";
                        break;
                    case 1:
                        DiagramCanvas.Name = "ShearForce_Vx";
                        break;
                    case 2:
                        DiagramCanvas.Name = "ShearForce_Vy";
                        break;
                    case 3:
                        DiagramCanvas.Name = "TorsionMoment_T";
                        break;
                    case 4:
                        DiagramCanvas.Name = "BendingMoment_Mx";
                        break;
                    case 5:
                        DiagramCanvas.Name = "BendingMoment_My";
                        break;
                    case 6:
                        DiagramCanvas.Name = "LocalDeflection_Delta_x";
                        break;
                    case 7:
                        DiagramCanvas.Name = "LocalDeflection_Delta_y";
                        break;
                    default:
                        DiagramCanvas.Name = "";
                        break;
                }

                string IFTypeUnit = "";
                //"N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz"
                if (IFtypeIndex <= 2) IFTypeUnit = "kN";
                else if (IFtypeIndex <= 6) IFTypeUnit = "kNm";
                else IFTypeUnit = "mm";

                // Internal forces / Deformation - default unit scale factor
                float fUnitFactor = 1;
                float fUnitFactor_IF = 0.001f; // N to kN or Nm to kNm
                float fUnitFactor_Def = 1000f; // m to mm

                if (IFtypeIndex <= 6)
                    fUnitFactor = fUnitFactor_IF;  // Forces and moments
                else
                    fUnitFactor = fUnitFactor_Def; // Deformations (Displacement / Deflection, Rotation)

                bool IncludeResults = false;

                double bestScaleRatio = GetBestScaleRatio(model, ListMemberInternalForcesInLoadCombinations, ListMemberDeflectionsInLoadCombinations,
                        model.m_arrMembers, lcomb, IFtypeIndex, fUnitFactor, fReal_Model_Zoom_Factor, UseCRSCGeometricalAxes);

                // Draw each member in the model and selected internal force diagram
                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    // Calculate Member Rotation angle (clockwise)
                    double rotAngle_radians = Math.Atan(((dTempMax_Y + factorSwitchYAxis * model.m_arrMembers[i].NodeEnd.Z) - (dTempMax_Y + factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z)) / (model.m_arrMembers[i].NodeEnd.X - model.m_arrMembers[i].NodeStart.X));
                    double rotAngle_degrees = Geom2D.RadiansToDegrees(rotAngle_radians);

                    // Get list of points from Dictionary, if not exist then calculate
                    List<Point> listMemberInternalForcePoints = GetMemberInternalForcePoints(model, ListMemberInternalForcesInLoadCombinations, ListMemberDeflectionsInLoadCombinations,
                        model.m_arrMembers[i], lcomb, IFtypeIndex, fUnitFactor, bestScaleRatio, fReal_Model_Zoom_Factor, UseCRSCGeometricalAxes);

                    double translationOffset_x = fmodelMarginLeft_x + fReal_Model_Zoom_Factor * model.m_arrMembers[i].NodeStart.X;
                    double translationOffset_y = fmodelBottomPosition_y + fReal_Model_Zoom_Factor * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z;

                    RotateTransform rotateTransform = new RotateTransform(rotAngle_degrees, 0, 0); // + clockwise, - counter-clockwise
                    TranslateTransform translateTransform = new TranslateTransform(translationOffset_x, translationOffset_y);
                    TransformGroup transformGroup_RandT = new TransformGroup();
                    transformGroup_RandT.Children.Add(rotateTransform);
                    transformGroup_RandT.Children.Add(translateTransform);

                    List<Point> points = new List<Point>();
                    foreach (Point p in listMemberInternalForcePoints)
                        points.Add(transformGroup_RandT.Transform(p));

                    // Analyse diagram - find minimum and maximum value (find local extremes ???)
                    // store index of extreme values

                    double dMinValue = Double.PositiveInfinity;
                    double dMaxValue = Double.NegativeInfinity;

                    int iIndexMinValue = 0;
                    int iIndexMaxValue = 0;

                    if (ListMemberInternalForcesInLoadCombinations == null) continue; // TODO - Sem by sa to uz nemalo ani dostat ak prut nema vysledky

                    int iNumberOfDesignSections = 11;
                    designBucklingLengthFactors[] sBucklingLengthFactors;
                    designMomentValuesForCb[] sMomentValuesforCb;
                    basicInternalForces[] sBIF_x;
                    basicDeflections[] sBDef_x;

                    CMemberResultsManager.SetMemberInternalForcesInLoadCombination(
                        UseCRSCGeometricalAxes,
                        model.m_arrMembers[i],
                        lcomb,
                        ListMemberInternalForcesInLoadCombinations,
                        iNumberOfDesignSections,
                        out sBucklingLengthFactors,
                        out sMomentValuesforCb,
                        out sBIF_x);

                    CMemberResultsManager.SetMemberDeflectionsInLoadCombination(
                        UseCRSCGeometricalAxes,
                        model.m_arrMembers[i],
                        lcomb,
                        ListMemberDeflectionsInLoadCombinations,
                        iNumberOfDesignSections,
                        out sBDef_x);

                    for (int c = 0; c < sBIF_x.Length; c++)
                    {
                        float IF_Value = GetInternalForcesValue(IFtypeIndex, sBIF_x[c], sBDef_x[c]);
                        if (IF_Value < dMinValue)
                        {
                            dMinValue = IF_Value;
                            iIndexMinValue = c;
                        }
                        if (IF_Value > dMaxValue)
                        {
                            dMaxValue = IF_Value;
                            iIndexMaxValue = c;
                        }
                    }

                    // TODO Ondrej - Tieto limity "0" by mali byt rovnake ako limity pre diagramy samostatnych prutov podla typu IF (IFtypeIndex)
                    // aby sa nestalo ze tam bude diagram pre ram ale nie pre prut

                    if (!MathF.d_equal(dMinValue, 0) || !MathF.d_equal(dMaxValue, 0)) IncludeResults = true;

                    for (int c = 0; c < sBIF_x.Length; c++)
                    {
                        if (c != 0 && c != (sBIF_x.Length - 1) && c != iIndexMinValue && c != iIndexMaxValue) continue;
                        float IF_Value = GetInternalForcesValue(IFtypeIndex, sBIF_x[c], sBDef_x[c]);

                        // Ignore and do not display zero value label
                        if (MathF.d_equal(IF_Value, 0))
                            continue;

                        string txt = (fUnitFactor * IF_Value).ToString($"F{NumberOfDecimalPlaces}");
                        txt += " " + IFTypeUnit;
                        //string txt = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * IF_Value, 2))) + " " + vm.IFTypeUnit;
                        Drawing2D.DrawText(txt, points[c + 1].X, points[c + 1].Y, 0, FontSize, Brushes.SlateGray, DiagramCanvas);
                    }

                    Drawing2D.DrawPolygon(points, Brushes.LightSlateGray, Brushes.SlateGray, PenLineCap.Flat, PenLineCap.Flat, 1, 0.3, DiagramCanvas);

                    // Draw Member on the Internal forces polygon
                    DrawMember(model, DiagramCanvas, i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees, fmodelMarginLeft_x, fmodelBottomPosition_y, Brushes.Black, 1);
                }

                if (IncludeResults)
                {
                    DiagramCanvas.ToolTip = GetInternalForceText(IFtypeIndex);
                    canvases.Add(DiagramCanvas);
                }
            }

            return canvases;
        }

        private static List<Point> GetMemberInternalForcePoints(CModel model, List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> ListMemberDeflectionsInLoadCombinations,
            CMember member, CLoadCombination lcomb, int IFtypeIndex, double dInternalForceScale, double dInternalForceScale_user, float fReal_Model_Zoom_Factor, bool UseCRSCGeometricalAxes)
        {
            List<Point> listMemberInternalForcePoints = new List<Point>();

            if (ListMemberInternalForcesInLoadCombinations == null)
            {
                return listMemberInternalForcePoints; // Return empty list ???
            }

            // Draw positive forces on + side, positive moments on -side (positive values are on the side with tension fibre)
            // TO Ondrej, existuje este taka vec - strana tahaneho vlakna, na tu stranu sa vykresluju ohybove momenty s kladnou hodnotou
            // Da sa prutu prednastavit ako strana kde ma prut zapornu zvislu os v LCS, teda -z alebo zmenit a potom sa vnutorne sily kreslia prevratene +/-

            float fInternalForceSignFactor = -1; // TODO 191 - TO Ondrej Vnutorne sily z BFENet maju opacne znamienko, takze ich potrebujeme zmenit, alebo musime zaviest ine vykreslovanie pre momenty a ine pre sily

            const int iNumberOfResultsSections = 11;
            double[] xLocations_rel = new double[iNumberOfResultsSections];

            // Fill relative coordinates (x_rel)
            for (int s = 0; s < iNumberOfResultsSections; s++)
                xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

            designBucklingLengthFactors[] sBucklingLengthFactors;
            designMomentValuesForCb[] sMomentValuesforCb;
            basicInternalForces[] sBIF_x;
            basicDeflections[] sBDef_x;

            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(UseCRSCGeometricalAxes,
                member,
                lcomb,
                ListMemberInternalForcesInLoadCombinations,
                iNumberOfResultsSections,
                out sBucklingLengthFactors,
                out sMomentValuesforCb,
                out sBIF_x);

            if (ListMemberDeflectionsInLoadCombinations == null)
            {
                return listMemberInternalForcePoints; // Return empty list ???
            }

            CMemberResultsManager.SetMemberDeflectionsInLoadCombination(UseCRSCGeometricalAxes,
                member,
                lcomb,
                ListMemberDeflectionsInLoadCombinations,
                iNumberOfResultsSections,
                out sBDef_x);

            // First point (start at [0,0])
            listMemberInternalForcePoints.Add(new Point(0, 0));

            // Internal force diagram points
            for (int j = 0; j < sBIF_x.Length; j++) // For each member create list of points [x, IF value]
            {
                double xlocationCoordinate = fReal_Model_Zoom_Factor * xLocations_rel[j] * member.FLength;

                float IF_Value = fInternalForceSignFactor * GetInternalForcesValue(IFtypeIndex, sBIF_x[j], sBDef_x[j]);
                double xlocationValue = dInternalForceScale * dInternalForceScale_user * IF_Value;

                //pozicie x sa ulozia, aby sa nemuseli pocitat znova
                listMemberInternalForcePoints.Add(new Point(xlocationCoordinate, xlocationValue));
            }

            // Last point (end at [L,0])
            listMemberInternalForcePoints.Add(new Point(fReal_Model_Zoom_Factor * member.FLength, 0));

            return listMemberInternalForcePoints;
        }

        private static double GetBestScaleRatio(CModel model, List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> ListMemberDeflectionsInLoadCombinations,
            CMember[] members, CLoadCombination lcomb, int IFtypeIndex, double dInternalForceScale, float fReal_Model_Zoom_Factor, bool UseCRSCGeometricalAxes)
        {
            double maxLocationValue = 100;
            double minRatio = double.MaxValue;
            const int iNumberOfResultsSections = 11;

            designBucklingLengthFactors[] sBucklingLengthFactors;
            designMomentValuesForCb[] sMomentValuesforCb;
            basicInternalForces[] sBIF_x;
            basicDeflections[] sBDef_x;

            foreach (CMember member in members)
            {
                CMemberResultsManager.SetMemberInternalForcesInLoadCombination(UseCRSCGeometricalAxes,
                member,
                lcomb,
                ListMemberInternalForcesInLoadCombinations,
                iNumberOfResultsSections,
                out sBucklingLengthFactors,
                out sMomentValuesforCb,
                out sBIF_x);
                
                CMemberResultsManager.SetMemberDeflectionsInLoadCombination(UseCRSCGeometricalAxes,
                    member,
                    lcomb,
                    ListMemberDeflectionsInLoadCombinations,
                    iNumberOfResultsSections,
                    out sBDef_x);

                // Internal force diagram points
                for (int j = 0; j < sBIF_x.Length; j++) // For each member create list of points [x, IF value]
                {
                    float IF_Value = GetInternalForcesValue(IFtypeIndex, sBIF_x[j], sBDef_x[j]);
                    double ratio = maxLocationValue / Math.Abs(dInternalForceScale * IF_Value);
                    if (ratio < minRatio) minRatio = ratio;
                }
            }
            return minRatio;
        }

        private static float GetInternalForcesValue(int IFtypeIndex, basicInternalForces bif, basicDeflections bdef)
        {
            //"N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz"
            switch (IFtypeIndex)
            {
                case 0: return bif.fN;
                case 1: return bif.fV_zz; //bif.fV_zv???
                case 2: return bif.fV_yy; //bif.fV_yu???
                case 3: return bif.fT;
                case 4: return bif.fM_yy;
                case 5: return bif.fM_zz;
                case 6: return bdef.fDelta_yy;
                case 7: return bdef.fDelta_zz;
                default: throw new Exception($"Not known internal force; IFTypeIndex: {IFtypeIndex}");
            }
        }

        private static string GetInternalForceText(int IFtypeIndex)
        {
            //"N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz"
            switch (IFtypeIndex)
            {
                case 0: return "Axial Force N [kN]";
                case 1: return "Shear Force Vx [kN]";
                case 2: return "Shear Force Vy [kN]";
                case 3: return "Torsion Moment T [kNm]";
                case 4: return "Bending Moment Mx [kNm]";
                case 5: return "Bending Moment My [kNm]";
                case 6: return "Local Deflection δx [mm]";
                case 7: return "Local Deflection δy [mm]";
                default: throw new Exception($"Not known internal force; IFTypeIndex: {IFtypeIndex}");
            }
        }

        private static void DrawMember(CModel model, Canvas canvas, int memberIndex, float fReal_Model_Zoom_Factor, int factorSwitchYAxis, double rotAngle_degrees,
            float fmodelMarginLeft_x, float fmodelBottomPosition_y, SolidColorBrush color, double thickness)
        {
            // Draw member
            List<Point> listMemberPoints = new List<Point>(2);
            listMemberPoints.Add(new Point(0, 0));
            listMemberPoints.Add(new Point(fReal_Model_Zoom_Factor * model.m_arrMembers[memberIndex].FLength, 0));

            double translationOffxet_x = fmodelMarginLeft_x + fReal_Model_Zoom_Factor * model.m_arrMembers[memberIndex].NodeStart.X;
            double translationOffset_y = fmodelBottomPosition_y + fReal_Model_Zoom_Factor * factorSwitchYAxis * model.m_arrMembers[memberIndex].NodeStart.Z;

            RotateTransform rotateTransform = new RotateTransform(rotAngle_degrees, 0, 0); // + clockwise, - counter-clockwise
            TranslateTransform translateTransform = new TranslateTransform(translationOffxet_x, translationOffset_y);
            TransformGroup transformGroup_RandT = new TransformGroup();
            transformGroup_RandT.Children.Add(rotateTransform);
            transformGroup_RandT.Children.Add(translateTransform);

            List<Point> points = new List<Point>();
            foreach (Point p in listMemberPoints)
                points.Add(transformGroup_RandT.Transform(p));

            Drawing2D.DrawPolyLine(false, points, color, PenLineCap.Flat, PenLineCap.Flat, thickness, canvas);
        }


        public static string GetValueString(float value, string identificator, Dictionary<string, DataExportTables> allItemsDict, Dictionary<string, QuantityLibraryItem> quantityLibrary, NumberFormatInfo nfi)
        {
            DataExportTables item = null;
            allItemsDict.TryGetValue(identificator, out item);
            if (item == null) return value.ToString(nfi); //ak sa to tuna dostane,tak v podstate je nejaka chyba

            QuantityLibraryItem qlItem = null;
            quantityLibrary.TryGetValue(item.UnitIdentificator, out qlItem);
            if(qlItem == null) return value.ToString(nfi); //ak sa to tuna dostane,tak v podstate je nejaka chyba

            if (qlItem.ID == 1) return string.Empty; //Blank
            else return (value * qlItem.ReportUnitFactor).ToString($"F{qlItem.ReportDecimalPlaces}", nfi);
        }

    }
}
