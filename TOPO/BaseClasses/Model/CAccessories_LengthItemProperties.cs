using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    public class CAccessories_LengthItemProperties : INotifyPropertyChanged
    {
        // Flashing / Gutter

        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Name;
        private double m_thickness;
        private double m_width_total;
        private double m_length_total;
        private CoatingColour m_coatingColor;
        //private Color m_color;

        private double m_density_kg_m3;
        private double m_mass_kg_m2;
        private double m_mass_kg_lm;
        private double m_price_PPLM_NZD;
        private double m_price_PPSM_NZD;
        private double m_price_PPKG_NZD;

        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }

        public double Thickness
        {
            get
            {
                return m_thickness;
            }

            set
            {
                m_thickness = value;
            }
        }

        public double Width_total
        {
            get
            {
                return m_width_total;
            }

            set
            {
                m_width_total = value;
            }
        }

        public double Length_total
        {
            get
            {
                return m_length_total;
            }

            set
            {
                m_length_total = value;
            }
        }

        /*
        public Color color
        {
            get
            {
                return m_color;
            }

            set
            {
                m_color = value;
            }
        }
        */

        public CoatingColour coatingColor
        {
            get
            {
                return m_coatingColor;
            }

            set
            {
                m_coatingColor = value;
            }
        }

        public double Density_kg_m3
        {
            get
            {
                return m_density_kg_m3;
            }

            set
            {
                m_density_kg_m3 = value;
            }
        }

        public double Mass_kg_m2
        {
            get
            {
                return m_mass_kg_m2;
            }

            set
            {
                m_mass_kg_m2 = value;
            }
        }

        public double Mass_kg_lm
        {
            get
            {
                return m_mass_kg_lm;
            }

            set
            {
                m_mass_kg_lm = value;
            }
        }

        public double Price_PPLM_NZD
        {
            get
            {
                return m_price_PPLM_NZD;
            }

            set
            {
                m_price_PPLM_NZD = value;
            }
        }

        public double Price_PPSM_NZD
        {
            get
            {
                return m_price_PPSM_NZD;
            }

            set
            {
                m_price_PPSM_NZD = value;
            }
        }

        public double Price_PPKG_NZD
        {
            get
            {
                return m_price_PPKG_NZD;
            }

            set
            {
                m_price_PPKG_NZD = value;
            }
        }

        public CAccessories_LengthItemProperties()
        {
        }

        public CAccessories_LengthItemProperties(string databaseName, string databaseTable, double totalLength, int colorIndex)
        {
            m_Name = databaseName;
            m_length_total = totalLength;
            //m_color = (Color)ColorConverter.ConvertFromString(CCoatingColorManager.LoadCoatingProperties(colorIndex).CodeHEX);
            m_coatingColor = CCoatingColorManager.LoadCoatingProperties(colorIndex);
            SetParametersFromDatabase(databaseName, databaseTable);
        }

        private void SetParametersFromDatabase(string databaseName, string databaseTable)
        {
            CLengthItemProperties prop = CLengthItemManager.GetLengthItemProperties(databaseName, databaseTable);
            m_thickness = prop.Thickness;
            m_width_total = prop.Width_total;

            //Color zinc = (Color)ColorConverter.ConvertFromString(CCoatingColorManager.LoadCoatingProperties("Zinc").CodeHEX);
            CoatingColour zinc = CCoatingColorManager.LoadCoatingProperties("Zinc");

            if (m_coatingColor == zinc) // Zinc
            {
                m_density_kg_m3 = prop.Density4_kg_m3;
                m_mass_kg_m2 = prop.Mass4_kg_m2;
                m_mass_kg_lm = prop.Mass4_kg_lm;
                m_price_PPLM_NZD = prop.Price4_PPLM_NZD;
                m_price_PPSM_NZD = prop.Price4_PPSM_NZD;
                m_price_PPKG_NZD = prop.Price4_PPKG_NZD;
            }
            else
            {
                m_density_kg_m3 = prop.Density1_kg_m3;
                m_mass_kg_m2 = prop.Mass1_kg_m2;
                m_mass_kg_lm = prop.Mass1_kg_lm;
                m_price_PPLM_NZD = prop.Price1_PPLM_NZD;
                m_price_PPSM_NZD = prop.Price1_PPSM_NZD;
                m_price_PPKG_NZD = prop.Price1_PPKG_NZD;
            }
    }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
