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

        private void crossBracingItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
       

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CrossBracingOptionsViewModel(int baysNum)
        {
            IsSetFromCode = true;

            ObservableCollection<CCrossBracingInfo> items = new ObservableCollection<CCrossBracingInfo>();

            for (int i = 1; i <= baysNum; i++)
            {
                CCrossBracingInfo cbi = new CCrossBracingInfo(i, false, false, true, "None", false, false);
                items.Add(cbi);
            }

            CrossBracingList = items;

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