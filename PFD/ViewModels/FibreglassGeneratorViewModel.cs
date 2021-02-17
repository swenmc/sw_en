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
        private List<string> m_XValues;

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

        public List<string> XValues
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

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FibreglassGeneratorViewModel()
        {
            IsSetFromCode = true;

            //AddDoors = false;
            //DeleteDoors = false;
            //DoorType = "Roller Door";
            //DoorsHeight = 2.1f;
            //DoorsWidth = 0.6f;
            //DoorCoordinateXinBlock = 0.5f;
            //CoatingColor = CoatingColors.First();

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}