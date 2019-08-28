using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // Class CSlab
    [Serializable]
    public class CSlab : CVolume
    {
        private float m_Eccentricity_x;
        private float m_Eccentricity_y;
        private float m_RotationAboutZ_deg;

        private CReinforcementBar m_Reference_Top_Bar_x;
        private CReinforcementBar m_Reference_Top_Bar_y;
        private CReinforcementBar m_Reference_Bottom_Bar_x;
        private CReinforcementBar m_Reference_Bottom_Bar_y;

        private List<CReinforcementBar> m_Top_Bars_x;
        private List<CReinforcementBar> m_Top_Bars_y;
        private List<CReinforcementBar> m_Bottom_Bars_x;
        private List<CReinforcementBar> m_Bottom_Bars_y;

        private int m_Count_Top_Bars_x;
        private int m_Count_Top_Bars_y;
        private int m_Count_Bottom_Bars_x;
        private int m_Count_Bottom_Bars_y;

        private float m_fDistanceOfBars_Top_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Top_y_SpacingInxDirection;
        private float m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Bottom_y_SpacingInxDirection;

        private float m_fConcreteCover;

        private string m_sMeshGradeName;

        private int m_NumberOfSawCutsInDirectionX;
        private int m_NumberOfSawCutsInDirectionY;
        private float m_FirstSawCutPositionInDirectionX;
        private float m_FirstSawCutPositionInDirectionY;
        private float m_SawCutsSpacingInDirectionX;
        private float m_SawCutsSpacingInDirectionY;

        private List<CSawCut> m_SawCuts;

        private int m_NumberOfControlJointsInDirectionX;
        private int m_NumberOfControlJointsInDirectionY;
        private float m_FirstControlJointPositionInDirectionX;
        private float m_FirstControlJointPositionInDirectionY;
        private float m_ControlJointsSpacingInDirectionX;
        private float m_ControlJointsSpacingInDirectionY;

        private List<CControlJoint> m_ControlJoints;

        private List<Point3D> MWireFramePoints;

        private CJointDesignDetails m_DesignDetails;
        public CJointDesignDetails DesignDetails
        {
            get { return m_DesignDetails; }
            set { m_DesignDetails = value; }
        }

        public float Eccentricity_x
        {
            get
            {
                return m_Eccentricity_x;
            }

            set
            {
                m_Eccentricity_x = value;
            }
        }

        public float Eccentricity_y
        {
            get
            {
                return m_Eccentricity_y;
            }

            set
            {
                m_Eccentricity_y = value;
            }
        }

        public float RotationAboutZ_deg
        {
            get
            {
                return m_RotationAboutZ_deg;
            }

            set
            {
                m_RotationAboutZ_deg = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_x
        {
            get
            {
                return m_Reference_Top_Bar_x;
            }

            set
            {
                m_Reference_Top_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_y
        {
            get
            {
                return m_Reference_Top_Bar_y;
            }

            set
            {
                m_Reference_Top_Bar_y = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_x
        {
            get
            {
                return m_Reference_Bottom_Bar_x;
            }

            set
            {
                m_Reference_Bottom_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_y
        {
            get
            {
                return m_Reference_Bottom_Bar_y;
            }

            set
            {
                m_Reference_Bottom_Bar_y = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_x
        {
            get
            {
                return m_Top_Bars_x;
            }

            set
            {
                m_Top_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_y
        {
            get
            {
                return m_Top_Bars_y;
            }

            set
            {
                m_Top_Bars_y = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_x
        {
            get
            {
                return m_Bottom_Bars_x;
            }

            set
            {
                m_Bottom_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_y
        {
            get
            {
                return m_Bottom_Bars_y;
            }

            set
            {
                m_Bottom_Bars_y = value;
            }
        }

        public int Count_Top_Bars_x
        {
            get
            {
                return m_Count_Top_Bars_x;
            }

            set
            {
                m_Count_Top_Bars_x = value;
            }
        }

        public int Count_Top_Bars_y
        {
            get
            {
                return m_Count_Top_Bars_y;
            }

            set
            {
                m_Count_Top_Bars_y = value;
            }
        }

        public int Count_Bottom_Bars_x
        {
            get
            {
                return m_Count_Bottom_Bars_x;
            }

            set
            {
                m_Count_Bottom_Bars_x = value;
            }
        }

        public int Count_Bottom_Bars_y
        {
            get
            {
                return m_Count_Bottom_Bars_y;
            }

            set
            {
                m_Count_Bottom_Bars_y = value;
            }
        }

        public float DistanceOfBars_Top_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Top_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_y_SpacingInxDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_y_SpacingInxDirection = value;
            }
        }

        public float ConcreteCover
        {
            get
            {
                return m_fConcreteCover;
            }

            set
            {
                m_fConcreteCover = value;
            }
        }

        public string MeshGradeName
        {
            get { return m_sMeshGradeName; }
            set { m_sMeshGradeName = value; }
        }

        public List<Point3D> WireFramePoints
        {
            get
            {
                if (MWireFramePoints == null) MWireFramePoints = new List<Point3D>();
                return MWireFramePoints;
            }

            set
            {
                MWireFramePoints = value;
            }
        }

        private Point3D m_PointText;

        public Point3D PointText
        {
            get { return m_PointText; }
            set { m_PointText = value; }
        }

        private string m_Text;

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public int NumberOfSawCutsInDirectionX
        {
            get { return m_NumberOfSawCutsInDirectionX; }
            set { m_NumberOfSawCutsInDirectionX = value; }
        }

        public int NumberOfSawCutsInDirectionY
        {
            get { return m_NumberOfSawCutsInDirectionY; }
            set { m_NumberOfSawCutsInDirectionY = value; }
        }

        public float FirstSawCutPositionInDirectionX
        {
            get { return m_FirstSawCutPositionInDirectionX; }
            set { m_FirstSawCutPositionInDirectionX = value; }
        }

        public float FirstSawCutPositionInDirectionY
        {
            get { return m_FirstSawCutPositionInDirectionY; }
            set { m_FirstSawCutPositionInDirectionY = value; }
        }

        public float SawCutsSpacingInDirectionX
        {
            get { return m_SawCutsSpacingInDirectionX; }
            set { m_SawCutsSpacingInDirectionX = value; }
        }

        public float SawCutsSpacingInDirectionY
        {
            get { return m_SawCutsSpacingInDirectionY; }
            set { m_SawCutsSpacingInDirectionY = value; }
        }

        public List<CSawCut> SawCuts
        {
            get { return m_SawCuts; }
            set { m_SawCuts = value; }
        }

        public int NumberOfControlJointsInDirectionX
        {
            get { return m_NumberOfControlJointsInDirectionX; }
            set { m_NumberOfControlJointsInDirectionX = value; }
        }

        public int NumberOfControlJointsInDirectionY
        {
            get { return m_NumberOfControlJointsInDirectionY; }
            set { m_NumberOfControlJointsInDirectionY = value; }
        }

        public float FirstControlJointPositionInDirectionX
        {
            get { return m_FirstControlJointPositionInDirectionX; }
            set { m_FirstControlJointPositionInDirectionX = value; }
        }

        public float FirstControlJointPositionInDirectionY
        {
            get { return m_FirstControlJointPositionInDirectionY; }
            set { m_FirstControlJointPositionInDirectionY = value; }
        }

        public float ControlJointsSpacingInDirectionX
        {
            get { return m_ControlJointsSpacingInDirectionX; }
            set { m_ControlJointsSpacingInDirectionX = value; }
        }

        public float ControlJointsSpacingInDirectionY
        {
            get { return m_ControlJointsSpacingInDirectionY; }
            set { m_ControlJointsSpacingInDirectionY = value; }
        }

        public List<CControlJoint> ControlJoints
        {
            get { return m_ControlJoints; }
            set { m_ControlJoints = value; }
        }

        public CSlab()
        {
        }

        // Rectangular
        public CSlab(int iSlab_ID,
            Point3D pControlEdgePoint,
            MATERIAL.CMat_02_00 materialConcrete,
            float fX,
            float fY,
            float fZ,
            float ex,
            float ey,
            float rotationAboiutZInDeg,
            float fConcreteCover,
            string sMeshGradeName,
            string descriptionText,
            int   iNumberOfSawCutsInDirectionX,
            int   iNumberOfSawCutsInDirectionY,
            float fFirstSawCutPositionInDirectionX,
            float fFirstSawCutPositionInDirectionY,
            float fSawCutsSpacingInDirectionX,
            float fSawCutsSpacingInDirectionY,
            int   iNumberOfControlJointsInDirectionX,
            int   iNumberOfControlJointsInDirectionY,
            float fFirstControlJointPositionInDirectionX,
            float fFirstControlJointPositionInDirectionY,
            float fControlJointsSpacingInDirectionX,
            float fControlJointsSpacingInDirectionY,
            //CReinforcementBar refTopBar_x,
            //CReinforcementBar refTopBar_y,
            //CReinforcementBar refBottomBar_x,
            //CReinforcementBar refBottomBar_y,
            //int iNumberOfBarsTop_x,
            //int iNumberOfBarsTop_y,
            //int iNumberOfBarsBottom_x,
            //int iNumberOfBarsBottom_y,
            Color volColor,
            float fvolOpacity,
            bool bIsDisplayed,
            float fTime)
        {
            ID = iSlab_ID;
            m_pControlPoint = pControlEdgePoint;
            m_Mat = materialConcrete;
            m_fDim1 = fX; // Width
            m_fDim2 = fY; // Length
            m_fDim3 = fZ;
            m_Eccentricity_x = ex;
            m_Eccentricity_y = ey;
            m_RotationAboutZ_deg = rotationAboiutZInDeg;
            m_fConcreteCover = fConcreteCover;
            m_sMeshGradeName = sMeshGradeName;
            m_Text = descriptionText;
            m_NumberOfSawCutsInDirectionX = iNumberOfSawCutsInDirectionX;
            m_NumberOfSawCutsInDirectionY = iNumberOfSawCutsInDirectionY;
            m_FirstSawCutPositionInDirectionX = fFirstSawCutPositionInDirectionX;
            m_FirstSawCutPositionInDirectionY = fFirstSawCutPositionInDirectionY;
            m_SawCutsSpacingInDirectionX = fSawCutsSpacingInDirectionX;
            m_SawCutsSpacingInDirectionY = fSawCutsSpacingInDirectionY;
            m_NumberOfControlJointsInDirectionX = iNumberOfControlJointsInDirectionX;
            m_NumberOfControlJointsInDirectionY = iNumberOfControlJointsInDirectionY;
            m_FirstControlJointPositionInDirectionX = fFirstControlJointPositionInDirectionX;
            m_FirstControlJointPositionInDirectionY = fFirstControlJointPositionInDirectionY;
            m_ControlJointsSpacingInDirectionX = fControlJointsSpacingInDirectionX;
            m_ControlJointsSpacingInDirectionY = fControlJointsSpacingInDirectionY;

            //m_Reference_Top_Bar_x = refTopBar_x;
            //m_Reference_Top_Bar_y = refTopBar_y;
            //m_Reference_Bottom_Bar_x = refBottomBar_x;
            //m_Reference_Bottom_Bar_y = refBottomBar_y;
            //m_Count_Top_Bars_x = iNumberOfBarsTop_x;
            //m_Count_Top_Bars_y = iNumberOfBarsTop_y;
            //m_Count_Bottom_Bars_x = iNumberOfBarsBottom_x;
            //m_Count_Bottom_Bars_y = iNumberOfBarsBottom_y;
            m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            SetTextPoint();

            CreateSawCuts();
            CreateControlJoints();
        }

        public void SetTextPoint()
        {
            // V systeme GCS
            float fAdditionalOffsetX = 0.6f;
            float fAdditionalOffsetY = 0.7f; // TODO Ondrej - Toto by bolo super vediet nastavit podla polohy saw cut a control joint aby sa neprekryvali texty
            float fOffsetX = 0.5f * m_fDim1 + fAdditionalOffsetX;
            float fOffsetY = 0.5f * m_fDim2 + fAdditionalOffsetY;
            float fOffsetFromPlane = m_fDim3 + 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

            m_PointText = new Point3D()
            {
                X = fOffsetX,
                Y = fOffsetY,
                Z = fOffsetFromPlane
            };
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(float fOpacity)
        {
            Visual_Object = CreateGeomModel3D(new SolidColorBrush(m_volColor_2), fOpacity);
            return Visual_Object;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(SolidColorBrush brush, float fOpacity)
        {
            brush.Opacity = fOpacity; // Set brush opacity // TODO - mozeme nejako vypesit a prepojit s GUI, aby to bol piamo parameter zadavany spolu s farbou zakladu

            GeometryModel3D model = new GeometryModel3D();

            // TODO - pohrat sa s nastavenim farieb
            DiffuseMaterial qDiffTrans = new DiffuseMaterial(brush);
            SpecularMaterial qSpecTrans = new SpecularMaterial(brush/*new SolidColorBrush(Color.FromArgb(210, 210, 210, 210))*/, 90.0);

            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTrans);
            qOuterMaterial.Children.Add(qSpecTrans);

            // Create slab - origin [0,0,0]
            CVolume volume = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial, true, 0);
            model = volume.CreateM_3D_G_Volume_8Edges(new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial);

            // Set the Transform property of the GeometryModel to the Transform3DGroup
            // Nastavim vyslednu transformaciu
            model.Transform = GetSlabTransformGroup();

            // Naplnime pole bodov wireFrame
            // TODO - Ondrej - chcelo by to nejako elegantne zjednotit u vsetkych objektov ktore maju 3D geometriu kde a ako ziskavat wireframe
            // TODO Ondrej - tu chyba v tom ze beriem pozicie z povodneho zakladu nie z posunuteho do finalnej pozicie
            WireFramePoints = GetWireFramePoints_Volume_8Edges(model);

            Visual_Object = model;

            return model;
        }

        public Transform3DGroup GetSlabTransformGroup()
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            // Rotate about Z axis - otocime patku okolo [0,0,0]
            // Create and apply a transformation that rotates the object.
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 1);
            myAxisAngleRotation3d.Angle = RotationAboutZ_deg;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            // Add the rotation transform to a Transform3DGroup
            myTransform3DGroup.Children.Add(myRotateTransform3D);

            // Presun celej dosky do GCS z [0,0,0] do control point
            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D_GCS = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D_GCS);

            return myTransform3DGroup;
        }

        private void CreateSawCuts()
        {
            bool bGenerateSawCuts = true;

            if (bGenerateSawCuts)
            {
                float fcutWidth = 0.01f;
                float fcutDepth = 0.03f;

                SawCuts = new List<CSawCut>();

                // Sawcuts per X axis - rezanie v smere Y
                for (int i = 0; i < NumberOfSawCutsInDirectionX; i++)
                {
                    double coordX = m_pControlPoint.X;
                    double coordStartY = m_pControlPoint.Y;
                    double coordEndY = m_pControlPoint.Y + m_fDim2;
                    double coordZ = 0;

                    if (i == 0) // First
                    {
                        coordX = m_pControlPoint.X + FirstSawCutPositionInDirectionX;
                        SawCuts.Add(new CSawCut(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), fcutWidth, fcutDepth, true, 0));
                    }
                    else
                    {
                        coordX = m_pControlPoint.X + FirstSawCutPositionInDirectionX + i * SawCutsSpacingInDirectionX;
                        SawCuts.Add(new CSawCut(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), fcutWidth, fcutDepth, true, 0));
                    }
                }
                // Sawcuts per Y axis - rezanie v smere X
                for (int i = 0; i < NumberOfSawCutsInDirectionY; i++)
                {
                    double coordStartX = m_pControlPoint.X;
                    double coordEndX = m_pControlPoint.X + m_fDim1;
                    double coordY = m_pControlPoint.Y;
                    double coordZ = 0;

                    if (i == 0) // First
                    {
                        coordY = m_pControlPoint.Y + FirstSawCutPositionInDirectionY;
                        SawCuts.Add(new CSawCut(NumberOfSawCutsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), fcutWidth, fcutDepth, true, 0));
                    }
                    else
                    {
                        coordY = m_pControlPoint.Y + FirstSawCutPositionInDirectionY + i * SawCutsSpacingInDirectionY;
                        SawCuts.Add(new CSawCut(NumberOfSawCutsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), fcutWidth, fcutDepth, true, 0));
                    }
                }
            }
        }

        private void CreateControlJoints()
        {
            bool bGenerateControlJoints = true;

            if (bGenerateControlJoints)
            {
                //Diameters available = 10, 12, 16, 20, 25, 32, 40
                /*
                12mm x 460mm Galvanised Dowel
                16mm x 400mm Galvanised Dowel
                16mm x 600mm Galvanised Dowel
                20mm x 400mm Galvanised Dowel
                20mm x 600mm Galvanised Dowel
                33mm x 450mm Galvanised Dowel
                */

                CDowel referenceDowel = new CDowel(new Point3D(0, 0, 0), 0.033f, 0.6f, 4.028f, true);
                float fDowelSpacing = 0.4f;

                // Create raster of lines in XY-plane
                ControlJoints = new List<CControlJoint>();

                // ControlJoints per X axis
                for (int i = 0; i < NumberOfControlJointsInDirectionX; i++)
                {
                    double coordX = m_pControlPoint.X;
                    double coordStartY = m_pControlPoint.Y;
                    double coordEndY = m_pControlPoint.Y + m_fDim2;
                    double coordZ = 0;

                    if (i == 0) // First
                    {
                        coordX = m_pControlPoint.X + FirstControlJointPositionInDirectionX;
                        ControlJoints.Add(new CControlJoint(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), referenceDowel, fDowelSpacing, true, 0));
                    }
                    else
                    {
                        coordX = m_pControlPoint.X + FirstControlJointPositionInDirectionX + i * ControlJointsSpacingInDirectionX;
                        ControlJoints.Add(new CControlJoint(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), referenceDowel, fDowelSpacing, true, 0));
                    }
                }
                // ControlJoints per Y axis
                for (int i = 0; i < NumberOfControlJointsInDirectionY; i++)
                {
                    double coordStartX = m_pControlPoint.X;
                    double coordEndX = m_pControlPoint.X + m_fDim1;
                    double coordY = m_pControlPoint.Y;
                    double coordZ = 0;

                    if (i == 0) // First
                    {
                        coordY = m_pControlPoint.Y + FirstControlJointPositionInDirectionY;
                        ControlJoints.Add(new CControlJoint(NumberOfControlJointsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), referenceDowel, fDowelSpacing, true, 0));
                    }
                    else
                    {
                        coordY = m_pControlPoint.Y + FirstControlJointPositionInDirectionY + i * ControlJointsSpacingInDirectionY;
                        ControlJoints.Add(new CControlJoint(NumberOfControlJointsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), referenceDowel, fDowelSpacing, true, 0));
                    }
                }
            }
        }
    }
}
