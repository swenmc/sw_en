using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CStructure_Window : CEntity3D
    {
        //public CPoint m_pControlPoint = new CPoint();

        EWindowShapeType m_eShapeType = EWindowShapeType.eClassic;

        public EWindowShapeType EShapeType
        {
            get { return m_eShapeType; }
            set { m_eShapeType = value; }
        }

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
        public CStructure_Window()
        {
        }

        // Constructor 2
        public CStructure_Window(int i_ID, int[] iPCollection, int fTime)
        {
            ID = i_ID;
            FTime = fTime;
        }

        // Constructor 4

        public CStructure_Window(int i_ID, EWindowShapeType iShapeType, CPoint pControlEdgePoint, float fX, float fY, float fZ, DiffuseMaterial volMat1, DiffuseMaterial volMat2, bool bIsDisplayed, float fTime)
        {
            ID = i_ID;
            m_eShapeType = iShapeType;
            m_pControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_fvolOpacity = 1.0f;
            m_Material_2 = volMat2;
            m_volColor_2 = volMat2.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        // Constructor 5
        public CStructure_Window(int iW_ID, EWindowShapeType iShapeType, int iSegmentNum, CPoint pControlEdgePoint, float fL, float fH, float ft, DiffuseMaterial volMat1, DiffuseMaterial volMat2, float fGlassThickness, float fRotationZDegrees, bool bIsDisplayed, float fTime)
        {
            ID = iW_ID;
            m_eShapeType = iShapeType;
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
            m_fGThickness = fGlassThickness;
            m_fRotationZDegrees = fRotationZDegrees;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            /*MObject3DModel =*/ CreateM_3D_G_Window(iSegmentNum, new Point3D(pControlEdgePoint.X, pControlEdgePoint.Y, pControlEdgePoint.Z), fL, fH, ft, volMat1, volMat2, fGlassThickness, fRotationZDegrees);
        }

        // Temporary auxiliary function - glass window (3D HOUSE)
        public Model3DGroup CreateM_3D_G_SegmWindow(int iSegm, float fL_X, float fH_Z, float fT_Y, DiffuseMaterial DiffMatF, DiffuseMaterial DiffMatG, float fGlassThickness)
        {
            // Create Window Segment in LCS 0,0,0
            Point3D p01_HB = new Point3D(0, 0, 0);
            Point3D p02_HU = new Point3D(0, 0, fH_Z - fT_Y);
            Point3D p03_V = new Point3D(0, 0, fT_Y);
            Point3D p04_V = new Point3D(fL_X - fT_Y, 0, fT_Y);
            Point3D p05_GlassTable = new Point3D(fT_Y, 0.5f * fT_Y - 0.5f * fGlassThickness, fT_Y);

            // Fill array of control points
            Point3D[] pArray = new Point3D[5];

            pArray[0] = p01_HB;
            pArray[1] = p02_HU;
            pArray[2] = p03_V;
            pArray[3] = p04_V;
            pArray[4] = p05_GlassTable;

            // Move Segment to its position in LCS
            for (int i = 0; i < pArray.Length; i++)
                pArray[i].X += iSegm * fL_X;

            CVolume mFrame_01_HB = new CVolume();
            CVolume mFrame_02_HU = new CVolume();
            CVolume mFrame_03_V = new CVolume();
            CVolume mFrame_04_V = new CVolume();
            CVolume mGlassTable = new CVolume();
            Model3DGroup gr = new Model3DGroup(); // Window Segment

            gr.Children.Add(mFrame_01_HB.CreateM_3D_G_Volume_8Edges(pArray[0], fL_X, fT_Y, fT_Y, DiffMatF, DiffMatF)); // Horizontal bottom;
            gr.Children.Add(mFrame_02_HU.CreateM_3D_G_Volume_8Edges(pArray[1], fL_X, fT_Y, fT_Y, DiffMatF, DiffMatF)); // Horizontal upper
            gr.Children.Add(mFrame_03_V.CreateM_3D_G_Volume_8Edges(pArray[2], fT_Y, fT_Y, fH_Z - 2 * fT_Y, DiffMatF, DiffMatF)); // Vertical
            gr.Children.Add(mFrame_04_V.CreateM_3D_G_Volume_8Edges(pArray[3], fT_Y, fT_Y, fH_Z - 2 * fT_Y, DiffMatF, DiffMatF)); // Vertical
            gr.Children.Add(mGlassTable.CreateM_3D_G_Volume_8Edges(pArray[4], fL_X - 2 * fT_Y, fGlassThickness, fH_Z - 2 * fT_Y, DiffMatG, DiffMatG)); // Glass No 1

            return gr;
        }
        public Model3DGroup CreateM_3D_G_Window(int iSegmentNum, Point3D pControlPoint, float fL_X, float fH_Z, float fT_Y, DiffuseMaterial DiffMatF, DiffuseMaterial DiffMatG, float fGlassThickness, float fRotationZDegrees)
        {
            Model3DGroup gr = new Model3DGroup();

            // Create Window in LCS
            for (int i = 0; i < iSegmentNum; i++) // Add segments
            {
                gr.Children.Add(CreateM_3D_G_SegmWindow(i, fL_X, fH_Z, fT_Y, DiffMatF, DiffMatG, fGlassThickness));
            }

            // Move and rotate window

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

        public Model3DGroup CreateM_3D_G_Window()
        {
            Model3DGroup m3Dg = new Model3DGroup();

            Point3D pControlEdge = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            m3Dg.Children.Add(CreateM_3D_G_Window(m_iSegmentNum, pControlEdge, m_fDim1, m_fDim2, m_fDim3, m_Material_1, m_Material_2, m_fGThickness, m_fRotationZDegrees));

            return m3Dg;
        }
    }
}
