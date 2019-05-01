using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CComponentPrefixes
    {
        int MID;
        string MComponentPrefix;
        string MComponentName;
        string MComponentColorCodeRGB;
        string MComponentColorName;

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

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

        public string ComponentPrefix
        {
            get
            {
                return MComponentPrefix;
            }

            set
            {
                MComponentPrefix = value;
            }
        }

        public string ComponentName
        {
            get
            {
                return MComponentName;
            }

            set
            {
                MComponentName = value;
            }
        }

        public string ComponentColorCodeRGB
        {
            get
            {
                return MComponentColorCodeRGB;
            }

            set
            {
                MComponentColorCodeRGB = value;
            }
        }

        public string ComponentColorName
        {
            get
            {
                return MComponentColorName;
            }

            set
            {
                MComponentColorName = value;
            }
        }

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public CComponentPrefixes() { }
    }
}
