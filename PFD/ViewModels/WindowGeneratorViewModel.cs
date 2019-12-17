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
    public class WindowGeneratorViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        
        private float m_fWindowHeight;
        private float m_fWindowWidth;        
        private float m_fWindowCoordinateXinBay;
        private float m_fWindowCoordinateZinBay;
        private int m_iNumberOfWindowColumns;
        private bool m_AddWindows;

        public bool IsSetFromCode = false;

        public float WindowHeight
        {
            get
            {
                return m_fWindowHeight;
            }

            set
            {
                m_fWindowHeight = value;
                NotifyPropertyChanged("WindowHeight");
            }
        }

        public float WindowWidth
        {
            get
            {
                return m_fWindowWidth;
            }

            set
            {
                m_fWindowWidth = value;
                NotifyPropertyChanged("WindowWidth");
            }
        }

        public float WindowCoordinateXinBay
        {
            get
            {
                return m_fWindowCoordinateXinBay;
            }

            set
            {
                m_fWindowCoordinateXinBay = value;
                NotifyPropertyChanged("WindowCoordinateXinBay");
            }
        }

        public float WindowCoordinateZinBay
        {
            get
            {
                return m_fWindowCoordinateZinBay;
            }

            set
            {
                m_fWindowCoordinateZinBay = value;
                NotifyPropertyChanged("WindowCoordinateZinBay");
            }
        }

        public int NumberOfWindowColumns
        {
            get
            {
                return m_iNumberOfWindowColumns;
            }

            set
            {
                m_iNumberOfWindowColumns = value;
                NotifyPropertyChanged("NumberOfWindowColumns");
            }
        }

        public List<int> WindowColumns
        {
            get
            {
                return new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            }
        }

        public bool AddWindows
        {
            get
            {
                return m_AddWindows;
            }

            set
            {
                m_AddWindows = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public WindowGeneratorViewModel()
        {
            IsSetFromCode = true;
            AddWindows = false;
            WindowHeight = 0.6f;
            WindowWidth = 0.6f;
            WindowCoordinateXinBay = 0.4f;
            WindowCoordinateZinBay = 0.8f;
            NumberOfWindowColumns = 2;

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