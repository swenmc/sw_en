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
    public class BayWidthOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
                
        ObservableCollection<CBayInfo> m_BayWidthList;        
        
        private List<int> m_Bays;
        private int m_BayFrom;
        private int m_BayTo;
        private float m_Width;
        
        public ObservableCollection<CBayInfo> BayWidthList
        {
            get
            {
                return m_BayWidthList;
            }

            set
            {
                m_BayWidthList = value;
                foreach (CBayInfo bi in m_BayWidthList)
                {
                    bi.PropertyChanged += bayWidthItem_PropertyChanged;
                }
                NotifyPropertyChanged("BayWidthList");
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

        public float Width
        {
            get
            {
                return m_Width;
            }

            set
            {
                m_Width = value;
                NotifyPropertyChanged("Width");
            }
        }

        private void bayWidthItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
        public BayWidthOptionsViewModel(int baysNum, float bayWidth)
        {
            IsSetFromCode = true;
            
            initBays(baysNum);

            initBaysWidths(baysNum, bayWidth);

            Width = bayWidth; 

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

        private void initBaysWidths(int baysNum, float bayWidth)
        {
            List<CBayInfo> items = new List<CBayInfo>();
            for (int i = 1; i <= baysNum; i++)
            {
                items.Add(new CBayInfo(i, bayWidth));
            }
            BayWidthList = new ObservableCollection<CBayInfo>(items);
        }

        public float GetTotalWidth()
        {
            float totalW = 0;
            
            foreach (CBayInfo bi in BayWidthList)
            {
                totalW += bi.Width;
            }
            return totalW;
        }

        public float GetBayWidth(int bayID)
        {
            CBayInfo bi = BayWidthList.FirstOrDefault(b => b.BayNumber == bayID);
            if (bi != null) return bi.Width;
            else return float.NaN;
        }

        public float GetBayWidth_ByIndex(int bayIndex)
        {
            CBayInfo bi = BayWidthList.FirstOrDefault(b => b.BayIndex == bayIndex);
            if (bi != null) return bi.Width;
            else return float.NaN;
        }

        public float GetPreviousBayWidth_ForFrameIndex(int frameIndex)
        {
            CBayInfo bi = BayWidthList.ElementAtOrDefault(frameIndex - 1);
            if (bi != null) return bi.Width;
            else return float.NaN;
        }
        public float GetNextBayWidth_ForFrameIndex(int frameIndex)
        {
            CBayInfo bi = BayWidthList.ElementAtOrDefault(frameIndex);
            if (bi != null) return bi.Width;
            else return float.NaN;
        }

        public List<float> GetBaysWidths()
        {
            List<float> values = new List<float>();
            foreach (CBayInfo bi in BayWidthList)
            {
                values.Add(bi.Width);
            }
            return values;
        }

        public void SetViewModel(BayWidthOptionsViewModel vm)
        {
            if (vm == null) return;

            BayWidthList = vm.BayWidthList;

            Bays = vm.Bays;
            BayFrom = vm.BayFrom;
            BayTo = vm.BayTo;            
            Width = vm.Width;            
        }

        public void ResetBaysWidths(int baysNum, float bayWidth)
        {
            initBaysWidths(baysNum, bayWidth);
        }
    }
}