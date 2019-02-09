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

namespace PFD
{
    /// <summary>
    /// Interaction logic for FrameInternalForces_2D.xaml
    /// </summary>
    public partial class FrameInternalForces_2D : Window
    {
        private CModel model;
        List<List<List<basicInternalForces>>> internalforces;

        Dictionary<string, List<Point>> DictMemberInternalForcePoints;
        
        public FrameInternalForces_2D(CExample example_model, List<List<List<basicInternalForces>>> list_internalforces)
        {
            model = example_model;
            internalforces = list_internalforces;

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
            0.7f, // zoom ratio 0-1 (zoom of 2D view), zobrazime model vo velkosti 50% z canvas aby bol dostatok priestoru pre vykreslenie vn sil
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
                // Este by to chcelo pridat aj texty s hodnotami
                for (int c = 0; c < internalforces[0][i].Count; c++)
                {
                    float IF_Value = GetInternalForcesValue(internalforces[0][i][c]);
                    string txt = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * IF_Value, 2))) + " " + vm.IFTypeUnit;
                    Drawing2D.DrawText(txt, points[c + 1].X, points[c + 1].Y, 0, 12, Brushes.Black, Canvas_InternalForceDiagram);                    
                }
                   
                Drawing2D.DrawPolygon(
                    points,   
                    Brushes.Blue,
                    Brushes.Red,
                    PenLineCap.Flat,
                    PenLineCap.Flat,
                    1,
                    0.5,
                    Canvas_InternalForceDiagram);

                //Draw Member on the Internal forces polygon
                DrawMember(i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees,
                    fmodelMarginLeft_x, fmodelBottomPosition_y);
            }
        }


        private float GetInternalForcesValue(basicInternalForces bif)
        {
            FrameInternalForces_2DViewModel vm = this.DataContext as FrameInternalForces_2DViewModel;
            //"N", "Vz", "Vy", "T", "My", "Mz"
            switch (vm.IFTypeIndex)
            {
                case 0: return bif.fN;
                case 1: return bif.fV_zz; //bif.fV_zv???
                case 2: return bif.fV_yy; //bif.fV_yu???
                case 3: return bif.fT;
                case 4: return bif.fM_yy;
                case 5: return bif.fM_zz;
                default: throw new Exception($"Not known internal force; IFTypeIndex: {vm.IFTypeIndex}");
            }
        }

        private void DrawMember(int memberIndex, float fReal_Model_Zoom_Factor, int factorSwitchYAxis, double rotAngle_degrees,
            float fmodelMarginLeft_x, float fmodelBottomPosition_y)
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
                        
            Drawing2D.DrawPolyLine(false, points, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 3, Canvas_InternalForceDiagram);
            
            //Drawing2D.DrawText($"[{memberIndex}]", points[1].X, points[1].Y, 0, 20, Brushes.Red, Canvas_InternalForceDiagram);
        }

        private List<Point> GetMemberInternalForcePoints(int memberIndex, double dInternalForceScale_user, float fReal_Model_Zoom_Factor, string key)
        {
            double dInternalForceScale = 0.001; // TODO - spocitat podla rozmerov canvas + nastavitelne uzivatelom
            
            List<Point> listMemberInternalForcePoints = new List<Point>();

            const int iNumberOfResultsSections = 11;
            double[] xLocations_rel = new double[iNumberOfResultsSections];

            // Fill relative coordinates (x_rel)
            for (int s = 0; s < iNumberOfResultsSections; s++)
                xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

            // First point (start at [0,0])
            listMemberInternalForcePoints.Add(new Point(0, 0));

            // Internal force diagram points
            for (int j = 0; j < internalforces[0][memberIndex].Count; j++) // For each member create list of points [x, IF value]
            {
                double xlocationCoordinate = fReal_Model_Zoom_Factor * xLocations_rel[j] * model.m_arrMembers[memberIndex].FLength;
                
                float IF_Value = GetInternalForcesValue(internalforces[0][memberIndex][j]);
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
            if (e.PropertyName == "IFTypeIndex")
            {
                RedrawDiagram();               
            }
            if (e.PropertyName == "InternalForceScale_user")
            {
                RedrawDiagram();
            }
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
