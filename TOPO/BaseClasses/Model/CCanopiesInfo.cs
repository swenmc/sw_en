﻿using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaseClasses
{
    [Serializable]
    public class CCanopiesInfo : INotifyPropertyChanged
    {        
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool MIsSetFromCode;

        private int m_BayNumber;
        private int m_BayIndex;
        private bool m_Left;
        private bool m_Right;
        private double m_WidthLeft;
        private double m_WidthRight;
        private int m_PurlinCountLeft;
        private int m_PurlinCountRight;
        private bool m_IsCrossBracedLeft;
        private bool m_IsCrossBracedRight;

        private double m_DefaultWidth;

        public bool IsSetFromCode
        {
            get
            {
                return MIsSetFromCode;
            }

            set
            {
                MIsSetFromCode = value;
                //NotifyPropertyChanged("IsSetFromCode");
            }
        }

        public int BayNumber
        {
            get
            {
                return m_BayNumber;
            }

            set
            {
                m_BayNumber = value;
                if (m_BayNumber < 1) m_BayIndex = 0;
                else m_BayIndex = m_BayNumber - 1;
                NotifyPropertyChanged("BayNumber");
            }
        }

        public int BayIndex
        {
            get
            {
                return m_BayIndex;
            }

            set
            {
                m_BayIndex = value;
            }
        }

        public bool Left
        {
            get
            {
                return m_Left;
            }

            set
            {
                m_Left = value;
                if (!IsSetFromCode)
                {
                    SetLeftDefaults();
                }
                NotifyPropertyChanged("Left");
            }
        }

        public bool Right
        {
            get
            {
                return m_Right;
            }

            set
            {
                m_Right = value;
                if (!IsSetFromCode)
                {
                    SetRightDefaults();
                }
                NotifyPropertyChanged("Right");
            }
        }

        public double WidthLeft
        {
            get
            {
                return m_WidthLeft;
            }

            set
            {
                if (Left) ValidateWidth(value);
                m_WidthLeft = value;
                NotifyPropertyChanged("WidthLeft");
            }
        }

        public double WidthRight
        {
            get
            {
                return m_WidthRight;
            }

            set
            {
                if (Right) ValidateWidth(value);
                m_WidthRight = value;
                NotifyPropertyChanged("WidthRight");
            }
        }

        public int PurlinCountLeft
        {
            get
            {
                return m_PurlinCountLeft;
            }

            set
            {
                if (Left) ValidatePurlinCount(value);
                m_PurlinCountLeft = value;
                NotifyPropertyChanged("PurlinCountLeft");
            }
        }

        public int PurlinCountRight
        {
            get
            {
                return m_PurlinCountRight;
            }

            set
            {
                if (Right) ValidatePurlinCount(value);
                m_PurlinCountRight = value;
                NotifyPropertyChanged("PurlinCountRight");
            }
        }

        public bool IsCrossBracedLeft
        {
            get
            {
                return m_IsCrossBracedLeft;
            }

            set
            {
                m_IsCrossBracedLeft = value;
                NotifyPropertyChanged("IsCrossBracedLeft");
            }
        }

        public bool IsCrossBracedRight
        {
            get
            {
                return m_IsCrossBracedRight;
            }

            set
            {
                m_IsCrossBracedRight = value;
                NotifyPropertyChanged("IsCrossBracedRight");
            }
        }

        public double DefaultWidth
        {
            get
            {
                return m_DefaultWidth;
            }

            set
            {
                m_DefaultWidth = value;
                if (m_DefaultWidth < 1) m_DefaultWidth = 1;
                if (m_DefaultWidth > 6) m_DefaultWidth = 6;
            }
        }

        public CCanopiesInfo(int bayNumber, bool left, bool right, double widthLeft, double widthRight, int purlinCountLeft, int purlinCountRight, bool isCrossBracedLeft, bool isCrossBracedRight, double defWidth)
        {
            MIsSetFromCode = false;

            DefaultWidth = defWidth;

            m_BayNumber = bayNumber;
            m_BayIndex = bayNumber - 1;
            Left = left;
            Right = right;
            WidthLeft = widthLeft;
            WidthRight = widthRight;
            PurlinCountLeft = purlinCountLeft;
            PurlinCountRight = purlinCountRight;
            IsCrossBracedLeft = isCrossBracedLeft;
            IsCrossBracedRight = isCrossBracedRight;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetLeftDefaults()
        {
            if (Left)
            {                
                WidthLeft = DefaultWidth;
                PurlinCountLeft = 2;
                IsCrossBracedLeft = true;
            }
            else
            {
                WidthLeft = 0;
                PurlinCountLeft = 0;
                IsCrossBracedLeft = false;
            }
        }

        private void SetRightDefaults()
        {
            if (Right)
            {
                WidthRight = DefaultWidth;
                PurlinCountRight = 2;
                IsCrossBracedRight = true;
            }
            else
            {
                WidthRight = 0;
                PurlinCountRight = 0;
                IsCrossBracedRight = false;
            }
        }

        private void ValidateWidth(double value)
        {
            if (value <= 0 || value > 10)
                throw new ArgumentException("Width must be between 0 and 10 [m]");
        }

        private void ValidatePurlinCount(int value)
        {
            if (value <= 0 || value > 15)
                throw new ArgumentException("Purlin count must be between 0 and 15.");
        }
    }
}
