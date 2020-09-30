﻿using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CStructure_Door : CEntity3D
    {
        //public Point3D m_pControlPoint = new Point3D();

        //EWindowShapeType m_eShapeType = EWindowShapeType.eClassic;
        //
        //public EWindowShapeType EShapeType
        //{
        //    get { return m_eShapeType; }
        //    set { m_eShapeType = value; }
        //}

        public float m_fvolOpacity;
        public Color m_volColor_1 = new Color(); // Default
        public Color m_volColor_2 = new Color();

        public DiffuseMaterial m_Material_1 = new DiffuseMaterial();
        public DiffuseMaterial m_Material_2 = new DiffuseMaterial();

        public float m_fDim1;
        public float m_fDim2;
        public float m_fDim3;

        public int m_iSegmentNum;
        public float m_fGThickness;
        public float m_fRotationZDegrees;

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
        public CStructure_Door(int iW_ID/*, EWindowShapeType iShapeType*/, int iSegmentNum, Point3D pControlEdgePoint, float fL, float fH, float ft, DiffuseMaterial volMat1, DiffuseMaterial volMat2, float fDoorPanelThickness, float fRotationZDegrees, bool bIsDisplayed, float fTime)
        {
            ID = iW_ID;
            //m_eShapeType = iShapeType;
            m_iSegmentNum = iSegmentNum;
            m_pControlPoint = pControlEdgePoint;
            m_fDim1 = fL;
            m_fDim2 = fH;
            m_fDim3 = ft;
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            // Set same properties for both materials
            m_Material_2 = volMat2;
            m_volColor_2 = volMat2.Color;
            m_fGThickness = fDoorPanelThickness;
            m_fRotationZDegrees = fRotationZDegrees;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            /*MObject3DModel =*/
            CreateM_3D_G_Door(iSegmentNum, new Point3D(pControlEdgePoint.X, pControlEdgePoint.Y, pControlEdgePoint.Z), fL, fH, ft, volMat1, volMat2, fDoorPanelThickness, fRotationZDegrees);
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

            return gr;
        }

        public Model3DGroup CreateM_3D_G_Door(int iSegmentNum, Point3D pControlPoint, float fL_X, float fH_Z, float fT_Y, DiffuseMaterial DiffMatF, DiffuseMaterial DiffMatG, float fGlassThickness, float fRotationZDegrees)
        {
            Model3DGroup gr = new Model3DGroup();

            // Create Door in LCS
            for (int i = 0; i < iSegmentNum; i++) // Add segments
            {
                gr.Children.Add(CreateM_3D_G_SegmDoor(i, fL_X, fH_Z, fT_Y, DiffMatF, DiffMatG, fGlassThickness));
            }

            // Move and rotate door

            Transform3DGroup transform3DGroup = new Transform3DGroup();
            // Rotation about Y axis
            RotateTransform3D rotateTransformation3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), fRotationZDegrees));
            // Translation - move to control point
            TranslateTransform3D translateTransform3D = new TranslateTransform3D(pControlPoint.X, pControlPoint.Y, pControlPoint.Z);
            // Adding transforms
            transform3DGroup.Children.Add(rotateTransformation3D);
            transform3DGroup.Children.Add(translateTransform3D);
            // Set transformation to group
            gr.Transform = transform3DGroup;

            return gr;
        }

        public Model3DGroup CreateM_3D_G_Door()
        {
            Model3DGroup m3Dg = new Model3DGroup();

            Point3D pControlEdge = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            m3Dg.Children.Add(CreateM_3D_G_Door(m_iSegmentNum, pControlEdge, m_fDim1, m_fDim2, m_fDim3, m_Material_1, m_Material_2, m_fGThickness, m_fRotationZDegrees));

            return m3Dg;
        }
    }
}
