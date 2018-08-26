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

        public List<string> ComponentsNames;
        

        public UC_InternalForces(CModel model, UC_ComponentList components)
        {
            InitializeComponent();

            CComponentListVM compList = (CComponentListVM)components.DataContext;
            ComponentsNames = compList.ComponentList.Select(i => i.ComponentName).ToList();
            
            // Add items into comboboxes
            FillComboboxValues(Combobox_LimitState, model.m_arrLimitStates);
            FillComboboxValues(Combobox_LoadCombination, model.m_arrLoadCombs);
            //FillComboboxValues(Combobox_ComponentType, components.MemberComponentName); binding napojit

            Combobox_ComponentType.ItemsSource = ComponentsNames;

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
            Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
            Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawAxisInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawAxisInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);

            // TODO
            // Vysledky by mali byt v N a Nm (pocitame v zakladnych jednotkach SI), pre zobrazenie prekonvertovat na kN a kNm, pripadne pridat nastavenie jednotiek do GUI

            // Draw y values
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);

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
        

        // Funkcie su rovnake ako u windspeed, len je pridany parameter pre canvas a vypocet FactorX a FactorY
        //public void DrawTexts(string[] array_text, float[] arrPointsCoordX, float fPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        //{
        //    float[] arrPointsCoordY = new float[arrPointsCoordX.Length];
        //    for (int i = 0; i < arrPointsCoordY.Length; i++)
        //        arrPointsCoordY[i] = fPointsCoordY; // Fill array Items with same value

        //    Drawing2D.DrawTexts(array_text, arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y,
        //        modelBottomPosition_y, color, canvasForImage);
        //}

        //public void DrawTexts(string[] array_text, float fPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        //{
        //    float[] arrPointsCoordX = new float[arrPointsCoordY.Length];
        //    for (int i = 0; i < arrPointsCoordX.Length; i++)
        //        arrPointsCoordX[i] = fPointsCoordX; // Fill array Items with same value

        //    Drawing2D.DrawTexts(array_text, arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y,
        //        modelBottomPosition_y, color, canvasForImage);
        //}
        
        //public void DrawTexts(float[] array_ValuesToDisplay, float[] arrPointsCoordX, float fPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        //{
        //    DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), arrPointsCoordX, fPointsCoordY, color, canvasForImage);
        //}

        //public void DrawTexts(float[] array_ValuesToDisplay, float fPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        //{
        //    DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), fPointsCoordX, arrPointsCoordY, color, canvasForImage);
        //}

        public void DrawTexts(float[] array_ValuesToDisplay, float[] arrPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        { 
            Drawing2D.DrawTexts(ConvertArrayFloatToString(array_ValuesToDisplay), arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y,
                modelBottomPosition_y, color, canvasForImage);
        }

        public string[] ConvertArrayFloatToString(float[] array_float, int iDecimalPlaces = 3)
        {
            //NumberFormatInfo ???
            if (array_float != null)
            {
                string[] array_string = new string[array_float.Length];
                for (int i = 0; i < array_string.Length; i++)
                    array_string[i] = (Math.Round(array_float[i], iDecimalPlaces)).ToString();

                return array_string;

            }
            return null;
        }

        

    }
}
