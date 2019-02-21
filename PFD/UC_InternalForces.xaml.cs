using System;
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
using Examples;

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
        CPFDMemberInternalForces ifinput;
        List<CMemberInternalForcesInLoadCases> ListMemberInternalForcesInLoadCases;

        // TODO - Ondrej
        // Potrebujeme do UC_InternalForces dostat nejakym sposobom geometriu ramov a vysledky na ramoch aby sme to mohli pre prislusny rozhodujuci MainColumn alebo Rafter zobrazit v FrameInternalForces_2D
        List<CFrame> frameModels;
        List<List<List<List<basicInternalForces>>>> internalforcesframes;
        List<List<List<List<basicDeflections>>>> deflectionsframes;

        public UC_InternalForces(CModel_PFD model, CComponentListVM compList,
            List<CMemberInternalForcesInLoadCases> listMemberInternalForcesInLoadCases,
            List<CFrame> frameModels_temp,
            List<List<List<List<basicInternalForces>>>> internalforcesframes_temp,
            List<List<List<List<basicDeflections>>>> deflectionsframes_temp
            )
        {
            InitializeComponent();

            Model = model;
            ListMemberInternalForcesInLoadCases = listMemberInternalForcesInLoadCases;

            // TODO Ondrej - prenos modelov a vyslekov frames do UC_InternalForces
            frameModels = frameModels_temp;
            internalforcesframes = internalforcesframes_temp; // TO Ondrej - tu je potrebne zohladnit ci boli pocitane v BFENet load cases alebo load combinations, ak load cases tak tento zoznam vysledkov treba nahradit zoznamom pre load combinations
            deflectionsframes = deflectionsframes_temp;

            // Internal forces
            ifinput = new CPFDMemberInternalForces(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            ifinput.PropertyChanged += HandleMemberInternalForcesPropertyChangedEvent;
            this.DataContext = ifinput;
        }

        protected void HandleMemberInternalForcesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDMemberInternalForces vm = sender as CPFDMemberInternalForces;
            if (vm == null) return;
            if (vm != null && vm.IsSetFromCode) return;

            if (e.PropertyName == "LimitStateIndex") return;
            if (e.PropertyName == "LoadCombinations")
            {
                return;
            }

            // Frame internal forces enabled only for type of members Main Columns and Rafters
            if (vm.ComponentTypeIndex == 0 || vm.ComponentTypeIndex == 1)
                Button_Frame_2D.IsEnabled = true;
            else
                Button_Frame_2D.IsEnabled = false;

            modelBottomPosition_y = fCanvasHeight - modelMarginBottom_y;

            CMember member = Model.listOfModelMemberGroups[vm.ComponentTypeIndex].ListOfMembers.FirstOrDefault();
            if (member == null) throw new Exception("No member in List of Members");
            fMemberLength_xMax = member.FLength;

            for (int i = 0; i < iNumberOfDesignSections; i++) // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)
                arrPointsCoordX[i] = ((float)i / (float)iNumberOfSegments) * fMemberLength_xMax; // Int must be converted to the float to get decimal numbers

            // Perform displayin of internal foces just for ULS combinations
            // Member basic internal forces
            designBucklingLengthFactors sBucklingLengthFactors;
            designMomentValuesForCb sMomentValuesforCb; // Nepouziva sa
            basicInternalForces[] sBIF_x;

            // Kombinacia ktorej vysledky chceme zobrazit
            CLoadCombination lcomb = Model.m_arrLoadCombs[Model.GetLoadCombinationIndex(vm.SelectedLoadCombinationID)];

            //TODO - nastavi sa sada vnutornych sil ktora sa ma pre dany prut zobrazit (podla vybraneho pruta a load combination)
            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(member, lcomb, ListMemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors, out sMomentValuesforCb, out sBIF_x);

            float[] fArr_AxialForceValuesN;
            float[] fArr_ShearForceValuesVx;
            float[] fArr_ShearForceValuesVy;
            float[] fArr_TorsionMomentValuesT;
            float[] fArr_BendingMomentValuesMx;
            float[] fArr_BendingMomentValuesMy;

            //TODO - tato transofrmacia je zbytocna ak grafiku 2D prerobime priamo na vykreslovanie vysledkych struktur
            //TODO - predpoklada sa ze pocet vysledkovych rezov na prute je pre kazdy load case alebo load combination rovnaky ale nemusi byt, je potrebne dopracovat

            bool bUseResultsForGeometricalCRSCAxis = true; // TODO - toto budem musiet nejako elegantne vyriesit LCS vs PCS pruta, problem sa tiahne uz od zadavaneho zatazenie, vypoctu vn. sil az do posudkov

            TransformIFStructureOnMemberToFloatArrays(bUseResultsForGeometricalCRSCAxis,
            sBIF_x,
            out fArr_AxialForceValuesN,
            out fArr_ShearForceValuesVx,
            out fArr_ShearForceValuesVy,
            out fArr_TorsionMomentValuesT,
            out fArr_BendingMomentValuesMx,
            out fArr_BendingMomentValuesMy
            );

            // Clear canvases
            Canvas_AxialForceDiagram.Children.Clear();
            Canvas_ShearForceDiagramVx.Children.Clear();
            Canvas_ShearForceDiagramVy.Children.Clear();
            Canvas_TorsionMomentDiagram.Children.Clear();
            Canvas_BendingMomentDiagramMx.Children.Clear();
            Canvas_BendingMomentDiagramMy.Children.Clear();

            // Draw axis (x, y)

            // BUG 211 - pokusy
            fArr_AxialForceValuesN = new float[11]{2000,1000,1000,3000,1000,1000,1000,2000,1000,1000,1000};
            fArr_AxialForceValuesN = new float[11] { -2000, -1000, -1000, -3000, -1000, -1000, -1000, -2000, -1000, -1000, -1000 };

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
            DrawTexts(true, fArr_AxialForceValuesN, arrPointsCoordX, fArr_AxialForceValuesN, Brushes.BlueViolet, Canvas_AxialForceDiagram);
            DrawTexts(true, fArr_ShearForceValuesVx, arrPointsCoordX, fArr_ShearForceValuesVx, Brushes.BlueViolet, Canvas_ShearForceDiagramVx);
            DrawTexts(true, fArr_ShearForceValuesVy, arrPointsCoordX, fArr_ShearForceValuesVy, Brushes.BlueViolet, Canvas_ShearForceDiagramVy);

            DrawTexts(false, fArr_TorsionMomentValuesT, arrPointsCoordX, fArr_TorsionMomentValuesT, Brushes.BlueViolet, Canvas_TorsionMomentDiagram);
            DrawTexts(false, fArr_BendingMomentValuesMx, arrPointsCoordX, fArr_BendingMomentValuesMx, Brushes.BlueViolet, Canvas_BendingMomentDiagramMx);
            DrawTexts(false, fArr_BendingMomentValuesMy, arrPointsCoordX, fArr_BendingMomentValuesMy, Brushes.BlueViolet, Canvas_BendingMomentDiagramMy);
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

        public void DrawTexts(bool bYOrientationIsUp, float[] array_ValuesToDisplay, float[] arrPointsCoordX, float[] arrPointsCoordY, SolidColorBrush color, Canvas canvasForImage)
        {
            if (!bYOrientationIsUp) // Draw positive values bellow x-axis
            {
                for (int i = 0; i < arrPointsCoordY.Length; i++)
                    arrPointsCoordY[i] *= -1f;
            }

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
        // TODO - !nastavuje sa tu ci brat vysledky v LCS alebo PCS pruta / resp prierezu
        public void TransformIFStructureOnMemberToFloatArrays(
            bool bUseResultsForGeometricalCRSCAxis, // Use cross-section geometrical axis IF or principal axis IF
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
                fArr_TorsionMomentValuesT[i] = sBIF_x[i].fT;

                if (bUseResultsForGeometricalCRSCAxis)
                {
                    fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yy;
                    fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zz;
                    fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yy;
                    fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zz;
                }
                else
                {
                    fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yu;
                    fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zv;
                    fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yu;
                    fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zv;
                }
            }
        }

        private void Button_Frame_2D_Click(object sender, RoutedEventArgs e)
        {
            int iFrameIndex = 0; // TODO Ondrej - urcit index ramu podla toho ktory konkretny member z daneho typu componenty je rozhodujuci
            CModel model = frameModels[iFrameIndex];

            // Nacitanie zoznamov vysledkov pre jednotlive load combinations, members, x-locations
            // TODO Ondrej - je potrebne zakomponovat ci sa v BFENet pocitaju load cases alebo load combinations, ak sa pocitaju len load cases musime load combinations vyrobit sami superpoziciou pre kazdy prut, miesto x a vnutornu silu na tom mieste ... podla zoznamu LC v kombinacii, tj. faktorLC1 * vysledok v mieste "x" z LC1 + faktorLC2 * vysledkok v mieste z "x" LC2 + ....
            List<List<List<basicInternalForces>>> internalforces = internalforcesframes[iFrameIndex]; // Vysledky na konkretnom rame

            // TODO - vypocet vzperneho faktora ramu - ak je mensi ako 10, je potrebne navysit ohybove momenty

            // 4.4.2.2.1
            // Second-order effects may be neglected for any frame where the elastic buckling load factor (λc) of the frame, as determined in accordance with 4.9, is greater than 10.

            // TODO napojit hodnoty

            float fN_om_column;
            float fLambda_c = GetFrameBucklingFactorLambda_c(model.m_arrMat[0].m_fE, // Modulus of Elasticity
            (float)model.m_arrCrSc[1].I_y, // Moment of inertia - rafter
            (float)model.m_arrCrSc[0].I_y,  // Moment of inertia - column
            73430f, // Axial force - column 1 (+ compression, - tension)
            -7.43f, // Axial force - column 2 (+ compression, - tension)
            12670f, // Axial force - rafter (+ compression, - tension)
            model.m_arrMembers[0].FLength, // Heigth - column
            model.m_arrMembers[1].FLength, // Length - rafter
            model.m_arrNSupports[0].m_bRestrain[2] == true ? 0 : 1, // 0 - fixed base joint, 1 - pinned base joint // TODO - napojit na podpory
            5, // Roof Pitch - Gable Roof Model
            out fN_om_column);

            float fLambda_m = 1.0f;

            // TODO napojit hodnoty

            float fLimitToConsiderMomentAmplification = 100f; // AS 4600 limit nema // NZS 3404 uvazuje 10
            if (fLambda_c < fLimitToConsiderMomentAmplification)
                fLambda_m = GetMomentAmplificationFactorDelta_m(
                fLambda_c,
                73430f, // Axial force - column 1 (+ compression, - tension)
                -7.43f, // Axial force - column 2 (+ compression, - tension)
                58240f, // Shear force - column 1 (absolute value)
                24160f, // Shear force - column 2 (absolute value)
                0.08054f, // Horizontal deflection (x-direction) [m] node ID 1 - knee point // Horny okraj stlpa
                model.m_arrMembers[0].FLength, // Heigth - column
                fN_om_column);

            // TODO Ondrej - ifinput.LoadCombinationIndex - chcem ziskat index kombinacie z comboboxu a poslat ho do FrameInternalForces_2D, aby som vedel ktore vysledky zobrazit, snad to je to OK, este bude treba overit ci naozaj odpovedaju index z comboboxu a index danej kombinacie vo vysledkoch
            //celovo je podla mna posielat indexy somarina, lepsie je poslat cely objekt, alebo ID kombinacie. Co ak v kombe nerobrazim vsetky kombinacie? potom mi bude index na 2 veci

            int lcIndex = model.GetLoadCombinationIndex(ifinput.SelectedLoadCombinationID);
            FrameInternalForces_2D window_2D_diagram = new FrameInternalForces_2D(model, lcIndex, internalforces);

            // TODO - faktorom fLambda_m treba prenasobit vnutorne sily ktore vstupuju do design
            window_2D_diagram.ShowDialog();
        }

        // TO Ondrej - toto potrebujem vlozit niekam medzi vypocet vnutornych sil a vnutorne sily ktore vstupuju do design
        // Jedna sa o to ze pri tej metode vypoctu, ktoru pouziva BFENet je potrebne spocitat sucinitele zvacsenia ohybovych momentov

        public float GetFrameBucklingFactorLambda_c(float fE,
            float fIy_majoraxis_pr, // Ι of outer rafter in a portal frame, for use in 4.9.2.4
            float fIy_majoraxis_pc, // Ι of outer column in a portal frame, for use in 4.9.2.4
            //float fN_c1_gravity, // Axial forces generated by gravity loading in the beams 4.9.2.3.1
            //float fN_c2_gravity,
            float fN_c1, // Compression positive, tension negative // design axial compression force from elastic analysis, in outer column of a portal frame, for use in 4.9.2.4
            float fN_c2,
            //float fV_c1, // Shear force in the columns
            //float fV_c2,
            //float fDelta_s_horizontal, // Horizontal deflection of frame knee // translational displacement of the top relative to the bottom for a storey height
            float fN_r, // Axial force in rafter on the side of column under maximum compression // design axial compression force from elastic analysis, in the outer rafter framing into the column carrying Nc*
            float fh_e, // Knee point heigth of columns // height to centre-line of rafter at the knee
            float fs_r, // Length of rafter // centre-line length of rafter, measured from knee to apex and ignoring the presence of any haunches
            int iColumnFixingCode, // 0 - fixed, 1 - pinned base joint
            float fRoofPitch_deg,
            out float fN_om) // TODO - asi by to malo byt pole, pocitat N_om pre kazdy stlp samostatne
        {
            float fs_r_modified = fs_r;

            // Profesor Machacek doporucuje uvazovat 2x dlzku priecle (between knee and apex joint) pre portalove ramy so sklonom mensim nez 15 stupnov
            // TODO - Docasne riesenie - zistit ci to ma zavisiet na sklone alebo nie, pozriet Appendix G
            bool bUseModificationOfRafterLength = false;

            if (bUseModificationOfRafterLength)
            {
                if (fRoofPitch_deg < 10)
                    fs_r_modified = 2f * fs_r;
                else if (fRoofPitch_deg < 15)
                    fs_r_modified = 1.5f * fs_r;
                else
                    fs_r_modified = fs_r;
            }

            // https://waset.org/publications/9830/establishing-a-new-simple-formula-for-buckling-length-factor-k-of-rigid-frames-columns
            int iNumberOfColumnsInFrame = 2;
            int iNumberOfRaftersInFrame = 2;

            // 4.8.3.4.1
            float fB_e_rafter_4834 = 1.0f; // Rigidly connected to a column

            // (Eq. 4.8.3.4)
            float fGA = (iNumberOfColumnsInFrame * (fIy_majoraxis_pc / fh_e)) / (iNumberOfRaftersInFrame * (fB_e_rafter_4834 * fIy_majoraxis_pr / fs_r));

            // TODO - APPENDIX G BRACED MEMBER BUCKLING IN FRAMES
            // Zapracovat vpliv osovej sily v rafter do rovnice pre GA, faktor alpha_sr

            // 4.8.3.4.1
            /*
            (a) For a compression member whose base is not rigidly connected to a footing, the γ - value shall
             not be taken as less than 10 unless justified by a rational analysis; and
            (b) For a compression member whose end is rigidly connected to a footing, the γ-value shall not
             be taken as less than 0.6 unless justified by a rational analysis.
            */

            if (iColumnFixingCode == 1) // Pinned
            {
                if (fGA < 10)
                    fGA = 10;
            }
            else // if (iColumnFixingCode == 0) // Fixed
            {
                if (fGA < 0.6f)
                    fGA = 0.6f;
            }

            float fGB;
            float fK_fr;
            float fK_new;
            float fK;

            if (iColumnFixingCode == 1)
            {
                // Side sway permitted
                fGB = 1f; // Pinned
                // French rule(Pierre Dumonteil, 1992)
                fK_fr = (float)Math.Sqrt((1.6 * fGA * fGB + 4 * (fGA + fGB) + 7.5) / (fGA + fGB + 7.5));

                // New formula(Ehab Hasan Ahmed Hasan Ali, 2012)
                if ((0 <= fGA && fGA <= 100) && (0 <= fGB && fGB <= 100))
                    fK_new = 1.168f + 0.09634f * fGA + 0.09634f * fGB - 0.0022f * MathF.Pow2(fGA) - 0.0022f * MathF.Pow2(fGB) + 0.00212f * fGA * fGB + 0.0000133f * MathF.Pow3(fGA) + 0.0000133f * MathF.Pow3(fGB) - 0.000007253f * fGA * MathF.Pow2(fGB) - 0.000007253f * MathF.Pow2(fGA) * fGB;
                else
                {
                    throw new ArgumentException("GA = " + fGA.ToString() + "\n"
                                              + "GB = " + fGB.ToString());
                }
            }
            else // if (iColumnFixingCode == 0)
            {
                // Braced - side sway prevented
                fGB = 0f; // Fixed

                // French rule(Pierre Dumonteil, 1992)
                fK_fr = (3f * fGA * fGB + 1.4f * (fGA + fGB) + 0.64f) / (3f * fGA * fGB + 2f * (fGA + fGB) + 1.28f);

                // New formula(Ehab Hasan Ahmed Hasan Ali, 2012)
                if ((0 <= fGA && fGA <= 10) && (0 <= fGB && fGB <= 10))
                    fK_new = 0.498f + 0.219f * fGA - 0.08935f * MathF.Pow2(fGA) + 0.0153927f * MathF.Pow3(fGA) - 0.000985f * MathF.Pow4(fGA) + 0.00001422f * MathF.Pow5(fGA) + 0.21769f * fGB - 0.0885f * MathF.Pow2(fGB) + 0.0152f * MathF.Pow3(fGB) - 0.0009713f * MathF.Pow4(fGB) + 0.000014f * MathF.Pow5(fGB);
                else
                {
                    throw new ArgumentException("GA = " + fGA.ToString() + "\n"
                                              + "GB = " + fGB.ToString());
                }
            }

            // Use French Rule or new approximation Ehab Hassan Ahmed Hassan Ali (Egypt)
            bool bUseFrenchRule = false;

            if (bUseFrenchRule)
                fK = fK_fr;
            else
                fK = fK_new;

            // TODO pridat kontrolu stihlosti
            // AS 4600 - 3.4 - Note
            // The slenderness ratio(le / r) of all compression members should not exceed 200

            // Calculation of column euler critical force // TODO - moze sa lisit pre jednotlive stlpy - dopracovat
            fN_om = MathF.fPI * fE * fIy_majoraxis_pc / MathF.Pow2(fK * fh_e); // 4.8.2

            float fLambda_c;

            // NZS 3404:Part 1:1997
            // 4.9.2.3 Rectangular frames with sway members

            // 4.9.2.3.2
            float flambda_ms = (iNumberOfColumnsInFrame * (fN_om / fh_e)) / (fN_c1 / fh_e + fN_c2 / fh_e);

            // 4.9.2.3.3
            // Sway member buckling load(Noms) for each column shall be determined in accordance with 4.8.2, 4.8.3.3 and 4.8.3.4

            float fLambda_c_49233 = flambda_ms;

            // 4.9.2.4 Portal frames of rigid construction
            float fPsi_f = fIy_majoraxis_pc * fs_r_modified / (fIy_majoraxis_pr * fh_e);

            float fLambda_c_4924; // NZS 3404:Part 1:1997
            if (iColumnFixingCode == 1)
            {
                // (a) For pinned - base frames
                fLambda_c_4924 = 3f * fE * fIy_majoraxis_pr / (fs_r_modified * (fN_c1 * fh_e + 0.3f * fN_r * fs_r_modified));
            }
            else
            {
                // (b) For fixed-base frames
                fLambda_c_4924 = (5f * fE * (10f + fPsi_f)) / ((5f * fN_r * MathF.Pow2(fs_r_modified) / fIy_majoraxis_pr) + (2 * fPsi_f * fN_c1 * MathF.Pow2(fh_e) / fIy_majoraxis_pc));
            }

            fLambda_c = Math.Min(fLambda_c_49233, fLambda_c_4924);

            return fLambda_c;
        }

        public float GetMomentAmplificationFactorDelta_m(
            float fLambda_c,
            float fN_c1,
            float fN_c2,
            float fV_c1, // Shear force in the columns
            float fV_c2,
            float fDelta_s_horizontaldeflection, // Horizontal deflection of frame knee // translational displacement of the top relative to the bottom for a storey height,
            float fh_s, // Storey height
            float fN_om)
        {
            float fDelta_b = 1.0f; // Temp - TODO
            float fDelta_s;
            float fMomentAmplificationFactorDelta_m;

            // AS 4600 3.5.1
            float fAlpha_nx = 1 - (Math.Max(fN_c1, fN_c2) / fN_om);
            float fMomentAmplificationFactor_AS4600_351 = 1 / fAlpha_nx;

            // TODO - delta m je minimum z delta b (braced) a delta s (sway)
            // Dopracovat urcenie delta b podla 4.4.3.2

            // 4.4.3.3.2(a)(i)
            float fDelta_s_44332_a_i = Math.Max(0.95f / (1f - ((fDelta_s_horizontaldeflection / fh_s) * ((fN_c1 + fN_c2) / (fV_c1 + fV_c2)))), 1f);
            // 4.4.3.3.2(a)(ii)
            float fLambda_ms = fLambda_c;
            float fDelta_s_44332_a_ii = Math.Max(0.95f / (1f - (1f / fLambda_ms)), 1f);
            // 4.4.3.3.2(a)(iii)
            float fDelta_s_44332_a_iii = Math.Max(0.95f / (1f - (1f / fLambda_c)), 1f);
            // 4.4.3.3.2(b)
            float fDelta_s_44332_b = Math.Max(0.95f / (1f - (1f / fLambda_c)), 1f);

            fDelta_s = MathF.Max(fDelta_s_44332_a_i, fDelta_s_44332_a_ii, fDelta_s_44332_a_iii, fDelta_s_44332_b);

            //4.4.2.1 General
            // A first-order elastic analysis with moment amplification in accordance with 4.4.3
            //  (i) δb ≤ 1.4 when calculated in accordance with 4.4.3.2.
            // (ii) δs ≤ 1.2 when calculated in accordance with 4.4.3.3.
            fDelta_b = Math.Min(fDelta_b, 1.4f);
            fDelta_s = Math.Min(fDelta_s, 1.2f);

            fMomentAmplificationFactorDelta_m = MathF.Max(fDelta_b, fDelta_s, fMomentAmplificationFactor_AS4600_351); // 4.4.3.3.2

            // 4.4.3.3 Moment amplification for a sway member
            return fMomentAmplificationFactorDelta_m;
        }
    }
}