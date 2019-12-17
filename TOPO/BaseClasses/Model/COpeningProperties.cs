using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    // Mala by to byt struktura spolocna pre door a window
    // Mozno by to mal byt skor predok tried BaseClasses DoorProperties a WindowProperties, ale nebol som si isty ci to nerozbijem, tak som to dal len tu
    public class COpeningProperties
    {
        // Type           | Width | Height | Count | Area  | Total Area | Price PSM | Price PP  | Total Price
        // Roller Door    | 3.3   | 4.5    | 5     | 12.20 | 62.21      | 301       | 3000      | 18000
        // Personnel Door | 1.0   | 2.1    | 4     |  2.20 |  8.21      | 350       | 700       |  2800
        // Personnel Door | 0.8   | 2.0    | 2     |  1.80 |  3.60      | 350       | 650       |  1300

        private string m_sType; // Roller Door, Personnel Door, Window
        private string m_sBuildingSide;
        private int m_iBayNumber;
        private float m_fHeight;
        private float m_fWidth;
        private CoatingColour m_coatingColor;

        private float m_fPerimeter;
        private float m_fArea;

        private float m_fUnitMass_SM; // Unit mass per square meter
        private float m_fPrice_PPSM_NZD; // Price per square meter
        private float m_fPrice_PPKG_NZD; // Price per kilogram
        private float m_fPrice_PPP_NZD; // Price per piece

        private int m_ColorID; // TODO Ondrej - Pracovat s ID alebo priamo s Color
        private int m_Count;

        public string Type
        {
            get
            {
                return m_sType;
            }

            set
            {
                m_sType = value;
            }
        }

        public string sBuildingSide
        {
            get
            {
                return m_sBuildingSide;
            }

            set
            {
                m_sBuildingSide = value;
            }
        }

        public int iBayNumber
        {
            get
            {
                return m_iBayNumber;
            }

            set
            {
                m_iBayNumber = value;

            }
        }

        public float fHeight
        {
            get
            {
                return m_fHeight;
            }

            set
            {
                m_fHeight = value;
            }
        }

        public float fWidth
        {
            get
            {
                return m_fWidth;
            }

            set
            {
                m_fWidth = value;
            }
        }

        public CoatingColour CoatingColor
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

        public float Perimeter
        {
            get
            {
                return m_fPerimeter;
            }

            set
            {
                m_fPerimeter = value;
            }
        }

        public float Area
        {
            get
            {
                return m_fArea;
            }

            set
            {
                m_fArea = value;
            }
        }

        public float UnitMass_SM
        {
            get
            {
                return m_fUnitMass_SM;
            }

            set
            {
                m_fUnitMass_SM = value;
            }
        }

        public float Price_PPSM_NZD
        {
            get
            {
                return m_fPrice_PPSM_NZD;
            }

            set
            {
                m_fPrice_PPSM_NZD = value;
            }
        }

        public float Price_PPKG_NZD
        {
            get
            {
                return m_fPrice_PPKG_NZD;
            }

            set
            {
                m_fPrice_PPKG_NZD = value;
            }
        }

        public float Price_PPP_NZD
        {
            get
            {
                return m_fPrice_PPP_NZD;
            }

            set
            {
                m_fPrice_PPP_NZD = value;
            }
        }

        public int ColorID
        {
            get
            {
                return m_ColorID;
            }

            set
            {
                m_ColorID = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Count;
            }

            set
            {
                m_Count = value;
            }
        }

        public COpeningProperties(string type, float width, float height, int coatingColorID)
        {
            m_sType = type;
            m_fWidth = width;
            m_fHeight = height;

            if (m_sType == "Window")
                m_fPerimeter = 2 * width + 2 * height;
            else
                m_fPerimeter = width + 2 * height; // ??? Uvazovat pre dvere prah  - zatial sa nikde nepouziva

            m_fArea = width * height;

            CPlaneItemProperties prop = CPlaneItemManager.GetPlaneItemProperties(m_sType, "DoorsAndWindows");

            m_fUnitMass_SM = (float)prop.Mass_kg_m2;

            ColorID = coatingColorID; // TODO - napojit index na GUI
            bool bIsDomesticSeries = true; // TODO - napojit na GUI, combobox v stplci s nazv om Series s dvomi polozka "Domestic" a "Roller Shutter"
            List <CoatingColour> colorsProp = CCoatingColorManager.LoadColours("AccessoriesSQLiteDB");
            m_coatingColor = colorsProp.ElementAtOrDefault(ColorID);
            m_fPrice_PPSM_NZD = GetDoorPriceByColor_PSM_NZD(colorsProp.ElementAtOrDefault(ColorID).PriceCode, prop);
            m_fPrice_PPKG_NZD = GetDoorPriceByColor_PPKG_NZD(colorsProp.ElementAtOrDefault(ColorID).PriceCode, prop);

            m_fPrice_PPP_NZD = m_fPrice_PPSM_NZD * m_fArea;

            if (m_sType == "Roller Door")
            {
                m_fPrice_PPP_NZD = CRollerDoorPricesManager.GetRollerDoorPrice(bIsDomesticSeries, m_fWidth, m_fHeight, colorsProp.ElementAtOrDefault(ColorID).Name == "Zinc"); // Zinc - ID farby v databaze je 23
                m_fPrice_PPSM_NZD = m_fPrice_PPP_NZD / m_fArea;
                m_fPrice_PPKG_NZD = m_fPrice_PPSM_NZD / m_fUnitMass_SM;
            }

            m_Count = 1;
        }

        public override bool Equals(object obj)
        {
            COpeningProperties op = obj as COpeningProperties;
            if (op == null) return false;

            return this.Type == op.Type && MathF.d_equal(this.fWidth, op.fWidth) && MathF.d_equal(this.fHeight, op.fHeight) && this.ColorID == op.ColorID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private float GetDoorPriceByColor_PSM_NZD(int priceCode, CPlaneItemProperties prop)
        {
            float fDoorPrice1_PSM_NZD = (float)prop.Price1_PPSM_NZD; // Cena door za 1 m^2 (Basic)
            float fDoorPrice2_PSM_NZD = (float)prop.Price2_PPSM_NZD; // Cena door za 1 m^2 (FormClad)
            float fDoorPrice3_PSM_NZD = (float)prop.Price3_PPSM_NZD; // Cena door za 1 m^2 (AZ)

            // TODO Ondrej - toto sa asi da urobit krajsie cez nejaku relacnu databazu alebo nieco take
            if (priceCode == 1)
                return fDoorPrice1_PSM_NZD;
            else if (priceCode == 2)
                return fDoorPrice2_PSM_NZD;
            else if (priceCode == 3)
                return fDoorPrice3_PSM_NZD;
            else
            {
                throw new Exception("Invalid door price code.");
            }
        }

        private float GetDoorPriceByColor_PPKG_NZD(int priceCode, CPlaneItemProperties prop)
        {
            float fDoorPrice1_PKG_NZD = (float)prop.Price1_PPKG_NZD; // Cena door za 1 m^2 (Basic)
            float fDoorPrice2_PKG_NZD = (float)prop.Price2_PPKG_NZD; // Cena door za 1 m^2 (FormClad)
            float fDoorPrice3_PKG_NZD = (float)prop.Price3_PPKG_NZD; // Cena door za 1 m^2 (AZ)

            // TODO Ondrej - toto sa asi da urobit krajsie cez nejaku relacnu databazu alebo nieco take
            if (priceCode == 1)
                return fDoorPrice1_PKG_NZD;
            else if (priceCode == 2)
                return fDoorPrice2_PKG_NZD;
            else if (priceCode == 3)
                return fDoorPrice3_PKG_NZD;
            else
            {
                throw new Exception("Invalid door price code.");
            }
        }
    }
}
