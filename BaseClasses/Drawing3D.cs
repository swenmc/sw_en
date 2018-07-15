﻿using _3DTools;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public static class Drawing3D
    {
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
            float fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y, fTempMax_Z, fTempMin_Z;

            member.CalculateMemberLimits(out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);            

            float fModel_Length_X = fTempMax_X - fTempMin_X;
            float fModel_Length_Y = fTempMax_Y - fTempMin_Y;
            float fModel_Length_Z = fTempMax_Z - fTempMin_Z;

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

        public static Model3DGroup CreateModel3DGroup(CModel model, EGCS egcs = EGCS.eGCSLeftHanded,
            bool displayMembersSurface = true, bool displayConnectionJoints = true, bool displayOtherObjects3D = true, bool addLights = true)
        {
            Model3DGroup gr = new Model3DGroup();
            if (model != null)
            {
                Model3D membersModel3D = null;
                if (displayMembersSurface) membersModel3D = Drawing3D.CreateMembersModel3D(model, null, null, null, false, egcs);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);

                Model3DGroup jointsModel3DGroup = null;
                if (displayConnectionJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);

                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);

                if(addLights) Drawing3D.AddLightsToModel3D(gr);
            }
            return gr;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Members Model3D
        public static Model3DGroup CreateMembersModel3D(CModel model, SolidColorBrush front = null, SolidColorBrush shell = null, SolidColorBrush back = null, bool transpartentModel = false, EGCS egcs = EGCS.eGCSLeftHanded)
        {
            if (front == null) front = new SolidColorBrush(Color.FromRgb(255, 64, 64)); // Material color - Front Side (red)
            if (shell == null) shell = new SolidColorBrush(Color.FromRgb(141, 238, 238)); // Material color - Shell (red)
            if (back == null) back = new SolidColorBrush(Color.FromRgb(238, 154, 73)); // Material color - Back Side (orange)

            if (transpartentModel)
            {
                front.Opacity = back.Opacity = 0.8; 
                shell.Opacity = 0.4;
            }
            else front.Opacity = shell.Opacity = back.Opacity = 0;

            Model3DGroup model3D = null;
            if (model.m_arrMembers != null) // Some members exist
            {
                // Model Group of Members
                // Prepare member model
                for (int i = 0; i < model.m_arrMembers.Length; i++) // !!! BUG pocet prvkov sa nacitava z xls aj z prazdnych riadkov pokial su nejako formatovane / nie default
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null) // Member object is valid (not empty)
                    {
                        if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null) // CCrSc is abstract without geometrical properties (dimensions), only centroid line could be displayed
                        {
                            bool bFastRendering = false;
                            if (bFastRendering || 
                                    (model.m_arrMembers[i].CrScStart.TriangleIndicesFrontSide == null || model.m_arrMembers[i].CrScStart.TriangleIndicesShell == null ||
                                     model.m_arrMembers[i].CrScStart.TriangleIndicesBackSide == null)) // Check if are particular surfaces defined
                            {
                                if (model3D == null) model3D = new Model3DGroup();
                                // Create Member model - one geometry model
                                model3D.Children.Add(model.m_arrMembers[i].getG_M_3D_Member(egcs, front));
                            }
                            else
                            {
                                // Create Member model - consist of 3 geometry models (member is one model group)
                                bool bUseCrossSectionColor = true;
                                if (bUseCrossSectionColor && model.m_arrMembers[i].CrScStart.CSColor != null)
                                    shell = new SolidColorBrush(model.m_arrMembers[i].CrScStart.CSColor);

                                if (model3D == null) model3D = new Model3DGroup();
                                model3D.Children.Add(model.m_arrMembers[i].getM_3D_G_Member(egcs, front, shell, back));
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

        //-------------------------------------------------------------------------------------------------------------
        // Create Connection joints model 3d group
        public static Model3DGroup CreateConnectionJointsModel3DGroup(CModel cmodel, SolidColorBrush brushPlates = null, SolidColorBrush brushConnectors = null, SolidColorBrush brushWelds = null)
        {
            if (brushPlates == null) brushPlates = new SolidColorBrush(Colors.Gray);
            if (brushConnectors == null) brushConnectors = new SolidColorBrush(Colors.Red);
            if (brushWelds == null) brushWelds = new SolidColorBrush(Colors.Orange);

            Model3DGroup JointsModel3DGroup = null;

            bool bDrawConnectors = false;

            if (cmodel.m_arrConnectionJoints != null) // Some joints exist
            {
                for (int i = 0; i < cmodel.m_arrConnectionJoints.Count; i++)
                {
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
                                JointModelGroup.Children.Add(cmodel.m_arrConnectionJoints[i].m_arrPlates[l].CreateGeomModel3D(brushPlates)); // Add plate 3D model to the model group

                                if (bDrawConnectors)
                                {
                                    // Add plate connectors
                                    if (cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors != null &&
                                        cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors.Length > 0)
                                    {
                                        for (int m = 0; m < cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors.Length; m++)
                                            JointModelGroup.Children.Add(cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors[m].CreateGeomModel3D(brushConnectors));
                                    }
                                }
                            }
                        }
                    }

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
                        // TODO prepracovat tento blok a podmienky tak, aby v nebol prazdny else a odstranit duplicitu

                        // Rotate model about local x-axis (LCS - local coordinate system of member)

                        // Joint is defined in LCS of first secondary member
                        if (cmodel.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                        cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null &&
                        !MathF.d_equal(cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x, 0))
                        {
                            AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x / MathF.fPI * 180);
                            RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x);
                            JointModelGroup.Transform = rotate;
                        }
                        else if (!MathF.d_equal(cmodel.m_arrConnectionJoints[i].m_MainMember.DTheta_x, 0)) // Joint is defined in LCS of main member and rotation degree is not zero
                        {
                            AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), cmodel.m_arrConnectionJoints[i].m_MainMember.DTheta_x / MathF.fPI * 180);
                            RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x);
                            JointModelGroup.Transform = rotate;
                        }
                        else
                        {
                            // There is no rotation

                        }

                        // Rotate and translate model in GCS (global coordinate system of whole structure / building)
                        // Create new model group
                        Model3DGroup JointModelGroup_temp = new Model3DGroup();
                        JointModelGroup_temp.Children.Add(JointModelGroup);

                        // Joint is defined in LCS of first secondary member
                        if (cmodel.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                        cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null)
                        {
                            // Transform model group
                            JointModelGroup = cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS(JointModelGroup_temp, cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0]);
                        }
                        else // Joint is defined in LCS of main member
                        {
                            // Transform model group
                            JointModelGroup = cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS(JointModelGroup_temp, cmodel.m_arrConnectionJoints[i].m_MainMember);
                        }
                    }

                    // Add joint model group to the global model group items
                    if (JointsModel3DGroup == null) JointsModel3DGroup = new Model3DGroup();
                    JointsModel3DGroup.Children.Add(JointModelGroup);
                } //for joints
            }
            return JointsModel3DGroup;
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
                        model3D_group.Children.Add(cmodel.m_arrGOVolumes[i].CreateM_3D_G_Volume_8Edges()); // Add solid to model group
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

            if (cmodel.m_arrNLoads != null) // Some nodal loads exist
            {
                // Model Groups of Nodal Loads
                for (int i = 0; i < cmodel.m_arrNLoads.Length; i++)
                {
                    if (cmodel.m_arrNLoads[i] != null && cmodel.m_arrNLoads[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                    {
                        model3D_group.Children.Add(cmodel.m_arrNLoads[i].CreateM_3D_G_Load()); // Add to model group

                        // Set load for all assigned nodes

                    }
                }
            }

            if (cmodel.m_arrMLoads != null) // Some member loads exist
            {
                // Model Groups of Member Loads
                for (int i = 0; i < cmodel.m_arrMLoads.Length; i++)
                {
                    if (cmodel.m_arrMLoads[i] != null && cmodel.m_arrMLoads[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                    {
                        Model3DGroup model_gr = new Model3DGroup();
                        model_gr = cmodel.m_arrMLoads[i].CreateM_3D_G_Load();
                        // Transform modelgroup from LCS to GCS
                        model_gr = cmodel.m_arrMLoads[i].Transform3D_OnMemberEntity_fromLCStoGCS(model_gr, cmodel.m_arrMLoads[i].Member);

                        model3D_group.Children.Add(model_gr); // Add Release to model group

                        // Set load for all assigned member

                    }
                }
            }
            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public static void DrawGlobalAxis(Viewport3D viewPort)
        {
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
            sAxisX_3D.Thickness = 2;

            sAxisY_3D.Points.Add(pGCS_centre);
            sAxisY_3D.Points.Add(pAxisY);
            sAxisY_3D.Color = Colors.Green;
            sAxisY_3D.Thickness = 2;

            sAxisZ_3D.Points.Add(pGCS_centre);
            sAxisZ_3D.Points.Add(pAxisZ);
            sAxisZ_3D.Color = Colors.Blue;
            sAxisZ_3D.Thickness = 2;

            viewPort.Children.Add(sAxisX_3D);
            viewPort.Children.Add(sAxisY_3D);
            viewPort.Children.Add(sAxisZ_3D);
        }
        //-------------------------------------------------------------------------------------------------------------
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
                        model.m_arrMembers[i].CrScStart != null) // Member object is valid (not empty)
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
        public static void DrawModelMembersWireFrame(CModel model, Viewport3D viewPort)
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
                        model.m_arrMembers[i].CrScStart != null) // Member object is valid (not empty)
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
        public static void DrawModelMembersinOneWireFrame(CModel model, Viewport3D viewPort)
        {
            // Members - Wire Frame
            if (model.m_arrMembers != null)
            {
                Color wireFrameColor = Color.FromRgb(60, 60, 60);
                double thickness = 1.0;
                ScreenSpaceLines3D wireFrameAllMembers = new ScreenSpaceLines3D(wireFrameColor, thickness); // Just one collection for all members
                Int32Collection wireFrameMemberPointNo = new Int32Collection();

                for (int i = 0; i < model.m_arrMembers.Length; i++) // Per each member
                {
                    if (model.m_arrMembers[i] != null &&
                        model.m_arrMembers[i].NodeStart != null &&
                        model.m_arrMembers[i].NodeEnd != null &&
                        model.m_arrMembers[i].CrScStart != null) // Member object is valid (not empty)
                    {
                        for (int j = 0; j < 3; j++) // Per front, back side and laterals
                        {
                            if (j == 0) // Front Side
                                wireFrameMemberPointNo = model.m_arrMembers[i].GetMemberWireFrameFrontIndices();
                            else if(j==1) // Laterals
                                wireFrameMemberPointNo = model.m_arrMembers[i].GetMemberWireFrameLateralIndices();
                            else //if (j == 2) // Back Side
                                wireFrameMemberPointNo = model.m_arrMembers[i].GetMemberWireFrameBackIndices();

                            foreach (Int32 no in wireFrameMemberPointNo) // Assign Point3D of surface model to the each number in the wireframe collection 
                            {
                                // TODO Ondrej - performance - Toto bude potrebne odstranit
                                // Mali by sa pouzit data zo surface modelu pruta, takto sa to vytvara 2 krat raz pre surface model a druhy krat pre wireframe
                                // Vyriesit co sa stane, ak budeme chciet zobrazit len samostatny wireframe a surface model teda nebude k dispozicii (vygeneruje sa, ale nepouzije sa pri vykresleni?)

                                Model3DGroup model3D = new Model3DGroup();
                                model3D = model.m_arrMembers[i].getM_3D_G_Member(EGCS.eGCSLeftHanded, Brushes.AliceBlue, Brushes.AliceBlue, Brushes.AliceBlue);

                                // Potrebujeme sa nejako dostat k bodom siete, asi sa to da urobit aj elegantnejsie :-/
                                GeometryModel3D m = new GeometryModel3D();
                                m = (GeometryModel3D)model3D.Children[j];
                                MeshGeometry3D geom = (MeshGeometry3D)m.Geometry;

                                wireFrameAllMembers.Points.Add(geom.Positions[no]); // Add Point3D to the collection
                            }
                        }
                    }
                }

                // Add Wireframe Lines to the trackport
                viewPort.Children.Add(wireFrameAllMembers);
            }
        }
        
        // Draw Model Connection Joints Wire Frame
        public static void DrawModelConnectionJointsWireFrame(CModel model, Viewport3D viewPort)
        {
            if (model.m_arrConnectionJoints != null)
            {
                for (int i = 0; i < model.m_arrConnectionJoints.Count; i++)
                {
                    if (model.m_arrConnectionJoints[i] != null) // Joint object is valid (not empty)
                    {
                        // Joint model wireframe
                        ScreenSpaceLines3D jointWireFrameGroup = new ScreenSpaceLines3D();
                        Transform3DGroup jointTransformGroup = new Transform3DGroup(); // Nepouzite

                        // Plates
                        if (model.m_arrConnectionJoints[i].m_arrPlates != null)
                        {
                            for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++)
                            {
                                // Create WireFrame in LCS
                                ScreenSpaceLines3D wireFrame = model.m_arrConnectionJoints[i].m_arrPlates[j].CreateWireFrameModel();

                                // Rotate from LCS system of plate to LCS system of member or GCS system (depends on joint type definition, in LCS of member or GCS system)
                                model.m_arrConnectionJoints[i].m_arrPlates[j].TransformPlateCoord(wireFrame);

                                // Prva transformacia plechu z jeho prvotneho system x,y do suradnic ako je ulozeny na neootocenom prute v lokalnych suradniciach pruta 
                                //ak je spoj definovany v LCS systeme alebo do globalnych suradnic ak je spoj definovany v GCS

                                Transform3DGroup a = model.m_arrConnectionJoints[i].m_arrPlates[j].CreateTransformCoordGroup();

                                var transformedPoints = wireFrame.Points.Select(p => a.Transform(p)); // TODO - ONDREJ: Toto asi nefunguje lebo suradnice sa neotacaju
                                jointWireFrameGroup.AddPoints(transformedPoints.ToList());

                                // TODO - pridat wireframe pre connectors v plechoch
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
                                jointWireFrameGroup.AddPoints(wireFrame.Points);
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
                                jointWireFrameGroup.AddPoints(wireFrame.Points);
                            }
                        }

                        // Rotate and translate wireframe in case joint is defined in LCS of member

                        if (!model.m_arrConnectionJoints[i].bIsJointDefinedinGCS) // Joint is defined in LCS
                        {
                            // TODO - refaktorovat a zjednotit funkcie pre rotaciu surface modelu a wireframe modelu

                            // TODO / BUG - ONDREJ
                            // Po prvej transformacii wireframe plechov z ich povodneho suradnicoveho systemu, kde (x,y) je v rovine rozvinu plechu a z smeruje smerom z obrazovky)
                            // do LCS pruta uz tato druha transformacia (otocenie vsetkych plechov v spojoch na prute okolo LCS osi x pruta) nefunguje,
                            // pretoze sa aplikuje na prvotne suradnice bodov a nie na aktualne suradnice po prvej transformacii

                            // Ak su lines v kolekcii a otacame jednotlive prvky kolekcie, tak sa vzdy otacaju v ramci svojho povodneho systemu a nie okolo LCS pruta
                            // Je potrebne poskladat postupne vsetky transformacie do jednej skupiny alebo kazdu dalsiu transfromaciu realizovat uz na zmenenych suradniciach points povodneho objektu
                            // Neviem odkial tie aktualne suradnice points zobrat, vidim len uplne povodne.

                            // Rotate model about local x-axis (LCS - local coordinate system of member)
                            if (!model.m_arrConnectionJoints[i].bIsJointDefinedinGCS)
                            {
                                // Druha transformacia - vsetky plechy s dalsie komponenty na prute sa pootocia okolo LCS - os x pruta o uhol theta v pripade ze je nenulovy

                                // TODO prepracovat tento blok a podmienky tak, aby v nebol prazdny else a odstranit duplicitu

                                // Joint is defined in LCS of first secondary member
                                if (model.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                                model.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null &&
                                !MathF.d_equal(model.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x, 0))
                                {
                                    AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), model.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x / MathF.fPI * 180);
                                    RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x); // We will rotate all joint components about member local x-axis

                                    // Rotate all lines in the collection about local x-axis
                                    var transformedPoints = jointWireFrameGroup.Points.Select(p => rotate.Transform(p)); // TODO - ONDREJ: Toto asi nefunguje
                                    List<Point3D> points = transformedPoints.ToList();
                                    jointWireFrameGroup.Points.Clear();
                                    jointWireFrameGroup.AddPoints(points);
                                }
                                else if (!MathF.d_equal(model.m_arrConnectionJoints[i].m_MainMember.DTheta_x, 0)) // Joint is defined in LCS of main member and rotation degree is not zero
                                {
                                    AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), model.m_arrConnectionJoints[i].m_MainMember.DTheta_x / MathF.fPI * 180);
                                    RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x); // We will rotate all joint components about member local x-axis

                                    // Rotate all lines in the collection about local x-axis
                                    var transformedPoints = jointWireFrameGroup.Points.Select(p => rotate.Transform(p)); // TODO - ONDREJ: Toto asi nefunguje
                                    List<Point3D> points = transformedPoints.ToList();
                                    jointWireFrameGroup.Points.Clear();
                                    jointWireFrameGroup.AddPoints(points);
                                }
                                else
                                {
                                    // There is no rotation defined
                                }
                            }

                            // TODO  po oprave rotacie okolo LCS pruta odkomentovat a presunut plechy definovane na prute v LCS do GCS systemu

                            // Tretia transformacia, vsetky plechy na prute sa potoocia do pozicie v GCS a presunu a tak ako by sa presunul prut z jeho LCS [0,0,0] do NodeStart suradnic definovanych v GCS

                            /*
                            // Joint is defined in LCS of first secondary member
                            if (cmodel.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                            cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null)
                            {
                                cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS(ref jointWireFrameGroup, cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0]);
                            }
                            else // Joint is defined in LCS of main member
                            {
                                cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS(ref jointWireFrameGroup, cmodel.m_arrConnectionJoints[i].m_MainMember);
                            }*/
                        }

                        // Add Wireframe Lines to the trackport
                        //_trackport.ViewPort.Children.Clear();
                        viewPort.Children.Add(jointWireFrameGroup);
                    }
                }
            }
        }

        public static void DrawModelConnectionJointsWireFrame(Model3DGroup model3DGroup, CModel model, Viewport3D viewPort)
        {
            ScreenSpaceLines3D jointWireFrameGroup = new ScreenSpaceLines3D();
            if (model.m_arrConnectionJoints != null)
            {
                for (int i = 0; i < model.m_arrConnectionJoints.Count; i++)
                {
                    if (model.m_arrConnectionJoints[i] != null) // Joint object is valid (not empty)
                    {
                        // Joint model wireframe
                        Transform3DGroup jointTransformGroup = new Transform3DGroup(); // Nepouzite

                        // Plates
                        if (model.m_arrConnectionJoints[i].m_arrPlates != null)
                        {
                            for (int j = 0; j < model.m_arrConnectionJoints[i].m_arrPlates.Length; j++)
                            {
                                // Create WireFrame in LCS
                                ScreenSpaceLines3D wireFrame = model.m_arrConnectionJoints[i].m_arrPlates[j].CreateWireFrameModel();

                                // Rotate from LCS system of plate to LCS system of member or GCS system (depends on joint type definition, in LCS of member or GCS system)
                                model.m_arrConnectionJoints[i].m_arrPlates[j].TransformPlateCoord(wireFrame);

                                // Prva transformacia plechu z jeho prvotneho system x,y do suradnic ako je ulozeny na neootocenom prute v lokalnych suradniciach pruta 
                                //ak je spoj definovany v LCS systeme alebo do globalnych suradnic ak je spoj definovany v GCS

                                Transform3DGroup a = model.m_arrConnectionJoints[i].m_arrPlates[j].CreateTransformCoordGroup();

                                var transformedPoints = wireFrame.Points.Select(p => a.Transform(p)); // TODO - ONDREJ: Toto asi nefunguje lebo suradnice sa neotacaju
                                jointWireFrameGroup.AddPoints(transformedPoints.ToList());

                                // TODO - pridat wireframe pre connectors v plechoch
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
                                jointWireFrameGroup.AddPoints(wireFrame.Points);
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
                                jointWireFrameGroup.AddPoints(wireFrame.Points);
                            }
                        }


                    }
                }
            }

            

            foreach (Model3D m in model3DGroup.Children)
            {
                var transPoints = jointWireFrameGroup.Points.Select(p => m.Transform.Transform(p));
                List<Point3D> points = transPoints.ToList();
                jointWireFrameGroup.Points.Clear();
                jointWireFrameGroup.AddPoints(points);
            }
            //ScreenSpaceLines3D wire = new ScreenSpaceLines3D();            

            //GeometryModel3D gm = GetGeoMetryModel3DFrom(model3DGroup);

            //if (gm.Geometry != null)
            //{
            //    MeshGeometry3D mesh = gm.Geometry as MeshGeometry3D;
            //    for(int i = 0; i < mesh.TriangleIndices.Count; i = i+6)
            //    {
            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i]]);
            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i + 1]]);

            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i + 2]]);
            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i]]);

            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i + 3]]);
            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i + 4]]);

            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i + 4]]);
            //        wire.Points.Add(mesh.Positions[mesh.TriangleIndices[i + 5]]);
            //    }
            //    //mesh.Positions;
            //    //mesh.TriangleIndices;
            //}
            //var transformedPoints = wireFrame.Points.Select(p => a.Transform(p)); // TODO - ONDREJ: Toto asi nefunguje lebo suradnice sa neotacaju
            //jointWireFrameGroup.AddPoints(transformedPoints.ToList());



            // Add Wireframe Lines to the trackport
            //_trackport.ViewPort.Children.Clear();
            viewPort.Children.Add(jointWireFrameGroup);
                    
        }

        public static GeometryModel3D GetGeoMetryModel3DFrom(Model3DGroup model3DGroup)
        {
            GeometryModel3D gm = null;
            foreach (Model3D m in model3DGroup.Children)
            {
                if (m is Model3DGroup) gm = GetGeoMetryModel3DFrom(m as Model3DGroup);
                else if (m is GeometryModel3D) gm = m as GeometryModel3D;
            }
            return gm;
        }

        // Draw Members Wire Frame
        public static void DrawMemberWireFrame(CMember member, Viewport3D viewPort, float memberLength)
        {
            // TODO
            //tu je otazne,ci by to nemohlo byt na urovni CMember, ktora by vratila jeden wireframe pre cely member
            //ScreenSpaceLines3D wireFrame = member.CreateMemberWireFrame();
            //viewPort.Children.Add(wireFrame);

            // To Ondrej: 
            // Kazdy topologicky objekt (prut, plech, skrutka (pripadne cely spoj) moze mat svoje funkcie, ktore vratia jeho surface model alebo wireframe model, pripadne objekty
            // Model3DGroup surfaceModel a ScreenSpaceLines3D wireFrame ako public v triede
            // Podla toho co je rychlejsie, mat velky objekt v triede alebo len volat funkciu ktora ho na poziadanie vygeneruje

            ScreenSpaceLines3D wireFrame_FrontSide = member.CreateWireFrame(0f);
            ScreenSpaceLines3D wireFrame_BackSide = member.CreateWireFrame(memberLength);
            ScreenSpaceLines3D wireFrame_Lateral = member.CreateWireFrameLateral();
            
            viewPort.Children.Add(wireFrame_FrontSide);
            viewPort.Children.Add(wireFrame_BackSide);
            viewPort.Children.Add(wireFrame_Lateral);
        }

        /*
                    The following lights derive from the base class Light:
                    AmbientLight : Provides ambient lighting that illuminates all objects uniformly regardless of their location or orientation.
                    DirectionalLight : Illuminates like a distant light source. Directional lights have a Direction specified as a Vector3D, but no specified location.
                    PointLight : Illuminates like a nearby light source. PointLights have a position and cast light from that position. Objects in the scene are illuminated depending on their position and distance with respect to the light. PointLightBase exposes a Range property, which determines a distance beyond which models will not be illuminated by the light. PointLight also exposes attenuation properties which determine how the light's intensity diminishes over distance. You can specify constant, linear, or quadratic interpolations for the light's attenuation.
                    SpotLight : Inherits from PointLight. Spotlights illuminate like PointLight and have both position and direction. They project light in a cone-shaped area set by InnerConeAngle and OuterConeAngle properties, specified in degrees.
         */

        // Mato - To tam naozaj potrebujeme tolko roznych svetiel, ci su to len pokusy?
        // TODO - To Ondrej: su to len pokusy a typy svetiel, doporucujem zapracovat do GUI moznosti nastavovania osvetlenia, pridavanie svetiel a podobne, nie je to urgentne

        public static void AddLightsToModel3D(Model3DGroup gr)
        {
            // Directional Light
            DirectionalLight Dir_Light = new DirectionalLight();
            Dir_Light.Color = Colors.White;
            Dir_Light.Direction = new Vector3D(0, 0, -1);
            gr.Children.Add(Dir_Light);

            // Point light values
            PointLight Point_Light = new PointLight();
            Point_Light.Position = new Point3D(0, 0, 30);
            Point_Light.Color = Brushes.White.Color;
            Point_Light.Range = 30.0;
            Point_Light.ConstantAttenuation = 0;
            Point_Light.LinearAttenuation = 0;
            Point_Light.QuadraticAttenuation = 0.2f;
            Point_Light.ConstantAttenuation = 5.0;
            gr.Children.Add(Point_Light);

            SpotLight Spot_Light = new SpotLight();
            Spot_Light.InnerConeAngle = 30;
            Spot_Light.OuterConeAngle = 30;
            Spot_Light.Color = Brushes.White.Color;
            Spot_Light.Direction = new Vector3D(0, 0, -1);
            Spot_Light.Position = new Point3D(8.5, 8.5, 20);
            Spot_Light.Range = 30;
            gr.Children.Add(Spot_Light);

            //Set Ambient Light
            AmbientLight Ambient_Light = new AmbientLight();
            Ambient_Light.Color = Color.FromRgb(250, 250, 230);
            gr.Children.Add(new AmbientLight());
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
                fMax_X = (float) cmodel.m_arrGOPoints.Max(p => p.X);
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
    }
}
