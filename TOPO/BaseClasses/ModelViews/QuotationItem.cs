using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class QuotationItem
    {
        string m_Prefix;
        //string m_Name;
        int m_Quantity;
        string m_MaterialName;
        float m_Width_bx;
        float m_Height_hy;
        float m_Ft;
        float m_Area;
        float m_MassPerPiece;
        float m_TotalArea;
        float m_TotalMass;
        float m_PricePerPiece;
        float m_TotalPrice;


        public string Prefix
        {
            get
            {
                return m_Prefix;
            }

            set
            {
                m_Prefix = value;
            }
        }
        //public string Name
        //{
        //    get
        //    {
        //        return m_Name;
        //    }

        //    set
        //    {
        //        m_Name = value;
        //    }
        //}

        public int Quantity
        {
            get
            {
                return m_Quantity;
            }

            set
            {
                m_Quantity = value;
            }
        }

        public string MaterialName
        {
            get
            {
                return m_MaterialName;
            }

            set
            {
                m_MaterialName = value;
            }
        }

        public float Width_bx
        {
            get
            {
                return m_Width_bx;
            }

            set
            {
                m_Width_bx = value;
            }
        }

        public float Height_hy
        {
            get
            {
                return m_Height_hy;
            }

            set
            {
                m_Height_hy = value;
            }
        }

        public float Ft
        {
            get
            {
                return m_Ft;
            }

            set
            {
                m_Ft = value;
            }
        }

        public float Area
        {
            get
            {
                return m_Area;
            }

            set
            {
                m_Area = value;
            }
        }

        public float MassPerPiece
        {
            get
            {
                return m_MassPerPiece;
            }

            set
            {
                m_MassPerPiece = value;
            }
        }

        public float TotalArea
        {
            get
            {
                return m_TotalArea;
            }

            set
            {
                m_TotalArea = value;
            }
        }

        public float TotalMass
        {
            get
            {
                return m_TotalMass;
            }

            set
            {
                m_TotalMass = value;
            }
        }

        public float PricePerPiece
        {
            get
            {
                return m_PricePerPiece;
            }

            set
            {
                m_PricePerPiece = value;
            }
        }

        public float TotalPrice
        {
            get
            {
                return m_TotalPrice;
            }

            set
            {
                m_TotalPrice = value;
            }
        }


        public QuotationItem() { }
    }
}
