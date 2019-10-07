using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CDetailSymbol
    {
        private Point3D m_ControlPoint;
        private Point3D m_PointLineStart_LCS;
        private Point3D m_PointLineEnd_LCS;
        private Point3D m_PointLabelText;
        private Vector3D m_Direction;
        private string m_LabelText;
        private double m_MarkObjectSize; // Square side or circle diameter
        private double m_LineLength;
        private float m_LineCylinderRadius;

        private ELinePatternType m_LinePatternType;

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

        public Point3D PointLineStart_LCS
        {
            get
            {
                return m_PointLineStart_LCS;
            }

            set
            {
                m_PointLineStart_LCS = value;
            }
        }

        public Point3D PointLineEnd_LCS
        {
            get
            {
                return m_PointLineEnd_LCS;
            }

            set
            {
                m_PointLineEnd_LCS = value;
            }
        }

        public Point3D PointLabelText
        {
            get
            {
                return m_PointLabelText;
            }

            set
            {
                m_PointLabelText = value;
            }
        }

        public Vector3D Direction
        {
            get
            {
                return m_Direction;
            }

            set
            {
                m_Direction = value;
            }
        }

        public string LabelText
        {
            get
            {
                return m_LabelText;
            }

            set
            {
                m_LabelText = value;
            }
        }

        public double MarkObjectSize
        {
            get
            {
                return m_MarkObjectSize;
            }

            set
            {
                m_MarkObjectSize = value;
            }
        }

        public double LineLength
        {
            get
            {
                return m_LineLength;
            }

            set
            {
                m_LineLength = value;
            }
        }

        public float LineCylinderRadius
        {
            get
            {
                return m_LineCylinderRadius;
            }

            set
            {
                m_LineCylinderRadius = value;
            }
        }

        public ELinePatternType LinePatternType
        {
            get
            {
                return m_LinePatternType;
            }

            set
            {
                m_LinePatternType = value;
            }
        }

        public Transform3DGroup TransformGr;

        public CDetailSymbol(
        Point3D controlPoint,
        //Point3D pointLineStart,
        //Point3D pointLineEnd,
        //Point3D pointLabelText,
        Vector3D direction,
        string labelText,
        double markObjectSize,
        double lineLength,
        ELinePatternType linePatternType
        )
        {
            m_ControlPoint = controlPoint;
            //m_PointLineStart_LCS = pointLineStart;
            //m_PointLineEnd_LCS = pointLineEnd;
            //m_PointLabelText_LCS = pointLabelText;
            m_Direction = direction;
            m_LabelText = labelText;
            m_MarkObjectSize = markObjectSize;
            m_LineLength = lineLength;
            m_LinePatternType = linePatternType;

            // TODO napojit - polomer valca
            //m_LineCylinderRadius = 0.005f;
            m_LineCylinderRadius = (float)markObjectSize / 50;

            m_PointLineStart_LCS = new Point3D((float)m_MarkObjectSize / 2f + m_LineCylinderRadius, 0, 0);
            m_PointLineEnd_LCS = new Point3D((float)m_MarkObjectSize / 2f + m_LineCylinderRadius + m_LineLength, 0, 0);

            float offset_x = 0.4f; // Umoznit Nastavovat podla velkosti text label ???
            float offset_y = -0.01f; // Default 0.0 (stred textu na stred ciary)
            //m_PointLabelText = new Point3D(m_PointLineEnd_LCS.X + offset_x, 0 + offset_y, -m_LineCylinderRadius);
            m_PointLabelText = new Point3D(0,0,0);
        }

        public Model3DGroup GetDetailSymbolModel(System.Windows.Media.Color color, bool drawLine)
        {
            Model3DGroup model_gr = new Model3DGroup();

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color)); // TODO Ondrej - nastavitelna hrubka a farba pre zobrazenie v GUI a pre Export

            // Vytvorime model v LCS [0,0,0] je uvazovana v bode m_ControlPoint, line smeruje v kladnom smere x a znacka s label text je v rovine xy

            // Mark (circle or square, rhombus)
            //short NumberOfCirclePointsMark = 32 + 1;//32 + 1;
            short NumberOfCirclePointsMark = 128 + 1;//32 + 1;
            model_gr.Children.Add(new CHollowCylinder(NumberOfCirclePointsMark, (float)m_MarkObjectSize / 2f - m_LineCylinderRadius, (float)m_MarkObjectSize / 2f + m_LineCylinderRadius, 2 * m_LineCylinderRadius, material).CreateM_G_M_3D_Volume(new Point3D(0,0, -m_LineCylinderRadius), (float)m_MarkObjectSize / 2f - m_LineCylinderRadius, (float)m_MarkObjectSize / 2f + m_LineCylinderRadius, 2 * m_LineCylinderRadius, material, 2));

            if (drawLine)
            {
                // Line
                short NumberOfCirclePointsLine = 8 + 1;//8 + 1;

                if (m_LinePatternType == ELinePatternType.CONTINUOUS) // Ak je continuous tak nepouzijeme CLine
                    model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(m_PointLineStart_LCS, NumberOfCirclePointsLine, m_LineCylinderRadius, (float)m_LineLength, material, 0, false, false));
                else // Iny typ ciary
                {
                    // dashed, dotted, divide, ....

                    // Vytvorime liniu zacinajucu v start point v smere x s celkovou dlzkou
                    CLine line = new CLine(m_LinePatternType, m_PointLineStart_LCS, m_PointLineEnd_LCS, m_MarkObjectSize / 3);

                    // Vyrobime sadu valcov pre segmenty ciary a pridame ju do zoznamu
                    for (int i = 0; i < line.PointsCollection.Count; i += 2) // Ako zaciatok berieme kazdy druhy bod
                    {
                        float fLineSegmentLength = (float)(line.PointsCollection[i + 1].X - line.PointsCollection[i].X);
                        model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(line.PointsCollection[i], NumberOfCirclePointsLine, m_LineCylinderRadius, fLineSegmentLength, material, 0, false, false));
                    }
                }
            }

            // Transformacie
            float offset_z = 0.5f; // Nastavitelne ???? Offset pred bodom smerom k pozorovatelovi (LCS z), aby bol text a znacka detailu pred bodom control point a nevnaral sa do modelu
            TranslateTransform3D translateOriginInLCS = new TranslateTransform3D(0, 0, offset_z);

            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            double dRotationX = 0;
            double dRotationZ = 0;

            if (Direction.Z == -1 || Direction.Z == 1)
                dRotationX = 90;

            if (Direction.Y == 1)
                dRotationZ = 90;

            if(Direction.Y == -1)
                dRotationZ = -90;

            if (Direction.X == -1)
                dRotationZ = 180;

            // About X
            AxisAngleRotation3D axisAngleRotation3dX = new AxisAngleRotation3D();
            axisAngleRotation3dX.Axis = new Vector3D(1, 0, 0);
            axisAngleRotation3dX.Angle = dRotationX;
            rotateX.Rotation = axisAngleRotation3dX;

            // About Z - plane XY
            AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
            axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
            axisAngleRotation3dZ.Angle = dRotationZ;
            rotateZ.Rotation = axisAngleRotation3dZ;

            TranslateTransform3D translateOrigin = new TranslateTransform3D(m_ControlPoint.X, m_ControlPoint.Y, m_ControlPoint.Z);

            TransformGr = new Transform3DGroup();
            TransformGr.Children.Add(translateOriginInLCS);
            TransformGr.Children.Add(rotateX);
            TransformGr.Children.Add(rotateY);
            TransformGr.Children.Add(rotateZ);
            TransformGr.Children.Add(translateOrigin); // Presun v ramci GCS

            model_gr.Transform = TransformGr;

            return model_gr;
        }
    }
}
