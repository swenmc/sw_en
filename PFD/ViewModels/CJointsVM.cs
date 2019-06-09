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
        
        private int m_SelectedTabIndex;

        private CComponentParamsView m_ChangedScrewArrangementParameter;
        private CComponentParamsView m_ChangedGeometryParameter;
        public bool IsSetFromCode = false;

        //private List<CComponentParamsView> m_ScrewArrangementParameters;
        //private List<CComponentParamsView> m_ComponentGeometry;
        //private List<CComponentParamsView> m_ComponentDetails;

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

        public int SelectedTabIndex
        {
            get
            {
                return m_SelectedTabIndex;
            }

            set
            {
                m_SelectedTabIndex = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        public CComponentParamsView ChangedScrewArrangementParameter
        {
            get
            {
                return m_ChangedScrewArrangementParameter;
            }

            set
            {
                m_ChangedScrewArrangementParameter = value;
                NotifyPropertyChanged("ChangedScrewArrangementParameter");
            }
        }

        public CComponentParamsView ChangedGeometryParameter
        {
            get
            {
                return m_ChangedGeometryParameter;
            }

            set
            {
                m_ChangedGeometryParameter = value;
                NotifyPropertyChanged("ChangedGeometryParameter");
            }
        }

        //public List<CComponentParamsView> ScrewArrangementParameters
        //{
        //    get
        //    {
        //        return m_ScrewArrangementParameters;
        //    }

        //    set
        //    {
        //        m_ScrewArrangementParameters = value;
        //    }
        //}

        //public List<CComponentParamsView> ComponentGeometry
        //{
        //    get
        //    {
        //        return m_ComponentGeometry;
        //    }

        //    set
        //    {
        //        m_ComponentGeometry = value;
        //    }
        //}

        //public List<CComponentParamsView> ComponentDetails
        //{
        //    get
        //    {
        //        return m_ComponentDetails;
        //    }

        //    set
        //    {
        //        m_ComponentDetails = value;
        //    }
        //}

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CJointsVM()
        {
            JointTypes = CJointsManager.LoadJointsConnectionDescriptions();
            JointTypeIndex = 0;
            SelectedTabIndex = 0;

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
