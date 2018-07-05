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
using CENEX;
using _3DTools;
using MATH;
using CRSC;
using BaseClasses;
using BaseClasses.GraphObj;
using HelixToolkit.Wpf;

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
        /////////////////////////////////////////////////

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
                

                /*
                    The following lights derive from the base class Light:
                    AmbientLight : Provides ambient lighting that illuminates all objects uniformly regardless of their location or orientation.
                    DirectionalLight : Illuminates like a distant light source. Directional lights have a Direction specified as a Vector3D, but no specified location.
                    PointLight : Illuminates like a nearby light source. PointLights have a position and cast light from that position. Objects in the scene are illuminated depending on their position and distance with respect to the light. PointLightBase exposes a Range property, which determines a distance beyond which models will not be illuminated by the light. PointLight also exposes attenuation properties which determine how the light's intensity diminishes over distance. You can specify constant, linear, or quadratic interpolations for the light's attenuation.
                    SpotLight : Inherits from PointLight. Spotlights illuminate like PointLight and have both position and direction. They project light in a cone-shaped area set by InnerConeAngle and OuterConeAngle properties, specified in degrees.
                */

                // Directional Light
                DirectionalLight Dir_Light = new DirectionalLight();
                Dir_Light.Color = Colors.White;
                Dir_Light.Direction = new Vector3D(0, 0, -1);
                gr.Children.Add(Dir_Light);

                // Point light values
                PointLight Point_Light = new PointLight();
                Point_Light.Position = new Point3D(0, 0, 30);
                Point_Light.Color = System.Windows.Media.Brushes.White.Color;
                Point_Light.Range = 30.0;
                Point_Light.ConstantAttenuation = 0;
                Point_Light.LinearAttenuation = 0;
                Point_Light.QuadraticAttenuation = 0.2f;
                Point_Light.ConstantAttenuation = 5.0;
                gr.Children.Add(Point_Light);

                SpotLight Spot_Light = new SpotLight();
                Spot_Light.InnerConeAngle = 30;
                Spot_Light.OuterConeAngle = 30;
                Spot_Light.Color = System.Windows.Media.Brushes.White.Color;
                Spot_Light.Direction = new Vector3D(0, 0, -1);
                Spot_Light.Position = new Point3D(8.5, 8.5, 20);
                Spot_Light.Range = 30;
                gr.Children.Add(Spot_Light);

                //Set Ambient Light
                AmbientLight Ambient_Light = new AmbientLight();
                Ambient_Light.Color = Color.FromRgb(250, 250, 230);
                gr.Children.Add(new AmbientLight());

                //Ondrej
                //Popravde absolutne netusim naco to tu je a co to robi
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

                bool bDisplayMembers_WireFrame = true;
                if (bDisplayMembers_WireFrame) Drawing3D.DrawModelMembersWireFrame(cmodel, _trackport.ViewPort);                

                bool bDisplayConnectionJointsWireFrame = true;
                if (bDisplayConnectionJointsWireFrame) Drawing3D.DrawModelConnectionJointsWireFrame(cmodel, _trackport.ViewPort);
                
                _trackport.SetupScene();
            }
        }

        public void CalculateModelLimits(CModel cmodel,
            out float fTempMax_X,
            out float fTempMin_X,
            out float fTempMax_Y,
            out float fTempMin_Y,
            out float fTempMax_Z,
            out float fTempMin_Z
            )
        {
            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;
            fTempMax_Z = float.MinValue;
            fTempMin_Z = float.MaxValue;

            if (cmodel.m_arrNodes != null) // Some nodes exist
            {
                for (int i = 0; i < cmodel.m_arrNodes.Length; i++)
                {
                    // Maximum X - coordinate
                    if (cmodel.m_arrNodes[i].X > fTempMax_X)
                        fTempMax_X = cmodel.m_arrNodes[i].X;

                    // Minimum X - coordinate
                    if (cmodel.m_arrNodes[i].X < fTempMin_X)
                        fTempMin_X = cmodel.m_arrNodes[i].X;

                    // Maximum Y - coordinate
                    if (cmodel.m_arrNodes[i].Y > fTempMax_Y)
                        fTempMax_Y = cmodel.m_arrNodes[i].Y;

                    // Minimum Y - coordinate
                    if (cmodel.m_arrNodes[i].Y < fTempMin_Y)
                        fTempMin_Y = cmodel.m_arrNodes[i].Y;

                    // Maximum Z - coordinate
                    if (cmodel.m_arrNodes[i].Z > fTempMax_Z)
                        fTempMax_Z = cmodel.m_arrNodes[i].Z;

                    // Minimum Z - coordinate
                    if (cmodel.m_arrNodes[i].Z < fTempMin_Z)
                        fTempMin_Z = cmodel.m_arrNodes[i].Z;
                }
            }
            else if (cmodel.m_arrGOPoints != null) // Some points exist
            {
                for (int i = 0; i < cmodel.m_arrGOPoints.Length; i++)
                {
                    // Maximum X - coordinate
                    if (cmodel.m_arrGOPoints[i].X > fTempMax_X)
                        fTempMax_X = (float)cmodel.m_arrGOPoints[i].X;

                    // Minimum X - coordinate
                    if (cmodel.m_arrGOPoints[i].X < fTempMin_X)
                        fTempMin_X = (float)cmodel.m_arrGOPoints[i].X;

                    // Maximum Y - coordinate
                    if (cmodel.m_arrGOPoints[i].Y > fTempMax_Y)
                        fTempMax_Y = (float)cmodel.m_arrGOPoints[i].Y;

                    // Minimum Y - coordinate
                    if (cmodel.m_arrGOPoints[i].Y < fTempMin_Y)
                        fTempMin_Y = (float)cmodel.m_arrGOPoints[i].Y;

                    // Maximum Z - coordinate
                    if (cmodel.m_arrGOPoints[i].Z > fTempMax_Z)
                        fTempMax_Z = (float)cmodel.m_arrGOPoints[i].Z;

                    // Minimum Z - coordinate
                    if (cmodel.m_arrGOPoints[i].Z < fTempMin_Z)
                        fTempMin_Z = (float)cmodel.m_arrGOPoints[i].Z;
                }
            }
            else
            {
                // Exception - no definition nodes or points
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

        public void DrawGlobalAxis(out ScreenSpaceLines3D sAxisX_3D, out ScreenSpaceLines3D sAxisY_3D, out ScreenSpaceLines3D sAxisZ_3D)
        {
            // Global coordinate system - axis
            sAxisX_3D = new ScreenSpaceLines3D();
            sAxisY_3D = new ScreenSpaceLines3D();
            sAxisZ_3D = new ScreenSpaceLines3D();
            Point3D pGCS_centre = new Point3D(0, 0, 0);
            Point3D pAxisX = new Point3D(1, 0, 0);
            Point3D pAxisY = new Point3D(0, 1, 0);
            Point3D pAxisZ = new Point3D(0, 0, 1);

            sAxisX_3D.Points.Add(pGCS_centre);
            sAxisX_3D.Points.Add(pAxisX);
            sAxisX_3D.Color = Colors.Red;
            sAxisX_3D.Thickness = 2;

            sAxisY_3D.Points.Add(pGCS_centre);
            sAxisY_3D.Points.Add(pAxisY);
            sAxisY_3D.Color = Colors.Green;
            sAxisY_3D.Thickness = 2;

            sAxisZ_3D.Points.Add(pGCS_centre);
            sAxisZ_3D.Points.Add(pAxisZ);
            sAxisZ_3D.Color = Colors.Blue;
            sAxisZ_3D.Thickness = 2;
        }
    }
}
