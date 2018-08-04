using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MATH;
using BaseClasses;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_InternalForces.xaml
    /// </summary>
    public partial class UC_InternalForces : UserControl
    {
        public float modelMarginLeft_x = 10;
        public float modelMarginRight_x = 10;
        public float modelMarginTop_y = 10;
        public float modelMarginBottom_y = 10;
        public float modelBottomPosition_y;
        const float fCanvasHeight = 150; // Size of Canvas // Same size of of diagrams ???
        const float fCanvasWidth = 600;  // Size of Canvas
        float fMemberLength_xMax = 10; // Temporary - Member theoretical length [m], nacitat z CMember

        // Number of x-axis points (result sections)
        // TODO - napojit na results - metoda calculate click pre kazdy prut, kazdu CO (oad combination) a kazdy Limit State
        const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
        const int iNumberOfSegments = iNumberOfDesignSections - 1;

        float[] arrPointsCoordX = new float[iNumberOfDesignSections]; // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)

        public UC_InternalForces(CModel model, UC_ComponentList components)
        {
            InitializeComponent();

            // Add items into comboboxes
            FillComboboxValues(Combobox_LimitState, model.m_arrLimitStates);
            FillComboboxValues(Combobox_LoadCombination, model.m_arrLoadCombs);            
            FillComboboxValues(Combobox_ComponentType, components.MemberComponentName);

            // Set default values of combobox index
            Combobox_LimitState.SelectedIndex = 0;
            Combobox_LoadCombination.SelectedIndex = 0;

            modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;

            for (int i = 0; i < iNumberOfDesignSections; i++) // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)
                arrPointsCoordX[i] = ((float)i / (float)iNumberOfSegments) * fMemberLength_xMax; // Int must be converted to the float to get decimal numbers

            // Temporary Data
            float[] fArr_AxialForceValuesN = new float[iNumberOfDesignSections] { 10000, 10000, 9000, 0000, 0000, -3000, -3000, -4000, -4500, -5000, -5000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_ShearForceValuesVx = new float[iNumberOfDesignSections] { -10000, -8000, -3000, -5000, -5000, -4600, 0, -12000, -8000, -20000, -3000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_ShearForceValuesVy = new float[iNumberOfDesignSections] { 15000, 8000, 3000, 5000, 5000, 4600, 0, 12000, 8000, 20000, 5000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_TorsionMomentValuesT = new float[iNumberOfDesignSections] { 5000, 8000, 3000, 5000, 5000, 4600, -2000, -1000, -8000, -0, -1000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_BendingMomentValuesMx = new float[iNumberOfDesignSections] { 100, 8000, 30000, 50000, 20000, 14600, 13000, 12000, 8000, 0, 300 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_BendingMomentValuesMy = new float[iNumberOfDesignSections] { -500, -1000, -2000, -3000, -4000, -4600, -4800, -3000, -2000, 0, -200 }; // Temporary data - napojit na vysledky vypoctu

            // Draw axis (x, y)

            DrawAxisInCanvas(true, fArr_AxialForceValuesN , Canvas_AxialForceDiagram);
            DrawAxisInCanvas(true, fArr_ShearForceValuesVx, Canvas_ShearForceDiagramVx);
            DrawAxisInCanvas(true, fArr_ShearForceValuesVy, Canvas_ShearForceDiagramVy);

            DrawAxisInCanvas(false, fArr_TorsionMomentValuesT , Canvas_TorsionMomentDiagram);
            DrawAxisInCanvas(false, fArr_BendingMomentValuesMx, Canvas_BendingMomentDiagramMx);
            DrawAxisInCanvas(false, fArr_BendingMomentValuesMy, Canvas_BendingMomentDiagramMy);

            // TODO
            // Vysledky by mali byt v N a Nm (pocitame v zakladnych jednotkach SI), pre zobrazenie prekonvertovat na kN a kNm, pripadne pridat nastavenie jednotiek do GUI

            // Draw y values

            DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN , Canvas_AxialForceDiagram);
            DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, Canvas_ShearForceDiagramVx);
            DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, Canvas_ShearForceDiagramVy);

            DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT , Canvas_TorsionMomentDiagram);
            DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, Canvas_BendingMomentDiagramMx);
            DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, Canvas_BendingMomentDiagramMy);

            // Draw values description
            DrawTexts(fArr_AxialForceValuesN , arrPointsCoordX, fArr_AxialForceValuesN , Brushes.BlueViolet, Canvas_AxialForceDiagram);
            DrawTexts(fArr_ShearForceValuesVx, arrPointsCoordX, fArr_ShearForceValuesVx, Brushes.BlueViolet, Canvas_ShearForceDiagramVx);
            DrawTexts(fArr_ShearForceValuesVy, arrPointsCoordX, fArr_ShearForceValuesVy, Brushes.BlueViolet, Canvas_ShearForceDiagramVy);

            DrawTexts(fArr_TorsionMomentValuesT , arrPointsCoordX, fArr_TorsionMomentValuesT , Brushes.BlueViolet, Canvas_TorsionMomentDiagram);
            DrawTexts(fArr_BendingMomentValuesMx, arrPointsCoordX, fArr_BendingMomentValuesMx, Brushes.BlueViolet, Canvas_BendingMomentDiagramMx);
            DrawTexts(fArr_BendingMomentValuesMy, arrPointsCoordX, fArr_BendingMomentValuesMy, Brushes.BlueViolet, Canvas_BendingMomentDiagramMy);

        }

        public void FillComboboxValues(ComboBox combobox, CObject[] array)
        {
            if (array != null)
            {
                foreach (CObject obj in array)
                {
                    if (obj.Name != null || obj.Name != "")
                        combobox.Items.Add(obj.Name);
                    else
                    {
                        // Exception
                        MessageBox.Show("Object ID = " + obj.ID + "." + " Object name is not defined correctly.");
                    }
                }
            }
        }
        public void FillComboboxValues(ComboBox combobox, List<string> values)
        {
            foreach (string s in values)
            {
                combobox.Items.Add(s);
            }
        }

        // Todo Ondrej - vytvorit bazovu triedu pre kreslenie 2D objektov a grafov pristupnu vo vsetkych projektoch
        public void DrawAxisInCanvas(bool bYOrientationIsUp, float[] arrPointsCoordY, Canvas canvasForImage)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            float fPositionOfXaxisToTheEndOfYAxis = 0;

            // TODO No 44 Ondrej
            // Temporary - TODO Ondrej zjednotit metody pre vykreslovanie v 2D do nejakej zakladnej triedy (mozno uz nejaka aj existuje v inom projekte "SW_EN\GRAPHICS\PAINT" alebo swen_GUI\WindowPaint)

            if (bYOrientationIsUp) // Up (Forces N, Vx, Vy)
            {
                // x-axis (middle)
                fPositionOfXaxisToTheEndOfYAxis = yValueMax < 0 ? 0 : yValueMax;
                DrawPolyLine(new float[2] { 0, 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis, modelMarginLeft_x, fFactorX, fFactorY, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);

                // y-axis (oriented upwards)
                DrawPolyLine(new float[2] { 0, 0 }, new float[2] { yValueMin < 0 ? yValueMin : 0, yValueMax < 0 ? 0 : yValueMin + yRangeOfValues }, modelMarginTop_y, modelMarginLeft_x, fFactorX, fFactorY, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            }
            else // Down (Torsion and bending moments T, Mx, My)
            {
                fPositionOfXaxisToTheEndOfYAxis = yValueMin < 0 ? Math.Abs(yValueMin) : 0;
                // x-axis (middle)
                DrawPolyLine(new float[2] { 0, 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fFactorY * fPositionOfXaxisToTheEndOfYAxis, modelMarginLeft_x, fFactorX, fFactorY, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);

                // y-axis (oriented downwards)
                DrawPolyLine(new float[2] { 0, 0 }, new float[2] { yValueMin < 0 ? yValueMin : 0, yValueMax < 0 ? 0 : yValueMin + yRangeOfValues }, modelMarginTop_y, modelMarginLeft_x, fFactorX, fFactorY, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            }
        }

        public void DrawYValuesCurveInCanvas(bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, Canvas canvasForImage)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            if (arrPointsCoordY != null)
            {
                if (!bYOrientationIsUp) // Draw positive values bellow x-axis
                {
                    for (int i = 0; i < arrPointsCoordY.Length; i++)
                        arrPointsCoordY[i] *= -1f;
                }

                DrawPolyLine(arrPointsCoordX, arrPointsCoordY, modelMarginTop_y, modelMarginLeft_x, fFactorX, fFactorY, Brushes.Blue, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            }
        }

        public float CalculateZoomFactor(float[] arrPointsCoord, float fCanvasDimension, float fMarginValue1, float fMarginValue2, out float fValueMin, out float fValueMax, out float fRangeOfValues, out float fAxisLength)
        {
            GetMinAndMaxValue(arrPointsCoord, out fValueMin, out fValueMax);
 
            if (fValueMin * fValueMax < 0)
                fAxisLength = fRangeOfValues = Math.Abs(fValueMin) + Math.Abs(fValueMax);
            else if (fValueMax > 0)
            {
                fAxisLength = fValueMax;
                fRangeOfValues = fValueMax - fValueMin;
            }
            else
            {
                fAxisLength = Math.Abs(fValueMin);
                fRangeOfValues = fValueMax - fValueMin;
            }

            return (float)((fCanvasDimension - fMarginValue1 - fMarginValue2) / fAxisLength);
        }

        public void DrawPolyLine(float[] arrPointsCoordX, float[] arrPointsCoordY, double dCanvasTopTemp, double dCanvasLeftTemp, float fFactorX, float fFactorY, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < arrPointsCoordX.Length; i++)
            {
                points.Add(new Point(modelMarginLeft_x + fFactorX * arrPointsCoordX[i], modelBottomPosition_y - fFactorY * arrPointsCoordY[i]));
            }

            Polyline myLine = new Polyline();
            myLine.Stretch = Stretch.Fill;
            myLine.Stroke = color;
            myLine.Points = points;
            myLine.StrokeThickness = thickness;
            myLine.StrokeStartLineCap = startCap;
            myLine.StrokeEndLineCap = endCap;
            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(myLine, dCanvasTopTemp);
            Canvas.SetLeft(myLine, dCanvasLeftTemp);
            imageCanvas.Children.Add(myLine);
        }

        public void GetMinAndMaxValue(float[] arrValues, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            if (arrValues != null)
            {
                for (int i = 0; i < arrValues.Length; i++)
                {
                    if (arrValues[i] < min)
                        min = arrValues[i];

                    if (arrValues[i] > max)
                        max = arrValues[i];
                }
            }
        }

        // Funkcie su rovnake ako u windspeed, len je pridany parameter pre canvas a vypocet FactorX a FactorY

        public void DrawTexts(string[] array_text, float[] arrPointsCoordX, float fPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        {
            float[] arrPointsCoordY = new float[arrPointsCoordX.Length];
            for (int i = 0; i < arrPointsCoordY.Length; i++)
                arrPointsCoordY[i] = fPointsCoordY; // Fill array Items with same value

            DrawTexts(array_text, arrPointsCoordX, arrPointsCoordY, color, canvasForImage);
        }

        public void DrawTexts(string[] array_text, float fPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        {
            float[] arrPointsCoordX = new float[arrPointsCoordY.Length];
            for (int i = 0; i < arrPointsCoordX.Length; i++)
                arrPointsCoordX[i] = fPointsCoordX; // Fill array Items with same value

            DrawTexts(array_text, arrPointsCoordX, arrPointsCoordY, color, canvasForImage);
        }

        public void DrawTexts(string[] array_text, float[] arrPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        {
            float xValueMin, xValueMax, xRangeOfValues, xAxisLength;
            float fFactorX = CalculateZoomFactor(arrPointsCoordX, fCanvasWidth, modelMarginLeft_x, modelMarginRight_x, out xValueMin, out xValueMax, out xRangeOfValues, out xAxisLength);

            float yValueMin, yValueMax, yRangeOfValues, yAxisLength;
            float fFactorY = CalculateZoomFactor(arrPointsCoordY, fCanvasHeight, modelMarginTop_y, modelMarginBottom_y, out yValueMin, out yValueMax, out yRangeOfValues, out yAxisLength);

            for (int i = 0; i < array_text.Length; i++)
            {
                DrawText(array_text[i], modelMarginLeft_x + fFactorX * arrPointsCoordX[i], modelBottomPosition_y - fFactorY * arrPointsCoordY[i], 12, color, canvasForImage);
            }
        }

        public void DrawTexts(float[] array_ValuesToDisplay, float[] arrPointsCoordX, float fPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        {
            DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), arrPointsCoordX, fPointsCoordY, color, canvasForImage);
        }

        public void DrawTexts(float[] array_ValuesToDisplay, float fPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        {
            DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), fPointsCoordX, arrPointsCoordY, color, canvasForImage);
        }

        public void DrawTexts(float[] array_ValuesToDisplay, float[] arrPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        { 
            DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), arrPointsCoordX, arrPointsCoordY, color, canvasForImage);
        }

        public string[] ConvertArrayFloatToString(float[] array_float, int iDecimalPlaces = 3)
        {
            if (array_float != null)
            {
                string[] array_string = new string[array_float.Length];
                for (int i = 0; i < array_string.Length; i++)
                    array_string[i] = (Math.Round(array_float[i], iDecimalPlaces)).ToString();

                return array_string;

            }
            return null;
        }

        public void DrawText(string text, double posx, double posy, double fontSize, SolidColorBrush color, Canvas imageCanvas)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = color;
            Canvas.SetLeft(textBlock, posx);
            Canvas.SetTop(textBlock, posy);
            textBlock.Margin = new Thickness(2, 2, 0, 0);
            textBlock.FontSize = fontSize;
            imageCanvas.Children.Add(textBlock);
        }

    }
}
