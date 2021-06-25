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
        [field: NonSerializedAttribute()]
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

        private bool m_ExportModelCladdingLayingSchemeViews;
        private bool m_ExportModelCladdingLayingSchemeViewsFront;
        private bool m_ExportModelCladdingLayingSchemeViewsBack;
        private bool m_ExportModelCladdingLayingSchemeViewsLeft;
        private bool m_ExportModelCladdingLayingSchemeViewsRight;
        private bool m_ExportModelCladdingLayingSchemeViewsRoof;

        private bool m_ExportJointTypes;
        private bool m_ExportFootingTypes;
        private bool m_ExportFloorDetails;
        private bool m_ExportStandardDetails;

        private int m_ExportPageSize;
        private int m_ExportPageOrientation;
        private int m_ExportPageSizeViews;
        private int m_ExportPageOrientationViews;
        private int m_ExportPageSizeViewsCladding;
        private int m_ExportPageOrientationViewsCladding;

        private int m_ExportImagesQuality;

        private List<string> m_ModelViews;
        private int m_ViewIndex;

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

        public bool ExportModelCladdingLayingSchemeViews
        {
            get
            {
                return m_ExportModelCladdingLayingSchemeViews;
            }

            set
            {
                bool changed = m_ExportModelCladdingLayingSchemeViews != value;
                m_ExportModelCladdingLayingSchemeViews = value;
                if (changed)
                {
                    ExportModelCladdingLayingSchemeViewsFront = m_ExportModelCladdingLayingSchemeViews;
                    ExportModelCladdingLayingSchemeViewsBack = m_ExportModelCladdingLayingSchemeViews;
                    ExportModelCladdingLayingSchemeViewsLeft = m_ExportModelCladdingLayingSchemeViews;
                    ExportModelCladdingLayingSchemeViewsRight = m_ExportModelCladdingLayingSchemeViews;
                    ExportModelCladdingLayingSchemeViewsRoof = m_ExportModelCladdingLayingSchemeViews;
                }

                NotifyPropertyChanged("ExportModelCladdingLayingSchemeViews");
            }
        }

        public bool ExportModelCladdingLayingSchemeViewsFront
        {
            get
            {
                return m_ExportModelCladdingLayingSchemeViewsFront;
            }

            set
            {
                m_ExportModelCladdingLayingSchemeViewsFront = value;
                NotifyPropertyChanged("ExportModelCladdingLayingSchemeViewsFront");
            }
        }

        public bool ExportModelCladdingLayingSchemeViewsBack
        {
            get
            {
                return m_ExportModelCladdingLayingSchemeViewsBack;
            }

            set
            {
                m_ExportModelCladdingLayingSchemeViewsBack = value;
                NotifyPropertyChanged("ExportModelCladdingLayingSchemeViewsBack");
            }
        }

        public bool ExportModelCladdingLayingSchemeViewsLeft
        {
            get
            {
                return m_ExportModelCladdingLayingSchemeViewsLeft;
            }

            set
            {
                m_ExportModelCladdingLayingSchemeViewsLeft = value;
                NotifyPropertyChanged("ExportModelCladdingLayingSchemeViewsLeft");
            }
        }

        public bool ExportModelCladdingLayingSchemeViewsRight
        {
            get
            {
                return m_ExportModelCladdingLayingSchemeViewsRight;
            }

            set
            {
                m_ExportModelCladdingLayingSchemeViewsRight = value;
                NotifyPropertyChanged("ExportModelCladdingLayingSchemeViewsRight");
            }
        }

        public bool ExportModelCladdingLayingSchemeViewsRoof
        {
            get
            {
                return m_ExportModelCladdingLayingSchemeViewsRoof;
            }

            set
            {
                m_ExportModelCladdingLayingSchemeViewsRoof = value;
                NotifyPropertyChanged("ExportModelCladdingLayingSchemeViewsRoof");
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

        public List<ComboItem> ImagesQualities
        {
            get
            {
                return new List<ComboItem>() { new ComboItem((int)EImagesQuality.Low, "Low"),
                    new ComboItem((int)EImagesQuality.Normal, "Normal"),
                    new ComboItem((int)EImagesQuality.Hight, "Hight"),
                    new ComboItem((int)EImagesQuality.Best, "Best")
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

        public int ExportPageSizeViews
        {
            get
            {
                return m_ExportPageSizeViews;
            }

            set
            {
                m_ExportPageSizeViews = value;
                NotifyPropertyChanged("ExportPageSizeViews");
            }
        }

        public int ExportPageOrientationViews
        {
            get
            {
                return m_ExportPageOrientationViews;
            }

            set
            {
                m_ExportPageOrientationViews = value;
                NotifyPropertyChanged("ExportPageOrientationViews");
            }
        }

        public int ExportImagesQuality
        {
            get
            {
                return m_ExportImagesQuality;
            }

            set
            {
                m_ExportImagesQuality = value;
                NotifyPropertyChanged("ExportImagesQuality");
            }
        }

        public int ExportPageSizeViewsCladding
        {
            get
            {
                return m_ExportPageSizeViewsCladding;
            }

            set
            {
                m_ExportPageSizeViewsCladding = value;
                NotifyPropertyChanged("ExportPageSizeViewsCladding");
            }
        }

        public int ExportPageOrientationViewsCladding
        {
            get
            {
                return m_ExportPageOrientationViewsCladding;
            }

            set
            {
                m_ExportPageOrientationViewsCladding = value;
                NotifyPropertyChanged("ExportPageOrientationViewsCladding");
            }
        }

        public int ViewIndex
        {
            get
            {
                return m_ViewIndex;
            }

            set
            {
                m_ViewIndex = value;
                NotifyPropertyChanged("ViewIndex");
            }
        }

        public List<string> ModelViews
        {
            get
            {
                if (m_ModelViews == null) m_ModelViews = new List<string>() { "ISO Front-Right", "ISO Front-Left", "ISO Back-Right", "ISO Back-Left", "Front", "Back", "Left", "Right", "Top", "Current" };
                return m_ModelViews;
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

            ExportModelCladdingLayingSchemeViews = true;
            ExportModelCladdingLayingSchemeViewsFront = true;
            ExportModelCladdingLayingSchemeViewsBack = true;
            ExportModelCladdingLayingSchemeViewsLeft = true;
            ExportModelCladdingLayingSchemeViewsRight = true;
            ExportModelCladdingLayingSchemeViewsRoof = true;

            ExportJointTypes = true;
            ExportFootingTypes = true;
            ExportFloorDetails = true;
            ExportStandardDetails = true;

            ExportPageSize = (int)EPageSizes.A3;
            ExportPageOrientation = (int)EPageOrientation.Landscape;

            ExportPageSizeViews = (int)EPageSizes.A3;
            ExportPageOrientationViews = (int)EPageOrientation.Landscape;

            ExportPageSizeViewsCladding = (int)EPageSizes.A3;
            ExportPageOrientationViewsCladding = (int)EPageOrientation.Landscape;

            ExportImagesQuality = (int)EImagesQuality.Normal;

            ViewIndex = (int)EModelViews.ISO_FRONT_RIGHT;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(LayoutsExportOptionsViewModel vm)
        {
            if (vm == null) return;

            ExportModel3D = vm.ExportModel3D;
            ExportModelViews = vm.ExportModelViews;
            ExportModelViewsFront = vm.ExportModelViewsFront;
            ExportModelViewsBack = vm.ExportModelViewsBack;
            ExportModelViewsLeft = vm.ExportModelViewsLeft;
            ExportModelViewsRight = vm.ExportModelViewsRight;
            ExportModelViewsRoof = vm.ExportModelViewsRoof;
            ExportModelViewsMiddleFrame = vm.ExportModelViewsMiddleFrame;
            ExportModelViewsColumns = vm.ExportModelViewsColumns;
            ExportModelViewsFoundations = vm.ExportModelViewsFoundations;
            ExportModelViewsFloor = vm.ExportModelViewsFloor;

            ExportModelCladdingLayingSchemeViews = vm.ExportModelCladdingLayingSchemeViews;
            ExportModelCladdingLayingSchemeViewsFront = vm.ExportModelCladdingLayingSchemeViewsFront;
            ExportModelCladdingLayingSchemeViewsBack = vm.ExportModelCladdingLayingSchemeViewsBack;
            ExportModelCladdingLayingSchemeViewsLeft = vm.ExportModelCladdingLayingSchemeViewsLeft;
            ExportModelCladdingLayingSchemeViewsRight = vm.ExportModelCladdingLayingSchemeViewsRight;
            ExportModelCladdingLayingSchemeViewsRoof = vm.ExportModelCladdingLayingSchemeViewsRoof;

            ExportJointTypes = vm.ExportJointTypes;
            ExportFootingTypes = vm.ExportFootingTypes;
            ExportFloorDetails = vm.ExportFloorDetails;
            ExportStandardDetails = vm.ExportStandardDetails;
            ExportPageSize = vm.ExportPageSize;
            ExportPageOrientation = vm.ExportPageOrientation;
            ExportPageSizeViews = vm.ExportPageSizeViews;
            ExportPageOrientationViews = vm.ExportPageOrientationViews;
            ExportPageSizeViewsCladding = vm.ExportPageSizeViewsCladding;
            ExportPageOrientationViewsCladding = vm.ExportPageOrientationViewsCladding;
            ExportImagesQuality = vm.ExportImagesQuality;

            ViewIndex = vm.ViewIndex;
        }
    }
}