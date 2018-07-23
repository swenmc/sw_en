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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BaseClasses;
using sw_en_GUI;
using CRSC;
using System.Windows.Media.Media3D;
using _3DTools;

namespace PFD
{
    /// <summary>
    /// Interaction logic for Page3Dmodel.xaml
    /// </summary>
    public partial class Page3Dmodel : Page
    {
        //bool bDebugging = false;
        bool bShowGlobalAxis = true;
        public bool bDisplay_WireFrame = true;
        public bool bDisplay_SurfaceModel = true;

        public Page3Dmodel(CModel model, bool directionalLigth, bool pointLight, bool spotLight, bool ambientLight)
        {
            InitializeComponent();

            //Color of Trackport
            _trackport.TrackportBackground = new SolidColorBrush(Colors.Black);

            // Global coordinate system - axis
            if (bShowGlobalAxis) Drawing3D.DrawGlobalAxis(_trackport.ViewPort);
            
            if (model != null)
            {
                // Frame Model
                Model3DGroup gr = Drawing3D.CreateModel3DGroup(model, EGCS.eGCSLeftHanded, true, true, true, directionalLigth, pointLight, spotLight, ambientLight);

                float fModel_Length_X = 0;
                float fModel_Length_Y = 0;
                float fModel_Length_Z = 0;
                Point3D pModelGeomCentre = Drawing3D.GetModelCentre(model, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(model, 1, -(2 * fModel_Length_Y), 2 * fModel_Length_Z);
                
                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);
                _trackport.Model = (Model3D)gr;

                //testing texts in 3D
                Point3D textPoint = new Point3D(pModelGeomCentre.X - fModel_Length_X / 2 - fModel_Length_X/10, pModelGeomCentre.Y - fModel_Length_Y / 2, pModelGeomCentre.Z);
                Point3D textPoint2 = new Point3D(pModelGeomCentre.X + fModel_Length_X / 2 + fModel_Length_X / 10, pModelGeomCentre.Y - fModel_Length_Y / 2, pModelGeomCentre.Z);
                Point3D tp = new Point3D(pModelGeomCentre.X, pModelGeomCentre.Y - fModel_Length_Y / 2 + 0.3, pModelGeomCentre.Z);
                Point3D tp2 = new Point3D(pModelGeomCentre.X, pModelGeomCentre.Y - fModel_Length_Y / 2 - 0.2, pModelGeomCentre.Z);

                _trackport.ViewPort.Children.Add(CreateTextLabel3D("10 m", new SolidColorBrush(Colors.White), true, 0.1, textPoint, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)));
                _trackport.ViewPort.Children.Add(CreateTextLabel3D("1234 mm", new SolidColorBrush(Colors.Red), true, 0.1, textPoint2, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)));
                _trackport.ViewPort.Children.Add(CreateTextLabel3D("TestText-row1", new SolidColorBrush(Colors.Gold), true, 0.1, tp, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)));
                _trackport.ViewPort.Children.Add(CreateTextLabel3D("TestText-row2", new SolidColorBrush(Colors.Gold), true, 0.05, tp2, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)));
            }

            // Add WireFrame Model
            if (bDisplay_WireFrame) Drawing3D.DrawModelMembersinOneWireFrame(model, _trackport.ViewPort);

            _trackport.SetupScene();
        }

        public Page3Dmodel(CConnectionComponentEntity3D model)
        {
            InitializeComponent();

            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(Colors.Cyan);

            // Cross-section Model
            GeometryModel3D ComponentGeomModel;

            float fTempMax_X;
            float fTempMin_X;
            float fTempMax_Y;
            float fTempMin_Y;
            float fTempMax_Z;
            float fTempMin_Z;

            if (model != null)
            {
                ComponentGeomModel = model.CreateGeomModel3D(brushDefault);

                // Get model limits
                CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                float fModel_Length_X = fTempMax_X - fTempMin_X;
                float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
                float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

                Point3D pModelGeomCentre = new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
                Point3D cameraPosition = new Point3D(pModelGeomCentre.X, pModelGeomCentre.Y + 0.1, pModelGeomCentre.Z + 1);

                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = new Vector3D(-(cameraPosition.X - pModelGeomCentre.X), -(cameraPosition.Y - pModelGeomCentre.Y), -(cameraPosition.Z - pModelGeomCentre.Z));

                if (bDisplay_SurfaceModel)
                {
                    _trackport.Model = (Model3D)ComponentGeomModel;
                }

                // Add WireFrame Model
                // Todo - Zjednotit funckie pre vykreslovanie v oknach WIN 2, AAC a PORTAL FRAME (PAGE3D)

                // Component - Wire Frame
                if (bDisplay_WireFrame && model != null)
                {
                    // Create WireFrime in LCS
                    ScreenSpaceLines3D wireFrame = model.CreateWireFrameModel();

                    // Add Wireframe Lines to the trackport
                    _trackport.ViewPort.Children.Add(wireFrame);
                }
            }

            _trackport.SetupScene();
        }

        public Page3Dmodel(CCrSc_TW crsc)
        {
            InitializeComponent();

            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(crsc.CSColor);

            // Cross-section Model
            Model3DGroup ComponentGeomModel = new Model3DGroup();
            
            if (crsc != null)
            {
                float fLengthMember = 0.2f;
                CMember member_temp = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, fLengthMember, 0, 0, 0), crsc, 0);

                ComponentGeomModel = member_temp.getM_3D_G_Member(EGCS.eGCSLeftHanded, brushDefault, brushDefault, brushDefault);

                Point3D pModelGeomCentre = Drawing3D.GetModelCentre(member_temp);
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(pModelGeomCentre, -0.2f, 0.005f, 0.05f);

                _trackport.PerspectiveCamera.Position = cameraPosition;
                //_trackport.PerspectiveCamera.LookDirection = new Vector3D(-(cameraPosition.X - pModelGeomCentre.X), -(cameraPosition.Y - pModelGeomCentre.Y), -(cameraPosition.Z - pModelGeomCentre.Z));
                _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);

                if (bDisplay_SurfaceModel)
                {
                    _trackport.Model = (Model3D)ComponentGeomModel;
                }

                // Add WireFrame Model
                if (bDisplay_WireFrame) Drawing3D.DrawMemberWireFrame(member_temp, _trackport.ViewPort, fLengthMember);
            }

            _trackport.SetupScene();
        }

        public void CalculateModelLimits(CConnectionComponentEntity3D componentmodel,
     out float fTempMax_X,
     out float fTempMin_X,
     out float fTempMax_Y,
     out float fTempMin_Y,
     out float fTempMax_Z,
     out float fTempMin_Z
     )
        {
            // TODO upravit tak aby sme vedeli ziskat obecne rozmery z modelu, z pruta, z plechu, telesa atd
            // Pripadne riesit vsetko ako cmodel, ale to je pre preview jedneho dielcieho objektu neumerne velke

            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;
            fTempMax_Z = float.MinValue;
            fTempMin_Z = float.MaxValue;

            if (componentmodel.arrPoints3D != null) // Some nodes exist
            {
                for (int i = 0; i < componentmodel.arrPoints3D.Length; i++)
                {
                    // Maximum X - coordinate
                    if (componentmodel.arrPoints3D[i].X > fTempMax_X)
                        fTempMax_X = (float)componentmodel.arrPoints3D[i].X;

                    // Minimum X - coordinate
                    if ((float)componentmodel.arrPoints3D[i].X < fTempMin_X)
                        fTempMin_X = (float)componentmodel.arrPoints3D[i].X;

                    // Maximum Y - coordinate
                    if ((float)componentmodel.arrPoints3D[i].Y > fTempMax_Y)
                        fTempMax_Y = (float)componentmodel.arrPoints3D[i].Y;

                    // Minimum Y - coordinate
                    if ((float)componentmodel.arrPoints3D[i].Y < fTempMin_Y)
                        fTempMin_Y = (float)componentmodel.arrPoints3D[i].Y;

                    // Maximum Z - coordinate
                    if ((float)componentmodel.arrPoints3D[i].Z > fTempMax_Z)
                        fTempMax_Z = (float)componentmodel.arrPoints3D[i].Z;

                    // Minimum Z - coordinate
                    if ((float)componentmodel.arrPoints3D[i].Z < fTempMin_Z)
                        fTempMin_Z = (float)componentmodel.arrPoints3D[i].Z;
                }
            }
        }


        /// <summary>
        /// Creates a ModelVisual3D containing a text label.
        /// </summary>
        /// <param name="text">The string</param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="bDoubleSided">Visible from both sides?</param>
        /// <param name="height">Height of the characters</param>
        /// <param name="center">The center of the label</param>
        /// <param name="over">Horizontal direction of the label</param>
        /// <param name="up">Vertical direction of the label</param>
        /// <returns>Suitable for adding to your Viewport3D</returns>
        public static ModelVisual3D CreateTextLabel3D(
            string text,
            Brush textColor,
            bool bDoubleSided,
            double height,
            Point3D center,
            Vector3D over,
            Vector3D up)
        {
            // First we need a textblock containing the text of our label
            TextBlock tb = new TextBlock(new Run(text));
            tb.Foreground = textColor;
            tb.FontFamily = new FontFamily("Arial");

            // Now use that TextBlock as the brush for a material
            DiffuseMaterial mat = new DiffuseMaterial();
            mat.Brush = new VisualBrush(tb);

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * over - height / 2 * up;
            Point3D p1 = p0 + up * 1 * height;
            Point3D p2 = p0 + over * width;
            Point3D p3 = p0 + up * 1 * height + over * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.

            MeshGeometry3D mg = new MeshGeometry3D();
            mg.Positions = new Point3DCollection();
            mg.Positions.Add(p0);    // 0
            mg.Positions.Add(p1);    // 1
            mg.Positions.Add(p2);    // 2
            mg.Positions.Add(p3);    // 3

            if (bDoubleSided)
            {
                mg.Positions.Add(p0);    // 4
                mg.Positions.Add(p1);    // 5
                mg.Positions.Add(p2);    // 6
                mg.Positions.Add(p3);    // 7
            }

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            if (bDoubleSided)
            {
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(5);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(6);
            }

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.

            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            if (bDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(1, 1));
                mg.TextureCoordinates.Add(new Point(1, 0));
                mg.TextureCoordinates.Add(new Point(0, 1));
                mg.TextureCoordinates.Add(new Point(0, 0));
            }

            // And that's all.  Return the result.

            ModelVisual3D mv3d = new ModelVisual3D();
            mv3d.Content = new GeometryModel3D(mg, mat); ;
            return mv3d;
        }


    }
}
