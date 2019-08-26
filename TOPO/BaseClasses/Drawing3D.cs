using _3DTools;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using HelixToolkit.Wpf;
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
        private static bool centerModel = true;
        private static float fModel_Length_X = 0;
        private static float fModel_Length_Y = 0;
        private static float fModel_Length_Z = 0;
        private static Transform3DGroup centerModelTransGr = null;

        private static Transform3DGroup GetModelRotationAccordingToView(DisplayOptions sDisplayOptions)
        {
            Transform3DGroup transGr = new Transform3DGroup();
            if (sDisplayOptions.ModelView == (int)EModelViews.ISO_FRONT_RIGHT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -70);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), -20);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.ISO_FRONT_LEFT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -70);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 20);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.ISO_BACK_RIGHT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 80);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 180);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));
                AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 20);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.ISO_BACK_LEFT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 80);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 180);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));
                AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), -20);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.FRONT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.BACK)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 180);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));
            }
            /*
            else if (sDisplayOptions.ModelView == (int)EModelViews.BOTTOM)
            {
                //takto pokial zachovavame Left/Right
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 180);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));

                //takto pokial chceme zachovat Front/Back
                //AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180);
                //transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }*/
            else if (sDisplayOptions.ModelView == (int)EModelViews.LEFT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.RIGHT)
            {
                AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), -90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.TOP)
            {
                AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), -90);
                transGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));
            }
            return transGr;
        }

        private static void SetOrtographicCameraWidth(ref DisplayOptions sDisplayOptions, float fModel_Length_X, float fModel_Length_Y, float fModel_Length_Z)
        {
            if (sDisplayOptions.ModelView == (int)EModelViews.ISO_FRONT_RIGHT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Z);
                sDisplayOptions.OrtographicCameraWidth *= 1.5;
            }
            if (sDisplayOptions.ModelView == (int)EModelViews.ISO_FRONT_LEFT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Z);
                sDisplayOptions.OrtographicCameraWidth *= 1.5;
            }
            if (sDisplayOptions.ModelView == (int)EModelViews.ISO_BACK_RIGHT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Z);
                sDisplayOptions.OrtographicCameraWidth *= 1.5;
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.ISO_BACK_LEFT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Z);
                sDisplayOptions.OrtographicCameraWidth *= 1.5;
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.FRONT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Z);
                sDisplayOptions.OrtographicCameraWidth *= 1.2;
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.BACK)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Z);
                sDisplayOptions.OrtographicCameraWidth *= 1.2;
            }
            /*else if (sDisplayOptions.ModelView == (int)EModelViews.BOTTOM)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Y);
                sDisplayOptions.OrtographicCameraWidth *= 1.5;
            }*/
            else if (sDisplayOptions.ModelView == (int)EModelViews.LEFT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_Z, fModel_Length_Y);
                sDisplayOptions.OrtographicCameraWidth *= 1.2;
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.RIGHT)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_Z, fModel_Length_Y);
                sDisplayOptions.OrtographicCameraWidth *= 1.2;
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.TOP)
            {
                sDisplayOptions.OrtographicCameraWidth = Math.Max(fModel_Length_X, fModel_Length_Y);
                sDisplayOptions.OrtographicCameraWidth *= 1.5;
            }
        }

        public static CModel DrawToTrackPort(Trackport3D _trackport, CModel _model, DisplayOptions sDisplayOptions, CLoadCase loadcase)
        {
            CModel model = null;
            //DateTime start = DateTime.Now;

            // Color of Trackport
            _trackport.TrackportBackground = new SolidColorBrush(sDisplayOptions.backgroundColor);

            //System.Diagnostics.Trace.WriteLine("Beginning: " + (DateTime.Now - start).TotalMilliseconds);
            if (_model != null)
            {
                model = Drawing3D.GetModelAccordingToView(_model, sDisplayOptions);

                fModel_Length_X = 0;
                fModel_Length_Y = 0;
                fModel_Length_Z = 0;
                Point3D pModelGeomCentre = Drawing3D.GetModelCentreWithoutCrsc(model, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);
                if (centerModel)
                {
                    centerModelTransGr = new Transform3DGroup();
                    centerModelTransGr.Children.Add(new TranslateTransform3D(-fModel_Length_X / 2.0f, -fModel_Length_Y / 2.0f, -fModel_Length_Z / 2.0f));

                    //model rotation for the VIEW
                    centerModelTransGr.Children.Add(GetModelRotationAccordingToView(sDisplayOptions));
                    //AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90);
                    //centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                }

                // Global coordinate system - axis
                if (sDisplayOptions.bDisplayGlobalAxis) DrawGlobalAxis(_trackport.ViewPort, model, (centerModel ? centerModelTransGr : null));

                if (sDisplayOptions.bDisplaySurfaceLoadAxis) DrawSurfaceLoadsAxis(loadcase, _trackport.ViewPort);

                if (sDisplayOptions.bDisplayLocalMembersAxis) DrawModelMembersAxis(model, _trackport.ViewPort);

                Model3DGroup gr = new Model3DGroup();

                // Najprv sa musia vykreslit labels lebo su nepriehliadne a az potom sa vykresluju transparentne objekty
                if (sDisplayOptions.bDisplayLoads && sDisplayOptions.bDisplayLoadsLabels)
                {
                    gr.Children.Add(Drawing3D.CreateLabels3DForLoadCase(model, loadcase, sDisplayOptions));
                    //System.Diagnostics.Trace.WriteLine("After CreateLabels3DForLoadCase: " + (DateTime.Now - start).TotalMilliseconds);
                }

                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayMembers)
                    membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial,
                        sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);
                //System.Diagnostics.Trace.WriteLine("After CreateMembersModel3D: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateConnectionJointsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup foundationsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayFoundations) foundationsModel3DGroup = Drawing3D.CreateModelFoundationsModel3DGroup(model, sDisplayOptions);
                if (foundationsModel3DGroup != null) gr.Children.Add(foundationsModel3DGroup);

                Model3DGroup slabsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayFloorSlab) slabsModel3DGroup = Drawing3D.CreateModelSlabsModel3DGroup(model, sDisplayOptions);
                if (slabsModel3DGroup != null) gr.Children.Add(slabsModel3DGroup);

                Model3DGroup sawCutsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplaySawCuts) sawCutsModel3DGroup = Drawing3D.CreateModelSawCutsModel3DGroup(model, sDisplayOptions);
                if (sawCutsModel3DGroup != null) gr.Children.Add(sawCutsModel3DGroup);

                Model3DGroup controlJointsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayControlJoints) controlJointsModel3DGroup = Drawing3D.CreateModelControlJointsModel3DGroup(model, sDisplayOptions);
                if (controlJointsModel3DGroup != null) gr.Children.Add(controlJointsModel3DGroup);

                bool displayOtherObjects3D = true;
                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model, sDisplayOptions);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateModelOtherObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup loadsModel3DGroup = null;
                if (sDisplayOptions.bDisplayLoads) loadsModel3DGroup = Drawing3D.CreateModelLoadObjectsModel3DGroup(loadcase, sDisplayOptions);
                if (loadsModel3DGroup != null) gr.Children.Add(loadsModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateModelLoadObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup nodes3DGroup = null;
                if (sDisplayOptions.bDisplayNodes) nodes3DGroup = Drawing3D.CreateModelNodes_Model3DGroup(model);
                if (nodes3DGroup != null) gr.Children.Add(nodes3DGroup);

                DrawDimensionsToTrackport(_trackport, sDisplayOptions, model, gr);

                // Pokus vyrobit lines 3D objekty
                // TO Ondrej - treba to nejako rozumne oddelit, aby sa wireframe nevytvaral a nepridaval 2x
                // Add WireFrame Model
                // Members
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMembersWireFrame)
                {
                    Model3DGroup lines; // linie ako 3D valcove plochy
                    if (membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                    Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                // Joints
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints && sDisplayOptions.bDisplayJointsWireFrame)
                {
                    Model3DGroup lines;  // linie ako 3D valcove plochy
                    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                // Foundations
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayFoundations && sDisplayOptions.bDisplayFoundationsWireFrame)
                {
                    Model3DGroup lines;  // linie ako 3D valcove plochy
                    if (foundationsModel3DGroup == null) foundationsModel3DGroup = Drawing3D.CreateModelFoundationsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelFoundationsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                // Slabs
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayFloorSlab && sDisplayOptions.bDisplayFloorSlabWireFrame)
                {
                    Model3DGroup lines;  // linie ako 3D valcove plochy
                    if (slabsModel3DGroup == null) slabsModel3DGroup = Drawing3D.CreateModelSlabsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelSlabsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                Drawing3D.AddLightsToModel3D(gr, sDisplayOptions);

                if (centerModel)
                {
                    //translate transform to model center
                    ((Model3D)gr).Transform = centerModelTransGr;
                    double maxLen = MathF.Max(fModel_Length_X, fModel_Length_Y, fModel_Length_Z);

                    Point3D cameraPosition = new Point3D(0, 0, maxLen * 2);
                    _trackport.PerspectiveCamera.Position = cameraPosition;
                    _trackport.PerspectiveCamera.LookDirection = new Vector3D(0, 0, -1);

                    if (sDisplayOptions.bUseOrtographicCamera)
                    {
                        SetOrtographicCameraWidth(ref sDisplayOptions, fModel_Length_X, fModel_Length_Y, fModel_Length_Z);
                        OrthographicCamera ort_camera = new OrthographicCamera(cameraPosition, new Vector3D(0, 0, -1), _trackport.PerspectiveCamera.UpDirection, sDisplayOptions.OrtographicCameraWidth);
                        ort_camera.FarPlaneDistance = double.PositiveInfinity;
                        ort_camera.NearPlaneDistance = double.NegativeInfinity;
                        _trackport.ViewPort.Camera = ort_camera;
                    }
                }
                else
                {
                    Point3D cameraPosition = Drawing3D.GetModelCameraPosition(model, 1, -(2 * fModel_Length_Y), 2 * fModel_Length_Z);
                    _trackport.PerspectiveCamera.Position = cameraPosition;
                    _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);
                }

                _trackport.Model = (Model3D)gr;

                // Add centerline member model
                if (sDisplayOptions.bDisplayMembersCenterLines && sDisplayOptions.bDisplayMembers) Drawing3D.DrawModelMembersCenterLines(model, _trackport.ViewPort);
                //System.Diagnostics.Trace.WriteLine("After DrawModelMembersCenterLines: " + (DateTime.Now - start).TotalMilliseconds);

                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints)
                //{
                //    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                //    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort, sDisplayOptions);
                //}
                //System.Diagnostics.Trace.WriteLine("After DrawModelConnectionJointsWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayFoundations)
                //{
                //    Model3DGroup lines;
                //    if (foundationsModel3DGroup == null) foundationsModel3DGroup = Drawing3D.CreateModelFoundationsModel3DGroup(model, sDisplayOptions);
                //    Drawing3D.DrawModelFoundationsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                //}

                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayFloorSlab)
                //{
                //    Model3DGroup lines;
                //    if (slabsModel3DGroup == null) slabsModel3DGroup = Drawing3D.CreateModelSlabsModel3DGroup(model, sDisplayOptions);
                //    Drawing3D.DrawModelSlabsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                //}

                if (sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMemberDescription)
                {
                    //Drawing3D.CreateMembersDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                    Drawing3D.CreateMembersDescriptionModel3D_POKUS_MC(model, _trackport.ViewPort, sDisplayOptions); // To Ondrej POKUS 17.8.2019
                    //System.Diagnostics.Trace.WriteLine("After CreateMembersDescriptionModel3D: " + (DateTime.Now - start).TotalMilliseconds);
                }

                if (sDisplayOptions.bDisplayNodesDescription)
                {
                    Drawing3D.CreateNodesDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }

                if (sDisplayOptions.bDisplaySawCuts)
                {
                    Drawing3D.CreateSawCutDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }

                if (sDisplayOptions.bDisplayControlJoints)
                {
                    Drawing3D.CreateControlJointDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }

                if (sDisplayOptions.bDisplayFoundationsDescription)
                {
                    Drawing3D.CreateFoundationsDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }

                if (sDisplayOptions.bDisplayFloorSlabDescription)
                {
                    Drawing3D.CreateFloorSlabsDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }

                //if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FRONT)
                //{
                //    CMember m1 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
                //    CMember m2 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);

                //    CDimensionLinear3D dim = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m2.NodeEnd.GetPoint3D(), new Vector3D(0, 0, -1), 0.5, 0.4,0.1, (model.fW_frame * 1000).ToString());

                //    Drawing3D.DrawDimension3D(dim, _trackport.ViewPort, sDisplayOptions);
                //}
            }

            _trackport.SetupScene();
            return model;
        }

        public static void DrawJointToTrackPort(Trackport3D _trackport, CModel model, DisplayOptions sDisplayOptions)
        {
            //DateTime start = DateTime.Now;

            // Color of Trackport
            _trackport.TrackportBackground = new SolidColorBrush(sDisplayOptions.backgroundColor);
            centerModel = true;
            //System.Diagnostics.Trace.WriteLine("Beginning: " + (DateTime.Now - start).TotalMilliseconds);
            if (model != null)
            {
                float fTempMax_X = 0f, fTempMin_X = 0f, fTempMax_Y = 0f, fTempMin_Y = 0f, fTempMax_Z = 0f, fTempMin_Z = 0f;

                if (model.m_arrMembers != null || model.m_arrGOPoints != null) // Some members or points must be defined in the model
                    CalculateModelLimitsWithCrsc(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                fModel_Length_X = 0;
                fModel_Length_Y = 0;
                fModel_Length_Z = 0;
                Point3D pModelGeomCentre = Drawing3D.GetModelCentreWithCrsc(model, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);

                centerModelTransGr = new Transform3DGroup();
                centerModelTransGr.Children.Add(new TranslateTransform3D(-fTempMin_X, -fTempMin_Y, -fTempMin_Z));
                centerModelTransGr.Children.Add(new TranslateTransform3D(-fModel_Length_X / 2.0f, -fModel_Length_Y / 2.0f, -fModel_Length_Z / 2.0f));

                if (sDisplayOptions.RotateModelX != 0)
                {
                    AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), sDisplayOptions.RotateModelX);
                    centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                }
                if (sDisplayOptions.RotateModelY != 0)
                {
                    if (!IsJointSecondaryMemberTowardsCamera(model)) sDisplayOptions.RotateModelY += 180;

                    AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), sDisplayOptions.RotateModelY);
                    centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
                }
                if (sDisplayOptions.RotateModelZ != 0)
                {
                    AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), sDisplayOptions.RotateModelZ);
                    centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));
                }

                // Global coordinate system - axis
                if (sDisplayOptions.bDisplayGlobalAxis) DrawGlobalAxis(_trackport.ViewPort, model, null);
                if (sDisplayOptions.bDisplayLocalMembersAxis) DrawModelMembersAxis(model, _trackport.ViewPort);

                Model3DGroup gr = new Model3DGroup();
                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayMembers)
                    membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial,
                        sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);
                //System.Diagnostics.Trace.WriteLine("After CreateMembersModel3D: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null)
                {
                    gr.Children.Add(jointsModel3DGroup);
                }
                //System.Diagnostics.Trace.WriteLine("After CreateConnectionJointsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                bool displayOtherObjects3D = true;
                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model, sDisplayOptions);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateModelOtherObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup nodes3DGroup = null;
                if (sDisplayOptions.bDisplayNodes) nodes3DGroup = Drawing3D.CreateModelNodes_Model3DGroup(model);
                if (nodes3DGroup != null) gr.Children.Add(nodes3DGroup);

                // Add WireFrame Model
                // Members
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMembersWireFrame)
                {
                    Model3DGroup lines; // linie ako 3D valcove plochy
                    if (membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                    Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                // Joints
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints && sDisplayOptions.bDisplayJointsWireFrame)
                {
                    Model3DGroup lines;  // linie ako 3D valcove plochy
                    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                Drawing3D.AddLightsToModel3D(gr, sDisplayOptions);

                if (centerModel)
                {
                    //translate transform to model center
                    ((Model3D)gr).Transform = centerModelTransGr;

                    double zoomFactor = 2;
                    if (fModel_Length_X > 2 * fModel_Length_Z) zoomFactor = 1.5;

                    Point3D cameraPosition = new Point3D(0, 0, MathF.Max(fModel_Length_X, fModel_Length_Y, fModel_Length_Z) * zoomFactor);
                    _trackport.PerspectiveCamera.Position = cameraPosition;
                    _trackport.PerspectiveCamera.LookDirection = new Vector3D(0, 0, -1);
                }
                else
                {
                    Point3D cameraPosition = Drawing3D.GetModelCameraPosition(model, 1, -(2 * fModel_Length_Y), 2 * fModel_Length_Z);
                    _trackport.PerspectiveCamera.Position = cameraPosition;
                    _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);
                }

                _trackport.Model = (Model3D)gr;

                // Add centerline member model
                if (sDisplayOptions.bDisplayMembersCenterLines && sDisplayOptions.bDisplayMembers) Drawing3D.DrawModelMembersCenterLines(model, _trackport.ViewPort);
                //System.Diagnostics.Trace.WriteLine("After DrawModelMembersCenterLines: " + (DateTime.Now - start).TotalMilliseconds);

                //// Add WireFrame Model
                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers)
                //{
                //    Model3DGroup lines;
                //    if (membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                //    Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                //}
                ////System.Diagnostics.Trace.WriteLine("After DrawModelMembersinOneWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints)
                //{
                //    Model3DGroup lines;
                //    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                //    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                //}
                //System.Diagnostics.Trace.WriteLine("After DrawModelConnectionJointsWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                if (sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMemberDescription)
                {
                    //Drawing3D.CreateMembersDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                    Drawing3D.CreateMembersDescriptionModel3D_POKUS_MC(model, _trackport.ViewPort, sDisplayOptions); // To Ondrej POKUS 17.8.2019
                    //System.Diagnostics.Trace.WriteLine("After CreateMembersDescriptionModel3D: " + (DateTime.Now - start).TotalMilliseconds);
                }
                if (sDisplayOptions.bDisplayNodesDescription)
                {
                    Drawing3D.CreateNodesDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }
            }

            ////test viem zobrazit ViewCube - problemov je ale kopec od toho,ze nie je klikatelny az po to,ze je sucastou modelu, co nie je vhodne            
            //ViewCubeVisual3D viewCube = new ViewCubeVisual3D();
            //viewCube.BackText = "Back";
            //viewCube.FrontText = "Front";            
            //AxisAngleRotation3D Rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), sDisplayOptions.RotateModelX);

            //Transform3DGroup tr_gr = new Transform3DGroup();
            //tr_gr.Children.Add(new ScaleTransform3D(0.01,0.01,0.01));
            //tr_gr.Children.Add(new RotateTransform3D(Rot_x));
            //tr_gr.Children.Add(new TranslateTransform3D(fModel_Length_X * 2, 0, 0));
            //viewCube.Transform = tr_gr;            
            //_trackport.ViewPort.Children.Add(viewCube);


            _trackport.SetupScene();
        }

        private static bool IsJointSecondaryMemberTowardsCamera(CModel model)
        {
            if (model.m_arrConnectionJoints == null) return true;
            if (model.m_arrConnectionJoints.Count != 1) return true;

            if (model.m_arrConnectionJoints[0].m_SecondaryMembers == null) return true;
            if (model.m_arrConnectionJoints[0].m_SecondaryMembers[0].NodeStart.Z < model.m_arrConnectionJoints[0].m_SecondaryMembers[0].NodeEnd.Z) return true;
            else return false;
        }

        public static void DrawFootingToTrackPort(Trackport3D _trackport, CModel model, DisplayOptions sDisplayOptions)
        {
            //DateTime start = DateTime.Now;

            // Color of Trackport
            _trackport.TrackportBackground = new SolidColorBrush(sDisplayOptions.backgroundColor);
            centerModel = true;
            //System.Diagnostics.Trace.WriteLine("Beginning: " + (DateTime.Now - start).TotalMilliseconds);
            if (model != null)
            {
                float fTempMax_X = 0f, fTempMin_X = 0f, fTempMax_Y = 0f, fTempMin_Y = 0f, fTempMax_Z = 0f, fTempMin_Z = 0f;

                if (model.m_arrMembers != null || model.m_arrGOPoints != null) // Some members or points must be defined in the model
                    CalculateModelLimitsWithCrsc(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

                fModel_Length_X = 0;
                fModel_Length_Y = 0;
                fModel_Length_Z = 0;
                Point3D pModelGeomCentre = Drawing3D.GetModelCentreWithCrsc(model, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);

                centerModelTransGr = new Transform3DGroup();
                centerModelTransGr.Children.Add(new TranslateTransform3D(-fTempMin_X, -fTempMin_Y, -fTempMin_Z));
                centerModelTransGr.Children.Add(new TranslateTransform3D(-fModel_Length_X / 2.0f, -fModel_Length_Y / 2.0f, -fModel_Length_Z / 2.0f));

                if (sDisplayOptions.RotateModelX != 0)
                {
                    AxisAngleRotation3D Rotation_LCS_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), sDisplayOptions.RotateModelX);
                    centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_x));
                }
                if (sDisplayOptions.RotateModelY != 0)
                {
                    if (!IsJointSecondaryMemberTowardsCamera(model)) sDisplayOptions.RotateModelY += 180;

                    AxisAngleRotation3D Rotation_LCS_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), sDisplayOptions.RotateModelY);
                    centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_y));
                }
                if (sDisplayOptions.RotateModelZ != 0)
                {
                    AxisAngleRotation3D Rotation_LCS_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), sDisplayOptions.RotateModelZ);
                    centerModelTransGr.Children.Add(new RotateTransform3D(Rotation_LCS_z));
                }

                // Global coordinate system - axis
                if (sDisplayOptions.bDisplayGlobalAxis) DrawGlobalAxis(_trackport.ViewPort, model, null);
                if (sDisplayOptions.bDisplayLocalMembersAxis) DrawModelMembersAxis(model, _trackport.ViewPort);

                Model3DGroup gr = new Model3DGroup();
                Model3D membersModel3D = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayMembers)
                    membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial,
                        sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);
                //System.Diagnostics.Trace.WriteLine("After CreateMembersModel3D: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateConnectionJointsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup foundationsModel3DGroup = null;
                if (sDisplayOptions.bDisplaySolidModel && sDisplayOptions.bDisplayFoundations) foundationsModel3DGroup = Drawing3D.CreateModelFoundationsModel3DGroup(model, sDisplayOptions);
                if (foundationsModel3DGroup != null) gr.Children.Add(foundationsModel3DGroup);

                bool displayOtherObjects3D = true;
                Model3DGroup othersModel3DGroup = null;
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model, sDisplayOptions);
                if (othersModel3DGroup != null) gr.Children.Add(othersModel3DGroup);
                //System.Diagnostics.Trace.WriteLine("After CreateModelOtherObjectsModel3DGroup: " + (DateTime.Now - start).TotalMilliseconds);

                Model3DGroup nodes3DGroup = null;
                if (sDisplayOptions.bDisplayNodes) nodes3DGroup = Drawing3D.CreateModelNodes_Model3DGroup(model);
                if (nodes3DGroup != null) gr.Children.Add(nodes3DGroup);

                // Pokus vyrobit lines 3D objekty
                // TO Ondrej - treba to nejako rozumne oddelit, aby sa wireframe nevytvaral a nepridaval 2x
                // Add WireFrame Model
                // Members
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMembersWireFrame)
                {
                    Model3DGroup lines; // linie ako 3D valcove plochy
                    if (membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                    Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                // Joints
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints && sDisplayOptions.bDisplayJointsWireFrame)
                {
                    Model3DGroup lines;  // linie ako 3D valcove plochy
                    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                // Foundations
                if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayFoundations && sDisplayOptions.bDisplayFoundationsWireFrame)
                {
                    Model3DGroup lines;  // linie ako 3D valcove plochy
                    if (foundationsModel3DGroup == null) foundationsModel3DGroup = Drawing3D.CreateModelFoundationsModel3DGroup(model, sDisplayOptions);
                    Drawing3D.DrawModelFoundationsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                    if (lines != null)
                        gr.Children.Add(lines); // Pridaj valcove plochy do modelu
                }

                Drawing3D.AddLightsToModel3D(gr, sDisplayOptions);

                if (centerModel)
                {
                    //translate transform to model center
                    ((Model3D)gr).Transform = centerModelTransGr;

                    double zoomFactor = 2.5;
                    if (fModel_Length_X > 2 * fModel_Length_Z) zoomFactor = 1.5;

                    Point3D cameraPosition = new Point3D(0, 0, MathF.Max(fModel_Length_X, fModel_Length_Y, fModel_Length_Z) * zoomFactor);
                    _trackport.PerspectiveCamera.Position = cameraPosition;
                    _trackport.PerspectiveCamera.LookDirection = new Vector3D(0, 0, -1);
                }
                else
                {
                    Point3D cameraPosition = Drawing3D.GetModelCameraPosition(model, 1, -(2 * fModel_Length_Y), 2 * fModel_Length_Z);
                    _trackport.PerspectiveCamera.Position = cameraPosition;
                    _trackport.PerspectiveCamera.LookDirection = Drawing3D.GetLookDirection(cameraPosition, pModelGeomCentre);
                }

                _trackport.Model = (Model3D)gr;

                // Add centerline member model
                if (sDisplayOptions.bDisplayMembersCenterLines && sDisplayOptions.bDisplayMembers) Drawing3D.DrawModelMembersCenterLines(model, _trackport.ViewPort);
                //System.Diagnostics.Trace.WriteLine("After DrawModelMembersCenterLines: " + (DateTime.Now - start).TotalMilliseconds);

                //// Add WireFrame Model
                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayMembers)
                //{
                //    Model3DGroup lines;
                //    if (membersModel3D == null) membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial, sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);
                //    Drawing3D.DrawModelMembersWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                //}
                ////System.Diagnostics.Trace.WriteLine("After DrawModelMembersinOneWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                //if (sDisplayOptions.bDisplayWireFrameModel && sDisplayOptions.bDisplayJoints)
                //{
                //    Model3DGroup lines;
                //    if (jointsModel3DGroup == null) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                //    Drawing3D.DrawModelConnectionJointsWireFrame(model, _trackport.ViewPort, sDisplayOptions, out lines);
                //}
                //System.Diagnostics.Trace.WriteLine("After DrawModelConnectionJointsWireFrame: " + (DateTime.Now - start).TotalMilliseconds);

                if (sDisplayOptions.bDisplayMembers && sDisplayOptions.bDisplayMemberDescription)
                {
                    //Drawing3D.CreateMembersDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                    Drawing3D.CreateMembersDescriptionModel3D_POKUS_MC(model, _trackport.ViewPort, sDisplayOptions); // To Ondrej POKUS 17.8.2019
                    //System.Diagnostics.Trace.WriteLine("After CreateMembersDescriptionModel3D: " + (DateTime.Now - start).TotalMilliseconds);
                }
                if (sDisplayOptions.bDisplayNodesDescription)
                {
                    Drawing3D.CreateNodesDescriptionModel3D(model, _trackport.ViewPort, sDisplayOptions);
                }
            }

            _trackport.SetupScene();
        }
        //-------------------------------------------------------------------------------------------------------------
        // Get model centre
        public static Point3D GetModelCentre(CModel model)
        {
            float fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y, fTempMax_Z, fTempMin_Z;

            CalculateModelLimitsWithoutCrsc(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

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
        public static Point3D GetModelCentreWithoutCrsc(CModel model, out float fModel_Length_X, out float fModel_Length_Y, out float fModel_Length_Z)
        {
            float fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y, fTempMax_Z, fTempMin_Z;

            CalculateModelLimitsWithoutCrsc(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

            fModel_Length_X = fTempMax_X - fTempMin_X;
            fModel_Length_Y = fTempMax_Y - fTempMin_Y;
            fModel_Length_Z = fTempMax_Z - fTempMin_Z;

            return new Point3D(fModel_Length_X / 2.0f, fModel_Length_Y / 2.0f, fModel_Length_Z / 2.0f);
        }
        public static Point3D GetModelCentreWithCrsc(CModel model, out float fModel_Length_X, out float fModel_Length_Y, out float fModel_Length_Z)
        {
            float fTempMax_X = 0f, fTempMin_X = 0f, fTempMax_Y = 0f, fTempMin_Y = 0f, fTempMax_Z = 0f, fTempMin_Z = 0f;

            if (model.m_arrMembers != null || model.m_arrGOPoints != null) // Some members or points must be defined in the model
                CalculateModelLimitsWithCrsc(model, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y, out fTempMax_Z, out fTempMin_Z);

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
                if (sDisplayOptions.bDisplayMembers)
                    membersModel3D = Drawing3D.CreateMembersModel3D(model, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial,
                        sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections, null, null, null, egcs);
                if (membersModel3D != null) gr.Children.Add(membersModel3D);

                Model3DGroup jointsModel3DGroup = null;
                if (sDisplayOptions.bDisplayJoints) jointsModel3DGroup = Drawing3D.CreateConnectionJointsModel3DGroup(model, sDisplayOptions);
                if (jointsModel3DGroup != null) gr.Children.Add(jointsModel3DGroup);

                Model3DGroup othersModel3DGroup = null;
                bool displayOtherObjects3D = true; // Temporary TODO
                if (displayOtherObjects3D) othersModel3DGroup = Drawing3D.CreateModelOtherObjectsModel3DGroup(model, sDisplayOptions);
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
            bool bColorsAccordingToMembers = true,
            bool bColorsAccordingToSections = false,
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
                        model.m_arrMembers[i].BIsGenerated &&
                        model.m_arrMembers[i].BIsDisplayed) // Member object is valid (not empty) and is active to display
                    {
                        if (model.m_arrMembers[i].CrScStart.CrScPointsOut != null) // CCrSc is abstract without geometrical properties (dimensions), only centroid line could be displayed
                        {
                            //Set Colors
                            if (bColorsAccordingToMembers && model.m_arrMembers[i].Color != null) shell = new SolidColorBrush(model.m_arrMembers[i].Color);
                            else if (bColorsAccordingToSections && model.m_arrMembers[i].CrScStart.CSColor != null) shell = new SolidColorBrush(model.m_arrMembers[i].CrScStart.CSColor);

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

                        // TO Ondrej - Ak zobrazujeme wireframe, mali by sme updatovat jeho body
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
        public static Model3DGroup CreateConnectionJointsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions, SolidColorBrush brushPlates = null, SolidColorBrush brushScrews = null, SolidColorBrush brushAnchors = null, SolidColorBrush brushWelds = null)
        {
            if (brushPlates == null) brushPlates = new SolidColorBrush(Colors.Gray);
            if (brushScrews == null) brushScrews = new SolidColorBrush(Colors.Red);
            if (brushAnchors == null) brushAnchors = new SolidColorBrush(Colors.Plum);
            if (brushWelds == null) brushWelds = new SolidColorBrush(Colors.Orange);

            Model3DGroup JointsModel3DGroup = null;

            if (cmodel.m_arrConnectionJoints != null) // Some joints exist
            {
                for (int i = 0; i < cmodel.m_arrConnectionJoints.Count; i++)
                {
                    if (cmodel.m_arrConnectionJoints[i] != null &&
                        cmodel.m_arrConnectionJoints[i].BIsGenerated &&
                        cmodel.m_arrConnectionJoints[i].BIsDisplayed)
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

                                    // Add plate anchors - only base plates
                                    if (cmodel.m_arrConnectionJoints[i].m_arrPlates[l] is CConCom_Plate_B_basic)
                                    {
                                        CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)cmodel.m_arrConnectionJoints[i].m_arrPlates[l];

                                        if (plate.AnchorArrangement != null &&
                                            plate.AnchorArrangement.Anchors != null &&
                                            plate.AnchorArrangement.Anchors.Length > 0)
                                        {
                                            Model3DGroup plateConnectorsModelGroup = new Model3DGroup();
                                            for (int m = 0; m < plate.AnchorArrangement.Anchors.Length; m++)
                                            {
                                                GeometryModel3D plateConnectorgeom = plate.AnchorArrangement.Anchors[m].CreateGeomModel3D(brushAnchors);
                                                plate.AnchorArrangement.Anchors[m].Visual_Connector = plateConnectorgeom;
                                                plateConnectorsModelGroup.Children.Add(plateConnectorgeom);
                                            }
                                            plateConnectorsModelGroup.Transform = plateGeom.Transform;
                                            if (sDisplayOptions.bDisplayConnectors)
                                            {
                                                JointModelGroup.Children.Add(plateConnectorsModelGroup);
                                            }
                                        }
                                    }

                                    // Add plate screws
                                    if (cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement != null &&
                                        cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws != null &&
                                        cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws.Length > 0)
                                    {
                                        Model3DGroup plateConnectorsModelGroup = new Model3DGroup();
                                        for (int m = 0; m < cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws.Length; m++)
                                        {
                                            GeometryModel3D plateConnectorgeom = cmodel.m_arrConnectionJoints[i].m_arrPlates[l].ScrewArrangement.Screws[m].CreateGeomModel3D(brushScrews);
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
                        bool bUseAdditionalConnectors = false; // Spojovacie prvky mimo tychto ktore su viazane na plechy (plates) napr spoj pomocou screws priamo medzi nosnikmi bez plechu (tiahla - bracing)

                        if (bUseAdditionalConnectors && cmodel.m_arrConnectionJoints[i].m_arrConnectors != null)
                        {
                            for (int l = 0; l < cmodel.m_arrConnectionJoints[i].m_arrConnectors.Length; l++)
                            {
                                if (cmodel.m_arrConnectionJoints[i].m_arrConnectors[l] != null &&
                                cmodel.m_arrConnectionJoints[i].m_arrConnectors[l].m_pControlPoint != null &&
                                cmodel.m_arrConnectionJoints[i].m_arrConnectors[l].BIsDisplayed == true) // Bolt object is valid (not empty) and should be displayed
                                {
                                    JointModelGroup.Children.Add(cmodel.m_arrConnectionJoints[i].m_arrConnectors[l].CreateGeomModel3D(brushScrews)); // Add bolt 3D model to the model group
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
                    }
                } //for joints
            }
            return JointsModel3DGroup;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Loading objects model 3d group
        public static Model3DGroup CreateModelLoadObjectsModel3DGroup(CLoadCase selectedLoadCase, DisplayOptions sDisplayOptions)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (selectedLoadCase != null)
            {
                if (selectedLoadCase.NodeLoadsList != null && sDisplayOptions.bDisplayNodalLoads) // Some nodal loads exist
                {
                    // Model Groups of Nodal Loads
                    for (int i = 0; i < selectedLoadCase.NodeLoadsList.Count; i++)
                    {
                        if (selectedLoadCase.NodeLoadsList[i] != null && selectedLoadCase.NodeLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            model3D_group.Children.Add(selectedLoadCase.NodeLoadsList[i].CreateM_3D_G_Load(sDisplayOptions.DisplayIn3DRatio)); // Add to the model group

                            // Set load for all assigned nodes

                        }
                    }
                }

                if (selectedLoadCase.MemberLoadsList != null && sDisplayOptions.bDisplayMemberLoads) // Some member loads exist
                {
                    // Model Groups of Member Loads
                    for (int i = 0; i < selectedLoadCase.MemberLoadsList.Count; i++)
                    {
                        if (selectedLoadCase.MemberLoadsList[i] != null && selectedLoadCase.MemberLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            if ((sDisplayOptions.bDisplayMemberLoads_Girts && selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eG) ||
                               (sDisplayOptions.bDisplayMemberLoads_Purlins && selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eP) ||
                               (sDisplayOptions.bDisplayMemberLoads_Columns && selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eC) ||
                               (sDisplayOptions.bDisplayMemberLoads_Frames &&
                               (selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eMC ||
                               selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eMR ||
                               selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eEC ||
                               selectedLoadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eER)
                               ))
                            {
                                Model3DGroup model_gr = new Model3DGroup();
                                model_gr = selectedLoadCase.MemberLoadsList[i].CreateM_3D_G_Load(sDisplayOptions.bDisplaySolidModel, sDisplayOptions.DisplayIn3DRatio);
                                // Transform modelgroup from LCS to GCS
                                model_gr = selectedLoadCase.MemberLoadsList[i].Transform3D_OnMemberEntity_fromLCStoGCS(model_gr, selectedLoadCase.MemberLoadsList[i].Member, selectedLoadCase.MemberLoadsList[i].ELoadCS == ELoadCoordSystem.eLCS);

                                model3D_group.Children.Add(model_gr); // Add member load to the model group

                                // Set member load for all assigned members
                            }
                        }
                    }
                }

                if (selectedLoadCase.SurfaceLoadsList != null && sDisplayOptions.bDisplaySurfaceLoads) // Some surface loads exist
                {
                    // Model Groups of Surface Loads
                    for (int i = 0; i < selectedLoadCase.SurfaceLoadsList.Count; i++)
                    {
                        if (selectedLoadCase.SurfaceLoadsList[i] != null && selectedLoadCase.SurfaceLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            Model3DGroup model_gr = new Model3DGroup();
                            model_gr = selectedLoadCase.SurfaceLoadsList[i].CreateM_3D_G_Load(sDisplayOptions.DisplayIn3DRatio);

                            model3D_group.Children.Add(model_gr); // Add surface load to the model group
                        }
                    }
                }
            }
            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create foundation model objects model 3d group
        public static Model3DGroup CreateModelFoundationsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (cmodel.m_arrFoundations != null && sDisplayOptions.bDisplayFoundations)
            {
                //SolidColorBrush brushFoundations = new SolidColorBrush(Colors.Gray);
                float fbrushOpacity = 0.4f;

                // Model Groups of Volumes
                for (int i = 0; i < cmodel.m_arrFoundations.Count; i++)
                {
                    if (cmodel.m_arrFoundations[i] != null &&
                        cmodel.m_arrFoundations[i].m_pControlPoint != null &&
                        cmodel.m_arrFoundations[i].BIsDisplayed == true) // Foundation object is valid (not empty) and should be displayed
                    {
                        if (sDisplayOptions.bDisplayReinforcementBars)
                        {
                            // TODO - Ondrej - vykreslujeme vystuz hore a dole v smere x a y, takze su to 4 zoznamy, asi by sa do dalo refaktorovat
                            // Top layer - x
                            if (cmodel.m_arrFoundations[i].Top_Bars_x != null)
                            {
                                for (int j = 0; j < cmodel.m_arrFoundations[i].Top_Bars_x.Count; j++) // Each bar in the list
                                {
                                    if (cmodel.m_arrFoundations[i].Top_Bars_x[j].m_pControlPoint != null &&
                                        cmodel.m_arrFoundations[i].Top_Bars_x[j].BIsDisplayed)
                                    {
                                        GeometryModel3D modelReinforcementBar = cmodel.m_arrFoundations[i].Top_Bars_x[j].CreateGeomModel3D(/*brushReinforcement*/ /*TEMPORARY*/ cmodel.m_arrFoundations[i].GetFoundationTransformGroup());
                                        model3D_group.Children.Add(modelReinforcementBar); // Add reinforcement bar to the model group
                                    }
                                }
                            }

                            // Top layer - y
                            if (cmodel.m_arrFoundations[i].Top_Bars_y != null)
                            {
                                for (int j = 0; j < cmodel.m_arrFoundations[i].Top_Bars_y.Count; j++) // Each bar in the list
                                {
                                    if (cmodel.m_arrFoundations[i].Top_Bars_y[j].m_pControlPoint != null &&
                                        cmodel.m_arrFoundations[i].Top_Bars_y[j].BIsDisplayed)
                                    {
                                        GeometryModel3D modelReinforcementBar = cmodel.m_arrFoundations[i].Top_Bars_y[j].CreateGeomModel3D(/*brushReinforcement*/ /*TEMPORARY*/ cmodel.m_arrFoundations[i].GetFoundationTransformGroup());
                                        model3D_group.Children.Add(modelReinforcementBar); // Add reinforcement bar to the model group
                                    }
                                }
                            }

                            // Bottom layer - x
                            if (cmodel.m_arrFoundations[i].Bottom_Bars_x != null)
                            {
                                for (int j = 0; j < cmodel.m_arrFoundations[i].Bottom_Bars_x.Count; j++) // Each bar in the list
                                {
                                    if (cmodel.m_arrFoundations[i].Bottom_Bars_x[j].m_pControlPoint != null &&
                                        cmodel.m_arrFoundations[i].Bottom_Bars_x[j].BIsDisplayed)
                                    {
                                        GeometryModel3D modelReinforcementBar = cmodel.m_arrFoundations[i].Bottom_Bars_x[j].CreateGeomModel3D(/*brushReinforcement*/ /*TEMPORARY*/ cmodel.m_arrFoundations[i].GetFoundationTransformGroup());
                                        model3D_group.Children.Add(modelReinforcementBar); // Add reinforcement bar to the model group
                                    }
                                }
                            }

                            // Bottom layer - y
                            if (cmodel.m_arrFoundations[i].Bottom_Bars_y != null)
                            {
                                for (int j = 0; j < cmodel.m_arrFoundations[i].Bottom_Bars_y.Count; j++) // Each bar in the list
                                {
                                    if (cmodel.m_arrFoundations[i].Bottom_Bars_y[j].m_pControlPoint != null &&
                                        cmodel.m_arrFoundations[i].Bottom_Bars_y[j].BIsDisplayed)
                                    {
                                        GeometryModel3D modelReinforcementBar = cmodel.m_arrFoundations[i].Bottom_Bars_y[j].CreateGeomModel3D(/*brushReinforcement*/ /*TEMPORARY*/ cmodel.m_arrFoundations[i].GetFoundationTransformGroup());
                                        model3D_group.Children.Add(modelReinforcementBar); // Add reinforcement bar to the model group
                                    }
                                }
                            }
                        }

                        //!!!!!!! POZOR PRIEHLADNOST ZAVISI NA PORADI VYKRESLOVANIA OBJEKTOV!!!!!!!!!
                        // Najprv vykreslit to co je "skryte vo vnutri - vyztuz" a az potom vonkajsi hlavny objekt zakladu
                        GeometryModel3D model = cmodel.m_arrFoundations[i].CreateGeomModel3D(/*brushFoundations*/ fbrushOpacity);
                        model3D_group.Children.Add(model); // Add foundation to the model group
                    }
                }
            }

            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create slab model objects model 3d group
        public static Model3DGroup CreateModelSlabsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (cmodel.m_arrSlabs != null && sDisplayOptions.bDisplayFloorSlab)
            {
                float fbrushOpacity = 0.3f;

                // Model Groups of Volumes
                for (int i = 0; i < cmodel.m_arrSlabs.Count; i++)
                {
                    if (cmodel.m_arrSlabs[i] != null &&
                        cmodel.m_arrSlabs[i].m_pControlPoint != null &&
                        cmodel.m_arrSlabs[i].BIsDisplayed == true) // Foundation object is valid (not empty) and should be displayed
                    {
                        // TODO - pridat vykreslovanie vystuze v doske - velmi nizka priorita, navyse moze byt dost narocne na grafiku

                        //!!!!!!! POZOR PRIEHLADNOST ZAVISI NA PORADI VYKRESLOVANIA OBJEKTOV!!!!!!!!!
                        // Najprv vykreslit to co je "skryte vo vnutri - vyztuz" a az potom vonkajsi hlavny objekt betonovej dosky
                        GeometryModel3D model = cmodel.m_arrSlabs[i].CreateGeomModel3D(/*brushFoundations*/ fbrushOpacity);
                        model3D_group.Children.Add(model); // Add slab to the model group
                    }
                }
            }

            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create saw cuts model objects model 3d group
        public static Model3DGroup CreateModelSawCutsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (cmodel.m_arrSawCuts != null && sDisplayOptions.bDisplaySawCuts)
            {
                float fbrushOpacity = 0.3f;

                // Model Groups of Volumes
                for (int i = 0; i < cmodel.m_arrSawCuts.Count; i++)
                {
                    if (cmodel.m_arrSawCuts[i] != null &&
                        cmodel.m_arrSawCuts[i].m_pControlPoint != null &&
                        cmodel.m_arrSawCuts[i].BIsDisplayed == true) // Foundation object is valid (not empty) and should be displayed
                    {
                        GeometryModel3D model = cmodel.m_arrSawCuts[i].GetSawCutModel(Colors.DarkGoldenrod);
                        model3D_group.Children.Add(model); // Add saw cut to the model group
                    }
                }
            }

            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create control joints model objects model 3d group
        public static Model3DGroup CreateModelControlJointsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            if (cmodel.m_arrControlJoints != null && sDisplayOptions.bDisplayControlJoints)
            {
                float fbrushOpacity = 0.3f;

                // Model Groups of Volumes
                for (int i = 0; i < cmodel.m_arrControlJoints.Count; i++)
                {
                    if (cmodel.m_arrControlJoints[i] != null &&
                        cmodel.m_arrControlJoints[i].m_pControlPoint != null &&
                        cmodel.m_arrControlJoints[i].BIsDisplayed == true) // Foundation object is valid (not empty) and should be displayed
                    {
                        GeometryModel3D model = cmodel.m_arrControlJoints[i].GetControlJointModel(Colors.DarkMagenta);
                        model3D_group.Children.Add(model); // Add saw cut to the model group
                    }
                }
            }

            return model3D_group;
        }

        //-------------------------------------------------------------------------------------------------------------
        // Create Other model objects model 3d group
        public static Model3DGroup CreateModelOtherObjectsModel3DGroup(CModel cmodel, DisplayOptions sDisplayOptions)
        {
            Model3DGroup model3D_group = new Model3DGroup();

            // Physical 3D Model

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

            // Structural Model

            if (cmodel.m_arrNSupports != null && sDisplayOptions.bDisplayNodalSupports) // Some nodal supports exist
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
        public static void DrawGlobalAxis(Viewport3D viewPort, CModel model, Transform3D trans)
        {
            float flineThickness = 3;
            // Global coordinate system - axis
            Point3D pGCS_centre = new Point3D(0, 0, 0);
            Point3D pAxisX = new Point3D(1, 0, 0);
            Point3D pAxisY = new Point3D(0, 1, 0);
            Point3D pAxisZ = new Point3D(0, 0, 1);

            bool useScreenSpaceLines3D = false;
            bool useWireLine = false;
            bool useLinesVisual3D = true;

            if (useScreenSpaceLines3D)
            {
                ScreenSpaceLines3D sAxisX_3D = new ScreenSpaceLines3D();
                ScreenSpaceLines3D sAxisY_3D = new ScreenSpaceLines3D();
                ScreenSpaceLines3D sAxisZ_3D = new ScreenSpaceLines3D();
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
                if (trans != null)
                {
                    sAxisX_3D.Transform = trans;
                    sAxisY_3D.Transform = trans;
                    sAxisZ_3D.Transform = trans;
                }
                viewPort.Children.Add(sAxisX_3D);
                viewPort.Children.Add(sAxisY_3D);
                viewPort.Children.Add(sAxisZ_3D);
            }

            if (useWireLine)
            {
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

                if (trans != null)
                {
                    wX.Transform = trans;
                    wY.Transform = trans;
                    wZ.Transform = trans;
                }
                viewPort.Children.Add(wX);
                viewPort.Children.Add(wY);
                viewPort.Children.Add(wZ);
            }

            if (useLinesVisual3D)
            {
                LinesVisual3D lX = new LinesVisual3D();
                LinesVisual3D lY = new LinesVisual3D();
                LinesVisual3D lZ = new LinesVisual3D();
                lX.Points.Add(pGCS_centre);
                lX.Points.Add(pAxisX);
                lX.Color = Colors.Red;
                lX.Thickness = flineThickness;

                lY.Points.Add(pGCS_centre);
                lY.Points.Add(pAxisY);
                lY.Color = Colors.Green;
                lY.Thickness = flineThickness;

                lZ.Points.Add(pGCS_centre);
                lZ.Points.Add(pAxisZ);
                lZ.Color = Colors.Blue;
                lZ.Thickness = flineThickness;

                if (trans != null)
                {
                    lX.Transform = trans;
                    lY.Transform = trans;
                    lZ.Transform = trans;
                }
                viewPort.Children.Add(lX);
                viewPort.Children.Add(lY);
                viewPort.Children.Add(lZ);
            }
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

                //priprava na centrovanie modelu
                if (centerModel)
                {
                    lines.Transform = centerModelTransGr;
                }

                viewPort.Children.Add(lines);
            }
        }

        // Draw Members Centerlines
        public static void DrawModelMembersAxis(CModel model, Viewport3D viewPort)
        {
            double axisL = 0.5;
            // Members
            if (model.m_arrMembers == null) return;

            foreach (CMember m in model.m_arrMembers)
            {
                if (m != null &&
                    m.NodeStart != null &&
                    m.NodeEnd != null &&
                    m.CrScStart != null &&
                    m.BIsDisplayed) // Member object is valid (not empty) and is active to be displayed
                {

                    Transform3DGroup transform = m.CreateTransformCoordGroup(m, true);
                    Point3D pC = new Point3D(0 + m.FLength * 0.35, 0, 0);

                    Point3D pAxisX = new Point3D(pC.X + axisL, pC.Y, pC.Z);
                    Point3D pAxisY = new Point3D(pC.X, pC.Y + axisL, pC.Z);
                    Point3D pAxisZ = new Point3D(pC.X, pC.Y, pC.Z + axisL);

                    pC = transform.Transform(pC);
                    pAxisX = transform.Transform(pAxisX);
                    pAxisY = transform.Transform(pAxisY);
                    pAxisZ = transform.Transform(pAxisZ);

                    DrawAxis(viewPort, pC, pAxisX, pAxisY, pAxisZ);
                }
            }
        }

        // Draw Axis
        public static void DrawAxis(Viewport3D viewPort, Point3D pGCS_centre, Point3D pAxisX, Point3D pAxisY, Point3D pAxisZ)
        {
            float flineThickness = 3;
            // axis     
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

            if (centerModel)
            {
                wX.Transform = centerModelTransGr;
                wY.Transform = centerModelTransGr;
                wZ.Transform = centerModelTransGr;
            }

            viewPort.Children.Add(wX);
            viewPort.Children.Add(wY);
            viewPort.Children.Add(wZ);
        }

        // Add all members in one wireframe collection
        public static void DrawModelMembersWireFrame(CModel model, Viewport3D viewPort, DisplayOptions sDiplayOptions, out Model3DGroup cylinders)
        {
            cylinders = null;

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

                if (sDiplayOptions.bTransformScreenLines3DToCylinders3D)
                {
                    cylinders = new Model3DGroup();

                    for (int i = 0; i < wireFramePoints.Count / 2; i++)
                    {
                        GeometryModel3D cylinder = Get3DLineReplacement(sDiplayOptions.wireFrameColor, sDiplayOptions.fWireFrameLineThickness, wireFramePoints[i * 2], wireFramePoints[i * 2 + 1]);
                        cylinders.Children.Add(cylinder);
                    }
                }
                else
                {
                    AddLineToViewPort(wireFramePoints, sDiplayOptions, viewPort);
                }

            }
        }

        // Draw Model Connection Joints Wire Frame
        public static void DrawModelConnectionJointsWireFrame(CModel model, Viewport3D viewPort, DisplayOptions sDisplayOptions, out Model3DGroup cylinders)
        {
            cylinders = null;

            //zaviedol som maxPoints z dovodu OutOfMemoryException - pocet bodov ide bezne aj cez 700.000
            int maxPoints = 100000;
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
                                List<Point3D> jointPlatePoints = new List<Point3D>();
                                if (sDisplayOptions.bDisplayPlatesWireFrame)
                                {
                                    // Create WireFrame in LCS
                                    jointPlatePoints = model.m_arrConnectionJoints[i].m_arrPlates[j].CreateWireFrameModel().Points.ToList();
                                }

                                if (sDisplayOptions.bDisplayConnectorsWireFrame)
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
                                if (model.m_arrConnectionJoints[i].m_arrPlates[j].Visual_Plate != null)
                                {
                                    var transPoints_Plate = jointPlatePoints.Select(p => model.m_arrConnectionJoints[i].m_arrPlates[j].Visual_Plate.Transform.Transform(p));
                                    jointPoints.AddRange(transPoints_Plate);
                                }
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

                    if (jointsWireFramePoints.Count > maxPoints)
                    {
                        if (sDisplayOptions.bTransformScreenLines3DToCylinders3D)
                        {
                            if (cylinders == null) cylinders = new Model3DGroup();
                            for (int j = 0; j < jointsWireFramePoints.Count / 2; j++)
                            {
                                GeometryModel3D cylinder = Get3DLineReplacement(sDisplayOptions.wireFrameColor, sDisplayOptions.fWireFrameLineThickness, jointsWireFramePoints[j * 2], jointsWireFramePoints[j * 2 + 1]);
                                cylinders.Children.Add(cylinder);
                            }
                        }
                        else
                        {
                            AddLineToViewPort(jointsWireFramePoints, sDisplayOptions, viewPort);
                        }
                        jointsWireFramePoints.Clear();
                    }
                }

                if (sDisplayOptions.bTransformScreenLines3DToCylinders3D)
                {
                    if (cylinders == null) cylinders = new Model3DGroup();
                    for (int j = 0; j < jointsWireFramePoints.Count / 2; j++)
                    {
                        GeometryModel3D cylinder = Get3DLineReplacement(sDisplayOptions.wireFrameColor, sDisplayOptions.fWireFrameLineThickness, jointsWireFramePoints[j * 2], jointsWireFramePoints[j * 2 + 1]);
                        cylinders.Children.Add(cylinder);
                    }
                }
                else
                {
                    AddLineToViewPort(jointsWireFramePoints, sDisplayOptions, viewPort);
                }
            }
        }

        // Add all foundations in one wireframe collection of ScreenSpaceLines3D
        public static void DrawModelFoundationsWireFrame(CModel model, Viewport3D viewPort, DisplayOptions sDisplayOptions, out Model3DGroup cylinders)
        {
            cylinders = null;

            // Foundations - Wire Frame
            if (model.m_arrFoundations != null)
            {
                List<Point3D> wireFramePoints = new List<Point3D>();
                for (int i = 0; i < model.m_arrFoundations.Count; i++) // Per each foundation
                {
                    if (model.m_arrFoundations[i] != null &&
                        model.m_arrFoundations[i].BIsDisplayed) // Foundation object is valid (not empty) and is active to be displayed
                    {
                        wireFramePoints.AddRange(model.m_arrFoundations[i].WireFramePoints);
                    }
                }

                if (sDisplayOptions.bTransformScreenLines3DToCylinders3D)
                {
                    cylinders = new Model3DGroup();
                    for (int i = 0; i < wireFramePoints.Count / 2; i++)
                    {
                        GeometryModel3D cylinder = Get3DLineReplacement(sDisplayOptions.wireFrameColor, sDisplayOptions.fWireFrameLineThickness, wireFramePoints[i * 2], wireFramePoints[i * 2 + 1]);
                        cylinders.Children.Add(cylinder);
                    }
                }
                else
                {
                    AddLineToViewPort(wireFramePoints, sDisplayOptions, viewPort);
                }
            }
        }

        // Add all slabs in one wireframe collection of ScreenSpaceLines3D
        public static void DrawModelSlabsWireFrame(CModel model, Viewport3D viewPort, DisplayOptions sDisplayOptions, out Model3DGroup cylinders)
        {
            cylinders = null;

            // Slabs - Wire Frame
            if (model.m_arrSlabs != null)
            {
                List<Point3D> wireFramePoints = new List<Point3D>();
                for (int i = 0; i < model.m_arrSlabs.Count; i++) // Per each slab
                {
                    if (model.m_arrSlabs[i] != null &&
                        model.m_arrSlabs[i].BIsDisplayed) // Slab object is valid (not empty) and is active to be displayed
                    {
                        wireFramePoints.AddRange(model.m_arrSlabs[i].WireFramePoints);
                    }
                }

                if (sDisplayOptions.bTransformScreenLines3DToCylinders3D)
                {
                    cylinders = new Model3DGroup();

                    for (int i = 0; i < wireFramePoints.Count / 2; i++)
                    {
                        GeometryModel3D cylinder = Get3DLineReplacement(sDisplayOptions.wireFrameColor, sDisplayOptions.fWireFrameLineThickness, wireFramePoints[i * 2], wireFramePoints[i * 2 + 1]);
                        cylinders.Children.Add(cylinder);
                    }
                }
                else
                {
                    AddLineToViewPort(wireFramePoints, sDisplayOptions, viewPort);
                }
            }
        }

        private static void AddLineToViewPort(List<Point3D> points, DisplayOptions opts, Viewport3D viewPort)
        {
            //TO Mato - tieto prepinace sa vyskytuju castejsie...je potrebne poprepinat a vyskusat jednotlive typy ciar, co sa tyka zobrazovania a tiez performance
            bool useWireLines = false;
            bool useScreenSpaceLines = true;
            bool useLinesVisual3D = false;

            if (useWireLines)
            {
                WireLines wl = new WireLines();
                wl.Lines = new Point3DCollection(points);
                wl.Color = opts.wireFrameColor;
                wl.Thickness = opts.fWireFrameLineThickness;
                if (centerModel) { wl.Transform = centerModelTransGr; }
                viewPort.Children.Add(wl);
            }
            if (useScreenSpaceLines)
            {
                //ScreenSpaceLines are much slower = performance issue                                        
                ScreenSpaceLines3D line_3D = new ScreenSpaceLines3D(opts.wireFrameColor, opts.fWireFrameLineThickness); // Just one collection for all members
                line_3D.Points = new Point3DCollection(points);
                if (centerModel) { line_3D.Transform = centerModelTransGr; }
                viewPort.Children.Add(line_3D);
            }
            if (useLinesVisual3D)
            {
                LinesVisual3D wl = new LinesVisual3D();
                wl.Points = new Point3DCollection(points);
                wl.Color = opts.wireFrameColor;
                wl.Thickness = opts.fWireFrameLineThickness;
                if (centerModel) { wl.Transform = centerModelTransGr; }
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
        public static void CreateMembersDescriptionModel3D(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
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

                        string sTextToDisplay = GetMemberDisplayText(displayOptions, model.m_arrMembers[i]);

                        TextBlock tb = new TextBlock();
                        tb.Text = sTextToDisplay;
                        tb.FontFamily = new FontFamily("Arial");
                        float fTextBlockVerticalSize = displayOptions.fMemberDescriptionTextFontSize / 100f; //  0.1f; // To Ondrej - da sa velkost textu nejako prepojit s velkostou realne textu v 3D, napriklad vyska fontu 12 by bola 0.12 metra ???
                        float fTextBlockVerticalSizeFactor = 0.8f;
                        float fTextBlockHorizontalSizeFactor = 0.3f;

                        // Tieto nastavenia sa nepouziju
                        tb.FontStretch = FontStretches.UltraCondensed;
                        tb.FontStyle = FontStyles.Normal;
                        tb.FontWeight = FontWeights.Thin;
                        tb.Foreground = new SolidColorBrush(displayOptions.MemberDescriptionTextColor);
                        tb.Background = new SolidColorBrush(displayOptions.backgroundColor); // TODO - In case that solid model is displayed it is reasonable to use black backround of text or offset texts usig cross-section dimension

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

                        Vector3D over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
                        Vector3D up = new Vector3D(0, 0, fTextBlockVerticalSizeFactor);

                        SetLabelsUpAndOverVectors(displayOptions, fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, out over, out up);
                        // Create text
                        textlabel = CreateTextLabel3D(tb, false, fTextBlockVerticalSize, pTextPosition, over, up);

                        if (centerModel)
                        {
                            textlabel.Transform = centerModelTransGr;
                        }
                        viewPort.Children.Add(textlabel);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // 17.8.2019
        // TO Ondrej - pokus kreslit text do LCS a potom ho presunut do GCS s tym ze vektrory pre smerovanie textu sa nastavia podla pohladu

        public static void CreateMembersDescriptionModel3D_POKUS_MC(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
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

                        string sTextToDisplay = GetMemberDisplayText(displayOptions, model.m_arrMembers[i]);

                        TextBlock tb = new TextBlock();
                        tb.Text = sTextToDisplay;
                        tb.FontFamily = new FontFamily("Arial");
                        float fTextBlockVerticalSize = displayOptions.fMemberDescriptionTextFontSize / 100f;
                        float fTextBlockVerticalSizeFactor = 0.8f;
                        float fTextBlockHorizontalSizeFactor = 0.3f;

                        // Tieto nastavenia sa nepouziju
                        tb.FontStretch = FontStretches.UltraCondensed;
                        tb.FontStyle = FontStyles.Normal;
                        tb.FontWeight = FontWeights.Thin;
                        tb.Foreground = new SolidColorBrush(displayOptions.MemberDescriptionTextColor); // TO Ondrej - zda sa mi ze u textu je mozne nastavit rozne farby pre vypln a outline pismenok - vieme to nejako urobit ???
                        tb.Background = new SolidColorBrush(displayOptions.backgroundColor); // TODO - In case that solid model is displayed it is reasonable to use black backround of text or offset texts usig cross-section dimension

                        float fRelativePositionFactor = 0.4f; //(0-1) // Relative position of member description on member

                        // TODO Ondrej - vylepsit vykreslovanie a odsadenie
                        // Teraz to kreslime priamo do GCS, ale asi by bolo lepsie kreslit do LCS a potom text transformovat
                        // pripadne vypocitat podla orientacie pruta vector z hodnot delta ako je prut orientovany v priestore a podla toho nastavit
                        // hodnoty vektorov pre funkciu CreateTextLabel3D) :over" and "up"
                        // Do user options by som dal nastavenie ci sa ma text kreslit horizontalne na obrazovke v rovine obrazovky
                        // alebo podla polohy pruta (rovnobezne s lokalnou osou x pruta) horizontalne alebo vertikalne podla orientacie osi x pruta v lokanych rovinach x,y alebo x,z pruta

                        // Zistime v akej vzajomnej pozicii su voci sebe osovy system pruta v LCS a pohlad
                        // Podla toho urcime ci vykreslujeme text pruta pre LCS pohlad x, y alebo z

                        // VIEW AXIS
                        Vector3D viewVector;
                        Vector3D viewHorizontalVector;
                        Vector3D viewVerticalVector;

                        if (displayOptions.ModelView == (int)EModelViews.BACK)
                        {
                            viewVector = new Vector3D(0, -1, 0);
                            viewHorizontalVector = new Vector3D(-1, 0, 0);
                            viewVerticalVector = new Vector3D(0, 0, 1);
                        }
                        else if (displayOptions.ModelView == (int)EModelViews.LEFT)
                        {
                            viewVector = new Vector3D(1, 0, 0);
                            viewHorizontalVector = new Vector3D(0, -1, 0);
                            viewVerticalVector = new Vector3D(0, 0, 1);
                        }
                        else if (displayOptions.ModelView == (int)EModelViews.RIGHT)
                        {
                            viewVector = new Vector3D(-1, 0, 0);
                            viewHorizontalVector = new Vector3D(0, 1, 0);
                            viewVerticalVector = new Vector3D(0, 0, 1);
                        }
                        else if (displayOptions.ModelView == (int)EModelViews.TOP)
                        {
                            viewVector = new Vector3D(0, 0, -1);
                            viewHorizontalVector = new Vector3D(0, 1, 0);
                            viewVerticalVector = new Vector3D(-1, 0, 0);
                        }
                        else //if (displayOptions.ModelView == (int)EModelViews.FRONT) // Front or default view
                        {
                            viewVector = new Vector3D(0, 1, 0);
                            viewHorizontalVector = new Vector3D(1, 0, 0);
                            viewVerticalVector = new Vector3D(0, 0, 1);
                        }

                        // Ziskame transformaciu pruta z LCS do GCS
                        // Neviem ci tato funkcia vracia spravne hodnoty, este by sa to dalo ziskat z transformacie 3D modelu pruta
                        Transform3DGroup transform = model.m_arrMembers[i].CreateTransformCoordGroup(model.m_arrMembers[i], true);

                        // TODO - toto kodovanie moze byt podla normaly textu alebo to mozeme urobit aj podla LCS rovin v ktorych text lezi
                        int iTextNormalInLCSCode; // 0 - text pre LCS x (rovina yz), 1 - text pre LCS y (rovina xz), 2 - text pre LCS z (rovina xy)

                        Vector3D memberAxis_xInLCS = new Vector3D(1, 0, 0);
                        Vector3D memberAxis_yInLCS = new Vector3D(0, 1, 0);
                        Vector3D memberAxis_zInLCS = new Vector3D(0, 0, 1);

                        Vector3D memberLCSAxis_xInGCS;
                        Vector3D memberLCSAxis_yInGCS;
                        Vector3D memberLCSAxis_zInGCS;

                        // Transformujeme vektory LCS os do GCS systemu, transformacna matica je vstup lebo ju potrebujeme este dalej, pripadne by mohla byt vystupny parameter
                        TransformVectorsFromLCSAxisToGCSAxis(model.m_arrMembers[i], transform, memberAxis_xInLCS, out memberLCSAxis_xInGCS);
                        TransformVectorsFromLCSAxisToGCSAxis(model.m_arrMembers[i], transform, memberAxis_yInLCS, out memberLCSAxis_yInGCS);
                        TransformVectorsFromLCSAxisToGCSAxis(model.m_arrMembers[i], transform, memberAxis_zInLCS, out memberLCSAxis_zInGCS);

                        // Vztah LCS osi a vektora pohladu
                        // TO ONDREJ
                        // Tu vieme vektory LCS pruta v GCS a vektory os smeru pohladu v GCS, potrebujeme ziskat vektora LCS pruta v systeme pohladu X - horizontalna, Y - v smere pohladu, Z vertikalna
                        // PODOBNY PROBLEM AKO U ZATAZENIA
                        // Vieme vztah LCS os ku GCS a vieme vztah VIEW AXIS ku GCS, potrebujeme zistit vztah medzi LCS pruta a VIEW AXIS

                        //Vector3D memberLCSAxis_xInView = new Vector3D(memberLCSAxis_xInGCS.X * viewHorizontalVector.X, memberLCSAxis_xInGCS.Y * viewHorizontalVector.Y, memberLCSAxis_xInGCS.Z * viewHorizontalVector.Z);
                        //Vector3D memberLCSAxis_yInView = new Vector3D(memberLCSAxis_yInGCS.X * viewVector.X, memberLCSAxis_yInGCS.Y * viewVector.Y, memberLCSAxis_yInGCS.Z * viewVector.Z);
                        //Vector3D memberLCSAxis_zInView = new Vector3D(memberLCSAxis_zInGCS.X * viewVerticalVector.X, memberLCSAxis_zInGCS.Y * viewVerticalVector.Y, memberLCSAxis_zInGCS.Z * viewVerticalVector.Z);

                        Matrix3D matrixViewInGCS = new Matrix3D(viewHorizontalVector.X, viewHorizontalVector.Y, viewHorizontalVector.Z, 0,
                            viewVector.X, viewVector.Y, viewVector.Z, 0,
                            viewVerticalVector.X, viewVerticalVector.Y, viewVerticalVector.Z, 0,
                            0, 0, 0, 1);

                        // Vytvorime inverznu maticu k matici pre View
                        Matrix3D matrixViewInGCS_Inverse = matrixViewInGCS.Inverse();

                        // Prevedieme osi pruta v GCS do osi pre View
                        Vector3D memberLCSAxis_xInView = memberLCSAxis_xInGCS * matrixViewInGCS_Inverse;
                        Vector3D memberLCSAxis_yInView = memberLCSAxis_yInGCS * matrixViewInGCS_Inverse;
                        Vector3D memberLCSAxis_zInView = memberLCSAxis_zInGCS * matrixViewInGCS_Inverse;

                        // Urcenie pozicie LCS pruta voci smeru pohladu (Y - view)

                        if (MathF.d_equal(memberLCSAxis_xInView.Y, 1) ||
                            MathF.d_equal(memberLCSAxis_xInView.Y, -1)
                            ) // Prut (osa x in LCS) smeruje kolmo na smer pohladu
                        {
                            // Text kreslime do roviny LCS yz
                            iTextNormalInLCSCode = 0;

                            // TODO - podla orientacie vektora mozeme nastavit vector over pre text
                        }
                        else if (MathF.d_equal(memberLCSAxis_yInView.Y, 1) ||
                            MathF.d_equal(memberLCSAxis_yInView.Y, -1)
                            ) // Lokalna osa y pruta v LCS smeruje kolmo na smer pohladu
                        {
                            // Text kreslime do roviny LCS xz
                            iTextNormalInLCSCode = 1;

                            // TODO - podla orientacie vektora mozeme nastavit vector over pre text
                        }
                        else if (MathF.d_equal(memberLCSAxis_zInView.Y, 1) ||
                            MathF.d_equal(memberLCSAxis_zInView.Y, -1)
                            ) // Lokalna osa z pruta v LCS smeruje kolmo na smer pohladu
                        {
                            // Text kreslime do roviny LCS xy
                            iTextNormalInLCSCode = 2;

                            // TODO - podla orientacie vektora mozeme nastavit vector over pre text
                        }
                        else
                        {
                            //???? Default pre 3D pohlad na scenu
                            iTextNormalInLCSCode = 2;
                        }

                        // Vzdialenost od stredovej taziskovej linie po okraj prierezu
                        float fOffsetInPlaneBasic_y = (float)Math.Max(model.m_arrMembers[i].CrScStart.y_min, model.m_arrMembers[i].CrScStart.y_max);
                        float fOffsetInPlaneBasic_z = (float)model.m_arrMembers[i].CrScStart.z_max;

                        // Tento offset urcuje, aka je medzera medzi prutom a riadiacim bodom textu
                        float fOffsetInPlaneAdd_y = 0.04f;
                        float fOffsetInPlaneAdd_z = 0.04f;

                        float fOffsetInPlane_y = fOffsetInPlaneBasic_y + fOffsetInPlaneAdd_y + 0.5f * fTextBlockVerticalSize;
                        float fOffsetInPlane_z = fOffsetInPlaneBasic_z + fOffsetInPlaneAdd_z + 0.5f * fTextBlockVerticalSize;

                        // Tento offset urcuje o kolko je text vysunuty pred prut, dal by sa nastavovat podla rozmerov prierezu
                        float fOffsetOutOfPlane_y = -0.3f; // Offset z roviny LCS xz (znamienko podla smerovania osy y a rotacie textu)
                        float fOffsetOutOfPlane_Z = 0.3f; // Offset z roviny LCS xy (znamienko podla smerovania osy z a rotacie textu)

                        Point3D pTextPositionInLCS = new Point3D(); // Riadiaci bod pre vlozenie textu v LCS pruta
                        Vector3D over_LCS; // Vektor smeru textu v LCS
                        Vector3D up_LCS; // Vektor smeru textu v LCS

                        Vector3D over; // Vektor smeru textu vo view
                        Vector3D up; // Vektor smeru textu vo view

                        if (iTextNormalInLCSCode == 0) // Text pre LCS x (rovina yz)
                        {
                            pTextPositionInLCS.X = fRelativePositionFactor * model.m_arrMembers[i].FLength;
                            pTextPositionInLCS.Y = fOffsetInPlane_y; // v pripade potreby upravit
                            pTextPositionInLCS.Z = fOffsetInPlane_z; // Kreslime nad prut v LCS smere z - v pripade potreby upravit alebo zohladnit znamienko (text nad alebo pod prierezom)
                            over_LCS = new Vector3D(0, 1, 0);
                            up_LCS = new Vector3D(0, 0, 1);

                            // Ak smeruje lokalna osa x v smere zapornej osi Z potrebujeme text otocit aby sa vykreslil citalne zhora
                            if (model.m_arrMembers[i].Delta_X == 0 && model.m_arrMembers[i].Delta_Y == 0 && model.m_arrMembers[i].Delta_Z < 0)
                            {
                                over_LCS = new Vector3D(0, -1, 0); // ??? doriesit opacny smer textu
                                up_LCS = new Vector3D(0, 0, 1);
                            }

                            // TO Ondrej
                            // Pre pohlad Top a Filter Columns treba pootacat vektory na stlpoch aby smerovali vsetky rovnako, pripadne to urobit natvrdo
                            //TransformTextVectorsFromLCSAxisToViewAxis(fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, model.m_arrMembers[i], transform, over_LCS, up_LCS, out over, out up);

                            // Vzdy smeruje podla pohladu
                            over = viewHorizontalVector * fTextBlockHorizontalSizeFactor;
                            up = viewVerticalVector * fTextBlockVerticalSizeFactor;
                        }
                        else if (iTextNormalInLCSCode == 1) // Text pre LCS y (rovina xz)
                        {
                            pTextPositionInLCS.X = fRelativePositionFactor * model.m_arrMembers[i].FLength;
                            pTextPositionInLCS.Y = fOffsetOutOfPlane_y; // v pripade potreby upravit / TODO nastavit znamienko
                            pTextPositionInLCS.Z = fOffsetInPlane_z; // Kreslime nad prut v LCS smere z - v pripade potreby upravit alebo zohladnit znamienko (text nad alebo pod prierezom)
                            over_LCS = new Vector3D(1, 0, 0); // ??? doriesit opacny smer textu
                            up_LCS = new Vector3D(0, 0, 1);

                            // Ak smeruje lokalna osa x v smere zapornej osi Z potrebujeme text otocit aby sa vykreslil zhora dole
                            if (model.m_arrMembers[i].Delta_X == 0 && model.m_arrMembers[i].Delta_Y == 0 && model.m_arrMembers[i].Delta_Z < 0)
                            {
                                over_LCS = new Vector3D(-1, 0, 0); // ??? doriesit opacny smer textu
                                up_LCS = new Vector3D(0, 0, -1);
                            }

                            // Sucin kladneho smeru LCS y a view Y je zaporny (osa y smeruje opacnym smerom ako je smer pohladu)
                            if (memberLCSAxis_yInView.Y < -1e-3f) //  TO Ondrej - otacam pretacam, ale akosi to nefunguje - skus sa s tym pohrat
                            {
                                over_LCS = new Vector3D(-1, 0, 0);
                                //up_LCS = new Vector3D(0, 0, -1);

                                pTextPositionInLCS.Y *= -1;
                            }

                            // Sucin kladneho smeru LCS x a view horizontal je zaporny (osa x smeruje opacnym smerom ako je smer horizontalnej osi pohladu)
                            // Osa LCS z smeruje nadol (v smere zapornej vertikalnej osi pohladu)
                            if (memberLCSAxis_xInView.X < -1e-3f && memberLCSAxis_zInView.Z < -1e-3f)
                            {
                                over_LCS = new Vector3D(-1, 0, 0);
                                up_LCS = new Vector3D(0, 0, -1);

                                pTextPositionInLCS.Z *= -1;
                            }

                            TransformTextVectorsFromLCSAxisToViewAxis(fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, model.m_arrMembers[i], transform, matrixViewInGCS_Inverse, over_LCS, up_LCS, out over, out up);

                            if (memberLCSAxis_xInView.Z == 1 || memberLCSAxis_xInView.Z == -1) // Osa pruta smeruje zvislo v smere verikalnej osi pohladu (vzdy chceme citat zprava a zdola nahor)
                            {
                                over = viewVerticalVector * fTextBlockHorizontalSizeFactor;
                                up = Vector3D.Multiply(-1, viewHorizontalVector) * fTextBlockVerticalSizeFactor;
                            }
                        }
                        else // if(iTextNormalInLCSCode == 2) // Text pre LCS z (rovina xy)
                        {
                            pTextPositionInLCS.X = fRelativePositionFactor * model.m_arrMembers[i].FLength;
                            pTextPositionInLCS.Y = fOffsetInPlane_y; // Kreslime vlavo / vpravo od pruta v LCS smere y - v pripade potreby upravit alebo zohladnit znamienko podla toho na ktorej strane pruta chceme text zobrazit
                            pTextPositionInLCS.Z = fOffsetOutOfPlane_Z; // v pripade potreby upravit / TODO nastavit znamienko
                            over_LCS = new Vector3D(1, 0, 0); // Text v smere kladnej osi x 
                            up_LCS = new Vector3D(0, 1, 0);

                            // Ak smeruje lokalna osa x v smere zapornej osi Z potrebujeme text otocit aby sa vykreslil zhora dole
                            if (model.m_arrMembers[i].Delta_X == 0 && model.m_arrMembers[i].Delta_Y == 0 && model.m_arrMembers[i].Delta_Z < 0)
                            {
                                over_LCS = new Vector3D(-1, 0, 0); // ??? doriesit opacny smer textu
                                up_LCS = new Vector3D(0, -1, 0);
                            }

                            // Sucin kladneho smeru LCS z a view Y je kladny (osa z smeruje rovnakym smerom ako je smer pohladu)
                            if (memberLCSAxis_zInView.Y > 1e-3f) //  TO Ondrej - otacam pretacam, ale akosi to nefunguje - skus sa s tym pohrat
                            {
                                over_LCS = new Vector3D(-1, 0, 0);
                                //up_LCS = new Vector3D(0, 1, 0);

                                pTextPositionInLCS.Z *= -1;
                            }

                            // Sucin kladneho smeru LCS z a view Y je zaporny (osa z smeruje opacnym smerom ako je smer pohladu)
                            // Osa LCS y smeruje v smere horizontalnej osi pohladu
                            if (memberLCSAxis_zInView.Y < 1e-3f && memberLCSAxis_yInView.X > 1e-3f) //  TO Ondrej - otacam pretacam, ale akosi to nefunguje - skus sa s tym pohrat
                            {
                                over_LCS = new Vector3D(-1, 0, 0);
                                up_LCS = new Vector3D(0, -1, 0);

                                pTextPositionInLCS.Y *= -1;
                            }

                            // Sucin kladneho smeru LCS x a view horizontal je kladny (osa x smeruje rovnakym smerom ako je smer horizontalnej osi pohladu)
                            // Osa LCS y smeruje nadol (v smere zapornej vertikalnej osi pohladu)
                            if (memberLCSAxis_xInView.X > 1e-3f && memberLCSAxis_yInView.Z < -1e-3f)
                            {
                                over_LCS = new Vector3D(1, 0, 0);
                                up_LCS = new Vector3D(0, -1, 0);

                                pTextPositionInLCS.Y *= -1;
                            }

                            TransformTextVectorsFromLCSAxisToViewAxis(fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, model.m_arrMembers[i], transform, matrixViewInGCS_Inverse, over_LCS, up_LCS, out over, out up);

                            if (memberLCSAxis_xInView.Z == 1 || memberLCSAxis_xInView.Z == -1) // Osa pruta smeruje zvislo v smere verikalnej osi pohladu (vzdy chceme citat zprava a zdola nahor)
                            {
                                over = viewVerticalVector * fTextBlockHorizontalSizeFactor;
                                up = Vector3D.Multiply(-1, viewHorizontalVector) * fTextBlockVerticalSizeFactor;
                            }
                        }

                        Point3D pTextPositionInGCS = new Point3D(pTextPositionInLCS.X, pTextPositionInLCS.Y, pTextPositionInLCS.Z); // Riadiaci bod pre vlozenie textu v GCS

                        // Transformujeme suradnice riadiaceho bodu z LCS do GCS
                        pTextPositionInGCS = transform.Transform(pTextPositionInGCS);

                        // Create text
                        textlabel = CreateTextLabel3D(tb, false, fTextBlockVerticalSize, pTextPositionInGCS, over, up);

                        if (centerModel)
                        {
                            textlabel.Transform = centerModelTransGr;
                        }

                        float fMemberLengthLimitToDisplayDescription = 0.5f;

                        // podmienky pre pridanie textu (napriklad typ alebo dlzka pruta)
                        if (model.m_arrMembers[i].FLength > fMemberLengthLimitToDisplayDescription)
                            viewPort.Children.Add(textlabel);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Draw Text in 3D
        public static void CreateNodesDescriptionModel3D(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            if (model.m_arrNodes != null)
            {
                ModelVisual3D textlabel = null;

                for (int i = 0; i < model.m_arrNodes.Length; i++)
                {
                    if (model.m_arrNodes[i] != null) // Node object is valid (not empty)
                    {
                        Point3D p = new Point3D(model.m_arrNodes[i].X, model.m_arrNodes[i].Y, model.m_arrNodes[i].Z);
                        string sTextToDisplay = model.m_arrNodes[i].ID.ToString();

                        TextBlock tb = new TextBlock();
                        tb.Text = sTextToDisplay;
                        tb.FontFamily = new FontFamily("Arial");
                        float fTextBlockVerticalSize = displayOptions.fNodeDescriptionTextFontSize / 100f;
                        float fTextBlockVerticalSizeFactor = 0.8f;
                        float fTextBlockHorizontalSizeFactor = 0.3f;

                        // Tieto nastavenia sa nepouziju
                        tb.FontStretch = FontStretches.UltraCondensed;
                        tb.FontStyle = FontStyles.Normal;
                        tb.FontWeight = FontWeights.Thin;
                        tb.Foreground = new SolidColorBrush(displayOptions.NodeDescriptionTextColor); // TO Ondrej - zda sa mi ze u textu je mozne nastavit rozne farby pre vypln a outline pismenok - vieme to nejako urobit ??? // Ina farba ako pre popis prutov
                        tb.Background = new SolidColorBrush(displayOptions.backgroundColor); // TODO - In case that solid model is displayed it is reasonable to use black backround of text or offset texts usig cross-section dimension

                        float fOffsetZ = 0.06f;
                        float fOffsetX = 0.06f;
                        Point3D pTextPosition = new Point3D();
                        pTextPosition.X = p.X - fOffsetX;
                        pTextPosition.Y = p.Y;
                        pTextPosition.Z = p.Z + fOffsetZ;

                        // Create text
                        textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTextPosition, new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0), new Vector3D(0, 0, fTextBlockVerticalSizeFactor));

                        if (centerModel)
                        {
                            textlabel.Transform = centerModelTransGr;
                        }
                        viewPort.Children.Add(textlabel);
                    }
                }
            }
        }

        // Draw Dimension Text 3D
        public static void DrawDimensionText3D(CDimensionLinear3D dimension, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = dimension.Text;
            tb.FontFamily = new FontFamily("Arial");
            float fTextBlockVerticalSize = displayOptions.fDimensionTextFontSize / 100f;
            float fTextBlockVerticalSizeFactor = 0.8f;
            float fTextBlockHorizontalSizeFactor = 0.3f;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = new SolidColorBrush(displayOptions.DimensionTextColor);
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);
            //Vector3D over = new Vector3D(dimension.Horizontal.X * fTextBlockHorizontalSizeFactor, dimension.Horizontal.Y * fTextBlockHorizontalSizeFactor, dimension.Horizontal.Z * fTextBlockHorizontalSizeFactor);
            //Vector3D up = new Vector3D(dimension.Vertical.X * fTextBlockVerticalSizeFactor, dimension.Vertical.Y * fTextBlockVerticalSizeFactor, dimension.Vertical.Z * fTextBlockVerticalSizeFactor);
            Vector3D over = new Vector3D(fTextBlockHorizontalSizeFactor * dimension.iVectorOverFactor_LCS, 0, 0);
            Vector3D up = new Vector3D(0, fTextBlockVerticalSizeFactor * dimension.iVectorUpFactor_LCS, 0);

            //SetLabelsUpAndOverVectors(displayOptions, fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, out over, out up);
            // Create text
            ModelVisual3D textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, dimension.PointText, over, up);

            if (centerModel)
            {
                Transform3DGroup tr = new Transform3DGroup();

                if (dimension.TransformGr == null)
                {
                    throw new Exception("Dimension in local coordinate system! \nTransformation object is null! \nText label is probably created before dimension model exists!");
                }

                if (dimension.TransformGr != null)
                {
                    tr.Children.Add(dimension.TransformGr); // TO Ondrej - tu si mal zakomentovanu podmienku a ak bola dimension.TransformGr null tak to tu padlo, neviem ci moze byt null, jedine ze sa s kotou nic nerobi ale to sa mi nezda

                    // Pokus transformovat samostatne bod a vektory a az potom vytvorit label, netransformuje sa teda label ako celok
                    //Vector3D overTransformed = dimension.TransformGr.Transform(over);
                    //Vector3D upTransformed = dimension.TransformGr.Transform(up);
                    //Point3D pTransformed = dimension.TransformGr.Transform(dimension.PointText);
                    //textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTransformed, overTransformed, upTransformed);
                }
                tr.Children.Add(centerModelTransGr); // To Ondrej - Mam otazku ci treba tuto transformaciu pre text robit samostatna alebo je uz obsiahnuta v transformacii koty
                textlabel.Transform = tr; //centerModelTransGr;
            }
            viewPort.Children.Add(textlabel);
        }

        // Draw Dimension 3D
        public static void DrawDimension3D(CDimensionLinear3D dimension, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = dimension.Text;
            tb.FontFamily = new FontFamily("Arial");
            float fTextBlockVerticalSize = 0.1f;
            float fTextBlockVerticalSizeFactor = 0.8f;
            float fTextBlockHorizontalSizeFactor = 0.3f;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = Brushes.Coral;
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);
            Vector3D over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
            Vector3D up = new Vector3D(0, 0, fTextBlockVerticalSizeFactor);

            SetLabelsUpAndOverVectors(displayOptions, fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, out over, out up);
            // Create text
            ModelVisual3D textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, dimension.PointText, over, up);

            if (centerModel)
            {
                textlabel.Transform = centerModelTransGr;
            }
            viewPort.Children.Add(textlabel);

            Color dimensionColor = Colors.Red;

            // LINES

            float flineThickness = 4;

            //WireLine wL1 = new WireLine();
            //wL1.Point1 = dimension.PointStart;
            //wL1.Point2 = dimension.PointStartL2;
            //wL1.Thickness = flineThickness;
            //wL1.Color = dimensionColor;

            //WireLine wL2 = new WireLine();
            //wL2.Point1 = dimension.PointEnd;
            //wL2.Point2 = dimension.PointEndL2;
            //wL2.Thickness = flineThickness;
            //wL2.Color = dimensionColor;

            //WireLine wMain = new WireLine();
            //wMain.Point1 = dimension.PointMainLine1;
            //wMain.Point2 = dimension.PointMainLine2;
            //wMain.Thickness = flineThickness;
            //wMain.Color = dimensionColor;

            //LinesVisual3D wL1 = new LinesVisual3D();
            //LinesVisual3D wL2 = new LinesVisual3D();
            //LinesVisual3D wMain = new LinesVisual3D();

            ScreenSpaceLines3D wL1 = new ScreenSpaceLines3D();
            ScreenSpaceLines3D wL2 = new ScreenSpaceLines3D();
            ScreenSpaceLines3D wMain = new ScreenSpaceLines3D();
            wL1.Points.Add(dimension.PointStart);
            wL1.Points.Add(dimension.PointStartL2);
            wL1.Color = dimensionColor;
            wL1.Thickness = flineThickness;

            wL2.Points.Add(dimension.PointEnd);
            wL2.Points.Add(dimension.PointEndL2);
            wL2.Color = dimensionColor;
            wL2.Thickness = flineThickness;

            wMain.Points.Add(dimension.PointMainLine1);
            wMain.Points.Add(dimension.PointMainLine2);
            wMain.Color = dimensionColor;
            wMain.Thickness = flineThickness;

            if (centerModel)
            {
                wL1.Transform = centerModelTransGr;
                wL2.Transform = centerModelTransGr;
                wMain.Transform = centerModelTransGr;
            }

            viewPort.Children.Add(wL1);
            viewPort.Children.Add(wL2);
            viewPort.Children.Add(wMain);
            //viewPort.UpdateLayout();
            wL1.Rescale();
            wL2.Rescale();
            wMain.Rescale();
            //viewPort.UpdateLayout();
        }

        // Draw Saw Cut Text 3D
        public static void DrawSawCutText3D(CSawCut sawcut, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = sawcut.Text;
            tb.FontFamily = new FontFamily("Arial");
            float fTextBlockVerticalSize = displayOptions.fSawCutTextFontSize / 100f;
            float fTextBlockVerticalSizeFactor = 0.8f;
            float fTextBlockHorizontalSizeFactor = 0.3f;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = new SolidColorBrush(displayOptions.SawCutTextColor);
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);
            Vector3D over = new Vector3D(fTextBlockHorizontalSizeFactor * sawcut.iVectorOverFactor_LCS, 0, 0);
            Vector3D up = new Vector3D(0, fTextBlockVerticalSizeFactor * sawcut.iVectorUpFactor_LCS, 0);

            // Create text
            ModelVisual3D textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, sawcut.PointText, over, up);

            if (centerModel)
            {
                Transform3DGroup tr = new Transform3DGroup();

                if (sawcut.TransformGr == null)
                {
                    throw new Exception("Saw cut in local coordinate system! \nTransformation object is null! \nText label is probably created before saw cut model exists!");
                }

                if (sawcut.TransformGr != null)
                {
                    tr.Children.Add(sawcut.TransformGr);
                }
                tr.Children.Add(centerModelTransGr);
                textlabel.Transform = tr;
            }
            viewPort.Children.Add(textlabel);
        }

        public static void CreateSawCutDescriptionModel3D(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            if (model.m_arrSawCuts != null)
            {
                for (int i = 0; i < model.m_arrSawCuts.Count; i++)
                {
                    if (model.m_arrSawCuts[i] != null) // Saw cut object is valid (not empty)
                    {
                        DrawSawCutText3D(model.m_arrSawCuts[i], viewPort, displayOptions);
                    }
                }
            }
        }

        // Draw Control Joint Text 3D
        public static void DrawControlJointText3D(CControlJoint controlJoint, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = controlJoint.Text;
            tb.FontFamily = new FontFamily("Arial");
            float fTextBlockVerticalSize = displayOptions.fControlJointTextFontSize / 100f;
            float fTextBlockVerticalSizeFactor = 0.8f;
            float fTextBlockHorizontalSizeFactor = 0.3f;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = new SolidColorBrush(displayOptions.ControlJointTextColor);
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);
            Vector3D over = new Vector3D(fTextBlockHorizontalSizeFactor * controlJoint.iVectorOverFactor_LCS, 0, 0);
            Vector3D up = new Vector3D(0, fTextBlockVerticalSizeFactor * controlJoint.iVectorUpFactor_LCS, 0);

            // Create text
            ModelVisual3D textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, controlJoint.PointText, over, up);

            if (centerModel)
            {
                Transform3DGroup tr = new Transform3DGroup();

                if (controlJoint.TransformGr == null)
                {
                    throw new Exception("Control joint in local coordinate system! \nTransformation object is null! \nText label is probably created before control joint model exists!");
                }

                if (controlJoint.TransformGr != null)
                {
                    tr.Children.Add(controlJoint.TransformGr);
                }
                tr.Children.Add(centerModelTransGr);
                textlabel.Transform = tr;
            }
            viewPort.Children.Add(textlabel);
        }

        public static void CreateControlJointDescriptionModel3D(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            if (model.m_arrControlJoints != null)
            {
                for (int i = 0; i < model.m_arrControlJoints.Count; i++)
                {
                    if (model.m_arrControlJoints[i] != null) // Control joint object is valid (not empty)
                    {
                        DrawControlJointText3D(model.m_arrControlJoints[i], viewPort, displayOptions);
                    }
                }
            }
        }

        // Draw Foundations Text 3D
        public static void DrawFoundationText3D(CFoundation foundation, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = foundation.Text;
            tb.FontFamily = new FontFamily("Arial");
            float fTextBlockVerticalSize = displayOptions.fFoundationTextFontSize / 100f;
            float fTextBlockVerticalSizeFactor = 0.8f;
            float fTextBlockHorizontalSizeFactor = 0.3f;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = new SolidColorBrush(displayOptions.FoundationTextColor);
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);

            // Nastavujeme pre GCS (rovina XY - text v smere Y)
            Vector3D over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
            Vector3D up = new Vector3D(-fTextBlockVerticalSizeFactor, 0, 0);

            // Create text
            ModelVisual3D textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, foundation.PointText, over, up); ;
            Transform3DGroup tr = new Transform3DGroup();

            if (foundation.GetFoundationTransformGroup_Complete() == null)
            {
                throw new Exception("Foundation in local coordinate system! \nTransformation object is null! \nText label is probably created before foundation model exists!");
            }

            if (foundation.GetFoundationTransformGroup_Complete() != null)
            {
                tr.Children.Add(foundation.GetFoundationTransformGroup_Complete());

                // Nechceme transofrmovat cely text label len vkladaci bod
                Point3D pTransformed = tr.Transform(foundation.PointText);
                textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTransformed, over, up);
            }

            if (centerModel)
            {
                textlabel.Transform = centerModelTransGr;
            }
            viewPort.Children.Add(textlabel);
        }

        public static void CreateFoundationsDescriptionModel3D(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            if (model.m_arrFoundations != null)
            {
                for (int i = 0; i < model.m_arrFoundations.Count; i++)
                {
                    if (model.m_arrFoundations[i] != null) // Foundation object is valid (not empty)
                    {
                        DrawFoundationText3D(model.m_arrFoundations[i], viewPort, displayOptions);
                    }
                }
            }
        }

        // Draw Floor Slabs Text 3D
        public static void DrawFloorSlabText3D(CSlab slab, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = slab.Text;
            tb.FontFamily = new FontFamily("Arial");
            float fTextBlockVerticalSize = displayOptions.fFloorSlabTextFontSize / 100f;
            int iNumberOfRowsInTexBlock = 6; // TODO Ondrej - tu sa snazim nastavit pomer velkosti podla poctu riadkov textu - da sa to vymysliet nejako tak ze sa toto udeje automaticky :)))
            float fTextBlockVerticalSizeFactor = 0.8f * iNumberOfRowsInTexBlock;
            float fTextBlockHorizontalSizeFactor = 0.3f / iNumberOfRowsInTexBlock;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = new SolidColorBrush(displayOptions.FloorSlabTextColor);
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);

            // Nastavujeme pre GCS (rovina XY - text v smere Y)
            Vector3D over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
            Vector3D up = new Vector3D(-fTextBlockVerticalSizeFactor, 0, 0);

            // Create text
            // Create text
            ModelVisual3D textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, slab.PointText, over, up); ;
            Transform3DGroup tr = new Transform3DGroup();

            if (slab.GetSlabTransformGroup() == null)
            {
                throw new Exception("Slab in local coordinate system! \nTransformation object is null! \nText label is probably created before slab model exists!");
            }

            if (slab.GetSlabTransformGroup() != null)
            {
                tr.Children.Add(slab.GetSlabTransformGroup());

                // Nechceme transofrmovat cely text label len vkladaci bod
                Point3D pTransformed = tr.Transform(slab.PointText);
                textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTransformed, over, up);
            }

            if (centerModel)
            {
                textlabel.Transform = centerModelTransGr;
            }
            viewPort.Children.Add(textlabel);
        }

        public static void CreateFloorSlabsDescriptionModel3D(CModel model, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            if (model.m_arrSlabs != null)
            {
                for (int i = 0; i < model.m_arrSlabs.Count; i++)
                {
                    if (model.m_arrSlabs[i] != null) // Slab object is valid (not empty)
                    {
                        DrawFloorSlabText3D(model.m_arrSlabs[i], viewPort, displayOptions);
                    }
                }
            }
        }

        private static Model3DGroup CreateModelDimensions_Model3DGroup(List<CDimensionLinear3D> dimensions, CModel model, DisplayOptions displayOptions)
        {
            if (dimensions == null || dimensions.Count == 0)
                return null;

            // ZATIAL POKUS VYKRESLIT KOTU INDIVIDUALNE, NIE VSETKY KOTY NARAZ Z CELEHO MODELU
            // Draw 3D objects (cylinder as a line)

            Model3DGroup gr = new Model3DGroup();

            foreach (CDimensionLinear3D dimension in dimensions)
            {
                gr.Children.Add(dimension.GetDimensionModelNew(displayOptions.DimensionLineColor));
            }

            return gr;
        }

        private static Model3DGroup CreateModelNodes_Model3DGroup(CModel model)
        {
            double nodesSize = 0.01; // Polovica dlzky strany kocky alebo polomer gule
            Model3DGroup model3D_group = new Model3DGroup();

            if (model.m_arrNodes != null)
            {
                for (int i = 0; i < model.m_arrNodes.Length; i++)
                {
                    if (model.m_arrNodes[i] != null) // Node object is valid (not empty)
                    {
                        Point3D p = new Point3D(model.m_arrNodes[i].X, model.m_arrNodes[i].Y, model.m_arrNodes[i].Z);

                        // TODO Ondrej - presunut / refaktorovat metodu GetCube, aby bolo vsetko v balicku CVolume
                        // Pridal som moznost ci chceme vykreslit uzol ako gulicku alebo kocku
                        // Pripadne mozes upravit celu triedu CVolume, je tam teraz trosku chaos a duplicity, rozne mozne konstruktory pre to iste atd :)

                        // Sphere
                        GraphObj.CVolume sphere = new GraphObj.CVolume();
                        model3D_group.Children.Add(sphere.CreateM_3D_G_Volume_Sphere(p, (float)nodesSize, new DiffuseMaterial(new SolidColorBrush(Colors.Cyan))));

                        // Cube
                        //model3D_group.Children.Add(GetCube(p, nodesSize, new SolidColorBrush(Colors.Cyan)));
                    }
                }
            }

            return model3D_group;
        }

        private static string GetMemberDisplayText(DisplayOptions options, CMember m)
        {
            string separator = " - ";
            List<string> parts = new List<string>();
            if (options.bDisplayMemberID) parts.Add(m.ID.ToString());
            if (options.bDisplayMemberPrefix) parts.Add(m.Prefix.ToString());
            if (options.bDisplayMemberCrossSectionStartName) parts.Add(m.CrScStart?.Name_short);
            if (options.bDisplayMemberRealLength)
            {
                if (options.bDisplayMemberRealLengthUnit)
                {
                    if (options.bDisplayMemberRealLengthInMM) parts.Add((m.FLength_real * 1000).ToString("F0") + " mm");
                    else parts.Add(m.FLength_real.ToString("F3") + " m");
                }
                else
                {
                    if (options.bDisplayMemberRealLengthInMM) parts.Add((m.FLength_real * 1000).ToString("F0"));
                    else parts.Add(m.FLength_real.ToString("F3"));
                }
            }
            return string.Join(separator, parts);
        }

        private static string GetNodeLoadDisplayText(DisplayOptions options, CNLoadSingle l)
        {
            // TODO - zistit smer a hodnotu CNLoad

            List<string> parts = new List<string>();

            float fLoadValue = 0f;
            float fUnitFactor = 0.001f; // Fx - Fz (N to kN) or Mx - Mz (N/m to kN/m)

            string sUnitString = "";

            switch (l.NLoadType)
            {
                case ENLoadType.eNLT_Fx:
                case ENLoadType.eNLT_Fy:
                case ENLoadType.eNLT_Fz:
                    {
                        sUnitString = "[kN]";
                        break;
                    }
                case ENLoadType.eNLT_Mx:
                case ENLoadType.eNLT_My:
                case ENLoadType.eNLT_Mz:
                    {
                        sUnitString = "[kNm]";
                        break;
                    }
                default:
                    Console.WriteLine("Not implemented nodal load.");
                    break;
            }

            fLoadValue = l.Value;

            string separator = " - ";
            parts.Add(l.ID.ToString());
            parts.Add(l.Prefix.ToString());
            if (options.bDisplayLoadsLabelsUnits) parts.Add((fLoadValue * fUnitFactor).ToString("F3") + " " + sUnitString);
            else parts.Add((fLoadValue * fUnitFactor).ToString("F3"));

            return string.Join(separator, parts);
        }

        private static string GetNodeLoadDisplayText(DisplayOptions options, CNLoadAll l)
        {
            List<string> listOfStrings = new List<string>();
            string separator = " - ";
            string id = l.ID.ToString();

            float fUnitFactor = 0.001f; // Fx - Fz (N to kN) or Mx - Mz (N/m to kN/m)

            List<string> parts1 = new List<string>();
            parts1.Add("Fx");
            parts1.Add((l.Value_FX * fUnitFactor).ToString("F3") + (options.bDisplayLoadsLabelsUnits ? " [kN]" : ""));
            string lineFx = string.Join(separator, parts1);

            List<string> parts2 = new List<string>();
            parts2.Add("Fy");
            parts2.Add((l.Value_FY * fUnitFactor).ToString("F3") + (options.bDisplayLoadsLabelsUnits ? " [kN]" : ""));
            string lineFy = string.Join(separator, parts2);

            List<string> parts3 = new List<string>();
            parts3.Add("Fz");
            parts3.Add((l.Value_FZ * fUnitFactor).ToString("F3") + (options.bDisplayLoadsLabelsUnits ? " [kN]" : ""));
            string lineFz = string.Join(separator, parts3);

            List<string> parts4 = new List<string>();
            parts4.Add("Fx");
            parts4.Add((l.Value_MX * fUnitFactor).ToString("F3") + (options.bDisplayLoadsLabelsUnits ? " [kNm]" : ""));
            string lineMx = string.Join(separator, parts4);

            List<string> parts5 = new List<string>();
            parts5.Add("Fx");
            parts5.Add((l.Value_MY * fUnitFactor).ToString("F3") + (options.bDisplayLoadsLabelsUnits ? " [kNm]" : ""));
            string lineMy = string.Join(separator, parts5);

            List<string> parts6 = new List<string>();
            parts6.Add("Fx");
            parts6.Add((l.Value_MZ * fUnitFactor).ToString("F3") + (options.bDisplayLoadsLabelsUnits ? " [kNm]" : ""));
            string lineMz = string.Join(separator, parts6);

            string wholeString = id + "\n" +
                lineFx + "\n" +
                lineFy + "\n" +
                lineFz + "\n" +
                lineMx + "\n" +
                lineMy + "\n" +
                lineMz + "\n";

            return wholeString;
        }

        // Draw Text in 3D
        public static Model3DGroup CreateLabels3DForLoadCase(CModel model, CLoadCase loadCase, DisplayOptions sDisplayOptions)
        {
            Model3DGroup gr = new Model3DGroup();

            float fRelativePositionFactor = 0.4f; //(0-1) // Relative position of member description on member
            float fRelativePositionOfTextOnMember_LCS = fRelativePositionFactor;

            float fTextBlockVerticalSize = 0.2f;
            float fTextBlockVerticalSizeFactor = 0.8f;
            float fTextBlockHorizontalSizeFactor = 0.3f;
            //float fOffsetZ = 0.07f;

            if (loadCase != null)
            {

                if (loadCase.NodeLoadsList != null && sDisplayOptions.bDisplayNodalLoads) // Some nodal loads exist and are displayed
                {
                    // Model Groups of Nodal Loads
                    for (int i = 0; i < loadCase.NodeLoadsList.Count; i++)
                    {
                        if (loadCase.NodeLoadsList[i] != null && loadCase.NodeLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            ModelVisual3D textlabel = DrawNodalLoadLabel3D(loadCase.NodeLoadsList[i], fTextBlockVerticalSize, fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, sDisplayOptions);
                            if (textlabel != null) gr.Children.Add(textlabel.Content);
                        }
                    }
                }

                if (loadCase.MemberLoadsList != null && sDisplayOptions.bDisplayMemberLoads) // Some member loads exist and are displayed
                {
                    // Model Groups of Member Loads
                    for (int i = 0; i < loadCase.MemberLoadsList.Count; i++)
                    {
                        if (loadCase.MemberLoadsList[i] != null && loadCase.MemberLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            // Label zobrazujeme len pre zatazenia ktore su zobrazene
                            if ((sDisplayOptions.bDisplayMemberLoads_Girts && loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eG) ||
                               (sDisplayOptions.bDisplayMemberLoads_Purlins && loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eP) ||
                               (sDisplayOptions.bDisplayMemberLoads_Columns && loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eC) ||
                               (sDisplayOptions.bDisplayMemberLoads_Frames &&
                               (loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eMC ||
                               loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eMR ||
                               loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eEC ||
                               loadCase.MemberLoadsList[i].Member.EMemberType == EMemberType_FS.eER)
                               ))
                            {
                                ModelVisual3D textlabel = DrawMemberLoadLabel3D(loadCase.MemberLoadsList[i], fTextBlockVerticalSize, fTextBlockHorizontalSizeFactor, fTextBlockVerticalSizeFactor, sDisplayOptions);
                                if (textlabel != null) gr.Children.Add(textlabel.Content);
                            }
                        }
                    }
                }

                if (loadCase.SurfaceLoadsList != null && sDisplayOptions.bDisplaySurfaceLoads) // Some surface loads exist and are displayed
                {
                    // Model Groups of Surface Loads
                    for (int i = 0; i < loadCase.SurfaceLoadsList.Count; i++)
                    {
                        if (loadCase.SurfaceLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            if (loadCase.SurfaceLoadsList[i] is CSLoad_FreeUniformGroup)
                            {
                                Transform3DGroup loadGroupTransform = ((CSLoad_FreeUniformGroup)loadCase.SurfaceLoadsList[i]).CreateTransformCoordGroupOfLoadGroup();
                                foreach (CSLoad_FreeUniform l in ((CSLoad_FreeUniformGroup)loadCase.SurfaceLoadsList[i]).LoadList)
                                {
                                    DrawSurfaceLoadLabel3D(l, fTextBlockVerticalSize, fTextBlockVerticalSizeFactor, fTextBlockHorizontalSizeFactor, sDisplayOptions, loadGroupTransform, ref gr);
                                }
                            }
                            else if (loadCase.SurfaceLoadsList[i] is CSLoad_FreeUniform)
                            {
                                CSLoad_FreeUniform l = (CSLoad_FreeUniform)loadCase.SurfaceLoadsList[i];
                                DrawSurfaceLoadLabel3D(l, fTextBlockVerticalSize, fTextBlockVerticalSizeFactor, fTextBlockHorizontalSizeFactor, sDisplayOptions, null, ref gr);
                            }
                            else throw new Exception("Load type not known.");
                        }
                    }
                }
            }
            return gr;
        }

        private static ModelVisual3D DrawNodalLoadLabel3D(CNLoad load, float fTextBlockVerticalSize, float fTextBlockHorizontalSizeFactor, float fTextBlockVerticalSizeFactor, DisplayOptions displayOptions)
        {
            ModelVisual3D textlabel = null;

            string sTextToDisplay;
            if (load is CNLoadSingle)
                sTextToDisplay = GetNodeLoadDisplayText(displayOptions, (CNLoadSingle)load);
            else
                sTextToDisplay = GetNodeLoadDisplayText(displayOptions, (CNLoadAll)load);

            TextBlock tb = new TextBlock();
            tb.Text = sTextToDisplay;
            tb.FontFamily = new FontFamily("Arial");
            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = Brushes.Coral; // musime nastavovat farbu textu, inak sa to kresli ciernou
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);

            Point3D pTextPosition = GetNodalLoadCoordinates_GCS(load, displayOptions);

            // Create text
            textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTextPosition, new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0), new Vector3D(0, 0, fTextBlockVerticalSizeFactor));
            return textlabel;
        }

        private static ModelVisual3D DrawMemberLoadLabel3D(CMLoad load, float fTextBlockVerticalSize, float fTextBlockHorizontalSizeFactor, float fTextBlockVerticalSizeFactor, DisplayOptions displayOptions)
        {
            // Set load for all assigned member
            ModelVisual3D textlabel = null;

            float fLoadValue = load.GetLoadValue(); //extension method used

            // Ak je hodnota zatazenia 0, tak nic nevykreslit
            if (MathF.d_equal(fLoadValue, 0)) return null;

            float fUnitFactor = 0.001f; // N/m to kN/m
            string sTextToDisplay = (fLoadValue * fUnitFactor).ToString("F3") + (displayOptions.bDisplayLoadsLabelsUnits ? " [kN/m]" : "");

            TextBlock tb = new TextBlock();
            tb.Text = sTextToDisplay;
            tb.FontFamily = new FontFamily("Arial");
            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = Brushes.Coral; // To Ondrej - asi musime nastavovat farbu textu, inak sa to kresli ciernou a nebolo to vidno
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);

            Model3DGroup model_gr = new Model3DGroup();
            model_gr = load.CreateM_3D_G_Load(displayOptions.bDisplaySolidModel, displayOptions.DisplayIn3DRatio);

            Model3D loadLine = model_gr.Children.Last();
            GeometryModel3D model3D = null;
            if (loadLine is GeometryModel3D) model3D = (GeometryModel3D)loadLine;
            else model3D = (GeometryModel3D)((Model3DGroup)loadLine).Children[0];
            MeshGeometry3D mesh = (MeshGeometry3D)model3D.Geometry;

            Point3D p1 = mesh.Positions.First();
            Point3D p2 = mesh.Positions.Last();
            Point3D pCenter = new Point3D((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, (p1.Z + p2.Z) / 2);

            pCenter = model_gr.Transform.Transform(pCenter);  //first trannsform whole group - posun FaA = startPoistion
            if (loadLine.Transform is TranslateTransform3D)
            {
                TranslateTransform3D tt = loadLine.Transform as TranslateTransform3D;
                tt.OffsetZ *= 1.1;
                pCenter = tt.Transform(pCenter);
            }
            else
            {
                pCenter = model3D.Transform.Transform(pCenter);  //toto si nie som isty ci treba, asi je to stale Identity
            }

            Transform3DGroup transGroup = load.CreateTransformCoordGroup(load.Member);
            Point3D pTextPosition = transGroup.Transform(pCenter);

            //                       Fq [kN/m]
            //           ________________*_____________
            //           |                            |
            //           |                            |
            //          \|/                          \|/
            //================================================= - MEMBER

            // Create text
            textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTextPosition, new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0), new Vector3D(0, 0, fTextBlockVerticalSizeFactor));
            return textlabel;
        }

        //najvacsi problem bol s dodatocnym odsadenim pre celu CSLoad_FreeUniformGroup a tato transformacia je ako parameter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="load">surface load</param>
        /// <param name="fTextBlockVerticalSize"></param>
        /// <param name="fTextBlockVerticalSizeFactor"></param>
        /// <param name="fTextBlockHorizontalSizeFactor"></param>
        /// <param name="groupTransform">transformacia celej </param>
        /// <param name="gr"></param>
        private static void DrawSurfaceLoadLabel3D(CSLoad_FreeUniform load, float fTextBlockVerticalSize, float fTextBlockVerticalSizeFactor, float fTextBlockHorizontalSizeFactor,
            DisplayOptions displayOptions, Transform3D groupTransform, ref Model3DGroup gr)
        {
            // Set load for all assigned surfaces
            ModelVisual3D textlabel = null;

            TextBlock tb = new TextBlock();
            tb.FontFamily = new FontFamily("Arial");
            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = Brushes.Coral;
            tb.Background = new SolidColorBrush(displayOptions.backgroundColor);

            float fUnitFactor = 0.001f; // N/m^2 to kN/m^2 (Pa to kPa)
            Point3D pTextPosition = new Point3D();

            if (load.pSurfacePoints != null) // Check that surface points are initialized
            {
                // Ak je hodnota zatazenia 0, tak nic nevykreslit
                if (MathF.d_equal(load.fValue, 0))
                    return;

                //Je mozne prepinat zobrazenie uprostred, na vrchu a na spodku kvadra
                //Pokial by boli vsetky loads rovnako vykreslene a vyuzili by sa SurfacePoints_h s nejakym odsadenim...
                //tak by sa cisla zobrazili mimo kvadra a bolo by to asi podstatne krajsie
                //show in load center
                load.PointsGCS = GetLoadCoordinates_GCS(load, groupTransform, displayOptions.DisplayIn3DRatio); // Positions in global coordinate system GCS
                                                                                                                //show on bottom
                                                                                                                //l.PointsGCS = GetLoadCoordinates_GCS_SurfacePoints(l, groupTransform);
                                                                                                                //show on top
                                                                                                                //l.PointsGCS = GetLoadCoordinates_GCS_SurfacePoints_h(l, groupTransform);  

                if (load.PointsGCS.Count > 0)
                {
                    bool drawPointsOnEdges = false;  //moznost vykreslit suradnice na vrcholy kvadra (neviem ci sa niekedy este vyuzije)
                    if (drawPointsOnEdges)
                    {
                        foreach (Point3D p in load.PointsGCS)
                        {
                            pTextPosition.X = p.X;
                            pTextPosition.Y = p.Y;
                            pTextPosition.Z = p.Z;
                            tb.Text = $"{load.ID}_{load.Name}[{p.X:F1};{p.Y:F1};{p.Z:F1}]";
                            textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTextPosition, new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0), new Vector3D(0, 0, fTextBlockVerticalSizeFactor));
                            gr.Children.Add(textlabel.Content);
                        }
                    }

                    pTextPosition.X = load.PointsGCS.Average(p => p.X);
                    pTextPosition.Y = load.PointsGCS.Average(p => p.Y);
                    pTextPosition.Z = load.PointsGCS.Average(p => p.Z);

                    // Set load value to display
                    string sTextToDisplay = (load.fValue * fUnitFactor).ToString("F3") + (displayOptions.bDisplayLoadsLabelsUnits ? " [kPa]" : "");
                    tb.Text = sTextToDisplay;

                    // Create text
                    textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTextPosition, new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0), new Vector3D(0, 0, fTextBlockVerticalSizeFactor));
                    gr.Children.Add(textlabel.Content);
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

        public static Point3D GetNodalLoadCoordinates_GCS(CNLoad load, DisplayOptions options)
        {
            Model3DGroup gr = load.CreateM_3D_G_Load(options.DisplayIn3DRatio);
            if (gr.Children.Count < 1) return new Point3D();

            GeometryModel3D model3D = (GeometryModel3D)gr.Children[0];
            MeshGeometry3D mesh = (MeshGeometry3D)model3D.Geometry;

            Point3D p2 = mesh.Positions.LastOrDefault();
            if (p2 == null) return new Point3D();
            //p2.Y += 0.1 * p2.Z;  // to odsadenie este mozno treba nejako vyladit
            p2.Z += 0.2 * p2.Z;

            Point3D transPoint = gr.Transform.Transform(p2);
            return transPoint;
        }

        public static List<Point3D> GetLoadCoordinates_GCS(CSLoad_FreeUniform load, Transform3D groupTransform, float displayIn3DRatio)
        {
            Model3DGroup gr = load.CreateM_3D_G_Load(displayIn3DRatio);
            if (gr.Children.Count < 1) return new List<Point3D>();

            GeometryModel3D model3D = (GeometryModel3D)gr.Children[0];
            MeshGeometry3D mesh = (MeshGeometry3D)model3D.Geometry;
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(gr.Transform);
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }

            List<Point3D> transPoints = new List<Point3D>();
            foreach (Point3D p in mesh.Positions)
                transPoints.Add(trans.Transform(p));

            return transPoints;
        }
        public static List<Point3D> GetLoadCoordinates_GCS_SurfacePoints_h(CSLoad_FreeUniform load, Transform3D groupTransform, float displayIn3DRatio)
        {
            Model3DGroup gr = load.CreateM_3D_G_Load(displayIn3DRatio);
            if (gr.Children.Count < 1) return new List<Point3D>();

            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(gr.Transform);
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }

            List<Point3D> transPoints = new List<Point3D>();
            foreach (Point3D p in load.pSurfacePoints_h)
                transPoints.Add(trans.Transform(p));

            return transPoints;
        }
        public static List<Point3D> GetLoadCoordinates_GCS_SurfacePoints(CSLoad_FreeUniform load, Transform3D groupTransform, float displayIn3DRatio)
        {
            Model3DGroup gr = load.CreateM_3D_G_Load(displayIn3DRatio);
            if (gr.Children.Count < 1) return new List<Point3D>();

            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(gr.Transform);
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }

            List<Point3D> transPoints = new List<Point3D>();
            foreach (Point3D p in load.pSurfacePoints_h)
                transPoints.Add(trans.Transform(p));

            return transPoints;
        }

        public static void DrawSurfaceLoadsAxis(CLoadCase loadCase, Viewport3D viewPort)
        {
            if (loadCase != null)
            {
                if (loadCase.SurfaceLoadsList != null) // Some surface loads exist
                {
                    // Model Groups of Surface Loads
                    for (int i = 0; i < loadCase.SurfaceLoadsList.Count; i++)
                    {
                        if (loadCase.SurfaceLoadsList[i].BIsDisplayed == true) // Load object is valid (not empty) and should be displayed
                        {
                            if (loadCase.SurfaceLoadsList[i] is CSLoad_FreeUniformGroup)
                            {
                                Transform3DGroup loadGroupTransform = ((CSLoad_FreeUniformGroup)loadCase.SurfaceLoadsList[i]).CreateTransformCoordGroupOfLoadGroup();
                                foreach (CSLoad_FreeUniform l in ((CSLoad_FreeUniformGroup)loadCase.SurfaceLoadsList[i]).LoadList)
                                {
                                    DrawSurfaceLoadAxis(l, loadGroupTransform, viewPort);
                                }
                            }
                            else if (loadCase.SurfaceLoadsList[i] is CSLoad_FreeUniform)
                            {
                                CSLoad_FreeUniform l = (CSLoad_FreeUniform)loadCase.SurfaceLoadsList[i];
                                DrawSurfaceLoadAxis(l, null, viewPort);
                            }
                            else throw new Exception("Load type not known.");
                        }
                    }
                }
            }

        }
        private static void DrawSurfaceLoadAxis(CSLoad_FreeUniform l, Transform3DGroup loadGroupTransform, Viewport3D viewPort)
        {
            double axisL = 0.5;

            if (l.SurfaceDefinitionPoints != null) // Check that surface points are initialized
            {
                Point3D pC = new Point3D();
                pC.X = l.SurfaceDefinitionPoints.Average(p => p.X);
                pC.Y = l.SurfaceDefinitionPoints.Average(p => p.Y);
                pC.Z = l.SurfaceDefinitionPoints.Average(p => p.Z);
                Point3D pAxisX = new Point3D(pC.X + axisL, pC.Y, pC.Z);
                Point3D pAxisY = new Point3D(pC.X, pC.Y + axisL, pC.Z);
                Point3D pAxisZ = new Point3D(pC.X, pC.Y, pC.Z + axisL);

                Transform3DGroup trans = new Transform3DGroup();
                trans.Children.Add(l.CreateTransformCoordGroup());
                if (loadGroupTransform != null)
                {
                    trans.Children.Add(loadGroupTransform);
                }

                pC = trans.Transform(pC);
                pAxisX = trans.Transform(pAxisX);
                pAxisY = trans.Transform(pAxisY);
                pAxisZ = trans.Transform(pAxisZ);

                DrawAxis(viewPort, pC, pAxisX, pAxisY, pAxisZ);
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        // tu sa mi nepaci,ze to uvazuje iba s Node-mi
        //ak by to malo byt pekne, tak sa musi uvazovat aj s Crsc a vhodne priratat polku zo sirky Crsc, potom by to bolo lepsie centrovane
        // pre velky model je to asi zanedbatelne, ale pre male preview je to uz rozhodujuce

        // TODO - upravit funkcie tak aby uvazovali maximum z node, members, GOpoints, GOvolumes, pripadne inych objektov
        // TODO - upravit tak, aby sa do vysledneho rozmeru ratal cely box opisany vsetkymi objektami rozneho typu

        public static void CalculateModelLimitsWithoutCrsc(CModel cmodel, out float fMax_X, out float fMin_X, out float fMax_Y, out float fMin_Y, out float fMax_Z, out float fMin_Z)
        {
            fMax_X = float.MinValue;
            fMin_X = float.MaxValue;
            fMax_Y = float.MinValue;
            fMin_Y = float.MaxValue;
            fMax_Z = float.MinValue;
            fMin_Z = float.MaxValue;

            // Take maximum / minimum coordinate from all relevant types of objects

            if (cmodel.m_arrNodes != null && cmodel.m_arrNodes.Length > 1) // Some nodes exist - pre urcenie rozmeru minimalne 2 uzly
            {
                fMax_X = Math.Max(fMax_X, cmodel.m_arrNodes.Max(p => p.X));
                fMin_X = Math.Min(fMin_X, cmodel.m_arrNodes.Min(p => p.X));
                fMax_Y = Math.Max(fMax_Y, cmodel.m_arrNodes.Max(p => p.Y));
                fMin_Y = Math.Min(fMin_Y, cmodel.m_arrNodes.Min(p => p.Y));
                fMax_Z = Math.Max(fMax_Z, cmodel.m_arrNodes.Max(p => p.Z));
                fMin_Z = Math.Min(fMin_Z, cmodel.m_arrNodes.Min(p => p.Z));
            }

            if (cmodel.m_arrGOPoints != null) // Some points exist
            {
                fMax_X = Math.Max(fMax_X, (float)cmodel.m_arrGOPoints.Max(p => p.X));
                fMin_X = Math.Min(fMin_X, (float)cmodel.m_arrGOPoints.Min(p => p.X));
                fMax_Y = Math.Max(fMax_Y, (float)cmodel.m_arrGOPoints.Max(p => p.Y));
                fMin_Y = Math.Min(fMin_Y, (float)cmodel.m_arrGOPoints.Min(p => p.Y));
                fMax_Z = Math.Max(fMax_Z, (float)cmodel.m_arrGOPoints.Max(p => p.Z));
                fMin_Z = Math.Min(fMin_Z, (float)cmodel.m_arrGOPoints.Min(p => p.Z));
            }

            if (cmodel.m_arrFoundations != null) // Some volumes / foundations exist
            {
                // Each foundation in model
                Point3DCollection allFoundationPoints = new Point3DCollection();

                // TO Ondrej - refaktorovat s riadkom 2400

                if (cmodel.m_arrFoundations != null) // Some members must exist
                {
                    foreach (CFoundation f in cmodel.m_arrFoundations)
                    {
                        Point3DCollection foundationPoints = new Point3DCollection();

                        GeometryModel3D model3D = f.Visual_Object;

                        if (f.Visual_Object == null) // In case that foundation exist but geometry is not generated
                            model3D = f.Visual_Object = f.CreateGeomModel3D(0.2f); // TODO zaviest opacity ako parameter

                        MeshGeometry3D mesh3D = (MeshGeometry3D)model3D.Geometry; // TO Ondrej - toto su podla mna uplne zakladna mesh a body geometrie zakladu, nemali by sme pracovat uz s transformovanymi ????

                        foreach (Point3D point3D in mesh3D.Positions)
                        {
                            // TO Ondrej - dve moznosti ako ziskat transformaciu zakladu
                            // 1
                            Transform3DGroup trans = f.GetFoundationTransformGroup_Complete();

                            // 2
                            //Transform3DGroup trans = new Transform3DGroup();
                            //trans.Children.Add(model3D.Transform);

                            Point3D p = trans.Transform(point3D); // Transformujeme povodny bod
                            foundationPoints.Add(p);
                        }

                        // Add member points to the main collection of all members
                        if (foundationPoints != null)
                        {
                            foreach (Point3D p in foundationPoints)
                            {
                                allFoundationPoints.Add(p);
                            }
                        }
                    }
                }

                fMax_X = Math.Max(fMax_X, (float)allFoundationPoints.Max(p => p.X));
                fMin_X = Math.Min(fMin_X, (float)allFoundationPoints.Min(p => p.X));
                fMax_Y = Math.Max(fMax_Y, (float)allFoundationPoints.Max(p => p.Y));
                fMin_Y = Math.Min(fMin_Y, (float)allFoundationPoints.Min(p => p.Y));
                fMax_Z = Math.Max(fMax_Z, (float)allFoundationPoints.Max(p => p.Z));
                fMin_Z = Math.Min(fMin_Z, (float)allFoundationPoints.Min(p => p.Z));
            }

            if (fMax_X == float.MinValue ||
            fMin_X == float.MaxValue ||
            fMax_Y == float.MinValue ||
            fMin_Y == float.MaxValue ||
            fMax_Z == float.MinValue ||
            fMin_Z == float.MaxValue)
            {
                // Exception - no definition nodes or points
                throw new Exception("Exception - no definition nodes or points");
            }
        }

        public static void CalculateModelLimitsWithCrsc(CModel cmodel, out float fMax_X, out float fMin_X, out float fMax_Y, out float fMin_Y, out float fMax_Z, out float fMin_Z)
        {
            fMax_X = float.MinValue;
            fMin_X = float.MaxValue;
            fMax_Y = float.MinValue;
            fMin_Y = float.MaxValue;
            fMax_Z = float.MinValue;
            fMin_Z = float.MaxValue;

            // Each member in model
            Point3DCollection allMembersPoints = new Point3DCollection();

            if (cmodel.m_arrMembers != null) // Some members must exist
            {
                foreach (CMember m in cmodel.m_arrMembers)
                {
                    Point3DCollection memberPoints = null;

                    // Get transformed external outline points of real member
                    memberPoints = m.GetRealExternalOutlinePointsTransformedToGCS();

                    // Add member points to the main collection of all members
                    if (memberPoints != null)
                    {
                        foreach (Point3D p in memberPoints)
                        {
                            allMembersPoints.Add(p);
                        }
                    }
                }
            }

            if (allMembersPoints != null) // Some member outline points exist (transformed external outline points of real member)
            {
                fMax_X = Math.Max(fMax_X, (float)allMembersPoints.Max(p => p.X));
                fMin_X = Math.Min(fMin_X, (float)allMembersPoints.Min(p => p.X));
                fMax_Y = Math.Max(fMax_Y, (float)allMembersPoints.Max(p => p.Y));
                fMin_Y = Math.Min(fMin_Y, (float)allMembersPoints.Min(p => p.Y));
                fMax_Z = Math.Max(fMax_Z, (float)allMembersPoints.Max(p => p.Z));
                fMin_Z = Math.Min(fMin_Z, (float)allMembersPoints.Min(p => p.Z));
            }

            if (cmodel.m_arrGOPoints != null) // Some points exist
            {
                fMax_X = Math.Max(fMax_X, (float)cmodel.m_arrGOPoints.Max(p => p.X));
                fMin_X = Math.Min(fMin_X, (float)cmodel.m_arrGOPoints.Min(p => p.X));
                fMax_Y = Math.Max(fMax_Y, (float)cmodel.m_arrGOPoints.Max(p => p.Y));
                fMin_Y = Math.Min(fMin_Y, (float)cmodel.m_arrGOPoints.Min(p => p.Y));
                fMax_Z = Math.Max(fMax_Z, (float)cmodel.m_arrGOPoints.Max(p => p.Z));
                fMin_Z = Math.Min(fMin_Z, (float)cmodel.m_arrGOPoints.Min(p => p.Z));
            }

            if (cmodel.m_arrFoundations != null) // Some volumes / foundations exist
            {
                // Each foundation in model
                Point3DCollection allFoundationPoints = new Point3DCollection();

                // TO Ondrej - refaktorovat s riadkom 2400

                if (cmodel.m_arrFoundations != null) // Some members must exist
                {
                    foreach (CFoundation f in cmodel.m_arrFoundations)
                    {
                        Point3DCollection foundationPoints = new Point3DCollection();

                        GeometryModel3D model3D = f.Visual_Object;

                        if (f.Visual_Object == null) // In case that foundation exist but geometry is not generated
                            model3D = f.Visual_Object = f.CreateGeomModel3D(0.2f); // TODO zaviest opacity ako parameter

                        MeshGeometry3D mesh3D = (MeshGeometry3D)model3D.Geometry; // TO Ondrej - toto su podla mna uplne zakladna mesh a body geometrie zakladu, nemali by sme pracovat uz s transformovanymi ????

                        foreach (Point3D point3D in mesh3D.Positions)
                        {
                            // TO Ondrej - dve moznosti ako ziskat transformaciu zakladu
                            // 1
                            Transform3DGroup trans = f.GetFoundationTransformGroup_Complete();

                            // 2
                            //Transform3DGroup trans = new Transform3DGroup();
                            //trans.Children.Add(model3D.Transform);

                            Point3D p = trans.Transform(point3D); // Transformujeme povodny bod
                            foundationPoints.Add(p);
                        }

                        // Add member points to the main collection of all members
                        if (foundationPoints != null)
                        {
                            foreach (Point3D p in foundationPoints)
                            {
                                allFoundationPoints.Add(p);
                            }
                        }
                    }
                }

                fMax_X = Math.Max(fMax_X, (float)allFoundationPoints.Max(p => p.X));
                fMin_X = Math.Min(fMin_X, (float)allFoundationPoints.Min(p => p.X));
                fMax_Y = Math.Max(fMax_Y, (float)allFoundationPoints.Max(p => p.Y));
                fMin_Y = Math.Min(fMin_Y, (float)allFoundationPoints.Min(p => p.Y));
                fMax_Z = Math.Max(fMax_Z, (float)allFoundationPoints.Max(p => p.Z));
                fMin_Z = Math.Min(fMin_Z, (float)allFoundationPoints.Min(p => p.Z));
            }

            if (fMax_X == float.MinValue ||
            fMin_X == float.MaxValue ||
            fMax_Y == float.MinValue ||
            fMin_Y == float.MaxValue ||
            fMax_Z == float.MinValue ||
            fMin_Z == float.MaxValue)
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
            double distance = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, new Point3D(n.X, n.Y, n.Z)));

            if (distance < dLimit)
                return true;
            else
                return false;
        }

        public static bool MemberLiesOnPlane(Point3D p1, Point3D p2, Point3D p3, CMember m, double dLimit = 0.000001)
        {
            double distanceStart = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z)));
            double distanceEnd = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z)));

            if (distanceStart < dLimit && distanceEnd < dLimit)
                return true;
            else
                return false;
        }
        public static bool LineLiesOnPlane(Point3D p1, Point3D p2, Point3D p3, Point3D pLineStart, Point3D pLineEnd, double dLimit = 0.000001)
        {
            double distanceStart = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, pLineStart));
            double distanceEnd = Math.Abs(GetDistanceFromPointToPlane(p1, p2, p3, pLineEnd));

            if (distanceStart < dLimit && distanceEnd < dLimit)
                return true;
            else
                return false;
        }

        public static Rect GetRectanglesIntersection(Point p1r1, Point p2r1, Point p1r2, Point p2r2)
        {
            Rect r1 = new Rect(p1r1, p2r1);
            Rect r2 = new Rect(p1r2, p2r2);
            r1.Intersect(r2);
            return r1;
        }

        public static Rect GetRectanglesIntersection(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            return r1;
        }

        public static Point GetPoint_IgnoreZ(Point3D p)
        {
            return new Point(p.X, p.Y);
        }

        public static Vector3D GetSurfaceNormalVector(Point3D a, Point3D b, Point3D c)
        {
            var dir = Vector3D.CrossProduct(b - a, c - a);
            Vector3D norm = dir;
            norm.Normalize();
            return norm;
        }

        // TODO Ondrej - tato metoda by sa mala refaktorovat s tym co je v GraphObj, trieda CVolume
        // V tejto zlozke by mali byt metody pre generovanie geometrie pre vsetky 2D a 3D geometricke utvary

        public static GeometryModel3D GetCube(Point3D p, double size, Brush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh            
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(p.X - size, p.Y - size, p.Z - size));
            points.Add(new Point3D(p.X - size, p.Y + size, p.Z - size));
            points.Add(new Point3D(p.X + size, p.Y - size, p.Z - size));
            points.Add(new Point3D(p.X + size, p.Y + size, p.Z - size));

            points.Add(new Point3D(p.X - size, p.Y - size, p.Z + size));
            points.Add(new Point3D(p.X - size, p.Y + size, p.Z + size));
            points.Add(new Point3D(p.X + size, p.Y - size, p.Z + size));
            points.Add(new Point3D(p.X + size, p.Y + size, p.Z + size));

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection(points);

            // Add Positions of plate edge nodes
            List<int> indices = new List<int>() { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 2, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
            mesh.TriangleIndices = new Int32Collection(indices);

            model.Geometry = mesh;
            model.Material = new DiffuseMaterial(brush);  // Set Model Material
            return model;
        }

        //private static bool Intersects()
        //{
        //    MeshGeometry3D 3d = new MeshGeometry3D();

        //}

        public static CModel GetJointPreviewModel(CConnectionJointTypes joint, CFoundation pad, ref DisplayOptions sDisplayOptions)
        {
            CConnectionJointTypes jointClone = joint.Clone();
            CFoundation padClone = pad.Clone();
            if (pad != null) padClone.Visual_Object = pad.Visual_Object;

            CModel jointModel = null;

            if (jointClone != null)
            {
                // TO Ondrej - tuto funkciu treba trosku ucesat, refaktorovat casti kodu pre Main Member a cast pre Secondary Members

                // Problem 1 - ani jeden z uzlov pruta, ktore patria ku joint nekonci v spoji (vyskytuje sa najma pre main member, napr purlin pripojena k rafter)
                // TODO - problem je u main members ak nekoncia v uzle spoja
                // TODO - potrebujeme vytvorit funkciu (Drawing3D.cs PointLiesOnLineSegment), ktora najde vsetky "medzilahle" uzly, ktore lezia na prute, naplni nejaky zoznam uzlov v objekte pruta (List<CNode>IntermediateNodes)
                // TODO - potom vieme pre Main Member zistit, ktory z tychto uzlov je joint Node a vykreslit segment main member na jednu a na druhu stranu od tohto uzla

                // Problem 2 - joint nema spravne definovany main member (definovany je napr. ako main member prut rovnakeho typu s najnizsim ID)
                // TODO - vyssie uvedeny zoznam medzilahlych uzlov na prute vieme pouzit aj na to ze ak Main member nie je skutocny main member prisluchajuci ku spoju ale len prvy prut rovnakeho typu, 
                // tak mozeme najst taky prut, ktory ma v zozname IntermediateNodes joint.m_Node
                // a zaroven je rovnakeho typu ako main member, to by mal byt skutocny main member, ktory patri k joint.m_Node a mozeme ho nahradit
                // tento problem by sme mali riesit uz niekde pred touto funkciou, idealne uz pri vytvarani spojov v CModel_PFD_01_GR.cs

                float fMainMemberLength = 0;
                float fSecondaryMemberLength = 0;

                for (int i = 0; i < jointClone.m_arrPlates.Length; i++)
                {
                    fMainMemberLength = Math.Max(jointClone.m_arrPlates[i].Width_bx, jointClone.m_arrPlates[i].Height_hy);
                    fSecondaryMemberLength = fMainMemberLength;
                }

                float fMainMemberLengthFactor = 1.1f;      // Upravi dlzku urcenu z maximalneho rozmeru plechu
                float fSecondaryMemberLengthFactor = 1.1f; // Upravi dlzku urcenu z maximalneho rozmeru plechu // Bug 320 - Musi byt rovnake ako main member kvoli plechu Apex - jeden rafter je main, jeden je secondary

                fMainMemberLength *= fMainMemberLengthFactor;
                fSecondaryMemberLength *= fSecondaryMemberLengthFactor;

                jointModel = new CModel();

                //jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { joint };

                int iNumberMainMembers = 0;
                int iNumberSecondaryMembers = 0;

                if (jointClone.m_MainMember != null)
                    iNumberMainMembers = 1;

                if (jointClone.m_SecondaryMembers != null)
                    iNumberSecondaryMembers = jointClone.m_SecondaryMembers.Length;

                jointModel.m_arrMembers = new CMember[iNumberMainMembers + iNumberSecondaryMembers];

                // Main Member
                if (jointClone.m_MainMember != null)
                {
                    CMember m = jointClone.m_MainMember;

                    CNode nodeJoint = jointClone.m_Node; // Joint Node
                    CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta
                    float fX;
                    float fY;
                    float fZ;

                    if (jointClone.m_Node.ID == m.NodeStart.ID)
                    {
                        nodeOtherEnd = m.NodeEnd;
                        m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                        fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                        fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                        fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                        v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)v.X;
                        fY = (float)v.Y;
                        fZ = (float)v.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                        nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                        nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                    }
                    else if (jointClone.m_Node.ID == m.NodeEnd.ID)
                    {
                        nodeOtherEnd = m.NodeStart;
                        m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                        fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                        fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                        fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                        v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)v.X;
                        fY = (float)v.Y;
                        fZ = (float)v.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                        nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                        nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                    }
                    else
                    {
                        fMainMemberLength *= 2; // Zdvojnasobime vykreslovanu dlzku pruta kedze vykreslujeme na 2 strany od nodeJoint

                        // Relativny priemet casti pruta medzi zaciatocnym uzlom a uzlom spoja do GCS
                        fX = (m.NodeStart.X - nodeJoint.X) / m.FLength;
                        fY = (m.NodeStart.Y - nodeJoint.Y) / m.FLength;
                        fZ = (m.NodeStart.Z - nodeJoint.Z) / m.FLength;

                        // TO Ondrej - ak je prut velmi dlhy a fX az fZ su velmi male cisla, tak pre pripad ze jointNode je blizko k Start alebo End Node hlavneho pruta
                        // vyjde vzdialenost (fX, resp. fY, fZ) * fMainMemberLength velmi mala
                        // Urobil som to tak ze urcim vektor z absolutnych dlzok priemetu a potom ho normalizujem, takze absolutna vzdialenost priemetu nodeJoint a m.NodeStart, resp. m.NodeEnd
                        // by nemala hrat rolu, mozes sa na to pozriet. Mozno Ta napadne nieco elegantnejsie, rozdiel vidiet napriklad pri spoji girt to Main column

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D vStart = new Vector3D(m.NodeStart.X - nodeJoint.X, m.NodeStart.Y - nodeJoint.Y, m.NodeStart.Z - nodeJoint.Z);
                        vStart.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)vStart.X;
                        fY = (float)vStart.Y;
                        fZ = (float)vStart.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        // Nastavenie novych suradnic - zaciatok skrateneho (orezaneho) pruta
                        m.NodeStart.X = nodeJoint.X + fX * fMainMemberLength / 2;
                        m.NodeStart.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                        m.NodeStart.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                        // Relativny priemet casti pruta medzi uzlom spoja a koncovym uzlom do GCS
                        fX = (m.NodeEnd.X - nodeJoint.X) / m.FLength;
                        fY = (m.NodeEnd.Y - nodeJoint.Y) / m.FLength;
                        fZ = (m.NodeEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D vEnd = new Vector3D(m.NodeEnd.X - nodeJoint.X, m.NodeEnd.Y - nodeJoint.Y, m.NodeEnd.Z - nodeJoint.Z);
                        vEnd.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)vEnd.X;
                        fY = (float)vEnd.Y;
                        fZ = (float)vEnd.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        // Nastavenie novych suradnic - koniec skrateneho (orezaneho) pruta
                        m.NodeEnd.X = nodeJoint.X + fX * fMainMemberLength / 2;
                        m.NodeEnd.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                        m.NodeEnd.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                        m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                        m.FAlignment_End = 0;   // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    }

                    m.Fill_Basic();

                    jointModel.m_arrMembers[0] = m; // Set new member (member array)
                    jointClone.m_MainMember = m; // Set new member (joint)
                }

                // Secondary members
                if (jointClone.m_SecondaryMembers != null)
                {
                    for (int i = 0; i < jointClone.m_SecondaryMembers.Length; i++)
                    {
                        CMember m = jointClone.m_SecondaryMembers[i];

                        CNode nodeJoint = jointClone.m_Node; // Joint Node
                        CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta

                        if (jointClone.m_Node.ID == m.NodeStart.ID)
                        {
                            nodeOtherEnd = m.NodeEnd;
                            m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                        }
                        else
                        {
                            nodeOtherEnd = m.NodeStart;
                            m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                        }

                        float fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                        float fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                        float fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                        //-------------------------------------------------------------------------------------------------------------------------------
                        // TODO - Pokus vyriesit prilis kratke pruty
                        Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                        v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                        fX = (float)v.X;
                        fY = (float)v.Y;
                        fZ = (float)v.Z;
                        //-------------------------------------------------------------------------------------------------------------------------------

                        nodeOtherEnd.X = nodeJoint.X + fX * fSecondaryMemberLength;
                        nodeOtherEnd.Y = nodeJoint.Y + fY * fSecondaryMemberLength;
                        nodeOtherEnd.Z = nodeJoint.Z + fZ * fSecondaryMemberLength;

                        m.Fill_Basic();

                        jointModel.m_arrMembers[1 + i] = m; // Set new member (member array)
                        jointClone.m_SecondaryMembers[i] = m; // Set new member (joint)
                    }
                }

                List<CNode> nodeList = new List<CNode>();

                for (int i = 0; i < jointModel.m_arrMembers.Length; i++)
                {
                    // Pridavat len uzly ktore este neboli pridane
                    if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeStart) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeStart);
                    if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeEnd) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeEnd);
                }
                jointModel.m_arrNodes = nodeList.ToArray();

                //--------------------------------------------------------------------------------------------------------------------------------------
                // TO Ondrej - ked das zobrazit v preview joints wireframe, ak sa beru body este z povodnych celych prutov, niekde to treba updatovat, ale neviem kde :)))))
                // Tu je nejaky pokus
                // Prutom musime niekde updatetovat wire frame positions, neviem ci prave tu alebo to bude lepsie inde
                // Tu som docasne vyrobil 3D model prutov
                Model3DGroup membersModel = Drawing3D.CreateMembersModel3D(jointModel, !sDisplayOptions.bDistinguishedColor, sDisplayOptions.bTransparentMemberModel, sDisplayOptions.bUseDiffuseMaterial,
                    sDisplayOptions.bUseEmissiveMaterial, sDisplayOptions.bColorsAccordingToMembers, sDisplayOptions.bColorsAccordingToSections);

                // Tu sa snazim nastavit prutom Wireframe indices podla aktualnej geometrie
                for (int i = 0; i < jointModel.m_arrMembers.Length; i++)
                {
                    CMember m = jointModel.m_arrMembers[i];
                    m.WireFramePoints.Clear();

                    GeometryModel3D gm3d = membersModel.Children[i] as GeometryModel3D;
                    MeshGeometry3D mesh = gm3d.Geometry as MeshGeometry3D;

                    if (m.CrScStart.WireFrameIndices != null) // Validation of cross-section wireframe data
                    {
                        foreach (int n in m.CrScStart.WireFrameIndices)
                        {
                            m.WireFramePoints.Add(mesh.Positions[n]);
                        }
                    }
                }
                //--------------------------------------------------------------------------------------------------------------------------------------

                joint = joint.RecreateJoint(); // To Ondrej - toto sa mi uplne nepozdava, chceme len pregenerovat povodny joint a plechy z neho nastavit jointClone ? predtym sa nastavoval jointClone na joint a tym padom sa zmenili dlzky a body wireframe
                jointClone.m_arrPlates = joint.m_arrPlates;

                jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { jointClone };
            }

            // Footing Pad
            if (padClone != null)
            {
                sDisplayOptions.bDisplayFoundations = true; // Display always footing pads
                sDisplayOptions.bDisplayReinforcementBars = true; // Display always reinforcement bars

                if (jointModel == null)
                    jointModel = new CModel();

                jointModel.m_arrFoundations = new List<CFoundation>();
                jointModel.m_arrFoundations.Add(padClone);
            }

            return jointModel;
        }


        public static CModel GetModelAccordingToView(CModel model, DisplayOptions sDisplayOptions)
        {
            CModel _model = new CModel();
            _model.fL1_frame = model.fL1_frame;
            _model.fL_tot = model.fL_tot;
            _model.fW_frame = model.fW_frame;
            _model.fH1_frame = model.fH1_frame;

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.All)
            {
                return model;
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FRONT)
            {
                _model.m_arrMembers = ModelHelper.GetFrontViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetFrontViewNodes(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.BACK)
            {
                _model.m_arrMembers = ModelHelper.GetBackViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetBackViewNodes(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.LEFT)
            {
                _model.m_arrMembers = ModelHelper.GetLeftViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetLeftViewNodes(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.RIGHT)
            {
                _model.m_arrMembers = ModelHelper.GetRightViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetRightViewNodes(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.ROOF)
            {
                _model.m_arrMembers = ModelHelper.GetRoofViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetRoofViewNodes(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.MIDDLE_FRAME)
            {
                _model.m_arrMembers = ModelHelper.GetMiddleFrameMembers(model);
                _model.m_arrNodes = ModelHelper.GetMiddleFrameNodes(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.COLUMNS)
            {
                _model.m_arrMembers = ModelHelper.GetColumnsViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetColumnsViewNodes(model);
                _model.m_arrSlabs = ModelHelper.GetColumnsViewSlabs(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FOUNDATIONS)
            {
                // Ak vyberiem filter pre foundations, chcem aby sa automaticky nastavilo v display options zobrazenie foundations
                sDisplayOptions.bDisplayFoundations = true;
                sDisplayOptions.bDisplayReinforcementBars = true;
                sDisplayOptions.bDisplayFloorSlab = true;
                sDisplayOptions.bDisplayFoundationsDescription = true;

                _model.m_arrMembers = ModelHelper.GetColumnsViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetColumnsViewNodes(model);
                _model.m_arrFoundations = ModelHelper.GetColumnsViewFoundations(model);
                _model.m_arrSlabs = ModelHelper.GetColumnsViewSlabs(model);
            }
            else if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FLOOR)
            {
                // Ak vyberiem filter pre floor slabs, chcem aby sa automaticky nastavilo v display options zobrazenie slabs
                sDisplayOptions.bDisplayFloorSlab = true;
                sDisplayOptions.bDisplaySawCuts = true;
                sDisplayOptions.bDisplayControlJoints = true;

                sDisplayOptions.bDisplayFloorSlabDescription = true;
                sDisplayOptions.bDisplayFoundationsDescription = false;

                _model.m_arrMembers = ModelHelper.GetColumnsViewMembers(model);
                _model.m_arrNodes = ModelHelper.GetColumnsViewNodes(model);
                _model.m_arrFoundations = ModelHelper.GetColumnsViewFoundations(model);
                _model.m_arrSlabs = ModelHelper.GetColumnsViewSlabs(model);

                _model.m_arrSawCuts = model.m_arrSawCuts;
                _model.m_arrControlJoints = model.m_arrControlJoints;
            }

            return _model;
        }

        private static void SetLabelsUpAndOverVectors(DisplayOptions sDisplayOptions, float fTextBlockHorizontalSizeFactor, float fTextBlockVerticalSizeFactor, out Vector3D over, out Vector3D up)
        {
            over = new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0);
            up = new Vector3D(0, 0, fTextBlockVerticalSizeFactor);

            if (sDisplayOptions.ModelView == (int)EModelViews.FRONT)
            {
                //over = new Vector3D(fTextBlockHorizontalSizeFactor, 0, 0);
                //up = new Vector3D(0, 0, fTextBlockVerticalSizeFactor);
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.BACK)
            {
                over = new Vector3D(-fTextBlockHorizontalSizeFactor, 0, 0);
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.LEFT)
            {
                over = new Vector3D(0, -fTextBlockHorizontalSizeFactor, 0);
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.RIGHT)
            {
                over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
            }
            else if (sDisplayOptions.ModelView == (int)EModelViews.TOP)
            {
                over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
                up = new Vector3D(-fTextBlockVerticalSizeFactor, 0, 0);
            }
            /*
            else if (sDisplayOptions.ModelView == (int)EModelViews.BOTTOM)
            {
                over = new Vector3D(0, fTextBlockHorizontalSizeFactor, 0);
                up = new Vector3D(fTextBlockVerticalSizeFactor, 0, 0);
            }*/
        }

        private static void TransformVectorsFromLCSAxisToGCSAxis(CMember m,
            Transform3DGroup transform,
            Vector3D memberAxisVectorInLCS, // Vektor v LCS
            out Vector3D memberLCSAxis_VectorInGCS // Vektor v GCS
            )
        {
            // To neviem ci sa pouzije
            //Vector3D memberVectorInGCS = new Vector3D(m.Delta_X, m.Delta_Y, m.Delta_Z);
            //memberVectorInGCS.Normalize(); // Normalizujem vektor, aby sa ignorovala dlzka priemetu pruta

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Urobit transformaciu priamo pre Vektor3D
            // Transformaciu z LCS do GCS aplikujeme na jednotlive lokalne osi pruta, ziskame tak ich vektor v GCS
            // TO Ondrej - myslim ze toto uz mame niekde pri generovani zatazeni urobene aj priamo pre vektor, aby sa to neuselo prevadzat cez point, ale neviem kde
            Point3D pLCSAxisVector = transform.Transform(new Point3D(memberAxisVectorInLCS.X, memberAxisVectorInLCS.Y, memberAxisVectorInLCS.Z));
            //Vector3D pLCSAxisVector = transform.Transform(new Vector3D(memberAxisVectorInLCS.X, memberAxisVectorInLCS.Y, memberAxisVectorInLCS.Z));

            // Chceme uplatnit len rotacne transformacie, nie posun
            pLCSAxisVector.X -= m.NodeStart.X;
            pLCSAxisVector.Y -= m.NodeStart.Y;
            pLCSAxisVector.Z -= m.NodeStart.Z;

            memberLCSAxis_VectorInGCS = new Vector3D(pLCSAxisVector.X, pLCSAxisVector.Y, pLCSAxisVector.Z);

            // TO Ondrej - Trosku sa mi nezdava co tu ziskam ako zlozky vektorov :-/ ak je prut zvislo to tak by som chcel len hodnoty 0,-1,1
            memberLCSAxis_VectorInGCS.Normalize(); // Normalizujem vektor, aby sa ignorovala dlzka priemetu pruta

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        private static void TransformTextVectorsFromLCSAxisToViewAxis(
            float fTextBlockHorizontalSizeFactor,
            float fTextBlockVerticalSizeFactor,
            CMember m,
            Transform3DGroup transform,
            Matrix3D matrixViewInGCS_Inverse,
            Vector3D over_LCS,
            Vector3D up_LCS,
            out Vector3D over,
            out Vector3D up
    )
        {
            // Vytvorime vektory pre urcenie smeru textu
            // To Ondrej - ako tak rozmyslam tak pre zakladny pohlad ked su vsetky texty zobrazene horizontalne a defaultne podla front je to up (0,0,1) a over (1,0,0),
            // asi by sa dalo urcit o kolko mas v pohlade pootoceny model oproti pohladu front okolo Z a podla toho by sa dalo rotovat text pocas manipulacie, tak aby bol vzdy kolmy na obrazovku
            // podobne pre potocenie modelu okolo osi X a Y

            // Nadobudane hodnoty by mali byt 0,-1, 1 (moze byt ine jedine pre sikme pruty ako su rafters alebo purlins)

            // Tato transformacia by sa mala nahradit transformaciou z LCS do VIEW AXIS
            Vector3D over_InGCS;
            TransformVectorsFromLCSAxisToGCSAxis(m, transform, over_LCS, out over_InGCS);

            // Transformujeme vektor z GCS do View ??? Toto by sa malo urobit, ale ked som to skusal tak to vyzeralo nejako horsie. Zamysli sa :)
            Vector3D over_InView = over_InGCS;// * matrixViewInGCS_Inverse;

            Vector3D up_InGCS;
            TransformVectorsFromLCSAxisToGCSAxis(m, transform, up_LCS, out up_InGCS);

            // Transformujeme vektor z GCS do View ??? Toto by sa malo urobit, ale ked som to skusal tak to vyzeralo nejako horsie. Zamysli sa :)
            Vector3D up_InView = up_InGCS;// * matrixViewInGCS_Inverse;

            // Finalne vektory (prenasobenie faktorom velkosti textbloku)
            over = new Vector3D(over_InView.X * fTextBlockHorizontalSizeFactor, over_InView.Y * fTextBlockHorizontalSizeFactor, over_InView.Z * fTextBlockHorizontalSizeFactor);
            up = new Vector3D(up_InView.X * fTextBlockVerticalSizeFactor, up_InView.Y * fTextBlockVerticalSizeFactor, up_InView.Z * fTextBlockVerticalSizeFactor);
        }


        // TO Ondrej - musi byt toto vracat Model3DGroup? Nastaci GeometryModel3D
        // Poredpokladam ze budeme celu ciaru kreslit rovnakou farbou, takze nepotrebujeme viacero materialov a mensich modelov pre rozne povrchy
        public static GeometryModel3D Get3DLineReplacement(Color color, float fLineThickness, Point3D pA, Point3D pB)
        {
            // TO Ondrej - ak chces pouzivat triedu R3, tak asi by stalo zato dat to vsetko nejako dokopy s Point3D a CNode a CPoint, uz som toho navytvaral vela :)
            // Potom pracne prevazdam hore dole mezi sebou tie objekty a pritom je to stale len bod v 2D alebo v 3D, akurat ze raz ma ID alebo nejaku inu pridavnu vlastnost

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color));

            float fLineCylinderRadius = fLineThickness / 2; //0.05f; // Polomer valca ako polovica hrubky ciary

            short NumberOfCirclePoints = 9; // Osem uholnik + stredovy bod (je pocet bodov len 4+1 tak moze mat ciara inu hrubku podla toho ako je hranol otoceny)

            // Priemet do osi GCS - rozdiel suradnic v GCS
            double Delta_X = pB.X - pA.X;
            double Delta_Y = pB.Y - pA.Y;
            double Delta_Z = pB.Z - pA.Z;

            //float distance = (float)Math.Sqrt((float)Math.Pow(Delta_X, 2f) + (float)Math.Pow(Delta_Y, 2f) + (float)Math.Pow(Delta_Z, 2f));
            float distance = pA.GetDistanceTo(pB);

            // Model valca v smere jeho lokalnej osi x
            // TO Ondrej - control point valca ma byt 0,0,0 kedze presun do bodu pA je zohladneny vo funkcii TransformMember_LCStoGCS
            // Model valca som nahradil len plastom a nebudeme kreslit hornu a spodnu podstavu, to bz malo pre "ciaru" postacovat
            // TODO - zjednotit triedy Cylinder a funkcie z CVolume pre valec
            GeometryModel3D gm3D = CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), NumberOfCirclePoints, fLineCylinderRadius, distance, material, 0, false, false);

            // Transform cylinder from its LCS to GCS
            CMember m = new CMember();
            Point3DCollection points = m.TransformMember_LCStoGCS(EGCS.eGCSLeftHanded, pA, Delta_X, Delta_Y, Delta_Z, 0, ((MeshGeometry3D)gm3D.Geometry).Positions);

            //set transformed points
            ((MeshGeometry3D)gm3D.Geometry).Positions = points;


            return gm3D;
        }



        private static void DrawDimensionsToTrackport(Trackport3D _trackport, DisplayOptions sDisplayOptions, CModel model, Model3DGroup gr)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TO ONDREJ POKUS KRESLIT KOTY V 3D ako 3D OBJEKTY, nie ciary

            // TODO - Ondrej - tieto if pre jednotlive pohlady je potrebne poriadne prejst a zrefaktorovat, je tu kopec nakopirovaneho kodu
            // Ide o to ze nejakym sposobom ziskame zoznam uzlov alebo bodov ktore chceme kotovat a potom vyrabame koty a texty
            // Kota moze byt jedna alebo niekolko za sebou v linii - retazove kóty
            // Dalej mozu byt dalsie koty pridavane s nejakym odstupom voci prvej kolmo na hlavnu kotovaciu ciaru, tak ziskame niekolko kot pod sebou
            // Ja ich tu mam 1 - 3

            Model3DGroup dimensions3DGroup = null;

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FRONT)
            {
                CMember m1 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
                CMember m2 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);

                CDimensionLinear3D dimPOKUSNA1 = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m2.NodeEnd.GetPoint3D(), new Vector3D(0, 0, -1), 2, -1, 0, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), 0.5, 0.4, 0.15, (model.fW_frame * 1000).ToString("F0"));
                CDimensionLinear3D dimPOKUSNA2 = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m1.NodeEnd.GetPoint3D(), new Vector3D(-1, 0, 0), 2, 0, -1, new Vector3D(0, 0, 1), new Vector3D(-1, 0, 0), 0.5, 0.4, 0.15, (model.fH1_frame * 1000).ToString("F0"));

                CMember m3 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.MainColumn);
                CMember m4 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.MainColumn);

                List<CDimensionLinear3D> listOfDimensions = new List<CDimensionLinear3D> { dimPOKUSNA1, dimPOKUSNA2 };

                // Create Dimensions
                if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                if (dimensions3DGroup != null)
                {
                    foreach (CDimensionLinear3D dim in listOfDimensions)
                    {
                        DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                    }
                }
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.LEFT)
            {
                // Najdeme girts na lavej strane
                // Girts
                CMember[] membersLeftSideGirts = null;

                membersLeftSideGirts = ModelHelper.GetMembersInDistance(model, 0, 0, EMemberType_FS.eG); // smer X

                CMember[] membersLeftSideFirstBayGirts = ModelHelper.GetMembersInDistanceInterval(model, 0, model.fL1_frame, 1, EMemberType_FS.eG, true, true, false); // smer Y

                if (membersLeftSideFirstBayGirts != null)
                {
                    // 1 kotovacia ciara - vsetky girts
                    bool bDrawDimension_1 = true;

                    // Pripravime si zoznamy kotovanych bodov
                    List<CNode> membersLeftSideFirstBayGirtsNodes_1 = null;
                    membersLeftSideFirstBayGirtsNodes_1 = new List<CNode>();

                    // Kedze chceme kotovat od hrany musime pridat uzly na krajoch
                    membersLeftSideFirstBayGirtsNodes_1.Add(new CNode(0, 0, model.fL1_frame, 0, 0));
                    membersLeftSideFirstBayGirtsNodes_1.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));

                    foreach (CMember m in membersLeftSideFirstBayGirts)
                    {
                        if (MathF.d_equal(m.NodeStart.Y, model.fL1_frame))
                        {
                            membersLeftSideFirstBayGirtsNodes_1.Add(m.NodeStart);
                        }

                        if (MathF.d_equal(m.NodeEnd.Y, model.fL1_frame))
                        {
                            membersLeftSideFirstBayGirtsNodes_1.Add(m.NodeEnd);
                        }
                    }

                    if (bDrawDimension_1 == false)
                        membersLeftSideFirstBayGirtsNodes_1 = null;

                    // Mame pripraveny zoznam bodov
                    // Body zoradime podla Z od najvacsieho - koty kreslime , resp z +X smerom k 0

                    // TO Ondrej - toto momentalne neplati lebo to by sme museli implementovat rotaciu kot
                    // kreslim a radim od 0 smerom k +Z

                    if (membersLeftSideFirstBayGirtsNodes_1 != null)
                        membersLeftSideFirstBayGirtsNodes_1 = membersLeftSideFirstBayGirtsNodes_1.OrderBy(n => n.Z).ToList();

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou

                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersLeftSideFirstBayGirtsNodes_1.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersLeftSideFirstBayGirtsNodes_1[i].GetPoint3D(), membersLeftSideFirstBayGirtsNodes_1[i + 1].GetPoint3D(), new Vector3D(0, 1, 0), 2, 0, 1, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersLeftSideFirstBayGirtsNodes_1[i + 1].Z - membersLeftSideFirstBayGirtsNodes_1[i].Z) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    // Create Dimensions 3D Model
                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.RIGHT)
            {
                CMember m1 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
                CMember m2 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);

                // stlpy na pravej strane maju PointEnd v Z = 0
                CDimensionLinear3D dimPOKUSNA1 = new CDimensionLinear3D(m1.NodeEnd.GetPoint3D(), m2.NodeEnd.GetPoint3D(), new Vector3D(0, 0, -1), 2, 1, 0, new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), 0.5, 0.4, 0.15, (model.fL_tot * 1000).ToString("F0"));

                // stlp vlavo - vyskova kota
                CDimensionLinear3D dimPOKUSNA2 = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m1.NodeEnd.GetPoint3D(), new Vector3D(0, 1, 0), 2, 0, 1, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), 0.5, 0.4, 0.15, (model.fH1_frame * 1000).ToString("F0"));

                List<CDimensionLinear3D> listOfDimensions = new List<CDimensionLinear3D> { dimPOKUSNA1, dimPOKUSNA2 };

                // Create Dimensions
                if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                if (dimensions3DGroup != null)
                {
                    foreach (CDimensionLinear3D dim in listOfDimensions)
                    {
                        DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                    }
                }
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.COLUMNS)
            {
                // Potrebujeme idenfikovat ktore pruty (stlpy) su, na ktorej strane a vyrobit medzi nimi koty

                // Front side
                CMember[] membersFrontSide = null;

                membersFrontSide = ModelHelper.GetMembersInDistance(model, 0, 1); // smer Y

                // Back side
                CMember[] membersBackSide = null;

                membersBackSide = ModelHelper.GetMembersInDistance(model, model.fL_tot, 1); // smer Y

                // Left side
                CMember[] membersLeftSide = null;

                membersLeftSide = ModelHelper.GetMembersInDistance(model, 0, 0); // smer X

                // Right side
                CMember[] membersRightSide = null;

                membersRightSide = ModelHelper.GetMembersInDistance(model, model.fW_frame, 0); // smer X

                // TO Ondrej - ak by sa toto zobecnilo mohlo by sa to pouzit aj v inych pohladoch alebo podorysoch

                if (membersFrontSide != null)
                {
                    // 1 kotovacia ciara - vsetky stlpy
                    // 2 kotovacia ciara - vsetky MC a EC (len left) alebo WP/C a EC (front a back)
                    // 3 kotovacia ciara - celkovy rozmer (len left a front)

                    bool bDrawDimension_1 = false;
                    bool bDrawDimension_2 = false;
                    bool bDrawDimension_3 = true;
                    // Pripravime si zoznamy kotovanych bodov

                    // float fh = (float)model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h;
                    float fh = (float)model.m_arrMembers[0].CrScStart.h; // Docasne, prvy prut by mal byt Main Column
                                                                         // Front side
                    List<CNode> membersBaseNodes_FrontSide_1 = null;
                    List<CNode> membersBaseNodes_FrontSide_2 = null; // Wind posts and edge columns
                    List<CNode> membersBaseNodes_FrontSide_3 = null; // Edges

                    // Tuto celkovu kotu kreslime vzdy
                    membersBaseNodes_FrontSide_3 = new List<CNode>();
                    membersBaseNodes_FrontSide_3.Add(new CNode(0, -0.5f * fh, 0, 0, 0)); // Suradnice sa menia pre smer X
                    membersBaseNodes_FrontSide_3.Add(new CNode(0, model.fW_frame + 0.5f * fh, 0, 0, 0));

                    membersBaseNodes_FrontSide_1 = new List<CNode>();
                    membersBaseNodes_FrontSide_2 = new List<CNode>();

                    // Kedze chceme kotovat od hrany musime pridat uzly na krajoch
                    membersBaseNodes_FrontSide_1.Add(new CNode(0, -0.5f * fh, 0, 0, 0));
                    membersBaseNodes_FrontSide_1.Add(new CNode(0, model.fW_frame + 0.5f * fh, 0, 0, 0));

                    membersBaseNodes_FrontSide_2.Add(new CNode(0, -0.5f * fh, 0, 0, 0));
                    membersBaseNodes_FrontSide_2.Add(new CNode(0, model.fW_frame + 0.5f * fh, 0, 0, 0));

                    foreach (CMember m in membersFrontSide)
                    {
                        if (MathF.d_equal(m.NodeStart.Z, 0))
                        {
                            membersBaseNodes_FrontSide_1.Add(m.NodeStart);

                            if (m.EMemberType == EMemberType_FS.eC || m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eWP)
                                membersBaseNodes_FrontSide_2.Add(m.NodeStart);
                        }

                        if (MathF.d_equal(m.NodeEnd.Z, 0))
                        {
                            membersBaseNodes_FrontSide_1.Add(m.NodeEnd);

                            if (m.EMemberType == EMemberType_FS.eC || m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eWP)
                                membersBaseNodes_FrontSide_2.Add(m.NodeEnd);
                        }
                    }

                    // Ak sa v zozname na urovni 1 nenachadzaju stlpy ktore patria dveram, kotu nekreslime a zoznam zmazeme
                    // To Ondrej - toto by sa asi dalo zistit uz vopred este predtym nez vyrabam zoznamy uzlov
                    foreach (CMember m in membersFrontSide)
                    {
                        if (!bDrawDimension_1 && (m.EMemberType == EMemberType_FS.eDF || m.EMemberType == EMemberType_FS.eDT))
                        {
                            bDrawDimension_1 = true;
                        }

                        if (!bDrawDimension_2 && (m.EMemberType == EMemberType_FS.eC || m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eWP))
                        {
                            bDrawDimension_2 = true;
                        }
                    }

                    if (bDrawDimension_1 == false)
                        membersBaseNodes_FrontSide_1 = null;

                    if (bDrawDimension_2 == false)
                        membersBaseNodes_FrontSide_2 = null;

                    // Mame pripravene 3 zoznamy bodov
                    // Body zoradime podla X od najvacsieho - koty kreslime zhora nadol, resp z +X smerom k 0

                    // TO Ondrej - toto momentalne neplati lebo to by sme museli implementovat rotaciu kot
                    // kreslim a radim od 0 smerom k +X

                    if (membersBaseNodes_FrontSide_1 != null)
                        membersBaseNodes_FrontSide_1 = membersBaseNodes_FrontSide_1.OrderBy(n => n.X).ToList();

                    if (membersBaseNodes_FrontSide_2 != null)
                        membersBaseNodes_FrontSide_2 = membersBaseNodes_FrontSide_2.OrderBy(n => n.X).ToList();

                    if (membersBaseNodes_FrontSide_3 != null)
                        membersBaseNodes_FrontSide_3 = membersBaseNodes_FrontSide_3.OrderBy(n => n.X).ToList();

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou

                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSide_1.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSide_1[i].GetPoint3D(), membersBaseNodes_FrontSide_1[i + 1].GetPoint3D(), new Vector3D(0, 0, -1), 0, 0, -1, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_FrontSide_1[i + 1].X - membersBaseNodes_FrontSide_1[i].X) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_2 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSide_2.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSide_2[i].GetPoint3D(), membersBaseNodes_FrontSide_2[i + 1].GetPoint3D(), new Vector3D(0, 0, -1), 0, 0, -1, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_FrontSide_2[i + 1].X - membersBaseNodes_FrontSide_2[i].X) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = +fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_3 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSide_3.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSide_3[i].GetPoint3D(), membersBaseNodes_FrontSide_3[i + 1].GetPoint3D(), new Vector3D(0, 0, -1), 0, 0, -1, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_FrontSide_3[i + 1].X - membersBaseNodes_FrontSide_3[i].X) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }
                    }

                    // Create Dimensions 3D Model
                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }

                // TODO Ondrej - niektore casti kodu pre jednotlive steny front a left by sli zrefaktorovat a zjednotit do funkcii
                // !!! menia sa pouzivane suradnice, pre front X a pre left Y, napriklad pre urcenie textu koty alebo zoradenie bodov koty
                // TODO - pridat kreslenie kot pre right a back, kota cislo 2 a 3 sa pre tieto strany nema zobrazit, staci zobrazit kotu cislo 1 ak existuju nejake dvere v pravej alebo zadnej stene
                // TO Ondrej - mozno by bolo dobre zaviest objekt Continuous Dimension, to by bola vlastne skupina kot, ktora ma Main Line v jednej priamke a postupne na seba nadvazuju, Extension lines mozu byt rozne dlhe ale ostatne parametre by boli podobne, dalo by sa s nou pracovat ako s celkom

                // Potrebovali by sme vymysliet co spravime ak je kota taka kratka ze sa sipky nemaju kam vykreslit a ani na text tam nie je miesto, zvycajne sa to robi cez odkazovu ciaru
                // Mozeme to urobit aj tak ze taka mini kota tam proste teraz nebude zobrazena a hotovo :)

                // TO Ondrej - ak by sa toto zobecnilo mohlo by sa to pouzit aj v inych pohladoch alebo podorysoch

                if (membersLeftSide != null)
                {
                    // 1 kotovacia ciara - vsetky stlpy
                    // 2 kotovacia ciara - vsetky MC a EC (len left) alebo WP/C a EC (front a back)
                    // 3 kotovacia ciara - cerlkovy rozmer (len left a front)

                    bool bDrawDimension_1 = false;
                    bool bDrawDimension_2 = false;
                    bool bDrawDimension_3 = true;
                    // Pripravime si zoznamy kotovanych bodov

                    // float fb = (float)model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].b;
                    float fb = (float)model.m_arrMembers[0].CrScStart.b; // Docasne, prvy prut by mal byt Main Column

                    // Left side
                    List<CNode> membersBaseNodes_LeftSide_1 = null;
                    List<CNode> membersBaseNodes_LeftSide_2 = null; // Main columns and edge columns
                    List<CNode> membersBaseNodes_LeftSide_3 = null; // Edges

                    // Toto celkovu kotu kreslime vzdy
                    membersBaseNodes_LeftSide_3 = new List<CNode>();
                    membersBaseNodes_LeftSide_3.Add(new CNode(0, 0, -0.5f * fb, 0, 0)); // Suradnice sa menia pre smer Y
                    membersBaseNodes_LeftSide_3.Add(new CNode(0, 0, model.fL_tot + 0.5f * fb, 0, 0));

                    membersBaseNodes_LeftSide_1 = new List<CNode>();
                    membersBaseNodes_LeftSide_2 = new List<CNode>();

                    // Kedze chceme kotovat od hrany musime pridat uzly na krajoch
                    membersBaseNodes_LeftSide_1.Add(new CNode(0, 0, -0.5f * fb, 0, 0));
                    membersBaseNodes_LeftSide_1.Add(new CNode(0, 0, model.fL_tot + 0.5f * fb, 0, 0));

                    membersBaseNodes_LeftSide_2.Add(new CNode(0, 0, -0.5f * fb, 0, 0));
                    membersBaseNodes_LeftSide_2.Add(new CNode(0, 0, model.fL_tot + 0.5f * fb, 0, 0));

                    foreach (CMember m in membersLeftSide)
                    {
                        if (MathF.d_equal(m.NodeStart.Z, 0))
                        {
                            membersBaseNodes_LeftSide_1.Add(m.NodeStart);

                            if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC)
                                membersBaseNodes_LeftSide_2.Add(m.NodeStart);
                        }

                        if (MathF.d_equal(m.NodeEnd.Z, 0))
                        {
                            membersBaseNodes_LeftSide_1.Add(m.NodeEnd);

                            if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC)
                                membersBaseNodes_LeftSide_2.Add(m.NodeEnd);
                        }
                    }

                    // Ak sa v zozname na urovni 1 nenachadzaju stlpy ktore patria dveram, kotu nekreslime a zoznam zmazeme
                    // To Ondrej - toto by sa asi dalo zistit uz vopred este predtym nez vyrabam zoznamy uzlov
                    foreach (CMember m in membersLeftSide)
                    {
                        if (!bDrawDimension_1 && (m.EMemberType == EMemberType_FS.eDF || m.EMemberType == EMemberType_FS.eDT))
                        {
                            bDrawDimension_1 = true;
                        }

                        if (!bDrawDimension_2 && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC))
                        {
                            bDrawDimension_2 = true;
                        }
                    }

                    if (bDrawDimension_1 == false)
                        membersBaseNodes_LeftSide_1 = null;

                    if (bDrawDimension_2 == false)
                        membersBaseNodes_LeftSide_2 = null;

                    // Mame pripravene 3 zoznamy bodov
                    // Body zoradime podla Y od najmensieho - koty kreslime zlava smerom doprava, resp od 0 do +Y
                    if (membersBaseNodes_LeftSide_1 != null)
                        membersBaseNodes_LeftSide_1 = membersBaseNodes_LeftSide_1.OrderBy(n => n.Y).ToList();

                    if (membersBaseNodes_LeftSide_2 != null)
                        membersBaseNodes_LeftSide_2 = membersBaseNodes_LeftSide_2.OrderBy(n => n.Y).ToList();

                    if (membersBaseNodes_LeftSide_3 != null)
                        membersBaseNodes_LeftSide_3 = membersBaseNodes_LeftSide_3.OrderBy(n => n.Y).ToList();

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou

                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_LeftSide_1.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_LeftSide_1[i].GetPoint3D(), membersBaseNodes_LeftSide_1[i + 1].GetPoint3D(), new Vector3D(0, 0, -1), 0, 1, 0, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_LeftSide_1[i + 1].Y - membersBaseNodes_LeftSide_1[i].Y) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_2 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_LeftSide_2.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_LeftSide_2[i].GetPoint3D(), membersBaseNodes_LeftSide_2[i + 1].GetPoint3D(), new Vector3D(0, 0, -1), 0, 1, 0, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_LeftSide_2[i + 1].Y - membersBaseNodes_LeftSide_2[i].Y) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = +fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_3 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_LeftSide_3.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_LeftSide_3[i].GetPoint3D(), membersBaseNodes_LeftSide_3[i + 1].GetPoint3D(), new Vector3D(0, 0, -1), 0, 1, 0, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_LeftSide_3[i + 1].Y - membersBaseNodes_LeftSide_3[i].Y) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }
                    }

                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.MIDDLE_FRAME)
            {
                // Potrebujeme identifikovat girts, purlins, eave purlins ???
                // Na ram sa pripajaju z oboch stran, vyberieme len tie, ktore na rame koncia a vidime ich v pohlade na ram zpredu, aby sme nemali body duplicitne

                // Girts
                CMember[] membersFrontSideGirts = null;

                membersFrontSideGirts = ModelHelper.GetMembersInDistanceInterval(model, 0, model.fL1_frame, 1, EMemberType_FS.eG, true, true, false); // smer Y

                // Purlins
                CMember[] membersFrontSidePurlins = null;

                membersFrontSidePurlins = ModelHelper.GetMembersInDistanceInterval(model, 0, model.fL1_frame, 1, EMemberType_FS.eP, true, true, false); // smer Y

                List<CNode> membersBaseNodes_FrontSideGirts_1 = null; // Girts
                                                                      // Tuto celkovu kotu kreslime vzdy
                List<CNode> membersBaseNodes_FrontSideVertical_2 = new List<CNode>(); // Overall knee height H1

                List<CNode> membersBaseNodes_FrontSidePurlins_1 = null; // Purlins
                                                                        // Tuto celkovu kotu kreslime vzdy
                List<CNode> membersBaseNodes_FrontSideRafter_2 = new List<CNode>(); // Overall rafter length

                // Kedze chceme kotovat od spodnej hrany a vo vyske H1 musime pridat uzly na koncoch stlpa
                membersBaseNodes_FrontSideVertical_2.Add(new CNode(0, 0, model.fL1_frame, 0, 0));
                membersBaseNodes_FrontSideVertical_2.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));

                // Kedze chceme kotovat od zaciatku po koniec raftera musime pridat uzly na koncoch
                CMember leftRafter = model.m_arrMembers.FirstOrDefault(m => m.EMemberType == EMemberType_FS.eMR);

                membersBaseNodes_FrontSideRafter_2.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));
                membersBaseNodes_FrontSideRafter_2.Add(new CNode(0, leftRafter.NodeEnd.X, model.fL1_frame, leftRafter.NodeEnd.Z, 0));

                if (membersFrontSideGirts != null)
                {
                    membersBaseNodes_FrontSideGirts_1 = new List<CNode>(); // Girts

                    membersBaseNodes_FrontSideGirts_1.Add(new CNode(0, 0, model.fL1_frame, 0, 0));
                    membersBaseNodes_FrontSideGirts_1.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));

                    foreach (CMember m in membersFrontSideGirts)
                    {
                        if (MathF.d_equal(m.NodeEnd.X, 0) && MathF.d_equal(m.NodeEnd.Y, model.fL1_frame)) // Koncovy bod pruta typu girt je na rame // Girt je na lavom stlpe
                        {
                            membersBaseNodes_FrontSideGirts_1.Add(m.NodeEnd);
                        }
                    }
                }

                if (membersFrontSidePurlins != null)
                {
                    membersBaseNodes_FrontSidePurlins_1 = new List<CNode>(); // Purlins

                    membersBaseNodes_FrontSidePurlins_1.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));
                    membersBaseNodes_FrontSidePurlins_1.Add(new CNode(0, leftRafter.NodeEnd.X, model.fL1_frame, leftRafter.NodeEnd.Z, 0));

                    foreach (CMember m in membersFrontSidePurlins)
                    {
                        if ((m.NodeEnd.X < leftRafter.NodeEnd.X) && MathF.d_equal(m.NodeEnd.Y, model.fL1_frame)) // Koncovy bod pruta typu purlin je na rame // Purlin je na lavom raftery
                        {
                            membersBaseNodes_FrontSidePurlins_1.Add(m.NodeEnd);
                        }
                    }
                }

                bool bDrawDimensionsOnWallMembers = true;
                bool bDrawDimesnionsOnRoofMembers = true;

                if (bDrawDimensionsOnWallMembers)
                {
                    // 1 kotovacia ciara - vsetky girts
                    // 2 kotovacia ciara - celkovy rozmer - vyska stlpa

                    bool bDrawDimension_1 = false;
                    bool bDrawDimension_2 = true;

                    if (membersBaseNodes_FrontSideGirts_1 != null)
                        membersBaseNodes_FrontSideGirts_1 = membersBaseNodes_FrontSideGirts_1.OrderBy(n => n.Z).ToList();

                    if (membersBaseNodes_FrontSideVertical_2 != null)
                        membersBaseNodes_FrontSideVertical_2 = membersBaseNodes_FrontSideVertical_2.OrderBy(n => n.Z).ToList();

                    if (membersBaseNodes_FrontSideGirts_1 != null)
                        bDrawDimension_1 = true;

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou
                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSideGirts_1.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSideGirts_1[i].GetPoint3D(), membersBaseNodes_FrontSideGirts_1[i + 1].GetPoint3D(), new Vector3D(-1, 0, 0), 1, 0, -1, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_FrontSideGirts_1[i + 1].Z - membersBaseNodes_FrontSideGirts_1[i].Z) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_2 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSideVertical_2.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSideVertical_2[i].GetPoint3D(), membersBaseNodes_FrontSideVertical_2[i + 1].GetPoint3D(), new Vector3D(-1, 0, 0), 1, 0, -1, new Vector3D(0, 0, 1), new Vector3D(-1, 0, 0), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((membersBaseNodes_FrontSideVertical_2[i + 1].Z - membersBaseNodes_FrontSideVertical_2[i].Z) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = +fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }

                if (bDrawDimesnionsOnRoofMembers)
                {
                    // 1 kotovacia ciara - vsetky purlins
                    // 2 kotovacia ciara - celkovy rozmer - dlzka raftera

                    bool bDrawDimension_1 = false;
                    bool bDrawDimension_2 = true;

                    if (membersBaseNodes_FrontSidePurlins_1 != null)
                        membersBaseNodes_FrontSidePurlins_1 = membersBaseNodes_FrontSidePurlins_1.OrderBy(n => n.Z).ToList();

                    if (membersBaseNodes_FrontSideRafter_2 != null)
                        membersBaseNodes_FrontSideRafter_2 = membersBaseNodes_FrontSideRafter_2.OrderBy(n => n.Z).ToList();

                    if (membersBaseNodes_FrontSidePurlins_1 != null)
                        bDrawDimension_1 = true;

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou
                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSidePurlins_1.Count - 1; i++)
                        {
                            float fLengthForText = MathF.Sqrt(MathF.Pow2(membersBaseNodes_FrontSidePurlins_1[i + 1].X - membersBaseNodes_FrontSidePurlins_1[i].X) + MathF.Pow2(membersBaseNodes_FrontSidePurlins_1[i + 1].Z - membersBaseNodes_FrontSidePurlins_1[i].Z));
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSidePurlins_1[i].GetPoint3D(), membersBaseNodes_FrontSidePurlins_1[i + 1].GetPoint3D(), new Vector3D(0, 0, 1), 1, 0, 0, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, (fLengthForText * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_2 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < membersBaseNodes_FrontSideRafter_2.Count - 1; i++)
                        {
                            float fLengthForText = MathF.Sqrt(MathF.Pow2(membersBaseNodes_FrontSideRafter_2[i + 1].X - membersBaseNodes_FrontSideRafter_2[i].X) + MathF.Pow2(membersBaseNodes_FrontSideRafter_2[i + 1].Z - membersBaseNodes_FrontSideRafter_2[i].Z));
                            CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSideRafter_2[i].GetPoint3D(), membersBaseNodes_FrontSideRafter_2[i + 1].GetPoint3D(), new Vector3D(0, 0, 1), 1, 0, 0, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, (fLengthForText * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = +fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FOUNDATIONS)
            {
                // Pre zakladne koty okotujeme podorys podobne ako pre Columns View

                // Potrebujeme okotovat jednotlive typy patiek
                // To znamena ziskat body na krajnej hrane patiek uplne vpravo a body na krajnej hrane patiek uplne vzadu pre jednotlive typy patiek

                // All foundation points
                List<Point3D> foundationPoints = ModelHelper.GetFootingPadsPoints(model);

                bool bDrawDimension_Right = true;
                bool bDrawDimension_Back = true;

                if (bDrawDimension_Right == true) // Kota vpravo od budovy
                {
                    // Maximum right coordinate points Z = 0
                    List<Point3D> foundationPointsX = ModelHelper.GetPointsInDistanceInterval(foundationPoints, model.fW_frame, model.fW_frame + 10, 0, true, false); // X
                    List<Point3D> dimensionPoints_1 = ModelHelper.GetPointsInDistance(foundationPointsX, 0, 2); // Z

                    dimensionPoints_1 = dimensionPoints_1.OrderBy(p => p.Y).ToList(); // Sort by Y coordinate
                    List<Point3D> dimensionPoints_2 = new List<Point3D>() { dimensionPoints_1.First(), dimensionPoints_1.Last() };

                    bool bDrawDimension_1 = true;
                    bool bDrawDimension_2 = true;

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < dimensionPoints_1.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_1[i], dimensionPoints_1[i + 1], new Vector3D(0, 0, -1), 0, 1, 0, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((dimensionPoints_1[i + 1].Y - dimensionPoints_1[i].Y) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_2 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < dimensionPoints_2.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_2[i], dimensionPoints_2[i + 1], new Vector3D(0, 0, -1), 0, 1, 0, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((dimensionPoints_2[i + 1].Y - dimensionPoints_2[i].Y) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = +fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }

                if (bDrawDimension_Back == true) // Kota v zadnej casti budovy
                {
                    // Maximum back coordinate points Z = 0
                    // Maximalna suradnica zakladu pre wind post Y moze byt teoreticky byt mensia dlzka budovy (je to nespravne, ale je to mozne tak zadat :)
                    List<Point3D> foundationPointsY = ModelHelper.GetPointsInDistanceInterval(foundationPoints, model.fL_tot - 0.2f, model.fL_tot + 10, 1, true, false); // Y
                    List<Point3D> dimensionPoints_1 = ModelHelper.GetPointsInDistance(foundationPointsY, 0, 2); // Z

                    dimensionPoints_1 = dimensionPoints_1.OrderBy(p => p.X).ToList(); // Sort by X coordinate

                    List<Point3D> dimensionPoints_2 = new List<Point3D>() { dimensionPoints_1.First(), dimensionPoints_1.Last() };

                    bool bDrawDimension_1 = true;
                    bool bDrawDimension_2 = true;

                    // Create Dimensions
                    List<CDimensionLinear3D> listOfDimensions = null;

                    float fExtensionLineLength = 0.5f;
                    float fMainLinePosition = 0.4f;
                    float fExtensionLineOffset = 0.15f;

                    float fDistanceBetweenMainLines = 0.2f;

                    if (bDrawDimension_1 == true)
                    {
                        listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < dimensionPoints_1.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_1[i], dimensionPoints_1[i + 1], new Vector3D(0, 0, -1), 0, 0, -1, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((dimensionPoints_1[i + 1].X - dimensionPoints_1[i].X) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (bDrawDimension_2 == true)
                    {
                        if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                        for (int i = 0; i < dimensionPoints_2.Count - 1; i++)
                        {
                            CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_2[i], dimensionPoints_2[i + 1], new Vector3D(0, 0, -1), 0, 0, -1, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), fExtensionLineLength, fMainLinePosition, fExtensionLineOffset, ((dimensionPoints_2[i + 1].X - dimensionPoints_2[i].X) * 1000).ToString("F0"));
                            listOfDimensions.Add(dim);
                        }

                        // Nastavime parametre pre dalsie koty
                        //fExtensionLineLength += fDistanceBetweenMainLines;
                        //fMainLinePosition = + fDistanceBetweenMainLines;
                        fExtensionLineOffset += fDistanceBetweenMainLines;
                    }

                    if (sDisplayOptions.bDisplayDimensions) dimensions3DGroup = Drawing3D.CreateModelDimensions_Model3DGroup(listOfDimensions, model, sDisplayOptions);
                    if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                    // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                    if (dimensions3DGroup != null)
                    {
                        foreach (CDimensionLinear3D dim in listOfDimensions)
                        {
                            DrawDimensionText3D(dim, _trackport.ViewPort, sDisplayOptions);
                        }
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }



    }
}
