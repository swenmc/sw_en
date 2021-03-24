using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CStructure_Window : CSurfaceComponent
    {
        //public Point3D m_ControlPoint = new Point3D();

        EWindowShapeType m_eShapeType = EWindowShapeType.eClassic;

        public EWindowShapeType EShapeType
        {
            get { return m_eShapeType; }
            set { m_eShapeType = value; }
        }

        public float m_fvolOpacity_1;
        public Color m_volColor_1;

        private float m_fvolOpacity_2;
        public Color m_volColor_2;

        public DiffuseMaterial m_Material_1 = null;
        public DiffuseMaterial m_Material_2 = null;

        public float m_fDim1;
        public float m_fDim2;
        public float m_fDim3;

        private int iSegmentNum;
        private float fGThickness;
        private float fRotationZDegrees;

        public int SegmentNum
        {
            get
            {
                return iSegmentNum;
            }

            set
            {
                iSegmentNum = value;
            }
        }

        public float GThickness
        {
            get
            {
                return fGThickness;
            }

            set
            {
                fGThickness = value;
            }
        }

        public float RotationZDegrees
        {
            get
            {
                return fRotationZDegrees;
            }

            set
            {
                fRotationZDegrees = value;
            }
        }

        public int iVectorOverFactor_LCS;
        public int iVectorUpFactor_LCS;

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

        public CStructure_Window(int i_ID, EWindowShapeType iShapeType, Point3D pControlEdgePoint, float fX, float fY, float fZ, DiffuseMaterial volMat1, DiffuseMaterial volMat2, bool bIsDisplayed, float fTime)
        {
            ID = i_ID;
            m_eShapeType = iShapeType;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_Material_1 = volMat1;
            m_volColor_2 = volMat1.Color;
            m_Material_2 = volMat2;
            m_volColor_2 = volMat2.Color;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        // Constructor 5
        public CStructure_Window(int iW_ID, EWindowShapeType iShapeType, int iSegmentNum, Point3D pControlEdgePoint, float fL, float fH, float ft,
            Color windowFlashingColor, Color windowPanelColor, float fGlassPanelOpacity, float fGlassThickness, float fRotationZDegrees, bool bIsDisplayed, float fTime)
        {
            ID = iW_ID;
            m_eShapeType = iShapeType;
            SegmentNum = iSegmentNum;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fL;
            m_fDim2 = fH;
            m_fDim3 = ft;

            m_fvolOpacity_1 = 1.0f; // Flashings - TODO
            m_fvolOpacity_2 = fGlassPanelOpacity; // Vypln okna - sklo

            m_volColor_1 = windowFlashingColor;
            m_volColor_2 = windowPanelColor;

            SolidColorBrush flashingSolidBrush = new SolidColorBrush(m_volColor_1);
            flashingSolidBrush.Opacity = m_fvolOpacity_1;
            m_Material_1 = new DiffuseMaterial(flashingSolidBrush);

            SolidColorBrush glassPanelSolidBrush = new SolidColorBrush(m_volColor_2);
            glassPanelSolidBrush.Opacity = m_fvolOpacity_2;
            m_Material_2 = new DiffuseMaterial(glassPanelSolidBrush);

            GThickness = fGlassThickness;
            RotationZDegrees = fRotationZDegrees;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            SetTextPointInLCS();

            CreateM_3D_G_Window(iSegmentNum, fL, fH, ft, fGlassThickness);
        }

        public CStructure_Window(int iW_ID, EWindowShapeType iShapeType, int iSegmentNum, Point3D pControlEdgePoint, float fL, float fH, float ft,
        DiffuseMaterial matF, DiffuseMaterial matG, float fGlassThickness, float fRotationZDegrees, bool bIsDisplayed, float fTime)
        {
            ID = iW_ID;
            m_eShapeType = iShapeType;
            SegmentNum = iSegmentNum;
            ControlPoint = pControlEdgePoint;
            m_fDim1 = fL;
            m_fDim2 = fH;
            m_fDim3 = ft;

            m_Material_1 = matF;
            m_Material_2 = matG;

            GThickness = fGlassThickness;
            RotationZDegrees = fRotationZDegrees;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            Text = "Window H x W" + "\n" + (m_fDim2 * 1000).ToString("F0") + " x " + (m_fDim1 * 1000).ToString("F0") + " mm"; // TODO - dopracovat texty podla nastaveni v GUI - Display Options, zaviest text popisu ako vstupny parameter objektu

            SetTextPointInLCS();

            CreateM_3D_G_Window(iSegmentNum, fL, fH, ft, fGlassThickness);
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
                WireFramePoints.AddRange(mFrame_01_HB.WireFramePoints);
                WireFramePoints.AddRange(mFrame_02_HU.WireFramePoints);
                WireFramePoints.AddRange(mFrame_03_V.WireFramePoints);
                WireFramePoints.AddRange(mFrame_04_V.WireFramePoints);
                WireFramePoints.AddRange(mGlassTable.WireFramePoints);
            }

            //ak sa nepouziva,tak treba zmazat z pamate
            mFrame_01_HB = null;
            mFrame_02_HU = null;
            mFrame_03_V = null;
            mFrame_04_V = null;
            mGlassTable = null;

            return gr;
        }

        public Model3DGroup CreateM_3D_G_Window(int iSegmentNum, float fL_X, float fH_Z, float fT_Y, float fGlassThickness)
        {
            Model3DGroup gr = new Model3DGroup();

            WireFramePoints = new List<Point3D>();
            // Create Window in LCS
            for (int i = 0; i < iSegmentNum; i++) // Add segments
            {
                gr.Children.Add(CreateM_3D_G_SegmWindow(i, fL_X, fH_Z, fT_Y, m_Material_1, m_Material_2, fGlassThickness));
            }

            // Create transform group
            Transform3DGroup transform3DGroup = GetTransformGroup();
            // Set transformation to group
            gr.Transform = transform3DGroup;

            //WireFramePoints transform
            for (int i = 0; i < WireFramePoints.Count; i++)
            {
                WireFramePoints[i] = transform3DGroup.Transform(WireFramePoints[i]);
            }

            return gr;
        }

        public Model3DGroup CreateM_3D_G_Window()
        {
            Model3DGroup m3Dg = new Model3DGroup();

            //Point3D pControlEdge = new Point3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            m3Dg.Children.Add(CreateM_3D_G_Window(SegmentNum, m_fDim1, m_fDim2, m_fDim3, GThickness));

            return m3Dg;
        }

        public Transform3DGroup GetTransformGroup()
        {
            // Move and rotate window
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

        public void SetTextPointInLCS()
        {
            iVectorOverFactor_LCS = 1;
            iVectorUpFactor_LCS = 1;

            float fOffsetFromPlane = - 0.050f; // Offset pred rovinou dveri, aby sa text nevnoril do 3D reprezentacie

            //PointText = new Point3D()
            //{
            //    X = 0.5 * m_fDim1, // Kreslime v 30% sirky zlava
            //    Y = 0.4 * m_fDim2, // Kreslime v 40% dlzky zdola
            //    Z = fOffsetFromPlane
            //};

            PointText = new Point3D()
            {
                X = 0.5 * m_fDim1, // Kreslime v 50% sirky zlava
                Y = fOffsetFromPlane,
                Z = 0.5 * m_fDim2, // Kreslime v 50% dlzky zdola
            };
        }
    }
}
