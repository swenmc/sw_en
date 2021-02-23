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
                m_Y = value;
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
                m_Length = value;
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
                m_Spacing = value;
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
                NotifyPropertyChanged("EnableVariableLengths");
            }
        }



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FibreglassGeneratorViewModel(EModelType_FS modelType, float lengthFront, float lengthLeft, double widthModularWall, double widthModularRoof)
        {
            IsSetFromCode = true;

            AddFibreglass = false;
            DeleteFibreglass = false;

            ModelType = modelType;

            ModelTotalLengthFront = lengthFront;
            ModelTotalLengthLeft = lengthLeft;
            CladdingWidthModular_Wall = (float)widthModularWall;
            CladdingWidthModular_Wall = (float)widthModularRoof;

            Y = 0.6f;
            Y2 = 1.6f;
            Y3 = 2.6f;
            Y4 = 3.6f;
            Y5 = 4.6f;
            Length = 1.8f;
            Length2 = 1.8f;
            Length3 = 1.8f;
            Length4 = 1.8f;
            Length5 = 1.8f;

            GenerateRaster = false;
            RowsCount = 4;
            EqualSpacing = true;
            Spacing = 1.2f;
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

            if (PeriodicityValues.Count != 0) Periodicity = PeriodicityValues.First();
        }

        public FibreglassProperties GetFibreglass()
        {
            FibreglassProperties f = new FibreglassProperties();
            f.ModelType = ModelType;
            f.ModelTotalLengthLeft = ModelTotalLengthLeft;
            f.ModelTotalLengthFront = ModelTotalLengthFront;
            f.CladdingWidthModular_Wall = CladdingWidthModular_Wall;
            f.CladdingWidthModular_Roof = CladdingWidthModular_Roof;

            f.Sides = Sides;
            f.Side = Side;
            f.XValues = XValues;
            f.Y = Y;
            f.Length = Length;

            return f;
        }

    }
}