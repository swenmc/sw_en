using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CCladdingOrFibreGlassSheet : CSurfaceComponent
    {
        double m_dCladdingWidthRibModular; // m // z databazy cladding MDBTrapezoidalSheeting widthRib_m

        string m_claddingShape;
        string m_claddingCoatingType;
        string m_ColorName;
        Color m_Color;
        float m_fOpacity;

        private float m_ft; // Thickness

        private float m_fSurface_netto;
        private float m_fSurface_brutto;
        private float m_fVolume_netto;
        private float m_fVolume_brutto;
        private float m_fMass_netto;
        private float m_fMass_brutto;

        private double _price_PPKG_NZD;
        private double _price_PPP_NZD_netto;
        private double _price_PPP_NZD_brutto;

        private double m_RotationX;
        private double m_rotationY;
        private double m_rotationZ;

        private bool m_IsCanopy;
        private double m_WpWidthOffset;

        private double m_Width_flat;



        private double m_BasicModularWidth;
        private double m_CoilWidth;
        private double m_CoilMass_kg_m2;
        private double m_CoilPrice_PPSM_NZD;
        

        public double CladdingWidthRibModular
        {
            get
            {
                return m_dCladdingWidthRibModular;
            }

            set
            {
                m_dCladdingWidthRibModular = value;
            }
        }

        public string CladdingShape
        {
            get
            {
                return m_claddingShape;
            }

            set
            {
                m_claddingShape = value;
            }
        }

        public string CladdingCoatingType
        {
            get
            {
                return m_claddingCoatingType;
            }

            set
            {
                m_claddingCoatingType = value;
            }
        }

        public string ColorName
        {
            get
            {
                return m_ColorName;
            }

            set
            {
                m_ColorName = value;
            }
        }

        public Color Color
        {
            get
            {
                return m_Color;
            }

            set
            {
                m_Color = value;
            }
        }

        public float Opacity
        {
            get
            {
                return m_fOpacity;
            }

            set
            {
                m_fOpacity = value;
            }
        }

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

        public float Surface_netto
        {
            get
            {
                return m_fSurface_netto;
            }

            set
            {
                m_fSurface_netto = value;
            }
        }

        public float Surface_brutto
        {
            get
            {
                return m_fSurface_brutto;
            }

            set
            {
                m_fSurface_brutto = value;
            }
        }

        public float Volume_netto
        {
            get
            {
                return m_fVolume_netto;
            }

            set
            {
                m_fVolume_netto = value;
            }
        }

        public float Volume_brutto
        {
            get
            {
                return m_fVolume_brutto;
            }

            set
            {
                m_fVolume_brutto = value;
            }
        }

        public float Mass_netto
        {
            get
            {
                return m_fMass_netto;
            }

            set
            {
                m_fMass_netto = value;
            }
        }

        public float Mass_brutto
        {
            get
            {
                return m_fMass_brutto;
            }

            set
            {
                m_fMass_brutto = value;
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

        public double Price_PPP_NZD_netto
        {
            get
            {
                return _price_PPP_NZD_netto;
            }

            set
            {
                _price_PPP_NZD_netto = value;
            }
        }

        public double Price_PPP_NZD_brutto
        {
            get
            {
                return _price_PPP_NZD_brutto;
            }

            set
            {
                _price_PPP_NZD_brutto = value;
            }
        }

        public double RotationX
        {
            get
            {
                return m_RotationX;
            }

            set
            {
                m_RotationX = value;
            }
        }

        public double RotationY
        {
            get
            {
                return m_rotationY;
            }

            set
            {
                m_rotationY = value;
            }
        }

        public double RotationZ
        {
            get
            {
                return m_rotationZ;
            }

            set
            {
                m_rotationZ = value;
            }
        }

        public bool IsFibreglass
        {
            get
            {
                if (Prefix == "RF") return true;
                else if (Prefix.StartsWith("WF")) return true;
                else return false;
            }
        }
        public bool IsWalllFibreglass
        {
            get
            {                
                if(Prefix.StartsWith("WF")) return true;
                else return false;
            }
        }
        public bool IsRoofFibreglass
        {
            get
            {
                if(Prefix.StartsWith("RF")) return true;
                else return false;
            }
        }
        public bool IsRoofCladding
        {
            get
            {
                if (Prefix.StartsWith("RC")) return true;
                else return false;
            }
        }
        public bool IsWallCladding
        {
            get
            {
                if (Prefix.StartsWith("WC")) return true;
                else return false;
            }
        }

        public bool IsCanopy
        {
            get
            {
                return m_IsCanopy;
            }

            set
            {
                m_IsCanopy = value;
            }
        }

        public double WpWidthOffset
        {
            get
            {
                return m_WpWidthOffset;
            }

            set
            {
                m_WpWidthOffset = value;
            }
        }

        public double Width_flat
        {
            get
            {
                return m_Width_flat;
            }

            set
            {
                m_Width_flat = value;
            }
        }

        public int iVectorOverFactor_LCS;
        public int iVectorUpFactor_LCS;

        public CCladdingOrFibreGlassSheet()
        {

        }

        public CCladdingOrFibreGlassSheet(int iCladdingSheet_ID, string prefix, string name, MATERIAL.CMat claddingMaterial,
        double thickness, double coilWidth, double coilMass_kg_m2, double coilPrice_PPSM_NZD,
        double basicModularWidth,
        int numberOfCorners, double coordinateInPlane_x, double coordinateInPlane_y, Point3D controlPoint_GCS,
        double width, double lengthTopLeft, double lengthTopRight, double tipCoordinate_x, double lengthTopTip,
        string colorName, string claddingShape, string claddingCoatingType,
        Color color, float opacity, double claddingWidthRib, bool bIsDisplayed, float fTime, bool isCanopy = false, double wpWidthOffset = 0)
        {
            ID = iCladdingSheet_ID;
            Prefix = prefix;
            Name = name;
            m_Mat = claddingMaterial; // Vseobecny material lebo FG nemusi byt steel

            Ft = (float)thickness;

            NumberOfEdges = numberOfCorners;
            CoordinateInPlane_x = coordinateInPlane_x;
            CoordinateInPlane_y = coordinateInPlane_y;
            ControlPoint = controlPoint_GCS;
            Width = width;
            LengthTopLeft = lengthTopLeft;
            LengthTopRight = lengthTopRight;
            TipCoordinate_x = tipCoordinate_x;
            LengthTopTip = lengthTopTip;
            m_ColorName = colorName;
            m_claddingShape = claddingShape;
            m_claddingCoatingType = claddingCoatingType;
            m_Color = color;
            m_fOpacity = opacity;
            m_dCladdingWidthRibModular = claddingWidthRib;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            IsCanopy = isCanopy;
            WpWidthOffset = wpWidthOffset;

            m_BasicModularWidth = basicModularWidth;
            m_CoilWidth = coilWidth;
            m_CoilMass_kg_m2 = coilMass_kg_m2;
            m_CoilPrice_PPSM_NZD = coilPrice_PPSM_NZD;

            // 5 edges
            //   3 ____ 2
            //    /    |
            // 4 /     |
            //  |      |
            //  |      |
            //  |______|
            // 0        1

            // 4 edges
            // 3 ______ 2
            //  |      |
            //  |      |
            //  |      |
            //  |      |
            //  |______|
            // 0        1
            //
            //
            //  y
            // /|\
            //  |___\ x
            //      /

            if (NumberOfEdges == 4)
                LengthTotal = Math.Max(LengthTopLeft, LengthTopRight);
            else
                LengthTotal = Math.Max(Math.Max(LengthTopLeft, LengthTopRight), LengthTopTip);


            Update();

            //Area_brutto = Width * LengthTotal;

            //Width_flat = width / basicModularWidth * coilWidth;
            //Surface_brutto = (float)(Width_flat * LengthTotal);
            //Volume_brutto = (float)(Surface_brutto * Ft);
            //Mass_brutto = (float)(coilMass_kg_m2 * Surface_brutto);
            //Price_PPKG_NZD = coilPrice_PPSM_NZD / coilMass_kg_m2;
            //Price_PPP_NZD_brutto = Price_PPKG_NZD * Mass_brutto;

            //SetTextPointInLCS(); // Text v LCS
        }

        public void Update()
        {
            Area_brutto = Width * LengthTotal;

            Width_flat = Width / m_BasicModularWidth * m_CoilWidth;
            Surface_brutto = (float)(Width_flat * LengthTotal);
            Volume_brutto = (float)(Surface_brutto * Ft);
            Mass_brutto = (float)(m_CoilMass_kg_m2 * Surface_brutto);
            Price_PPKG_NZD = m_CoilPrice_PPSM_NZD / m_CoilMass_kg_m2;
            Price_PPP_NZD_brutto = Price_PPKG_NZD * Mass_brutto;

            SetTextPointInLCS(); // Text v LCS

        }

        // TO Ondrej - vieme nejako krajsie pracovat s potomkami jednej triedy, aby sme ich mohli vzajomne pretypovat
        // To Mato - toto je uplna blbost, lebo potomok je aj predok,cize nemusis robit vobec nic,ale to musi asi tato trieda priamo dedit od COpening 
        // taketo veci si musime prekonzultovat,az potom to viem zrefaktorovat
        public COpening ConvertToOpening()
        {
            return new COpening(ID, NumberOfEdges, CoordinateInPlane_x, CoordinateInPlane_y, ControlPoint,
                Width, LengthTopLeft, LengthTopRight, TipCoordinate_x, LengthTopTip, BIsDisplayed, FTime);
        }

        public GeometryModel3D GetCladdingSheetModel(DisplayOptions options, DiffuseMaterial material, double outOffPlaneOffset_y = 0)
        {
            OutOffPlaneOffset_y = outOffPlaneOffset_y; // Nastavime offset z roviny

            Point3D pfront0_baseleft = new Point3D(0, OutOffPlaneOffset_y, 0);
            Point3D pfront1_baseright = new Point3D(Width, OutOffPlaneOffset_y, 0);
            Point3D pfront2_topright = new Point3D(Width, OutOffPlaneOffset_y, LengthTopRight);
            Point3D pfront3_toptip = new Point3D(TipCoordinate_x, OutOffPlaneOffset_y, LengthTopTip);// TODO - dopracovat moznost zadania presnej suradnice hornej spicky
            Point3D pfront4_topleft = new Point3D(0, OutOffPlaneOffset_y, LengthTopLeft);

            // Local in-plane offset
            pfront0_baseleft.X += CoordinateInPlane_x;
            pfront0_baseleft.Z += CoordinateInPlane_y;

            pfront1_baseright.X += CoordinateInPlane_x;
            pfront1_baseright.Z += CoordinateInPlane_y;

            pfront2_topright.X += CoordinateInPlane_x;
            pfront2_topright.Z += CoordinateInPlane_y;

            pfront3_toptip.X += CoordinateInPlane_x;
            pfront3_toptip.Z += CoordinateInPlane_y;

            pfront4_topleft.X += CoordinateInPlane_x;
            pfront4_topleft.Z += CoordinateInPlane_y;

            if (options.bCladdingSheetColoursByID && !options.bUseTextures) // Ak je zapnuta textura, tak je nadradena solid brush farbam
            {
                // TODO Ondrej - pouzije sa farba priradena objektu
                // Chcelo by to nejako vylepsit, aby sme tu farbu a material nastavovali len raz a nemuselo sa to tu prepisovat
                Brush solidBrush = new SolidColorBrush(m_Color);
                solidBrush.Opacity = m_fOpacity;
                material = new DiffuseMaterial(solidBrush);
            }

            // TODO 783 - Ondrej
            BIsDebugging = true; // Docasne

            if (BIsDebugging)
            {
                // Testovanie
                // TODO 783 - Ondrej
                // Pre tie sheets ktore maju sadu real lengths rovnaku ako lengths nastavime specificku farbu
                // Malo by sa tym dat vizualne skontrolovat, ktore sheet maju pripocitany overlap a ktore nie

                if (MATH.MathF.d_equal(LengthTotal, LengthTotal_Real))
                    material = new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)); // Nastavime zltu farbu
            }

            if (NumberOfEdges == 4)
            {
                EdgePoints2D = new List<System.Windows.Point>
                {
                new System.Windows.Point(0, 0),
                new System.Windows.Point(Width, 0),
                new System.Windows.Point(Width, LengthTopRight),
                new System.Windows.Point(0, LengthTopLeft)
                };

                Area_netto = MATH.Geom2D.PolygonArea(EdgePoints2D.ToArray());
                Surface_netto = (float)(Width_flat / Width * Area_netto);
                Volume_netto = Surface_netto * Ft;
                Mass_netto = Volume_netto * GlobalConstants.MATERIAL_DENSITY_STEEL;
                Price_PPP_NZD_netto = Price_PPKG_NZD * Mass_netto;

                EdgePointList = new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_topright, pfront4_topleft };
                return CreateArea(options.bUseTextures, material);
            }
            else
            {
                EdgePoints2D = new List<System.Windows.Point>
                {
                new System.Windows.Point(0,0),
                new System.Windows.Point(Width, 0),
                new System.Windows.Point(Width,LengthTopRight),
                new System.Windows.Point(TipCoordinate_x, LengthTopTip),
                new System.Windows.Point(0, LengthTopLeft)
                };

                Area_netto = MATH.Geom2D.PolygonArea(EdgePoints2D.ToArray());
                Surface_netto = (float)(Width_flat / Width * Area_netto);
                Volume_netto = Surface_netto * Ft;
                Mass_netto = Volume_netto * GlobalConstants.MATERIAL_DENSITY_STEEL;
                Price_PPP_NZD_netto = Price_PPKG_NZD * Mass_netto;

                EdgePointList = new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_topright, pfront3_toptip, pfront4_topleft };
                return CreateArea(options.bUseTextures, material);
            }
        }

        public Transform3DGroup GetTransformGroup(/*double dRot_X_deg, double dRot_Y_deg, double dRot_Z_deg*/)
        {
            // Transformacie
            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            // About X
            AxisAngleRotation3D axisAngleRotation3dX = new AxisAngleRotation3D();
            axisAngleRotation3dX.Axis = new Vector3D(1, 0, 0);
            axisAngleRotation3dX.Angle = RotationX; //dRot_X_deg;
            rotateX.Rotation = axisAngleRotation3dX;

            // About Y
            AxisAngleRotation3D axisAngleRotation3dY = new AxisAngleRotation3D();
            axisAngleRotation3dY.Axis = new Vector3D(0, 1, 0);
            axisAngleRotation3dY.Angle = RotationY; //dRot_Y_deg;
            rotateY.Rotation = axisAngleRotation3dY;

            // About Z
            AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
            axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
            axisAngleRotation3dZ.Angle = RotationZ; //dRot_Z_deg;
            rotateZ.Rotation = axisAngleRotation3dZ;

            TranslateTransform3D translateOrigin = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            Transform3DGroup TransformGr = new Transform3DGroup();
            TransformGr.Children.Add(rotateX);
            TransformGr.Children.Add(rotateY);
            TransformGr.Children.Add(rotateZ);
            TransformGr.Children.Add(translateOrigin); // Presun v ramci GCS

            return TransformGr;
        }

        public void SetTextPointInLCS()
        {
            iVectorOverFactor_LCS = 1;
            iVectorUpFactor_LCS = 1;

            float fOffsetFromPlane = -0.010f; // Offset nad / pred urovnou panela, aby sa text nevnoril do 3D reprezentacie

            //PointText = new Point3D(0, 0, 0);
            PointText = new Point3D()
            {
                X = 0.5 * Width, // Kreslime v 30% sirky zlava
                Y = /*OutOffPlaneOffset_y +*/ fOffsetFromPlane,
                Z = 0.4 * LengthTotal // Kreslime v 40% dlzky zdola
            };

            // Posun v LCS v ramci side
            PointText.X += CoordinateInPlane_x;
            PointText.Z += CoordinateInPlane_y;
        }
    }
}
