using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class DoorProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string m_sBuildingSide;
        private int m_iBayNumber;
        private List<int> m_Bays;
        private string m_sDoorType;
        private float m_fDoorsHeight;
        private float m_fDoorsWidth;
        private float m_fDoorCoordinateXinBlock;


        //validationValues
        private float m_WallHeight = float.NaN;
        private float m_L1 = float.NaN;
        private float m_distFrontColumns = float.NaN;
        private float m_distBackColumns = float.NaN;



        public string sBuildingSide
        {
            get
            {
                return m_sBuildingSide;
            }

            set
            {
                string temp = m_sBuildingSide;
                m_sBuildingSide = value;
                
                if (!ValidateDoorInsideBay())
                {
                    m_sBuildingSide = temp;
                    MessageBox.Show("Doors outside of bay width.");
                }
                else NotifyPropertyChanged("sBuildingSide");
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

        public string sDoorType
        {
            get
            {
                return m_sDoorType;
            }

            set
            {
                m_sDoorType = value;
                NotifyPropertyChanged("sDoorType");
            }
        }

        public float fDoorsHeight
        {
            get
            {
                return m_fDoorsHeight;
            }

            set
            {
                if (!float.IsNaN(m_WallHeight) && (value < 1 || value > m_WallHeight))
                {
                    MessageBox.Show($"Doors Height must be between 1 and {m_WallHeight} [m]");
                }
                else
                {
                    m_fDoorsHeight = value;
                    NotifyPropertyChanged("fDoorsHeight");
                }
                
            }
        }

        public float fDoorsWidth
        {
            get
            {
                return m_fDoorsWidth;
            }

            set
            {
                float temp = m_fDoorsWidth;
                m_fDoorsWidth = value;
                if (!ValidateDoorInsideBay()) { 
                    m_fDoorsWidth = temp;
                    MessageBox.Show("Doors outside of bay width.");
                }
                else NotifyPropertyChanged("fDoorsWidth");
            }
        }

        public float fDoorCoordinateXinBlock
        {
            get
            {
                return m_fDoorCoordinateXinBlock;
            }

            set
            {
                float temp = m_fDoorCoordinateXinBlock;
                m_fDoorCoordinateXinBlock = value;
                if (!ValidateDoorInsideBay()) {
                    m_fDoorCoordinateXinBlock = temp;
                    MessageBox.Show("Doors outside of bay width.");
                }
                else NotifyPropertyChanged("fDoorCoordinateXinBlock");
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
                if(m_Bays != null) NotifyPropertyChanged("Bays");
            }
        }


        public DoorProperties()
        {
            
        }

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
            if (string.IsNullOrEmpty(m_sDoorType)) return false;
            if (m_iBayNumber < 0f) return false;
            if (m_fDoorsHeight < 0.1f) return false;
            if (m_fDoorsWidth < 0.1f) return false;
            if (m_fDoorCoordinateXinBlock < 0f) return false;

            return isValid;
        }

        public bool ValidateDoorInsideBay()
        {
            if (sBuildingSide == "Front")
            {
                if (float.IsNaN(m_distFrontColumns)) return true;
                if (m_distFrontColumns < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            else if (sBuildingSide == "Back")
            {
                if (float.IsNaN(m_distBackColumns)) return true;
                if (m_distBackColumns < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            else if (sBuildingSide == "Left") {
                if (float.IsNaN(m_L1)) return true;
                if (m_L1 < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            else if (sBuildingSide == "Right")
            {
                if (float.IsNaN(m_L1)) return true;
                if (m_L1 < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            
            return true;
        }

        public void SetValidationValues(float wallHeight, float L1, float distFrontColumns, float distBackColumns)
        {
            m_WallHeight = wallHeight;
            m_L1 = L1;
            m_distFrontColumns = distFrontColumns;
            m_distBackColumns = distBackColumns;

        }
        
    }
}
