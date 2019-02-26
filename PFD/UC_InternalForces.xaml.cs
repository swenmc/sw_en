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
        float[] fArr_AxialForceValuesN;
        float[] fArr_ShearForceValuesVx;
        float[] fArr_ShearForceValuesVy;
        float[] fArr_TorsionMomentValuesT;
        float[] fArr_BendingMomentValuesMx;
        float[] fArr_BendingMomentValuesMy;

        bool DeterminateCombinationResultsByFEMSolver;
        CModel_PFD Model;
        CPFDMemberInternalForces vm;

        public List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations;
        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS;

        // TODO - Ondrej
        // Potrebujeme do UC_InternalForces dostat nejakym sposobom geometriu ramov a vysledky na ramoch aby sme to mohli pre prislusny rozhodujuci MainColumn alebo Rafter zobrazit v FrameInternalForces_2D
        List<CFrame> frameModels;
        //List<List<List<List<basicInternalForces>>>> internalforcesframes;
        //List<List<List<List<basicDeflections>>>> deflectionsframes;

        GraphWindow graph;

        public UC_InternalForces(
            bool bDeterminateCombinationResultsByFEMSolver,
            CModel_PFD model,
            CComponentListVM compList,
            List<CMemberInternalForcesInLoadCombinations> listMemberInternalForcesInLoadCombinations,
            List<CMemberLoadCombinationRatio_ULS> memberDesignResults_ULS,
            List<CMemberLoadCombinationRatio_SLS> memberDesignResults_SLS,
            List<CFrame> frameModels_temp
            )
        {
            InitializeComponent();

            DeterminateCombinationResultsByFEMSolver = bDeterminateCombinationResultsByFEMSolver;
            Model = model; // 3D model
            ListMemberInternalForcesInLoadCombinations = listMemberInternalForcesInLoadCombinations;
            frameModels = frameModels_temp; // particular 2D models

            MemberDesignResults_ULS = memberDesignResults_ULS;
            MemberDesignResults_SLS = memberDesignResults_SLS;

            // Internal forces
            vm = new CPFDMemberInternalForces(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            vm.PropertyChanged += HandleMemberInternalForcesPropertyChangedEvent;
            this.DataContext = vm;

            vm.LimitStateIndex = 0;
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

            // Perform display in of internal foces just for ULS combinations
            // Member basic internal forces
            designBucklingLengthFactors sBucklingLengthFactors;
            designMomentValuesForCb sMomentValuesforCb; // Nepouziva sa
            basicInternalForces[] sBIF_x;

            // Kombinacia ktorej vysledky chceme zobrazit
            CLoadCombination lcomb = Model.GetLoadCombinationWithID(vm.SelectedLoadCombinationID);

            // TODO - Ondrej - Musime hladat nielen prut s maximalnym design ratio ale s maximalnym design ratio v ramci skupiny urcenej v comboboxe Component Type
            // O nieco som sa pokusil ale nie je to velmi pekne. Skus to vylepsit prosim

            // TODO - Ondrej - Ak este combobox nie je inicializovany nemali by sme sa tu asi ani dostat, na riadku 105 je nejaky return, asi by sa to uz tam niekde malo oddychitit
            // Validate that combobox is initialized
            //if (Combobox_ComponentType == null || Combobox_ComponentType.SelectedItem == null) return;


            string selectedGroupName = vm.ComponentList[vm.ComponentTypeIndex].ComponentName;
                // ((CComponentInfo)Combobox_ComponentType.Items[vm.ComponentTypeIndex]).ComponentName; // TODO - identifikovat nazov alebo ENUM skupiny podla vyberu v comboboxe - pozri ci to takto moze byt alebo sa to da urobit prehladnejsie
            CMember member = FindMemberWithMaximumDesignRatio(lcomb, selectedGroupName, Model.listOfModelMemberGroups); // Find member in group (members with same compoment type) with maximum design ratio
            if (member == null) return; // nemame vypocitane vysledky...nie je co zobrazovat //throw new Exception("Member with maximum design ratio not found.");

            fMemberLength_xMax = member.FLength;

            for (int i = 0; i < iNumberOfDesignSections; i++) // TODO Ondrej - toto pole by malo prist do dialogu spolu s hodnotami y, moze sa totiz stat ze v jednom x mieste budu 2 hodnoty y (2 vysledky pre zobrazenie), pole bude teda ine pre kazdu vnutornu silu (N, Vx, Vy, ....)
                arrPointsCoordX[i] = ((float)i / (float)iNumberOfSegments) * fMemberLength_xMax; // Int must be converted to the float to get decimal numbers

            // TODO - nastavi sa sada vnutornych sil ktora sa ma pre dany prut zobrazit (podla vybraneho pruta a load combination)
            // Zmena 22.02.20019 - Potrebujeme pracovat s LoadCombinations, pretoze BFENet moze vracat vysledky v Load Cases aj Load Combinations
            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(member, lcomb, ListMemberInternalForcesInLoadCombinations, iNumberOfDesignSections, out sBucklingLengthFactors, out sMomentValuesforCb, out sBIF_x);

            //TODO - tato transofrmacia je zbytocna ak grafiku 2D prerobime priamo na vykreslovanie vysledkovych struktur
            //TODO - predpoklada sa ze pocet vysledkovych rezov na prute je pre kazdy load case, resp. load combination rovnaky ale nemusi byt, je potrebne dopracovat

            bool bUseResultsForGeometricalCRSCAxis = true; // TODO - toto budem musiet nejako elegantne vyriesit LCS vs PCS pruta, problem sa tiahne uz od zadavaneho zatazenie, vypoctu vn. sil az do posudkov
            int iUnitConversionFactor = 1000; // N to kN, Nm to kNm
            TransformIFStructureOnMemberToFloatArrays(bUseResultsForGeometricalCRSCAxis,
            iUnitConversionFactor,
            sBIF_x,
            out fArr_AxialForceValuesN,
            out fArr_ShearForceValuesVx,
            out fArr_ShearForceValuesVy,
            out fArr_TorsionMomentValuesT,
            out fArr_BendingMomentValuesMx,
            out fArr_BendingMomentValuesMy
            );

            if (arrPointsCoordX == null) return;
            if (fArr_AxialForceValuesN == null) return;
            if (fArr_ShearForceValuesVx == null) return;
            if (fArr_ShearForceValuesVy == null) return;
            if (fArr_TorsionMomentValuesT == null) return;
            if (fArr_BendingMomentValuesMx == null) return;
            if (fArr_BendingMomentValuesMy == null) return;

            // Clear canvases
            Canvas_AxialForceDiagram.Children.Clear();
            Canvas_ShearForceDiagramVx.Children.Clear();
            Canvas_ShearForceDiagramVy.Children.Clear();
            Canvas_TorsionMomentDiagram.Children.Clear();
            Canvas_BendingMomentDiagramMx.Children.Clear();
            Canvas_BendingMomentDiagramMy.Children.Clear();

            // Draw axis (x, y)

            // BUG 211 - pokusy
            //fArr_AxialForceValuesN = new float[11]{2000,1000,1000,3000,1000,1000,3000,2000,500,1000,1000};
            //fArr_AxialForceValuesN = new float[11] { -2000, -1000, -1000, -3000, -1000, -1000, -1000, -2000, -1000, -1000, -1000 };
            //fArr_BendingMomentValuesMx = new float[11] { 2000, 1000, 1000, 3000, 1000, 1000, 1000, 2000, 1000, 1000, 1000 };
            //fArr_BendingMomentValuesMx = new float[11] { -2000, -1000, -1000, -3000, -1000, -1000, -1000, -2000, -1000, -1000, -1000 };
            //fArr_AxialForceValuesN = new float[11] { 3000, 1000, 1000, 500, -1000, -1000, -1000, -2000, -1000, -1000, -4000 };
            //fArr_BendingMomentValuesMx = new float[11] { 3000, 1000, 1000, 500, -1000, -1000, -1000, -2000, -1000, -1000, -4000 };

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
            // Chcelo by to nejako pekne zjednotit s vykreslovanim FrameInternalForces, ale tu je to zlozitejsie lebo som navymyslal rozne pozicie a orientaciu osi x podla toho ake su hodnoty

            //List<Point> listAxialForceValuesN = AddFirstAndLastDiagramPoint(fArr_AxialForceValuesN, member, 40, arrPointsCoordX, -1, 0.01, 1);
            //Drawing2D.DrawYValuesPolygonInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, listAxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);

            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawYValuesCurveInCanvas(false, arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_BendingMomentDiagramMy);

            // Draw values description
            int iNumberOfDecimalPlaces = 2;
            Drawing2D.DrawTexts(true, true, ConvertArrayFloatToString(fArr_AxialForceValuesN, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_AxialForceDiagram);
            Drawing2D.DrawTexts(true, true, ConvertArrayFloatToString(fArr_ShearForceValuesVx, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_ShearForceValuesVx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_ShearForceDiagramVx);
            Drawing2D.DrawTexts(true, true, ConvertArrayFloatToString(fArr_ShearForceValuesVy, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_ShearForceValuesVy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_ShearForceDiagramVy);

            Drawing2D.DrawTexts(false, true, ConvertArrayFloatToString(fArr_TorsionMomentValuesT, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_TorsionMomentValuesT, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_TorsionMomentDiagram);
            Drawing2D.DrawTexts(false, true, ConvertArrayFloatToString(fArr_BendingMomentValuesMx, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_BendingMomentValuesMx, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_BendingMomentDiagramMx);
            Drawing2D.DrawTexts(false, true, ConvertArrayFloatToString(fArr_BendingMomentValuesMy, iNumberOfDecimalPlaces), arrPointsCoordX, fArr_BendingMomentValuesMy, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Brushes.SlateGray, Canvas_BendingMomentDiagramMy);
        }

        private List<Point> AddFirstAndLastDiagramPoint(
            float [] valuesArray,
            CMember member,
            float fReal_Model_Zoom_Factor,
            float [] xLocations_rel,
            float fInternalForceSignFactor,
            double dInternalForceScale,
            double dInternalForceScale_user
            )
        {
            List<Point> listMemberInternalForcePoints = new List<Point>();
            // First point (start at [0,0])
            listMemberInternalForcePoints.Add(new Point(0, 0));

            // Internal force diagram points
            for (int j = 0; j < valuesArray.Length; j++) // For each member create list of points [x, IF value]
            {
                double xlocationCoordinate = fReal_Model_Zoom_Factor * xLocations_rel[j] * member.FLength;

                float IF_Value = fInternalForceSignFactor * valuesArray[j];
                double xlocationValue = dInternalForceScale * dInternalForceScale_user * IF_Value;

                //pozicie x sa ulozia, aby sa nemuseli pocitat znova
                listMemberInternalForcePoints.Add(new Point(xlocationCoordinate, xlocationValue));
            }

            // Last point (end at [L,0])
            listMemberInternalForcePoints.Add(new Point(fReal_Model_Zoom_Factor * member.FLength, 0));

            return listMemberInternalForcePoints;
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
            int iUnitConversionFactor,
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
                fArr_AxialForceValuesN[i] = sBIF_x[i].fN / iUnitConversionFactor;
                fArr_TorsionMomentValuesT[i] = sBIF_x[i].fT / iUnitConversionFactor;

                if (bUseResultsForGeometricalCRSCAxis)
                {
                    fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yy / iUnitConversionFactor;
                    fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zz / iUnitConversionFactor;
                    fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yy / iUnitConversionFactor;
                    fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zz / iUnitConversionFactor;
                }
                else
                {
                    fArr_ShearForceValuesVx[i] = sBIF_x[i].fV_yu / iUnitConversionFactor;
                    fArr_ShearForceValuesVy[i] = sBIF_x[i].fV_zv / iUnitConversionFactor;
                    fArr_BendingMomentValuesMx[i] = sBIF_x[i].fM_yu / iUnitConversionFactor;
                    fArr_BendingMomentValuesMy[i] = sBIF_x[i].fM_zv / iUnitConversionFactor;
                }
            }
        }

        private void Button_Frame_2D_Click(object sender, RoutedEventArgs e)
        {
            CLoadCombination lcomb = vm.LoadCombinations.Find(lc => lc.ID == vm.SelectedLoadCombinationID);
            if (lcomb == null) throw new Exception("Load combination not found.");
            CMember member = FindMemberWithMaximumDesignRatio(lcomb, Combobox_ComponentType.Text, Model.listOfModelMemberGroups);
            if (member == null) throw new Exception("Member with maximum design ratio not found.");

            int iFrameIndex = CModelHelper.GetFrameIndexForMember(member, frameModels);
            CModel frameModel = frameModels[iFrameIndex];

            // TODO - vypocet vzperneho faktora ramu - ak je mensi ako 10, je potrebne navysit ohybove momenty
            // 4.4.2.2.1
            // Second-order effects may be neglected for any frame where the elastic buckling load factor (λc) of the frame, as determined in accordance with 4.9, is greater than 10.

            // TODO napojit hodnoty

            float fN_om_column;
            float fLambda_c = GetFrameBucklingFactorLambda_c(frameModel.m_arrMat[0].m_fE, // Modulus of Elasticity
            (float)frameModel.m_arrCrSc[1].I_y, // Moment of inertia - rafter
            (float)frameModel.m_arrCrSc[0].I_y,  // Moment of inertia - column
            73430f, // Axial force - column 1 (+ compression, - tension)
            -7.43f, // Axial force - column 2 (+ compression, - tension)
            12670f, // Axial force - rafter (+ compression, - tension)
            frameModel.m_arrMembers[0].FLength, // Heigth - column
            frameModel.m_arrMembers[1].FLength, // Length - rafter
            frameModel.m_arrNSupports[0].m_bRestrain[2] == true ? 0 : 1, // 0 - fixed base joint, 1 - pinned base joint // TODO - napojit na podpory
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
                frameModel.m_arrMembers[0].FLength, // Heigth - column
                fN_om_column);

            // TODO Ondrej - ifinput.LoadCombinationIndex - chcem ziskat index kombinacie z comboboxu a poslat ho do FrameInternalForces_2D, aby som vedel ktore vysledky zobrazit, snad to je to OK, este bude treba overit ci naozaj odpovedaju index z comboboxu a index danej kombinacie vo vysledkoch
            //celovo je podla mna posielat indexy somarina, lepsie je poslat cely objekt, alebo ID kombinacie. Co ak v kombe nerobrazim vsetky kombinacie? potom mi bude index na 2 veci

            int lcombIndex = frameModel.GetLoadCombinationIndex(vm.SelectedLoadCombinationID);
            FrameInternalForces_2D window_2D_diagram = new FrameInternalForces_2D(DeterminateCombinationResultsByFEMSolver, frameModel, lcombIndex, ListMemberInternalForcesInLoadCombinations);

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

        private void GraphButton_Click(object sender, RoutedEventArgs e)
        {
            graph = new GraphWindow(arrPointsCoordX, fArr_AxialForceValuesN, fArr_ShearForceValuesVx, fArr_ShearForceValuesVy, fArr_TorsionMomentValuesT, fArr_BendingMomentValuesMx, fArr_BendingMomentValuesMy);
            graph.Show();
        }

        private CMember FindMemberWithMaximumDesignRatio(CLoadCombination lc, string memberGroupName, List<CMemberGroup> listOfGroups)
        {
            float maximumDesignRatio = float.MinValue;
            CMember memberWithMaxDesignRatio = null;

            if (lc.eLComType == ELSType.eLS_SLS)
            {
                foreach (CMemberLoadCombinationRatio_SLS sls in MemberDesignResults_SLS)
                {
                    if (sls.LoadCombination.ID != lc.ID) continue;
                    if (sls.MaximumDesignRatio > maximumDesignRatio && bIsMemberInGroup(sls.Member, memberGroupName, listOfGroups))
                    {
                        maximumDesignRatio = sls.MaximumDesignRatio;
                        memberWithMaxDesignRatio = sls.Member;
                    }
                }
            }
            else
            {
                foreach (CMemberLoadCombinationRatio_ULS uls in MemberDesignResults_ULS)
                {
                    if (uls.LoadCombination.ID != lc.ID) continue;
                    if (uls.MaximumDesignRatio > maximumDesignRatio && bIsMemberInGroup(uls.Member, memberGroupName, listOfGroups))
                    {
                        maximumDesignRatio = uls.MaximumDesignRatio;
                        memberWithMaxDesignRatio = uls.Member;
                    }
                }
            }
            return memberWithMaxDesignRatio;
        }

        bool bIsMemberInGroup(CMember member, string groupName, List<CMemberGroup> listOfGroups)
        {
            // TODO - toto vyhladavanie podla podla skupiny je dost kostrbate, lepsi by bol ENUM EMemberGroupNames
            for (int i = 0; i < listOfGroups.Count; i++)
            {
                for (int j = 0; j < listOfGroups[i].ListOfMembers.Count; j++)
                {
                    if (listOfGroups[i].ListOfMembers[j] == member && listOfGroups[i].Name == groupName)
                        return true;
                }
            }

            return false;
        }
    }
}