//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using DATABASE;
//using DATABASE.DTO;

//namespace BaseClasses
//{
//    [Serializable]
//    public class CrossBracingProperties : INotifyPropertyChanged
//    {
//        [field: NonSerializedAttribute()]
//        public event PropertyChangedEventHandler PropertyChanged;

//        private int m_iBayNumber;
//        private int m_iBayNumber_old;

//        private ObservableCollection<int> m_Bays;

//        // Nove properties
//        private int m_iBayIndex;

//        private bool m_bWallLeftSide;
//        private bool m_bWallRightSide;
//        private bool m_bRoof;

//        private int m_iNumberOfCrossBracingMembers_WallLeftSide;
//        private int m_iNumberOfCrossBracingMembers_WallRightSide;
//        private int m_iNumberOfCrossBracingMembers_Walls;

//        private int m_iNumberOfCrossesPerRafter_Maximum;
//        private int m_iNumberOfCrossesPerRafter;
//        private int m_iNumberOfCrossBracingMembers_BayRoof;
//        private int m_iEveryXXPurlin; // Index of purlin 0 - no bracing 1 - every, 2 - every second purlin, 3 - every third purlin, ...
//        private bool m_bOnlyFirstCrossOnRafter;

//        private int m_iNumberOfCrossBracingMembers_Bay;

//        public bool IsSetFromCode = false;

//        public int iBayNumber
//        {
//            get
//            {
//                return m_iBayNumber;
//            }

//            set
//            {
//                m_iBayNumber_old = m_iBayNumber;
//                m_iBayNumber = value;
//                NotifyPropertyChanged("iBayNumber");
//            }
//        }

//        /*
//        public int iBayNumber_old
//        {
//            get
//            {
//                return m_iBayNumber_old;
//            }

//            set
//            {
//                m_iBayNumber_old = value;
//            }
//        }*/

//        public ObservableCollection<int> Bays
//        {
//            get
//            {
//                if (m_Bays == null) m_Bays = new ObservableCollection<int>();
//                return m_Bays;
//            }

//            set
//            {
//                m_Bays = value;
//                if (m_Bays != null) NotifyPropertyChanged("Bays");
//            }
//        }


//        // Nove properties

//        public int iBayIndex
//        {
//            get
//            {
//                return m_iBayIndex;
//            }

//            set
//            {
//                m_iBayIndex = value;
//            }
//        }

//        public bool bWallLeftSide
//        {
//            get
//            {
//                return m_bWallLeftSide;
//            }

//            set
//            {
//                m_bWallLeftSide = value;
//                NotifyPropertyChanged("bWallLeftSide");
//            }
//        }

//        public bool bWallRightSide
//        {
//            get
//            {
//                return m_bWallRightSide;
//            }

//            set
//            {
//                m_bWallRightSide = value;
//                NotifyPropertyChanged("bWallRightSide");
//            }
//        }

//        public bool bRoof
//        {
//            get
//            {
//                return m_bRoof;
//            }

//            set
//            {
//                m_bRoof = value;
//                NotifyPropertyChanged("bRoof");
//            }
//        }

//        public int iNumberOfCrossBracingMembers_WallLeftSide
//        {
//            get
//            {
//                return m_iNumberOfCrossBracingMembers_WallLeftSide;
//            }

//            set
//            {
//                m_iNumberOfCrossBracingMembers_WallLeftSide = value;
//            }
//        }

//        public int iNumberOfCrossBracingMembers_WallRightSide
//        {
//            get
//            {
//                return m_iNumberOfCrossBracingMembers_WallRightSide;
//            }

//            set
//            {
//                m_iNumberOfCrossBracingMembers_WallRightSide = value;
//            }
//        }

//        public int iNumberOfCrossBracingMembers_Walls
//        {
//            get
//            {
//                return m_iNumberOfCrossBracingMembers_Walls;
//            }

//            set
//            {
//                m_iNumberOfCrossBracingMembers_Walls = value;
//            }
//        }

//        public int iNumberOfCrossesPerRafter_Maximum
//        {
//            get
//            {
//                return m_iNumberOfCrossesPerRafter_Maximum;
//;
//            }

//            set
//            {
//                m_iNumberOfCrossesPerRafter_Maximum = value;
//            }
//        }

//        public int iNumberOfCrossesPerRafter
//        {
//            get
//            {
//                return m_iNumberOfCrossesPerRafter;
//            }

//            set
//            {
//                m_iNumberOfCrossesPerRafter = value;
//            }
//        }

//        public int iNumberOfCrossBracingMembers_BayRoof
//        {
//            get
//            {
//                return m_iNumberOfCrossBracingMembers_BayRoof;
//            }

//            set
//            {
//                m_iNumberOfCrossBracingMembers_BayRoof = value;
//            }
//        }

//        public int iEveryXXPurlin
//        {
//            get
//            {
//                return m_iEveryXXPurlin;
//            }

//            set
//            {
//                m_iEveryXXPurlin = value;
//                NotifyPropertyChanged("iEveryXXPurlin");
//            }
//        }

//        public bool bOnlyFirstCrossOnRafter
//        {
//            get
//            {
//                return m_bOnlyFirstCrossOnRafter;
//            }

//            set
//            {
//                m_bOnlyFirstCrossOnRafter = value;
//                NotifyPropertyChanged("bOnlyFirstCrossOnRafter");
//            }
//        }

//        public int iNumberOfCrossBracingMembers_Bay
//        {
//            get
//            {
//                return m_iNumberOfCrossBracingMembers_Bay;
//            }

//            set
//            {
//                m_iNumberOfCrossBracingMembers_Bay = value;
//            }
//        }

//        public CrossBracingProperties()
//        {
//        }

//        //-------------------------------------------------------------------------------------------------------------
//        protected void NotifyPropertyChanged(string propertyName)
//        {
//            if (this.PropertyChanged != null)
//                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//        }

//        public bool Validate()
//        {
//            bool isValid = true;

//            // TODO doplnit

//            return isValid;
//        }

//        public bool ValidateBays()
//        {
//            if (iBayNumber <= Bays.Count) return true;
//            else return false;
//        }
//    }
//}
