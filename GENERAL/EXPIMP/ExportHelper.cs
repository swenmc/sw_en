using _3DTools;
using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
using FEM_CALC_BASE;
using MATH;
using CRSC;
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
using System.Windows.Media.Media3D;

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

        public static Stream GetCanvasStream(Canvas canvas)
        {
            RenderTargetBitmap bmp = RenderVisual(canvas);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));

            MemoryStream stm = new MemoryStream();
            png.Save(stm);
            stm.Position = 0;

            return stm;
        }
        public static Stream GetViewPortStream(Viewport3D viewPort)
        {
            RenderTargetBitmap bmp = RenderVisual(viewPort);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));

            MemoryStream stm = new MemoryStream();
            png.Save(stm);
            stm.Position = 0;

            return stm;
        }

        public static RenderTargetBitmap RenderVisual(UIElement elt)
        {
            Size size = new Size(elt.RenderSize.Width, elt.RenderSize.Height);
            elt.Measure(size);
            elt.Arrange(new Rect(size));
            elt.UpdateLayout();

            var bitmap = new RenderTargetBitmap(
                (int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);

            bitmap.Render(elt);
            return bitmap;
        }
        public static BitmapImage RenderVisual(UIElement elt, double scale)
        {
            Size size = new Size(elt.RenderSize.Width, elt.RenderSize.Height);
            elt.Measure(size);
            elt.Arrange(new Rect(size));
            elt.UpdateLayout();

            //foreach (ModelVisual3D line in ((Viewport3D)elt).Children)
            //{
            //    if (line is ScreenSpaceLines3D) ((ScreenSpaceLines3D)line).Rescale();
            //}
            //elt.UpdateLayout();

            var bitmap = new RenderTargetBitmap(
                (int)(size.Width * scale), (int)(size.Height * scale), 96 * scale, 96 * scale, PixelFormats.Default);
            
            bitmap.Render(elt);
            elt = null;

            // temp
            // tento kod si nepamatam naco tu je :-(
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            //SaveBitmapImage(bitmap, DateTime.Now.Ticks + ".png");
            return bitmapImage;
            //end temp
            //return bitmap;
        }

        // lcomb - Kombinacia ktorej vysledky chceme zobrazit
        public static List<Canvas> GetIFCanvases(bool IsULS, bool UseCRSCGeometricalAxes, CLoadCombination lcomb, CMember member, List<CMemberInternalForcesInLoadCombinations> listMemberLoadForces, List<CMemberDeflectionsInLoadCombinations> listMemberDeflections)
        {
            List<Canvas> canvases = new List<Canvas>();
            if (lcomb == null || member == null || listMemberLoadForces == null || listMemberDeflections == null) return canvases;
            
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
            
            float fCanvasHeight = 160;
            float fCanvasWidth = 625;
            float modelMarginLeft_x = 10;
            float modelMarginRight_x = 35;
            float modelMarginTop_y = 15;
            float modelMarginBottom_y = 15;
            float modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;            

            double limitForce = 1e-3; // 0.001 kN
            double limitMoment = 1e-3; // 0.001 kNm
            double limitDeflection = 1e-2; // 0.01 mm
            
            if (IsULS)
            {
                if (fArr_AxialForceValuesN.Any(n => Math.Abs(n) > limitForce))
                {
                    Canvas Canvas_AxialForceDiagram = new Canvas();
                    Canvas_AxialForceDiagram.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_AxialForceDiagram.Name = "AxialForce_N";
                    Canvas_AxialForceDiagram.ToolTip = "Axial Force N [kN]";
                    Drawing2D.DrawXYDiagramToCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
                    canvases.Add(Canvas_AxialForceDiagram);
                }
                if (fArr_ShearForceValuesVx.Any(n => Math.Abs(n) > limitForce))
                {
                    Canvas Canvas_ShearForceDiagramVx = new Canvas();
                    Canvas_ShearForceDiagramVx.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_ShearForceDiagramVx.Name = "ShearForce_Vx";
                    Canvas_ShearForceDiagramVx.ToolTip = "Shear Force Vx [kN]";
                    Drawing2D.DrawXYDiagramToCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, 
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
                    canvases.Add(Canvas_ShearForceDiagramVx);
                }
                if (fArr_ShearForceValuesVy.Any(n => Math.Abs(n) > limitForce))
                {
                    Canvas Canvas_ShearForceDiagramVy = new Canvas();
                    Canvas_ShearForceDiagramVy.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_ShearForceDiagramVy.Name = "ShearForce_Vy";
                    Canvas_ShearForceDiagramVy.ToolTip = "Shear Force Vy [kN]";
                    Drawing2D.DrawXYDiagramToCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, 
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);
                    canvases.Add(Canvas_ShearForceDiagramVy);
                }
                if (fArr_TorsionMomentValuesT.Any(n => Math.Abs(n) > limitMoment))
                {
                    Canvas Canvas_TorsionMomentDiagram = new Canvas();
                    Canvas_TorsionMomentDiagram.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_TorsionMomentDiagram.Name = "TorsionMoment_T";
                    Canvas_TorsionMomentDiagram.ToolTip = "Torsion Moment T [kNm]";
                    Drawing2D.DrawXYDiagramToCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, 
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
                    canvases.Add(Canvas_TorsionMomentDiagram);
                }
                if (fArr_BendingMomentValuesMx.Any(n => Math.Abs(n) > limitMoment))
                {
                    Canvas Canvas_BendingMomentDiagramMx = new Canvas();
                    Canvas_BendingMomentDiagramMx.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_BendingMomentDiagramMx.Name = "BendingMoment_Mx";
                    Canvas_BendingMomentDiagramMx.ToolTip = "Bending Moment Mx [kNm]";
                    Drawing2D.DrawXYDiagramToCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, 
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
                    canvases.Add(Canvas_BendingMomentDiagramMx);
                }
                if (fArr_BendingMomentValuesMy.Any(n => Math.Abs(n) > limitMoment))
                {
                    Canvas Canvas_BendingMomentDiagramMy = new Canvas();
                    Canvas_BendingMomentDiagramMy.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_BendingMomentDiagramMy.Name = "BendingMoment_My";
                    Canvas_BendingMomentDiagramMy.ToolTip = "Bending Moment My [kNm]";
                    Drawing2D.DrawXYDiagramToCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, 
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);
                    canvases.Add(Canvas_BendingMomentDiagramMy);
                }
            }
            else if (!IsULS)
            {   
                if (fArr_DeflectionValuesDeltax.Any(n => Math.Abs(n) > limitDeflection))
                {
                    Canvas Canvas_DeflectionDiagramDeltax = new Canvas();
                    Canvas_DeflectionDiagramDeltax.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_DeflectionDiagramDeltax.Name = "LocalDeflection_Delta_x";
                    Canvas_DeflectionDiagramDeltax.ToolTip = "Local Deflection δx [mm]";
                    Drawing2D.DrawXYDiagramToCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltax, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x,
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltax);
                    canvases.Add(Canvas_DeflectionDiagramDeltax);
                }
                if (fArr_DeflectionValuesDeltay.Any(n => Math.Abs(n) > limitDeflection))
                {
                    Canvas Canvas_DeflectionDiagramDeltay = new Canvas();
                    Canvas_DeflectionDiagramDeltay.RenderSize = new Size(fCanvasWidth, fCanvasHeight);
                    Canvas_DeflectionDiagramDeltay.Name = "LocalDeflection_Delta_y";
                    Canvas_DeflectionDiagramDeltay.ToolTip = "Local Deflection δy [mm]";
                    Drawing2D.DrawXYDiagramToCanvas(false, arrPointsCoordX, fArr_DeflectionValuesDeltay, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x,
                        modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_DeflectionDiagramDeltay);
                    canvases.Add(Canvas_DeflectionDiagramDeltay);
                }
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
                        double dTextWidth;
                        Drawing2D.DrawText(txt, points[c + 1].X, points[c + 1].Y, 0, FontSize, Brushes.SlateGray, DiagramCanvas, out dTextWidth);
                    }
                    
                    Drawing2D.DrawPolygon(points, Brushes.LightSlateGray, Brushes.SlateGray, PenLineCap.Flat, PenLineCap.Flat, 1, 0.3, DiagramCanvas);
                    
                    // Draw Member on the Internal forces polygon
                    DrawMember(model, DiagramCanvas, i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees, fmodelMarginLeft_x, fmodelBottomPosition_y, Brushes.Black, 1);
                }

                if (IncludeResults)
                {
                    Drawing2D.DetectAndResolveTextColisions(DiagramCanvas);
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
                        double dTextWidth;
                        Drawing2D.DrawText(txt, points[c + 1].X, points[c + 1].Y, 0, FontSize, Brushes.SlateGray, DiagramCanvas, out dTextWidth);
                    }

                    Drawing2D.DrawPolygon(points, Brushes.LightSlateGray, Brushes.SlateGray, PenLineCap.Flat, PenLineCap.Flat, 1, 0.3, DiagramCanvas);

                    // Draw Member on the Internal forces polygon
                    DrawMember(model, DiagramCanvas, i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees, fmodelMarginLeft_x, fmodelBottomPosition_y, Brushes.Black, 1);
                }

                if (IncludeResults)
                {
                    Drawing2D.DetectAndResolveTextColisions(DiagramCanvas);
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


        private static CConnectionJointTypes GetFirstSameJointFromModel(CConnectionJointTypes joint, CModel model)
        {
            foreach (CConnectionJointTypes j in model.m_arrConnectionJoints)
            {
                if (joint.JointType == j.JointType) return j;
            }
            return joint;
        }
        
        public static Viewport3D GetJointViewPort(CConnectionJointTypes joint, DisplayOptions sDisplayOptions, CModel model, out Trackport3D _trackport, double width = 570, double height = 430)
        {
            CConnectionJointTypes firstSameJoint = GetFirstSameJointFromModel(joint, model);
            
            CConnectionJointTypes jointClone = firstSameJoint.Clone();
            
            float fMainMemberLength = 0;
            float fSecondaryMemberLength = 0;

            for (int i = 0; i < jointClone.m_arrPlates.Length; i++)
            {
                fMainMemberLength = Math.Max(jointClone.m_arrPlates[i].Width_bx, jointClone.m_arrPlates[i].Height_hy);
                fSecondaryMemberLength = fMainMemberLength;
            }

            float fMainMemberLengthFactor = 1.1f;      // Upravi dlzku urcenu z maximalneho rozmeru plechu
            float fSecondaryMemberLengthFactor = 1.1f; // Upravi dlzku urcenu z maximalneho rozmeru plechu // Bug 320 - Musi byt rovnake ako main member kvoli plechu Apex - jeden rafter je main, jeden je secondary

            fMainMemberLength *= fMainMemberLengthFactor;
            fSecondaryMemberLength *= fSecondaryMemberLengthFactor;

            CModel jointModel = new CModel();

            int iNumberMainMembers = 0;
            int iNumberSecondaryMembers = 0;

            if (jointClone.m_MainMember != null)
                iNumberMainMembers = 1;

            if (jointClone.m_SecondaryMembers != null)
                iNumberSecondaryMembers = jointClone.m_SecondaryMembers.Length;

            jointModel.m_arrMembers = new CMember[iNumberMainMembers + iNumberSecondaryMembers];

            // Main Member
            if (jointClone.m_MainMember != null)
            {
                CMember m = jointClone.m_MainMember;
                CNode nodeJoint = jointClone.m_Node; // Joint Node
                CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta
                float fX;
                float fY;
                float fZ;

                if (jointClone.m_Node.ID == m.NodeStart.ID)
                {
                    nodeOtherEnd = m.NodeEnd;
                    m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                    fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                    fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                    fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                    v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)v.X;
                    fY = (float)v.Y;
                    fZ = (float)v.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                    nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                    nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                }
                else if (jointClone.m_Node.ID == m.NodeEnd.ID)
                {
                    nodeOtherEnd = m.NodeStart;
                    m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                    fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                    fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                    fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                    v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)v.X;
                    fY = (float)v.Y;
                    fZ = (float)v.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                    nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                    nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                }
                else
                {
                    fMainMemberLength *= 2; // Zdvojnasobime vykreslovanu dlzku pruta kedze vykreslujeme na 2 strany od nodeJoint

                    // Relativny priemet casti pruta medzi zaciatocnym uzlom a uzlom spoja do GCS
                    fX = (m.NodeStart.X - nodeJoint.X) / m.FLength;
                    fY = (m.NodeStart.Y - nodeJoint.Y) / m.FLength;
                    fZ = (m.NodeStart.Z - nodeJoint.Z) / m.FLength;

                    // TO Ondrej - ak je prut velmi dlhy a fX az fZ su velmi male cisla, tak pre pripad ze jointNode je blizko k Start alebo End Node hlavneho pruta
                    // vyjde vzdialenost (fX, resp. fY, fZ) * fMainMemberLength velmi mala
                    // Urobil som to tak ze urcim vektor z absolutnych dlzok priemetu a potom ho normalizujem, takze absolutna vzdialenost priemetu nodeJoint a m.NodeStart, resp. m.NodeEnd
                    // by nemala hrat rolu, mozes sa na to pozriet. Mozno Ta napadne nieco elegantnejsie, rozdiel vidiet napriklad pri spoji girt to Main column

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D vStart = new Vector3D(m.NodeStart.X - nodeJoint.X, m.NodeStart.Y - nodeJoint.Y, m.NodeStart.Z - nodeJoint.Z);
                    vStart.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)vStart.X;
                    fY = (float)vStart.Y;
                    fZ = (float)vStart.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    // Nastavenie novych suradnic - zaciatok skrateneho (orezaneho) pruta
                    m.NodeStart.X = nodeJoint.X + fX * fMainMemberLength / 2;
                    m.NodeStart.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                    m.NodeStart.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                    // Relativny priemet casti pruta medzi uzlom spoja a koncovym uzlom do GCS
                    fX = (m.NodeEnd.X - nodeJoint.X) / m.FLength;
                    fY = (m.NodeEnd.Y - nodeJoint.Y) / m.FLength;
                    fZ = (m.NodeEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D vEnd = new Vector3D(m.NodeEnd.X - nodeJoint.X, m.NodeEnd.Y - nodeJoint.Y, m.NodeEnd.Z - nodeJoint.Z);
                    vEnd.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)vEnd.X;
                    fY = (float)vEnd.Y;
                    fZ = (float)vEnd.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    // Nastavenie novych suradnic - koniec skrateneho (orezaneho) pruta
                    m.NodeEnd.X = nodeJoint.X + fX * fMainMemberLength / 2;
                    m.NodeEnd.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                    m.NodeEnd.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                    m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    m.FAlignment_End = 0;   // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                }
                
                m.Fill_Basic();

                jointModel.m_arrMembers[0] = m; // Set new member (member array)
                jointClone.m_MainMember = m; // Set new member (joint)                
            }

            // Secondary members
            if (jointClone.m_SecondaryMembers != null)
            {
                for (int i = 0; i < jointClone.m_SecondaryMembers.Length; i++)
                {
                    CMember m = jointClone.m_SecondaryMembers[i];

                    CNode nodeJoint = jointClone.m_Node; // Joint Node
                    CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta

                    if (jointClone.m_Node.ID == m.NodeStart.ID)
                    {
                        nodeOtherEnd = m.NodeEnd;
                        m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    }
                    else
                    {
                        nodeOtherEnd = m.NodeStart;
                        m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    }

                    float fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                    float fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                    float fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                    v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)v.X;
                    fY = (float)v.Y;
                    fZ = (float)v.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    nodeOtherEnd.X = nodeJoint.X + fX * fSecondaryMemberLength;
                    nodeOtherEnd.Y = nodeJoint.Y + fY * fSecondaryMemberLength;
                    nodeOtherEnd.Z = nodeJoint.Z + fZ * fSecondaryMemberLength;

                    m.Fill_Basic();

                    jointModel.m_arrMembers[1 + i] = m; // Set new member (member array)
                    jointClone.m_SecondaryMembers[i] = m; // Set new member (joint)
                }
            }

            List<CNode> nodeList = new List<CNode>();

            for (int i = 0; i < jointModel.m_arrMembers.Length; i++)
            {
                // Pridavat len uzly ktore este neboli pridane
                if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeStart) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeStart);
                if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeEnd) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeEnd);
            }
            jointModel.m_arrNodes = nodeList.ToArray();

            // Prutom musime niekde updatetovat wire frame positions
            if (sDisplayOptions.bDisplayWireFrameModel)
            {
                Drawing3D.UpdateWireFramePoints(jointModel, sDisplayOptions);
            }

            //--------------------------------------------------------------------------------------------------------------------------------------
            jointClone = firstSameJoint.RecreateJoint();
            jointClone.m_arrPlates = firstSameJoint.m_arrPlates;
            
            jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { jointClone };
            
            _trackport = new Trackport3D();
            _trackport.Background = new SolidColorBrush(sDisplayOptions.backgroundColor);
            _trackport.Width = width;
            _trackport.Height = height;
            _trackport.ViewPort.RenderSize = new Size(width, height);
            
            Size size = new Size(_trackport.ViewPort.RenderSize.Width, _trackport.ViewPort.RenderSize.Height);
            _trackport.ViewPort.Measure(size);
            _trackport.ViewPort.Arrange(new Rect(size));
            
            CJointHelper.SetJoinModelRotationDisplayOptions(firstSameJoint, ref sDisplayOptions);
            Drawing3D.DrawJointToTrackPort(_trackport, jointModel, sDisplayOptions);
            return _trackport.ViewPort;
        }


        public static Viewport3D GetFootingViewPort(CConnectionJointTypes joint, CFoundation pad, DisplayOptions sDisplayOptions, out Trackport3D _trackport, double width = 570, double height = 430)
        {
            CConnectionJointTypes jointClone = joint.Clone();
            CFoundation padClone = pad.Clone();

            CModel jointModel = null;

            if (jointClone != null)
            {
                // TO Ondrej - tuto funkciu treba trosku ucesat, refaktorovat casti kodu pre Main Member a cast pre Secondary Members

                // Problem 1 - ani jeden z uzlov pruta, ktore patria ku joint nekonci v spoji (vyskytuje sa najma pre main member, napr purlin pripojena k rafter)
                // TODO - problem je u main members ak nekoncia v uzle spoja
                // TODO - potrebujeme vytvorit funkciu (Drawing3D.cs PointLiesOnLineSegment), ktora najde vsetky "medzilahle" uzly, ktore lezia na prute, naplni nejaky zoznam uzlov v objekte pruta (List<CNode>IntermediateNodes)
                // TODO - potom vieme pre Main Member zistit, ktory z tychto uzlov je joint Node a vykreslit segment main member na jednu a na druhu stranu od tohto uzla

                // Problem 2 - joint nema spravne definovany main member (definovany je napr. ako main member prut rovnakeho typu s najnizsim ID)
                // TODO - vyssie uvedeny zoznam medzilahlych uzlov na prute vieme pouzit aj na to ze ak Main member nie je skutocny main member prisluchajuci ku spoju ale len prvy prut rovnakeho typu, 
                // tak mozeme najst taky prut, ktory ma v zozname IntermediateNodes joint.m_Node
                // a zaroven je rovnakeho typu ako main member, to by mal byt skutocny main member, ktory patri k joint.m_Node a mozeme ho nahradit
                // tento problem by sme mali riesit uz niekde pred touto funkciou, idealne uz pri vytvarani spojov v CModel_PFD_01_GR.cs

                float fMainMemberLength = 0;
                float fSecondaryMemberLength = 0;

                for (int i = 0; i < jointClone.m_arrPlates.Length; i++)
                {
                    fMainMemberLength = Math.Max(jointClone.m_arrPlates[i].Width_bx, jointClone.m_arrPlates[i].Height_hy);
                    fSecondaryMemberLength = fMainMemberLength;
                }

                float fMainMemberLengthFactor = 1.1f;      // Upravi dlzku urcenu z maximalneho rozmeru plechu
                float fSecondaryMemberLengthFactor = 1.1f; // Upravi dlzku urcenu z maximalneho rozmeru plechu // Bug 320 - Musi byt rovnake ako main member kvoli plechu Apex - jeden rafter je main, jeden je secondary

                fMainMemberLength *= fMainMemberLengthFactor;
                fSecondaryMemberLength *= fSecondaryMemberLengthFactor;

                jointModel = new CModel();

                //jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { joint };

                int iNumberMainMembers = 0;
                int iNumberSecondaryMembers = 0;

                if (jointClone.m_MainMember != null)
                    iNumberMainMembers = 1;

                if (jointClone.m_SecondaryMembers != null)
                    iNumberSecondaryMembers = jointClone.m_SecondaryMembers.Length;

                jointModel.m_arrMembers = new CMember[iNumberMainMembers + iNumberSecondaryMembers];

                // Main Member
                if (jointClone.m_MainMember != null)
                {
                    CMember m = jointClone.m_MainMember;

                    CNode nodeJoint = jointClone.m_Node; // Joint Node
                    CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta
                    float fX;
                    float fY;
                    float fZ;

                    if (jointClone.m_Node.ID == m.NodeStart.ID)
                    {
                        nodeOtherEnd = m.NodeEnd;
                        m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                        fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                        fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                        fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                        v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)v.X;
                        fY = (float)v.Y;
                        fZ = (float)v.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                        nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                        nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                    }
                    else if (jointClone.m_Node.ID == m.NodeEnd.ID)
                    {
                        nodeOtherEnd = m.NodeStart;
                        m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                        fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                        fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                        fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                        v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)v.X;
                        fY = (float)v.Y;
                        fZ = (float)v.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                        nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                        nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                    }
                    else
                    {
                        fMainMemberLength *= 2; // Zdvojnasobime vykreslovanu dlzku pruta kedze vykreslujeme na 2 strany od nodeJoint

                        // Relativny priemet casti pruta medzi zaciatocnym uzlom a uzlom spoja do GCS
                        fX = (m.NodeStart.X - nodeJoint.X) / m.FLength;
                        fY = (m.NodeStart.Y - nodeJoint.Y) / m.FLength;
                        fZ = (m.NodeStart.Z - nodeJoint.Z) / m.FLength;

                        // TO Ondrej - ak je prut velmi dlhy a fX az fZ su velmi male cisla, tak pre pripad ze jointNode je blizko k Start alebo End Node hlavneho pruta
                        // vyjde vzdialenost (fX, resp. fY, fZ) * fMainMemberLength velmi mala
                        // Urobil som to tak ze urcim vektor z absolutnych dlzok priemetu a potom ho normalizujem, takze absolutna vzdialenost priemetu nodeJoint a m.NodeStart, resp. m.NodeEnd
                        // by nemala hrat rolu, mozes sa na to pozriet. Mozno Ta napadne nieco elegantnejsie, rozdiel vidiet napriklad pri spoji girt to Main column

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D vStart = new Vector3D(m.NodeStart.X - nodeJoint.X, m.NodeStart.Y - nodeJoint.Y, m.NodeStart.Z - nodeJoint.Z);
                        vStart.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)vStart.X;
                        fY = (float)vStart.Y;
                        fZ = (float)vStart.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        // Nastavenie novych suradnic - zaciatok skrateneho (orezaneho) pruta
                        m.NodeStart.X = nodeJoint.X + fX * fMainMemberLength / 2;
                        m.NodeStart.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                        m.NodeStart.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                        // Relativny priemet casti pruta medzi uzlom spoja a koncovym uzlom do GCS
                        fX = (m.NodeEnd.X - nodeJoint.X) / m.FLength;
                        fY = (m.NodeEnd.Y - nodeJoint.Y) / m.FLength;
                        fZ = (m.NodeEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D vEnd = new Vector3D(m.NodeEnd.X - nodeJoint.X, m.NodeEnd.Y - nodeJoint.Y, m.NodeEnd.Z - nodeJoint.Z);
                        vEnd.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)vEnd.X;
                        fY = (float)vEnd.Y;
                        fZ = (float)vEnd.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        // Nastavenie novych suradnic - koniec skrateneho (orezaneho) pruta
                        m.NodeEnd.X = nodeJoint.X + fX * fMainMemberLength / 2;
                        m.NodeEnd.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                        m.NodeEnd.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                        m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                        m.FAlignment_End = 0;   // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    }

                    m.Fill_Basic();

                    jointModel.m_arrMembers[0] = m; // Set new member (member array)
                    jointClone.m_MainMember = m; // Set new member (joint)
                }

                // Secondary members
                if (jointClone.m_SecondaryMembers != null)
                {
                    for (int i = 0; i < jointClone.m_SecondaryMembers.Length; i++)
                    {
                        CMember m = jointClone.m_SecondaryMembers[i];

                        CNode nodeJoint = jointClone.m_Node; // Joint Node
                        CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta

                        if (jointClone.m_Node.ID == m.NodeStart.ID)
                        {
                            nodeOtherEnd = m.NodeEnd;
                            m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                        }
                        else
                        {
                            nodeOtherEnd = m.NodeStart;
                            m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                        }

                        float fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                        float fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                        float fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                        v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)v.X;
                        fY = (float)v.Y;
                        fZ = (float)v.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        nodeOtherEnd.X = nodeJoint.X + fX * fSecondaryMemberLength;
                        nodeOtherEnd.Y = nodeJoint.Y + fY * fSecondaryMemberLength;
                        nodeOtherEnd.Z = nodeJoint.Z + fZ * fSecondaryMemberLength;

                        m.Fill_Basic();

                        jointModel.m_arrMembers[1 + i] = m; // Set new member (member array)
                        jointClone.m_SecondaryMembers[i] = m; // Set new member (joint)
                    }
                }

                List<CNode> nodeList = new List<CNode>();

                for (int i = 0; i < jointModel.m_arrMembers.Length; i++)
                {
                    // Pridavat len uzly ktore este neboli pridane
                    if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeStart) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeStart);
                    if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeEnd) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeEnd);
                }
                jointModel.m_arrNodes = nodeList.ToArray();

                // Prutom musime niekde updatetovat wire frame positions
                if (sDisplayOptions.bDisplayWireFrameModel)
                {
                    Drawing3D.UpdateWireFramePoints(jointModel, sDisplayOptions);
                }

                //--------------------------------------------------------------------------------------------------------------------------------------
                jointClone = joint.RecreateJoint();
                jointClone.m_arrPlates = joint.m_arrPlates;

                jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { jointClone };
            }

            // Footing Pad
            if (padClone != null)
            {
                if (jointModel == null)
                    jointModel = new CModel();

                jointModel.m_arrFoundations = new List<CFoundation>();
                jointModel.m_arrFoundations.Add(padClone);
            }

            _trackport = new Trackport3D();
            _trackport.Background = new SolidColorBrush(sDisplayOptions.backgroundColor);
            _trackport.Width = width;
            _trackport.Height = height;
            _trackport.ViewPort.RenderSize = new Size(width, height);
            
            Size size = new Size(_trackport.ViewPort.RenderSize.Width, _trackport.ViewPort.RenderSize.Height);
            _trackport.ViewPort.Measure(size);
            _trackport.ViewPort.Arrange(new Rect(size));
            
            Drawing3D.DrawFootingToTrackPort(_trackport, jointModel, sDisplayOptions);
            return _trackport.ViewPort;
        }

        public static Viewport3D GetBaseModelViewPort(DisplayOptions sDisplayOptions, CModelData modelData, out CModel filteredModel, out Trackport3D _trackport, double width = 1400, double height = 1000)
        {
            _trackport = new Trackport3D();
            _trackport.Background = new SolidColorBrush(sDisplayOptions.backgroundColor);
            //_trackport.Width = 2800;
            //_trackport.Height = 2000;
            //_trackport.ViewPort.RenderSize = new Size(2800, 2000);
            _trackport.Width = width;
            _trackport.Height = height;
            _trackport.ViewPort.RenderSize = new Size(width, height);

            //Size size = new Size(_trackport.ViewPort.RenderSize.Width, _trackport.ViewPort.RenderSize.Height);
            //_trackport.ViewPort.Measure(size);
            //_trackport.ViewPort.Arrange(new Rect(size));
            //_trackport.ViewPort.UpdateLayout();

            filteredModel = Drawing3D.DrawToTrackPort(_trackport, modelData.Model, sDisplayOptions, null, modelData.JointsDict);

            //todo skusit refaktorovat Trackport3D a vyrobit mu nejaku dispose metodu na uvolennei pamate
            //pripadne skusit stale pouzivat jeden Trackport napriec celym exportom a len mu mazat model a viewport

            return _trackport.ViewPort;
        }

        public static DisplayOptions GetDisplayOptionsForMainModelExport(CModelData data, bool bCenterLinesMemberModelAndIDs = false)
        {
            DisplayOptions opts = data.DisplayOptions; // Display properties pre export do PDF - TO Ondrej - mohla by to byt samostatna sada nastaveni nezavisla na 3D scene
            opts.bUseOrtographicCamera = false;
            opts.bColorsAccordingToMembers = false;
            opts.bColorsAccordingToSections = true;
            opts.bDisplayGlobalAxis = false;
            opts.bDisplayMemberDescription = false;
            opts.ModelView = (int)EModelViews.ISO_FRONT_RIGHT;
            opts.ViewModelMembers = (int)EViewModelMemberFilters.All;
            opts.bDisplaySolidModel = true;
            opts.bDisplayMembersCenterLines = false;
            opts.bDisplayWireFrameModel = false; //musi byt false, lebo to je neskutocne vela dat a potom OutOfMemory Exception
            opts.bTransformScreenLines3DToCylinders3D = true;

            opts.bDisplayMembers = true;
            opts.bDisplayJoints = true;
            opts.bDisplayPlates = true;

            opts.bDisplayNodes = false;
            opts.bDisplayNodesDescription = false;
            opts.bDisplayNodalSupports = false;

            opts.bDisplayFoundations = false;
            opts.bDisplayFloorSlab = false;
            opts.bDisplaySawCuts = false;
            opts.bDisplayControlJoints = false;

            opts.bDisplayFoundationsDescription = false;
            opts.bDisplayFloorSlabDescription = false;
            opts.bDisplaySawCutsDescription = false;
            opts.bDisplayControlJointsDescription = false;

            opts.bDisplayGridlines = false;
            opts.bDisplaySectionSymbols = false;
            opts.bDisplayDetailSymbols = false;

            opts.bCreateHorizontalGridlines = true;
            opts.bCreateVerticalGridlinesFront = false;
            opts.bCreateVerticalGridlinesBack = false;
            opts.bCreateVerticalGridlinesLeft = false;
            opts.bCreateVerticalGridlinesRight = false;

            if (bCenterLinesMemberModelAndIDs) // Prenastavujeme hodnoty pre centerline model a zobrazene member IDs
            {
                opts.bDisplaySolidModel = false;
                opts.bDisplayMembersCenterLines = true;

                opts.bDisplayJoints = false;
                opts.bDisplayPlates = false;

                opts.bDisplayMemberDescription = true;
                opts.bDisplayMemberID = true;
                opts.bDisplayMemberCrossSectionStartName = false;
                opts.bDisplayMemberPrefix = false;
                opts.bDisplayMemberRealLength = false;
                opts.bDisplayMemberRealLengthInMM = false;
                opts.bDisplayMemberRealLengthUnit = false;

                opts.MemberDescriptionTextColor = Colors.Black;
                opts.memberCenterlineColor = Colors.Black;
            }

            return opts;
        }
        

    }
}
