using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;
using System.Windows.Media;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    [Serializable]
    public class CControlJoint : CEntity3D
    {
        private Point3D m_PointStart;

        public Point3D PointStart
        {
            get { return m_PointStart; }
            set { m_PointStart = value; }
        }
        private Point3D m_PointEnd;

        public Point3D PointEnd
        {
            get { return m_PointEnd; }
            set { m_PointEnd = value; }
        }

        private Point3D m_PointText;

        public Point3D PointText
        {
            get { return m_PointText; }
            set { m_PointText = value; }
        }

        private float m_fLength;

        public float Length
        {
            get { return m_fLength; }
            set { m_fLength = value; }
        }

        private string m_Text;

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        private float m_DowelSpacing;

        public float DowelSpacing
        {
            get { return m_DowelSpacing; }
            set { m_DowelSpacing = value; }
        }

        private CDowel m_ReferenceDowel;

        public CDowel ReferenceDowel
        {
            get { return m_ReferenceDowel; }
            set { m_ReferenceDowel = value; }
        }

        // TO Ondrej - ak maju podla teba tie premenne zmysel tak z nich treba urobit properties, mozno by sa dalo riesit priamo v tomto objekte aky je smer textu
        bool bTextAboveLine; // true - text je medzi nad liniou (alebo nalavo od nej), false - text je na opacnej strane (pod liniou) alebo napravo od nej
        public int iVectorOverFactor_LCS;
        public int iVectorUpFactor_LCS;

        public Transform3DGroup TransformGr;

        public CControlJoint(int id, Point3D start, Point3D end, CDowel referenceDowel, float fDowelSpacing, bool bIsDiplayed_temp, int fTime)
        {
            ID = id;
            m_PointStart = start;
            m_PointEnd = end;
            m_ReferenceDowel = referenceDowel;
            m_DowelSpacing = fDowelSpacing;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;

            m_Text = "CONTROL JOINT";

            Length = Drawing3D.GetPoint3DDistanceFloat(m_PointStart, m_PointEnd);

            SetTextPointInLCS(); // Text v LCS
        }

        // TODO Ondrej - Refaktorovat
        public void SetTextPointInLCS()
        {
            bTextAboveLine = false;

            iVectorOverFactor_LCS = -1;
            iVectorUpFactor_LCS = -1;

            float fOffsetFromLine;
            float fOffsetFromPlane = 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

            if (bTextAboveLine)
                fOffsetFromLine = 0.1f; // Mezera medzi ciarou a textom (kladna - text nad ciarou (+y), zaporna, text pod ciarou (-y))
            else
                fOffsetFromLine = -0.1f;

            m_PointText = new Point3D()
            {
                X = 0.2 * m_fLength, // Kreslime v 20% dlzky od zaciatku
                Y = fOffsetFromLine,
                Z = fOffsetFromPlane
            };
        }

        // Nastavovat ! polomer valca, co najmensi ale viditelny - 3D
        public Model3DGroup GetControlJointModel(Color color, ELinePatternType linePatternType, float fLineCylinderRadius/* = 0.005f*/)
        {
            Model3DGroup model_gr = new Model3DGroup();
            
            DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(color));

            //float fLineThickness = 0.002f; // hrubka = priemer pre export do 2D (2 x polomer valca)
            float fDashSegmentLen = fLineCylinderRadius * 40;

            // LCS - line in x-direction
            if (linePatternType == ELinePatternType.CONTINUOUS) // Ak je continuous tak nepouzijeme CLine
                model_gr.Children.Add(Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), 13, fLineCylinderRadius, m_fLength, material, 0));
            else // Iny typ ciary
            {
                // dashed, dotted, divide, ....

                // Vytvorime liniu zacinajucu v start point v smere x s celkovou dlzkou
                CLine line = new CLine(linePatternType, new Point3D(0, 0, 0), new Point3D(m_fLength, 0, 0), fDashSegmentLen);

                // Vyrobime sadu valcov pre segmenty ciary a pridame ju do zoznamu
                for (int i = 0; i < line.PointsCollection.Count; i += 2) // Ako zaciatok berieme kazdy druhy bod
                {
                    float fLineSegmentLength = (float)(line.PointsCollection[i + 1].X - line.PointsCollection[i].X);
                    model_gr.Children.Add(Cylinder.CreateM_G_M_3D_Volume_Cylinder(line.PointsCollection[i], 13, fLineCylinderRadius, fLineSegmentLength, material, 0, false, false));
                }
            }

            // Spocitame priemety
            double dDeltaX = m_PointEnd.X - m_PointStart.X;
            double dDeltaY = m_PointEnd.Y - m_PointStart.Y;
            double dDeltaZ = m_PointEnd.Z - m_PointStart.Z;

            // Returns transformed coordinates of member nodes
            // Angles
            double dAlphaX = 0, dBetaY = 0, dGammaZ = 0;

            // Uhly pootocenia LCS okolo osi GCS
            // Angles
            dAlphaX = Geom2D.GetAlpha2D_CW(dDeltaY, dDeltaZ);
            dBetaY = Geom2D.GetAlpha2D_CW_2(dDeltaX, dDeltaZ); // !!! Pre pootocenie okolo Y su pouzite ine kvadranty !!!
            dGammaZ = Geom2D.GetAlpha2D_CW(dDeltaX, dDeltaY);

            // To Ondrej - toto treba nejako normalnejsie poskladat a refaktorovat s CDimensionLineLinear3D a CSawCut
            if (dGammaZ > 0) // Otacame proti smeru hodinovych ruciciek - text chceme mat citatelny horizontalne takze musime prehodit vektor
            {
                bTextAboveLine = true;

                iVectorOverFactor_LCS = 1;
                iVectorUpFactor_LCS = 1;

                float fOffsetFromLine = 0.1f;
                float fOffsetFromPlane = 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

                m_PointText = new Point3D()
                {
                    X = 0.2 * m_fLength, // Kreslime v 20% dlzky od zaciatku
                    Y = fOffsetFromLine,
                    Z = fOffsetFromPlane
                };
            }

            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            // About Z - plane XY
            AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
            axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
            axisAngleRotation3dZ.Angle = Geom2D.RadiansToDegrees(dGammaZ);
            rotateZ.Rotation = axisAngleRotation3dZ;

            TranslateTransform3D translateOrigin = new TranslateTransform3D(m_PointStart.X, m_PointStart.Y, m_PointStart.Z);

            TransformGr = new Transform3DGroup();
            TransformGr.Children.Add(rotateX);
            TransformGr.Children.Add(rotateY);
            TransformGr.Children.Add(rotateZ);
            TransformGr.Children.Add(translateOrigin); // Presun celej koty v ramci GCS

            model_gr.Transform = TransformGr;

            return model_gr;
        }
    }
}
