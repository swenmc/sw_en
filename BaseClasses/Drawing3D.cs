using _3DTools;
using MATH;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public static class Drawing3D
    {
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

        //-------------------------------------------------------------------------------------------------------------
        // Get model camera position
        public static Point3D GetModelCameraPosition(CModel model, double x, double y, double z)
        {
            Point3D center = GetModelCentre(model);
            return new Point3D(center.X + x, center.Y + y, center.Z + z);
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Members Model3D
        public static Model3D CreateMembersModel3D(CModel model, SolidColorBrush front = null, SolidColorBrush shell = null, SolidColorBrush back = null, bool transpartentModel = false, EGCS egcs = EGCS.eGCSLeftHanded)
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

            Model3D model3D = null;
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
                                // Create Member model - one geometry model                                
                                model3D = model.m_arrMembers[i].getG_M_3D_Member(egcs, front);
                            }
                            else
                            {
                                // Create Member model - consist of 3 geometry models (member is one model group)                                
                                bool bUseCrossSectionColor = true;
                                if (bUseCrossSectionColor && model.m_arrMembers[i].CrScStart.CSColor != null)
                                    shell = new SolidColorBrush(model.m_arrMembers[i].CrScStart.CSColor);

                                model3D = model.m_arrMembers[i].getM_3D_G_Member(egcs, front, shell, back);
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
        public static Model3DGroup CreateConnectionJointsModel3DGroup(CModel cmodel, SolidColorBrush brushPlates = null, SolidColorBrush brushBolts = null, SolidColorBrush brushWelds = null)
        {
            if (brushPlates == null) brushPlates = new SolidColorBrush(Colors.Gray);
            if (brushBolts == null) brushBolts = new SolidColorBrush(Colors.Red);
            if (brushWelds == null) brushWelds = new SolidColorBrush(Colors.Orange);

            Model3DGroup JointsModel3DGroup = null;

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
                            }
                        }
                    }

                    // Bolts
                    if (cmodel.m_arrConnectionJoints[i].m_arrBolts != null)
                    {
                        for (int l = 0; l < cmodel.m_arrConnectionJoints[i].m_arrBolts.Length; l++)
                        {
                            if (cmodel.m_arrConnectionJoints[i].m_arrBolts[l] != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrBolts[l].m_pControlPoint != null &&
                            cmodel.m_arrConnectionJoints[i].m_arrBolts[l].BIsDisplayed == true) // Bolt object is valid (not empty) and should be displayed
                            {
                                JointModelGroup.Children.Add(cmodel.m_arrConnectionJoints[i].m_arrBolts[l].CreateGeomModel3D(brushBolts)); // Add bolt 3D model to the model group
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
                            // There is not rotation

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
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private static void CalculateModelLimits(CModel cmodel, out float fMax_X, out float fMin_X, out float fMax_Y, out float fMin_Y, out float fMax_Z, out float fMin_Z)
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

    }
}
