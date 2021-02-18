﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DATABASE;
using DATABASE.DTO;

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


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FibreglassProperties() { }

        public FibreglassProperties(EModelType_FS modelType, string side, float x, List<float> xValues, float y, float length)
        {
            IsSetFromCode = true;

            ModelType = modelType;

            XValues = xValues;
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


        public void SetDefaults(EModelType_FS modelType)
        {
            ModelType = modelType;
            
            this.Side = "Left";
            this.X = 0f;
            this.Y = 0.6f;
            this.Length = 1.8f;
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

        //private void InitXValues()
        //{
        //    List<float> x_values = new List<float>();
        //    float x = 0f;
        //    int i = 0;
        //    int maxCount = 100; //aby sme nespravili virus pri zlej inicializacii
        //    if (Side == "Left" || Side == "Right")
        //    {
        //        while (x <= ModelTotalLengthLeft)
        //        {
        //            x_values.Add(x);
        //            x += CladdingWidthModular_Wall;
        //            i++;
        //            if (i > maxCount) return;
        //        }
        //    }
        //    else if (Side == "Front" || Side == "Back")
        //    {
        //        while (x <= ModelTotalLengthFront)
        //        {
        //            x_values.Add(x);
        //            x += CladdingWidthModular_Wall;
        //            i++;
        //            if (i > maxCount) return;
        //        }
        //    }
        //    else if (Side == "Roof" || Side == "Roof-Left Side" || Side == "Roof-Right Side")
        //    {
        //        while (x <= ModelTotalLengthLeft)
        //        {
        //            x_values.Add(x);
        //            x += CladdingWidthModular_Roof;
        //            i++;
        //            if (i > maxCount) return;
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Side not recognized: " + Side);
        //    }

        //    XValues = x_values;

        //    if (XValues.Count != 0) X = XValues.First();
        //}

    }
}
