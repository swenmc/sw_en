using BaseClasses;
using BaseClasses.Helpers;
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
    public class FrameMembersInfo : INotifyPropertyChanged
    {        
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_IsSetFromCode;

        private int m_FrameID;
        private string m_ColumnSection;
        private string m_ColumnSectionColor;
        private string m_RafterSection;
        private string m_RafterSectionColor;
        private string m_ColumnMaterial;
        private string m_RafterMaterial;
        private List<Helpers.CComboColor> m_Colors;
        private List<string> m_Sections;

        public bool IsSetFromCode
        {
            get
            {
                return m_IsSetFromCode;
            }

            set
            {
                m_IsSetFromCode = value;
            }
        }

        public int FrameID
        {
            get
            {
                return m_FrameID;
            }

            set
            {
                m_FrameID = value;
            }
        }

        public string ColumnSection
        {
            get
            {
                return m_ColumnSection;
            }

            set
            {
                m_ColumnSection = value;
                ColumnSectionColor = CSectionManager.GetSectionColor(m_ColumnSection);
                NotifyPropertyChanged("ColumnSection");
            }
        }

        public string ColumnSectionColor
        {
            get
            {
                return m_ColumnSectionColor;
            }

            set
            {
                m_ColumnSectionColor = value;
                NotifyPropertyChanged("ColumnSectionColor");
            }
        }

        public string RafterSection
        {
            get
            {
                return m_RafterSection;
            }

            set
            {
                m_RafterSection = value;
                RafterSectionColor = CSectionManager.GetSectionColor(m_RafterSection);
                NotifyPropertyChanged("RafterSection");
            }
        }

        public string RafterSectionColor
        {
            get
            {
                return m_RafterSectionColor;
            }

            set
            {
                m_RafterSectionColor = value;
                NotifyPropertyChanged("RafterSectionColor");
            }
        }

        public string ColumnMaterial
        {
            get
            {
                return m_ColumnMaterial;
            }

            set
            {
                m_ColumnMaterial = value;
                NotifyPropertyChanged("ColumnMaterial");
            }
        }

        public string RafterMaterial
        {
            get
            {
                return m_RafterMaterial;
            }

            set
            {
                m_RafterMaterial = value;
                NotifyPropertyChanged("RafterMaterial");
            }
        }

        public List<CComboColor> Colors
        {
            get
            {
                return m_Colors;
            }

            set
            {
                m_Colors = value;
            }
        }

        public List<string> Sections
        {
            get
            {
                return m_Sections;
            }

            set
            {
                m_Sections = value;
            }
        }

        public FrameMembersInfo(int frameID, string columnSection, string rafterSection, string columnMaterial, string rafterMaterial, List<Helpers.CComboColor> colors, List<string> sections)
        {
            IsSetFromCode = false;
            Sections = sections;
            FrameID = frameID;
            ColumnSection = columnSection;
            RafterSection = rafterSection;            
            ColumnMaterial = columnMaterial;
            RafterMaterial = rafterMaterial;
            Colors = colors;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
