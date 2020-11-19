﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private float m_fvolOpacity;
        private Color m_volColor_1; 
        private string m_doorCladdingColorName;
        private Color m_volColor_2;
        private DiffuseMaterial m_Material_1 = null;
        private DiffuseMaterial m_Material_2 = null;
        private string m_claddingCoatingType_Wall;

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
        public CStructure_Door(int iW_ID, int iSegmentNum, Point3D pControlEdgePoint, float fL, float fH, float ft, float fDoorPanelThickness, float fRotationZDegrees, bool bIsDisplayed, float fTime, 
            Color doorFlashingColor, Color doorCladdingColor, string doorCladdingColorName, bool useTextures)
        {
            ID = iW_ID;
            m_iSegmentNum = iSegmentNum;
            m_pControlPoint = pControlEdgePoint;
            m_fDim1 = fL;
            m_fDim2 = fH;
            m_fDim3 = ft;
            m_fvolOpacity = 1.0f;
            m_fGThickness = fDoorPanelThickness;
            m_fRotationZDegrees = fRotationZDegrees;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            m_volColor_1 = doorFlashingColor;
            m_Material_1 = new DiffuseMaterial(new SolidColorBrush(m_volColor_1));

            m_doorCladdingColorName = doorCladdingColorName;
            m_volColor_2 = doorCladdingColor;
            if (!useTextures)
            {
                m_Material_2 = new DiffuseMaterial(new SolidColorBrush(m_volColor_2));
            }

            CreateM_3D_G_Door(iSegmentNum, new Point3D(pControlEdgePoint.X, pControlEdgePoint.Y, pControlEdgePoint.Z), fL, fH, ft, fDoorPanelThickness, fRotationZDegrees, useTextures);
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
        
        public Model3DGroup CreateM_3D_G_Door(int iSegmentNum, Point3D pControlPoint, float fL_X, float fH_Z, float fT_Y, float fGlassThickness, float fRotationZDegrees, bool useTextures)
        {
            Model3DGroup gr = new Model3DGroup();

            ImageBrush imgBrush = null;

            if (useTextures) // Použijeme len pre typ roller door
            {
                string uriString = "pack://application:,,,/Resources/Textures/Corrugate/Corrugate_" + m_doorCladdingColorName + ".jpg";

                imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
                imgBrush.TileMode = TileMode.Tile;
                imgBrush.ViewportUnits = BrushMappingMode.Absolute;
                imgBrush.Stretch = Stretch.Fill;
                imgBrush.Opacity = m_fvolOpacity;

                double claddingWidthRibModular_door = 0.09; // m  TODO Mato

                double wpWidth = claddingWidthRibModular_door / fL_X;
                double wpHeight = claddingWidthRibModular_door / fH_Z;
                imgBrush.Viewport = new System.Windows.Rect(0, 0, wpHeight, wpWidth); //lebo otocene o 90stupnov
                imgBrush.Transform = new RotateTransform(90);
                m_Material_2 = new DiffuseMaterial(imgBrush);
            }

            // Create Door in LCS
            for (int i = 0; i < iSegmentNum; i++) // Add segments
            {
                gr.Children.Add(CreateM_3D_G_SegmDoor(i, fL_X, fH_Z, fT_Y, m_Material_1, m_Material_2, fGlassThickness));
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

        public Model3DGroup CreateM_3D_G_Door(bool useTextures)
        {
            Model3DGroup m3Dg = new Model3DGroup();

            Point3D pControlEdge = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            m3Dg.Children.Add(CreateM_3D_G_Door(m_iSegmentNum, pControlEdge, m_fDim1, m_fDim2, m_fDim3, m_fGThickness, m_fRotationZDegrees, useTextures));

            return m3Dg;
        }
    }
}
