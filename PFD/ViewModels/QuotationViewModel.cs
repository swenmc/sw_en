using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace PFD
{
    [Serializable]
    public class QuotationViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        double m_BuildingNetPrice_WithoutMargin_WithoutGST;

        double m_Margin_Absolute;
        double m_Margin_Percentage;
        double m_Markup_Absolute;
        double m_Markup_Percentage;

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

        double m_Freight;

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

        public double Margin_Absolute
        {
            get
            {
                return m_Margin_Absolute;
            }

            set
            {
                m_Margin_Absolute = value;
                NotifyPropertyChanged("Margin_Absolute");
            }
        }

        public double Markup_Absolute
        {
            get
            {
                return m_Markup_Absolute;
            }

            set
            {
                m_Markup_Absolute = value;
                NotifyPropertyChanged("Markup_Absolute");
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
                if (value < 0.0 || value > 30)
                    throw new ArgumentException("GST must be between 0.0 and 30 [%]");
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
                if (value < 0.0 || value > 80)
                    throw new ArgumentException("Margin must be between 0.0 and 80 [%]");
                m_Margin_Percentage = value;
                NotifyPropertyChanged("Margin_Percentage");
            }
        }

        public double Markup_Percentage
        {
            get
            {
                return m_Markup_Percentage;
            }

            set
            {
                //if (value < 0.0 || value > 70)
                //    throw new ArgumentException("Markup must be between 0.0 and 70 [%]");
                m_Markup_Percentage = value;
                NotifyPropertyChanged("Markup_Percentage");
            }
        }

        public double Freight
        {
            get
            {
                return m_Freight;
            }

            set
            {
                if (value < 0.0 || value > 1000000)
                    throw new ArgumentException("Freight must be between 0.0 and 1000000 NZD");
                m_Freight = value;
                NotifyPropertyChanged("Freight");
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
