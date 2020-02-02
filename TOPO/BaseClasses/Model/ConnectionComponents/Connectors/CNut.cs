using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Windows;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using MATH;
using _3DTools;
using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    [Serializable]
    public class CNut : CConnectionComponentEntity3D
    {
        private string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }

        private float m_fPitch_coarse;
        public float Pitch_coarse
        {
            get
            {
                return m_fPitch_coarse;
            }

            set
            {
                m_fPitch_coarse = value;
            }
        }

        private float m_fSizeAcrossFlats_max;
        public float SizeAcrossFlats_max
        {
            get
            {
                return m_fSizeAcrossFlats_max;
            }

            set
            {
                m_fSizeAcrossFlats_max = value;
            }
        }

        private float m_fSizeAcrossFlats_min;
        public float SizeAcrossFlats_min
        {
            get
            {
                return m_fSizeAcrossFlats_min;
            }

            set
            {
                m_fSizeAcrossFlats_min = value;
            }
        }

        private float m_fSizeAcrossCorners;
        public float SizeAcrossCorners
        {
            get
            {
                return m_fSizeAcrossCorners;
            }

            set
            {
                m_fSizeAcrossCorners = value;
            }
        }

        private float m_fThickness_max;
        public float Thickness_max
        {
            get
            {
                return m_fThickness_max;
            }

            set
            {
                m_fThickness_max = value;
            }
        }

        private float m_fThickness_min;
        public float Thickness_min
        {
            get
            {
                return m_fThickness_min;
            }

            set
            {
                m_fThickness_min = value;
            }
        }

        private float m_fMass;
        public float Mass
        {
            get
            {
                return m_fMass;
            }

            set
            {
                m_fMass = value;
            }
        }

        private float m_fPrice_PPKG_NZD;
        public float Price_PPKG_NZD
        {
            get
            {
                return m_fPrice_PPKG_NZD;
            }

            set
            {
                m_fPrice_PPKG_NZD = value;
            }
        }

        private float m_fPrice_PPP_NZD;
        public float Price_PPP_NZD
        {
            get
            {
                return m_fPrice_PPP_NZD;
            }

            set
            {
                m_fPrice_PPP_NZD = value;
            }
        }

        List<Point> pointsIn_2D;
        List<Point> pointsOut_2D;

        short iEdgesOutBasic = 6;
        short iNumberOfSegmentsPerSideOut = 8;

        int INoPoints2Dfor3D;
        int ITotNoPointsin3D;

        [NonSerialized]
        public DiffuseMaterial m_DiffuseMat;
        [NonSerialized]
        public GeometryModel3D Visual_Nut;

        public float m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg;

        public CNut() : base()
        {
        }

        public CNut(string name_temp, string nameMaterial_temp, Point3D controlpoint, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg)
        {
            Prefix = "HEX Nut";
            m_Name = name_temp;

            m_pControlPoint = controlpoint;

            // Load properties from the database
            if (name_temp != null)
            {
                CBoltNutProperties properties = CBoltNutsManager.GetBoltNutProperties(name_temp, "Nuts");

                if (properties != null)
                {
                    m_fPitch_coarse = (float)properties.Pitch_coarse;
                    m_fSizeAcrossFlats_max = (float)properties.SizeAcrossFlats_max;
                    m_fSizeAcrossFlats_min = (float)properties.SizeAcrossFlats_min;
                    m_fSizeAcrossCorners = (float)properties.SizeAcrossCorners;
                    m_fThickness_max = (float)properties.Thickness_max;
                    m_fThickness_min = (float)properties.Thickness_min;
                    m_fMass = (float)properties.Mass;
                    m_fPrice_PPKG_NZD = (float)properties.Price_PPKG_NZD;
                    m_fPrice_PPP_NZD = (float)properties.Price_PPP_NZD;
                }
                else
                {
                    throw new ArgumentNullException("Selected nut is not defined in the database.");
                }
            }

            INoPoints2Dfor3D = 2 * iEdgesOutBasic * iNumberOfSegmentsPerSideOut;
            ITotNoPointsin3D = 2 * INoPoints2Dfor3D;

            // Create Array - allocate memory
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            m_Mat.Name = nameMaterial_temp;
            ((CMat_03_00)m_Mat).m_ft_interval = new float[1] { 0.100f };

            CMatPropertiesBOLT materialProperties = CMaterialManager.LoadMaterialPropertiesBOLT(m_Mat.Name);

            ((CMat_03_00)m_Mat).m_ff_yk = new float[1] { (float)materialProperties.Fy };
            ((CMat_03_00)m_Mat).m_ff_u = new float[1] { (float)materialProperties.Fu };

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
        }

        //----------------------------------------------------------------------------
        // TODO Ondrej Refactoring Washer
        public void Calc_Coord2D()
        {
            // Outline Points
            // Outline Edges
            float fRadiusOut = 0.5f * m_fSizeAcrossCorners;  // Toto ta ma nastavit ako polovica dlzky uhlopriecky maximalneho rozmeru nut
            float fRadiusIn = 0.008f; // TODO - toto ta ma nastavit podla priemeru anchor

            int iEdgeOut = iEdgesOutBasic * iNumberOfSegmentsPerSideOut;
            int iEdgesInBasic = iEdgeOut;

            float fAngleBasic_rad = MathF.fPI / iEdgesOutBasic;

            List<Point> pointsOutBasic_2D = Geom2D.GetPolygonPointCoord_RadiusInput_CW(fRadiusOut, iEdgesOutBasic);
            pointsIn_2D = Geom2D.GetPolygonPointCoord_RadiusInput_CW(fRadiusIn, (short)iEdgesInBasic); // Kruh vo vnutri
            pointsOut_2D = Geom2D.GetPolygonPointsIncludingIntermediateOnSides_CW(fRadiusOut, iEdgesOutBasic, iNumberOfSegmentsPerSideOut);
        }

        // TODO Ondrej Refactoring Washer
        public void Calc_Coord3D()
        {
            int iNumberOfPointsPerPolygon = INoPoints2Dfor3D / 2;

            // First layer
            for (int i = 0; i < iNumberOfPointsPerPolygon; i++)
            {
                // Vonkajsi obvod
                arrPoints3D[i].X = pointsOut_2D[i].X;
                arrPoints3D[i].Y = pointsOut_2D[i].Y;
                arrPoints3D[i].Z = 0;

                // Vnutorny kruh
                arrPoints3D[iNumberOfPointsPerPolygon + i].X = pointsIn_2D[i].X;
                arrPoints3D[iNumberOfPointsPerPolygon + i].Y = pointsIn_2D[i].Y;
                arrPoints3D[iNumberOfPointsPerPolygon + i].Z = 0;
            }

            // Second layer
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                arrPoints3D[INoPoints2Dfor3D + i].X = arrPoints3D[i].X;
                arrPoints3D[INoPoints2Dfor3D + i].Y = arrPoints3D[i].Y;
                arrPoints3D[INoPoints2Dfor3D + i].Z = m_fThickness_max;
            }
        }

        protected override void loadIndices()
        {
            LoadIndicesPrismWithOpening(iEdgesOutBasic * iNumberOfSegmentsPerSideOut);
        }

        // TODO Ondrej - refaktorovat s CConnector

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            foreach (Point3D point in arrPoints3D)
                pMeshPositions.Add(point);

            return pMeshPositions;
        }

        public void TransformCoord(GeometryModel3D model)
        {
            model.Transform = CreateTransformCoordGroup();
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), m_fRotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_fRotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ_deg); // Rotation in degrees

            // Move 0,0,0 to the control point
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }

        // TO Ondrej - Refaktorovat s CPlate a dalsimi objektami
        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D(); // Create geometry mesh
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of nut points
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;            // Set Model Geometry
            model.Material = new DiffuseMaterial(brush);  // Set Model Material
            TransformCoord(model);

            return model;
        }
        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            return CreateWireFrameModel(iEdgesOutBasic, iNumberOfSegmentsPerSideOut, false);
        }

        /*public override void loadWireFrameIndices()
        { }*/
    }
}
