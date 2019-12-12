using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace PFD
{
    public class QuotationViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        double m_BuildingNetPrice_WithoutMargin_WithoutGST;

        double m_MarginAbsolute;
        double m_Margin_Percentage;
        double m_BuildingPrice_WithMargin_WithoutGST;
        double m_GST_Absolute;
        double m_GST_Percentage;
        double m_TotalBuildingPrice_IncludingGST;

        float m_BuildingArea_Gross;
        float m_BuildingVolume_Gross;
        double m_BuildingMass;

        double m_BuildingPrice_PSM;
        double m_BuildingPrice_PCM;
        double m_BuildingPrice_PPKG;
        
        public bool IsSetFromCode = false;


        //-------------------------------------------------------------------------------------------------------------
        public double BuildingNetPrice_WithoutMargin_WithoutGST
        {
            get
            {
                return m_BuildingNetPrice_WithoutMargin_WithoutGST;
            }

            set
            {
                m_BuildingNetPrice_WithoutMargin_WithoutGST = value;
                NotifyPropertyChanged("BuildingNetPrice_WithoutMargin_WithoutGST");
            }
        }

        public double MarginAbsolute
        {
            get
            {
                return m_MarginAbsolute;
            }

            set
            {
                m_MarginAbsolute = value;
                NotifyPropertyChanged("MarginAbsolute");
            }
        }

        public double BuildingPrice_WithMargin_WithoutGST
        {
            get
            {
                return m_BuildingPrice_WithMargin_WithoutGST;
            }

            set
            {
                m_BuildingPrice_WithMargin_WithoutGST = value;
                NotifyPropertyChanged("BuildingPrice_WithMargin_WithoutGST");
            }
        }

        public double GST_Absolute
        {
            get
            {
                return m_GST_Absolute;
            }

            set
            {
                m_GST_Absolute = value;
                NotifyPropertyChanged("GST_Absolute");
            }
        }

        public double TotalBuildingPrice_IncludingGST
        {
            get
            {
                return m_TotalBuildingPrice_IncludingGST;
            }

            set
            {
                m_TotalBuildingPrice_IncludingGST = value;
                NotifyPropertyChanged("TotalBuildingPrice_IncludingGST");
            }
        }

        public float BuildingArea_Gross
        {
            get
            {
                return m_BuildingArea_Gross;
            }

            set
            {
                m_BuildingArea_Gross = value;
                NotifyPropertyChanged("BuildingArea_Gross");
            }
        }

        public float BuildingVolume_Gross
        {
            get
            {
                return m_BuildingVolume_Gross;
            }

            set
            {
                m_BuildingVolume_Gross = value;
                NotifyPropertyChanged("BuildingVolume_Gross");
            }
        }

        public double BuildingMass
        {
            get
            {
                return m_BuildingMass;
            }

            set
            {
                m_BuildingMass = value;
                NotifyPropertyChanged("BuildingMass");
            }
        }

        public double BuildingPrice_PSM
        {
            get
            {
                return m_BuildingPrice_PSM;
            }

            set
            {
                m_BuildingPrice_PSM = value;
                NotifyPropertyChanged("BuildingPrice_PSM");
            }
        }

        public double BuildingPrice_PCM
        {
            get
            {
                return m_BuildingPrice_PCM;
            }

            set
            {
                m_BuildingPrice_PCM = value;
                NotifyPropertyChanged("BuildingPrice_PCM");
            }
        }

        public double BuildingPrice_PPKG
        {
            get
            {
                return m_BuildingPrice_PPKG;
            }

            set
            {
                m_BuildingPrice_PPKG = value;
                NotifyPropertyChanged("BuildingPrice_PPKG");
            }
        }

        public double GST_Percentage
        {
            get
            {
                return m_GST_Percentage;
            }

            set
            {
                m_GST_Percentage = value;
                NotifyPropertyChanged("GST_Percentage");
            }
        }

        public double Margin_Percentage
        {
            get
            {
                return m_Margin_Percentage;
            }

            set
            {
                m_Margin_Percentage = value;
                NotifyPropertyChanged("Margin_Percentage");
            }
        }



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public QuotationViewModel()
        {            
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
