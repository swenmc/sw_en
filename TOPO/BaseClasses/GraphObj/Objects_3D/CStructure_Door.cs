﻿using BaseClasses.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CStructure_Door : CSurfaceComponent
    {
        //public Point3D m_pControlPoint = new Point3D();

        //EWindowShapeType m_eShapeType = EWindowShapeType.eClassic;
        //
        //public EWindowShapeType EShapeType
        //{
        //    get { return m_eShapeType; }
        //    set { m_eShapeType = value; }
        //}

        private float m_fvolOpacity_1;
        private Color m_volColor_1;

        private string m_doorPanelColorName;
        private float m_fvolOpacity_2;
        private Color m_volColor_2;
        private DiffuseMaterial m_Material_1 = null;
        private DiffuseMaterial m_Material_2 = null;
        private bool m_isRollerDoor;
        private bool m_LeftOrBack;

        public float m_fDim1;
        public float m_fDim2;
        public float m_fDim3;

        private int m_iSegmentNum;
        private float m_fGThickness;
        private float m_fRotationZDegrees;



        public bool IsRollerDoor
        {
            get
            {
                return m_isRollerDoor;
            }

            set
            {
                m_isRollerDoor = value;
            }
        }

        public float RotationZDegrees
        {
            get
            {
                return m_fRotationZDegrees;
            }

            set
            {
                m_fRotationZDegrees = value;
            }
        }

        public int SegmentNum
        {
            get
            {
                return m_iSegmentNum;
            }

            set
            {
                m_iSegmentNum = value;
            }
        }

        public float GThickness
        {
            get
            {
                return m_fGThickness;
            }

            set
            {
                m_fGThickness = value;
            }
        }

        //public int iVectorOverFactor_LCS;
        //public int iVectorUpFactor_LCS;

        // Constructor 1
        public CStructure_Door()
        {
        }

        // Constructor 2
        public CStructure_Door(int i_ID, int[] iPCollection, int fTime)
        {
            ID = i_ID;
            FTime = fTime;
        }
        
        // Constructor 3
        public CStructure_Door(int iW_ID, int iSegmentNum, Point3D pControlEdgePoint, float fL, float fH, float ft, float fDoorPanelThickness, float fRotationZDegrees, bool bIsDisplayed, float fTime, 
            Color doorFlashingColor, Color doorPanelColor, string doorPanelColorName, float doorPanelOpacity, bool isRollerDoor, bool LeftOrBack, bool useTextures)
        {
            ID = iW_ID;
            SegmentNum = iSegmentNum;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fL;
            m_fDim2 = fH;
            m_fDim3 = ft;

            m_fvolOpacity_1 = 1.0f; // Flashings - TODO
            m_fvolOpacity_2 = doorPanelOpacity; // Vypln dveri

            GThickness = fDoorPanelThickness;
            RotationZDegrees = fRotationZDegrees;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
            IsRollerDoor = isRollerDoor;
            m_LeftOrBack = LeftOrBack;

            m_volColor_1 = doorFlashingColor;

            SolidColorBrush flashingSolidBrush = new SolidColorBrush(m_volColor_1);
            flashingSolidBrush.Opacity = m_fvolOpacity_1;
            m_Material_1 = new DiffuseMaterial(flashingSolidBrush);

            m_doorPanelColorName = doorPanelColorName;
            m_volColor_2 = doorPanelColor;

            SetTextPointInLCS();

            CreateM_3D_G_Door(iSegmentNum, fL, fH, ft, fDoorPanelThickness, useTextures);
        }

        // Temporary auxiliary function
        public Model3DGroup CreateM_3D_G_SegmDoor(int iSegm, float fL_X, float fH_Z, float fT_Y, DiffuseMaterial DiffMatF, DiffuseMaterial DiffMatD, float fDoorPanelThickness)
        {
            // Create Door Segment in LCS 0,0,0
            Point3D p01_HU = new Point3D(0, 0, fH_Z - fT_Y);
            Point3D p02_V = new Point3D(0, 0, 0);
            Point3D p03_V = new Point3D(fL_X - fT_Y, 0, 0);
            Point3D p04_DoorPanel = new Point3D(fT_Y, 0.5f * fT_Y - 0.5f * fDoorPanelThickness, 0);

            // Fill array of control points
            Point3D[] pArray = new Point3D[5];

            pArray[0] = p01_HU;
            pArray[1] = p02_V;
            pArray[2] = p03_V;
            pArray[3] = p04_DoorPanel;

            // Move Segment to its position in LCS
            for (int i = 0; i < pArray.Length; i++)
                pArray[i].X += iSegm * fL_X;

            CVolume mFrame_01_HU = new CVolume();
            CVolume mFrame_02_V = new CVolume();
            CVolume mFrame_03_V = new CVolume();
            CVolume mDoorPanel = new CVolume();
            Model3DGroup gr = new Model3DGroup(); // Door Segment

            gr.Children.Add(mFrame_01_HU.CreateM_3D_G_Volume_8Edges(pArray[0], fL_X, fT_Y, fT_Y, DiffMatF, DiffMatF)); // Horizontal upper
            gr.Children.Add(mFrame_02_V.CreateM_3D_G_Volume_8Edges(pArray[1], fT_Y, fT_Y, fH_Z - 1 * fT_Y, DiffMatF, DiffMatF)); // Vertical
            gr.Children.Add(mFrame_03_V.CreateM_3D_G_Volume_8Edges(pArray[2], fT_Y, fT_Y, fH_Z - 1 * fT_Y, DiffMatF, DiffMatF)); // Vertical
            gr.Children.Add(mDoorPanel.CreateM_3D_G_Volume_8Edges(pArray[3], fL_X - 2 * fT_Y, fDoorPanelThickness, fH_Z - 1 * fT_Y, DiffMatD, DiffMatD)); // Door Panel No 1

            UseSimpleWireFrame2D = true; // TODO - Zapracovat ako volbu v GUI

            EdgePoints2D = new List<System.Windows.Point>()
            {
                new System.Windows.Point(0, 0),
                new System.Windows.Point(m_fDim1, 0),
                new System.Windows.Point(m_fDim1, m_fDim2),
                new System.Windows.Point(0, m_fDim2)
            };

            if (UseSimpleWireFrame2D)
            {
                // GCS -system plane XZ
                double offset = 0.010;

                WireFramePoints.Add(new Point3D(0, offset, 0));
                WireFramePoints.Add(new Point3D(m_fDim1, offset, 0));

                WireFramePoints.Add(new Point3D(m_fDim1, offset, 0));
                WireFramePoints.Add(new Point3D(m_fDim1, offset, m_fDim2));

                WireFramePoints.Add(new Point3D(m_fDim1, offset, m_fDim2));
                WireFramePoints.Add(new Point3D(0, offset, m_fDim2));

                WireFramePoints.Add(new Point3D(0, offset, m_fDim2));
                WireFramePoints.Add(new Point3D(0, offset, 0));
            }
            else
            {
                //to Mato - tu je nutne nastavit wireframePoints
                //tu je nutne niekde ziskat Wireframe a aj ho nastavit
                WireFramePoints.AddRange(mFrame_01_HU.WireFramePoints);
                WireFramePoints.AddRange(mFrame_02_V.WireFramePoints);
                WireFramePoints.AddRange(mFrame_03_V.WireFramePoints);
                WireFramePoints.AddRange(mDoorPanel.WireFramePoints);
            }

            //ak sa nepouziva,tak treba zmazat z pamate
            mFrame_01_HU = null;
            mFrame_02_V = null;
            mFrame_03_V = null;
            mDoorPanel = null;

            return gr;
        }

        public Model3DGroup CreateM_3D_G_Door(int iSegmentNum, float fL_X, float fH_Z, float fT_Y, float fGlassThickness, bool useTextures)
        {
            Model3DGroup gr = new Model3DGroup();

            ImageBrush imgBrush = null;

            if (useTextures && IsRollerDoor) // Použijeme len pre typ roller door
            {
                string uriString = "pack://application:,,,/Resources/Textures/Corrugate/Corrugate_" + m_doorPanelColorName + ".jpg";

                imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
                imgBrush.TileMode = TileMode.Tile;
                imgBrush.ViewportUnits = BrushMappingMode.Absolute;
                imgBrush.Stretch = Stretch.Fill;
                imgBrush.Opacity = m_fvolOpacity_2;

                double claddingWidthRibModular_door = 0.09; // m  TODO Mato

                double wpWidth = claddingWidthRibModular_door / fL_X;
                double wpHeight = claddingWidthRibModular_door / fH_Z;
                imgBrush.Viewport = new System.Windows.Rect(0, 0, wpHeight, wpWidth); //lebo otocene o 90stupnov
                imgBrush.Transform = new RotateTransform(90);
                m_Material_2 = new DiffuseMaterial(imgBrush);
            }
            else
            {
                SolidColorBrush panelSolidBrush = new SolidColorBrush(m_volColor_2);
                panelSolidBrush.Opacity = m_fvolOpacity_2;
                m_Material_2 = new DiffuseMaterial(panelSolidBrush);
            }

            WireFramePoints = new List<Point3D>();
            // Create Door in LCS
            for (int i = 0; i < iSegmentNum; i++) // Add segments
            {
                gr.Children.Add(CreateM_3D_G_SegmDoor(i, fL_X, fH_Z, fT_Y, m_Material_1, m_Material_2, fGlassThickness));
            }

            // Create transform group
            Transform3DGroup transform3DGroup = GetTransformGroup();
            // Set transformation to group
            gr.Transform = transform3DGroup;

            //WireFramePoints transform
            Drawing3DHelper.TransformPoints(WireFramePoints, transform3DGroup);
            //for (int i = 0; i < WireFramePoints.Count; i++)
            //{
            //    WireFramePoints[i] = transform3DGroup.Transform(WireFramePoints[i]);
            //}

            return gr;
        }

        public Transform3DGroup GetTransformGroup(/*Point3D pControlPoint, float fRotationZDegrees*/)
        {
            // Move and rotate door
            Transform3DGroup transform3DGroup = new Transform3DGroup();
            // Rotation about Y axis
            RotateTransform3D rotateTransformation3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), RotationZDegrees));
            // Translation - move to control point
            TranslateTransform3D translateTransform3D = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);
            // Adding transforms
            transform3DGroup.Children.Add(rotateTransformation3D);
            transform3DGroup.Children.Add(translateTransform3D);

            return transform3DGroup;
        }

        //upravene vramci refaktoringu 2.3.2021 naco sa vytvara dalsia struktura Model3DGroup
        //public Model3DGroup CreateM_3D_G_Door(bool useTextures)
        //{
        //    Model3DGroup m3Dg = new Model3DGroup();

        //    Point3D pControlEdge = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

        //    m3Dg.Children.Add(CreateM_3D_G_Door(m_iSegmentNum, pControlEdge, m_fDim1, m_fDim2, m_fDim3, m_fGThickness, m_fRotationZDegrees, useTextures));

        //    return m3Dg;
        //}
        public Model3DGroup CreateM_3D_G_Door(bool useTextures)
        {
            //omg toto je naco
            //Point3D pControlEdge = new Point3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            return CreateM_3D_G_Door(SegmentNum, m_fDim1, m_fDim2, m_fDim3, GThickness, useTextures);
        }

        public void SetTextPointInLCS()
        {
            //iVectorOverFactor_LCS = 1;
            //iVectorUpFactor_LCS = 1;

            float fOffsetFromPlane = -0.050f; // Offset pred rovinou dveri, aby sa text nevnoril do 3D reprezentacie
            if (m_LeftOrBack) fOffsetFromPlane = -fOffsetFromPlane + m_fDim3 + GThickness;
            
            PointText = new Point3D()
            {
                X = 0.5 * m_fDim1, // Kreslime v 50% sirky zlava
                Y = fOffsetFromPlane,
                Z = 0.4 * m_fDim2 // Kreslime v 40% dlzky zdola
            };
        }
    }
}
