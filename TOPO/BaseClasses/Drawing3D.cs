using _3DTools;
using MATH;
using Petzold.Media3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public static class Drawing3D
    {
        public static void DrawToTrackPort(Trackport3D _trackport, CModel model, DisplayOptions sDisplayOptions, CLoadCase loadcase)
        {
            //DateTime start = DateTime.Now;

            // Color of Trackport
            _trackport.TrackportBackground = new SolidColorBrush(Colors.Black);

            // Global coordinate system - axis
            if (sDisplayOptions.bDisplayGlobalAxis) Drawing3D.DrawGlobalAxis(_trackport.ViewPort, model);

            //System.Diagnostics.Trace.WriteLine("Beginning: " + (DateTime.Now - start).TotalMilliseconds);
            if (model != null)
            {
                Model3DGroup gr = new Model3DGroup();

                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayMembers) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);
                //System.Diagnostics.Trace.WriteLine("After CreateMembersModel3D: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateConnectionJointsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                bool displayOtherObjects3D = true;
                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateModelOtherObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup loadsModel3DGroup = null;
                if (sDisplayOptions.bDisplayLoads) loadsModel3DGroup = Drawing3D.CreateModelLoadObjectsModel3DGroup(loadcase);
                if (loadsModel3DGroup != null) gr.Children.Add(loadsModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateModelLoadObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Drawing3D.AddLightsToModel3D(gr, sDisplayOptions);

                float fModel_Length_X = 0;
                float fModel_Length_Y = 0;
                float fModel_Length_Z = 0;
                Point3D pModelGeomCentre = Drawing3D.GetModelCentre(model, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);
                Point3D cameraPosition = Drawing3D.GetModelCameraPosition(model, 1, -(2 * fModel_Length_Y), 2 * fModel_Length_Z);

                _trackport.PerspectiveCamera.Position = cameraPosition;
                _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);
                _trackport.Model = (Model3D)gr;

                // Add centerline member model
                if (sDisplayOptions.bDisplayMembersCenterLines && sDisplayOptions.bDisplayMembers) Drawing3D.DrawModelMembersCenterLines(model, _trackport.ViewPort);
                //System.Diagnostics.Trace.WriteLine("After DrawModelMembersCenterLines: " + (DateTime.Now - start).TotalMilliseconds);

                // Add WireFrame Model
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers)
                {
                    if(membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial);
                    Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort);
                } 
                //System.Diagnostics.Trace.WriteLine("After DrawModelMembersinOneWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints)
                {
                    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort);
                }
                //System.Diagnostics.Trace.WriteLine("After DrawModelConnectionJointsWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                if (sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMemberDescription)
                {
                    Drawing3D.CreateMembersDescriptionModel3D(model, _trackport.ViewPort);
                    //System.Diagnostics.Trace.WriteLine("After DrawMemberDescriptionTexts: " + (DateTime.Now - start).TotalMilliseconds);
                }

            }

            _trackport.SetupScene();
        }

        //-------------------------------------------------------------------------------------------------------------
        // Get model centre
        public static Point3D GetModelCentre(CModel model)
        {
            float fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y, fTempMax_Z, fTempMin_Z;

            CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

            float fModel_Length_X = fTempMax_X - fTempMin_X;
            float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
            float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

            return new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
        }
        public static Point3D GetModelCentre(CMember member)
        {
            double fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y, fTempMax_Z, fTempMin_Z;

            member.CalculateMemberLimits(out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

            double fModel_Length_X = fTempMax_X - fTempMin_X;
            double fModel_Length_Y = fTempMax_Y - fTempMin_Y;
            double fModel_Length_Z = fTempMax_Z - fTempMin_Z;

            return new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
        }
        public static Point3D GetModelCentre(CModel model, out float fModel_Length_X, out float fModel_Length_Y, out float fModel_Length_Z)
        {
            float fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y, fTempMax_Z, fTempMin_Z;

            CalculateModelLimits(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

            fModel_Length_X = fTempMax_X - fTempMin_X;
            fModel_Length_Y = fTempMax_Y - fTempMin_Y;
            fModel_Length_Z = fTempMax_Z - fTempMin_Z;

            return new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
        }
        public static Vector3D GetLookDirection(Point3D cameraPosition, Point3D modelCentre)
        {
            Vector3D lookDirection = new Vector3D(-(cameraPosition.X - modelCentre.X), -(cameraPosition.Y - modelCentre.Y), -(cameraPosition.Z - modelCentre.Z));
            return lookDirection;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Get model camera position
        public static Point3D GetModelCameraPosition(CModel model, double x, double y, double z)
        {
            Point3D center = GetModelCentre(model);
            return new Point3D(center.X + x, center.Y + y, center.Z + z);
        }

        public static Point3D GetModelCameraPosition(Point3D centerPoint, double x, double y, double z)
        {
            return new Point3D(centerPoint.X + x, centerPoint.Y + y, centerPoint.Z + z);
        }

        public static Model3DGroup CreateModel3DGroup(CModel model, DisplayOptions sDisplayOptions, EGCS egcs = EGCS.eGCSLeftHanded)
        {
            Model3DGroup gr = new Model3DGroup();
            if (model != null && sDisplayOptions.bDisplaySolidModel)
            {
                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplayMembers) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel,sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial, null, null, null, egcs);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);

                Model3DGroup othersModel3DGroup = null;
                bool displayOtherObjects3D = true; // Temporary TODO
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);

                Drawing3D.AddLightsToModel3D(gr, sDisplayOptions);
            }
            return gr;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Members Model3D
        public static Model3DGroup CreateMembersModel3D(CModel model,
            bool bFastRendering = true,
            bool bTranspartentModel = false,
            bool bUseDiffuseMaterial = true,
            bool bUseEmissiveMaterial = true,
            SolidColorBrush front = null,
            SolidColorBrush shell = null,
            SolidColorBrush back = null,
            EGCS egcs = EGCS.eGCSLeftHanded)
        {
            if (front == null) front = new SolidColorBrush(Colors.Red); // Material color - Front Side
            if (back == null) back = new SolidColorBrush(Colors.Red); // Material color - Back Side
            if (shell == null) shell = new SolidColorBrush(Colors.SlateBlue); // Material color - Shell

            if (bTranspartentModel)
            {
                front.Opacity = back.Opacity = 0.6;
                shell.Opacity = 0.2;
            }
            else front.Opacity = shell.Opacity = back.Opacity = 0.8f;

            Model3DGroup model3D = null;
            if (model.m_arrMembers != null) // Some members exist
            {
                // Model Group of Members
                // Prepare member model
                for (int i = 0; i < model.m_arrMembers.Length; i++) // !!! Import z xls - BUG pocet prvkov sa nacitava z xls aj z prazdnych riadkov pokial su nejako formatovane / nie default
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to display
                    {
                        if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null) // CCrSc is abstract without geometrical properties (dimensions), only centroid line could be displayed
                        {
                            bool bUseCrossSectionColor = true;

                            if (bUseCrossSectionColor && model.m_arrMembers[i].CrScStart.CSColor != null)
                            {
                                // Set color of shell
                                shell = new SolidColorBrush(model.m_arrMembers[i].CrScStart.CSColor);
                            }

                            if (bFastRendering ||
                                    (model.m_arrMembers[i].CrScStart.TriangleIndicesFrontSide == null ||
                                     model.m_arrMembers[i].CrScStart.TriangleIndicesShell == null ||
                                     model.m_arrMembers[i].CrScStart.TriangleIndicesBackSide == null)) // Check if are particular surfaces defined
                            {
                                // Create Member model - one geometry model
                                if (model3D == null) model3D = new Model3DGroup();
                                GeometryModel3D geom3D = model.m_arrMembers[i].getG_M_3D_Member(egcs, shell, bUseDiffuseMaterial, bUseEmissiveMaterial);
                                model3D.Children.Add(geom3D); // Use shell color for whole member
                            }
                            else
                            {
                                // Create Member model - consist of 3 geometry models (member is one model group)
                                if (model3D == null) model3D = new Model3DGroup();
                                Model3DGroup mgr = model.m_arrMembers[i].getM_3D_G_Member(egcs, front, shell, back, bUseDiffuseMaterial, bUseEmissiveMaterial);
                                model3D.Children.Add(mgr);
                            }
                        }
                        else
                        {
                            // Display axis line, member is not valid to display in 3D
                        }
                    }
                }
            }
            return model3D;
        }

        private static List<Point3D> GetWireFramePointsFromGeometryPositions(Point3DCollection positions)
        {
            List<Point3D> wireframePoints = new List<Point3D>();
            for (int i = 0; i < positions.Count - 1; i++)
            {
                wireframePoints.Add(positions[i]);
                wireframePoints.Add(positions[i + 1]);
            }
            return wireframePoints;
        }
        private static List<Point3D> GetWireFramePointsFromGeometryShellPositions(Point3DCollection positions)
        {
            List<Point3D> wireframePoints = new List<Point3D>();
            int halfIndex = positions.Count / 2;
            for (int i = 0; i < halfIndex; i++)
            {
                wireframePoints.Add(positions[i]);
                wireframePoints.Add(positions[i + halfIndex]);
            }
            return wireframePoints;
        }
        private static List<Point3D> GetWireFramePointsFromMemberGeometryPositions(Point3DCollection positions)
        {
            List<Point3D> wireframePoints = new List<Point3D>();
            int shift = positions.Count / 4;

            for (int i = 0; i < positions.Count - 1; i++)
            {
                if (i < positions.Count / 4)  // Front side
                {
                    wireframePoints.Add(positions[i]);
                    wireframePoints.Add(positions[i + 1]);
                }
                else if (i >= positions.Count * 3 / 4) // Back side
                {
                    wireframePoints.Add(positions[i]);
                    wireframePoints.Add(positions[i + 1]);
                }
                else // Shell - tu ma byt shell??
                {
                    wireframePoints.Add(positions[i]);
                    wireframePoints.Add(positions[i + shift]);
                }
            }
            return wireframePoints;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Connection joints model 3d group
        public static Model3DGroup CreateConnectionJointsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions, SolidColorBrush brushPlates = null, SolidColorBrush brushConnectors = null, SolidColorBrush brushWelds = null)
        {
            if (brushPlates == null) brushPlates = new SolidColorBrush(Colors.Gray);
            if (brushConnectors == null) brushConnectors = new SolidColorBrush(Colors.Red);
            if (brushWelds == null) brushWelds = new SolidColorBrush(Colors.Orange);

            Model3DGroup JointsModel3DGroup = null;

            if (cmodel.m_arrConnectionJoints != null && sDisplayOptions.bDisplayJoints) // Some joints exist
            {
                for (int i = 0; i < cmodel.m_arrConnectionJoints.Count; i++)
                {
                    // Set different colors of plates in joints defined in LCS of member and GCS of model
                    if (cmodel.m_arrConnectionJoints[i].bIsJointDefinedinGCS)
                        brushPlates = new SolidColorBrush(Colors.DeepSkyBlue);

                    // Models3D or ModelGroups Components
                    Model3DGroup JointModelGroup = new Model3DGroup();

                    // Plates
                    if (cmodel.m_arrConnectionJoints[i].m_arrPlates != null)
                    {
                        for (int l = 0; l < cmodel.m_arrConnectionJoints[i].m_arrPlates.Length; l++)
                        {
                            if (cmodel.m_arrConnectionJoints[i].m_arrPlates[l] != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_pControlPoint != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrPlates[l].BIsDisplayed == true) // Plate object is valid (not empty) and should be displayed
                            {
                                GeometryModel3D plateGeom = cmodel.m_arrConnectionJoints[i].m_arrPlates[l].CreateGeomModel3D(brushPlates);
                                cmodel.m_arrConnectionJoints[i].m_arrPlates[l].Visual_Plate = plateGeom;

                                if (sDisplayOptions.bDisplayPlates)
                                {
                                    // Add plates
                                    JointModelGroup.Children.Add(plateGeom); // Add plate 3D model to the model group
                                }

                                // Add plate connectors
                                if (cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws != null &&
                                    cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws.Length > 0)
                                {
                                    Model3DGroup plateConnectorsModelGroup = new Model3DGroup();
                                    for (int m = 0; m < cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws.Length; m++)
                                    {
                                        GeometryModel3D plateConnectorgeom = cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws[m].CreateGeomModel3D(brushConnectors);
                                        cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws[m].Visual_Connector = plateConnectorgeom;
                                        plateConnectorsModelGroup.Children.Add(plateConnectorgeom);
                                    }
                                    plateConnectorsModelGroup.Transform = plateGeom.Transform;
                                    if (sDisplayOptions.bDisplayConnectors)
                                    {
                                        JointModelGroup.Children.Add(plateConnectorsModelGroup);
                                    }
                                }
                            }
                        }
                    }

                    // Set plates color to default
                    brushPlates = new SolidColorBrush(Colors.Gray);

                    // Connectors
                    bool bUseAdditionalConnectors = false; // Spojovacie prvky mimo tychto ktore su viazane na plechy (plates) napr spoj priamo medzi nosnikmi bez plechu

                    if (bUseAdditionalConnectors && cmodel.m_arrConnectionJoints[i].m_arrConnectors != null)
                    {
                        for (int l = 0; l < cmodel.m_arrConnectionJoints[i].m_arrConnectors.Length; l++)
                        {
                            if (cmodel.m_arrConnectionJoints[i].m_arrConnectors[l] != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrConnectors[l].m_pControlPoint != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrConnectors[l].BIsDisplayed == true) // Bolt object is valid (not empty) and should be displayed
                            {
                                JointModelGroup.Children.Add(cmodel.m_arrConnectionJoints[i].m_arrConnectors[l].CreateGeomModel3D(brushConnectors)); // Add bolt 3D model to the model group
                            }
                        }
                    }

                    // Welds
                    if (cmodel.m_arrConnectionJoints[i].m_arrWelds != null)
                    {
                        for (int l = 0; l < cmodel.m_arrConnectionJoints[i].m_arrWelds.Length; l++)
                        {
                            if (cmodel.m_arrConnectionJoints[i].m_arrWelds[l] != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrWelds[l].m_pControlPoint != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrWelds[l].BIsDisplayed == true) // Weld object is valid (not empty) and should be displayed
                            {
                                JointModelGroup.Children.Add(cmodel.m_arrConnectionJoints[i].m_arrWelds[l].CreateGeomModel3D(brushWelds)); // Add weld 3D model to the model group
                            }
                        }
                    }

                    if (!cmodel.m_arrConnectionJoints[i].bIsJointDefinedinGCS)
                    {
                        // Joint is defined in LCS of first secondary member
                        if (cmodel.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                        cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null &&
                        !MathF.d_equal(cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x, 0))
                        {
                            AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x / MathF.fPI * 180);
                            RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x);
                            JointModelGroup.Transform = rotate;
                        }
                        else if (cmodel.m_arrConnectionJoints[i].m_MainMember != null && !MathF.d_equal(cmodel.m_arrConnectionJoints[i].m_MainMember.DTheta_x, 0)) // Joint is defined in LCS of main member and rotation degree is not zero
                        {
                            AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), cmodel.m_arrConnectionJoints[i].m_MainMember.DTheta_x / MathF.fPI * 180);
                            RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x);
                            JointModelGroup.Transform = rotate;
                        }
                        else
                        {
                            // There is no rotation
                        }

                        // Joint is defined in LCS of first secondary member
                        if (cmodel.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                        cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null)
                        {
                            cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS_ChangeOriginal(JointModelGroup, cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0]);
                        }
                        else // Joint is defined in LCS of main member
                        {
                            cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS_ChangeOriginal(JointModelGroup, cmodel.m_arrConnectionJoints[i].m_MainMember);
                        }
                    }
                    cmodel.m_arrConnectionJoints[i].Visual_ConnectionJoint = JointModelGroup;

                    // Add joint model group to the global model group items
                    if (JointsModel3DGroup == null) JointsModel3DGroup = new Model3DGroup();
                    JointsModel3DGroup.Children.Add(JointModelGroup);
                } //for joints
            }
            return JointsModel3DGroup;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Loading objects model 3d group
        public static Model3DGroup CreateModelLoadObjectsModel3DGroup(CLoadCase selectedLoadCase)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (selectedLoadCase != null)
            {
                if (selectedLoadCase.NodeLoadsList != null) // Some nodal loads exist
                {
                    // Model Groups of Nodal Loads
                    for (int i = 0; i < selectedLoadCase.NodeLoadsList.Count; i++)
                    {
                        if (selectedLoadCase.NodeLoadsList[i] != null && selectedLoadCase.NodeLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            model3D_group.Children.Add(selectedLoadCase.NodeLoadsList[i].CreateM_3D_G_Load()); // Add to the model group

                            // Set load for all assigned nodes

                        }
                    }
                }

                if (selectedLoadCase.MemberLoadsList != null) // Some member loads exist
                {
                    // Model Groups of Member Loads
                    for (int i = 0; i < selectedLoadCase.MemberLoadsList.Count; i++)
                    {
                        if (selectedLoadCase.MemberLoadsList[i] != null && selectedLoadCase.MemberLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            Model3DGroup model_gr = new Model3DGroup();
                            model_gr = selectedLoadCase.MemberLoadsList[i].CreateM_3D_G_Load();
                            // Transform modelgroup from LCS to GCS
                            model_gr = selectedLoadCase.MemberLoadsList[i].Transform3D_OnMemberEntity_fromLCStoGCS(model_gr, selectedLoadCase.MemberLoadsList[i].Member);

                            model3D_group.Children.Add(model_gr); // Add member load to the model group

                            // Set load for all assigned member

                        }
                    }
                }

                if (selectedLoadCase.SurfaceLoadsList != null) // Some surface loads exist
                {
                    // Model Groups of Surface Loads
                    for (int i = 0; i < selectedLoadCase.SurfaceLoadsList.Count; i++)
                    {
                        if (selectedLoadCase.SurfaceLoadsList[i] != null && selectedLoadCase.SurfaceLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            Model3DGroup model_gr = new Model3DGroup();
                            model_gr = selectedLoadCase.SurfaceLoadsList[i].CreateM_3D_G_Load();

                            model3D_group.Children.Add(model_gr); // Add surface load to the model group

                            // Set load for all assigned surfaces

                        }
                    }
                }
            }
            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Other model objects model 3d group
        public static Model3DGroup CreateModelOtherObjectsModel3DGroup(CModel cmodel)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (cmodel.m_arrGOAreas != null) // Some areas exist
            {
                // Model Groups of Areas



            }

            if (cmodel.m_arrGOVolumes != null) // Some volumes exist
            {
                // Model Groups of Volumes
                for (int i = 0; i < cmodel.m_arrGOVolumes.Length; i++)
                {
                    if (cmodel.m_arrGOVolumes[i] != null &&
                        cmodel.m_arrGOVolumes[i].m_pControlPoint != null &&
                        cmodel.m_arrGOVolumes[i].BIsDisplayed == true) // Volume object is valid (not empty) and should be displayed
                    {
                        // Get shape - prism , sphere, ...

                        switch (cmodel.m_arrGOVolumes[i].m_eShapeType)
                        {
                            case EVolumeShapeType.eShape3DCube:
                            case EVolumeShapeType.eShape3DPrism_8Edges:
                                model3D_group.Children.Add(cmodel.m_arrGOVolumes[i].CreateM_3D_G_Volume_8Edges()); // Add solid to model group
                                break;
                            case EVolumeShapeType.eShape3D_Cylinder:
                                model3D_group.Children.Add(cmodel.m_arrGOVolumes[i].CreateM_G_M_3D_Volume_Cylinder()); // Add solid to model group
                                break;
                            default:
                                //TODO - prepracovat a dopracovat
                                break;
                        }
                    }
                }
            }

            if (cmodel.m_arrGOStrWindows != null) // Some windows exist
            {
                // Model Groups of Windows
                for (int i = 0; i < cmodel.m_arrGOStrWindows.Length; i++)
                {
                    if (cmodel.m_arrGOStrWindows[i] != null &&
                        cmodel.m_arrGOStrWindows[i].m_pControlPoint != null &&
                        cmodel.m_arrGOStrWindows[i].BIsDisplayed == true) // Volume object is valid (not empty) and should be displayed
                    {
                        if (cmodel.m_arrGOStrWindows[i].EShapeType == EWindowShapeType.eClassic)
                            model3D_group.Children.Add(cmodel.m_arrGOStrWindows[i].CreateM_3D_G_Window()); // Add solid to model group
                        else
                        {
                            //Exception - not implemented
                        }
                    }
                }
            }

            if (cmodel.m_arrNSupports != null) // Some nodal supports exist
            {
                // Model Groups of Nodal Suports
                for (int i = 0; i < cmodel.m_arrNSupports.Length; i++)
                {
                    if (cmodel.m_arrNSupports[i] != null && cmodel.m_arrNSupports[i].BIsDisplayed == true) // Support object is valid (not empty) and should be displayed
                    {
                        model3D_group.Children.Add(cmodel.m_arrNSupports[i].CreateM_3D_G_NSupport()); // Add solid to model group

                        // Set support for all assigned nodes
                    }
                }
            }

            if (cmodel.m_arrNReleases != null) // Some member release exist
            {
                // Model Groups of Member Releases
                for (int i = 0; i < cmodel.m_arrNReleases.Length; i++)
                {
                    if (cmodel.m_arrNReleases[i] != null && cmodel.m_arrNReleases[i].BIsDisplayed == true) // Support object is valid (not empty) and should be displayed
                    {
                        /*
                        for (int j = 0; j < cmodel.m_arrNReleases[i].m_iMembCollection.Length; j++) // Set release for all assigned members (member nodes)
                        {
                            Model3DGroup model_gr = new Model3DGroup();
                            model_gr = cmodel.m_arrNReleases[i].CreateM_3D_G_MNRelease();
                            // Transform modelgroup from LCS to GCS
                            model_gr = cmodel.m_arrNReleases[i].Transform3D_OnMemberEntity_fromLCStoGCS(model_gr, cmodel.m_arrMembers[cmodel.m_arrNReleases[i].m_iMembCollection[j]]);

                            gr.Children.Add(model_gr); // Add Release to model group
                        }*/

                        Model3DGroup model_gr = new Model3DGroup();
                        model_gr = cmodel.m_arrNReleases[i].CreateM_3D_G_MNRelease();
                        // Transform modelgroup from LCS to GCS
                        model_gr = cmodel.m_arrNReleases[i].Transform3D_OnMemberEntity_fromLCStoGCS(model_gr, cmodel.m_arrNReleases[i].Member);

                        model3D_group.Children.Add(model_gr); // Add Release to model group
                    }
                }
            }

            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        // Draw GCS Axis
        public static void DrawGlobalAxis(Viewport3D viewPort, CModel model)
        {
            float flineThickness = 3;
            // Global coordinate system - axis
            ScreenSpaceLines3D sAxisX_3D = new ScreenSpaceLines3D();
            ScreenSpaceLines3D sAxisY_3D = new ScreenSpaceLines3D();
            ScreenSpaceLines3D sAxisZ_3D = new ScreenSpaceLines3D();
            Point3D pGCS_centre = new Point3D(0, 0, 0);
            Point3D pAxisX = new Point3D(1, 0, 0);
            Point3D pAxisY = new Point3D(0, 1, 0);
            Point3D pAxisZ = new Point3D(0, 0, 1);
                        
            sAxisX_3D.Points.Add(pGCS_centre);
            sAxisX_3D.Points.Add(pAxisX);
            sAxisX_3D.Color = Colors.Red;
            sAxisX_3D.Thickness = flineThickness;
            sAxisX_3D.Name = "AxisX";

            sAxisY_3D.Points.Add(pGCS_centre);
            sAxisY_3D.Points.Add(pAxisY);
            sAxisY_3D.Color = Colors.Green;
            sAxisY_3D.Thickness = flineThickness;
            sAxisY_3D.Name = "AxisY";

            sAxisZ_3D.Points.Add(pGCS_centre);
            sAxisZ_3D.Points.Add(pAxisZ);
            sAxisZ_3D.Color = Colors.Blue;
            sAxisZ_3D.Thickness = flineThickness;
            sAxisZ_3D.Name = "AxisZ";

            if (model != null)
            {
                model.AxisX = sAxisX_3D;
                model.AxisY = sAxisY_3D;
                model.AxisZ = sAxisZ_3D;
            }
            
            //temp skuska
            WireLine wX = new WireLine();
            wX.Point1 = pGCS_centre;
            wX.Point2 = pAxisX;
            wX.Thickness = flineThickness;
            wX.Color = Colors.Red;

            WireLine wY = new WireLine();
            wY.Point1 = pGCS_centre;
            wY.Point2 = pAxisY;
            wY.Thickness = flineThickness;
            wY.Color = Colors.Green;

            WireLine wZ = new WireLine();
            wZ.Point1 = pGCS_centre;
            wZ.Point2 = pAxisZ;
            wZ.Thickness = flineThickness;
            wZ.Color = Colors.Blue;

            viewPort.Children.Add(wX);
            viewPort.Children.Add(wY);
            viewPort.Children.Add(wZ);
            //end temp skuska

            //viewPort.Children.Add(sAxisX_3D);
            //viewPort.Children.Add(sAxisY_3D);
            //viewPort.Children.Add(sAxisZ_3D);
        }

        // Draw Members Centerlines
        public static void DrawModelMembersCenterLines(CModel model, Viewport3D viewPort)
        {
            ScreenSpaceLines3D lines = new ScreenSpaceLines3D();

            // Members
            if (model.m_arrMembers != null)
            {
                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                    {
                        Point3D pNodeStart = new Point3D(model.m_arrMembers[i].NodeStart.X, model.m_arrMembers[i].NodeStart.Y, model.m_arrMembers[i].NodeStart.Z);
                        Point3D pNodeEnd = new Point3D(model.m_arrMembers[i].NodeEnd.X, model.m_arrMembers[i].NodeEnd.Y, model.m_arrMembers[i].NodeEnd.Z);

                        // Create centerline of member
                        lines.Points.Add(pNodeStart); // Add Start Node
                        lines.Points.Add(pNodeEnd); // Add End Node
                    }
                }

                viewPort.Children.Add(lines);
            }
        }

        // TODO Ondrej - z tychto troch metod staci asi ponechat uz len jednu ??? pripadne popisat naco sa ktora este moze hodit aby to bolo hned zrejme
        // Draw Members Wire Frame
        public static void DrawModelMembersWireFrame_temp(CModel model, Viewport3D viewPort)
        {
            // Members - Wire Frame
            if (model.m_arrMembers != null)
            {
                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                    {
                        // Create WireFrame in LCS
                        ScreenSpaceLines3D wireFrame_FrontSide = model.m_arrMembers[i].CreateWireFrame(-model.m_arrMembers[i].FAlignment_Start);
                        ScreenSpaceLines3D wireFrame_BackSide = model.m_arrMembers[i].CreateWireFrame(model.m_arrMembers[i].FLength + model.m_arrMembers[i].FAlignment_End);
                        ScreenSpaceLines3D wireFrame_Lateral = model.m_arrMembers[i].CreateWireFrameLateral();

                        // Add Wireframe Lines to the trackport
                        viewPort.Children.Add(wireFrame_FrontSide);
                        viewPort.Children.Add(wireFrame_BackSide);
                        viewPort.Children.Add(wireFrame_Lateral);
                    }
                }
            }
        }
        // Draw Members Wire Frame - test for better performance
        public static void DrawModelMembersWireFrame_test(CModel model, Viewport3D viewPort)
        {
            // Members - Wire Frame
            if (model.m_arrMembers != null)
            {
                Color wireFrameColor = Color.FromRgb(60, 60, 60);
                double thickness = 1.0;
                ScreenSpaceLines3D wireFrame_FrontSide = new ScreenSpaceLines3D(wireFrameColor, thickness);
                ScreenSpaceLines3D wireFrame_BackSide = new ScreenSpaceLines3D(wireFrameColor, thickness);
                ScreenSpaceLines3D wireFrame_Lateral = new ScreenSpaceLines3D(wireFrameColor, thickness);

                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                    {
                        // Create WireFrame in LCS
                        wireFrame_FrontSide.AddPoints(model.m_arrMembers[i].CreateWireFrame(-model.m_arrMembers[i].FAlignment_Start).Points);
                        wireFrame_BackSide.AddPoints(model.m_arrMembers[i].CreateWireFrame(model.m_arrMembers[i].FLength + model.m_arrMembers[i].FAlignment_End).Points);
                        wireFrame_Lateral.AddPoints(model.m_arrMembers[i].CreateWireFrameLateral().Points);
                    }
                }
                // Add Wireframe Lines to the trackport
                viewPort.Children.Add(wireFrame_FrontSide);
                viewPort.Children.Add(wireFrame_BackSide);
                viewPort.Children.Add(wireFrame_Lateral);
            }
        }
        // Add all members in one wireframe collection of ScreenSpaceLines3D
        public static void DrawModelMembersWireFrame(CModel model, Viewport3D viewPort)
        {
            // Members - Wire Frame
            if (model.m_arrMembers != null)
            {                
                List<Point3D> wireFramePoints = new List<Point3D>();
                for (int i = 0; i < model.m_arrMembers.Length; i++) // Per each member
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                    {
                        wireFramePoints.AddRange(model.m_arrMembers[i].WireFramePoints);
                    }
                }

                WireLines wl = new WireLines();
                wl.Lines = new Point3DCollection(wireFramePoints);
                wl.Color = Colors.White;
                viewPort.Children.Add(wl);

                //ScreenSpaceLines are much slower = performance issue
                //Color wireFrameColor = Color.FromRgb(60, 60, 60);
                //double thickness = 1.0;
                //ScreenSpaceLines3D wireFrameAllMembers = new ScreenSpaceLines3D(wireFrameColor, thickness); // Just one collection for all members
                //wireFrameAllMembers.Points = new Point3DCollection(wireFramePoints);
                //viewPort.Children.Add(wireFrameAllMembers);
            }
        }

        // Draw Model Connection Joints Wire Frame
        public static void DrawModelConnectionJointsWireFrame(CModel model, Viewport3D viewPort, bool drawConnectors = true)
        {
            //Wireframe Points of all joints
            List<Point3D> jointsWireFramePoints = new List<Point3D>();

            if (model.m_arrConnectionJoints != null)
            {
                for (int i = 0; i < model.m_arrConnectionJoints.Count; i++)
                {
                    if (model.m_arrConnectionJoints[i] != null) // Joint object is valid (not empty)
                    {
                        // Wireframe Points of one joint (all components in joint)
                        List<Point3D> jointPoints = new List<Point3D>();

                        // Plates
                        if (model.m_arrConnectionJoints[i].m_arrPlates != null)
                        {
                            for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++)
                            {
                                // Create WireFrame in LCS
                                List<Point3D> jointPlatePoints = model.m_arrConnectionJoints[i].m_arrPlates[j].CreateWireFrameModel().Points.ToList();

                                if (drawConnectors)
                                {
                                    // Add plate connectors
                                    if (model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws != null &&
                                        model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws.Length > 0)
                                    {
                                        for (int m = 0; m < model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws.Length; m++)
                                        {
                                            GeometryModel3D geom = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[m].Visual_Connector;
                                            Point3DCollection pointsConnector = model.m_arrConnectionJoints[i].m_arrPlates[j].ScrewArrangement.Screws[m].WireFrameModelPointsFromVisual();
                                            var transPoints_PlateConnector = pointsConnector.Select(p => geom.Transform.Transform(p));
                                            jointPlatePoints.AddRange(transPoints_PlateConnector);
                                        }
                                    }
                                }
                                var transPoints_Plate = jointPlatePoints.Select(p => model.m_arrConnectionJoints[i].m_arrPlates[j].Visual_Plate.Transform.Transform(p));
                                jointPoints.AddRange(transPoints_Plate);
                            }
                        }

                        // Connectors
                        bool bUseAdditionalConnectors = false; // Spojovacie prvky mimo tychto ktore su viazane na plechy (plates) napr spoj priamo medzi nosnikmi bez plechu

                        if (bUseAdditionalConnectors && model.m_arrConnectionJoints[i].m_arrConnectors != null)
                        {
                            for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrConnectors.Length; j++)
                            {
                                // Create WireFrame in LCS
                                ScreenSpaceLines3D wireFrame = model.m_arrConnectionJoints[i].m_arrConnectors[j].CreateWireFrameModel();

                                // Rotate from LCS to GCS
                                // TODO
                                jointPoints.AddRange(wireFrame.Points);
                            }
                        }

                        // Welds
                        if (model.m_arrConnectionJoints[i].m_arrWelds != null)
                        {
                            for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrWelds.Length; j++)
                            {
                                // Create WireFrame in LCS
                                ScreenSpaceLines3D wireFrame = model.m_arrConnectionJoints[i].m_arrWelds[j].CreateWireFrameModel();

                                // Rotate from LCS to GCS
                                // TODO
                                jointPoints.AddRange(wireFrame.Points);
                            }
                        }
                        var transPoints = jointPoints.Select(p => model.m_arrConnectionJoints[i].Visual_ConnectionJoint.Transform.Transform(p));
                        jointsWireFramePoints.AddRange(transPoints);
                    }
                }

                WireLines wl = new WireLines();
                wl.Lines = new Point3DCollection(jointsWireFramePoints);
                wl.Color = Colors.White;
                viewPort.Children.Add(wl);
            }
        }
        public static GeometryModel3D GetGeometryModel3DFrom(Model3DGroup model3DGroup)
        {
            GeometryModel3D gm = null;
            foreach (Model3D m in model3DGroup.Children)
            {
                if (m is Model3DGroup) gm = GetGeometryModel3DFrom(m as Model3DGroup);
                else if (m is GeometryModel3D) gm = m as GeometryModel3D;
            }
            return gm;
        }
        // Draw Members Wire Frame
        public static void DrawMemberWireFrame(CMember member, Viewport3D viewPort, float memberLength)
        {
            ScreenSpaceLines3D wireFrame_FrontSide = member.CreateWireFrame(0f);
            ScreenSpaceLines3D wireFrame_BackSide = member.CreateWireFrame(memberLength);
            ScreenSpaceLines3D wireFrame_Lateral = member.CreateWireFrameLateral();

            viewPort.Children.Add(wireFrame_FrontSide);
            viewPort.Children.Add(wireFrame_BackSide);
            viewPort.Children.Add(wireFrame_Lateral);
        }

        //  Lights
        public static void AddLightsToModel3D(Model3DGroup gr, DisplayOptions sDisplayOptions)
        {
            /*
            The following lights derive from the base class Light:
            AmbientLight : Provides ambient lighting that illuminates all objects uniformly regardless of their location or orientation.
            DirectionalLight : Illuminates like a distant light source. Directional lights have a Direction specified as a Vector3D, but no specified location.
            PointLight : Illuminates like a nearby light source. PointLights have a position and cast light from that position. Objects in the scene are illuminated depending on their position and distance with respect to the light. PointLightBase exposes a Range property, which determines a distance beyond which models will not be illuminated by the light. PointLight also exposes attenuation properties which determine how the light's intensity diminishes over distance. You can specify constant, linear, or quadratic interpolations for the light's attenuation.
            SpotLight : Inherits from PointLight. Spotlights illuminate like PointLight and have both position and direction. They project light in a cone-shaped area set by InnerConeAngle and OuterConeAngle properties, specified in degrees.
            */

            if (sDisplayOptions.bUseLightDirectional)
            {
                DirectionalLight Dir_Light = new DirectionalLight();
                Dir_Light.Color = Colors.White;
                Dir_Light.Direction = new Vector3D(0, 0, -1);
                gr.Children.Add(Dir_Light);
            }

            if (sDisplayOptions.bUseLightPoint)
            {
                PointLight Point_Light = new PointLight();
                Point_Light.Position = new Point3D(0, 0, 30);
                Point_Light.Color = Brushes.White.Color;
                Point_Light.Range = 30.0;
                Point_Light.ConstantAttenuation = 0;
                Point_Light.LinearAttenuation = 0;
                Point_Light.QuadraticAttenuation = 0.2f;
                Point_Light.ConstantAttenuation = 5.0;
                gr.Children.Add(Point_Light);
            }

            if (sDisplayOptions.bUseLightSpot)
            {
                SpotLight Spot_Light = new SpotLight();
                Spot_Light.InnerConeAngle = 30;
                Spot_Light.OuterConeAngle = 30;
                Spot_Light.Color = Brushes.White.Color;
                Spot_Light.Direction = new Vector3D(0, 0, -1);
                Spot_Light.Position = new Point3D(8.5, 8.5, 20);
                Spot_Light.Range = 30;
                gr.Children.Add(Spot_Light);
            }

            if (sDisplayOptions.bUseLightAmbient)
            {
                AmbientLight Ambient_Light = new AmbientLight();
                Ambient_Light.Color = Colors.Gray;
                gr.Children.Add(new AmbientLight());
            }
        }
        // Draw Text in 3D
        public static void CreateMembersDescriptionModel3D(CModel model, Viewport3D viewPort)
        {
            // Members
            if (model.m_arrMembers != null)
            {
                ModelVisual3D textlabel = null;

                for (int i = 0; i < model.m_arrMembers.Length; i++)
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                    {
                        Point3D pNodeStart = new Point3D(model.m_arrMembers[i].NodeStart.X, model.m_arrMembers[i].NodeStart.Y, model.m_arrMembers[i].NodeStart.Z);
                        Point3D pNodeEnd = new Point3D(model.m_arrMembers[i].NodeEnd.X, model.m_arrMembers[i].NodeEnd.Y, model.m_arrMembers[i].NodeEnd.Z);

                        // TODO - Ondrej - vytvorit v GUI dialog options kde si uzivatel moze vybrat co chce na prute zobrazit a z toho vyskladat tento text;
                        string sTextToDisplay =
                            model.m_arrMembers[i].ID.ToString() + " - " +   // Number of member
                            model.m_arrMembers[i].Prefix + " - " +          // Member prefix
                            model.m_arrMembers[i].CrScStart.Name + " - " +  // Member cross-section (start)
                            Math.Round(model.m_arrMembers[i].FLength_real,3).ToString() + "m"; // Member real length

                        TextBlock tb = new TextBlock();
                        tb.Text = sTextToDisplay;
                        tb.FontFamily = new FontFamily("Arial");
                        float fTextBlockVerticalSize = 0.1f;
                        float fTextBlockVerticalSizeFactor = 0.8f;
                        float fTextBlockHorizontalSizeFactor = 0.3f;

                        // Tieto nastavenia sa nepouziju
                        tb.FontStretch = FontStretches.UltraCondensed;
                        tb.FontStyle = FontStyles.Normal;
                        tb.FontWeight = FontWeights.Thin;
                        tb.Foreground = Brushes.Coral;
                        tb.Background = Brushes.Black; // TODO - In case that solid model is displayed it is reasonable to use black backround of text or offset texts usig cross-section dimension

                        float fRelativePositionFactor = 0.4f; //(0-1) // Relative position of member description on member

                        // TODO Ondrej - vylepsit vykreslovanie a odsadenie
                        // Teraz to kreslime priamo do GCS, ale asi by bolo lepsie kreslit do LCS a potom text transformovat
                        // pripadne vypocitat podla orientacie pruta vector z hodnot delta ako je prut orientovany v priestore a podla toho nastavit
                        // hodnoty vektorov pre funkciu CreateTextLabel3D) :over" and "up"
                        // Do user options by som dal nastavenie ci sa ma text kreslit horizontalne na obrazovke v rovine obrazovky
                        // alebo podla polohy pruta (rovnobezne s lokalnou osou x pruta) horizontalne alebo vertikalne podla orientacie osi x pruta v lokanych rovinach x,y alebo x,z pruta

                        float fOffsetZ = 0.07f;
                        Point3D pTextPosition = new Point3D();
                        pTextPosition.X = pNodeStart.X + fRelativePositionFactor * model.m_arrMembers[i].Delta_X;
                        pTextPosition.Y = pNodeStart.Y + fRelativePositionFactor * model.m_arrMembers[i].Delta_Y;
                        pTextPosition.Z = pNodeStart.Z + fRelativePositionFactor * model.m_arrMembers[i].Delta_Z + fOffsetZ;

                        // Create text
                        textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTextPosition, new Vector3D(fTextBlockHorizontalSizeFactor, 0,0),  new Vector3D (0,0, fTextBlockVerticalSizeFactor));
                        viewPort.Children.Add(textlabel);
                    }
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
            FontFamily font,
            double height,
            Point3D center,
            Vector3D over,
            Vector3D up)
        {
            // First we need a textblock containing the text of our label
            TextBlock tb = new TextBlock(new Run(text));
            tb.Foreground = textColor;
            tb.FontFamily = font;

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
            mv3d.Content = new GeometryModel3D(mg, mat);
            return mv3d;
        }

        public static ModelVisual3D CreateTextLabel3D(
            TextBlock tb,
            bool bDoubleSided,
            double height,
            Point3D center,
            Vector3D over,
            Vector3D up)
        {
            return CreateTextLabel3D(tb.Text, tb.Foreground, bDoubleSided, tb.FontFamily, height, center, over, up);
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public static void CalculateModelLimits(CModel cmodel, out float fMax_X, out float fMin_X, out float fMax_Y, out float fMin_Y, out float fMax_Z, out float fMin_Z)
        {
            fMax_X = float.MinValue;
            fMin_X = float.MaxValue;
            fMax_Y = float.MinValue;
            fMin_Y = float.MaxValue;
            fMax_Z = float.MinValue;
            fMin_Z = float.MaxValue;

            if (cmodel.m_arrNodes != null) // Some nodes exist
            {
                fMax_X = cmodel.m_arrNodes.Max(p => p.X);
                fMin_X = cmodel.m_arrNodes.Min(p => p.X);
                fMax_Y = cmodel.m_arrNodes.Max(p => p.Y);
                fMin_Y = cmodel.m_arrNodes.Min(p => p.Y);
                fMax_Z = cmodel.m_arrNodes.Max(p => p.Z);
                fMin_Z = cmodel.m_arrNodes.Min(p => p.Z);
            }
            else if (cmodel.m_arrGOPoints != null) // Some points exist
            {
                fMax_X = (float)cmodel.m_arrGOPoints.Max(p => p.X);
                fMin_X = (float)cmodel.m_arrGOPoints.Min(p => p.X);
                fMax_Y = (float)cmodel.m_arrGOPoints.Max(p => p.Y);
                fMin_Y = (float)cmodel.m_arrGOPoints.Min(p => p.Y);
                fMax_Z = (float)cmodel.m_arrGOPoints.Max(p => p.Z);
                fMin_Z = (float)cmodel.m_arrGOPoints.Min(p => p.Z);
            }
            else
            {
                // Exception - no definition nodes or points
                throw new Exception("Exception - no definition nodes or points");
            }
        }
        public static void CalculateModelSizes(CModel cmodel, out float fMax_X, out float fMin_X, out float fMax_Y, out float fMin_Y, out float fMax_Z, out float fMin_Z)
        {
            fMax_X = float.MinValue;
            fMin_X = float.MaxValue;
            fMax_Y = float.MinValue;
            fMin_Y = float.MaxValue;
            fMax_Z = float.MinValue;
            fMin_Z = float.MaxValue;

            if (cmodel.m_arrNodes != null) // Some nodes exist
            {
                fMax_X = cmodel.m_arrNodes.Max(p => p.X);
                fMin_X = cmodel.m_arrNodes.Min(p => p.X);
                fMax_Y = cmodel.m_arrNodes.Max(p => p.Y);
                fMin_Y = cmodel.m_arrNodes.Min(p => p.Y);
                fMax_Z = cmodel.m_arrNodes.Max(p => p.Z);
                fMin_Z = cmodel.m_arrNodes.Min(p => p.Z);
            }
            else if (cmodel.m_arrGOPoints != null) // Some points exist
            {
                fMax_X = (float)cmodel.m_arrGOPoints.Max(p => p.X);
                fMin_X = (float)cmodel.m_arrGOPoints.Min(p => p.X);
                fMax_Y = (float)cmodel.m_arrGOPoints.Max(p => p.Y);
                fMin_Y = (float)cmodel.m_arrGOPoints.Min(p => p.Y);
                fMax_Z = (float)cmodel.m_arrGOPoints.Max(p => p.Z);
                fMin_Z = (float)cmodel.m_arrGOPoints.Min(p => p.Z);
            }
            else
            {
                // Exception - no definition nodes or points
                throw new Exception("Exception - no definition nodes or points");
            }
        }

        //    Public Function GetPointOnPlaneZ(p1 As point3d, p2 As point3d, p3 As point3d, x As Single, y As Single)
        //    'call with 3 points p1, p2, p3 that form plane and another point (x,y)
        //    'returns point (x,y) on plane from 3 points
        public static Point3D GetPointOnPlaneZ(Point3D p1, Point3D p2, Point3D p3, double x, double y)
        {
            Vector3D v1 = new Vector3D();
            Vector3D v2 = new Vector3D();
            Point3D abc = new Point3D();
                        
            //'Create 2 vectors by subtracting p3 from p1 and p2
            v1.X = p1.X - p3.X;
            v1.Y = p1.Y - p3.Y;
            v1.Z = p1.Z - p3.Z;

            v2.X = p2.X - p3.X;
            v2.Y = p2.Y - p3.Y;
            v2.Z = p2.Z - p3.Z;

            //  'Create cross product from the 2 vectors
            abc.X = v1.Y * v2.Z - v1.Z * v2.Y;
            abc.Y = v1.Z * v2.X - v1.X * v2.Z;
            abc.Z = v1.X * v2.Y - v1.Y * v2.X;

            //    'find d in the equation aX + bY + cZ = d
            double d = abc.X * p3.X + abc.Y * p3.Y + abc.Z * p3.Z;

            //    'calc z coordinate for point (x,y)
            //    GetPointOnPlaneZ = 
            double z = (d - abc.X * x - abc.Y * y) / abc.Z;
            
            Point3D resultPoint = new Point3D(x, y, z);
            return resultPoint;
        }

        // Distance of Point p from Plane defined by 3 points
        public static double GetDistanceFromPointToPlane(Point3D p1, Point3D p2, Point3D p3, Point3D p)
        {
            Vector3D v1 = new Vector3D();
            Vector3D v2 = new Vector3D();
            v1 = p1 - p3;
            v2 = p2 - p3;
            Vector3D abc = Vector3D.CrossProduct(v1, v2); // Normal vector
            //    'find d in the equation a * X0 + b * Y0 + c * Z0 + d = 0
            // d = − a * X0 − b * Y0 − c * Z0 (point on the plane R [X0, Y0, Z0], we use point p3
            double d = -abc.X * p3.X - abc.Y * p3.Y - abc.Z * p3.Z;
            double dist = (abc.X * p.X + abc.Y * p.Y + abc.Z * p.Z + d) / Math.Sqrt(abc.X * abc.X + abc.Y * abc.Y + abc.Z * abc.Z); // alternative 1

            // https://mathinsight.org/distance_point_plane

            return dist; // Returns negative or positive value ???!!!
        }

        // Get Closest point on plane defined by 3 points(p1,p2,p3) from point p
        public static Point3D GetClosestPointOnPlane(Point3D p1, Point3D p2, Point3D p3, Point3D p)
        {
            Vector3D v1 = new Vector3D();
            Vector3D v2 = new Vector3D();
            v1 = p1 - p3;
            v2 = p2 - p3;
            Vector3D abc = Vector3D.CrossProduct(v1, v2);
            //    'find d in the equation aX + bY + cZ = d
            double d = abc.X * p3.X + abc.Y * p3.Y + abc.Z * p3.Z;

            double dist = (abc.X * p.X + abc.Y * p.Y + abc.Z * p.Z + d) / Math.Sqrt(abc.X * abc.X + abc.Y * abc.Y + abc.Z * abc.Z);

            Point3D closestPoint = p - dist * abc;
            return closestPoint;
        }

        public static bool PointLiesOnPlane(Point3D p1, Point3D p2, Point3D p3, Point3D p, double dLimit = 0.000001)
        {
            double distance = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, p));

            if (distance < dLimit)
                return true;
            else
                return false;
        }

        public static bool NodeLiesOnPlane(Point3D p1, Point3D p2, Point3D p3, CNode n, double dLimit = 0.000001)
        {
            double distance = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, new Point3D(n.X,n.Y,n.Z)));

            if (distance < dLimit)
                return true;
            else
                return false;
        }

        public static bool MemberLiesOnPlane(Point3D p1, Point3D p2, Point3D p3, CMember m, double dLimit = 0.000001)
        {
            double distanceStart = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z)));
            double distanceEnd   = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z)));

            if (distanceStart < dLimit && distanceEnd < dLimit)
                return true;
            else
                return false;
        }
    }
}
