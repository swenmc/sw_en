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
        public float modelMarginBottom_y;
        public float fReal_Model_Zoom_FactorX;
        public float fReal_Model_Zoom_FactorY;
        public double dWidth;
        public double dHeight;

        float[] farrayValuesX;
        float[] farrayValuesY1;
        float[] farrayValuesY2;

        public WindSpeedChart(CCalcul_1170_2 windCalculationData_temp)
        {
            InitializeComponent();

            windCalculationData = windCalculationData_temp;

            modelMarginLeft_x = 10;
            modelMarginBottom_y = 400;

            farrayValuesX = windCalculationData.fAnglesForV_sit;

            bool bDisplayFactor_M_D = true;

            dWidth = this.Width;
            dHeight = this.Height;

            fReal_Model_Zoom_FactorX = 0.9f * (float)(dWidth / farrayValuesX[farrayValuesX.Length - 1]);

            if (bDisplayFactor_M_D)
            {
                farrayValuesY1 = windCalculationData.fM_D_array360;
                farrayValuesY2 = null;

                fReal_Model_Zoom_FactorY = 0.7f * (float)(dHeight / farrayValuesY1[0]);
            }
            else
            {
                farrayValuesY1 = windCalculationData.fV_sit_ULS_Theta;
                farrayValuesY2 = windCalculationData.fV_sit_SLS_Theta;

                float fReal_Model_Zoom_FactorY_temp1 = 0.7f * (float)(dHeight / farrayValuesY1[0]);
                float fReal_Model_Zoom_FactorY_temp2 = 0.7f * (float)(dHeight / farrayValuesY2[0]);

                fReal_Model_Zoom_FactorY = MathF.Min(fReal_Model_Zoom_FactorY_temp1, fReal_Model_Zoom_FactorY_temp2);
            }

            if (farrayValuesY1 != null)
                DrawPolyLine(farrayValuesX, farrayValuesY1, 10, 10, Brushes.Red, new PenLineCap(), new PenLineCap(), 1, canvasForImage);

            if(farrayValuesY2 != null)
                DrawPolyLine(farrayValuesX, farrayValuesY2, 10, 10, Brushes.Blue, new PenLineCap(), new PenLineCap(), 1, canvasForImage);

            // x-axis
            DrawPolyLine(new float[2] { 0, 1.02f * 360}, new float[2] { modelMarginBottom_y, modelMarginBottom_y }, modelMarginBottom_y, 10, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);
            // y-axis
            DrawPolyLine(new float[2] { 0, 0 }, new float[2] { 0, 1.2f * farrayValuesY1[0] }, modelMarginBottom_y - fReal_Model_Zoom_FactorY * 1.2f * farrayValuesY1[0], 10, Brushes.Black, new PenLineCap(), PenLineCap.Triangle, 1, canvasForImage);

            Canvas2D = canvasForImage;
        }

        public void DrawPolyLine(float[] arrPointsCoordX, float[] arrPointsCoordY, double dCanvasTopTemp, double dCanvasLeftTemp, SolidColorBrush color, PenLineCap startCap, PenLineCap endCap, double thickness, Canvas imageCanvas)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < arrPointsCoordX.Length; i++)
            {
                points.Add(new Point(modelMarginLeft_x + fReal_Model_Zoom_FactorX * arrPointsCoordX[i], modelMarginBottom_y - fReal_Model_Zoom_FactorY * arrPointsCoordY[i]));
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
    }
}
