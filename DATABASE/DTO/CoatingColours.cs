﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    [Serializable]
    public class CoatingColour
    {
        int MID;
        string MName;
        string MCodeRGB;
        string MCodeHEX;
        string MCodeHSV;
        int MPriceCode;

        public int ID
        {
            get
            {
                return MID;
            }

            set
            {
                MID = value;
            }
        }

        public string Name
        {
            get
            {
                return MName;
            }

            set
            {
                MName = value;
            }
        }

        public string CodeRGB
        {
            get
            {
                return MCodeRGB;
            }

            set
            {
                MCodeRGB = value;
            }
        }

        public string CodeHEX
        {
            get
            {
                return MCodeHEX;
            }

            set
            {
                MCodeHEX = value;
            }
        }

        public string CodeHSV
        {
            get
            {
                return MCodeHSV;
            }

            set
            {
                MCodeHSV = value;
            }
        }

        public int PriceCode
        {
            get
            {
                return MPriceCode;
            }

            set
            {
                MPriceCode = value;
            }
        }

        public CoatingColour() { }

        public override bool Equals(object obj)
        {
            if (obj is CoatingColour)
            {
                return this.ID == ((CoatingColour)obj).ID;
            }
            else return false;            
        }
    }
}
