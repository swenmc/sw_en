using BaseClasses.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class LayoutsExportOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool m_ExportModel3D;
        private bool m_ExportModelViews;
        private bool m_ExportModelViewsFront;
        private bool m_ExportModelViewsBack;
        private bool m_ExportModelViewsLeft;
        private bool m_ExportModelViewsRight;
        private bool m_ExportModelViewsRoof;
        private bool m_ExportModelViewsMiddleFrame;
        private bool m_ExportModelViewsColumns;
        private bool m_ExportModelViewsFoundations;
        private bool m_ExportModelViewsFloor;        

        private bool m_ExportJointTypes;
        private bool m_ExportFootingTypes;
        private bool m_ExportFloorDetails;
        private bool m_ExportStandardDetails;

        private int m_ExportPageSize;
        private int m_ExportPageOrientation;

        public bool ExportModel3D
        {
            get
            {
                return m_ExportModel3D;
            }

            set
            {
                m_ExportModel3D = value;
                NotifyPropertyChanged("ExportModel3D");
            }
        }

        public bool ExportModelViews
        {
            get
            {
                return m_ExportModelViews;
            }

            set
            {
                bool changed = m_ExportModelViews != value;
                m_ExportModelViews = value;
                if (changed)
                {
                    ExportModelViewsFront = m_ExportModelViews;
                    ExportModelViewsBack = m_ExportModelViews;
                    ExportModelViewsLeft = m_ExportModelViews;
                    ExportModelViewsRight = m_ExportModelViews;
                    ExportModelViewsRoof = m_ExportModelViews;
                    ExportModelViewsMiddleFrame = m_ExportModelViews;
                    ExportModelViewsColumns = m_ExportModelViews;
                    ExportModelViewsFoundations = m_ExportModelViews;
                    ExportModelViewsFloor = m_ExportModelViews;
                }
               
                NotifyPropertyChanged("ExportModelViews");
            }
        }

        public bool ExportJointTypes
        {
            get
            {
                return m_ExportJointTypes;
            }

            set
            {
                m_ExportJointTypes = value;
                NotifyPropertyChanged("ExportJointTypes");
            }
        }

        public bool ExportFootingTypes
        {
            get
            {
                return m_ExportFootingTypes;
            }

            set
            {
                m_ExportFootingTypes = value;
                NotifyPropertyChanged("ExportFootingTypes");
            }
        }

        public bool ExportFloorDetails
        {
            get
            {
                return m_ExportFloorDetails;
            }

            set
            {
                m_ExportFloorDetails = value;
                NotifyPropertyChanged("ExportFloorDetails");
            }
        }

        public bool ExportStandardDetails
        {
            get
            {
                return m_ExportStandardDetails;
            }

            set
            {
                m_ExportStandardDetails = value;
                NotifyPropertyChanged("ExportStandardDetails");
            }
        }

        public bool ExportModelViewsFront
        {
            get
            {
                return m_ExportModelViewsFront;
            }

            set
            {
                m_ExportModelViewsFront = value;
                NotifyPropertyChanged("ExportModelViewsFront");
            }
        }

        public bool ExportModelViewsBack
        {
            get
            {
                return m_ExportModelViewsBack;
            }

            set
            {
                m_ExportModelViewsBack = value;
                NotifyPropertyChanged("ExportModelViewsBack");
            }
        }

        public bool ExportModelViewsLeft
        {
            get
            {
                return m_ExportModelViewsLeft;
            }

            set
            {
                m_ExportModelViewsLeft = value;
                NotifyPropertyChanged("ExportModelViewsLeft");
            }
        }

        public bool ExportModelViewsRight
        {
            get
            {
                return m_ExportModelViewsRight;
            }

            set
            {
                m_ExportModelViewsRight = value;
                NotifyPropertyChanged("ExportModelViewsRight");
            }
        }

        public bool ExportModelViewsRoof
        {
            get
            {
                return m_ExportModelViewsRoof;
            }

            set
            {
                m_ExportModelViewsRoof = value;
                NotifyPropertyChanged("ExportModelViewsRoof");
            }
        }

        public bool ExportModelViewsMiddleFrame
        {
            get
            {
                return m_ExportModelViewsMiddleFrame;
            }

            set
            {
                m_ExportModelViewsMiddleFrame = value;
                NotifyPropertyChanged("ExportModelViewsMiddleFrame");
            }
        }

        public bool ExportModelViewsColumns
        {
            get
            {
                return m_ExportModelViewsColumns;
            }

            set
            {
                m_ExportModelViewsColumns = value;
                NotifyPropertyChanged("ExportModelViewsColumns");
            }
        }

        public bool ExportModelViewsFoundations
        {
            get
            {
                return m_ExportModelViewsFoundations;
            }

            set
            {
                m_ExportModelViewsFoundations = value;
                NotifyPropertyChanged("ExportModelViewsFoundations");
            }
        }

        public bool ExportModelViewsFloor
        {
            get
            {
                return m_ExportModelViewsFloor;
            }

            set
            {
                m_ExportModelViewsFloor = value;
                NotifyPropertyChanged("ExportModelViewsFloor");
            }
        }

        public List<ComboItem> PageSizes
        {
            get
            {
                return new List<ComboItem>() { new ComboItem((int)EPageSizes.A4, "A4"),
                    new ComboItem((int)EPageSizes.A3, "A3"),
                    new ComboItem((int)EPageSizes.A2, "A2"),
                    new ComboItem((int)EPageSizes.A1, "A1"),
                    new ComboItem((int)EPageSizes.A0, "A0")
                };
            }
        }
        public List<ComboItem> PageOrientations
        {
            get
            {
                return new List<ComboItem>() { new ComboItem((int)EPageOrientation.Portrait, "Portrait"),
                    new ComboItem((int)EPageOrientation.Landscape, "Landscape")
                };
            }
        }

        public int ExportPageSize
        {
            get
            {
                return m_ExportPageSize;
            }

            set
            {
                m_ExportPageSize = value;
                NotifyPropertyChanged("ExportPageSize");
            }
        }

        public int ExportPageOrientation
        {
            get
            {
                return m_ExportPageOrientation;
            }

            set
            {
                m_ExportPageOrientation = value;
                NotifyPropertyChanged("ExportPageOrientation");
            }
        }

        public LayoutsExportOptionsViewModel()
        {
            ExportModel3D = true;
            ExportModelViews = true;
            ExportModelViewsFront = true;
            ExportModelViewsBack = true;
            ExportModelViewsLeft = true;
            ExportModelViewsRight = true;
            ExportModelViewsRoof = true;
            ExportModelViewsMiddleFrame = true;
            ExportModelViewsColumns = true;
            ExportModelViewsFoundations = true;
            ExportModelViewsFloor = true;

            ExportJointTypes = true;
            ExportFootingTypes = true;
            ExportFloorDetails = true;
            ExportStandardDetails = true;

            ExportPageSize = (int)EPageSizes.A4;
            ExportPageOrientation = (int)EPageOrientation.Landscape;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
