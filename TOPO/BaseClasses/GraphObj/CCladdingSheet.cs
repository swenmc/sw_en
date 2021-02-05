using BaseClasses.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CCladdingSheet : CEntity3D
    {
        private Point3D m_ControlPoint;

        double claddingWidthRibModular = 0.190; // m // z databazy cladding MDBTrapezoidalSheeting widthRib_m

        string m_claddingShape;
        string m_claddingCoatingType;
        string m_ColorName;
        Color m_Color;
        float m_fOpacity;

        int m_iNumberOfEdges;
        float m_fWidthModular; // TODO // m // z databazy cladding MDBTrapezoidalSheeting
        double m_dLengthTotal;
        double m_dLengthTopRight;
        double m_tipCoordinate_x;
        double m_dLengthTopTip;
        double m_dLengthTopLeft;

        float m_fCoordinateInPlane_x;
        float m_fCoordinateInPlane_y;

        public Point3D ControlPoint
        {
            get
            {
                return m_ControlPoint;
            }

            set
            {
                m_ControlPoint = value;
            }
        }

        public CCladdingSheet()
        {

        }

        public CCladdingSheet(int iCladdingSheet_ID, int numberOfCorners,
        float coordinateInPlane_x, float coordinateInPlane_y, Point3D controlPoint_GCS,
        float widthModular, double lengthTopLeft, double lengthTopRight, double tipCoordinate_x, double lengthTopTip,
        string colorName, string claddingShape, string claddingCoatingType,
        Color color, float opacity, double claddingWidthRib, bool bIsDisplayed, int fTime)
        {
            ID = iCladdingSheet_ID;
            m_iNumberOfEdges = numberOfCorners;
            m_fCoordinateInPlane_x = coordinateInPlane_x;
            m_fCoordinateInPlane_y = coordinateInPlane_y;
            m_ControlPoint = controlPoint_GCS;
            m_fWidthModular = widthModular;
            m_dLengthTopLeft = lengthTopLeft;
            m_dLengthTopRight = lengthTopRight;
            m_tipCoordinate_x = tipCoordinate_x;
            m_dLengthTopTip = lengthTopTip;
            m_ColorName = colorName;
            m_claddingShape = claddingShape;
            m_claddingCoatingType = claddingCoatingType;
            m_Color = color;
            m_fOpacity = opacity;
            claddingWidthRibModular = claddingWidthRib;
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

            if (numberOfCorners == 4)
                m_dLengthTotal = Math.Max(m_dLengthTopLeft, m_dLengthTopRight);
            else
                m_dLengthTotal = Math.Max(Math.Max(m_dLengthTopLeft, m_dLengthTopRight), m_dLengthTopTip);
        }

        public GeometryModel3D GetCladdingSheetModel(DisplayOptions options)
        {
            Point3D pfront0_baseleft = new Point3D(0,0,0);
            Point3D pfront1_baseright = new Point3D(m_fWidthModular, 0,0);
            Point3D pfront2_topright = new Point3D(m_fWidthModular,0, m_dLengthTopRight);
            Point3D pfront3_toptip = new Point3D(m_tipCoordinate_x, 0, m_dLengthTopTip);// TODO - dopracovat moznost zadania presnej suradnice hornej spicky
            Point3D pfront4_topleft = new Point3D(0, 0, m_dLengthTopLeft);

            Brush solidBrush = new SolidColorBrush(m_Color);
            solidBrush.Opacity = m_fOpacity;
            DiffuseMaterial material = new DiffuseMaterial(solidBrush);

            return new CAreaPolygonal(ID, new List<Point3D>() { pfront0_baseleft, pfront1_baseright, pfront2_topright, pfront3_toptip, pfront4_topleft }, 0).CreateArea(options.bUseTextures, material);
        }

        public Transform3DGroup GetTransformGroup(float fRot_X_deg, float fRot_Y_deg, float fRot_Z_deg)
        {
            // Transformacie
            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            // About X
            AxisAngleRotation3D axisAngleRotation3dX = new AxisAngleRotation3D();
            axisAngleRotation3dX.Axis = new Vector3D(1, 0, 0);
            axisAngleRotation3dX.Angle = fRot_X_deg;
            rotateX.Rotation = axisAngleRotation3dX;

            // About Y
            AxisAngleRotation3D axisAngleRotation3dY = new AxisAngleRotation3D();
            axisAngleRotation3dY.Axis = new Vector3D(0, 1, 0);
            axisAngleRotation3dY.Angle = fRot_Y_deg;
            rotateY.Rotation = axisAngleRotation3dY;

            // About Z
            AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
            axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
            axisAngleRotation3dZ.Angle = fRot_Z_deg;
            rotateZ.Rotation = axisAngleRotation3dZ;

            TranslateTransform3D translateOrigin = new TranslateTransform3D(m_ControlPoint.X, m_ControlPoint.Y, m_ControlPoint.Z);

            Transform3DGroup TransformGr = new Transform3DGroup();
            TransformGr.Children.Add(rotateX);
            TransformGr.Children.Add(rotateY);
            TransformGr.Children.Add(rotateZ);
            TransformGr.Children.Add(translateOrigin); // Presun v ramci GCS

            return TransformGr;
        }
    }
}
