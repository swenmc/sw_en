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

        //private Point3D m_PointStartL2;
        //private Point3D m_PointEndL2;

        private Point3D m_Point1_ExtensionLine1;
        private Point3D m_Point2_ExtensionLine1;
        private Point3D m_Point1_ExtensionLine2;
        private Point3D m_Point2_ExtensionLine2;

        private Point3D m_Point1_MainLine;
        private Point3D m_Point2_MainLine;

        private Vector3D m_Direction;        
        //private double m_ExtensionLinesLength;
        private double m_ExtensionLine1Length;
        private double m_ExtensionLine2Length;
        private double m_ExtensionLines_OffsetBehindMainLine;


        private double m_DimensionMainLineDistance;

        private double m_fOffSetFromPoint;
        private float m_fMainLineLength;
        private double m_DimensionMainLinePositionIncludingOffset;

        private string m_Text;
        
        private bool bTextInside; // true - text je medzi vynasacimi ciarami, false - text je na opacnej strane nez vynasacie ciary
        private int iVectorOverFactor_LCS;
        private int iVectorUpFactor_LCS;

        private EGlobalPlane m_GlobalPlane;
        //public int iGlobalPlane; // Globalna rovina GCS do ktorej sa kota kresli 0 - XY, 1 - YZ, 2 - XZ, -1 nedefinovana (vseobecna kota)
        public int iVectorOfProjectionToHorizontalViewAxis; // -1 kota sa kresli horizontalne pod body, 1 kota sa kresli horizontalne nad body, 0 - nie je definovane
        public int iVectorOfProjectionToVerticalViewAxis; // -1 kota sa kresli vertikalne nalavo od bodov, 1 kota sa kresli vertiklane napravo od bodov, 0 - nie je definovane

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

        //public double DimensionLinesLength
        //{
        //    get
        //    {
        //        return m_ExtensionLinesLength;
        //    }

        //    set
        //    {
        //        m_ExtensionLinesLength = value;
        //    }
        //}

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

        //public Point3D PointStartL2
        //{
        //    get
        //    {
        //        return m_PointStartL2;
        //    }

        //    set
        //    {
        //        m_PointStartL2 = value;
        //    }
        //}

        //public Point3D PointEndL2
        //{
        //    get
        //    {
        //        return m_PointEndL2;
        //    }

        //    set
        //    {
        //        m_PointEndL2 = value;
        //    }
        //}

        public Point3D PointMainLine1
        {
            get
            {
                return m_Point1_MainLine;
            }

            set
            {
                m_Point1_MainLine = value;
            }
        }

        public Point3D PointMainLine2
        {
            get
            {
                return m_Point2_MainLine;
            }

            set
            {
                m_Point2_MainLine = value;
            }
        }

        public double OffSetFromPoint  // Odsadenie bodu vynasacej ciary (extension line) od kotovaneho bodu
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

        public float MainLineLength  // Dlzka hlavnej kotovacej ciary
        {
            get
            {
                return m_fMainLineLength;
            }

            set
            {
                m_fMainLineLength = value;
            }
        }

        public double DimensionMainLinePositionIncludingOffset  // Finalna poloha hlavnej kotovacej ciary vratane offsetu od kotovaneho bodu
        {
            get
            {
                return m_DimensionMainLinePositionIncludingOffset;
            }

            set
            {
                m_DimensionMainLinePositionIncludingOffset = value;
            }
        }

        public bool TextInside
        {
            get
            {
                return bTextInside;
            }

            set
            {
                bTextInside = value;
            }
        }

        public int VectorOverFactor_LCS
        {
            get
            {
                return iVectorOverFactor_LCS;
            }

            set
            {
                iVectorOverFactor_LCS = value;
            }
        }

        public int VectorUpFactor_LCS
        {
            get
            {
                return iVectorUpFactor_LCS;
            }

            set
            {
                iVectorUpFactor_LCS = value;
            }
        }

        public EGlobalPlane GlobalPlane
        {
            get
            {
                return m_GlobalPlane;
            }

            set
            {
                m_GlobalPlane = value;
            }
        }

        public double ExtensionLine1Length
        {
            get
            {
                return m_ExtensionLine1Length;
            }

            set
            {
                m_ExtensionLine1Length = value;
            }
        }

        public double ExtensionLine2Length
        {
            get
            {
                return m_ExtensionLine2Length;
            }

            set
            {
                m_ExtensionLine2Length = value;
            }
        }

        public double ExtensionLines_OffsetBehindMainLine
        {
            get
            {
                return m_ExtensionLines_OffsetBehindMainLine;
            }

            set
            {
                m_ExtensionLines_OffsetBehindMainLine = value;
            }
        }

        public Transform3DGroup TransformGr;


        //TODO
        //upravit vstupne parametre tak aby som vedel zadat rovinu v ktorej sa ma kota vykreslit, body koty, 
        //polohu textu nad a pod ciarkou a orientaciu koty nejakym sposobom nad alebo pod bodmi v LCS

        public CDimensionLinear3D() { }
        public CDimensionLinear3D(Point3D pointStart,
            Point3D pointEnd,
            Vector3D direction,
            EGlobalPlane globalPlane, // Globalna rovina GCS do ktorej sa kota kresli 0 - XY, 1 - YZ, 2 - XZ, -1 nedefinovana (vseobecna kota)            
            int iVectorOfProjectionToHorizontalViewAxis_temp, // -1 kota sa kresli horizontalne pod body, 1 kota sa kresli horizontalne nad body, 0 - nie je definovane
            int iVectorOfProjectionToVerticalViewAxis_temp, // -1 kota sa kresli vertikalne nalavo od bodov, 1 kota sa kresli vertikalne napravo od bodov, 0 - nie je definovane
            double extensionLinesLength,
            double dimensionMainLineDistance,
            double fOffsetFromPoint,
            string text, 
            bool textIsInside = false)  //default by mohol byt text vo vnutri  // true - text je medzi vynasacimi ciarami, false - text je na opacnej strane nez vynasacie ciary
        {
            // Nazvy - main dimension line (hlavna kotovacia ciara) (ta hlavna dlha ciara na ktoru sa pise text)
            // extension line (vynasacia ciara) (tie kratke ciarky smerujuce od kotovaneho bodu ku koncom hlavnej ciary), moze mat fixnu dlzku alebo moze mat fixny odstup od kotovaneho bodu, 
            // moze mat aj presah, tj konci az za hlavnou ciarou

            m_PointStart = pointStart;
            m_PointEnd = pointEnd;

            bTextInside = textIsInside;
            // TO Ondrej
            // m_Direction - Tento parameter by som mozno nahradil/doplnil parametrom ktory urcuje do akej roviny GCS sa kota ma kreslit XY, XZ, XY 
            // (ak vieme rovinu, tak vieme ako mame kotu potocit kedze pozname pA a pB suradnicu v rovine,
            // pripadne sa da nastavovat to ze budeme kotovat priemet do osi tvoriacich rovinu,
            // napriklad pre XY bude mozne este nastavit ci chcem priemet do X, priemet do Y alebo realnu vzdialenost medzi pA a pB
            // (pootocena kota, ak nemaju body rovnake suradnice X ani Y)

            // TO Ondrej - tu som pripravil nejake parametre, ktore by sme mohli pouzit na inspiraciu, predpokladam ze sa Ti to nebude pacit a nebudes tomu asi ani uplne rozumiet :)
            // Ide mi o to mat moznost nastavit do akej GCS roviny chcem kotu kreslit
            // Mat moznost nastavit ci chcem kotovat realnu dlzku alebo priemet a ktorym smerom podla nastaveneho pohladu kotu priemetu orientovat
            // ak by boli obe hodnoty iVectorOfProjectionToHorizontalViewAxis a iVectorOfProjectionToVerticalViewAxis rovne 0 znamenalo by to ze chcem v danej rovine kotovat skutocnu vzdialenost 
            // a kota moze byt teda pootocena okolo osi kolmej na tuto rovinu

            // Pre nastavenie projekcie a identifikaciu parametrov pohladu by sme mohli pouzit tieto vektory
            // VIEW AXIS
            //Vector3D viewVector;
            //Vector3D viewHorizontalVector;
            //Vector3D viewVerticalVector;

            m_GlobalPlane = globalPlane; // Globalna rovina GCS do ktorej sa kota kresli 0 - XY, 1 - YZ, 2 - XZ, -1 nedefinovana (vseobecna kota)
            iVectorOfProjectionToHorizontalViewAxis = iVectorOfProjectionToHorizontalViewAxis_temp; // -1 kota sa kresli horizontalne pod body, 1 kota sa kresli horizontalne nad body, 0 - nie je definovane
            iVectorOfProjectionToVerticalViewAxis = iVectorOfProjectionToVerticalViewAxis_temp; // -1 kota sa kresli vertikalne nalavo od bodov, 1 kota sa kresli vertikalne napravo od bodov, 0 - nie je definovane

            // V system komponent viewer kotujeme aj skutocne dlzky aj tie priemety, ale priemety trosku klamem tym ze tam neposielam skutocne body ale take body/suradnice,
            // aby som ziskal kotu v smere osy

            m_Direction = direction;            

            // TO Ondrej - su by sme mohli vyrobit viacero moznosti a kombinacii:
            // zadavat fixnu dlzku extension line
            // zadavat vzdialenost hlavnej kotovacej ciary od kotovaneho bodu
            // zadavat fixny offset extension line of kotovaneho bodu
            // zadavat presah extension line za main dimension line

            // V praxi vacsinou chceme, aby boli extension line rozne dlhe s fixnym odstupom od kotovaneho bodu
            // Moznost ze su extension line konstantnej dlzky a odsadenie je rozne je menej casta lebo ak je odsadenie velke tak nemusi byt jasne ktory bod kotujeme

            m_ExtensionLine1Length = extensionLinesLength;
            m_ExtensionLine2Length = extensionLinesLength;
            m_DimensionMainLineDistance = dimensionMainLineDistance;
            m_ExtensionLines_OffsetBehindMainLine = (extensionLinesLength - DimensionMainLineDistance);
            m_fOffSetFromPoint = fOffsetFromPoint; // Odsadenie bodu vynasacej ciary (extension line) od kotovaneho bodu
            m_Text = text;
            
            SetPoints();

            m_fMainLineLength = (float)Math.Sqrt((float)Math.Pow(m_Point2_MainLine.X - m_Point1_MainLine.X, 2f) + (float)Math.Pow(m_Point2_MainLine.Y - m_Point1_MainLine.Y, 2f) + (float)Math.Pow(m_Point2_MainLine.Z - m_Point1_MainLine.Z, 2f));
            // Suradnica y main line
            m_DimensionMainLinePositionIncludingOffset = - (m_DimensionMainLineDistance + m_fOffSetFromPoint);

            SetTextPointInLCS(); // Text v LCS
        }


        public CDimensionLinear3D(Point3D pointStart, Point3D pointEnd,
            Vector3D direction,
            EGlobalPlane globalPlane, // Globalna rovina GCS do ktorej sa kota kresli 0 - XY, 1 - YZ, 2 - XZ, -1 nedefinovana (vseobecna kota)            
            int iVectorOfProjectionToHorizontalViewAxis_temp, // -1 kota sa kresli horizontalne pod body, 1 kota sa kresli horizontalne nad body, 0 - nie je definovane
            int iVectorOfProjectionToVerticalViewAxis_temp, // -1 kota sa kresli vertikalne nalavo od bodov, 1 kota sa kresli vertikalne napravo od bodov, 0 - nie je definovane
            double extensionLine1Length,
            double extensionLine2Length,
            double fExtensionLine_OffsetBehindMainLine,
            double fOffsetFromPoint,
            string text,
            bool textIsInside = false)  //default by mohol byt text vo vnutri  // true - text je medzi vynasacimi ciarami, false - text je na opacnej strane nez vynasacie ciary
        {
            m_PointStart = pointStart;
            m_PointEnd = pointEnd;

            bTextInside = textIsInside;
            
            m_GlobalPlane = globalPlane; // Globalna rovina GCS do ktorej sa kota kresli 0 - XY, 1 - YZ, 2 - XZ, -1 nedefinovana (vseobecna kota)
            iVectorOfProjectionToHorizontalViewAxis = iVectorOfProjectionToHorizontalViewAxis_temp; // -1 kota sa kresli horizontalne pod body, 1 kota sa kresli horizontalne nad body, 0 - nie je definovane
            iVectorOfProjectionToVerticalViewAxis = iVectorOfProjectionToVerticalViewAxis_temp; // -1 kota sa kresli vertikalne nalavo od bodov, 1 kota sa kresli vertikalne napravo od bodov, 0 - nie je definovane

            m_Direction = direction;

            m_ExtensionLine1Length = extensionLine1Length;
            m_ExtensionLine1Length = extensionLine2Length;
            m_ExtensionLines_OffsetBehindMainLine = fExtensionLine_OffsetBehindMainLine;            
            m_fOffSetFromPoint = fOffsetFromPoint; // Odsadenie bodu vynasacej ciary (extension line) od kotovaneho bodu
            m_Text = text;

            SetPoints2();

            m_fMainLineLength = (float)Math.Sqrt((float)Math.Pow(m_Point2_MainLine.X - m_Point1_MainLine.X, 2f) + (float)Math.Pow(m_Point2_MainLine.Y - m_Point1_MainLine.Y, 2f) + (float)Math.Pow(m_Point2_MainLine.Z - m_Point1_MainLine.Z, 2f));
            // Suradnica y main line
            m_DimensionMainLinePositionIncludingOffset = -(m_DimensionMainLineDistance + m_fOffSetFromPoint);

            SetTextPointInLCS(); // Text v LCS
        }


        public void SetTextPointInLCS()
        {
            // To Ondrej - toto vsetko treba poupravovat aby to malo nejaky koncept a hlavu a patu.
            // Potrebujeme riesit to ze ked kota otocena tak ci onak, ci je pB s nejakou nizsou suradnicou nez pA, tak text je v rovine pohladu a je citatelny horizontalne, pripadne zprava
            // podobne ako sme to robili pre Plates v SystemComponent Viewer, mozno by sa dalo z toho nieco pouzit aby by sme celu tuto ulohu pre koty zuzili na 2D problem 
            // ak uz vieme v akej rovine GCS kreslime

            if (Direction.Z == -1)
            {
                iVectorOverFactor_LCS = 1;
                iVectorUpFactor_LCS = 1;
            }
            else
            {
                iVectorOverFactor_LCS = -1;
                iVectorUpFactor_LCS = -1;
            }
            
            float fOffsetFromMainLine;

            if (bTextInside)
                fOffsetFromMainLine = 0.1f; // Mezera medzi ciarou a textom (kladna - text nad ciarou (+y), zaporna, text pod ciarou (-y))
            else
                fOffsetFromMainLine = -0.1f;

            m_PointText = new Point3D()
            {
                X = 0.5 * m_fMainLineLength,
                Y = m_DimensionMainLinePositionIncludingOffset + fOffsetFromMainLine,
                Z = 0
            };
        }
        
        public void SetPoints()
        {
            //m_Point1_MainLine = new Point3D()
            //{
            //    X = m_PointStart.X + Direction.X * DimensionMainLineDistance,
            //    Y = m_PointStart.Y + Direction.Y * DimensionMainLineDistance,
            //    Z = m_PointStart.Z + Direction.Z * DimensionMainLineDistance,
            //};

            //m_Point2_MainLine = new Point3D()
            //{
            //    X = m_PointEnd.X + Direction.X * DimensionMainLineDistance,
            //    Y = m_PointEnd.Y + Direction.Y * DimensionMainLineDistance,
            //    Z = m_PointEnd.Z + Direction.Z * DimensionMainLineDistance,
            //};

            m_Point1_MainLine = new Point3D()
            {
                X = m_PointStart.X + Direction.X * (ExtensionLine1Length - ExtensionLines_OffsetBehindMainLine),
                Y = m_PointStart.Y + Direction.Y * (ExtensionLine1Length - ExtensionLines_OffsetBehindMainLine),
                Z = m_PointStart.Z + Direction.Z * (ExtensionLine1Length - ExtensionLines_OffsetBehindMainLine),
            };

            m_Point2_MainLine = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * (ExtensionLine2Length - ExtensionLines_OffsetBehindMainLine),
                Y = m_PointEnd.Y + Direction.Y * (ExtensionLine2Length - ExtensionLines_OffsetBehindMainLine),
                Z = m_PointEnd.Z + Direction.Z * (ExtensionLine2Length - ExtensionLines_OffsetBehindMainLine),
            };

            //m_PointStartL2 = new Point3D()
            //{
            //    X = m_PointStart.X + Direction.X * DimensionLinesLength,
            //    Y = m_PointStart.Y + Direction.Y * DimensionLinesLength,
            //    Z = m_PointStart.Z + Direction.Z * DimensionLinesLength,
            //};

            //m_PointEndL2 = new Point3D()
            //{
            //    X = m_PointEnd.X + Direction.X * DimensionLinesLength,
            //    Y = m_PointEnd.Y + Direction.Y * DimensionLinesLength,
            //    Z = m_PointEnd.Z + Direction.Z * DimensionLinesLength,
            //};
        }
        public void SetPoints2()
        {
            // TO Ondrej - tu by som Ti poradil ujasnit, ci su pre vsetky body suradnice v GCS alebo v LCS
            // Myslim ze by mali byt v LCS
            // Vector Direction a vazbu na m_PointStart a m_PointEnd by som tu uplne zrusil

            // pA                         pE
            // * LCS plane XY [0,0,0]     * [mainLineLength, 0, 0]
            //
            // |                          |
            // |                          |
            // |           value          |
            // |/________________________\|
            // |\                        /|

            m_Point1_ExtensionLine1 = new Point3D()
            {
                X = m_PointStart.X + Direction.X * (OffSetFromPoint),
                Y = m_PointStart.Y + Direction.Y * (OffSetFromPoint),
                Z = m_PointStart.Z + Direction.Z * (OffSetFromPoint),
            };
            m_Point2_ExtensionLine1 = new Point3D()
            {
                X = m_PointStart.X + Direction.X * (OffSetFromPoint + ExtensionLine1Length),
                Y = m_PointStart.Y + Direction.Y * (OffSetFromPoint + ExtensionLine1Length),
                Z = m_PointStart.Z + Direction.Z * (OffSetFromPoint + ExtensionLine1Length),
            };

            m_Point1_ExtensionLine2 = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * (OffSetFromPoint),
                Y = m_PointEnd.Y + Direction.Y * (OffSetFromPoint),
                Z = m_PointEnd.Z + Direction.Z * (OffSetFromPoint),
            };
            m_Point2_ExtensionLine2 = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * (OffSetFromPoint + ExtensionLine2Length),
                Y = m_PointEnd.Y + Direction.Y * (OffSetFromPoint + ExtensionLine2Length),
                Z = m_PointEnd.Z + Direction.Z * (OffSetFromPoint + ExtensionLine2Length),
            };


            m_Point1_MainLine = new Point3D()
            {
                X = m_PointStart.X + Direction.X * (ExtensionLine1Length - ExtensionLines_OffsetBehindMainLine),
                Y = m_PointStart.Y + Direction.Y * (ExtensionLine1Length - ExtensionLines_OffsetBehindMainLine),
                Z = m_PointStart.Z + Direction.Z * (ExtensionLine1Length - ExtensionLines_OffsetBehindMainLine),
            };

            m_Point2_MainLine = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * (ExtensionLine2Length - ExtensionLines_OffsetBehindMainLine),
                Y = m_PointEnd.Y + Direction.Y * (ExtensionLine2Length - ExtensionLines_OffsetBehindMainLine),
                Z = m_PointEnd.Z + Direction.Z * (ExtensionLine2Length - ExtensionLines_OffsetBehindMainLine),
            };
        }

        public Model3DGroup GetDimensionModelNew(System.Windows.Media.Color color, float fLineCylinderRadius)
        {
            // Zakladny model koty - hlavna kotovacia ciara - smer X, vynasacie ciary - smer Y
            // TEXT by som kreslil v LCS koty do roviny XY a potom ho otacal s kotou (system potom mozeme pouzit aj pre popisy prutov, staci vyplnut zobrazenie koty a ostane len text)

            Model3DGroup model_gr = new Model3DGroup();
            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color));

            float fMainLineLength = (float)Math.Sqrt((float)Math.Pow(m_Point2_MainLine.X - m_Point1_MainLine.X, 2f) + (float)Math.Pow(m_Point2_MainLine.Y - m_Point1_MainLine.Y, 2f) + (float)Math.Pow(m_Point2_MainLine.Z - m_Point1_MainLine.Z, 2f));
            
            // Main Line - uvazuje sa ze [0,0,0] je v kotovanom bode
            // Main line
            // Default tip (cone height is 20% from length)
            Objects_3D.StraightLineArrow3D arrow = new Objects_3D.StraightLineArrow3D(new Point3D(0, DimensionMainLinePositionIncludingOffset,0), fMainLineLength, fLineCylinderRadius, 0, true);
            GeometryModel3D model = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions = arrow.ArrowPoints;
            mesh.TriangleIndices = arrow.GetArrowIndices();
            model.Geometry = mesh;
            model.Material = material;
            model_gr.Children.Add(model);  // Add straight arrow

            // Add other lines
            // TODO - Zapracovat nastavitelnu dlzku a nastavitelny offset pre extension lines
            short NumberOfCirclePoints = 16 + 1; // Toto by malo byt rovnake ako u arrow, je potrebne to zjednotit, pridany jeden bod pre stred

            // TO Ondrej - toto treba prerobit, ja kreslim tuto kotu v rovine XY a potom ju chcem otacat a presuvat podla potreby
            float fExtensionLine1_Length = (float)ExtensionLine1Length;
            float fExtensionLine2_Length = (float)ExtensionLine2Length;

            float fExtensionLine1_OffsetBehindMainLine = (float)ExtensionLines_OffsetBehindMainLine;
            float fExtensionLine2_OffsetBehindMainLine = (float)ExtensionLines_OffsetBehindMainLine;

            // Extension line 1 (start)
            //model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, m_DimensionMainLinePositionIncludingOffset - fExtensionLine1_OffsetBehindMainLine, 0), NumberOfCirclePoints, fLineCylinderRadius, fExtensionLine1_Length, material, 1));
            model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(m_Point1_ExtensionLine1, NumberOfCirclePoints, fLineCylinderRadius, fExtensionLine1_Length, material, 1));

            // Extension line 2 (end)
            //model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(fMainLineLength, m_DimensionMainLinePositionIncludingOffset - fExtensionLine2_OffsetBehindMainLine, 0), NumberOfCirclePoints, fLineCylinderRadius, fExtensionLine2_Length, material, 1));
            model_gr.Children.Add(CVolume.CreateM_G_M_3D_Volume_Cylinder(m_Point1_ExtensionLine2, NumberOfCirclePoints, fLineCylinderRadius, fExtensionLine2_Length, material, 1));

            // TO ONDREJ - tu treba pridat dalsie transformacie ak chceme kotu ako celok posuvat alebo otacat atd, trosku sa s tym treba pohrat, 
            // zobecnit tak aby sa dali kreslit aj sikme koty napriklad na rafteroch pri pohlade na ram zpredu
            // TODO - toto otacanie by chcelo nejako sprehladnit a zjednodusit - kotu vyrobim v rovine XY a potom ju mozeme otacat tak aby bola v rovine pohladu, 
            // moze sa stat ze je pootocena okolo osy smerujucej kolmo na monitor (oznacena pre view Y - vid member description)
            // Skus porozmyslat a navrhnut nejaky univerzalny system ako budeme tie koty a ich texty umiestnovat


            //TO Mato - ty kokso ja som taky zmateny z toho ze koniec
            // tieto transformacie asi budem potrebovat vysvetlit,ze co sa tu vlastne deje

            // TO Ondrej
            /*
            Tie parametre na ktore si sa pytal by mali zmanenat ci chcem kotu v pohlade kreslit nahor, nadol, nalavo, napravo od kotovanych bodov.
            Mozno sa do da urobit aj automaticky smerom od stredu modelu.
            Parameter Direction som tam nechal este z Tvojho kotu.Direction ma asi zmysel len,
            ak je kota rovnobezna s niektorou osou, ak je kota v rovine pootocena tak asi nema zmysel.

            Tieto rotacie nizie robia len to ze kotu v LCS tj rovine XY, main line v smere X a extension lines vykreslene v smere zapornej osi Y
            rozne rotujem a otacam a preklapam, aby sa v GCS dostala na spravnu poziciu.

            Myslim ze sa to da zjednodusit. Toto su len moje experimenty co som skusal
            // Potrebujeme jasne zadefinovat vztah medzi osovym systemom koty a poziciou koty v GCS 
            (tj. v akej ma byt rovine a z ktorej strany sa na nu pozerame, ci ma byt "nad, pod, vlavo, vpravo" od kotovanych bodov atd)
            */

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

            if (Direction.X == 0 && MathF.Equals(m_PointStart.X, m_PointEnd.X)) // Kota v smere Y
            {
                // About Z
                AxisAngleRotation3D axisAngleRotation3dZ = new AxisAngleRotation3D();
                axisAngleRotation3dZ.Axis = new Vector3D(0, 0, 1);
                axisAngleRotation3dZ.Angle = 90;
                rotateZ.Rotation = axisAngleRotation3dZ;
            }

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
