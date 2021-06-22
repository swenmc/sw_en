using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses;
using _3DTools;
using CRSC;
using System.IO;
using System;
using System.Windows;
using System.Collections.Generic;
using DATABASE.DTO;

namespace PFD
{
    /// <summary>
    /// Interaction logic for Page3Dmodel.xaml
    /// </summary>
    public partial class Page3Dmodel : Page
    {
        //bool bDebugging = false;
        private DisplayOptions sDisplayOptions;
        public Model3DGroup gr = new Model3DGroup();
        public EGCS eGCS = EGCS.eGCSLeftHanded;        

        public Page3Dmodel(CModel model, DisplayOptions sDisplayOptions_temp, CLoadCase loadcase, Dictionary<CConnectionDescription, CConnectionJointTypes> jointsDict)
        {
            sDisplayOptions = sDisplayOptions_temp;

            InitializeComponent();

            Drawing3D.DrawToTrackPort(_trackport, model, 1f, sDisplayOptions, loadcase, jointsDict);            
        }
        public Page3Dmodel(CModel model, DisplayOptions sDisplayOptions_temp, EModelType modelType)
        {
            sDisplayOptions = sDisplayOptions_temp;

            InitializeComponent();

            if (modelType == EModelType.eJoint)
                Drawing3D.DrawJointToTrackPort(_trackport, model, 1f, sDisplayOptions);
            else if(modelType == EModelType.eFooting)
                Drawing3D.DrawFootingToTrackPort(_trackport, model, 1f, sDisplayOptions);
            else
                Drawing3D.DrawToTrackPort(_trackport, model, 1f, sDisplayOptions, null, null);
        }
        public Page3Dmodel(CConnectionComponentEntity3D model, DisplayOptions sDisplayOptions_temp)
        {
            sDisplayOptions = sDisplayOptions_temp;

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

                if (sDisplayOptions.bDisplaySolidModel)
                {
                    _trackport.Model = (Model3D)ComponentGeomModel;
                }

                // Add WireFrame Model
                // Todo - Zjednotit funckie pre vykreslovanie v oknach WIN 2, AAC a PORTAL FRAME (PAGE3D)

                // Component - Wire Frame
                if (sDisplayOptions.bDisplayWireFrameModel && model != null)
                {
                    // Create WireFrime in LCS
                    ScreenSpaceLines3D wireFrame = model.CreateWireFrameModel();

                    // Add Wireframe Lines to the trackport
                    _trackport.ViewPort.Children.Add(wireFrame);
                }
            }

            _trackport.SetupScene();
        }
        public Page3Dmodel(CPlate model, DisplayOptions sDisplayOptions_temp)
        {
            sDisplayOptions = sDisplayOptions_temp;

            InitializeComponent();

            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(Colors.Cyan);
            
            float fTempMax_X;
            float fTempMin_X;
            float fTempMax_Y;
            float fTempMin_Y;
            float fTempMax_Z;
            float fTempMin_Z;

            if (model != null)
            {
                if (sDisplayOptions.bMirrorPlate3D)
                {
                    if (model is CPlate_Frame)
                    {
                        (model as CPlate_Frame).MirrorPlate();
                    }
                }                

                // Get model limits
                CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                float fModel_Length_X = fTempMax_X - fTempMin_X;
                float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
                float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

                Point3D pModelGeomCentre = new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
                Point3D cameraPosition = new Point3D(0,0, Math.Max(fModel_Length_X, fModel_Length_Y) * 2);

                _trackport.TrackportBackground = new SolidColorBrush(Colors.Black);
                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = new Vector3D(0,0,-1);

                if (sDisplayOptions.bDisplaySolidModel)
                {
                    Model3DGroup gr = null;
                    if (sDisplayOptions.bDisplayConnectors) gr = model.CreateGeomModel3DWithConnectors(brushDefault, null);
                    else { gr = new Model3DGroup(); gr.Children.Add(model.CreateGeomModel3D(brushDefault)); }
                                        
                    if (sDisplayOptions.bDisplayNodes)
                    {
                        Model3DGroup nodes3DGroup = null;
                        if (sDisplayOptions.bDisplayNodes) nodes3DGroup = Drawing3D.CreateModelNodes_Model3DGroup(model, sDisplayOptions);
                        if (nodes3DGroup != null) gr.Children.Add(nodes3DGroup);                        
                    }
                    if (sDisplayOptions.bDisplayNodesDescription)
                    {   
                        Drawing3D.CreateNodesDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions, fModel_Length_X, fModel_Length_Y, fModel_Length_Z);
                    }

                    //translate transform to model center
                    ((Model3D)gr).Transform = new TranslateTransform3D(-fModel_Length_X / 2.0f, -fModel_Length_Y / 2.0f, -fModel_Length_Z / 2.0f);

                    Drawing3D.AddLightsToModel3D(gr, sDisplayOptions);
                    if(sDisplayOptions.bDisplayGlobalAxis) Drawing3D.DrawGlobalAxis(_trackport.ViewPort, null, null);
                    
                    _trackport.Model = (Model3D)gr;
                }
                
                // Component - Wire Frame
                if (sDisplayOptions.bDisplayWireFrameModel && model != null)
                {
                    // Create WireFrime in LCS
                    ScreenSpaceLines3D wireFrame = model.CreateWireFrameModel();

                    //translate transform to model center
                    wireFrame.Transform = new TranslateTransform3D(-fModel_Length_X / 2.0f, -fModel_Length_Y / 2.0f, -fModel_Length_Z / 2.0f);
                    // Add Wireframe Lines to the trackport
                    _trackport.ViewPort.Children.Add(wireFrame);
                }
            }

            _trackport.SetupScene();
        }
        public Page3Dmodel(CCrSc_TW crsc, DisplayOptions sDisplayOptions_temp)
        {
            sDisplayOptions = sDisplayOptions_temp;

            InitializeComponent();

            // Default color
            SolidColorBrush brushDefault = new SolidColorBrush(crsc.CSColor);

            // Cross-section Model
            Model3DGroup ComponentGeomModel = new Model3DGroup();
            
            if (crsc != null)
            {
                float fLengthMember = 0.2f;
                CMember member_temp = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, fLengthMember, 0, 0, 0), crsc, 0);

                ComponentGeomModel = member_temp.getM_3D_G_Member(EGCS.eGCSLeftHanded, brushDefault, brushDefault, brushDefault,true, true);

                Point3D pModelGeomCentre = Drawing3D.GetModelCentre(member_temp);
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(pModelGeomCentre, -0.2f, 0.005f, 0.05f);

                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);

                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayMembers)
                {
                    _trackport.Model = (Model3D)ComponentGeomModel;
                }

                // Add WireFrame Model
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers) Drawing3D.DrawMemberWireFrame(member_temp, _trackport.ViewPort, fLengthMember);
            }

            _trackport.SetupScene();
        }

        public Page3Dmodel(string pathToModel, PerspectiveCamera camera)
        {
            InitializeComponent();

            if (camera != null)
            {
                _trackport.PerspectiveCamera.Position = camera.Position;
                _trackport.PerspectiveCamera.LookDirection = camera.LookDirection;
                _trackport.PerspectiveCamera.UpDirection = camera.UpDirection;
                _trackport.PerspectiveCamera.FieldOfView = camera.FieldOfView;
            } 
            LoadXAMLResource(pathToModel);
        }

        private void LoadXAMLResource(string path)
        {
            if (path != null)
            {
                try
                {
                    using (FileStream file = File.OpenRead(path))
                    {
                        _trackport.LoadModel(file);
                        
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        String.Format("Unable to parse file:\r\n\r\n{0}",
                        e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
