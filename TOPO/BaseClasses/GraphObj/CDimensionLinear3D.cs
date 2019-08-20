using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CDimensionLinear3D
    {
        private Point3D m_PointStart;
        private Point3D m_PointEnd;
        private Point3D m_PointText;

        private Point3D m_PointStartL2;
        private Point3D m_PointEndL2;
        private Point3D m_PointMainLine1;
        private Point3D m_PointMainLine2;

        private Vector3D m_Direction;
        private Vector3D m_Horizontal;
        private Vector3D m_Vertical;
        private double m_ExtensionLinesLength;
        private double m_DimensionMainLineDistance;

        private float m_fOffSetFromPoint;

        private string m_Text;

        public Point3D PointStart
        {
            get
            {
                return m_PointStart;
            }

            set
            {
                m_PointStart = value;
            }
        }

        public Point3D PointEnd
        {
            get
            {
                return m_PointEnd;
            }

            set
            {
                m_PointEnd = value;
            }
        }

        public Vector3D Horizontal
        {
            get
            {
                return m_Horizontal;
            }

            set
            {
                m_Horizontal = value;
            }
        }

        public Vector3D Vertical
        {
            get
            {
                return m_Vertical;
            }

            set
            {
                m_Vertical = value;
            }
        }

        public double DimensionLinesLength
        {
            get
            {
                return m_ExtensionLinesLength;
            }

            set
            {
                m_ExtensionLinesLength = value;
            }
        }

        public double DimensionMainLineDistance
        {
            get
            {
                return m_DimensionMainLineDistance;
            }

            set
            {
                m_DimensionMainLineDistance = value;
            }
        }

        public string Text
        {
            get
            {
                return m_Text;
            }

            set
            {
                m_Text = value;
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

        public Point3D PointText
        {
            get
            {
                return m_PointText;
            }

            set
            {
                m_PointText = value;
            }
        }

        public Point3D PointStartL2
        {
            get
            {
                return m_PointStartL2;
            }

            set
            {
                m_PointStartL2 = value;
            }
        }

        public Point3D PointEndL2
        {
            get
            {
                return m_PointEndL2;
            }

            set
            {
                m_PointEndL2 = value;
            }
        }

        public Point3D PointMainLine1
        {
            get
            {
                return m_PointMainLine1;
            }

            set
            {
                m_PointMainLine1 = value;
            }
        }

        public Point3D PointMainLine2
        {
            get
            {
                return m_PointMainLine2;
            }

            set
            {
                m_PointMainLine2 = value;
            }
        }

        public float OffSetFromPoint  // Odsadenie bodu vynasacej ciary (extension line) od kotovaneho bodu
        {
            get
            {
                return m_fOffSetFromPoint;
            }

            set
            {
                m_fOffSetFromPoint = value;
            }
        }

        public CDimensionLinear3D() { }
        public CDimensionLinear3D(Point3D pointStart, Point3D pointEnd, Vector3D direction, Vector3D textHorizontal, Vector3D textVertical, double extensionLinesLength, double dimensionMainLineDistance, double fOffsetFromPoint, string text)
        {
            // TO Ondrej
            // Nazvy - main dimension line (hlavna kotovacia ciara) (ta hlavna dlha ciara na ktoru sa pise text)
            // extension line (vynasacia ciara) (tie kratke ciarky smerujuce od kotovaneho bodu ku koncom hlavnej ciary), moze mat fixnu dlzku alebo moze mat fixny odstup od kotovaneho bodu, moze mat aj presah, tj konci az za hlavnou ciarou

            m_PointStart = pointStart;
            m_PointEnd = pointEnd;
            m_Direction = direction;
            m_Horizontal = textHorizontal;
            m_Vertical = textVertical;

            // TO Ondrej - su by sme mohli vyrobit viacero moznosti a kombinacii:
            // zadavat fixnu dlzku extension line
            // zadavat vzdialenost hlavnej kotovacej ciary od kotovaneho bodu
            // zadavat fixny offset extension line of kotovaneho bodu
            // zadavat presah extension line za main dimension line

            // V praxi vacsinou chceme, aby boli extension line rozne dlhe s fixnym odstupom od kotovaneho bodu
            // Moznost ze su extension line konstantnej dlzky a odsadenie je rozne je menej casta lebo ak je odsadenie velke tak nemusi byt jasne ktory bod kotujeme

            m_ExtensionLinesLength = extensionLinesLength;
            m_DimensionMainLineDistance = dimensionMainLineDistance;
            m_fOffSetFromPoint = (float)fOffsetFromPoint; // Odsadenie bodu vynasacej ciary (extension line) od kotovaneho bodu
            m_Text = text;
            SetTextPoint();
            SetPoints();
        }

        public void SetTextPoint()
        {
            m_PointText = new Point3D() {
                X = (m_PointStart.X + m_PointEnd.X) / 2 + Direction.X * DimensionLinesLength,
                Y = (m_PointStart.Y + m_PointEnd.Y) / 2 + Direction.Y * DimensionLinesLength,
                Z = (m_PointStart.Z + m_PointEnd.Z) / 2 + Direction.Z * DimensionLinesLength
            };
        }

        public void SetPoints()
        {
            m_PointMainLine1 = new Point3D()
            {
                X = m_PointStart.X + Direction.X * DimensionMainLineDistance,
                Y = m_PointStart.Y + Direction.Y * DimensionMainLineDistance,
                Z = m_PointStart.Z + Direction.Z * DimensionMainLineDistance,
            };

            m_PointMainLine2 = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * DimensionMainLineDistance,
                Y = m_PointEnd.Y + Direction.Y * DimensionMainLineDistance,
                Z = m_PointEnd.Z + Direction.Z * DimensionMainLineDistance,
            };

            m_PointStartL2 = new Point3D()
            {
                X = m_PointStart.X + Direction.X * DimensionLinesLength,
                Y = m_PointStart.Y + Direction.Y * DimensionLinesLength,
                Z = m_PointStart.Z + Direction.Z * DimensionLinesLength,
            };

            m_PointEndL2 = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * DimensionLinesLength,
                Y = m_PointEnd.Y + Direction.Y * DimensionLinesLength,
                Z = m_PointEnd.Z + Direction.Z * DimensionLinesLength,
            };
        }

        public Model3DGroup GetDimensionModel(System.Windows.Media.Color color)
        {
            // Zakladny model koty - hlavna kotovacia ciara - smer X, vynasacie ciary - smer Y
            // TEXT by som kreslil v LCS koty do roviny XY a potom ho otacal s kotou (system potom mozeme pouzit aj pre popisy prutov, staci vyplnut zobrazenie koty a ostane len text)
            // Vhodne zvolit kde ma byt bod [0,0,0]

            Model3DGroup model_gr = new Model3DGroup();

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color)); // TODO Ondrej - urobit nastavitelnu hrubku a farbu kotovacich ciar (Okno options pre zobrazenie v GUI a pre Export)

            float fMainLineLength = (float)Math.Sqrt((float)Math.Pow(m_PointMainLine2.X - m_PointMainLine1.X, 2f) + (float)Math.Pow(m_PointMainLine2.Y - m_PointMainLine1.Y, 2f) + (float)Math.Pow(m_PointMainLine2.Z - m_PointMainLine1.Z, 2f));
            float fLineThickness = 0.002f; // hrubka = priemer pre export do 2D (2 x polomer valca)
            float fLineCylinderRadius = 0.005f; //0.005f * fMainLineLength; // Nastavovat ! polomer valca, co najmensi ale viditelny - 3D

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TO Ondre pozri na toto a ak rozumies o com pisem mozes to zmazat, ak nie tak nakreslim a dovysvetlim :-)
            // Komentar - TO Ondrej - nemozeme sem pridavat do skupiny modelu koty tento transformovany valec, lebo ta sa kresli v smere x a az potom sa transformuje, takze tieto transformovane suradnice sa transformuju potom este raz
            // Sem mozeme pridat len valec v LCS smere x koty
            // Pokus - Ondrej
            //model_gr.Children.Add(Drawing3D.Get3DLineReplacement(color, m_PointMainLine1, m_PointMainLine2));
            // Pokus - Martin
            bool bUkazkaPreOndreja_MozeSaZmazat = false;
            if(bUkazkaPreOndreja_MozeSaZmazat)
               model_gr.Children.Add(Drawing3D.Get3DLineReplacement(color, fLineThickness, new Point3D(0,0,0), new Point3D(fMainLineLength, 0, 0))); // Tak to by to bolo spravne
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Main line
            // Default tip (cone height is 20% from length)
            Objects_3D.StraightLineArrow3D arrow = new Objects_3D.StraightLineArrow3D(fMainLineLength, fLineCylinderRadius, 0, true);
            GeometryModel3D model = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions = arrow.GetArrowPoints();
            mesh.TriangleIndices = arrow.GetArrowIndices();
            model.Geometry = mesh;
            model.Material = material;
            model_gr.Children.Add(model);  // Add straight arrow

            // Add other lines
            //TODO - Zapracovat nastavitelnu dlzku a nastavitelny offset pre extension lines
            short NumberOfCirclePoints = 16 + 1; // Toto by malo byt rovnake ako u arrow, je potrebne to zjednotit, pridany jeden bod pre stred

            // TO Ondrej - toto treba prerobit, ja kreslim tuto kotu v rovine XY a potom ju chcem otacat a presuvat podla potreby
            float fExtensionLine1_Length = (float)DimensionLinesLength;
            float fExtensionLine2_Length = (float)DimensionLinesLength;

            float fExtensionLine1_OffsetBehindMainLine = (float)(DimensionLinesLength - DimensionMainLineDistance);
            float fExtensionLine2_OffsetBehindMainLine = (float)(DimensionLinesLength - DimensionMainLineDistance);

            // Extension line 1 (start)
            //CVolume temp = new CVolume(); // Pomocne - bolo by dobre sa toho zbavit a vytvarat objekt typu cylinder priamo
            model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, -fExtensionLine1_OffsetBehindMainLine, 0), NumberOfCirclePoints, fLineCylinderRadius, fExtensionLine1_Length, material,1));

            // Extension line 2 (end)
            model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(fMainLineLength, -fExtensionLine2_OffsetBehindMainLine, 0), NumberOfCirclePoints, fLineCylinderRadius, fExtensionLine2_Length, material,1));

            // Nastavit offset celej koty od kotovaneho bodu (smer -Y)

            TranslateTransform3D translateOffset = new TranslateTransform3D(0, -(fExtensionLine1_Length - fExtensionLine1_OffsetBehindMainLine + m_fOffSetFromPoint), 0);

            // TO ONDREJ - tu treba pridat dalsie transformacie ak chceme kotu ako celok posuvat alebo otacat atd, trosku sa s tym treba pohrat, zobecnit tak aby sa dali kreslit aj sikme koty napriklad na rafteroch pri pohlade na ram zpredu
            // TODO - toto otacanie by chcelo nejako sprehladnit a zjednodusit - kotu vyrobim v rovine XY a potom ju mozeme otacat tak aby bola v rovine pohladu, moze sa stat ze je pootocena okolo osy smerujucej kolmo na monitor (oznacena pre view Y - vid member description)
            // Skus porozmyslat a navrhnut nejaky univerzalny system ako budeme tie koty a ich texty umiestnovat

            RotateTransform3D rotateX = new RotateTransform3D();
            RotateTransform3D rotateY = new RotateTransform3D();
            RotateTransform3D rotateZ = new RotateTransform3D();

            bool bDrawHorizontalDimesnionsInXY = false; // Toto by mohlo byt nastavitelne pre 3D scenu v GUI a pre export 3D pohladov, pre exporty 2D pohladov na rovinu XZ alebo YZ sa to nehodi, lebo kotu by nebolo vidno

            if (!bDrawHorizontalDimesnionsInXY && Direction.Z != 0) // Kota v smere X alebo Y - ak chceme aby nebola vykreslena v rovine XY ale zvislo v XZ alebo YZ
            {
                // About X
                AxisAngleRotation3D axisAngleRotation3dX = new AxisAngleRotation3D();
                axisAngleRotation3dX.Axis = new Vector3D(1, 0, 0);
                axisAngleRotation3dX.Angle = Direction.Z == -1 ? 90 : 270; // Otocena nadol 90 od kotovanych bodov alebo nahor 270 okolo LCS x
                rotateX.Rotation = axisAngleRotation3dX;

                // Kota je v rovine XZ alebo YZ
                // Sikme koty potrebujeme pootocit okolo globalnych os X resp. Y, tomu zodpoveda pootocenie okolo LCS z

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

                if (MathF.d_equal(dDeltaY, 0)) // Kota je v GCS rovine XZ
                {
                    // About Y
                    AxisAngleRotation3D axisAngleRotation3dY = new AxisAngleRotation3D();
                    axisAngleRotation3dY.Axis = new Vector3D(0, 1, 0);
                    axisAngleRotation3dY.Angle = Geom2D.RadiansToDegrees(dBetaY);
                    rotateY.Rotation = axisAngleRotation3dY;
                }
            }

            if ((Direction.X != 0 && (MathF.Equals(m_PointStart.X, m_PointEnd.X))) ||
            (Direction.Y != 0 && MathF.Equals(m_PointStart.Y, m_PointEnd.Y))) // Zvisla kota TODO - Dopracovat pre ine smery
            {
                // About Y
                AxisAngleRotation3D axisAngleRotation3dY = new AxisAngleRotation3D();
                axisAngleRotation3dY.Axis = new Vector3D(0, 1, 0);
                axisAngleRotation3dY.Angle = -90;
                rotateY.Rotation = axisAngleRotation3dY;

                // About Z
                AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
                axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
                axisAngleRotation3dZ.Angle = -90;
                rotateZ.Rotation = axisAngleRotation3dZ;
            }

            if(Direction.X == 0 && MathF.Equals(m_PointStart.X, m_PointEnd.X)) // Kota v smere Y
            {
                // About Z
                AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
                axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
                axisAngleRotation3dZ.Angle = 90;
                rotateZ.Rotation = axisAngleRotation3dZ;
            }

            TranslateTransform3D translateOrigin = new TranslateTransform3D(m_PointStart.X, m_PointStart.Y, m_PointStart.Z);

            Transform3DGroup transformGr = new Transform3DGroup();
            transformGr.Children.Add(translateOffset); // Posun o offset v rovine XY
            transformGr.Children.Add(rotateX);
            transformGr.Children.Add(rotateY);
            transformGr.Children.Add(rotateZ);
            transformGr.Children.Add(translateOrigin); // Presun celej koty v ramci GCS

            model_gr.Transform = transformGr;

            return model_gr;
        }
    }
}
