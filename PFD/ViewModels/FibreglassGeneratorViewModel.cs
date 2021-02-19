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
            Length = 1.8f;

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