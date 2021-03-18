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

        private Point3D m_PointText;
        //private string m_Text;

        private List<Point3D> m_WireFramePoints;
        private double m_RotationX;
        private double m_rotationY;
        private double m_rotationZ;

        public List<Point3D> WireFramePoints
        {
            get
            {
                if (m_WireFramePoints == null) m_WireFramePoints = new List<Point3D>();
                return m_WireFramePoints;
            }

            set
            {
                m_WireFramePoints = value;
            }
        }



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

        public Point3D PointText
        {
            get { return m_PointText; }
            set { m_PointText = value; }
        }

        //public string Text
        //{
        //    get { return m_Text; }
        //    set { m_Text = value; }
        //}

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

        public int iVectorOverFactor_LCS;
        public int iVectorUpFactor_LCS;

        public CCladdingOrFibreGlassSheet()
        {

        }

        public CCladdingOrFibreGlassSheet(int iCladdingSheet_ID, int numberOfCorners,
        double coordinateInPlane_x, double coordinateInPlane_y, Point3D controlPoint_GCS,
        double width, double lengthTopLeft, double lengthTopRight, double tipCoordinate_x, double lengthTopTip,
        string colorName, string claddingShape, string claddingCoatingType,
        Color color, float opacity, double claddingWidthRib, bool bIsDisplayed, float fTime)
        {
            ID = iCladdingSheet_ID;
            //To Mato - 760, Prefix je null treba ho nejako inicializovat
            Prefix = $"WC {ID}";

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

            //toto nie je podla mna spravne miesto - zmazat
            //m_Text = "WC" + ID.ToString(); // TODO - dopracovat texty podla nastaveni v GUI - Display Options, zaviest text popisu ako vstupny parameter objektu

            SetTextPointInLCS(); // Text v LCS
        }

        // TO Ondrej - vieme nejako krajsie pracovat s potomkami jednej triedy, aby sme ich mohli vzajomne pretypovat
        public COpening ConvertToOpening()
        {
            return new COpening(ID, NumberOfEdges, CoordinateInPlane_x, CoordinateInPlane_y, ControlPoint,
                Width, LengthTopLeft, LengthTopRight, TipCoordinate_x, LengthTopTip, BIsDisplayed, FTime);
        }

        public GeometryModel3D GetCladdingSheetModel(DisplayOptions options, DiffuseMaterial material, bool createWireframe, double outOffPlaneOffset_y = 0)
        {
            Point3D pfront0_baseleft = new Point3D(0, outOffPlaneOffset_y, 0);
            Point3D pfront1_baseright = new Point3D(Width, outOffPlaneOffset_y, 0);
            Point3D pfront2_topright = new Point3D(Width, outOffPlaneOffset_y, LengthTopRight);
            Point3D pfront3_toptip = new Point3D(TipCoordinate_x, outOffPlaneOffset_y, LengthTopTip);// TODO - dopracovat moznost zadania presnej suradnice hornej spicky
            Point3D pfront4_topleft = new Point3D(0, outOffPlaneOffset_y, LengthTopLeft);

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

            if (NumberOfEdges == 4)
            {
                CAreaPolygonal area = new CAreaPolygonal(ID, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_topright, pfront4_topleft }, 0);
                if(createWireframe) WireFramePoints.AddRange(area.GetWireFrame());
                return area.CreateArea(options.bUseTextures, material);
            }
            else
            {
                CAreaPolygonal area = new CAreaPolygonal(ID, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_topright, pfront3_toptip, pfront4_topleft }, 0);
                if (createWireframe) WireFramePoints.AddRange(area.GetWireFrame());
                return area.CreateArea(options.bUseTextures, material);
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

            float fOffsetFromPlane = 0.005f; // Offset nad/ pred urovnou panela, aby sa text nevnoril do 3D reprezentacie

            m_PointText = new Point3D()
            {
                X = 0.3 * Width, // Kreslime v 30% sirky zlava
                Y = 0.4 * LengthTotal, // Kreslime v 40% dlzky zdola
                Z = fOffsetFromPlane
            };
        }
    }
}
