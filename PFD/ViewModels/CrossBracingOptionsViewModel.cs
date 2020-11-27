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
using System.Collections.ObjectModel;

namespace PFD
{
    [Serializable]
    public class CrossBracingOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        private List<string> m_RoofPositions;
        ObservableCollection<CCrossBracingInfo> m_CrossBracingList;
        private int m_SelectedCrossBracingIndex;
        
        private List<int> m_Bays;
        private int m_BayFrom;
        private int m_BayTo;
        private bool m_WallLeft;
        private bool m_WallRight;
        private bool m_Roof;
        private string m_RoofPosition;
        private bool m_FirstCrossOnRafter;
        private bool m_LastCrossOnRafter;

        public List<string> RoofPositions
        {
            get
            {
                if (m_RoofPositions == null) m_RoofPositions = new List<string>() { "None", "Every Purlin", "Every second purlin", "Every 3rd purlin", "Every 4th purlin", "Every 5th purlin" };
                return m_RoofPositions;
            }

            set
            {
                m_RoofPositions = value;
                NotifyPropertyChanged("RoofPositions");
            }
        }

        public ObservableCollection<CCrossBracingInfo> CrossBracingList
        {
            get
            {
                return m_CrossBracingList;
            }

            set
            {
                m_CrossBracingList = value;
                foreach (CCrossBracingInfo cbi in CrossBracingList)
                {
                    cbi.PropertyChanged += crossBracingItem_PropertyChanged;
                }
                NotifyPropertyChanged("CrossBracingList");
            }
        }

        public int SelectedCrossBracingIndex
        {
            get
            {
                return m_SelectedCrossBracingIndex;
            }

            set
            {
                m_SelectedCrossBracingIndex = value;
                NotifyPropertyChanged("SelectedCrossBracingIndex");
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
                if (m_Bays != null && m_Bays.Count > 1)
                {
                    BayFrom = m_Bays.First();
                    BayTo = m_Bays.Last();
                }
                
                NotifyPropertyChanged("Bays");
            }
        }

        public int BayFrom
        {
            get
            {
                return m_BayFrom;
            }

            set
            {
                m_BayFrom = value;
                NotifyPropertyChanged("BayFrom");
            }
        }

        public int BayTo
        {
            get
            {
                return m_BayTo;
            }

            set
            {
                m_BayTo = value;
                NotifyPropertyChanged("BayTo");
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
                    FirstCrossOnRafter = false;
                    LastCrossOnRafter = false;
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

        private void crossBracingItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(PropertyChanged != null) PropertyChanged(sender, e);
            //NotifyPropertyChanged("CrossBracingItem_PropertyChanged");
        }

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
       

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CrossBracingOptionsViewModel(int baysNum, int iOneRafterPurlinNo)
        {
            IsSetFromCode = true;
            RoofPositions = GetRoofPositions(iOneRafterPurlinNo);

            initBays(baysNum);

            ObservableCollection<CCrossBracingInfo> items = new ObservableCollection<CCrossBracingInfo>();

            for (int i = 1; i <= baysNum; i++)
            {
                CCrossBracingInfo cbi = null;
                if (i == 1 || i == baysNum)
                {
                    cbi = new CCrossBracingInfo(i, true, true, true, RoofPositions[2], 2, false, false, RoofPositions);
                }
                else
                {
                    cbi = new CCrossBracingInfo(i, false, false, false, RoofPositions[0], 0, false, false, RoofPositions);
                }
                
                items.Add(cbi);
            }

            CrossBracingList = items;

            WallLeft = false;
            WallRight = false;
            Roof = false;
            RoofPosition = "None";

            FirstCrossOnRafter = false;
            LastCrossOnRafter = false;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void initBays(int baysNum)
        {
            List<int> bays = new List<int>(baysNum);
            for (int i = 1; i <= baysNum; i++)
            {
                bays.Add(i);
            }
            Bays = bays;
        }

        
        public void ResetCounters()
        {
            foreach (CCrossBracingInfo cb in CrossBracingList)
            {
                cb.NumberOfCrossBracingMembers_WallLeftSide = 0;
                cb.NumberOfCrossBracingMembers_WallRightSide = 0;
                cb.NumberOfCrossBracingMembers_Walls = 0;

                cb.NumberOfCrossBracingMembers_Bay = 0;
                cb.NumberOfCrossBracingMembers_BayRoof = 0;

                //cb.NumberOfCrossesPerRafter = 0;
                //cb.NumberOfCrossesPerRafter_Maximum = 0;
            }
        }

        public void SetRoofPositions(List<string> roofPositions)
        {
            RoofPositions = roofPositions;

            foreach (CCrossBracingInfo cb in CrossBracingList)
            {
                cb.SetRoofPositions(roofPositions);
            }
        }

        public List<string> GetRoofPositions(int iPurlinsNum)
        {
            List<string> items = new List<string>();
            for (int i = 0; i <= iPurlinsNum; i++)
            {
                if (i == 0) items.Add("None");
                if (i == 1) items.Add("Every purlin");
                if (i == 2) items.Add("Every 2nd purlin");
                if (i == 3) items.Add("Every 3rd purlin");
                if (i >= 4) items.Add($"Every {i}th purlin");

            }
            return items;
        }


        public int GetLeftWallCrosses()
        {
            int count = CrossBracingList.Count(c => c.WallLeft == true);            
            return count;
        }
        public int GetRightWallCrosses()
        {
            int count = CrossBracingList.Count(c => c.WallRight == true);            
            return count;
        }

        public int GetRoofCrosses()
        {
            int count = CrossBracingList.Count(c => c.Roof == true && c.RoofPosition != "None" && c.FirstCrossOnRafter == false && c.LastCrossOnRafter == false);
            return count;
        }

    }
}