using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    [Serializable]
    public class CAccessories_LengthItemProperties : INotifyPropertyChanged
    {
        // Flashing / Gutter
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_ID;
        private string m_Name;
        private string m_NameOld;
        private string m_DatabaseTable;
        private double m_thickness;
        private double m_width_total;
        private double m_length_total;
        private CoatingColour m_coatingColor;
        private List<CoatingColour> m_CoatingColors;
        //private Color m_color;

        private double m_density_kg_m3;
        private double m_mass_kg_m2;
        private double m_mass_kg_lm;
        private double m_price_PPLM_NZD;
        private double m_price_PPSM_NZD;
        private double m_price_PPKG_NZD;

        private CLengthItemProperties m_properties;

        public int ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_NameOld = m_Name;
                m_Name = value;
                SetParametersFromDatabase();
                NotifyPropertyChanged("Name");
            }
        }
        public string NameOld
        {
            get
            {
                return m_NameOld;
            }

            set
            {
                m_NameOld = value;
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
                NotifyPropertyChanged("Thickness");
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
                if (!double.IsNaN(value) && (value < 0.1 || value > 1))
                {
                    MessageBox.Show($"Total width must be between 0.1 and 1 [m]");
                }
                else
                {
                    m_width_total = value;
                    NotifyPropertyChanged("Width_total");
                }
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

        public CoatingColour CoatingColor
        {
            get
            {
                return m_coatingColor;
            }

            set
            {
                m_coatingColor = value;
                SetColorProperties();
                NotifyPropertyChanged("CoatingColor");
            }
        }

        public List<CoatingColour> CoatingColors
        {
            get
            {
                if (m_CoatingColors == null) m_CoatingColors = CCoatingColorManager.LoadColours("AccessoriesSQLiteDB");
                return m_CoatingColors;
            }

            set
            {
                m_CoatingColors = value;
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

        public string DatabaseTable
        {
            get
            {
                return m_DatabaseTable;
            }

            set
            {
                m_DatabaseTable = value;
            }
        }

        public CAccessories_LengthItemProperties()
        {
        }

        public CAccessories_LengthItemProperties(string databaseName, string databaseTable, double totalLength, int colorIndex)
        {
            //m_ID = id;
            m_Name = databaseName;
            m_DatabaseTable = databaseTable;
            m_length_total = totalLength;
            m_coatingColor = CoatingColors[colorIndex];
            SetParametersFromDatabase();
        }

        private void SetParametersFromDatabase()
        {
            m_properties = CLengthItemManager.GetLengthItemProperties(m_Name, m_DatabaseTable);
            Thickness = m_properties.Thickness * 1000; // z [m] do [mm]
            Width_total = m_properties.Width_total;
            ID = m_properties.ID;

            SetColorProperties();
        }

        private void SetColorProperties()
        {
            //Color zinc = (Color)ColorConverter.ConvertFromString(CCoatingColorManager.LoadCoatingProperties("Zinc").CodeHEX);
            //CoatingColour zinc = CCoatingColorManager.LoadCoatingProperties("Zinc");

            if (m_coatingColor.Name == "Zinc") // Zinc
            {
                m_density_kg_m3 = m_properties.Density4_kg_m3;
                m_mass_kg_m2 = m_properties.Mass4_kg_m2;
                m_mass_kg_lm = m_properties.Mass4_kg_lm;
                m_price_PPLM_NZD = m_properties.Price4_PPLM_NZD;
                m_price_PPSM_NZD = m_properties.Price4_PPSM_NZD;
                m_price_PPKG_NZD = m_properties.Price4_PPKG_NZD;
            }
            else
            {
                m_density_kg_m3 = m_properties.Density1_kg_m3;
                m_mass_kg_m2 = m_properties.Mass1_kg_m2;
                m_mass_kg_lm = m_properties.Mass1_kg_lm;
                m_price_PPLM_NZD = m_properties.Price1_PPLM_NZD;
                m_price_PPSM_NZD = m_properties.Price1_PPSM_NZD;
                m_price_PPKG_NZD = m_properties.Price1_PPKG_NZD;
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