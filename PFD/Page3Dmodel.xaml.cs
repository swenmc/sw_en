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

        public Page3Dmodel(CModel model)
        {
            InitializeComponent();

            //zmena farby pre Trackport
            _trackport.TrackportBackground = new SolidColorBrush(Colors.Black);
            //_trackport.TrackportBackground = new SolidColorBrush(Colors.DarkBlue);

            // Global coordinate system - axis
            if (bShowGlobalAxis) Drawing3D.DrawGlobalAxis(_trackport.ViewPort);
            
            if (model != null)
            {
                // Frame Model
                Model3DGroup gr = Drawing3D.CreateModel3DGroup(model, EGCS.eGCSLeftHanded, true, true, true, true);

                float fModel_Length_X = 0;
                float fModel_Length_Y = 0;
                float fModel_Length_Z = 0;
                Point3D pModelGeomCentre = Drawing3D.GetModelCentre(model, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(model, 1, -(2 * fModel_Length_Y), 2 * fModel_Length_Z);
                                
                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);
                _trackport.Model = (Model3D)gr;
            }

            // Add WireFrame Model
            //if (bDisplay_WireFrame) Drawing3D.DrawModelMembersWireFrame_temp(model, _trackport.ViewPort);
            if (bDisplay_WireFrame) Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort);
            
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

                //Point3D pModelGeomCentre = new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
                //Point3D cameraPosition = new Point3D(pModelGeomCentre.X - 0.2f, pModelGeomCentre.Y + 0.005f, pModelGeomCentre.Z + 0.05f);
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

       
    }
}
