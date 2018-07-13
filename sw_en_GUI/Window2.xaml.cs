using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Configuration;
using _3DTools;
using BaseClasses;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private bool bDebugging;
        public Model3DGroup gr = new Model3DGroup();
        public EGCS eGCS = EGCS.eGCSLeftHanded;
        //EGCS eGCS = EGCS.eGCSRightHanded;

        ///////////////////////////////////////////////////////////////
        // Create switch command for various sections, split code into separate objects / function of 3D drawing for each type
        ///////////////////////////////////////////////////////////////

        // Tutorial
        /// http://kindohm.com/technical/WPF3DTutorial.htm  ScreenSpaceLines3D

        /// <summary>
        /// ///////////////////////////////////////////////////////
        /// MAIN CONSTRUCTOR
        /// ///////////////////////////////////////////////////////
        /// </summary>
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        public Window2()
        {
        }

        public Window2(bool bDebugging_temp)
        {
            InitializeComponent();

            bDebugging = bDebugging_temp;

            //gr.Children.Add(new AmbientLight());

            GeometryModel3D SolidModel3D = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();

            //ScreenSpaceLines3D line = new ScreenSpaceLines3D();
            //line.Color = Color.FromRgb(0,255,0);
            //line.Points.Add(mesh.Positions[0]);
            //line.Points.Add(mesh.Positions[1]);

            //Viewport3D view = new Viewport3D();
            //view.Children.Add(line);

            //gr.Children.Add(new AmbientLight());
            SolidModel3D.Geometry = mesh;
            SolidColorBrush br = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            SolidModel3D.Material = new DiffuseMaterial(br);

            gr.Children.Add(SolidModel3D); // Add solid to model group

            _trackport.Model = (Model3D)gr; //CreateRectangle(p3, p2, p6, p7, Brushes.Red);

            _trackport.Trackball.TranslateScale = 1000;   //step for moving object (panning)

            _trackport.SetupScene();
        }

        public Window2(CModel cmodel, bool bDebugging_temp)
        {
            InitializeComponent();

            bDebugging = bDebugging_temp;

            if (cmodel != null)
            {
                // Default color
                SolidColorBrush brushDefault = new SolidColorBrush(Colors.Red);

                bool bShowGlobalAxis = true;
                if(bShowGlobalAxis) Drawing3D.DrawGlobalAxis(_trackport.ViewPort);
                
                bool bDisplayMembersSurface = true;
                Model3D membersModel3D = null;
                if (bDisplayMembersSurface) membersModel3D = Drawing3D.CreateMembersModel3D(cmodel);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);
                
                bool displayConnectionJoints = true;
                Model3DGroup jointsModel3DGroup = null;
                if(displayConnectionJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(cmodel);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);

                bool displayOtherObjects3D = true;
                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(cmodel);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);
                
                Drawing3D.AddLightsToModel3D(gr);

                //Mato? toto tu potrebujeme?
                //Popravde absolutne netusim naco to tu je a co to robi

                // To Ondrej, to je nejaky stary pokusny kod ktory kresli wireframe kocky
                // Doporucujem presunut niekam do nejakych testovacich prikladov, mozno sa to zide niekedy

                if (cmodel.m_arrGOLines != null) // Some lines exist
                {
                    Point3D solidCenter = new Point3D(-5, 0, 0);

                    float fa = 0.5f;

                    Point3D p0 = new Point3D(-fa, -fa, -fa);
                    Point3D p1 = new Point3D(fa, -fa, -fa);
                    Point3D p2 = new Point3D(fa, fa, -fa);
                    Point3D p3 = new Point3D(-fa, fa, -fa);
                    Point3D p4 = new Point3D(-fa, -fa, fa);
                    Point3D p5 = new Point3D(fa, -fa, fa);
                    Point3D p6 = new Point3D(fa, fa, fa);
                    Point3D p7 = new Point3D(-fa, fa, fa);

                    // Lines

                    ScreenSpaceLines3D line1 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line2 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line3 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line4 = new ScreenSpaceLines3D();

                    Color lineColor = Color.FromRgb(250, 30, 30);
                    line1.Color = lineColor;
                    line1.Points.Add(p0);
                    line1.Points.Add(p1);

                    line2.Color = lineColor;
                    line2.Points.Add(p1);
                    line2.Points.Add(p2);

                    line3.Color = lineColor;
                    line3.Points.Add(p2);
                    line3.Points.Add(p3);

                    line4.Color = lineColor;
                    line4.Points.Add(p3);
                    line4.Points.Add(p0);

                    _trackport.ViewPort.Children.Add(line1);
                    _trackport.ViewPort.Children.Add(line2);
                    _trackport.ViewPort.Children.Add(line3);
                    _trackport.ViewPort.Children.Add(line4);

                    ScreenSpaceLines3D line5 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line6 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line7 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line8 = new ScreenSpaceLines3D();

                    line5.Color = lineColor;
                    line5.Points.Add(p4);
                    line5.Points.Add(p5);

                    line6.Color = lineColor;
                    line6.Points.Add(p5);
                    line6.Points.Add(p6);

                    line7.Color = lineColor;
                    line7.Points.Add(p6);
                    line7.Points.Add(p7);

                    line8.Color = lineColor;
                    line8.Points.Add(p7);
                    line8.Points.Add(p4);

                    _trackport.ViewPort.Children.Add(line5);
                    _trackport.ViewPort.Children.Add(line6);
                    _trackport.ViewPort.Children.Add(line7);
                    _trackport.ViewPort.Children.Add(line8);

                    ScreenSpaceLines3D line09 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line10 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line11 = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D line12 = new ScreenSpaceLines3D();

                    line09.Color = lineColor;
                    line09.Points.Add(p0);
                    line09.Points.Add(p4);

                    line10.Color = lineColor;
                    line10.Points.Add(p1);
                    line10.Points.Add(p5);

                    line11.Color = lineColor;
                    line11.Points.Add(p2);
                    line11.Points.Add(p6);

                    line12.Color = lineColor;
                    line12.Points.Add(p3);
                    line12.Points.Add(p7);

                    _trackport.ViewPort.Children.Add(line09);
                    _trackport.ViewPort.Children.Add(line10);
                    _trackport.ViewPort.Children.Add(line11);
                    _trackport.ViewPort.Children.Add(line12);
                }
                
                //Count camera Position for Model
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(cmodel, 0, 300, 100);

                //SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                //GeometryModel3D model = getGeometryModel3D(brush, obj_CrSc, new Point3D(10, 10, 10), new Point3D(500, 300, 200));
                //gr.Children.Add(model);

                ////Point3D cameraPosition = ((MeshGeometry3D)model.Geometry).Positions[0];
                ////cameraPosition.Z -= 1000;

                //brush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                //model = getGeometryModel3D(brush, obj_CrSc, new Point3D(110, 110, 10), new Point3D(600, 400, 200));
                //gr.Children.Add(model);

                //IMPORTANT: this is the best way to do it, but we can't use it because of trackball
                //because camera is set by trackball Transform this.Camera.Transform = _trackball.Transform;
                //and headlite too:  this.Headlight.Transform = _trackball.Transform;

                _trackport.PerspectiveCamera.Position = cameraPosition;
                //_trackport.PerspectiveCamera.LookDirection = new Vector3D(cameraPosition.X, cameraPosition.Y, cameraPosition.Z - 100);

                _trackport.PerspectiveCamera.LookDirection = new Vector3D(0, -1, -0.2);

                _trackport.Model = (Model3D)gr;

                //bool bDisplayMembers_WireFrame = true;
                //if (bDisplayMembers_WireFrame) Drawing3D.DrawModelMembersWireFrame(cmodel, _trackport.ViewPort);

                bool bDisplayMembers_WireFrame = true;
                if (bDisplayMembers_WireFrame) Drawing3D.DrawModelMembersinOneWireFrame(cmodel, _trackport.ViewPort);

                bool bDisplayConnectionJointsWireFrame = true;
                if (bDisplayConnectionJointsWireFrame) Drawing3D.DrawModelConnectionJointsWireFrame(cmodel, _trackport.ViewPort);
                
                _trackport.SetupScene();
            }
        }

        //public ScreenSpaceLines3D wireFrame(CMember member, float x)
        //{
        //    ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();
        //    wireFrame.Color = Color.FromRgb(60, 60, 60);
        //    wireFrame.Thickness = 1.0;

        //    // Todo Prepracovat pre vnutornu a vonkajsiu outline
        //    // Zjednotit s AAC panel

        //    float fy, fz;

        //    if (member.CrScStart.CrScPointsOut != null && member.CrScStart.CrScPointsOut.Length > 0)
        //    {
        //        for (int i = 0; i < member.CrScStart.CrScPointsOut.Length / 2 - member.CrScStart.INoAuxPoints; i++)
        //        {
        //            Point3D pi = new Point3D();
        //            Point3D pj = new Point3D();

        //            if (i < member.CrScStart.CrScPointsOut.Length / 2 - member.CrScStart.INoAuxPoints - 1)
        //            {
        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);

        //                pi = new Point3D(x, fy, fz);

        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints + 1, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i + 1, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints + 1, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i + 1, 1], member.DTheta_x);

        //                pj = new Point3D(x, fy, fz);
        //            }
        //            else // Last line
        //            {
        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);

        //                pi = new Point3D(x, fy, fz);

        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + 0, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + 0, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + 0, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + 0, 1], member.DTheta_x);

        //                pj = new Point3D(x, fy, fz);
        //            }

        //            // Add points
        //            wireFrame.Points.Add(pi);
        //            wireFrame.Points.Add(pj);
        //        }
        //    }

        //    if(member.CrScStart.CrScPointsIn != null && member.CrScStart.CrScPointsIn.Length > 0)
        //    {
        //        for (int i = 0; i < member.CrScStart.CrScPointsIn.Length / 2 - member.CrScStart.INoAuxPoints; i++)
        //        {
        //            Point3D pi = new Point3D();
        //            Point3D pj = new Point3D();

        //            if (i < member.CrScStart.CrScPointsIn.Length / 2 - member.CrScStart.INoAuxPoints - 1)
        //            {
        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
                        
        //                pi = new Point3D(x, fy, fz);

        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints + 1, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i + 1, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints + 1, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i + 1, 1], member.DTheta_x);

        //                pj = new Point3D(x, fy, fz);
        //            }
        //            else // Last line
        //            {
        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);

        //                pi = new Point3D(x, fy, fz);

        //                // Rotate about local x-axis
        //                fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + 0, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + 0, 1], member.DTheta_x);
        //                fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + 0, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + 0, 1], member.DTheta_x);

        //                pj = new Point3D(x, fy, fz);
        //            }

        //            // Add points
        //            wireFrame.Points.Add(pi);
        //            wireFrame.Points.Add(pj);
        //        }
        //    }

        //    // Transform coordinates from LCS to GCS
        //    Point3D p_temp = new Point3D();
        //    p_temp.X = member.NodeStart.X;
        //    p_temp.Y = member.NodeStart.Y;
        //    p_temp.Z = member.NodeStart.Z;

        //    member.TransformMember_LCStoGCS(eGCS, p_temp, member.Delta_X, member.Delta_Y, member.Delta_Z, member.m_dTheta_x, wireFrame.Points);

        //    return wireFrame;
        //}

        //public ScreenSpaceLines3D wireFrameLateral(CMember member)
        //{
        //    ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();
        //    wireFrame.Color = Color.FromRgb(60, 60, 60);
        //    wireFrame.Thickness = 1.0;

        //    // Todo Prepracovat pre vnutornu a vonkajsiu outline
        //    // Zjednotit s AAC panel

        //    float fy, fz;

        //    if (member.CrScStart.CrScPointsOut != null && member.CrScStart.CrScPointsOut.Length > 0)
        //    {
        //        for (int i = 0; i < member.CrScStart.CrScPointsOut.Length / 2 - member.CrScStart.INoAuxPoints; i++)
        //        {
        //            Point3D pi = new Point3D();
        //            Point3D pj = new Point3D();

        //            // Rotate about local x-axis
        //            fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //            fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);

        //            pi = new Point3D(-member.FAlignment_Start, fy, fz);

        //            // Rotate about local x-axis
        //            fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //            fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsOut[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsOut[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);

        //            pj = new Point3D(member.FLength + member.FAlignment_End, fy, fz);

        //            // Add points
        //            wireFrame.Points.Add(pi);
        //            wireFrame.Points.Add(pj);
        //        }
        //    }

        //    if (member.CrScStart.CrScPointsIn != null && member.CrScStart.CrScPointsIn.Length > 0)
        //    {
        //        for (int i = 0; i < member.CrScStart.CrScPointsIn.Length / 2 - member.CrScStart.INoAuxPoints; i++)
        //        {
        //            Point3D pi = new Point3D();
        //            Point3D pj = new Point3D();

        //            // Rotate about local x-axis
        //            fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //            fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);

        //            pi = new Point3D(-member.FAlignment_Start, fy , fz);

        //            // Rotate about local x-axis
        //            fy = (float)Geom2D.GetRotatedPosition_x_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
        //            fz = (float)Geom2D.GetRotatedPosition_y_CCW(member.CrScStart.CrScPointsIn[i + member.CrScStart.INoAuxPoints, 0], member.CrScStart.CrScPointsIn[member.CrScStart.INoAuxPoints + i, 1], member.DTheta_x);
                    
        //            pj = new Point3D(member.FLength + member.FAlignment_End, fy, fz);

        //            // Add points
        //            wireFrame.Points.Add(pi);
        //            wireFrame.Points.Add(pj);
        //        }
        //    }

        //    // Transform coordinates from LCS to GCS
        //    Point3D p_temp = new Point3D();
        //    p_temp.X = member.NodeStart.X;
        //    p_temp.Y = member.NodeStart.Y;
        //    p_temp.Z = member.NodeStart.Z;

        //    member.TransformMember_LCStoGCS(eGCS, p_temp, member.Delta_X, member.Delta_Y, member.Delta_Z, member.m_dTheta_x, wireFrame.Points);

        //    return wireFrame;
        //}

        //public void DrawGlobalAxis(out ScreenSpaceLines3D sAxisX_3D, out ScreenSpaceLines3D sAxisY_3D, out ScreenSpaceLines3D sAxisZ_3D)
        //{
        //    // Global coordinate system - axis
        //    sAxisX_3D = new ScreenSpaceLines3D();
        //    sAxisY_3D = new ScreenSpaceLines3D();
        //    sAxisZ_3D = new ScreenSpaceLines3D();
        //    Point3D pGCS_centre = new Point3D(0, 0, 0);
        //    Point3D pAxisX = new Point3D(1, 0, 0);
        //    Point3D pAxisY = new Point3D(0, 1, 0);
        //    Point3D pAxisZ = new Point3D(0, 0, 1);

        //    sAxisX_3D.Points.Add(pGCS_centre);
        //    sAxisX_3D.Points.Add(pAxisX);
        //    sAxisX_3D.Color = Colors.Red;
        //    sAxisX_3D.Thickness = 2;

        //    sAxisY_3D.Points.Add(pGCS_centre);
        //    sAxisY_3D.Points.Add(pAxisY);
        //    sAxisY_3D.Color = Colors.Green;
        //    sAxisY_3D.Thickness = 2;

        //    sAxisZ_3D.Points.Add(pGCS_centre);
        //    sAxisZ_3D.Points.Add(pAxisZ);
        //    sAxisZ_3D.Color = Colors.Blue;
        //    sAxisZ_3D.Thickness = 2;
        //}
    }
}
