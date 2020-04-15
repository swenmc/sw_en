using BaseClasses.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    [Serializable]
    public class DocumentationSettingsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool m_ExportMembersXLS;
        private bool m_ExportPlatesPDF;
        private bool m_ExportCNCSetup;
        private bool m_ExportCNCDrilling;
        private bool m_Export2D_DXF;
        private bool m_Export3D_DXF;
        private bool m_ExportSCV;
        
        
        public bool ExportMembersXLS
        {
            get
            {
                return m_ExportMembersXLS;
            }

            set
            {
                m_ExportMembersXLS = value;
                NotifyPropertyChanged("ExportMembersXLS");
            }
        }

        public bool ExportPlatesPDF
        {
            get
            {
                return m_ExportPlatesPDF;
            }

            set
            {
                m_ExportPlatesPDF = value;
                NotifyPropertyChanged("ExportPlatesPDF");
            }
        }

        public bool ExportCNCSetup
        {
            get
            {
                return m_ExportCNCSetup;
            }

            set
            {
                m_ExportCNCSetup = value;
                NotifyPropertyChanged("ExportCNCSetup");
            }
        }

        public bool ExportCNCDrilling
        {
            get
            {
                return m_ExportCNCDrilling;
            }

            set
            {
                m_ExportCNCDrilling = value;
                NotifyPropertyChanged("ExportCNCDrilling");
            }
        }

        public bool Export2D_DXF
        {
            get
            {
                return m_Export2D_DXF;
            }

            set
            {
                m_Export2D_DXF = value;
                NotifyPropertyChanged("Export2D_DXF");
            }
        }

        public bool Export3D_DXF
        {
            get
            {
                return m_Export3D_DXF;
            }

            set
            {
                m_Export3D_DXF = value;
                NotifyPropertyChanged("Export3D_DXF");
            }
        }

        public bool ExportSCV
        {
            get
            {
                return m_ExportSCV;
            }

            set
            {
                m_ExportSCV = value;
                NotifyPropertyChanged("ExportSCV");
            }
        }

        public DocumentationSettingsViewModel()
        {
            ExportMembersXLS = true;
            ExportPlatesPDF = true;
            ExportCNCSetup = true;
            ExportCNCDrilling = true;
            Export2D_DXF = true;
            Export3D_DXF = true;
            ExportSCV = true;            
        }


        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
