using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class WindowProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string m_sBuildingSide;
        private int m_iBayNumber;
        private List<int> m_Bays;
        private float m_fWindowsHeight;
        private float m_fWindowsWidth;
        private float m_fWindowCoordinateXinBay;
        private float m_fWindowCoordinateZinBay;
        private int m_iNumberOfWindowColumns;

        public string sBuildingSide
        {
            get
            {
                return m_sBuildingSide;
            }

            set
            {
                m_sBuildingSide = value;
                NotifyPropertyChanged("sBuildingSide");
            }
        }

        public int iBayNumber
        {
            get
            {
                return m_iBayNumber;
            }

            set
            {
                m_iBayNumber = value;
                NotifyPropertyChanged("iBayNumber");
            }
        }

        public float fWindowsHeight
        {
            get
            {
                return m_fWindowsHeight;
            }

            set
            {
                m_fWindowsHeight = value;
                NotifyPropertyChanged("fWindowsHeight");
            }
        }

        public float fWindowsWidth
        {
            get
            {
                return m_fWindowsWidth;
            }

            set
            {
                m_fWindowsWidth = value;
                NotifyPropertyChanged("fWindowsWidth");
            }
        }

        public float fWindowCoordinateXinBay
        {
            get
            {
                return m_fWindowCoordinateXinBay;
            }

            set
            {
                m_fWindowCoordinateXinBay = value;
                NotifyPropertyChanged("fWindowCoordinateXinBay");
            }
        }

        public float fWindowCoordinateZinBay
        {
            get
            {
                return m_fWindowCoordinateZinBay;
            }

            set
            {
                m_fWindowCoordinateZinBay = value;
                NotifyPropertyChanged("fWindowCoordinateZinBay");
            }
        }

        public int iNumberOfWindowColumns
        {
            get
            {
                return m_iNumberOfWindowColumns;
            }

            set
            {
                m_iNumberOfWindowColumns = value;
                NotifyPropertyChanged("iNumberOfWindowColumns");
            }
        }
        public List<int> Bays
        {
            get
            {
                if (m_Bays == null) m_Bays = new List<int>();
                return m_Bays;
            }

            set
            {
                m_Bays = value;
                if (m_Bays != null) NotifyPropertyChanged("Bays");
            }
        }

        public WindowProperties() { }


        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Validate()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(m_sBuildingSide)) return false;
            
            if (m_iBayNumber < 0) return false;
            if (m_fWindowsHeight < 0.1f) return false;
            if (m_fWindowsWidth < 0.1f) return false;
            if (m_fWindowCoordinateXinBay < 0f) return false;
            if (m_fWindowCoordinateZinBay < 0f) return false;
            if (m_iNumberOfWindowColumns < 0) return false;
            
            return isValid;
        }
    }
}
