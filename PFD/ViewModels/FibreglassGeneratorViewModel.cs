using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace PFD
{
    public class FibreglassGeneratorViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private string m_Side;
        private float m_X;
        private float m_Y;
        private float m_Length;
        
        private List<string> m_Sides;
        private List<float> m_XValues;
        private string m_Periodicity;
        private List<string> m_PeriodicityValues;

        private EModelType_FS m_ModelType;
        private float m_ModelTotalLengthFront;
        private float m_ModelTotalLengthLeft;
        private float m_CladdingWidthModular_Wall;
        private float m_CladdingWidthModular_Roof;

        private float m_RoofPitch_deg;
        private double m_Height_1_final_edge_LR_Wall;
        private double m_Height_2_final_edge_LR_Wall;
        private double m_Height_1_final_edge_FB_Wall;
        private double m_Height_2_final_edge_FB_Wall;

        private double m_BottomEdge_z;
        private double m_AdditionalOffsetWall;
        private double m_MaxHeight;
        private float m_RoofEdgeOverhang_X;
        private float m_RoofEdgeOverhang_Y;

        private float m_Y2;
        private float m_Y3;
        private float m_Y4;
        private float m_Y5;
        private float m_Length2;
        private float m_Length3;
        private float m_Length4;
        private float m_Length5;

        private bool m_GenerateRaster;
        private int m_RowsCount;
        private List<int> m_RowsCountValues;
        private bool m_EqualSpacing;
        private float m_Spacing;
        private bool m_EnableVariableLengths;

        private bool m_AddFibreglass;
        private bool m_DeleteFibreglass;

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public string Side
        {
            get
            {
                return m_Side;
            }

            set
            {
                m_Side = value;
                InitXValues();
                NotifyPropertyChanged("Side");
            }
        }

        public float X
        {
            get
            {
                return m_X;
            }

            set
            {
                if (value < 0.0 || value > 300.00)
                    throw new ArgumentException("Sheet position must be between 0 and 300 [m]");
                m_X = value;
                NotifyPropertyChanged("X");
            }
        }

        public float Y
        {
            get
            {
                return m_Y;
            }

            set
            {
                if (value < 0.00 || value > 50.00)
                    throw new ArgumentException("Sheet position must be between 0.0 and 50 [m]");
                m_Y = value;
                                
                if (EqualSpacing) // Prepocitat suradnice
                {
                    Y2 = m_Y + Spacing;
                    Y3 = m_Y2 + Spacing;
                    Y4 = m_Y3 + Spacing;
                    Y5 = m_Y4 + Spacing;
                }
                NotifyPropertyChanged("Y");
            }
        }

        public float Length
        {
            get
            {
                return m_Length;
            }

            set
            {
                if (value < 0.0 || value > 6.00)
                    throw new ArgumentException("Sheet length must be between 0 and 6 [m]");
                m_Length = value;

                if (!m_EnableVariableLengths) // Nastavit dlzky
                {
                    Length2 = m_Length;
                    Length3 = m_Length;
                    Length4 = m_Length;
                    Length5 = m_Length;
                }

                NotifyPropertyChanged("Length");
            }
        }

        public List<string> Sides
        {
            get
            {
                return m_Sides;
            }

            set
            {
                m_Sides = value;
                NotifyPropertyChanged("Sides");
            }
        }

        public EModelType_FS ModelType
        {
            get
            {
                return m_ModelType;
            }

            set
            {
                m_ModelType = value;
                InitSides(); 
            }
        }

        public float CladdingWidthModular_Wall
        {
            get
            {
                return m_CladdingWidthModular_Wall;
            }

            set
            {
                m_CladdingWidthModular_Wall = value;
            }
        }

        public float CladdingWidthModular_Roof
        {
            get
            {
                return m_CladdingWidthModular_Roof;
            }

            set
            {
                m_CladdingWidthModular_Roof = value;
            }
        }

        public float ModelTotalLengthFront
        {
            get
            {
                return m_ModelTotalLengthFront;
            }

            set
            {
                m_ModelTotalLengthFront = value;
            }
        }

        public float ModelTotalLengthLeft
        {
            get
            {
                return m_ModelTotalLengthLeft;
            }

            set
            {
                m_ModelTotalLengthLeft = value;
            }
        }

        public List<float> XValues
        {
            get
            {
                return m_XValues;
            }

            set
            {
                m_XValues = value;
            }
        }

        public string Periodicity
        {
            get
            {
                return m_Periodicity;
            }

            set
            {
                m_Periodicity = value;
                NotifyPropertyChanged("Periodicity");
            }
        }

        public List<string> PeriodicityValues
        {
            get
            {
                return m_PeriodicityValues;
            }

            set
            {
                m_PeriodicityValues = value;
                NotifyPropertyChanged("PeriodicityValues");
            }
        }

        public bool AddFibreglass
        {
            get
            {
                return m_AddFibreglass;
            }

            set
            {
                m_AddFibreglass = value;
            }
        }

        public bool DeleteFibreglass
        {
            get
            {
                return m_DeleteFibreglass;
            }

            set
            {
                m_DeleteFibreglass = value;
            }
        }

        public float Y2
        {
            get
            {
                return m_Y2;
            }

            set
            {
                if (!m_EqualSpacing && value < (m_Y + m_Length) || value > 50.00)
                    throw new ArgumentException("Sheet position must be between " + (m_Y + m_Length).ToString("F3") + " and 50 [m]");
                m_Y2 = value;
                NotifyPropertyChanged("Y2");
            }
        }

        public float Y3
        {
            get
            {
                return m_Y3;
            }

            set
            {
                if (!m_EqualSpacing && value < (m_Y2 + m_Length2) || value > 50.00)
                    throw new ArgumentException("Sheet position must be between " + (m_Y2 + m_Length2).ToString("F3") + " and 50 [m]");
                m_Y3 = value;
                NotifyPropertyChanged("Y3");
            }
        }

        public float Y4
        {
            get
            {
                return m_Y4;
            }

            set
            {
                if (!m_EqualSpacing && value < (m_Y3 + m_Length3) || value > 50.00)
                    throw new ArgumentException("Sheet position must be between " + (m_Y3 + m_Length3).ToString("F3") + " and 50 [m]");
                m_Y4 = value;
                NotifyPropertyChanged("Y4");
            }
        }

        public float Y5
        {
            get
            {
                return m_Y5;
            }

            set
            {
                if (!m_EqualSpacing && value < (m_Y4 + m_Length4) || value > 50.00)
                    throw new ArgumentException("Sheet position must be between " + (m_Y4 + m_Length4).ToString("F3") + " and 50 [m]");
                m_Y5 = value;
                NotifyPropertyChanged("Y5");
            }
        }

        public float Length2
        {
            get
            {
                return m_Length2;
            }

            set
            {
                if (value < 0.50 || value > 6.00)
                    throw new ArgumentException("Sheet length must be between 0.5 and 6 [m]");
                m_Length2 = value;
                NotifyPropertyChanged("Length2");
            }
        }

        public float Length3
        {
            get
            {
                return m_Length3;
            }

            set
            {
                if (value < 0.50 || value > 6.00)
                    throw new ArgumentException("Sheet length must be between 0.5 and 6 [m]");
                m_Length3 = value;
                NotifyPropertyChanged("Length3");
            }
        }

        public float Length4
        {
            get
            {
                return m_Length4;
            }

            set
            {
                if (value < 0.50 || value > 6.00)
                    throw new ArgumentException("Sheet length must be between 0.5 and 6 [m]");
                m_Length4 = value;
                NotifyPropertyChanged("Length4");
            }
        }

        public float Length5
        {
            get
            {
                return m_Length5;
            }

            set
            {
                if (value < 0.50 || value > 6.00)
                    throw new ArgumentException("Sheet length must be between 0.5 and 6 [m]");
                m_Length5 = value;
                NotifyPropertyChanged("Length5");
            }
        }

        public bool GenerateRaster
        {
            get
            {
                return m_GenerateRaster;
            }

            set
            {
                m_GenerateRaster = value;
                NotifyPropertyChanged("GenerateRaster");
            }
        }

        public int RowsCount
        {
            get
            {
                return m_RowsCount;
            }

            set
            {
                m_RowsCount = value;
                NotifyPropertyChanged("RowsCount");
            }
        }

        public List<int> RowsCountValues
        {
            get
            {
                if (m_RowsCountValues == null) m_RowsCountValues = new List<int>() { 2, 3, 4, 5 };
                return m_RowsCountValues;
            }

            set
            {
                m_RowsCountValues = value;
                NotifyPropertyChanged("RowsCountValues");
            }
        }

        public bool EqualSpacing
        {
            get
            {
                return m_EqualSpacing;
            }

            set
            {
                m_EqualSpacing = value;

                if (m_EqualSpacing) // Prepocitat suradnice
                {
                    Y2 = m_Y + m_Spacing;
                    Y3 = m_Y2 + m_Spacing;
                    Y4 = m_Y3 + m_Spacing;
                    Y5 = m_Y4 + m_Spacing;
                }

                NotifyPropertyChanged("EqualSpacing");
            }
        }

        public float Spacing
        {
            get
            {
                return m_Spacing;
            }

            set
            {
                if (value < 0.0 || value > 40.00)
                    throw new ArgumentException("Spacing of sheet positions must be between 0 and 40 [m]");
                m_Spacing = value;

                if (m_EqualSpacing) // Prepocitat suradnice
                {
                    Y2 = m_Y + m_Spacing;
                    Y3 = m_Y2 + m_Spacing;
                    Y4 = m_Y3 + m_Spacing;
                    Y5 = m_Y4 + m_Spacing;
                }

                NotifyPropertyChanged("Spacing");
            }
        }

        public bool EnableVariableLengths
        {
            get
            {
                return m_EnableVariableLengths;
            }

            set
            {
                m_EnableVariableLengths = value;

                if (!m_EnableVariableLengths) //changed to false
                {   
                    Length2 = Length;
                    Length3 = Length;
                    Length4 = Length;
                    Length5 = Length;
                }
                NotifyPropertyChanged("EnableVariableLengths");
            }
        }

        public float RoofPitch_deg
        {
            get
            {
                return m_RoofPitch_deg;
            }

            set
            {
                m_RoofPitch_deg = value;
            }
        }

        public double Height_1_final_edge_LR_Wall
        {
            get
            {
                return m_Height_1_final_edge_LR_Wall;
            }

            set
            {
                m_Height_1_final_edge_LR_Wall = value;
            }
        }

        public double Height_2_final_edge_LR_Wall
        {
            get
            {
                return m_Height_2_final_edge_LR_Wall;
            }

            set
            {
                m_Height_2_final_edge_LR_Wall = value;
            }
        }

        public double Height_1_final_edge_FB_Wall
        {
            get
            {
                return m_Height_1_final_edge_FB_Wall;
            }

            set
            {
                m_Height_1_final_edge_FB_Wall = value;
            }
        }

        public double Height_2_final_edge_FB_Wall
        {
            get
            {
                return m_Height_2_final_edge_FB_Wall;
            }

            set
            {
                m_Height_2_final_edge_FB_Wall = value;
            }
        }

        public double BottomEdge_z
        {
            get
            {
                return m_BottomEdge_z;
            }

            set
            {
                m_BottomEdge_z = value;
            }
        }

        public double AdditionalOffsetWall
        {
            get
            {
                return m_AdditionalOffsetWall;
            }

            set
            {
                m_AdditionalOffsetWall = value;
            }
        }

        public double MaxHeight
        {
            get
            {
                return m_MaxHeight;
            }

            set
            {
                m_MaxHeight = value;
            }
        }

        public float RoofEdgeOverhang_X
        {
            get
            {
                return m_RoofEdgeOverhang_X;
            }

            set
            {
                m_RoofEdgeOverhang_X = value;
            }
        }

        public float RoofEdgeOverhang_Y
        {
            get
            {
                return m_RoofEdgeOverhang_Y;
            }

            set
            {
                m_RoofEdgeOverhang_Y = value;
            }
        }



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FibreglassGeneratorViewModel(EModelType_FS modelType, float lengthFront, float lengthLeft, 
            float roofPitch_deg,
            double height_1_LR_wall, double height_2_LR_wall,
            double height_1_FB_wall, double height_2_FB_wall,
            double bottomEdgeOffset, double additionalOffset_wall,
            float roofEdgeOverhang_X, float roofEdgeOverhang_Y, 
            double widthModularWall, double widthModularRoof)
        {
            IsSetFromCode = true;

            AddFibreglass = false;
            DeleteFibreglass = false;

            ModelType = modelType;

            ModelTotalLengthFront = lengthFront;
            ModelTotalLengthLeft = lengthLeft;
            CladdingWidthModular_Wall = (float)widthModularWall;
            CladdingWidthModular_Roof = (float)widthModularRoof;

            RoofPitch_deg = roofPitch_deg;
            Height_1_final_edge_LR_Wall = height_1_LR_wall;
            Height_2_final_edge_LR_Wall = height_2_LR_wall;
            Height_1_final_edge_FB_Wall = height_1_FB_wall;
            Height_2_final_edge_FB_Wall = height_2_FB_wall;

            BottomEdge_z = bottomEdgeOffset;
            AdditionalOffsetWall = additionalOffset_wall;
            RoofEdgeOverhang_X = roofEdgeOverhang_X;
            RoofEdgeOverhang_Y = roofEdgeOverhang_Y;

            Spacing = 2.0f; // Malo by byt vacsie ako default Length

            Y = 0.6f;
            Y2 = m_Y + Spacing;
            Y3 = m_Y2 + Spacing;
            Y4 = m_Y3 + Spacing;
            Y5 = m_Y4 + Spacing;
            Length = 1.8f;
            Length2 = 1.8f;
            Length3 = 1.8f;
            Length4 = 1.8f;
            Length5 = 1.8f;

            GenerateRaster = false;
            RowsCount = 3;
            EqualSpacing = true;
            EnableVariableLengths = false;

            InitXValues();

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitSides()
        {
            List<string> sides = new List<string>();
            if (ModelType == EModelType_FS.eKitsetGableRoofEnclosed)
            {
                sides = new List<string>() { "Left", "Right", "Front", "Back", "Roof-Left Side", "Roof-Right Side" };
            }
            else if (ModelType == EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                sides = new List<string>() { "Left", "Right", "Front", "Back", "Roof" };
            }
            else
            {

            }

            Sides = sides;

            if (Sides.Count != 0) Side = Sides.First();
        }

        private void InitXValues()
        {
            List<float> x_values = new List<float>();
            float x = 0f;
            int i = 0;
            int maxCount = 100; //aby sme nespravili virus pri zlej inicializacii
            if (Side == "Left" || Side == "Right")
            {
                while (x <= ModelTotalLengthLeft)
                {
                    x_values.Add(x);
                    x += CladdingWidthModular_Wall;
                    i++;
                    if (i > maxCount) return;
                }
            }
            else if (Side == "Front" || Side == "Back")
            {
                while (x <= ModelTotalLengthFront)
                {
                    x_values.Add(x);
                    x += CladdingWidthModular_Wall;
                    i++;
                    if (i > maxCount) return;
                }
            }
            else if (Side == "Roof" || Side == "Roof-Left Side" || Side == "Roof-Right Side")
            {
                while (x <= ModelTotalLengthLeft)
                {
                    x_values.Add(x);
                    x += CladdingWidthModular_Roof;
                    i++;
                    if (i > maxCount) return;
                }
            }
            else
            {
                throw new Exception("Side not recognized: " + Side);
            }

            XValues = x_values;

            if (XValues.Count != 0) X = XValues.First();

            InitPeriodicity();
        }

        private void InitPeriodicity()
        {
            List<string> values = new List<string>();
            if (XValues.Count > 0) values.Add("Every Sheet");
            if (XValues.Count > 1) values.Add("Every Second");
            if (XValues.Count > 2) values.Add("Every Third");

            for (int i = 3; i < XValues.Count && i < 20; i++)
            {
                values.Add($"Every {i + 1}th");
            }

            PeriodicityValues = values;

            if (PeriodicityValues.Count != 0)
            {
                if(!PeriodicityValues.Contains(Periodicity)) Periodicity = PeriodicityValues.First();
            }
        }

        public FibreglassProperties GetFibreglass()
        {
            FibreglassProperties f = new FibreglassProperties();
            f.IsSetFromCode = true;

            f.ModelType = ModelType;
            f.ModelTotalLengthLeft = ModelTotalLengthLeft;
            f.ModelTotalLengthFront = ModelTotalLengthFront;
            f.CladdingWidthModular_Wall = CladdingWidthModular_Wall;
            f.CladdingWidthModular_Roof = CladdingWidthModular_Roof;

            f.RoofPitch_deg = RoofPitch_deg;
            f.Height_1_final_edge_LR_Wall = Height_1_final_edge_LR_Wall;
            f.Height_2_final_edge_LR_Wall = Height_2_final_edge_LR_Wall;
            f.Height_1_final_edge_FB_Wall = Height_1_final_edge_FB_Wall;
            f.Height_2_final_edge_FB_Wall = Height_2_final_edge_FB_Wall;
            f.BottomEdge_z = BottomEdge_z;
            f.AdditionalOffsetWall = AdditionalOffsetWall;
            f.RoofEdgeOverhang_X = RoofEdgeOverhang_X;
            f.RoofEdgeOverhang_Y = RoofEdgeOverhang_Y;
            
            f.Sides = Sides;
            f.Side = Side;
            f.XValues = XValues;
            f.Y = Y;
            f.Length = Length;
                        
            return f;
        }
    }
}