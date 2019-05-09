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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;
using BaseClasses;
using MATH;
using PFD.ViewModels;
using FEM_CALC_BASE;

namespace PFD
{
    /// <summary>
    /// Interaction logic for FrameInternalForces_2D.xaml
    /// </summary>
    public partial class FrameInternalForces_2D : Window
    {
        private CModel model;
        public List<CMemberInternalForcesInLoadCombinations> ListMemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> ListMemberDeflectionsInLoadCombinations;

        Dictionary<string, List<Point>> DictMemberInternalForcePoints;
        int iLoadCombinationIndex;
        bool UseCRSCGeometricalAxes;

        public FrameInternalForces_2D(bool bUseCRSCGeometricalAxes, CModel example_model, int iLoadCombinationIndex_temp, List<CMemberInternalForcesInLoadCombinations> listMemberInternalForcesInLoadCombinations, List<CMemberDeflectionsInLoadCombinations> listMemberDeflectionsInLoadCombinations)
        {
            UseCRSCGeometricalAxes = bUseCRSCGeometricalAxes;
            model = example_model;
            iLoadCombinationIndex = iLoadCombinationIndex_temp;
            ListMemberInternalForcesInLoadCombinations = listMemberInternalForcesInLoadCombinations;
            ListMemberDeflectionsInLoadCombinations = listMemberDeflectionsInLoadCombinations;

            DictMemberInternalForcePoints = new Dictionary<string, List<Point>>();

            InitializeComponent();

            FrameInternalForces_2DViewModel vm = new FrameInternalForces_2DViewModel();
            vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            this.DataContext = vm;

            DrawDiagram();
        }

        private void DrawDiagram()
        {
            FrameInternalForces_2DViewModel vm = this.DataContext as FrameInternalForces_2DViewModel;
            // LCS of member (x,z) = (x,-y)
            // Draw member

            // Generate IF diagram on member

            // GCS in XZ plane (X,Z) = (x,-y)
            // Rotate and translate member and diagram

            // Draw diagram in GCS

            ////////////////////////////////////////////////////////////////////////////////////////////////
            float fCanvasWidth = (float)Canvas_InternalForceDiagram.Width; // Size of Canvas
            float fCanvasHeight = (float)Canvas_InternalForceDiagram.Height; // Size of Canvas
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

            //float fmodelMarginRight_x = fCanvasWidth - fmodelMarginLeft_x - fModel_Length_x_page;
            float fmodelMarginBottom_y = fCanvasHeight - fmodelMarginTop_y - fModel_Length_y_page;

            // TO Ondrej
            // Tento diagram by chcelo vylepsit a sprehladnit.
            // TODO - doplnit texty, pre texty si odlozit povodne hodnoty IF separatne (grafika diagramu sa moze scalovat ale hodnoty zobrazenych sil v texte ostavaju rovnake)
            // Texty by mali mat rozne moznosti, zobrazit hodnoty na vsetkych rezoch (kazdy, druhy, treti, ... rez), len na koncoch pruta, na koncoch pruta a v mieste extremu, len extremy atd
            // Zobrazovat jednotky alebo bez nich
            // Niekde by mohla byt legenda s popisom co sa vykresluje (cislo ramu, vybrana load combination, vybrany typ zobrazovanej IF)

            int factorSwitchYAxis = -1;
            // Draw each member in the model and selected internal force diagram
            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                // Calculate Member Rotation angle (clockwise)
                double rotAngle_radians = Math.Atan(((dTempMax_Y + factorSwitchYAxis * model.m_arrMembers[i].NodeEnd.Z) - (dTempMax_Y + factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z)) / (model.m_arrMembers[i].NodeEnd.X - model.m_arrMembers[i].NodeStart.X));
                double rotAngle_degrees = Geom2D.RadiansToDegrees(rotAngle_radians);
                
                //get list of points from Dictionary, if not exist then calculate
                List<Point> listMemberInternalForcePoints;
                string key = $"{vm.IFTypeIndex}_{i}_{vm.InternalForceScale_user.ToString("F3")}";
                if (DictMemberInternalForcePoints.ContainsKey(key))
                {
                    listMemberInternalForcePoints = DictMemberInternalForcePoints[key];
                }
                else
                {
                    listMemberInternalForcePoints = GetMemberInternalForcePoints(i, vm.InternalForceScale_user, fReal_Model_Zoom_Factor, key);
                }

                double translationOffset_x = fmodelMarginLeft_x + fReal_Model_Zoom_Factor * model.m_arrMembers[i].NodeStart.X ;
                double translationOffset_y = fmodelBottomPosition_y + fReal_Model_Zoom_Factor * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z; 

                RotateTransform rotateTransform = new RotateTransform(rotAngle_degrees, 0, 0); // + clockwise, - counter-clockwise
                TranslateTransform translateTransform = new TranslateTransform(translationOffset_x, translationOffset_y);
                TransformGroup transformGroup_RandT = new TransformGroup();
                transformGroup_RandT.Children.Add(rotateTransform);
                transformGroup_RandT.Children.Add(translateTransform);

                List<Point> points = new List<Point>();
                foreach (Point p in listMemberInternalForcePoints)
                    points.Add(transformGroup_RandT.Transform(p));

                float fUnitFactor = 0.001f; // N to kN or Nm to kNm

                if (vm.ShowLabels)
                {
                    // Analyse diagram - find minimum and maximum value (find local extremes ???)
                    // store index of extreme values

                    double dMinValue = Double.PositiveInfinity;
                    double dMaxValue = Double.NegativeInfinity;

                    int iIndexMinValue = 0;
                    int iIndexMaxValue = 0;

                    if (ListMemberInternalForcesInLoadCombinations == null)
                        continue; // TODO - Sem by sa to uz nemalo ani dostat ak je prut nema vysledky

                    int iNumberOfDesignSections = 11;
                    designBucklingLengthFactors[] sBucklingLengthFactors;
                    designMomentValuesForCb[] sMomentValuesforCb;
                    basicInternalForces[] sBIF_x;
                    basicDeflections[] sBDef_x;

                    CMemberResultsManager.SetMemberInternalForcesInLoadCombination(
                        UseCRSCGeometricalAxes,
                        model.m_arrMembers[i],
                        model.m_arrLoadCombs[iLoadCombinationIndex],
                        ListMemberInternalForcesInLoadCombinations,
                        iNumberOfDesignSections,
                        out sBucklingLengthFactors,
                        out sMomentValuesforCb,
                        out sBIF_x);

                    CMemberResultsManager.SetMemberDeflectionsInLoadCombination(
                        UseCRSCGeometricalAxes,
                        model.m_arrMembers[i],
                        model.m_arrLoadCombs[iLoadCombinationIndex],
                        ListMemberDeflectionsInLoadCombinations,
                        iNumberOfDesignSections,
                        out sBDef_x);

                    for (int c = 0; c < sBIF_x.Length; c++)
                    {
                        float IF_Value = GetInternalForcesValue(sBIF_x[c], sBDef_x[c]);

                        if(IF_Value < dMinValue)
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

                    // Display text depending on GUI options
                    for (int c = 0; c < sBIF_x.Length; c++)
                    {
                        if (!vm.ShowAll && !vm.ShowEndValues && !vm.ShowExtremeValues && !vm.ShowEverySecondSection && !vm.ShowEveryThirdSection) continue;
                        else if (!vm.ShowAll && vm.ShowEndValues && !(c == 0 || c == (sBIF_x.Length - 1))) continue; // First and last value
                        else if (!vm.ShowAll && vm.ShowExtremeValues && !(c == iIndexMinValue || c == iIndexMaxValue)) continue; // ??? TODO - tu potrebujeme prejst vsetky hodnoty, najst min a max a tie zobrazit, pripadne ak vieme najst aj lokalne minima a maxima, ignorovat nuly - Local extreme - min or max in absolute value
                        else if (!vm.ShowAll && vm.ShowEndValues && vm.ShowExtremeValues && !(c == 0 || c == (sBIF_x.Length - 1) || c == iIndexMinValue || c == iIndexMaxValue)) continue; // TODO / BUG 198 - Pre extremy a konce zobrazit "zjednotenie" tychto hodnot, tj. najdene extremy aj koncove hodnoty - upravit podmienku
                        else if (!vm.ShowAll && vm.ShowEverySecondSection && c % 2 == 1) continue;
                        else if (!vm.ShowAll && vm.ShowEverySecondSection && vm.ShowExtremeValues && !(c % 2 != 1 || c == iIndexMinValue || c == iIndexMaxValue)) continue; // TODO / BUG 198 - Ked je zakrtnuty extrem aj tato volba chcem zobrazit zjednotenie hodnot - upravit podmienku
                        else if (!vm.ShowAll && vm.ShowEveryThirdSection && c % 3 != 0) continue;
                        else if (!vm.ShowAll && vm.ShowEveryThirdSection && vm.ShowExtremeValues && !(c % 3 == 0 || c == iIndexMinValue || c == iIndexMaxValue)) continue; // TODO / BUG 198 - Ked je zakrtnuty extrem aj tato volba chcem zobrazit zjednotenie hodnot - upravit podmienku

                        float IF_Value = GetInternalForcesValue(sBIF_x[c], sBDef_x[c]);

                        // Ignore and do not display zero value label
                        if (MathF.d_equal(IF_Value, 0))
                            continue;

                        string txt = (fUnitFactor * IF_Value).ToString($"F{vm.NumberOfDecimalPlaces}");
                        if (vm.ShowUnits) txt += " " + vm.IFTypeUnit;
                        //string txt = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * IF_Value, 2))) + " " + vm.IFTypeUnit;
                        Drawing2D.DrawText(txt, points[c + 1].X, points[c + 1].Y, 0, vm.FontSize, Brushes.SlateGray, Canvas_InternalForceDiagram);
                    }
                }

                Drawing2D.DrawPolygon(
                    points,
                    Brushes.LightSlateGray,
                    Brushes.SlateGray,
                    PenLineCap.Flat,
                    PenLineCap.Flat,
                    1,
                    0.3,
                    Canvas_InternalForceDiagram);

                if(vm.ShowMembers)
                    // Draw Member on the Internal forces polygon
                    DrawMember(i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees, fmodelMarginLeft_x, fmodelBottomPosition_y, Brushes.Black, 1);
            }
        }

        private float GetInternalForcesValue(basicInternalForces bif, basicDeflections bdef)
        {
            FrameInternalForces_2DViewModel vm = this.DataContext as FrameInternalForces_2DViewModel;
            //"N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz"
            switch (vm.IFTypeIndex)
            {
                case 0: return bif.fN;
                case 1: return bif.fV_zz; //bif.fV_zv???
                case 2: return bif.fV_yy; //bif.fV_yu???
                case 3: return bif.fT;
                case 4: return bif.fM_yy;
                case 5: return bif.fM_zz;
                case 6: return bdef.fDelta_yy;
                case 7: return bdef.fDelta_zz;
                default: throw new Exception($"Not known internal force; IFTypeIndex: {vm.IFTypeIndex}");
            }
        }

        private void DrawMember(int memberIndex, float fReal_Model_Zoom_Factor, int factorSwitchYAxis, double rotAngle_degrees,
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

            Drawing2D.DrawPolyLine(false, points, color, PenLineCap.Flat, PenLineCap.Flat, thickness, Canvas_InternalForceDiagram);

            //Drawing2D.DrawText($"[{memberIndex}]", points[1].X, points[1].Y, 0, 20, Brushes.Red, Canvas_InternalForceDiagram);
        }

        private List<Point> GetMemberInternalForcePoints(int memberIndex, double dInternalForceScale_user, float fReal_Model_Zoom_Factor, string key)
        {
            List<Point> listMemberInternalForcePoints = new List<Point>();

            if (ListMemberInternalForcesInLoadCombinations == null)
            {
                return listMemberInternalForcePoints; // Return empty list ???
            }

            double dInternalForceScale = 0.001; // TODO - spocitat podla rozmerov canvas + nastavitelne uzivatelom

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
                model.m_arrMembers[memberIndex],
                model.m_arrLoadCombs[iLoadCombinationIndex],
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
                model.m_arrMembers[memberIndex],
                model.m_arrLoadCombs[iLoadCombinationIndex],
                ListMemberDeflectionsInLoadCombinations,
                iNumberOfResultsSections,
                out sBDef_x);

            // First point (start at [0,0])
            listMemberInternalForcePoints.Add(new Point(0, 0));

            // Internal force diagram points
            for (int j = 0; j < sBIF_x.Length; j++) // For each member create list of points [x, IF value]
            {
                double xlocationCoordinate = fReal_Model_Zoom_Factor * xLocations_rel[j] * model.m_arrMembers[memberIndex].FLength;

                float IF_Value = fInternalForceSignFactor * GetInternalForcesValue(sBIF_x[j], sBDef_x[j]);
                double xlocationValue = dInternalForceScale * dInternalForceScale_user * IF_Value;

                //pozicie x sa ulozia, aby sa nemuseli pocitat znova
                listMemberInternalForcePoints.Add(new Point(xlocationCoordinate, xlocationValue));
            }

            // Last point (end at [L,0])
            listMemberInternalForcePoints.Add(new Point(fReal_Model_Zoom_Factor * model.m_arrMembers[memberIndex].FLength, 0));

            DictMemberInternalForcePoints.Add(key, listMemberInternalForcePoints);

            return listMemberInternalForcePoints;
        }

        protected void HandleViewModelPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;

            RedrawDiagram();
            //if (e.PropertyName == "IFTypeIndex")
            //{
                
            //}
            //if (e.PropertyName == "InternalForceScale_user")
            //{
            //    RedrawDiagram();
            //}
        }
        private void RedrawDiagram()
        {
            ClearCanvas();
            DrawDiagram();
        }

        private void ClearCanvas()
        {
            Canvas_InternalForceDiagram.Children.Clear();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            FrameInternalForces_2DViewModel vm = this.DataContext as FrameInternalForces_2DViewModel;

            //u mna je e.Delta 120/-120
            vm.InternalForceScale_user = vm.InternalForceScale_user + (e.Delta / 120 * 0.1);
        }
    }
}
