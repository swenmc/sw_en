using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CSectionSymbol
    {
        private Point3D m_ControlPoint;
        private Point3D m_PointLineStart_LCS;
        private Point3D m_PointLineEnd_LCS;
        private Vector3D m_ViewDirection;
        private Point3D m_PointLabelText;
        private string m_LabelText;
        private float m_LineFarEndToControlPointOffsetDistance;
        private float m_LineLength;

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

        public Vector3D ViewDirection
        {
            get
            {
                return m_ViewDirection;
            }

            set
            {
                m_ViewDirection = value;
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

        public float LineFarEndToControlPointOffsetDistance
        {
            get
            {
                return m_LineFarEndToControlPointOffsetDistance;
            }

            set
            {
                m_LineFarEndToControlPointOffsetDistance = value;
            }
        }

        public float LineLength
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

        float fOffsetFromPlane = 0.05f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

        ELinePatternType m_LinePatternType;
        float fArrowSize;
        float fArrowWidth;
        float fAdditionalOffset;

        public bool m_bArrowLeftFromControlPoint;

        public int iVectorOverFactor_LCS;
        public int iVectorUpFactor_LCS;

        public Transform3DGroup TransformGr;

        public CSectionSymbol(
        Point3D controlPoint,
        Vector3D viewdirection,
        string sLabelText,
        float fLineFarEndToControlPointOffsetDistance,
        float fLineLength,
        bool bArrowLeftFromControlPoint
            )
        {
            m_ControlPoint = controlPoint;
            m_ViewDirection = viewdirection;
            m_LabelText = sLabelText;
            m_LineFarEndToControlPointOffsetDistance = fLineFarEndToControlPointOffsetDistance; // Vzdy je kladne, o tom ci je to bod vlavo alebo vpravo rozhoduje bArrowLeftFromControlPoint
            m_LineLength = fLineLength;

            m_LinePatternType = ELinePatternType.DASHDOTTED; // Mohlo by byt nastavitelne aj ale je to velmi zriedave

            m_bArrowLeftFromControlPoint = bArrowLeftFromControlPoint;

            m_PointLineStart_LCS = new Point3D(-m_LineFarEndToControlPointOffsetDistance, 0, 0);
            m_PointLineEnd_LCS = new Point3D(-m_LineFarEndToControlPointOffsetDistance + m_LineLength, 0, 0);

            float fTextSize = 40; // Vyska textu 1 point = 0.01 metra = 30 / 100 = 0.3 metra
            float fSpaceToLine = 10; // medzera medzi hornou hranou textu a ciarou
            fArrowSize = (fTextSize + fSpaceToLine) / 100f;
            fArrowWidth = 0.1f * fArrowSize;

            fAdditionalOffset = 0.5f * fArrowWidth; // Pridavna konstanta (pre vzdialenost sipky od konca ciary)
            float fDistanceOfTextCenterToTheArrow = 0.5f * fTextSize / 100f;

            // TODO - teraz je text vzdy v polovici vzdialenosti zaciatku ciary od urcujuceho bodu
            // Asi by bolo dobre urobit to nastavitelne
            // Podobne urobit nastavitelnu poziciu sipky - fArrowPosition_LCS_x
            m_PointLabelText = new Point3D(-m_LineFarEndToControlPointOffsetDistance + 0.5f * fArrowWidth + fAdditionalOffset + fDistanceOfTextCenterToTheArrow, - fArrowSize + (0.3f * fTextSize / 100f), 0); // Pozicia textu v LCS - text je pod osou x

            if (!m_bArrowLeftFromControlPoint)
            {
                m_PointLineStart_LCS = new Point3D(m_LineFarEndToControlPointOffsetDistance - m_LineLength, 0, 0);
                m_PointLineEnd_LCS = new Point3D(m_LineFarEndToControlPointOffsetDistance, 0, 0);

                // 1.5 * ???? neviem preco je 0.5 * zle, vid popis nizsie
                m_PointLabelText = new Point3D(m_LineFarEndToControlPointOffsetDistance - 1.5f * fArrowWidth - fAdditionalOffset - fDistanceOfTextCenterToTheArrow, -fArrowSize + (0.3f * fTextSize / 100f), 0);
            }

            SetTextPointInLCS();
        }

        // TODO Ondrej - Refaktorovat
        public void SetTextPointInLCS()
        {
            iVectorOverFactor_LCS = 1;
            iVectorUpFactor_LCS = 1;
        }

        public Model3DGroup GetSectionSymbolModel(System.Windows.Media.Color color)
        {
            Model3DGroup model_gr = new Model3DGroup();

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color)); // TODO Ondrej - nastavitelna hrubka a farba pre zobrazenie v GUI a pre Export

            // Vytvorime model gridline v LCS, [0,0,0] je uvazovane v bode m_ControlPoint, line smeruje v kladnom smere x a znacka s label text je v rovine xy
            // Sipka smeruje v smere LCS y

            float fLineCylinderRadius = 0.02f; // Nastavovat ! polomer valca, malo by to byt zhruba 0.7 mm hrube na vykrese (zhruba 3x taka hrubka ako maju ostatne ciary)
            
            // Line
            short NumberOfCirclePointsLine = 8 + 1;//8 + 1;

            if (m_LinePatternType == ELinePatternType.CONTINUOUS) // Ak je continuous tak nepouzijeme CLine
                model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(m_PointLineStart_LCS, NumberOfCirclePointsLine, fLineCylinderRadius, (float)m_LineLength, material, 0, false, false));
            else // Iny typ ciary
            {
                // dashed, dotted, divide, ....

                // Vytvorime liniu zacinajucu v start point v smere x s celkovou dlzkou
                CLine line = new CLine(m_LinePatternType, m_PointLineStart_LCS, m_PointLineEnd_LCS, fLineCylinderRadius * 7);

                // Vyrobime sadu valcov pre segmenty ciary a pridame ju do zoznamu
                for (int i = 0; i < line.PointsCollection.Count; i += 2) // Ako zaciatok berieme kazdy druhy bod
                {
                    float fLineSegmentLength = (float)(line.PointsCollection[i + 1].X - line.PointsCollection[i].X);
                    model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(line.PointsCollection[i], NumberOfCirclePointsLine, fLineCylinderRadius, fLineSegmentLength, material, 0, false, false));
                }
            }

            bool bAddArrow = true;

            if(bAddArrow)
            {
                // Pridame model sipky smerujucej v smere LCS y, nastavitelna poloha sipky v smere x
                float fArrowPosition_LCS_x = -m_LineFarEndToControlPointOffsetDistance + 0.5f * fArrowWidth + fAdditionalOffset;

                if (!m_bArrowLeftFromControlPoint)
                {
                    // fArrowPosition_LCS_x = m_LineFarEndToControlPointOffsetDistance - 0.5f * fArrowWidth - fAdditionalOffset; // To Ondrej - Nechapem preco je to toto zle, mala by to byt pozicia sipky od zaciatocneho bodu ked sa znacka kresli na pravu stranu
                    // Dal som tam 1.5 nasobok ale neviem preco :)))
                    fArrowPosition_LCS_x = m_LineFarEndToControlPointOffsetDistance - 1.5f * fArrowWidth - fAdditionalOffset;
                }

                Point3D arrowControlPoint = new Point3D(fArrowPosition_LCS_x, 0, 0); // Arrow Tip in LCS of section symbol
                Objects_3D.StraightLineArrow3D arrow = new Objects_3D.StraightLineArrow3D(arrowControlPoint, fArrowSize, 1);

                GeometryModel3D model = new GeometryModel3D();
                MeshGeometry3D mesh = new MeshGeometry3D();

                mesh.Positions = arrow.ArrowPoints;
                mesh.TriangleIndices = arrow.GetArrowIndices();
                model.Geometry = mesh;
                model.Material = material;

                // Vyrobime novu transformaciu kde okrem posunu bude aj rotacia sipky okolo z o 180 stupnov
                Transform3DGroup trGroup = new Transform3DGroup();
                trGroup.Children.Add(model.Transform);

                // About Z - plane XY
                AxisAngleRotation3D axisAngleRotation3dZ_Arrow = new AxisAngleRotation3D();
                axisAngleRotation3dZ_Arrow.Axis = new Vector3D(0, 0, 1);
                axisAngleRotation3dZ_Arrow.Angle = 180;

                RotateTransform3D rotateZ_Arrow = new RotateTransform3D(axisAngleRotation3dZ_Arrow, arrowControlPoint);
                trGroup.Children.Add(rotateZ_Arrow);

                model.Transform = trGroup; // Nastavime modelu sipky novu transoformaciu vratane otocenia o 180 stupnov

                model_gr.Children.Add(model); // Add straight arrow
            }

            // Transformacie
            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            double dRotationZ = 0;

            if (ViewDirection.Y == 1)
            {
                // Nastavenie textu citatelneho zdola nahor (citatel je vpravo)
                iVectorOverFactor_LCS = -1;
                iVectorUpFactor_LCS = -1;
            }

            if (ViewDirection.Y == -1)
                dRotationZ = 180;

            if (ViewDirection.X == 1)
            {
                dRotationZ = -90;

                // Nastavenie textu citatelneho zlava doprava (citatel je dole)
                iVectorOverFactor_LCS = -1;
                iVectorUpFactor_LCS = -1;
            }

            if (ViewDirection.X == -1)
                dRotationZ = 90;


            // About Z - plane XY
            AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
            axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
            axisAngleRotation3dZ.Angle = dRotationZ;
            rotateZ.Rotation = axisAngleRotation3dZ;

            TranslateTransform3D translateOrigin = new TranslateTransform3D(m_ControlPoint.X, m_ControlPoint.Y, m_ControlPoint.Z + fOffsetFromPlane);

            TransformGr = new Transform3DGroup();
            TransformGr.Children.Add(rotateX);
            TransformGr.Children.Add(rotateY);
            TransformGr.Children.Add(rotateZ);
            TransformGr.Children.Add(translateOrigin); // Presun v ramci GCS

            model_gr.Transform = TransformGr;

            return model_gr;
        }
    }
}
