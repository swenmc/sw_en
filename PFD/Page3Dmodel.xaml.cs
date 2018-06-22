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
        public Page3Dmodel(CModel model)
        {
            InitializeComponent();

            bool bDebugging = false;

            // Create 3D window
            Window2 win1 = new Window2(model, bDebugging);

            // Global Axis System
            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            EGCS eGCS = EGCS.eGCSLeftHanded;
            //EGCS eGCS = EGCS.eGCSRightHanded;

            bool bShowGlobalAxis = true;

            // Global coordinate system - axis
            if (bShowGlobalAxis)
            {
                // Global coordinate system - axis
                ScreenSpaceLines3D sAxisX_3D;
                ScreenSpaceLines3D sAxisY_3D;
                ScreenSpaceLines3D sAxisZ_3D;

                win1.DrawGlobalAxis(out sAxisX_3D, out sAxisY_3D, out sAxisZ_3D);

                //I made ViewPort public property to Access ViewPort object inside TrackPort3D
                //to ViewPort add 3 children (3 axis)
                _trackport.ViewPort.Children.Add(sAxisX_3D);
                _trackport.ViewPort.Children.Add(sAxisY_3D);
                _trackport.ViewPort.Children.Add(sAxisZ_3D);
            }

            // Frame Model
            Model3DGroup gr = new Model3DGroup();

            float fTempMax_X;
            float fTempMin_X;
            float fTempMax_Y;
            float fTempMin_Y;
            float fTempMax_Z;
            float fTempMin_Z;

            if (model != null)
            {
                gr = win1.gr;

                // Get model centre

                win1.CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                float fModel_Length_X = fTempMax_X - fTempMin_X;
                float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
                float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

                Point3D pModelGeomCentre = new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
                Point3D cameraPosition = new Point3D(pModelGeomCentre.X + 1, pModelGeomCentre.Y - (2 * fModel_Length_Y), pModelGeomCentre.Z + (2 * fModel_Length_Z));

                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = new Vector3D(-(cameraPosition.X - pModelGeomCentre.X), -(cameraPosition.Y - pModelGeomCentre.Y), -(cameraPosition.Z - pModelGeomCentre.Z));
                _trackport.Model = (Model3D)gr;
            }

            // Add WireFrame Model
            // Todo - Zjednotit funckie pre vykreslovanie v oknach WIN 2, AAC a PORTAL FRAME

            bool bDisplayMembers_WireFrame = true;

            // Members - Wire Frame
            if (bDisplayMembers_WireFrame && model != null  && model.m_arrMembers != null)
            {
                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null) // Member object is valid (not empty)
                    {
                        // Create WireFrime in LCS
                        ScreenSpaceLines3D wireFrame_FrontSide = win1.wireFrame(model.m_arrMembers[i], 0);
                        ScreenSpaceLines3D wireFrame_BackSide = win1.wireFrame(model.m_arrMembers[i], model.m_arrMembers[i].FLength);
                        ScreenSpaceLines3D wireFrame_Lateral = win1.wireFrameLateral(model.m_arrMembers[i]);

                        // Add Wireframe Lines to the trackport
                        _trackport.ViewPort.Children.Add(wireFrame_FrontSide);
                        _trackport.ViewPort.Children.Add(wireFrame_BackSide);
                        _trackport.ViewPort.Children.Add(wireFrame_Lateral);
                    }
                }
            }

            _trackport.SetupScene();
        }

        public Page3Dmodel(CConnectionComponentEntity3D model)
        {
            InitializeComponent();

            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(Colors.LightGreen);

            bool bDisplay_SurfaceModel = true;
            GeometryModel3D ComponentGeomModel;

            // Surface Model (Solid / Transparent (depends on material opacity)
            if (bDisplay_SurfaceModel && model != null)
            {
                // Component Model
                ComponentGeomModel = new GeometryModel3D();
            }

            float fTempMax_X;
            float fTempMin_X;
            float fTempMax_Y;
            float fTempMin_Y;
            float fTempMax_Z;
            float fTempMin_Z;

            if (model != null)
            {
                ComponentGeomModel = model.CreateGeomModel3D(brushDefault);

                // Get model centre
                CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                float fModel_Length_X = fTempMax_X - fTempMin_X;
                float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
                float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

                Point3D pModelGeomCentre = new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
                Point3D cameraPosition = new Point3D(pModelGeomCentre.X + 0.1 * fModel_Length_X, pModelGeomCentre.Y - (2 * fModel_Length_Y), pModelGeomCentre.Z + (2 * fModel_Length_Z));

                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = new Vector3D(-(cameraPosition.X - pModelGeomCentre.X), -(cameraPosition.Y - pModelGeomCentre.Y), -(cameraPosition.Z - pModelGeomCentre.Z));
                _trackport.Model = (Model3D)ComponentGeomModel;
            }

            // Add WireFrame Model
            // Todo - Zjednotit funckie pre vykreslovanie v oknach WIN 2, AAC a PORTAL FRAME

            bool bDisplay_WireFrame = true;

            // Wire Frame
            if (bDisplay_WireFrame && model != null)
            {
                // Create WireFrime in LCS
                ScreenSpaceLines3D wireFrame = model.CreateWireFrameModel();

                // Add Wireframe Lines to the trackport
                _trackport.ViewPort.Children.Add(wireFrame);
            }

            _trackport.SetupScene();
        }

        public Page3Dmodel(CCrSc_TW crsc)
        {
            InitializeComponent();

            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(Colors.Cyan);

            // Component Model
            Model3DGroup ComponentGeomModel = new Model3DGroup();

            bool bDisplay_WireFrame = true;
            bool bDisplay_SurfaceModel = true;
            float fTempMax_X;
            float fTempMin_X;
            float fTempMax_Y;
            float fTempMin_Y;
            float fTempMax_Z;
            float fTempMin_Z;

            if (crsc != null)
            {
                CMember member_temp = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0.5f, 0, 0, 0), crsc, 0);

                ComponentGeomModel = member_temp.getM_3D_G_Member(EGCS.eGCSLeftHanded, brushDefault, brushDefault, brushDefault);

                // Get model centre
                CalculateModelLimits(member_temp, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                float fModel_Length_X = fTempMax_X - fTempMin_X;
                float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
                float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

                Point3D pModelGeomCentre = new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
                Point3D cameraPosition = new Point3D(pModelGeomCentre.X - 0.2f, pModelGeomCentre.Y + 0.005f, pModelGeomCentre.Z + 0.05f);

                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = new Vector3D(-(cameraPosition.X - pModelGeomCentre.X), -(cameraPosition.Y - pModelGeomCentre.Y), -(cameraPosition.Z - pModelGeomCentre.Z));

                if (bDisplay_SurfaceModel)
                {
                    _trackport.Model = (Model3D)ComponentGeomModel;
                }

                // Add WireFrame Model
                // Todo - Zjednotit funckie pre vykreslovanie v oknach WIN 2, AAC a PORTAL FRAME

                // Members - Wire Frame
                if (bDisplay_WireFrame)
                {
                    // Create WireFrime in LCS
                    //wireFrame = model.CreateWireFrameModel(); // TODO dopracovat funkcie pre zobrazenie, implementovat v objekte pruta a nie na urovni cmodelu

                    ScreenSpaceLines3D wireFrame_FrontSide = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D wireFrame_BackSide = new ScreenSpaceLines3D();
                    ScreenSpaceLines3D wireFrame_Lateral = new ScreenSpaceLines3D();

                    Window2 wtemp = new Window2(); // ToDo - odstranit

                    wireFrame_FrontSide = wtemp.wireFrame(member_temp, 0f);
                    wireFrame_BackSide = wtemp.wireFrame(member_temp, 0.5f);
                    wireFrame_Lateral = wtemp.wireFrameLateral(member_temp);

                    // Add Wireframe Lines to the trackport
                    //_trackport.ViewPort.Children.Add(wireFrame);
                    _trackport.ViewPort.Children.Add(wireFrame_FrontSide);
                    _trackport.ViewPort.Children.Add(wireFrame_BackSide);
                    _trackport.ViewPort.Children.Add(wireFrame_Lateral);
                }
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

        public void CalculateModelLimits(CMember member,
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

            // TODO upravit tak aby sme vedeli ziskat obecne rozmery z modelu, z pruta, z plechu atd
            // Pripadne riesit vsetko ako cmodel, ale to je pre preview jedneho dielcieho objektu neumerne velke


            if (member.CrScStart.CrScPointsOut != null) // Some cross-section points exist
            {
                // Maximum X - coordinate
                fTempMax_X = member.NodeStart.X;

                // Minimum X - coordinate
                fTempMin_X = member.NodeEnd.X;

                for (int i = 0; i < member.CrScStart.CrScPointsOut.Length/2; i++)
                {
                    // Maximum Y - coordinate
                    if (member.CrScStart.CrScPointsOut[i,0] > fTempMax_Y)
                        fTempMax_Y = member.CrScStart.CrScPointsOut[i, 0];

                    // Minimum Y - coordinate
                    if (member.CrScStart.CrScPointsOut[i, 0] < fTempMin_Y)
                        fTempMin_Y = member.CrScStart.CrScPointsOut[i, 0];

                    // Maximum Z - coordinate
                    if (member.CrScStart.CrScPointsOut[i, 1] > fTempMax_Z)
                        fTempMax_Z = member.CrScStart.CrScPointsOut[i, 1];

                    // Minimum Z - coordinate
                    if (member.CrScStart.CrScPointsOut[i, 1] < fTempMin_Z)
                        fTempMin_Z = member.CrScStart.CrScPointsOut[i, 1];
                }
            }
        }
    }
}
