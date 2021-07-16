using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using MATERIAL;
using DATABASE;
using DATABASE.DTO;
using _3DTools;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace BaseClasses
{
    [Serializable]
    public class CFlashing : CEntity3D
    {
        private int m_DatabaseID;

        private int m_type_ID;
        private string m_type_Name;
        private int m_group_ID;
        private string m_group_Name;
        private string m_elements_snakeModel_deg_mm;
        private double[] m_ArrElements_snakeModel_deg_mm;
        private double m_width_total;
        private double m_thickness;
        private double m_mass_kg_lm;
        private double m_price_PPLM_NZD;
        private double m_price_PPKG_NZD;
        private string m_note;

        private List<Point> m_PointsOutline;
        private List<Point3D> m_Points3D;

        private CDimension[] m_dimensions; // Pole kot pre Flashing v 2D

        private double m_length;

        private Color m_FL_Color;

        public int Database_ID
        {
            get
            {
                return m_DatabaseID;
            }

            set
            {
                m_DatabaseID = value;
            }
        }

        public int Type_ID
        {
            get
            {
                return m_type_ID;
            }

            set
            {
                m_type_ID = value;
            }
        }

        public string Type_Name
        {
            get
            {
                return m_type_Name;
            }

            set
            {
                m_type_Name = value;
            }
        }

        public int Group_ID
        {
            get
            {
                return m_group_ID;
            }

            set
            {
                m_group_ID = value;
            }
        }

        public string Group_Name
        {
            get
            {
                return m_group_Name;
            }

            set
            {
                m_group_Name = value;
            }
        }

        public string Elements_snakeModel_deg_mm
        {
            get
            {
                return m_elements_snakeModel_deg_mm;
            }

            set
            {
                m_elements_snakeModel_deg_mm = value;
            }
        }

        public double[] ArrElements_snakeModel_deg_mm
        {
            get
            {
                return m_ArrElements_snakeModel_deg_mm;
            }

            set
            {
                m_ArrElements_snakeModel_deg_mm = value;
            }
        }

        public double Width_total
        {
            get
            {
                return m_width_total;
            }

            set
            {
                m_width_total = value;
            }
        }

        public double Thickness
        {
            get
            {
                return m_thickness;
            }

            set
            {
                m_thickness = value;
            }
        }

        public double Mass_kg_lm
        {
            get
            {
                return m_mass_kg_lm;
            }

            set
            {
                m_mass_kg_lm = value;
            }
        }

        public double Price_PPLM_NZD
        {
            get
            {
                return m_price_PPLM_NZD;
            }

            set
            {
                m_price_PPLM_NZD = value;
            }
        }

        public double Price_PPKG_NZD
        {
            get
            {
                return m_price_PPKG_NZD;
            }

            set
            {
                m_price_PPKG_NZD = value;
            }
        }

        public string Note
        {
            get
            {
                return m_note;
            }

            set
            {
                m_note = value;
            }
        }

        public List<Point> PointsOutline
        {
            get
            {
                return m_PointsOutline;
            }

            set
            {
                m_PointsOutline = value;
            }
        }

        public List<Point3D> Points3D
        {
            get
            {
                return m_Points3D;
            }

            set
            {
                m_Points3D = value;
            }
        }

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

        public double Length
        {
            get
            {
                return m_length;
            }

            set
            {
                m_length = value;
            }
        }

        public Color FL_Color
        {
            get
            {
                return m_FL_Color;
            }

            set
            {
                m_FL_Color = value;
            }
        }

        public CFlashing()
        { }

        public CFlashing(string sPrefix_temp, float length_temp, Color color_temp)
        {
            m_Mat = new CMat_03_00("G550‡"); // TODO - pripravit databazu materialov pre flashing

            Name = sPrefix_temp;
            m_length = length_temp;
            m_FL_Color = color_temp;

            CFlashingItemProperties flashingProperties = CFlashingManager.GetFlashingProperties(sPrefix_temp);
            SetFlashingValuesFromDatabase(flashingProperties);

            // Suradnice bodov prierezu flashing v 2D
            m_PointsOutline = GetOutlinePointsCoordinates();

            // Suradnice bodov v 3D
            FillPoints3D();

            // Pole kot
            m_dimensions = GetDimensions().ToArray();
        }

        public void SetFlashingValuesFromDatabase(CFlashingItemProperties properties)
        {
            //NumberFormatInfo nfi = new NumberFormatInfo();
            //nfi.NumberDecimalSeparator = ".";

            m_DatabaseID = properties.ID;

            m_type_ID = properties.Type_ID;
            m_type_Name = properties.Type_Name;
            m_group_ID = properties.Group_ID;
            m_group_Name = properties.Group_Name;
            m_elements_snakeModel_deg_mm = properties.Elements_snakeModel_deg_mm;
            m_ArrElements_snakeModel_deg_mm = properties.ArrElements_snakeModel_deg_mm;
            m_width_total = properties.Width_total;
            m_thickness = properties.Thickness;
            m_mass_kg_lm = properties.Mass_kg_lm;
            m_price_PPLM_NZD = properties.Price_PPLM_NZD;
            m_price_PPKG_NZD = properties.Price_PPKG_NZD;
            m_note = properties.Note;
        }

        public void UpdateAllValuesOnPrefixChange()
        {
            CFlashingItemProperties flashingProperties = CFlashingManager.GetFlashingProperties(Name); // !!! Ak je to int tak sa uvazuje ID, ak je to string tak prefix
            SetFlashingValuesFromDatabase(flashingProperties);

            System.Diagnostics.Trace.WriteLine("UpdateAllValuesOnGaugeChange call " + DateTime.Now.ToLongTimeString());
        }

        List<Point> GetOutlinePointsCoordinates()
        {
            if (m_ArrElements_snakeModel_deg_mm != null && m_ArrElements_snakeModel_deg_mm.Length > 2)
            {
                List<Point> list = new List<Point>();

                // V zozname double je neparne poradove cislo indexu reprezentovane ako uhol [deg] odklonu od x, resp. od predchadzajuceho elementu
                // V zozname double je parne poradove cislo indexu reprezentovane dlzka elemetnu [mm]

                float fCurrentAngle_deg = 0;
                Point currentPoint = new Point(0, 0);

                list.Add(currentPoint); // Inicializacny bod [0,0]

                int elemIndex = 0;

                for (int i = 0; i < m_ArrElements_snakeModel_deg_mm.Length / 2; i++)
                {
                    float elemAngle_deg = (float)m_ArrElements_snakeModel_deg_mm[elemIndex];
                    float elemLength = (float)m_ArrElements_snakeModel_deg_mm[elemIndex + 1] / 1000;

                    fCurrentAngle_deg += elemAngle_deg;

                    double x = currentPoint.X + MATH.Geom2D.GetPositionX_deg(elemLength, fCurrentAngle_deg);
                    double y = currentPoint.Y + MATH.Geom2D.GetPositionY_CCW_deg(elemLength, fCurrentAngle_deg); // Counter-clockwise (right-hand system)

                    list.Add(new Point(x, y)); // Add new point to the collection

                    // Update last point coordinates
                    currentPoint.X = x;
                    currentPoint.Y = y;

                    elemIndex += 2;
                }

                return list;
            }
            else
                return null;
        }

        List<CDimension> GetDimensions()
        {
            if (m_PointsOutline == null || m_PointsOutline.Count < 1)
                return null;

            Point modelCenter = Drawing2D.CalculateModelCenter(m_PointsOutline.ToArray());

            List<CDimension> list = new List<CDimension>();

            for (int i = 0; i < m_PointsOutline.Count - 1; i++) // Pocet kot je pocet bodov - 1
            {
                Point start = m_PointsOutline[i];
                Point end = m_PointsOutline[i + 1];

                double length = MATH.Geom2D.GetPointDistanceDouble(start, end);

                if (length > 0.005) // Zobrazime kotu ak je dlzka viac nez 5 mm, TODO - urobit nastavitelne
                    list.Add(new CDimensionLinear(modelCenter, (length * 1000).ToString("F0"), start, end, true, true, 15));
            }

            return list;
        }

        public void FillPoints3D()
        {
            if (m_PointsOutline == null) return;

            m_Points3D = new List<Point3D>();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // Front Side
            for (int i = 0; i < m_PointsOutline.Count; i++)
            {
                pi = new Point3D(0, m_PointsOutline[i].X, m_PointsOutline[i].Y);
                m_Points3D.Add(pi);
            }

            // Back Side
            for (int i = 0; i < m_PointsOutline.Count; i++)
            {
                pi = new Point3D(m_length, m_PointsOutline[i].X, m_PointsOutline[i].Y);
                m_Points3D.Add(pi);
            }
        }

        public ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            // Front Side
            for (int i = 0; i < m_PointsOutline.Count - 1; i++)
            {
                // Add points
                wireFrame.Points.Add(m_Points3D[i]);
                wireFrame.Points.Add(m_Points3D[i + 1]);
            }

            // Back Side
            for (int i = 0; i < m_PointsOutline.Count - 1; i++)
            {
                // Add points
                wireFrame.Points.Add(m_Points3D[m_PointsOutline.Count + i]);
                wireFrame.Points.Add(m_Points3D[m_PointsOutline.Count + i + 1]);
            }

            // Lateral
            for (int i = 0; i < m_PointsOutline.Count; i++)
            {
                // Add points
                wireFrame.Points.Add(m_Points3D[i]);
                wireFrame.Points.Add(m_Points3D[m_PointsOutline.Count + i]);
            }

            return wireFrame;
        }

        public Model3DGroup CreateModel3DGroup()
        {
            Model3DGroup modelGroup = new Model3DGroup();
            SolidColorBrush brush = new SolidColorBrush(FL_Color);
            DiffuseMaterial mat = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D();
            MeshGeometry3D meshGeom3D = new MeshGeometry3D();

            meshGeom3D.Positions = new Point3DCollection(m_Points3D);

            Int32Collection TriangleIndices = new Int32Collection();

            // Shell Surface
            for (int i = 0; i < m_PointsOutline.Count - 1; i++)
            {
                AddRectangleIndices_CCW_1234(TriangleIndices, i, i + 1, m_PointsOutline.Count + i + 1, m_PointsOutline.Count + i);
            }

            meshGeom3D.TriangleIndices = TriangleIndices;
            model.Geometry = meshGeom3D; // Set mesh to model
            model.Material = mat;
            model.BackMaterial = mat; // Back Material
            modelGroup.Children.Add(model);
            return modelGroup;
        }

        public void CalculateFlashingLimits(out double fTempMax_X, out double fTempMin_X, out double fTempMax_Y, out double fTempMin_Y, out double fTempMax_Z, out double fTempMin_Z)
        {
            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;
            fTempMax_Z = float.MinValue;
            fTempMin_Z = float.MaxValue;

            if (m_PointsOutline != null) // Some cross-section points exist
            {
                // Maximum X - coordinate
                fTempMax_X = Length;
                // Minimum X - coordinate
                fTempMin_X = 0;

                for (int i = 0; i < m_PointsOutline.Count; i++)
                {
                    // Maximum Y - coordinate
                    if (m_PointsOutline[i].X > fTempMax_Y)
                        fTempMax_Y = m_PointsOutline[i].X;
                    // Minimum Y - coordinate
                    if (m_PointsOutline[i].X < fTempMin_Y)
                        fTempMin_Y = m_PointsOutline[i].X;
                    // Maximum Z - coordinate
                    if (m_PointsOutline[i].Y > fTempMax_Z)
                        fTempMax_Z = m_PointsOutline[i].Y;
                    // Minimum Z - coordinate
                    if (m_PointsOutline[i].Y < fTempMin_Z)
                        fTempMin_Z = m_PointsOutline[i].Y;
                }
            }
        }

        // REFAKTOROVAT !!!!!!

        // Draw Rectangle / Add rectangle indices - countrer-clockwise CCW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,4,3,2
        public static void AddRectangleIndices_CCW_1234(Int32Collection Indices,
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
    }
}
