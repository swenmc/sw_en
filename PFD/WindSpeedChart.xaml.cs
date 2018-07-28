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
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using sw_en_GUI;
using BaseClasses;
using MATH;
using M_EC1;
using M_EC1.AS_NZS;

namespace PFD
{
    /// <summary>
    /// Interaction logic for WindSpeedChart.xaml
    /// </summary>
    public partial class WindSpeedChart : Window
    {
        public Canvas Canvas2D = null;
        public CCalcul_1170_2 windCalculationData;
        public float modelMarginLeft_x;
        public float modelMarginRight_x;
        public float modelMarginTop_y;
        public float modelMarginBottom_y;
        public float modelBottomPosition_y;
        public float fReal_Model_Zoom_FactorX;
        public float fReal_Model_Zoom_FactorY;
        public double dWidth;
        public double dHeight;

        float[] farrayValuesX;
        float[] farrayValuesY1;
        float[] farrayValuesY2;

        string[] arrayTextX1_CardinalDirections;

        public WindSpeedChart(CCalcul_1170_2 windCalculationData_temp)
        {
            InitializeComponent();

            windCalculationData = windCalculationData_temp;

            dWidth = this.Width;
            dHeight = this.Height;

            modelMarginLeft_x = 10;
            modelMarginRight_x = 10;
            modelMarginTop_y = 50;
            modelMarginBottom_y = 10;
            modelBottomPosition_y = (float)dHeight - modelMarginBottom_y;

            arrayTextX1_CardinalDirections = windCalculationData.sWindDirections_9;

            bool bUse_9_Values = true;
            bool bDisplayFactor_M_D = false;

            if (bUse_9_Values)
                farrayValuesX = windCalculationData.fM_D_array_angles_9;
            else
                farrayValuesX = windCalculationData.fM_D_array_angles_360;

            fReal_Model_Zoom_FactorX = (float)((dWidth - modelMarginLeft_x - modelMarginRight_x) / farrayValuesX[farrayValuesX.Length - 1]);

            if (bDisplayFactor_M_D)
            {
                if (bUse_9_Values)
                    farrayValuesY1 = windCalculationData.fM_D_array_values_9;
                else
                    farrayValuesY1 = windCalculationData.fM_D_array_values_360;

                farrayValuesY2 = null;

                fReal_Model_Zoom_FactorY = (float)((dHeight - modelMarginTop_y - modelMarginTop_y) / farrayValuesY1[0]);
            }
            else
            {
                if (bUse_9_Values)
                {
                    farrayValuesY1 = windCalculationData.fV_sit_ULS_Theta_9;
                    farrayValuesY2 = windCalculationData.fV_sit_SLS_Theta_9;
                }
                else
                {
                    farrayValuesY1 = windCalculationData.fV_sit_ULS_Theta_360;
                    farrayValuesY2 = windCalculationData.fV_sit_SLS_Theta_360;
                }

                float fReal_Model_Zoom_FactorY_temp1 = (float)((dHeight - modelMarginTop_y - modelMarginTop_y) / farrayValuesY1[0]);
                float fReal_Model_Zoom_FactorY_temp2 = (float)((dHeight - modelMarginTop_y - modelMarginTop_y) / farrayValuesY2[0]);

                fReal_Model_Zoom_FactorY = MathF.Min(fReal_Model_Zoom_FactorY_temp1, fReal_Model_Zoom_FactorY_temp2);
            }

            // Values
            if (farrayValuesY1 != null)
                DrawPolyLine(farrayValuesX, farrayValuesY1, modelMarginTop_y, modelMarginLeft_x, Brushes.Red, new PenLineCap(), new PenLineCap(), 1, canvasForImage);

            if(farrayValuesY2 != null)
                DrawPolyLine(farrayValuesX, farrayValuesY2, modelMarginTop_y, modelMarginLeft_x, Brushes.Blue, new PenLineCap(), new PenLineCap(), 1, canvasForImage);

            // x-axis
            DrawPolyLine(new float[2] { modelMarginLeft_x, modelMarginLeft_x + 1.02f * 360}, new float[2] { modelBottomPosition_y, modelBottomPosition_y }, modelBottomPosition_y, modelMarginLeft_x, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            // y-axis
            DrawPolyLine(new float[2] { modelMarginLeft_x, modelMarginLeft_x }, new float[2] { 0, 1.1f * farrayValuesY1[0] }, modelBottomPosition_y - fReal_Model_Zoom_FactorY * 1.1f * farrayValuesY1[0], modelMarginLeft_x, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);

            // x- axis description
            // Angles of Cardinal Directions
            DrawTexts(farrayValuesX, farrayValuesX, 0.01f, Brushes.Blue);

            // Cardinal Directions
            DrawTexts(arrayTextX1_CardinalDirections, farrayValuesX, -0.04f, Brushes.Red);

            // y-axis description
            float[] arrYDescription_Factors = new float[12] { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f };
            float[] arrYDescription_Speed = new float[17] { 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f, 50f, 55f, 60f, 65f, 70f, 75f, 80f };

            if (bDisplayFactor_M_D)
                DrawTexts(arrYDescription_Factors, 0, arrYDescription_Factors, Brushes.ForestGreen);
            else
                DrawTexts(arrYDescription_Speed, 0, arrYDescription_Speed, Brushes.ForestGreen);

            // Values description
            DrawTexts(farrayValuesY1, farrayValuesX, farrayValuesY1, Brushes.Firebrick);

            // Draw wind zones Design Wind Speed for Theta
            if (!bDisplayFactor_M_D)
                DrawWindSpeedValues(windCalculationData.fV_sit_ULS_Theta_4, windCalculationData.fV_sit_Theta_angles_8);

            Canvas2D = canvasForImage;
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

        public void DrawTexts(string[] array_text, float[] arrPointsCoordX, float fPointsCoordY, SolidColorBrush color)
        {
            float[] arrPointsCoordY = new float[arrPointsCoordX.Length];
            for (int i = 0; i < arrPointsCoordY.Length; i++)
                arrPointsCoordY[i] = fPointsCoordY; // Fill array Items with same value

            DrawTexts(array_text, arrPointsCoordX, arrPointsCoordY, color);
        }

        public void DrawTexts(string[] array_text, float fPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color)
        {
            float[] arrPointsCoordX = new float[arrPointsCoordY.Length];
            for (int i = 0; i < arrPointsCoordX.Length; i++)
                arrPointsCoordX[i] = fPointsCoordX; // Fill array Items with same value

            DrawTexts(array_text, arrPointsCoordX, arrPointsCoordY, color);
        }

        public void DrawTexts(string [] array_text, float[] arrPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color)
        {
            for (int i = 0; i < array_text.Length; i++)
            {
                DrawText(array_text[i], modelMarginLeft_x + fReal_Model_Zoom_FactorX * arrPointsCoordX[i], modelBottomPosition_y - fReal_Model_Zoom_FactorY * arrPointsCoordY[i], 16, color, canvasForImage);
            }
        }

        public void DrawTexts(float[] array_ValuesToDisplay, float[] arrPointsCoordX, float fPointsCoordY, SolidColorBrush color)
        {
            DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), arrPointsCoordX, fPointsCoordY, color);
        }

        public void DrawTexts(float[] array_ValuesToDisplay, float fPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color)
        {
            DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), fPointsCoordX, arrPointsCoordY, color);
        }

        public void DrawTexts(float[] array_ValuesToDisplay, float[] arrPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color)
        {
            DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay),arrPointsCoordX, arrPointsCoordY, color);
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

        public void DrawRectangle(SolidColorBrush strokeColor, SolidColorBrush fillColor, double fillColorOpacity, double thickness, Canvas imageCanvas, Point lt, Point br)
        {
            SolidColorBrush brush = new SolidColorBrush(fillColor.Color) { Opacity = fillColorOpacity };

            Rectangle rect = new Rectangle();
            rect.Stretch = Stretch.Fill;
            rect.Fill = brush;
            rect.Stroke = strokeColor;
            rect.Width = br.X - lt.X;
            rect.Height = br.Y - lt.Y;
            Canvas.SetTop(rect, lt.Y);
            Canvas.SetLeft(rect, lt.X);
            imageCanvas.Children.Add(rect);
        }

        public void DrawWindSpeedValues(float [] fWindSpeedValues, float [] fWindSpeedAngles)
        {
            float fillColorOpacity = 0.5f;
            float thickness = 1;

            Color[] colors = new Color[4] { Colors.DeepSkyBlue, Colors.DarkGreen, Colors.DarkOrange, Colors.DeepPink };
            Point lt;
            Point br;
            float fx_coord_lt;
            float fx_coord_br;

            // 4 basic segments
            for (int i = 0; i < fWindSpeedValues.Length; i++)
            {
                if (fWindSpeedAngles[i * 2] < 0)
                    fx_coord_lt = 0;
                else
                    fx_coord_lt = fWindSpeedAngles[i * 2];

                if (fWindSpeedAngles[i * 2 + 1] > 360)
                    fx_coord_br = 360;
                else
                    fx_coord_br = fWindSpeedAngles[i * 2 + 1];

                lt = new Point(modelMarginLeft_x + fReal_Model_Zoom_FactorX * fx_coord_lt, modelBottomPosition_y - fReal_Model_Zoom_FactorY * fWindSpeedValues[i]);
                br = new Point(modelMarginLeft_x + fReal_Model_Zoom_FactorX * fx_coord_br, modelBottomPosition_y);

                DrawRectangle(new SolidColorBrush(colors[i]), new SolidColorBrush(colors[i]), fillColorOpacity, thickness, canvasForImage, lt, br);
            }

            if(fWindSpeedAngles[0] < 0 || fWindSpeedAngles[7] > 360) // We need to draw 5th segment
            {
                fx_coord_lt = fWindSpeedAngles[7];
                fx_coord_br = 360;

                lt = new Point(modelMarginLeft_x + fReal_Model_Zoom_FactorX * fx_coord_lt, modelBottomPosition_y - fReal_Model_Zoom_FactorY * fWindSpeedValues[0]);
                br = new Point(modelMarginLeft_x + fReal_Model_Zoom_FactorX * fx_coord_br, modelBottomPosition_y);

                DrawRectangle(new SolidColorBrush(colors[0]), new SolidColorBrush(colors[0]), fillColorOpacity, thickness, canvasForImage, lt, br);
            }
        }
    }
}
