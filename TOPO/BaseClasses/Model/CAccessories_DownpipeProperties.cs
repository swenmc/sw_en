using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    public class CAccessories_DownpipeProperties : INotifyPropertyChanged
    {
        // Downpipe

        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Name;
        private string m_Shape;
        private double m_diameter;
        private double m_length_total;
        private double m_length_piece;
        private CoatingColour m_coatingColor;
        //private Color m_color;

        private double m_mass_kg_lm;
        private double m_mass_kg_piece;
        private double m_price_PPLM_NZD;
        private double m_price_PPP_NZD;

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

        public string Shape
        {
            get
            {
                return m_Shape;
            }

            set
            {
                m_Shape = value;
            }
        }

        public double Diameter
        {
            get
            {
                return m_diameter;
            }

            set
            {
                m_diameter = value;
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

        public double Length_piece
        {
            get
            {
                return m_length_piece;
            }

            set
            {
                m_length_piece = value;
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

        public double Mass_kg_piece
        {
            get
            {
                return m_mass_kg_piece;
            }

            set
            {
                m_mass_kg_piece = value;
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

        public double Price_PPP_NZD
        {
            get
            {
                return m_price_PPP_NZD;
            }

            set
            {
                m_price_PPP_NZD = value;
            }
        }

        public CAccessories_DownpipeProperties()
        {
        }

        public CAccessories_DownpipeProperties(string name, double totalLength, int colorIndex)
        {
            m_Name = name;
            m_length_total = totalLength;
            //m_color = (Color)ColorConverter.ConvertFromString(CCoatingColorManager.LoadCoatingProperties(colorIndex).CodeHEX);
            m_coatingColor = CCoatingColorManager.LoadCoatingProperties(colorIndex);
            SetParametersFromDatabase();
        }

        private void SetParametersFromDatabase()
        {
            CDownpipeProperties prop = CDownpipesManager.GetDownpipesProperties(m_Name);

            m_Shape = prop.Shape;
            m_diameter = prop.Diameter;
            m_length_piece = prop.Length;
            m_mass_kg_lm = prop.Mass_kg_lm;
            m_mass_kg_piece = prop.Mass_kg_piece;
            m_price_PPLM_NZD = prop.Price_PPLM_NZD;
            m_price_PPP_NZD = prop.Price_PPP_NZD;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
