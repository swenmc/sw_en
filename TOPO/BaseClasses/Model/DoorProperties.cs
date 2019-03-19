using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                m_fDoorsHeight = value;
                NotifyPropertyChanged("fDoorsHeight");
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
                m_fDoorsWidth = value;
                NotifyPropertyChanged("fDoorsWidth");
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
                m_fDoorCoordinateXinBlock = value;
                NotifyPropertyChanged("fDoorCoordinateXinBlock");
            }
        }

        public List<int> Bays
        {
            get
            {
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
    }
}
