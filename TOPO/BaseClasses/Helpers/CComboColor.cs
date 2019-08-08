using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaseClasses.Helpers
{
    public class CComboColor
    {
        int MID;
        string MName;
        string MCodeRGB;
        string MCodeHEX;
        string MCodeHSV;
        Color MColor;

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

        public Color Color
        {
            get
            {
                return MColor;
            }

            set
            {
                MColor = value;
            }
        }

        public CComboColor()
        {

        }

        public CComboColor(string name, Color color)
        {
            MName = name;
            MColor = color;
            MCodeHEX = color.ToString();
        }
    }
}
