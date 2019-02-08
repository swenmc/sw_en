﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
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
using FEM_CALC_BASE;
using BriefFiniteElementNet.CodeProjectExamples;

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

        CModel_PFD Model;
        List<CMemberInternalForcesInLoadCases> ListMemberInternalForcesInLoadCases;

        public UC_InternalForces(CModel_PFD model, CComponentListVM compList, List<CMemberInternalForcesInLoadCases> listMemberInternalForcesInLoadCases)
        {
            InitializeComponent();

            Model = model;
            ListMemberInternalForcesInLoadCases = listMemberInternalForcesInLoadCases;

            // Internal forces
            CPFDMemberInternalForces ifinput = new CPFDMemberInternalForces(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            ifinput.PropertyChanged += HandleMemberInternalForcesPropertyChangedEvent;
            this.DataContext = ifinput;
        }


        protected void HandleMemberInternalForcesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDMemberInternalForces memberIF = sender as CPFDMemberInternalForces;
            if (memberIF == null) return;
            if (memberIF != null && memberIF.IsSetFromCode) return;
            
            modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;
            
            CMember member = Model.listOfModelMemberGroups[memberIF.ComponentTypeIndex].ListOfMembers.FirstOrDefault();
            if (member == null) throw new Exception("No member in List of Members");
            fMemberLength_xMax = member.FLength;

            for (int i = 0; i < iNumberOfDesignSections; i++) // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)
                arrPointsCoordX[i] = ((float)i / (float)iNumberOfSegments) * fMemberLength_xMax; // Int must be converted to the float to get decimal numbers

            // Perform displayin of internal foces just for ULS combinations
            // Member basic internal forces
            designBucklingLengthFactors sBucklingLengthFactors;
            designMomentValuesForCb sMomentValuesforCb; // Nepouziva sa
            basicInternalForces[] sBIF_x;

            // TODO - kombinacia ktorej vysledky chceme zobrazit
            CLoadCombination lcomb = Model.m_arrLoadCombs[memberIF.LoadCombinationIndex];
            // TODO - nastavi sa sada vnutornych sil ktora sa ma pre dany prut zobrazit (podla vybraneho pruta a load combination)
            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(member, lcomb, ListMemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors, out sMomentValuesforCb, out sBIF_x);

            float[] fArr_AxialForceValuesN;
            float[] fArr_ShearForceValuesVx;
            float[] fArr_ShearForceValuesVy;
            float[] fArr_TorsionMomentValuesT;
            float[] fArr_BendingMomentValuesMx;
            float[] fArr_BendingMomentValuesMy;

            //TODO - tato transofrmacia je zbytocna ak grafiku 2D prerobime priamo na vykreslovanie vysledkych struktur
            //TODO - predpoklada sa ze pocet vysledkovych rezov na prute je pre kazdy load case alebo load combination rovnaky ale nemusi byt, je potrebne dopracovat

            TransformIFStructureOnMemberToFloatArrays(
            sBIF_x,
            out fArr_AxialForceValuesN,
            out fArr_ShearForceValuesVx,
            out fArr_ShearForceValuesVy,
            out fArr_TorsionMomentValuesT,
            out fArr_BendingMomentValuesMx,
            out fArr_BendingMomentValuesMy
            );

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
            DrawTexts(fArr_AxialForceValuesN, arrPointsCoordX, fArr_AxialForceValuesN, Brushes.BlueViolet, Canvas_AxialForceDiagram);
            DrawTexts(fArr_ShearForceValuesVx, arrPointsCoordX, fArr_ShearForceValuesVx, Brushes.BlueViolet, Canvas_ShearForceDiagramVx);
            DrawTexts(fArr_ShearForceValuesVy, arrPointsCoordX, fArr_ShearForceValuesVy, Brushes.BlueViolet, Canvas_ShearForceDiagramVy);

            DrawTexts(fArr_TorsionMomentValuesT, arrPointsCoordX, fArr_TorsionMomentValuesT, Brushes.BlueViolet, Canvas_TorsionMomentDiagram);
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
            Drawing2D.DrawTexts(true, ConvertArrayFloatToString(array_ValuesToDisplay), arrPointsCoordX, arrPointsCoordY, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y,
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

        // TODO - tato transformacia je zbytocna ak prepracujeme zobrazovanie priamo na vykreslovanie poloziek struktury
        public void TransformIFStructureOnMemberToFloatArrays(
            basicInternalForces[] sBIF_x,
            out float[] fArr_AxialForceValuesN,
            out float[] fArr_ShearForceValuesVx,
            out float[] fArr_ShearForceValuesVy,
            out float[] fArr_TorsionMomentValuesT,
            out float[] fArr_BendingMomentValuesMx,
            out float[] fArr_BendingMomentValuesMy
            )
        {
            fArr_AxialForceValuesN = new float[iNumberOfDesignSections];
            fArr_ShearForceValuesVx = new float[iNumberOfDesignSections];
            fArr_ShearForceValuesVy = new float[iNumberOfDesignSections];
            fArr_TorsionMomentValuesT = new float[iNumberOfDesignSections];
            fArr_BendingMomentValuesMx = new float[iNumberOfDesignSections];
            fArr_BendingMomentValuesMy = new float[iNumberOfDesignSections];

            for (int i = 0; i < iNumberOfDesignSections; i++)
            {
                // TODO indexy pre cross-section principal axes vs indexy pre local axes
                fArr_AxialForceValuesN[i] = sBIF_x[i].fN;
                fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yu;
                fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zv;
                fArr_TorsionMomentValuesT[i] = sBIF_x[i].fT;
                fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yu;
                fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zv;
            }
        }

        private void Button_Frame_2D_Click(object sender, RoutedEventArgs e)
        {
            // Spocita testovaci model ramu
            RunExample3 BFENet_testovaciModelRamu = new RunExample3();

            CExample model = BFENet_testovaciModelRamu.model_SW_EN;
            // Nacitanie zoznamov vysledkov pre jednotlive load combinations, members, x-locations
            List<List<List<basicInternalForces>>> internalforces = BFENet_testovaciModelRamu.results;

            FrameInternalForces_2D window_2D_diagram = new FrameInternalForces_2D(model, internalforces);
            window_2D_diagram.ShowDialog();
        }
    }
}