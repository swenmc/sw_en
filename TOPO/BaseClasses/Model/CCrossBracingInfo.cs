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
        private List<string> m_RoofPositions;
        private bool m_FirstCrossOnRafter;
        private bool m_LastCrossOnRafter;

        private int m_NumberOfCrossBracingMembers_WallLeftSide;
        private int m_NumberOfCrossBracingMembers_WallRightSide;
        private int m_NumberOfCrossBracingMembers_Walls;
        private int m_NumberOfCrossBracingMembers_Bay;

        private int m_NumberOfCrossesPerRafter_Maximum;
        private int m_NumberOfCrossesPerRafter;
        private int m_NumberOfCrossBracingMembers_BayRoof;
        private int m_EveryXXPurlin; // Index of purlin 0 - no bracing 1 - every, 2 - every second purlin, 3 - every third purlin, ...



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
                if (!m_Roof)
                {
                    RoofPosition = RoofPositions[0];                    
                }
                else
                {
                    RoofPosition = RoofPositions[2];                    
                }
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
                if (!string.IsNullOrEmpty(m_RoofPosition)) EveryXXPurlin = RoofPositions.IndexOf(m_RoofPosition);
                if (m_Roof)
                {
                    if (m_RoofPosition == "None") throw new ArgumentException("None is not valid value.");
                }
                NotifyPropertyChanged("RoofPosition");
            }
        }

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

        public int NumberOfCrossBracingMembers_WallLeftSide
        {
            get
            {
                return m_NumberOfCrossBracingMembers_WallLeftSide;
            }

            set
            {
                m_NumberOfCrossBracingMembers_WallLeftSide = value;
            }
        }

        public int NumberOfCrossBracingMembers_WallRightSide
        {
            get
            {
                return m_NumberOfCrossBracingMembers_WallRightSide;
            }

            set
            {
                m_NumberOfCrossBracingMembers_WallRightSide = value;
            }
        }

        public int NumberOfCrossBracingMembers_Walls
        {
            get
            {
                return m_NumberOfCrossBracingMembers_Walls;
            }

            set
            {
                m_NumberOfCrossBracingMembers_Walls = value;
            }
        }

        public int NumberOfCrossBracingMembers_Bay
        {
            get
            {
                return m_NumberOfCrossBracingMembers_Bay;
            }

            set
            {
                m_NumberOfCrossBracingMembers_Bay = value;
            }
        }

        public int NumberOfCrossesPerRafter_Maximum
        {
            get
            {
                return m_NumberOfCrossesPerRafter_Maximum;
            }

            set
            {
                m_NumberOfCrossesPerRafter_Maximum = value;
            }
        }

        public int NumberOfCrossesPerRafter
        {
            get
            {
                return m_NumberOfCrossesPerRafter;
            }

            set
            {
                m_NumberOfCrossesPerRafter = value;
            }
        }

        public int NumberOfCrossBracingMembers_BayRoof
        {
            get
            {
                return m_NumberOfCrossBracingMembers_BayRoof;
            }

            set
            {
                m_NumberOfCrossBracingMembers_BayRoof = value;
            }
        }

        public int EveryXXPurlin
        {
            get
            {
                return m_EveryXXPurlin;
            }

            set
            {
                m_EveryXXPurlin = value;
                if (m_Roof)
                {
                    if (m_EveryXXPurlin == 0) throw new ArgumentException("None is not valid value.");
                }                
            }
        }

        public List<string> RoofPositions
        {
            get
            {
                return m_RoofPositions;
            }

            set
            {
                m_RoofPositions = value;
            }
        }

        public CCrossBracingInfo(int bayNumber, bool wallLeft, bool wallRight, bool roof, string roofPosition, int everyXXpurlin, bool firstCrossOnRafter, bool lastCrossOnRafter, List<string> roofPositions)
        {
            MIsSetFromCode = false;

            m_RoofPositions = roofPositions;

            m_BayNumber = bayNumber;
            m_BayIndex = bayNumber - 1;
            m_WallLeft = wallLeft;
            m_WallRight = wallRight;
            m_Roof = roof;
            m_RoofPosition = roofPosition;
            m_EveryXXPurlin = everyXXpurlin;
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
