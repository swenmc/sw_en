using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.Results
{
    public class MaterialListMember
    {
        private string m_Prefix;
        private string m_CrScName;
        private int m_Quantity;
        private string m_MaterialName;
        private double m_Length;
        private double m_MassPerLength;
        private double m_MassPerPiece;
        private string m_LengthStr;
        private string m_MassPerLengthStr;
        private string m_MassPerPieceStr;
        private double m_TotalLength;
        private double m_TotalMass;
        private double m_TotalPrice;

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

        public string CrScName
        {
            get
            {
                return m_CrScName;
            }

            set
            {
                m_CrScName = value;
            }
        }

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

        public double Length
        {
            get
            {
                return m_Length;
            }

            set
            {
                m_Length = value;
            }
        }

        public double MassPerLength
        {
            get
            {
                return m_MassPerLength;
            }

            set
            {
                m_MassPerLength = value;
            }
        }

        public double MassPerPiece
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

        public double TotalLength
        {
            get
            {
                return m_TotalLength;
            }

            set
            {
                m_TotalLength = value;
            }
        }

        public double TotalMass
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

        public double TotalPrice
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

        public string LengthStr
        {
            get
            {
                return m_LengthStr;
            }

            set
            {
                m_LengthStr = value;
            }
        }

        public string MassPerLengthStr
        {
            get
            {
                return m_MassPerLengthStr;
            }

            set
            {
                m_MassPerLengthStr = value;
            }
        }

        public string MassPerPieceStr
        {
            get
            {
                return m_MassPerPieceStr;
            }

            set
            {
                m_MassPerPieceStr = value;
            }
        }

        public MaterialListMember() { }

    }
}
