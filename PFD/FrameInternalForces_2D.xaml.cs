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

namespace PFD
{
    /// <summary>
    /// Interaction logic for FrameInternalForces_2D.xaml
    /// </summary>
    public partial class FrameInternalForces_2D : Window
    {
        public FrameInternalForces_2D(CExample model, List<List<List<basicInternalForces>>> internalforces)
        {
            InitializeComponent();

            //CFrameInternalForces_2DViewModel vm = new CFrameInternalForces_2DViewModel();
            //vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            //this.DataContext = vm;

            // LCS of member (x,z) = (x,-y)
            // Draw member

            // Generate IF diagram on member

            // GCS in XZ plane (X,Z) = (x,-y)
            // Rotate and translate member and diagram

            // Draw diagram in GCS

            ////////////////////////////////////////////////////////////////////////////////////////////////
            double dMemberLengthScale = 70; // TODO - spocitat podla rozmerov canvas
            double dInternalForceScale = 0.001; // TODO - spocitat podla rozmerov canvas + nastavitelne uzivatelom
            double dInternalForceScale_user = 1; // Uzivatelske scalovanie zadane numericky alebo to moze to byt napriklad aj klavesova skratka napr. d + wheel button (zvacsi / zmensi sa diagram v smere kolmom na pruty)






            // TO Ondrej
            // Tento diagram by chcelo vylepsit a sprehladnit.
            // TODO - doplnit texty, pre texty si odlozit povodne hodnoty IF separatne (grafika diagramu sa moze scalovat ale hodnoty zobrazenych sil v texte ostavaju rovnake)
            // Texty by mali mat rozne moznosti, zobrazit hodnoty na vsetkych rezoch (kazdy, druhy, treti, ... rez), len na koncoch pruta, na koncoch pruta a v mieste extremu, len extremy atd
            // Zobrazovat jednotky alebo bez nich
            // Niekde by mohla byt legenda s popisom co sa vykresluje (cislo ramu, vybrana load combination, vybrany typ zobrazovanej IF)
            // Je potrebne doplnit moznost prepinat medzi typmi IF (ja som spravil len M_yy)





            double maximumOriginalYCoordinate = 8.0f + 1.5f;  // TODO - spocitat z geometrie modelu + nejaky odstup ??? (mozno to nie je nutne, pouzije sa top margin)
            double factorSwitchYAxis = -1;
            // Draw each member in the model and selected internal force diagram
            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                // Draw member
                List<Point> listMemberPoints = new List<Point>(2);
                listMemberPoints.Add(new Point(0, 0));
                listMemberPoints.Add(new Point(dMemberLengthScale * model.m_arrMembers[i].FLength, 0));

                // Calculate Member Rotation angle (clockwise)
                double rotAngle_radians = Math.Atan(((maximumOriginalYCoordinate + factorSwitchYAxis * model.m_arrMembers[i].NodeEnd.Z) - (maximumOriginalYCoordinate + factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z)) / (model.m_arrMembers[i].NodeEnd.X - model.m_arrMembers[i].NodeStart.X));
                double rotAngle_degrees = Geom2D.RadiansToDegrees(rotAngle_radians);

                Drawing2D.DrawPolyLine(false, listMemberPoints, 10, 10, 5, 5, 1, rotAngle_degrees, new Point(0, 0),
                    dMemberLengthScale * model.m_arrMembers[i].NodeStart.X, dMemberLengthScale * maximumOriginalYCoordinate + dMemberLengthScale * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z,
                    Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, Canvas_InternalForceDiagram);

                // Draw diagram curve

                List<Point> listMemberInternalForcePoints = new List<Point>();

                const int iNumberOfResultsSections = 11;
                double[] xLocations_rel = new double[iNumberOfResultsSections];

                // Fill relative coordinates (x_rel)
                for (int s = 0; s < iNumberOfResultsSections; s++)
                    xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

                // First point (start at [0,0])
                listMemberInternalForcePoints.Add(new Point(0, 0));

                // Internal force diagram points
                for (int j = 0; j < internalforces[0][i].Count; j++) // For each member create list of points [x, IF value]
                {
                    double xlocationCoordinate = dMemberLengthScale * xLocations_rel[j] * model.m_arrMembers[i].FLength;
                    double xlocationValue = dInternalForceScale * dInternalForceScale_user * internalforces[0][i][j].fM_yy; // TODO - vytvorit enum pre internal force a nacitat vybrany typ

                    // TODO - pozicie x by sa mohli ulozit spolu s vysledkami, aby sa nemuseli pocitat znova
                    listMemberInternalForcePoints.Add(new Point(xlocationCoordinate, xlocationValue));
                }

                // Last point (end at [L,0])
                listMemberInternalForcePoints.Add(new Point(dMemberLengthScale * model.m_arrMembers[i].FLength, 0));

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

                Drawing2D.DrawPolyLine(false, listMemberInternalForcePoints, 10, 10, 5, 5, 1, rotAngle_degrees, new Point(0, 0),
                dMemberLengthScale * model.m_arrMembers[i].NodeStart.X + dAdditionalOffset_x,
                dMemberLengthScale * maximumOriginalYCoordinate + dMemberLengthScale * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z + dAdditionalOffset_y,
                Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 1, Canvas_InternalForceDiagram);


                // TO Ondrej - tak trosku narychlo :)))
                // POKUS O VYKRESLENIE TEXTU S HODNOTOU INTERNAL FORCE NA ZACIATKU A NA KONCI PRUTA

                // Number of points
                int iNumberOfPoints = listMemberInternalForcePoints.Count;
                Point startPointText = listMemberInternalForcePoints[1]; // Pridali sme [0,0] preto zaciname az indexom 1
                Point endPointText = listMemberInternalForcePoints[iNumberOfPoints - 1 - 1]; // Pridali sme [L,0] preto zaciname uz indexom (n-1-1)

                string[] pointText = new string[2];
                float fUnitFactor = 0.001f; // N to kN or Nm to kNm
                string unitForce = "kN"; // N, Vy, Vz (resp. Fx, Fy, Fz)
                string unitMoment = "kNm"; // T, My, Mz (resp. Mx My, Mz)

                // TODO - napojit vybrany typ IF - teraz je pouzity natvrdo M_yy

                pointText[0] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * internalforces[0][i][0].fM_yy,2))) + " " + unitMoment;
                pointText[1] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Math.Round(fUnitFactor * internalforces[0][i][iNumberOfPoints - 3].fM_yy, 2))) + " " + unitMoment;

                // Transform text points from LCS of member to GCS of frame in 2D graphics
                // Rotate and translate points - same as for polyline

                float fx_start = Geom2D.GetRotatedPosition_x_CW_rad((float)startPointText.X, (float)startPointText.Y, rotAngle_radians);
                float fy_start = Geom2D.GetRotatedPosition_y_CW_rad((float)startPointText.X, (float)startPointText.Y, rotAngle_radians);

                float fx_end = Geom2D.GetRotatedPosition_x_CW_rad((float)endPointText.X, (float)endPointText.Y, rotAngle_radians);
                float fy_end = Geom2D.GetRotatedPosition_y_CW_rad((float)endPointText.X, (float)endPointText.Y, rotAngle_radians);

                Point startPointText_newRotated = new Point(fx_start, fy_start);
                Point endPointText_newRotated = new Point(fx_end, fy_end);

                // Translate text points

                float[] textpointCoordinates_x = new float[2];
                textpointCoordinates_x[0] = (float)(startPointText_newRotated.X);
                textpointCoordinates_x[1] = (float)(endPointText_newRotated.X);

                float[] textpointCoordinates_y = new float[2];
                textpointCoordinates_y[0] = (float)(startPointText_newRotated.Y);
                textpointCoordinates_y[1] = (float)(endPointText_newRotated.Y);

                Drawing2D.DrawTexts(pointText, textpointCoordinates_x, textpointCoordinates_y, 
                    (float)Canvas_InternalForceDiagram.Width, (float)Canvas_InternalForceDiagram.Height,
                    10, 10, 5, 5, 400, Brushes.DarkSeaGreen, Canvas_InternalForceDiagram);
            }
        }


        protected void HandleViewModelPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }
    }
}
