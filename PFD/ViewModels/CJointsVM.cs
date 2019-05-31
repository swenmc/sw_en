using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace PFD
{
    public class CJointsVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int m_JointTypeIndex;
        private List<CConnectionDescription> m_JointTypes;

        private List<TabItem> m_TabItems;


        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        public int JointTypeIndex
        {
            get
            {
                return m_JointTypeIndex;
            }

            set
            {
                m_JointTypeIndex = value;
                NotifyPropertyChanged("JointTypeIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<CConnectionDescription> JointTypes
        {
            get
            {
                return m_JointTypes;
            }

            set
            {
                m_JointTypes = value;
                NotifyPropertyChanged("JointTypes");
            }
        }

        public List<TabItem> TabItems
        {
            get
            {
                return m_TabItems;
            }

            set
            {
                m_TabItems = value;
                NotifyPropertyChanged("TabItems");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CJointsVM()
        {
            JointTypes = CJointsManager.LoadJointsConnectionDescriptions();
            JointTypeIndex = 0;

            

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
