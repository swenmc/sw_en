using BaseClasses;
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
    public class CCrossBracingInfo : INotifyPropertyChanged
    {        
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool MIsSetFromCode;

        private int m_BayNumber;
        private int m_BayIndex;
        private bool m_WallLeft;
        private bool m_WallRight;
        private bool m_Roof;
        private string m_RoofPosition;
        //private List<string> m_RoofPositions;
        private bool m_FirstCrossOnRafter;
        private bool m_LastCrossOnRafter;


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

        public bool WallLeft
        {
            get
            {
                return m_WallLeft;
            }

            set
            {
                m_WallLeft = value;
                NotifyPropertyChanged("WallLeft");
            }
        }

        public bool WallRight
        {
            get
            {
                return m_WallRight;
            }

            set
            {
                m_WallRight = value;
                NotifyPropertyChanged("WallRight");
            }
        }

        public bool Roof
        {
            get
            {
                return m_Roof;
            }

            set
            {
                m_Roof = value;
                NotifyPropertyChanged("Roof");
            }
        }

        public string RoofPosition
        {
            get
            {
                return m_RoofPosition;
            }

            set
            {
                m_RoofPosition = value;
                NotifyPropertyChanged("RoofPosition");
            }
        }

        //public List<string> RoofPositions
        //{
        //    get
        //    {
        //        return m_RoofPositions;
        //    }

        //    set
        //    {
        //        m_RoofPositions = value;
        //        NotifyPropertyChanged("RoofPositions");
        //    }
        //}

        public bool FirstCrossOnRafter
        {
            get
            {
                return m_FirstCrossOnRafter;
            }

            set
            {
                m_FirstCrossOnRafter = value;
                NotifyPropertyChanged("FirstCrossOnRafter");
            }
        }

        public bool LastCrossOnRafter
        {
            get
            {
                return m_LastCrossOnRafter;
            }

            set
            {
                m_LastCrossOnRafter = value;
                NotifyPropertyChanged("LastCrossOnRafter");
            }
        }

        public CCrossBracingInfo(int bayNumber, bool wallLeft, bool wallRight, bool roof, string roofPosition, bool firstCrossOnRafter, bool lastCrossOnRafter)
        {
            MIsSetFromCode = false;

            m_BayNumber = bayNumber;
            m_BayIndex = bayNumber - 1;
            m_WallLeft = wallLeft;
            m_WallRight = wallRight;
            m_Roof = roof;
            m_RoofPosition = roofPosition;
            //m_RoofPositions = roofPositions;
            m_FirstCrossOnRafter = firstCrossOnRafter;
            m_LastCrossOnRafter = lastCrossOnRafter;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

       
    }
}
