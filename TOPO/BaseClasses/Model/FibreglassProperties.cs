using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BaseClasses.Helpers;
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

        private string m_Side_old;
        private float m_X_old;
        private float m_Y_old;
        private float m_Length_old;

        private List<string> m_Sides;
        private List<float> m_XValues;

        private EModelType_FS m_ModelType;
        private float m_ModelTotalLengthFront;
        private float m_ModelTotalLengthLeft;
        private float m_CladdingWidthModular_Wall;
        private float m_CladdingWidthModular_Roof;

        private float m_RoofPitch_deg;
        private float m_WallHeightOverall;
        private double m_MaxHeight;

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
                Side_old = m_Side;
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
                X_old = m_X;
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
                Y_old = m_Y;
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
                Length_old = m_Length;
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

        public string Side_old
        {
            get
            {
                return m_Side_old;
            }

            set
            {
                m_Side_old = value;
            }
        }

        public float X_old
        {
            get
            {
                return m_X_old;
            }

            set
            {
                m_X_old = value;
            }
        }

        public float Y_old
        {
            get
            {
                return m_Y_old;
            }

            set
            {
                m_Y_old = value;
            }
        }

        public float Length_old
        {
            get
            {
                return m_Length_old;
            }

            set
            {
                m_Length_old = value;
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
        public float WallHeightOverall
        {
            get
            {
                return m_WallHeightOverall;
            }

            set
            {
                m_WallHeightOverall = value;
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

        


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FibreglassProperties() { }

        public FibreglassProperties(EModelType_FS modelType, float lengthFront, float lengthLeft, double widthModularWall, double widthModularRoof, float roofPitch_deg, float wallHeightOverall,
            string side, float x, float y, float length)
        {
            IsSetFromCode = true;

            ModelType = modelType;

            RoofPitch_deg = roofPitch_deg;
            WallHeightOverall = wallHeightOverall;
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

        private bool ValidateMaxHeight()
        {
            if (Side == "Left" || Side == "Right")
            {
                MaxHeight = ModelHelper.GetVerticalCoordinate(Side, ModelType, ModelTotalLengthLeft, WallHeightOverall, X, RoofPitch_deg);
            }
            else if (Side == "Front" || Side == "Back")
            {
                //748 To Mato
                //tu bude potrebne ostatne pridat a vypocitat maxHeight ktoru chceme validovat
                //MaxHeight = ModelHelper.GetVerticalCoordinate(Side, ModelType, ModelTotalLengthLeft, WallHeightOverall, X, RoofPitch_deg);
            }
            else //roof
            {
                //748 To Mato
                //tu bude potrebne ostatne pridat a vypocitat maxHeight ktoru chceme validovat
                //MaxHeight = ModelHelper.GetVerticalCoordinate(Side, ModelType, ModelTotalLengthLeft, WallHeightOverall, X, RoofPitch_deg);
                //roofEdgeOverhang_Y
            }


            if (Y + Length > MaxHeight) return false;
            else return true;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetDefaults(EModelType_FS modelType, float lengthFront, float lengthLeft, float roofPitch_deg, float wallHeightOverall, double widthModularWall, double widthModularRoof)
        {
            IsSetFromCode = true;

            ModelType = modelType;

            RoofPitch_deg = roofPitch_deg;
            WallHeightOverall = wallHeightOverall;
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

        public bool IsInCollisionWith(FibreglassProperties f)
        {
            if (Side != f.Side) return false;
            if (!MathF.d_equal(X, f.X)) return false;
            if (Y < f.Y && !MathF.d_equal(Y + Length, f.Y) && Y + Length > f.Y) return true;    //musime kontrolovat na dequal, lebo podla PC je 2.6 > 2.6
            if (f.Y < Y && !MathF.d_equal(f.Y + f.Length, Y) && f.Y + f.Length > Y) return true;

            //equal =  collision
            if (Side == f.Side && MathF.d_equal(X, f.X) && MathF.d_equal(Y, f.Y) && MathF.d_equal(Length, f.Length)) return true;

            return false;
        }

        public void Undo()
        {
            IsSetFromCode = true;
            Side = Side_old;
            X = X_old;
            Y = Y_old;
            Length = Length_old;
            IsSetFromCode = false;
        }
        public void UndoSide()
        {
            Side = Side_old;
        }
        public void UndoX()
        {
            X = X_old;
        }
        public void UndoY()
        {
            Y = Y_old;
        }
        public void UndoLength()
        {
            Length = Length_old;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is FibreglassProperties)) return false;

            FibreglassProperties fp = obj as FibreglassProperties;

            if (!MathF.d_equal(X, fp.X)) return false;
            if (!MathF.d_equal(Y, fp.Y)) return false;
            if (!MathF.d_equal(Length, fp.Length)) return false;
            if (Side != fp.Side) return false;

            return true; //all params same => objects equal
        }

    }
}
