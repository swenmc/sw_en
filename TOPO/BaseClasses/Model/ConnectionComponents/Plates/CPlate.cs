using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using _3DTools;
using MATH;
using MATERIAL;
using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BaseClasses
{
    [Serializable]
    public abstract class CPlate : CConnectionComponentEntity3D
    {
        public ESerieTypePlate m_ePlateSerieType_FS; // Type of plate - FS

        private float fWidth_bx;

        public float Width_bx
        {
            get
            {
                return fWidth_bx;
            }

            set
            {
                fWidth_bx = value;
            }
        }

        private float fHeight_hy;

        public float Height_hy
        {
            get
            {
                return fHeight_hy;
            }

            set
            {
                fHeight_hy = value;
            }
        }

        private float fWidth_bx_Stretched; // Rozvinuta sirka - horizontalny smer

        public float Width_bx_Stretched
        {
            get
            {
                return fWidth_bx_Stretched;
            }

            set
            {
                fWidth_bx_Stretched = value;
            }
        }

        private float fHeight_hy_Stretched; // Rozvinuta vyska - vertikalny smer

        public float Height_hy_Stretched
        {
            get
            {
                return fHeight_hy_Stretched;
            }

            set
            {
                fHeight_hy_Stretched = value;
            }
        }

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

        private double _price_PPKG_NZD;
        private double _price_PPP_NZD;

        [NonSerialized]
        public GeometryModel3D Visual_Plate;
        //[NonSerialized]
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

        public double Price_PPKG_NZD
        {
            get
            {
                return _price_PPKG_NZD;
            }

            set
            {
                _price_PPKG_NZD = value;
            }
        }

        public double Price_PPP_NZD
        {
            get
            {
                return _price_PPP_NZD;
            }

            set
            {
                _price_PPP_NZD = value;
            }
        }

        public CPlate()
        {
            // Set Default Material
            CMaterialManager.LoadSteelMaterialProperties((CMat_03_00) m_Mat, "G450");
        }

        // TODO - zjednotit funkcie s triedou CCRSC
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
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }

        // TO Ondrej - Refaktorovat s CNut a dalsimi objektami
        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            //loadIndices(); tato metoda prepisuje znovu triangle indices
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;            // Set Model Geometry
            model.Material = new DiffuseMaterial(brush);  // Set Model Material
            TransformPlateCoord(model);
            return model;
        }

        public override GeometryModel3D CreateGeomModel3DWithTexture(float opacity)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            for (int i = 0; i < mesh.Positions.Count; i = i + 4)
            {
                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(0, 1));
                mesh.TextureCoordinates.Add(new Point(1, 1));
                mesh.TextureCoordinates.Add(new Point(1, 0));
            }

            model.Geometry = mesh;            // Set Model Geometry

            var image = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Textures/zinc04.jpg", UriKind.RelativeOrAbsolute)) };
            RenderOptions.SetCachingHint(image, CachingHint.Cache);
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            VisualBrush visualBrush = new VisualBrush(image);
            visualBrush.Opacity = opacity;
            var material = new DiffuseMaterial(visualBrush);
            model.BackMaterial = material;

            model.Material = material;  // Set Model Material
            TransformPlateCoord(model);
            return model;
        }

        public Model3DGroup CreateGeomModel3DWithConnectors(SolidColorBrush brush, SolidColorBrush brushConnectors)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            //mesh.Positions = new Point3DCollection();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            //loadIndices(); tato funkcia stale prepisuje TriangleIndices
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
            if (arrPoints3D == null) return pMeshPositions;

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
            return 2 * Geom2D.PolygonArea(PointsOut2D) + Ft * GetCuttingRouteDistance(); // front side, back side, lateral area
        }

        public float GetVolumeIgnoringHoles()
        {
            return Ft * Geom2D.PolygonArea(PointsOut2D);
        }

        public void GetFlatedPlateCoordinates(out float fMax_X, out float fMin_X, out float fMax_Y, out float fMin_Y)
        {
            fMax_X = float.MinValue;
            fMin_X = float.MaxValue;
            fMax_Y = float.MinValue;
            fMin_Y = float.MaxValue;

            // Take maximum / minimum coordinate

            if (PointsOut2D != null && PointsOut2D.Length > 2) // Some points exist - pre urcenie rozmeru minimalne 3 body
            {
                foreach (Point p in PointsOut2D)
                {
                    fMax_X = Math.Max(fMax_X, (float)p.X);
                    fMin_X = Math.Min(fMin_X, (float)p.X);
                    fMax_Y = Math.Max(fMax_Y, (float)p.Y);
                    fMin_Y = Math.Min(fMin_Y, (float)p.Y);
                }
            }
        }

        public void SetFlatedPlateDimensions()
        {
            // Get edge coordinates
            float fMax_X = 0, fMin_X = 0, fMax_Y = 0, fMin_Y = 0;
            GetFlatedPlateCoordinates(out fMax_X, out fMin_X, out fMax_Y, out fMin_Y);

            // Calculation dimensions of plate
            fWidth_bx_Stretched = fMax_X - fMin_X;
            fHeight_hy_Stretched = fMax_Y - fMin_Y;
        }

        public float GetMassIgnoringHoles()
        {
            if (m_Mat == null) return -1;

            return m_Mat.m_fRho * GetVolumeIgnoringHoles();
        }

        // Return the polygon's area in "square units."
        // The value will be negative if the polygon is
        // oriented clockwise.

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
        public virtual void MirrorPlateAboutX()
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
        public virtual void MirrorPlateAboutY()
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
        public virtual void RotatePlateAboutZ_CW(float fTheta_deg)
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

        public bool IsSymmetric() // TODO - zistit naco sa to pouziva, ci je to spravne a ci to nema byt v CPlate_Frame pre KNEE a APEX plates
        {
            if (this.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_K)
            {
                if (this is CConCom_Plate_KA) return true;
                if (this is CConCom_Plate_KK) return true;

                return false;
            }
            return true;
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

        public void SetParams(string plateDatabaseName, ESerieTypePlate plateDatabaseSerie)
        {
            // Tato funckia by mala nastavit parametre plate z databazy, mohla by byt override podla jednotlivych parametrov plate

            switch (plateDatabaseSerie)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        CPlate_B_Properties prop = new CPlate_B_Properties();
                        prop = CJointsManager.GetPlate_B_Properties(plateDatabaseName);

                        // Doriesit identicku geometriu v modeli a v database a presnost
                        //fWidth_bx = (float)prop.TotalDim_x; // Rozvinuta sirka - horizontalny smer
                        //fHeight_hy = (float)prop.TotalDim_y; // Rozvinuta vyska - vertikalny smer
                        //m_ft = (float)prop.t; // Thickness
                        //fArea = (float)prop.Area;

                        // Doplnit do databazy obvod a povrch
                        //fVolume = (float)prop.Volume;
                        //fMass = (float)prop.Mass;

                        if (prop != null) Price_PPKG_NZD = (float)prop.Price_PPKG_NZD;
                        break;
                    }
                case ESerieTypePlate.eSerie_L:
                    {
                        CPlate_L_Properties prop = new CPlate_L_Properties();
                        prop = CJointsManager.GetPlate_L_Properties(plateDatabaseName);

                        // Doriesit identicku geometriu v modeli a v database a presnost
                        //fWidth_bx = (float)prop.TotalDim_x; // Rozvinuta sirka - horizontalny smer
                        //fHeight_hy = (float)prop.TotalDim_y; // Rozvinuta vyska - vertikalny smer
                        //m_ft = (float)prop.thickness; // Thickness
                        //fArea = (float)prop.Area;

                        // Doplnit do databazy obvod a povrch
                        //fVolume = (float)prop.Volume;
                        //fMass = (float)prop.Mass;

                         if(prop != null) Price_PPKG_NZD = (float)prop.Price_PPKG_NZD;
                        break;
                    }
                case ESerieTypePlate.eSerie_F:
                    {
                        CPlate_F_Properties prop = new CPlate_F_Properties();
                        prop = CJointsManager.GetPlate_F_Properties(plateDatabaseName);

                        // Doriesit identicku geometriu v modeli a v database a presnost
                        //fWidth_bx = (float)prop.TotalDim_x; // Rozvinuta sirka - horizontalny smer
                        //fHeight_hy = (float)prop.TotalDim_y; // Rozvinuta vyska - vertikalny smer
                        //m_ft = (float)prop.thickness; // Thickness
                        //fArea = (float)prop.Area;

                        // Doplnit do databazy obvod a povrch
                        //fVolume = (float)prop.Volume;
                        //fMass = (float)prop.Mass;

                        if (prop != null) Price_PPKG_NZD = (float)prop.Price_PPKG_NZD;
                        break;
                    }
                case ESerieTypePlate.eSerie_LL:
                    {
                        CPlate_LL_Properties prop = new CPlate_LL_Properties();
                        prop = CJointsManager.GetPlate_LL_Properties(plateDatabaseName);

                        // Doriesit identicku geometriu v modeli a v database a presnost
                        //fWidth_bx = (float)prop.TotalDim_x / 1000; // Rozvinuta sirka - horizontalny smer
                        //fHeight_hy = (float)prop.TotalDim_y / 1000; // Rozvinuta vyska - vertikalny smer
                        //m_ft = (float)prop.thickness / 1000; // Thickness
                        //fArea = (float)prop.Area / 1000000;

                        // Doplnit do databazy obvod a povrch
                        //fVolume = (float)prop.Volume / 1000000000;
                        //fMass = (float)prop.Mass;

                        if (prop != null) Price_PPKG_NZD = (float)prop.Price_PPKG_NZD;
                        break;
                    }
                case ESerieTypePlate.eSerie_W:
                    {
                        CRectWasher_W_Properties prop = new CRectWasher_W_Properties();
                        prop = CWashersManager.GetPlate_W_Properties(plateDatabaseName);

                        // Doriesit identicku geometriu v modeli a v database a presnost
                        //fWidth_bx = (float)prop.TotalDim_x / 1000; // Rozvinuta sirka - horizontalny smer
                        //fHeight_hy = (float)prop.TotalDim_y / 1000; // Rozvinuta vyska - vertikalny smer
                        //m_ft = (float)prop.thickness / 1000; // Thickness
                        //fArea = (float)prop.Area / 1000000;

                        // Doplnit do databazy obvod a povrch
                        //fVolume = (float)prop.Volume / 1000000000;
                        //fMass = (float)prop.Mass;

                        if (prop != null) Price_PPKG_NZD = (float)prop.Price_PPKG_NZD;
                        break;
                    }
                default:
                    {
                        // Doimplementovat
                        break;
                    }
            }
        }

        // Set plate 3D object rotations in degrees 
        public void SetPlateRotation(Vector3D rotations_XYZ)
        {
            m_fRotationX_deg = (float)rotations_XYZ.X;
            m_fRotationY_deg = (float)rotations_XYZ.Y;
            m_fRotationZ_deg = (float)rotations_XYZ.Z;
        }

        public virtual void CopyParams(CPlate plate)
        {
            // TO Ondrej - tu som nakopiroval vsetko co som nasiel, mozno su niektore z tych parametrov zbytocne tak to potom este zakomentujeme
            this.Width_bx = plate.Width_bx;
            this.Height_hy = plate.Height_hy;
            this.fWidth_bx_Stretched = plate.fWidth_bx_Stretched;
            this.Height_hy_Stretched = plate.Height_hy_Stretched;
            this.Ft = plate.Ft;
            this.fArea = plate.fArea;
            this.fA_g = plate.fA_g;
            this.fA_n = plate.fA_n;
            this.fA_v_zv = plate.fA_v_zv;
            this.fA_vn_zv = plate.fA_vn_zv;
            this.fI_yu = plate.fI_yu;
            this.fW_el_yu = plate.fW_el_yu;
            this.fCuttingRouteDistance = plate.fCuttingRouteDistance;
            this.fSurface = plate.fSurface;
            this.fVolume = plate.fVolume;
            this.fMass = plate.fMass;
            //this.m_fRotationX_deg = plate.m_fRotationX_deg; // Neviem ci toto treba kopirovat
            //this.m_fRotationY_deg = plate.m_fRotationY_deg;
            //this.m_fRotationZ_deg = plate.m_fRotationZ_deg;
            this.ITotNoPointsin3D = plate.ITotNoPointsin3D;
            this.ITotNoPointsin2D = plate.ITotNoPointsin2D;
            this.m_drillingRoutePoints = plate.m_drillingRoutePoints;
            this.m_dimensions = plate.m_dimensions;
            this.m_MemberOutlines = plate.m_MemberOutlines;
            this.m_BendLines = plate.m_BendLines;
            this.arrConnectorControlPoints3D = plate.arrConnectorControlPoints3D;
            this.INoPoints2Dfor3D = plate.INoPoints2Dfor3D;
            this.m_fs_f_min = plate.m_fs_f_min;
            this.m_fs_f_max = plate.m_fs_f_max;
            this.m_iNumberOfConnectorsInSection = plate.m_iNumberOfConnectorsInSection;
            this.ScrewArrangement = plate.ScrewArrangement.GetClonedScrewArrangement();
        }

        public double GetDimensionsMaxOffset()
        {
            double maxOffset = 0;
            if (Dimensions == null) return maxOffset;
            double textSize = 10;
            foreach (CDimension d in Dimensions)
            {
                if (!(d is CDimensionLinear)) continue;
                CDimensionLinear ld = d as CDimensionLinear;
                double offset = 0;

                //berie sa do uvahy iba ak je kota smerom von a pokial je text von tak este plus velkost textu
                if (ld.IsDimensionOutSide)
                {
                    offset = ld.OffsetFromOrigin_pxs;
                    if (ld.IsTextOutSide) offset += textSize;
                } 
                if (offset > maxOffset) maxOffset = offset;
            }
            return maxOffset;
        }
    }
}
