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

        // Tutorial
        /// http://kindohm.com/technical/WPF3DTutorial.htm  ScreenSpaceLines3D

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

            GeometryModel3D SolidModel3D = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();

            SolidModel3D.Geometry = mesh;
            SolidColorBrush br = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            SolidModel3D.Material = new DiffuseMaterial(br);

            gr.Children.Add(SolidModel3D); // Add solid to model group

            _trackport.Model = (Model3D)gr;

            _trackport.Trackball.TranslateScale = 1000; //step for moving object (panning)

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

                // Camera Position for Model
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(cmodel, 0, 300, 100);

                //IMPORTANT: this is the best way to do it, but we can't use it because of trackball
                //because camera is set by trackball Transform this.Camera.Transform = _trackball.Transform;
                //and headlite too:  this.Headlight.Transform = _trackball.Transform;

                _trackport.PerspectiveCamera.Position = cameraPosition;
                //_trackport.PerspectiveCamera.LookDirection = new Vector3D(cameraPosition.X, cameraPosition.Y, cameraPosition.Z - 100);

                _trackport.PerspectiveCamera.LookDirection = new Vector3D(0, -1, -0.2);

                _trackport.Model = (Model3D)gr;

                bool bDisplayMembers_WireFrame = true;
                if (bDisplayMembers_WireFrame) Drawing3D.DrawModelMembersinOneWireFrame(cmodel, _trackport.ViewPort);

                bool bDisplayConnectionJointsWireFrame = true;

                if (bDisplayConnectionJointsWireFrame)
                {
                    //if(jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(cmodel);
                    Drawing3D.DrawModelConnectionJointsWireFrame(cmodel, _trackport.ViewPort);
                }

                _trackport.SetupScene();
            }
        }
    }
}
