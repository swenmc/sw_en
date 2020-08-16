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
    public class OthersMembersInfo : INotifyPropertyChanged
    {        
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_IsSetFromCode;

        private string m_OthersComponentName;
        private string m_OthersSection;
        private string m_OthersSectionColor;
        private string m_OthersMaterial;        
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

        public string OthersComponentName
        {
            get
            {
                return m_OthersComponentName;
            }

            set
            {
                m_OthersComponentName = value;
                NotifyPropertyChanged("OthersComponentName");
            }
        }

        public string OthersSection
        {
            get
            {
                return m_OthersSection;
            }

            set
            {
                m_OthersSection = value;
                OthersSectionColor = CSectionManager.GetSectionColor(m_OthersSection);
                NotifyPropertyChanged("OthersSection");
            }
        }

        public string OthersSectionColor
        {
            get
            {
                return m_OthersSectionColor;
            }

            set
            {
                m_OthersSectionColor = value;
                NotifyPropertyChanged("OthersSectionColor");
            }
        }

        public string OthersMaterial
        {
            get
            {
                return m_OthersMaterial;
            }

            set
            {
                m_OthersMaterial = value;
                NotifyPropertyChanged("OthersMaterial");
            }
        }

        public OthersMembersInfo(string componentName, string section, string material, List<Helpers.CComboColor> colors, List<string> sections)
        {
            IsSetFromCode = false;
            Sections = sections;

            OthersComponentName = componentName;
            OthersSection = section;
            OthersMaterial = material;

            Colors = colors;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
