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

            float fmodelMarginRight_x = fCanvasWidth - fmodelMarginLeft_x - fModel_Length_x_page;
            float fmodelMarginBottom_y = fCanvasHeight - fmodelMarginTop_y - fModel_Length_y_page;

            // To Ondrej - tu som to asi prekombinoval s tym je mam aj canvas aj margin a uz som schaoseny co a ako pouzit
            float fCanvasTop = 10f;
            float fCanvasLeft = fmodelMarginLeft_x;
            
            // TO Ondrej
            // Tento diagram by chcelo vylepsit a sprehladnit.
            // TODO - doplnit texty, pre texty si odlozit povodne hodnoty IF separatne (grafika diagramu sa moze scalovat ale hodnoty zobrazenych sil v texte ostavaju rovnake)
            // Texty by mali mat rozne moznosti, zobrazit hodnoty na vsetkych rezoch (kazdy, druhy, treti, ... rez), len na koncoch pruta, na koncoch pruta a v mieste extremu, len extremy atd
            // Zobrazovat jednotky alebo bez nich
            // Niekde by mohla byt legenda s popisom co sa vykresluje (cislo ramu, vybrana load combination, vybrany typ zobrazovanej IF)
            // Je potrebne doplnit moznost prepinat medzi typmi IF (ja som spravil len M_yy)

            // TO Ondrej, malo by sa nejako rozhodnut v ci mam najprv vsetko pocitat v stutocnych jednotkach a potom to prenasobit alebo cim skor prejst na zobrazovacie jednotky
            // Teraz to mam pri niecom tak, pri niecom inak ... trosku som sa zamotal
            
            // TO MATA: no ja som zamotany este viac, ono to rozdielne funguje, takze ja som este viac schaoseny

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
                
                
                // Draw diagram curve
                double MinValue = Double.MaxValue;
                double MaxValue = Double.MinValue;

                for (int k = 0; k < listMemberInternalForcePoints.Count; k++)
                {
                    double xlocationValue = listMemberInternalForcePoints[k].Y;

                    if (xlocationValue < MinValue)
                        MinValue = xlocationValue;

                    if (xlocationValue > MaxValue)
                        MaxValue = xlocationValue;
                }

                // TO Ondrej - bojujem tu s odsadenim a stredom pootocenia, potrebujem aby konce modrej krivky (vnutorne sily) "dosadli" na cierne usecky (pruty)
                // Nejako som to tam dal ale vobec nerozumiem tej logike preco to tak je :)))) pokus / omyl
                // Ak by sa Ti to podarilo vylepsit, sprehladnit a zjednodusit bol by som rad :)
                // Este by to chcelo pridat aj texty s hodnotami :)

                double dAdditionalOffset_x = MaxValue * Math.Sin(rotAngle_radians);
                double dAdditionalOffset_y = -MaxValue * Math.Cos(rotAngle_radians);

                double translationOffset_x = fReal_Model_Zoom_Factor * model.m_arrMembers[i].NodeStart.X + dAdditionalOffset_x;
                double translationOffset_y = fmodelBottomPosition_y + fReal_Model_Zoom_Factor * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z + dAdditionalOffset_y;

                List<Point> translatedPoints = Drawing2D.DrawPolygon(
                    listMemberInternalForcePoints,
                    fCanvasTop,
                    fCanvasLeft,
                    fmodelMarginLeft_x,
                    fmodelMarginBottom_y,
                    1,
                    rotAngle_degrees,
                    new Point(0, 0),
                    translationOffset_x,
                    translationOffset_y,
                    Brushes.Blue,
                    Brushes.Red,
                    PenLineCap.Flat,
                    PenLineCap.Flat,
                    1,
                    0.5,
                    Canvas_InternalForceDiagram);

                //Draw Member on the Internal forces polygon
                DrawMember(i, fReal_Model_Zoom_Factor, factorSwitchYAxis, rotAngle_degrees,
                    fCanvasTop, fCanvasLeft, fmodelMarginLeft_x, fmodelMarginBottom_y, fmodelBottomPosition_y);


                // TO Ondrej - tak trosku narychlo :))) Treba sa pohrat s odsadeniami a dostat tie texty na okraj krivky
                // POKUS O VYKRESLENIE TEXTU S HODNOTOU INTERNAL FORCE NA ZACIATKU A NA KONCI PRUTA

                // Number of points
                int iNumberOfPoints = listMemberInternalForcePoints.Count;
                Point startPointText = listMemberInternalForcePoints[1]; // Pridali sme [0,0] preto zaciname az indexom 1
                Point endPointText = listMemberInternalForcePoints[iNumberOfPoints - 1 - 1]; // Pridali sme [L,0] preto zaciname uz indexom (n-1-1)

                string[] pointText = new string[2];
                float fUnitFactor = 0.001f; // N to kN or Nm to kNm
                
                float IF_Value1 = GetInternalForcesValue(internalforces[0][i][0]);                
                pointText[0] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * IF_Value1, 2))) + " " + vm.IFTypeUnit;
                float IF_Value2 = GetInternalForcesValue(internalforces[0][i][iNumberOfPoints - 3]);
                pointText[1] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * IF_Value2, 2))) + " " + vm.IFTypeUnit;

                // Transform text points from LCS of member to GCS of frame in 2D graphics
                // Rotate and translate points - same as for polyline

                // TO Ondrej
                // Tu niekde som v mojom snazeni skoncil, mam suradnice pre text otacat pred tym ako vygenerujem text alebo az potom,
                // Malo by to byt analogicky k tomu ako sa pracuje s bodmi polyline, polyline otacam a posuvam celu az potom co sa vytvori
                // navyse funkcia DrawText ktora sa vola v DrawTexts bola nejako modifikovana aby kreslila texty kot a uz to asi velmi nefunguje pre tieto potreby

                float fx_start = Geom2D.GetRotatedPosition_x_CW_rad((float)(startPointText.X), (float)(factorSwitchYAxis * startPointText.Y), rotAngle_radians);
                float fy_start = Geom2D.GetRotatedPosition_y_CW_rad((float)(startPointText.X), (float)(factorSwitchYAxis * startPointText.Y), rotAngle_radians);

                float fx_end = Geom2D.GetRotatedPosition_x_CW_rad((float)(endPointText.X), (float)(factorSwitchYAxis * endPointText.Y), rotAngle_radians);
                float fy_end = Geom2D.GetRotatedPosition_y_CW_rad((float)(endPointText.X), (float)(factorSwitchYAxis * endPointText.Y), rotAngle_radians);

                // Translate text points
                fx_start += (float)translationOffset_x;
                fy_start += (float)translationOffset_y;

                fx_end += (float)translationOffset_x;
                fy_end += (float)translationOffset_y;

                // Create new points
                Point startPointText_newRotated = new Point(fx_start, fy_start);
                Point endPointText_newRotated = new Point(fx_end, fy_end);

                //// Create array (To Ondrej - toto je asi zbytocne, preklapam to z Point na polia float a podobne, neviem co je vhodnejsie ak chcem vykreslovat nejaku sadu kriviek XY alebo texty v bodoch, ale malo by to byt len jedno)
                //float[] textpointCoordinates_x = new float[2];
                //textpointCoordinates_x[0] = (float)(startPointText_newRotated.X);
                //textpointCoordinates_x[1] = (float)(endPointText_newRotated.X);

                //float[] textpointCoordinates_y = new float[2];
                //textpointCoordinates_y[0] = (float)(startPointText_newRotated.Y);
                //textpointCoordinates_y[1] = (float)(endPointText_newRotated.Y);
                pointText[0] += $" [{startPointText_newRotated.X}; {startPointText_newRotated.Y}]";
                //Drawing2D.DrawText(pointText[0], startPointText.X + translationOffset_x, startPointText.Y + translationOffset_y, 0, 12, Brushes.DarkSeaGreen, Canvas_InternalForceDiagram);
                // Drawing2D.DrawText(pointText[1], endPointText_newRotated.X, endPointText_newRotated.Y, 0, 12, Brushes.DarkSeaGreen, Canvas_InternalForceDiagram);


                foreach (Point p in translatedPoints)
                {
                    Drawing2D.DrawText($"[{p.X.ToString("F1")};{p.Y.ToString("F1")}]", p.X, p.Y, 0, 12, Brushes.DarkSeaGreen, Canvas_InternalForceDiagram);
                }
                    

                //Drawing2D.DrawTexts(false, pointText, textpointCoordinates_x, textpointCoordinates_y,
                //            fCanvasWidth, fCanvasHeight,
                //            fmodelMarginLeft_x, fmodelMarginRight_x, fmodelMarginTop_y, fmodelMarginBottom_y, fmodelBottomPosition_y, false, Brushes.DarkSeaGreen, Canvas_InternalForceDiagram);

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
            float fCanvasTop, float fCanvasLeft, float fmodelMarginLeft_x, float fmodelMarginBottom_y, float fmodelBottomPosition_y)
        {
            // Draw member
            List<Point> listMemberPoints = new List<Point>(2);
            listMemberPoints.Add(new Point(0, 0));
            listMemberPoints.Add(new Point(fReal_Model_Zoom_Factor * model.m_arrMembers[memberIndex].FLength, 0));
            
            //Draw member
            Drawing2D.DrawPolyLine(false, listMemberPoints, fCanvasTop, fCanvasLeft, fmodelMarginLeft_x, fmodelMarginBottom_y, 1, rotAngle_degrees, new Point(0, 0),
                fReal_Model_Zoom_Factor * model.m_arrMembers[memberIndex].NodeStart.X, fmodelBottomPosition_y + fReal_Model_Zoom_Factor * factorSwitchYAxis * model.m_arrMembers[memberIndex].NodeStart.Z,
                Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 3, Canvas_InternalForceDiagram);
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
