﻿using System;
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

        public float fReal_Model_Zoom_FactorX;
        public float fReal_Model_Zoom_FactorY;

        float xValueMax = 10; // Temporary - Member theoretical length [m], nacitat z CMember

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
            // TODO Ondrej - fill combobox with UC_ComponentList Names
            //FillComboboxValues(Combobox_ComponentType, components);

            // Set default values of combobox index
            Combobox_LimitState.SelectedIndex = 0;
            Combobox_LoadCombination.SelectedIndex = 0;

            modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;

            for (int i = 0; i < iNumberOfDesignSections; i++) // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)
                arrPointsCoordX[i] = ((float)i / (float)iNumberOfSegments) * xValueMax; // Int must be converted to the float to get decimal numbers

            // Temporary Data
            float[] fArr_AxialForceValues = new float[iNumberOfDesignSections] { 10000, 10000, 9000, 0000, 0000, -3000, -3000, -4000, -4500, -5000, -5000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_ShearForceValuesVx = new float[iNumberOfDesignSections] { -10000, -8000, -3000, -5000, -5000, -4600, -2000, -12000, -8000, -20000, 3000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_ShearForceValuesVy = new float[iNumberOfDesignSections] { 15000, 8000, 3000, 5000, 5000, 4600, 2000, 12000, 8000, 20000, 5000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_TorsionMomentValues = new float[iNumberOfDesignSections] { 5000, 8000, 3000, 5000, 5000, 4600, -2000, -1000, -8000, -200, -1000 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_BendingMomentValuesMx = new float[iNumberOfDesignSections] { 100, 8000, 30000, 50000, 20000, 14600, 13000, 12000, 8000, 4000, 300 }; // Temporary data - napojit na vysledky vypoctu
            float[] fArr_BendingMomentValuesMy = new float[iNumberOfDesignSections] { -500, -1000, -2000, -3000, -4000, -4600, -4800, -3000, -2000, -1000, -200 }; // Temporary data - napojit na vysledky vypoctu

            // Draw axis (x, y)
            DrawAxisInCanvas(true, xValueMax, fArr_AxialForceValues, Canvas_AxialForceDiagram);
            DrawAxisInCanvas(true, xValueMax, fArr_ShearForceValuesVx, Canvas_ShearForceDiagramVx);
            DrawAxisInCanvas(true, xValueMax, fArr_ShearForceValuesVy, Canvas_ShearForceDiagramVy);
            DrawAxisInCanvas(false, xValueMax, fArr_TorsionMomentValues,Canvas_TorsionMomentDiagram);
            DrawAxisInCanvas(false, xValueMax, fArr_BendingMomentValuesMx, Canvas_BendingMomentDiagramMx);
            DrawAxisInCanvas(false, xValueMax, fArr_BendingMomentValuesMy, Canvas_BendingMomentDiagramMy);

            // TODO
            // Vysledky by mali byt v N a Nm (pocitame v zakladnych jednotkach SI), pre zobrazenie prekonvertovat na kN a kNm, pripadne pridat nastavenie jednotiek do GUI

            // Draw y values
            DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValues, Canvas_AxialForceDiagram);
            DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, Canvas_ShearForceDiagramVx);
            DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, Canvas_ShearForceDiagramVy);

            DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValues, Canvas_TorsionMomentDiagram);
            DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, Canvas_BendingMomentDiagramMx);
            DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, Canvas_BendingMomentDiagramMy);

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

        // Todo Ondrej - vytvorit bazovu triedu pre kreslenie 2D objektov a grafov pristupnu vo vsetkych projektoch
        public void DrawAxisInCanvas(bool bYOrientationIsUp, float xValueMax, float[] arrPointsCoordY, Canvas canvasForImage)
        {
            fReal_Model_Zoom_FactorX = (float)((fCanvasWidth - modelMarginLeft_x - modelMarginRight_x) / xValueMax);

            // y-values range (TODO - spocitat presne z pola hodnot, ak su vsetky kladne brat y_max, ak su vsetky zaporne tak abs(y_min)

            float yValueMin, yValueMax;
            GetMinAndMaxValue(arrPointsCoordY, out yValueMin, out yValueMax);

            float fYAxisLength;
            float fRangeOfYValues;

            if (yValueMin * yValueMax < 0)
                fYAxisLength = fRangeOfYValues = Math.Abs(yValueMin) + Math.Abs(yValueMax);
            else if (yValueMax > 0)
            {
                fYAxisLength = yValueMax;
                fRangeOfYValues = yValueMax - yValueMin;
            }
            else
            {
                fYAxisLength = Math.Abs(yValueMin);
                fRangeOfYValues = yValueMax - yValueMin;
            }

            fReal_Model_Zoom_FactorY = (float)((fCanvasHeight - modelMarginTop_y - modelMarginBottom_y) / fYAxisLength);

            // TODO No 44 Ondrej
            // Temporary - TODO Ondrej zjednotit metody pre vykreslovanie v 2D do nejakej zakladnej triedy (mozno uz nejaka aj existuje v inom projekte "SW_EN\GRAPHICS\PAINT" alebo swen_GUI\WindowPaint)

            if (bYOrientationIsUp) // Up (Forces N, Vx, Vy)
            {
                // x-axis (middle)
                DrawPolyLine(new float[2] { modelMarginLeft_x, modelMarginLeft_x + 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fReal_Model_Zoom_FactorY * Math.Abs(yValueMax), modelMarginLeft_x, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);

                // y-axis (oriented upwards)
                DrawPolyLine(new float[2] { modelMarginLeft_x, modelMarginLeft_x }, new float[2] { yValueMin, yValueMin + fRangeOfYValues }, modelMarginTop_y, modelMarginLeft_x, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            }
            else // Down (Torsion and bending moments T, Mx, My)
            {
                // x-axis (middle)
                DrawPolyLine(new float[2] { modelMarginLeft_x, modelMarginLeft_x + 1.02f * xValueMax }, new float[2] { 0, 0 }, modelMarginTop_y + fReal_Model_Zoom_FactorY * Math.Abs(yValueMin), modelMarginLeft_x, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);

                // y-axis (oriented downwards)
                DrawPolyLine(new float[2] { modelMarginLeft_x, modelMarginLeft_x }, new float[2] { yValueMin + fRangeOfYValues, yValueMin }, modelMarginTop_y, modelMarginLeft_x, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            }
        }

        public void DrawYValuesCurveInCanvas(bool bYOrientationIsUp, float[] arrPointsCoordX, float[] arrPointsCoordY, Canvas canvasForImage)
        {
            float yValueMin, yValueMax;
            GetMinAndMaxValue(arrPointsCoordY, out yValueMin, out yValueMax);

            float fYAxisLength;
            float fRangeOfYValues;

            if (yValueMin * yValueMax < 0)
                fYAxisLength = fRangeOfYValues = Math.Abs(yValueMin) + Math.Abs(yValueMax);
            else if (yValueMax > 0)
            {
                fYAxisLength = yValueMax;
                fRangeOfYValues = yValueMax - yValueMin;
            }
            else
            {
                fYAxisLength = Math.Abs(yValueMin);
                fRangeOfYValues = yValueMax - yValueMin;
            }

            fReal_Model_Zoom_FactorY = (float)((fCanvasHeight - modelMarginTop_y - modelMarginBottom_y) / fYAxisLength);

            if (arrPointsCoordY != null)
            {
                if (!bYOrientationIsUp) // Draw positive values bellow x-axis
                {
                    for (int i = 0; i < arrPointsCoordY.Length; i++)
                        arrPointsCoordY[i] *= -1f;
                }

                DrawPolyLine(arrPointsCoordX, arrPointsCoordY, modelMarginTop_y/*modelBottomPosition_y*/, modelMarginLeft_x, Brushes.Blue, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            }
        }

        public void DrawPolyLine(float[] arrPointsCoordX, float[] arrPointsCoordY, double dCanvasTopTemp, double dCanvasLeftTemp, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < arrPointsCoordX.Length; i++)
            {
                points.Add(new Point(modelMarginLeft_x + fReal_Model_Zoom_FactorX * arrPointsCoordX[i], modelBottomPosition_y - fReal_Model_Zoom_FactorY * arrPointsCoordY[i]));
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
    }
}
