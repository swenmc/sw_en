using _3DTools;
using MATH;
using Petzold.Media3D;
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
        public static void DrawToTrackPort(Trackport3D _trackport, CModel model, DisplayOptions sDisplayOptions, CLoadCase loadcase)
        {
            DateTime start = DateTime.Now;

            // Color of Trackport
            _trackport.TrackportBackground = new SolidColorBrush(Colors.Black);

            // Global coordinate system - axis
            if (sDisplayOptions.bDisplayGlobalAxis) Drawing3D.DrawGlobalAxis(_trackport.ViewPort, model);

            System.Diagnostics.Trace.WriteLine("Begining: " + (DateTime.Now - start).TotalMilliseconds);
            if (model != null)
            {
                Model3DGroup gr = new Model3DGroup();

                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayMembers) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);
                System.Diagnostics.Trace.WriteLine("After CreateMembersModel3D: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);
                System.Diagnostics.Trace.WriteLine("After CreateConnectionJointsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                bool displayOtherObjects3D = true;
                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);
                System.Diagnostics.Trace.WriteLine("After CreateModelOtherObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup loadsModel3DGroup = null;
                if (sDisplayOptions.bDisplayLoads) loadsModel3DGroup = Drawing3D.CreateModelLoadObjectsModel3DGroup(loadcase);
                if (loadsModel3DGroup != null) gr.Children.Add(loadsModel3DGroup);
                System.Diagnostics.Trace.WriteLine("After CreateModelLoadObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

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
                System.Diagnostics.Trace.WriteLine("After DrawModelMembersCenterLines: " + (DateTime.Now - start).TotalMilliseconds);

                // Add WireFrame Model
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers)
                {
                    if(membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial);
                    //Drawing3D.DrawModelMembersinOneWireFrame(model, _trackport.ViewPort);
                    Drawing3D.DrawModelMembersWireFrame_OP(model, _trackport.ViewPort);
                } 
                System.Diagnostics.Trace.WriteLine("After DrawModelMembersinOneWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints)
                {
                    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort);
                }
                System.Diagnostics.Trace.WriteLine("After DrawModelConnectionJointsWireFrame: " + (DateTime.Now - start).TotalMilliseconds);
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

        public static Model3DGroup CreateModel3DGroup(CModel model, DisplayOptions sDisplayOptions, EGCS egcs = EGCS.eGCSLeftHanded)
        {
            // TODO - Ondrej - pridat do nastaveni, default = (emissive???)

            bool bUseDiffuseMaterial = true;
            bool bUseEmissiveMaterial = true;

            Model3DGroup gr = new Model3DGroup();
            if (model != null && sDisplayOptions.bDisplaySolidModel)
            {
                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplayMembers) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, bUseDiffuseMaterial, bUseEmissiveMaterial, null, null, null, egcs);
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
            if (front == null) front = new SolidColorBrush(Colors.OrangeRed); // Material color - Front Side
            if (shell == null) shell = new SolidColorBrush(Colors.SlateBlue); // Material color - Shell
            if (back == null) back = new SolidColorBrush(Colors.OrangeRed); // Material color - Back Side

            if (bTranspartentModel)
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
                                model.m_arrMembers[i].WireFramePoints = GetWireFramePointsFromMemberGeometryPositions(((MeshGeometry3D)geom3D.Geometry).Positions);
                                model3D.Children.Add(geom3D); // Use shell color for whole member
                            }
                            else
                            {
                                // Create Member model - consist of 3 geometry models (member is one model group)
                                if (model3D == null) model3D = new Model3DGroup();
                                Model3DGroup mgr = model.m_arrMembers[i].getM_3D_G_Member(egcs, front, shell, back, bUseDiffuseMaterial, bUseEmissiveMaterial);
                                model.m_arrMembers[i].WireFramePoints = GetWireFramePointsFromGeometryPositions(((MeshGeometry3D)((GeometryModel3D)mgr.Children[0]).Geometry).Positions);
                                model.m_arrMembers[i].WireFramePoints.AddRange(GetWireFramePointsFromGeometryShellPositions(((MeshGeometry3D)((GeometryModel3D)mgr.Children[1]).Geometry).Positions));
                                model.m_arrMembers[i].WireFramePoints.AddRange(GetWireFramePointsFromGeometryPositions(((MeshGeometry3D)((GeometryModel3D)mgr.Children[2]).Geometry).Positions));
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
                if (i < positions.Count / 4)  //1/4 front
                {
                    wireframePoints.Add(positions[i]);
                    wireframePoints.Add(positions[i + 1]);
                }
                else if (i >= positions.Count / 4 * 3) // 3/4 Back side
                {
                    wireframePoints.Add(positions[i]);
                    wireframePoints.Add(positions[i + 1]);
                }
                else //between 1/4 and 3/4 is Shell
                {
                    //zase raz je to standardne spravene takze raz su body pre Shell pridavane tak a raz tak - nutne je to zjednotit

                    wireframePoints.Add(positions[i]);
                    wireframePoints.Add(positions[i + shift]);

                    //wireframePoints.Add(positions[i]);
                    //wireframePoints.Add(positions[positions.Count - 1 - i]);
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
                        brushPlates = new SolidColorBrush(Colors.DarkGreen);

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
                                if (cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors != null &&
                                    cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors.Length > 0)
                                {
                                    Model3DGroup plateConnectorsModelGroup = new Model3DGroup();
                                    for (int m = 0; m < cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors.Length; m++)
                                    {
                                        GeometryModel3D plateConnectorgeom = cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors[m].CreateGeomModel3D(brushConnectors);
                                        cmodel.m_arrConnectionJoints[i].m_arrPlates[l].m_arrPlateConnectors[m].Visual_Connector = plateConnectorgeom;
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
                        // TODO Ondrej 15/07/2018
                        // na riadku 181 sme pridali do spoja postupne plechy a na riakdu 190 vsetky skrutky v jednotlivych plechoch
                        // mame teda spoj so suradnicami v LCS pruta (na zaciatku alebo na konci)

                        // Update 3
                        // Cely spoj na prute (vsetky plechy aj skrutky v spoji) tu pootocime o uhol theta okolo LCS osi x pruta
                        // Po tomto pootoceni by sa mali suradnice plechov a skrutiek v spoji prepocitat o pootocenie theta ulozit

                        // TODO prepracovat tento blok a podmienky tak, aby v nebol prazdny else a odstranit duplicitu

                        // Rotate model about local x-axis (LCS - local coordinate system of member)


                        // O.P. 16.7.2018 - zakomentoval som a nevidim aby to nieco robilo =  neviem,ci to treba
                        // M.C. 18.7.2018 - je to potrebne, ak je spoj definovany v LCS pruta, tak sa musi pootocit spolu s prutom o uhol theta (pripadne sa mu musi nastavit este aj excentricita ktora je nastavena prutu), ak je definovany v GCS tak sa neotaca

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

                        // TODO Ondrej 15/07/2018
                        // ak sme vsetky prvky v spoji pootocili o theta mame pripraveny spoj na transformaciu z LCS do GCS

                        // Update 4
                        // Cely spoj na prute (vsetky plechy aj skrutky v spoji) tu pootocime a presunieme na poziciu kde sa ma nachazat na prute v GCS
                        // Ma sa pouzit rovnaka transformacia akou sa presuva samotny prut z LCS do GCS
                        // Po tomto presune a pootoceni by sa mali suradnice plechov a skrutiek v spoji prepocitat z LCS GCS a ulozit, to je finalny stav

                        // Rotate and translate model in GCS (global coordinate system of whole structure / building)
                        // Create new model group

                        //Model3DGroup JointModelGroup_temp = new Model3DGroup();
                        //JointModelGroup_temp.Children.Add(JointModelGroup);

                        // Joint is defined in LCS of first secondary member
                        if (cmodel.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                        cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null)
                        {
                            // Transform model group
                            //JointModelGroup = cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS(JointModelGroup_temp, cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0]);

                            //temp
                            cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS_ChangeOriginal(JointModelGroup, cmodel.m_arrConnectionJoints[i].m_SecondaryMembers[0]);
                        }
                        else // Joint is defined in LCS of main member
                        {
                            // Transform model group
                            //JointModelGroup = cmodel.m_arrConnectionJoints[i].Transform3D_OnMemberEntity_fromLCStoGCS(JointModelGroup_temp, cmodel.m_arrConnectionJoints[i].m_MainMember);

                            //temp
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
                            model3D_group.Children.Add(selectedLoadCase.NodeLoadsList[i].CreateM_3D_G_Load()); // Add to model group

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

                            model3D_group.Children.Add(model_gr); // Add Release to model group

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
                            // Transform modelgroup from LCS to GCS
                            //model_gr = selectedLoadCase.SurfaceLoadsList[i].Transform3D_OnMemberEntity_fromLCStoGCS(model_gr, selectedLoadCase.MemberLoadsList[i].Member);

                            model3D_group.Children.Add(model_gr); // Add Release to model group

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
        public static void DrawGlobalAxis(Viewport3D viewPort, CModel model)
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
            sAxisX_3D.Name = "AxisX";

            sAxisY_3D.Points.Add(pGCS_centre);
            sAxisY_3D.Points.Add(pAxisY);
            sAxisY_3D.Color = Colors.Green;
            sAxisY_3D.Thickness = 2;
            sAxisY_3D.Name = "AxisY";

            sAxisZ_3D.Points.Add(pGCS_centre);
            sAxisZ_3D.Points.Add(pAxisZ);
            sAxisZ_3D.Color = Colors.Blue;
            sAxisZ_3D.Thickness = 2;
            sAxisZ_3D.Name = "AxisZ";

            model.AxisX = sAxisX_3D;
            model.AxisY = sAxisY_3D;
            model.AxisZ = sAxisZ_3D;

            //temp skuska
            WireLine wX = new WireLine();
            wX.Point1 = pGCS_centre;
            wX.Point2 = pAxisX;
            wX.Thickness = 2;
            wX.Color = Colors.Red;

            WireLine wY = new WireLine();
            wY.Point1 = pGCS_centre;
            wY.Point2 = pAxisY;
            wY.Thickness = 2;
            wY.Color = Colors.Green;

            WireLine wZ = new WireLine();
            wZ.Point1 = pGCS_centre;
            wZ.Point2 = pAxisZ;
            wZ.Thickness = 2;
            wZ.Color = Colors.Blue;

            viewPort.Children.Add(wX);
            viewPort.Children.Add(wY);
            viewPort.Children.Add(wZ);
            //end temp skuska

            //viewPort.Children.Add(sAxisX_3D);
            //viewPort.Children.Add(sAxisY_3D);
            //viewPort.Children.Add(sAxisZ_3D);
        }
        //-------------------------------------------------------------------------------------------------------------

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
                        Point3D pEndStart = new Point3D(model.m_arrMembers[i].NodeEnd.X, model.m_arrMembers[i].NodeEnd.Y, model.m_arrMembers[i].NodeEnd.Z);

                        // Create centerline of member
                        lines.Points.Add(pNodeStart); // Add Start Node
                        lines.Points.Add(pEndStart); // Add End Node
                    }
                }

                viewPort.Children.Add(lines);
            }
        }

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
                        model.m_arrMembers[i].CrScStart != null &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                    {
                        for (int j = 0; j < 3; j++) // Per front, back side and laterals
                        {
                            if (j == 0) // Front Side
                                wireFrameMemberPointNo = model.m_arrMembers[i].GetMemberWireFrameFrontIndices();
                            else if (j == 1) // Laterals
                                wireFrameMemberPointNo = model.m_arrMembers[i].GetMemberWireFrameLateralIndices();
                            else //if (j == 2) // Back Side
                                wireFrameMemberPointNo = model.m_arrMembers[i].GetMemberWireFrameBackIndices();

                            foreach (Int32 no in wireFrameMemberPointNo) // Assign Point3D of surface model to the each number in the wireframe collection 
                            {
                                // TODO Ondrej - performance - Toto bude potrebne odstranit
                                // Mali by sa pouzit data zo surface modelu pruta, takto sa to vytvara 2 krat raz pre surface model a druhy krat pre wireframe
                                // Vyriesit co sa stane, ak budeme chciet zobrazit len samostatny wireframe a surface model teda nebude k dispozicii (vygeneruje sa, ale nepouzije sa pri vykresleni?)

                                Model3DGroup model3D = new Model3DGroup();
                                model3D = model.m_arrMembers[i].getM_3D_G_Member(EGCS.eGCSLeftHanded, Brushes.AliceBlue, Brushes.AliceBlue, Brushes.AliceBlue, true, true);

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
                wireFrameAllMembers.Name = "WireFrame_Members";
                model.WireFrameMembers = wireFrameAllMembers;
                viewPort.Children.Add(wireFrameAllMembers);
            }
        }

        // Add all members in one wireframe collection of ScreenSpaceLines3D
        public static void DrawModelMembersWireFrame_OP(CModel model, Viewport3D viewPort)
        {
            // Members - Wire Frame
            if (model.m_arrMembers != null)
            {
                Color wireFrameColor = Color.FromRgb(60, 60, 60);
                double thickness = 1.0;
                ScreenSpaceLines3D wireFrameAllMembers = new ScreenSpaceLines3D(wireFrameColor, thickness); // Just one collection for all members
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

                // Add Wireframe Lines to the trackport                
                //wireFrameAllMembers.Name = "WireFrame_Members";
                //wireFrameAllMembers.Points = new Point3DCollection(wireFramePoints);
                //model.WireFrameMembers = wireFrameAllMembers;
                //viewPort.Children.Add(wireFrameAllMembers);

                WireLines wl = new WireLines();
                wl.Lines = new Point3DCollection(wireFramePoints);
                wl.Color = Colors.White;
                viewPort.Children.Add(wl);

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
                                    if (model.m_arrConnectionJoints[i].m_arrPlates[j].m_arrPlateConnectors != null &&
                                        model.m_arrConnectionJoints[i].m_arrPlates[j].m_arrPlateConnectors.Length > 0)
                                    {
                                        for (int m = 0; m < model.m_arrConnectionJoints[i].m_arrPlates[j].m_arrPlateConnectors.Length; m++)
                                        {
                                            GeometryModel3D geom = model.m_arrConnectionJoints[i].m_arrPlates[j].m_arrPlateConnectors[m].Visual_Connector;
                                            Point3DCollection pointsConnector = model.m_arrConnectionJoints[i].m_arrPlates[j].m_arrPlateConnectors[m].WireFrameModelPointsFromVisual();
                                            var transPoints_PlateConnector = pointsConnector.Select(p => geom.Transform.Transform(p));
                                            jointPlatePoints.AddRange(transPoints_PlateConnector);
                                        }
                                    }
                                }
                                var transPoints_Plate = jointPlatePoints.Select(p => model.m_arrConnectionJoints[i].m_arrPlates[j].Visual_Plate.Transform.Transform(p));
                                jointPoints.AddRange(transPoints_Plate);
                            }
                        }

                        // Joint is defined in LCS of first secondary member
                        //if (model.m_arrConnectionJoints[i].m_SecondaryMembers != null &&
                        //model.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null &&
                        //!MathF.d_equal(model.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x, 0))
                        //{
                        //    AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), model.m_arrConnectionJoints[i].m_SecondaryMembers[0].DTheta_x / MathF.fPI * 180);
                        //    RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x);
                        //    jointPoints = jointPoints.Select(p => rotate.Transform(p)).ToList();
                        //}
                        //else if (!MathF.d_equal(model.m_arrConnectionJoints[i].m_MainMember.DTheta_x, 0)) // Joint is defined in LCS of main member and rotation degree is not zero
                        //{
                        //    AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), model.m_arrConnectionJoints[i].m_MainMember.DTheta_x / MathF.fPI * 180);
                        //    RotateTransform3D rotate = new RotateTransform3D(Rotation_LCS_x);
                        //    jointPoints = jointPoints.Select(p => rotate.Transform(p)).ToList();
                        //}
                        //else
                        //{
                        //    // There is no rotation
                        //}

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

                        //if (!model.m_arrConnectionJoints[i].bIsJointDefinedinGCS) // Joint is defined in LCS
                        //{
                        //    Transform3DGroup tr;

                        //    if (model.m_arrConnectionJoints[i].m_SecondaryMembers != null && model.m_arrConnectionJoints[i].m_SecondaryMembers[0] != null)
                        //    {
                        //        // Create Transformation Matrix
                        //        tr = model.m_arrConnectionJoints[i].CreateTransformCoordGroup(model.m_arrConnectionJoints[i].m_SecondaryMembers[0]);
                        //    }
                        //    else
                        //    {
                        //        // Create Transformation Matrix
                        //        tr = model.m_arrConnectionJoints[i].CreateTransformCoordGroup(model.m_arrConnectionJoints[i].m_MainMember);
                        //    }
                        //    jointPoints = jointPoints.Select(p => tr.Transform(p)).ToList();
                        //}

                        //23.7.2018
                        //Mato - otestuj,ci vsetko funguje ako ma
                        //prerobil som Transform3D_OnMemberEntity_fromLCStoGCS metodu na Transform3D_OnMemberEntity_fromLCStoGCS_ChangeOriginal a jednym riadkom to funguje...takze chyba bola tam
                        //vsetko prerobene do jedneho riadku kodu - narocky som nechal zakomentovane riadky,aby bolo vidno ktore riadky to nahradza
                        var transPoints = jointPoints.Select(p => model.m_arrConnectionJoints[i].Visual_ConnectionJoint.Transform.Transform(p));
                        jointsWireFramePoints.AddRange(transPoints);
                    }
                }

                //ScreenSpaceLines3D jointsWireFrameTotal = new ScreenSpaceLines3D();
                //jointsWireFrameTotal.Points = new Point3DCollection(jointsWireFramePoints);
                //jointsWireFrameTotal.Name = "WireFrame_Joints";
                //model.WireFrameJoints = jointsWireFrameTotal;
                //viewPort.Children.Add(jointsWireFrameTotal);

                WireLines wl = new WireLines();
                wl.Lines = new Point3DCollection(jointsWireFramePoints);
                wl.Color = Colors.White;
                viewPort.Children.Add(wl);
                

            }
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
    }
}
