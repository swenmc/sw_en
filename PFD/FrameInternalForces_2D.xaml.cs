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

            // LCS of member (x,z) = (x,-y)
            // Draw member

            // Draw IF diagram on member

            //Drawing2D.DrawYValuesCurveInCanvas(true, arrPointsCoordX, fArr_AxialForceValuesN, fCanvasWidth, fCanvasHeight, modelMarginLeft_x, modelMarginRight_x, modelMarginTop_y, modelMarginBottom_y, modelBottomPosition_y, Canvas_AxialForceDiagram);

            // GCS in XZ plane (X,Z) = (x,-y)
            // Rotate and translate member and diagram

            double dMemberLengthScale = 70; // TODO - spocitat podla rozmerov canvas
            double dInternalForceScale = 0.001; // TODO - spocitat podla rozmerov canvas + nastavitelne uzivatelom
            double dInternalForceScale_user = 1;

            double maximumOriginalYCoordinate = 8.0f + 1.0f;  // TODO - spocitat z modelu + nejaky odstup
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
                dMemberLengthScale * maximumOriginalYCoordinate + dMemberLengthScale * factorSwitchYAxis * model.m_arrMembers[i].NodeStart.Z  + dAdditionalOffset_y,
                Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 2, Canvas_InternalForceDiagram);
            }
        }
    }
}
