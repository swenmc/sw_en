using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DATABASE;
using DATABASE.DTO;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class FibreglassProperties : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private string m_Side;
        private float m_X;
        private float m_Y;
        private float m_Length;

        private List<string> m_Sides;
        private List<float> m_XValues;

        private EModelType_FS m_ModelType;
        private float m_ModelTotalLengthFront;
        private float m_ModelTotalLengthLeft;
        private float m_CladdingWidthModular_Wall;
        private float m_CladdingWidthModular_Roof;

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
                InitXValuesAndSetX();
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

        public List<float> XValues
        {
            get
            {
                if (m_XValues == null) m_XValues = new List<float>();
                return m_XValues;
            }

            set
            {
                m_XValues = value;
                NotifyPropertyChanged("XValues");
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

        public float ModelTotalLengthFront
        {
            get
            {
                return m_ModelTotalLengthFront;
            }

            set
            {
                m_ModelTotalLengthFront = value;
                if (!IsSetFromCode) InitXValuesAndSetX();
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
                if (!IsSetFromCode) InitXValuesAndSetX();
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
                if (!IsSetFromCode) InitXValuesAndSetX();
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
                if (!IsSetFromCode) InitXValuesAndSetX();
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FibreglassProperties() { }

        public FibreglassProperties(EModelType_FS modelType, float lengthFront, float lengthLeft, double widthModularWall, double widthModularRoof, string side, float x, float y, float length)
        {
            IsSetFromCode = true;

            ModelType = modelType;

            ModelTotalLengthFront = lengthFront;
            ModelTotalLengthLeft = lengthLeft;
            CladdingWidthModular_Wall = (float)widthModularWall;
            CladdingWidthModular_Roof = (float)widthModularRoof;

            InitXValues();

            Side = side;
            X = x;
            Y = y;
            Length = length;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetDefaults(EModelType_FS modelType, float lengthFront, float lengthLeft, double widthModularWall, double widthModularRoof)
        {
            IsSetFromCode = true;

            ModelType = modelType;
            ModelTotalLengthFront = lengthFront;
            ModelTotalLengthLeft = lengthLeft;
            CladdingWidthModular_Wall = (float)widthModularWall;
            CladdingWidthModular_Roof = (float)widthModularRoof;

            InitXValues();

            this.Side = "Left";
            this.X = 0f;
            this.Y = 0.6f;
            this.Length = 1.8f;

            IsSetFromCode = false;
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

            //if (XValues.Count != 0) X = XValues.First();
        }

        private void InitXValuesAndSetX()
        {
            int index = XValues.IndexOf(X);
            InitXValues();
            //if (XValues.Count > 0 && !XValues.Contains(X)) X = XValues.First();
            if (XValues.Count == 0) return;

            if (index < XValues.Count && index != -1) X = XValues.ElementAt(index);
            else X = XValues.First();
        }



    }
}
