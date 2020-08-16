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
    public class BayMembersInfo : INotifyPropertyChanged
    {        
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_IsSetFromCode;

        private int m_BayID;
        private string m_Section_EP;
        private string m_Section_G;
        private string m_Section_P;
        private string m_Section_GB;
        private string m_Section_PB;
        private string m_Section_CBW;
        private string m_Section_CBR;
        private string m_SectionColor_EP;
        private string m_SectionColor_G;
        private string m_SectionColor_P;
        private string m_SectionColor_GB;
        private string m_SectionColor_PB;
        private string m_SectionColor_CBW;
        private string m_SectionColor_CBR;
        private string m_Material_EP;
        private string m_Material_G;
        private string m_Material_P;
        private string m_Material_GB;
        private string m_Material_PB;
        private string m_Material_CBW;
        private string m_Material_CBR;

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

       

        

        public int BayID
        {
            get
            {
                return m_BayID;
            }

            set
            {
                m_BayID = value;
            }
        }

        public string Section_EP
        {
            get
            {
                return m_Section_EP;
            }

            set
            {
                m_Section_EP = value;
                m_SectionColor_EP = CSectionManager.GetSectionColor(m_Section_EP);
                NotifyPropertyChanged("Section_EP");
            }
        }

        public string Section_G
        {
            get
            {
                return m_Section_G;
            }

            set
            {
                m_Section_G = value;
                m_SectionColor_G = CSectionManager.GetSectionColor(m_Section_G);
                NotifyPropertyChanged("Section_G");
            }
        }

        public string Section_P
        {
            get
            {
                return m_Section_P;
            }

            set
            {
                m_Section_P = value;
                m_SectionColor_P = CSectionManager.GetSectionColor(m_Section_P);
                NotifyPropertyChanged("Section_P");
            }
        }

        public string Section_GB
        {
            get
            {
                return m_Section_GB;
            }

            set
            {
                m_Section_GB = value;
                m_SectionColor_GB = CSectionManager.GetSectionColor(m_Section_GB);
                NotifyPropertyChanged("Section_GB");
            }
        }

        public string Section_PB
        {
            get
            {
                return m_Section_PB;
            }

            set
            {
                m_Section_PB = value;
                m_SectionColor_PB = CSectionManager.GetSectionColor(m_Section_PB);
                NotifyPropertyChanged("Section_PB");
            }
        }

        public string Section_CBW
        {
            get
            {
                return m_Section_CBW;
            }

            set
            {
                m_Section_CBW = value;
                m_SectionColor_CBW = CSectionManager.GetSectionColor(m_Section_CBW);
                NotifyPropertyChanged("Section_CBW");
            }
        }

        public string Section_CBR
        {
            get
            {
                return m_Section_CBR;
            }

            set
            {
                m_Section_CBR = value;
                m_SectionColor_CBR = CSectionManager.GetSectionColor(m_Section_CBR);
                NotifyPropertyChanged("Section_CBR");
            }
        }

        public string SectionColor_EP
        {
            get
            {
                return m_SectionColor_EP;
            }

            set
            {
                m_SectionColor_EP = value;
                NotifyPropertyChanged("SectionColor_EP");
            }
        }

        public string SectionColor_G
        {
            get
            {
                return m_SectionColor_G;
            }

            set
            {
                m_SectionColor_G = value;
                NotifyPropertyChanged("SectionColor_G");
            }
        }

        public string SectionColor_P
        {
            get
            {
                return m_SectionColor_P;
            }

            set
            {
                m_SectionColor_P = value;
                NotifyPropertyChanged("SectionColor_P");
            }
        }

        public string SectionColor_GB
        {
            get
            {
                return m_SectionColor_GB;
            }

            set
            {
                m_SectionColor_GB = value;
                NotifyPropertyChanged("SectionColor_GB");
            }
        }

        public string SectionColor_PB
        {
            get
            {
                return m_SectionColor_PB;
            }

            set
            {
                m_SectionColor_PB = value;
                NotifyPropertyChanged("SectionColor_PB");
            }
        }

        public string SectionColor_CBW
        {
            get
            {
                return m_SectionColor_CBW;
            }

            set
            {
                m_SectionColor_CBW = value;
                NotifyPropertyChanged("SectionColor_CBW");
            }
        }

        public string SectionColor_CBR
        {
            get
            {
                return m_SectionColor_CBR;
            }

            set
            {
                m_SectionColor_CBR = value;
                NotifyPropertyChanged("SectionColor_CBR");
            }
        }

        public string Material_EP
        {
            get
            {
                return m_Material_EP;
            }

            set
            {
                m_Material_EP = value;
                NotifyPropertyChanged("Material_EP");
            }
        }

        public string Material_G
        {
            get
            {
                return m_Material_G;
            }

            set
            {
                m_Material_G = value;
                NotifyPropertyChanged("Material_G");
            }
        }

        public string Material_P
        {
            get
            {
                return m_Material_P;
            }

            set
            {
                m_Material_P = value;
                NotifyPropertyChanged("Material_P");
            }
        }

        public string Material_GB
        {
            get
            {
                return m_Material_GB;
            }

            set
            {
                m_Material_GB = value;
                NotifyPropertyChanged("Material_GB");
            }
        }

        public string Material_PB
        {
            get
            {
                return m_Material_PB;
            }

            set
            {
                m_Material_PB = value;
                NotifyPropertyChanged("Material_PB");
            }
        }

        public string Material_CBW
        {
            get
            {
                return m_Material_CBW;
            }

            set
            {
                m_Material_CBW = value;
                NotifyPropertyChanged("Material_CBW");
            }
        }

        public string Material_CBR
        {
            get
            {
                return m_Material_CBR;
            }

            set
            {
                m_Material_CBR = value;
                NotifyPropertyChanged("Material_CBR");
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

        public BayMembersInfo(int bayID, string section_EP, string section_G, string section_P, string section_GB, string section_PB, string section_CBW, string section_CBR,
            string material_EP, string material_G, string material_P, string material_GB, string material_PB, string material_CBW, string material_CBR,
            List<Helpers.CComboColor> colors, List<string> sections)
        {
            IsSetFromCode = false;
            Sections = sections;
            BayID = bayID;

            Section_EP = section_EP;
            Section_G = section_G;
            Section_P = section_P;
            Section_GB = section_GB;
            Section_PB = section_PB;
            Section_CBW = section_CBW;
            Section_CBR = section_CBR;

            Material_EP = material_EP;
            Material_G = material_G;
            Material_P = material_P;
            Material_GB = material_GB;
            Material_PB = material_PB;
            Material_CBW = material_CBW;
            Material_CBR = material_CBR;

            Colors = colors;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
