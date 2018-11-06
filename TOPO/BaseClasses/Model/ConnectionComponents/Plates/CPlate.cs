using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using MATH;
using MATERIAL;
using BaseClasses.GraphObj;
using System.Collections.ObjectModel;

namespace BaseClasses
{
    [Serializable]
    public abstract class CPlate : CConnectionComponentEntity3D
    {
        public ESerieTypePlate m_ePlateSerieType_FS; // Type of plate - FormSteel
        public float fWidth_bx;
        public float fHeight_hy;

        private float m_ft; // Thickness

        public float Ft
        {
            get
            {
                return m_ft;
            }

            set
            {
                m_ft = value;
            }
        }

        public float fArea;
        public float fA_g; // Gross area
        public float fA_n; // Net area
        public float fA_v_zv; // Shear area
        public float fA_vn_zv; // Net shear area
        public float fI_yu; // Moment of inertia of plate
        public float fW_el_yu; // Elastic section modulus
        public float fCuttingRouteDistance;
        public float fSurface;
        public float fVolume;
        public float fMass;

        [NonSerialized]
        public GeometryModel3D Visual_Plate;
        [NonSerialized]
        public CAnchorArrangement AnchorArrangement;
        public CScrewArrangement ScrewArrangement;

        public float m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg;

        public int ITotNoPointsin3D; // Number of all points in 3D (excluding auxiliary)
        public int ITotNoPointsin2D; // Number of all points in 2D (excluding auxiliary)
        private List<Point> m_drillingRoutePoints;
        public List<Point> DrillingRoutePoints
        {
            get
            {
                return m_drillingRoutePoints;
            }

            set
            {
                m_drillingRoutePoints = value;
            }
        }
        [NonSerialized]
        private CDimension[] m_dimensions; // Pole kot pre Plate
        public CDimension[] Dimensions
        {
            get
            {
                return m_dimensions;
            }

            set
            {
                m_dimensions = value;
            }
        }

        private CLine2D [] m_MemberOutlines; // Pole linii pre vykreslenie outline pruta v plechu

        public CLine2D[] MemberOutlines
        {
            get
            {
                return m_MemberOutlines;
            }

            set
            {
                m_MemberOutlines = value;
            }
        }

        private CLine2D[] m_BendLines; // Pole ohybovych linii - hrany ohnuteho plechu

        public CLine2D[] BendLines
        {
            get
            {
                return m_BendLines;
            }

            set
            {
                m_BendLines = value;
            }
        }

        public const int INumberOfPointsOfHole = 12; // Have to be Even - Todo funguje pre 12 bodov, napr. pre 24 je tam chyba, je potrebne "doladit"
        public Point3D[] arrConnectorControlPoints3D; // Array of control points for inserting connectors (bolts, screws, anchors, ...)

        public int INoPoints2Dfor3D; // Number of points in one surface used for 3D model (holes lines are divided to the straight segments)

        private float m_fs_f_min;  // Minimalna vzdialenost skrutiek kolmo na smer osovej sily v prute

        public float S_f_min
        {
            get
            {
                return m_fs_f_min;
            }

            set
            {
                m_fs_f_min = value;
            }
        }

        private float m_fs_f_max;  // Maximalna vzdialenost skrutiek kolmo na smer osovej sily v prute

        public float S_f_max
        {
            get
            {
                return m_fs_f_max;
            }

            set
            {
                m_fs_f_max = value;
            }
        }

        private int m_iNumberOfConnectorsInSection;  // Pocet skrutiek v reze kolmom na smer osovej sily v prute

        public int INumberOfConnectorsInSection
        {
            get
            {
                return m_iNumberOfConnectorsInSection;
            }

            set
            {
                m_iNumberOfConnectorsInSection = value;
            }
        }

        public ObservableCollection<string> Series = new ObservableCollection<string>()
        {
             "Serie B",
             "Serie L",
             "Serie LL",
             "Serie F",
             "Serie Q",
             "Serie S",
             "Serie T",
             "Serie X",
             "Serie Y",
             "Serie J",
             "Serie K"
        };

        public CPlate()
        {
            BIsDisplayed = true;
            m_Mat = new CMat_03_00();
        }

        public CPlate(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
            m_Mat = new CMat_03_00();
        }

        protected override void loadIndices()
        { }

        public override void loadWireFrameIndices()
        { }

        // TODO - zjednotit funkcie s triedou CCRSC

        // Draw Hexagon / Add hexagon indices - clockwise CW numbering of input points 1,2,3,4,5,6 (see scheme)
        // Add in order 1,2,3,4,5,6
        protected void AddHexagonIndices_CW_123456(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4,
              int point5, int point6)
        {
            // Main numbering is clockwise

            //   6  _  1
            // 5  /   \  2
            //   |_____|
            // 4         3

            // Triangles Numbering is Counterclockwise

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point5);
            Indices.Add(point4);

            Indices.Add(point1);
            Indices.Add(point6);
            Indices.Add(point5);
        }

        // Draw Hexagon / Add hexagon indices - countrer-clockwise CCW numbering of input points 1,2,3,4,5,6 (see scheme)
        // Add in order 1,6,5,4,3,2
        protected void AddHexagonIndices_CCW_123456(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4,
              int point5, int point6)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            //   6  _  1
            // 5  /   \  2
            //   |_____|
            // 4         3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point4);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point5);

            Indices.Add(point1);
            Indices.Add(point5);
            Indices.Add(point6);
        }

        // Draw Penthagon / Add penthagon indices - clockwise CW numbering of input points 1,2,3,4,5 (see scheme)
        // Add in order 1,2,3,4,5
        protected void AddPenthagonIndices_CW_12345(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4, int point5)
        {
            // Main numbering is clockwise

            //     1
            // 5  / \  2
            //   |___|
            // 4       3

            // Triangles Numbering is Counterclockwise

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point5);
            Indices.Add(point4);
        }

        // Draw Penthagon / Add pengthagon indices - countrer-clockwise CCW numbering of input points 1,2,3,4,5 (see scheme)
        // Add in order 1,5,4,3,2
        protected void AddPenthagonIndices_CCW_12345(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4, int point5)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            //     1
            // 5  / \  2
            //   |___|
            // 4       3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);

            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point4);

            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point5);
        }

        // Draw Rectangle / Add rectangle indices - clockwise CW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,2,3,4
        protected void AddRectangleIndices_CW_1234(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4)
        {
            // Main numbering is clockwise

            // 1  _______  2
            //   |_______| 
            // 4           3

            // Triangles Numbering is Counterclockwise
            // Top Right
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);

            // Bottom Left
            Indices.Add(point1);
            Indices.Add(point4);
            Indices.Add(point3);
        }

        // Draw Rectangle / Add rectangle indices - countrer-clockwise CCW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,4,3,2
        protected void AddRectangleIndices_CCW_1234(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            // 1  _______  2
            //   |_______| 
            // 4           3

            // Triangles Numbering is Clockwise
            // Top Right
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);

            // Bottom Left
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point4);
        }

        // Draw Triangle / Add triangle indices - clockwise CW numbering of input points 1,2,3 (see scheme)
        // Add in order 1,2,3,4
        protected void AddTriangleIndices_CW_123(Int32Collection Indices,
              int point1, int point2,
              int point3)
        {
            // Main numbering is clockwise

            // 1  _______  2
            //           | 
            //             3

            // Triangle Numbering is Counterclockwise
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);
        }

        // Draw Triangle / Add triangle indices - countrer-clockwise CCW numbering of input points 1,2,3 (see scheme)
        // Add in order 1,3,2
        protected void AddTriangleIndices_CCW_123(Int32Collection Indices,
              int point1, int point2,
              int point3)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            // 1  _______  2
            //           | 
            //             3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre kontinualne pravidelne cislovanie bodov
        protected void DrawCaraLaterals_CW(int secNum, Int32Collection Indices)
        {
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CW_1234(Indices, i, secNum + i, secNum + i + 1, i + 1);
                else
                    AddRectangleIndices_CW_1234(Indices, i, secNum + i, secNum, 0); // Last Element
            }
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre pravidelne cislovanie bodov s vynechanim pociatocnych uzlov - pomocne
        protected void DrawCaraLaterals_CW(int iAuxNum, int secNum, Int32Collection Indices)
        {
            // iAuxNum - number of auxiliary points - start ofset
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy (tento pocet neobsahuje pomocne body iAuxNum)

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum + i + 1, iAuxNum + i + 1);
                else
                    AddRectangleIndices_CW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum, iAuxNum + 0); // Last Element
            }
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre kontinualne pravidelne cislovanie bodov
        protected void DrawCaraLaterals_CCW(int secNum, Int32Collection Indices)
        {
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CCW_1234(Indices, i, secNum + i, secNum + i + 1, i + 1);
                else
                    AddRectangleIndices_CCW_1234(Indices, i, secNum + i, secNum, 0); // Last Element
            }
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre pravidelne cislovanie bodov s vynechanim pociatocnych uzlov - pomocne
        protected void DrawCaraLaterals_CCW(int iAuxNum, int secNum, Int32Collection Indices)
        {
            // iAuxNum - number of auxiliary points - start ofset
            // secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy (tento pocet neobsahuje pomocne body iAuxNum)

            // Shell (Face)Surface
            // Cycle for regular numbering of section points

            for (int i = 0; i < secNum; i++)
            {
                if (i < secNum - 1)
                    AddRectangleIndices_CCW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum + i + 1, iAuxNum + i + 1);
                else
                    AddRectangleIndices_CCW_1234(Indices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum, iAuxNum + 0); // Last Element
            }
        }

        public void TransformPlateCoord(GeometryModel3D model)
        {
            model.Transform = CreateTransformCoordGroup();
        }

        public void TransformPlateCoord(ScreenSpaceLines3D wireframeModel)
        {
            wireframeModel.Transform = CreateTransformCoordGroup();
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate Plate from its cs to joint cs system in LCS of member or GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), m_fRotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_fRotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ_deg); // Rotation in degrees

            // Move 0,0,0 to control point in LCS of member or GCS
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }

        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;

            model.Material = new DiffuseMaterial(brush);  // Set Model Material

            TransformPlateCoord(model);
            return model;
        }

        public Model3DGroup CreateGeomModel3DWithConnectors(SolidColorBrush brush, SolidColorBrush brushConnectors)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;

            model.Material = new DiffuseMaterial(brush);  // Set Model Material

            TransformPlateCoord(model);

            Model3DGroup gr = new Model3DGroup();
            gr.Children.Add(model);

            // Add plate connectors
            if (ScrewArrangement != null && ScrewArrangement.Screws != null && ScrewArrangement.Screws.Length > 0)
            {
                Model3DGroup plateConnectorsModelGroup = new Model3DGroup();
                if (brushConnectors == null) brushConnectors = new SolidColorBrush(Colors.Red);

                for (int m = 0; m < ScrewArrangement.Screws.Length; m++)
                {
                    GeometryModel3D plateConnectorgeom = ScrewArrangement.Screws[m].CreateGeomModel3D(brushConnectors);
                    ScrewArrangement.Screws[m].Visual_Connector = plateConnectorgeom;
                    plateConnectorsModelGroup.Children.Add(plateConnectorgeom);
                }
                plateConnectorsModelGroup.Transform = model.Transform;
                gr.Children.Add(plateConnectorsModelGroup);
            }

            return gr;
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D ssl3D = new ScreenSpaceLines3D();
            return ssl3D;
        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            foreach (Point3D point in arrPoints3D)
                pMeshPositions.Add(point);

            return pMeshPositions;
        }

        public float GetCuttingRouteDistance()
        {
            float fDistance = 0;

            for (int i = 0; i < PointsOut2D.Length; i++)
            {
                if (i < PointsOut2D.Length - 1)
                {
                    fDistance += (float)Math.Sqrt(MathF.Pow2(PointsOut2D[i + 1].X - PointsOut2D[i].X) + MathF.Pow2(PointsOut2D[i + 1].Y - PointsOut2D[i].Y));
                }
                else // Last segment
                {
                    fDistance += (float)Math.Sqrt(MathF.Pow2(PointsOut2D[0].X - PointsOut2D[i].X) + MathF.Pow2(PointsOut2D[0].Y - PointsOut2D[i].Y));
                }
            }

            return fDistance;
        }

        public float GetSurfaceIgnoringHoles()
        {
            return 2 * PolygonArea() + Ft * GetCuttingRouteDistance(); // front side, back side, lateral area
        }

        public float GetVolumeIgnoringHoles()
        {
            return Ft * PolygonArea();
        }

        public float GetMassIgnoringHoles()
        {
            if (m_Mat == null) return -1;

            return m_Mat.m_fRho * GetVolumeIgnoringHoles();
        }

        // Return the polygon's area in "square units."
        // The value will be negative if the polygon is
        // oriented clockwise.

        private float SignedPolygonArea()
        {
            // Add the first point to the end.
            int num_points = PointsOut2D.Length;
            Point[] pts = new Point[num_points + 1];

            for (int i = 0; i < num_points; i++)
            {
                pts[i].X = PointsOut2D[i].X;
                pts[i].Y = PointsOut2D[i].Y;
            }

            pts[num_points].X = PointsOut2D[0].X;
            pts[num_points].Y = PointsOut2D[0].Y;

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area += (float)(
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2);
            }

            // Return the result.
            return area;
        }

        // Return the polygon's area in "square units."
        public float PolygonArea()
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polyogn is
            // oriented clockwise.
            return Math.Abs(SignedPolygonArea());
        }

        public float Get_A_rect(float ft, float fh)
        {
            return ft * fh; // Gross sectional area of plate
        }

        public float Get_A_channel(float fb, float ft_web, float ft_lip, float fh)
        {
            float fA_rect_1 = Get_A_rect(fb, fh);
            float fA_rect_2 = Get_A_rect(fb - ft_web, fh - 2f * ft_lip);

            return fA_rect_1 - fA_rect_2; // Gross sectional area of plate
        }

        public float Get_I_yu_rect(float ft, float fh)
        {
            return 1f / 12f * ft * MathF.Pow3(fh); // Moment of inertia of plate
        }

        public float Get_I_yu_channel(float fb, float ft_web, float ft_lip, float fh)
        {
            float fI_yu_rect_1 = Get_I_yu_rect(fb, fh);
            float fI_yu_rect_2 = Get_I_yu_rect(fb - ft_web, fh - 2f * ft_lip);

            return fI_yu_rect_1 - fI_yu_rect_2; // Moment of inertia of plate
        }

        public float Get_W_el_yu(float fI_yu_rect, float fh)
        {
            return 2f * fI_yu_rect / fh; // Elastic section modulus
        }

        // Member Outline - add mirrored lines (plates J)
        public CLine2D[] AddMirroredLinesAboutY(float fXDistanceOfMirrorAxis, CLine2D[] MemberOutlines_temp)
        {
            if (MemberOutlines_temp != null && MemberOutlines_temp.Length > 0)
            {
                // Resize array
                Array.Resize(ref MemberOutlines_temp, MemberOutlines_temp.Length * 2);

                for (int i = 0; i < MemberOutlines_temp.Length / 2; i++)
                {
                    // Start Point
                    MemberOutlines_temp[MemberOutlines_temp.Length / 2 + i] = new CLine2D(new Point(2 * fXDistanceOfMirrorAxis + MemberOutlines_temp[i].X1 * (-1f), MemberOutlines_temp[i].Y1),
                                                                                          new Point(2 * fXDistanceOfMirrorAxis + MemberOutlines_temp[i].X2 * (-1f), MemberOutlines_temp[i].Y2));
                }
            }

            return MemberOutlines_temp;
        }

        // Modification
        // Mirror plate about x
        public void MirrorPlateAboutX()
        {
            Geom2D.MirrorAboutX_ChangeYCoordinates(ref PointsOut2D);
            //Geom3D.MirrorAboutX_ChangeYCoordinates(ref arrPoints3D);

            // TODO ??? ScrewArrangement.HolesCentersPoints2D sa neda predat refenciou
            // BUG No. 72 - Ondrej - tu sa do ScrewArrangement.HolesCentersPoints2D nastavia odzrkadlene suradnice ale ked sa to zavola znova tak su tam povodne resp nejake uplne ine po opakovanom spusteni funckie
            // Problem je vo vsetkych funkciach pre mirror, rotate of plate
            if (ScrewArrangement != null)
            {
                Point[] pArrayTemp = ScrewArrangement.HolesCentersPoints2D;
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref pArrayTemp);
                ScrewArrangement.HolesCentersPoints2D = pArrayTemp;
            }

            //Geom3D.MirrorAboutX_ChangeYCoordinates(ref arrConnectorControlPoints3D);

            if (DrillingRoutePoints != null)
            {
                Geom2D.MirrorAboutX_ChangeYCoordinates(ref m_drillingRoutePoints);
            }

            //Dimensions
            MirrorAboutX_ChangeYDimensionsCoordinates();

            //CLine2D[]
            MirrorAboutX_ChangeYLinesCoordinates();
        }

        private void MirrorAboutX_ChangeYDimensionsCoordinates()
        {
            if (Dimensions != null)
            {
                foreach (CDimension d in Dimensions)
                {
                    d.MirrorYCoordinates();
                }
            }
        }
        private void MirrorAboutX_ChangeYLinesCoordinates()
        {
            if (MemberOutlines != null)
            {
                foreach (CLine2D l in MemberOutlines)
                {
                    l.MirrorYCoordinates();
                }
            }
            if (BendLines != null)
            {
                foreach (CLine2D l in BendLines)
                {
                    l.MirrorYCoordinates();
                }
            }
        }

        // Mirror plate about y
        public void MirrorPlateAboutY()
        {
            Geom2D.MirrorAboutY_ChangeXCoordinates(ref PointsOut2D);
            //Geom3D.MirrorAboutY_ChangeXCoordinates(ref arrPoints3D);

            // TODO ??? ScrewArrangement.HolesCentersPoints2D sa neda predat refenciou
            // BUG No. 72 - Ondrej - tu sa do ScrewArrangement.HolesCentersPoints2D nastavia odzrkadlene suradnice ale ked sa to zavola znova tak su tam povodne resp nejake uplne ine po opakovanom spusteni funckie
            // Problem je vo vsetkych funkciach pre mirror, rotate of plate
            if (ScrewArrangement != null)
            {
                Point[] pArrayTemp = ScrewArrangement.HolesCentersPoints2D;
                Geom2D.MirrorAboutY_ChangeXCoordinates(ref pArrayTemp);
                ScrewArrangement.HolesCentersPoints2D = pArrayTemp;
            }

            Geom3D.MirrorAboutY_ChangeXCoordinates(ref arrConnectorControlPoints3D);

            if (DrillingRoutePoints != null)
            {
                Geom2D.MirrorAboutY_ChangeXCoordinates(ref m_drillingRoutePoints);
            }

            //Dimensions
            MirrorAboutY_ChangeXDimensionsCoordinates();

            //CLine2D[]
            MirrorAboutY_ChangeXLinesCoordinates();
        }

        private void MirrorAboutY_ChangeXDimensionsCoordinates()
        {
            if (Dimensions != null)
            {
                foreach (CDimension d in Dimensions)
                {
                    d.MirrorXCoordinates();
                }
            }
        }

        private void MirrorAboutY_ChangeXLinesCoordinates()
        {
            if (MemberOutlines != null)
            {
                foreach (CLine2D l in MemberOutlines)
                {
                    l.MirrorXCoordinates();
                }
            }
            if (BendLines != null)
            {
                foreach (CLine2D l in BendLines)
                {
                    l.MirrorXCoordinates();
                }
            }
        }

        // Rotate plate
        public void RotatePlateAboutZ_CW(float fTheta_deg)
        {
            Geom2D.TransformPositions_CW_deg(0, 0, fTheta_deg, ref PointsOut2D);
            //Geom3D.TransformPositionsAboutZ_CW_deg(new Point3D(0, 0, 0), fTheta_deg, ref arrPoints3D);

            // TODO ??? ScrewArrangement.HolesCentersPoints2D sa neda predat refenciou
            // BUG No. 72 - pri opakovanom volani sa nepouziji nastavene suradnice ale podovne, resp uplne ineProblem je vo vsetkych funkciach pre mirror, rotate of plate
            // TO Mato - je ten koment vyssie aktualny?
            if (ScrewArrangement != null)
            {
                Point[] pArrayTemp = ScrewArrangement.HolesCentersPoints2D;
                Geom2D.TransformPositions_CW_deg(0, 0, fTheta_deg, ref pArrayTemp);
                ScrewArrangement.HolesCentersPoints2D = pArrayTemp;
            }

            //Geom3D.TransformPositionsAboutZ_CW_deg(new Point3D(0, 0, 0), fTheta_deg, ref arrConnectorControlPoints3D);

            if (DrillingRoutePoints != null)
            {
                Geom2D.TransformPositions_CW_deg(0, 0, fTheta_deg, ref m_drillingRoutePoints);
            }

            //DIMENSIONS
            TransformPlateDimensions_CW_deg(fTheta_deg);

            //CLine2D[]
            TransformPlateLines2D_CW_deg(fTheta_deg);

        }

        private void TransformPlateDimensions_CW_deg(double theta_deg)
        {
            if (Dimensions != null)
            {
                foreach (CDimension d in Dimensions)
                {
                    if (d is CDimensionLinear)
                    {
                        CDimensionLinear dl = d as CDimensionLinear;
                        dl.ControlPointStart = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, dl.ControlPointStart);
                        dl.ControlPointEnd = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, dl.ControlPointEnd);
                        dl.ControlPointRef = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, dl.ControlPointRef);
                    }
                    if (d is CDimensionArc)
                    {
                        CDimensionArc da = d as CDimensionArc;
                        da.ControlPointStart = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, da.ControlPointStart);
                        da.ControlPointEnd = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, da.ControlPointEnd);
                        da.ControlPointCenter = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, da.ControlPointCenter);
                        da.ControlPointRef = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, da.ControlPointRef);
                    }
                }
            }
        }

        private void TransformPlateLines2D_CW_deg(double theta_deg)
        {
            if (MemberOutlines != null)
            {
                foreach (CLine2D l in MemberOutlines)
                {
                    l.P1 = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, l.P1);
                    l.P2 = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, l.P2);
                }
            }
            if (BendLines != null)
            {
                foreach (CLine2D l in BendLines)
                {
                    l.P1 = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, l.P1);
                    l.P2 = Geom2D.TransformPositions_CW_deg(0, 0, theta_deg, l.P2);
                }
            }
        }

        public virtual void Calc_Coord2D()
        {
            //to override
        }

        public virtual void Calc_Coord3D()
        {
            //to override
        }

        public virtual void Set_DimensionPoints2D()
        {
            //to override
        }

        public virtual void Set_MemberOutlinePoints2D()
        {
            //to override
        }

        public virtual void Set_BendLinesPoints2D()
        {
            //to override
        }

        public virtual void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            //to override
        }

        public virtual void UpdatePlateData(CAnchorArrangement_BB_BG anchorArrangement, CScrewArrangement screwArrangement)
        {
            //to overiride
        }
    }
}
