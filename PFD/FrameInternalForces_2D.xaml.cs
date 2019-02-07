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


            // Draw each member in the model and selected internal force diagram
            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                // Draw member
                List<Point> listMemberPoints = new List<Point>(2);
                listMemberPoints.Add(new Point(0, 0));
                listMemberPoints.Add(new Point(dMemberLengthScale * model.m_arrMembers[i].FLength, 0));

                Drawing2D.DrawPolyLine(false, listMemberPoints, 10, 10, 5, 5, 1, Brushes.Black, PenLineCap.Flat, PenLineCap.Flat, 2, Canvas_InternalForceDiagram);

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
                    // TODO - pozicie x by sa mohli ulozit spolu s vysledkami, aby sa nemuseli pocitat znova

                    listMemberInternalForcePoints.Add(new Point(dMemberLengthScale * xLocations_rel[j] * model.m_arrMembers[i].FLength, dInternalForceScale * dInternalForceScale_user * internalforces[0][i][j].fM_yy)); // TODO - vytvorit enum pre internal force a nacitat vybrany typ
                }

                // Last point (end at [L,0])
                listMemberInternalForcePoints.Add(new Point(dMemberLengthScale * model.m_arrMembers[i].FLength, 0));

                Drawing2D.DrawPolyLine(false, listMemberInternalForcePoints, 10, 10, 5, 5, 1, Brushes.Blue, PenLineCap.Flat, PenLineCap.Flat, 2, Canvas_InternalForceDiagram);
            }
        }
    }
}
