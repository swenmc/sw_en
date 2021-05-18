using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class ColumnProperties : DataColumn
    {
        public float EP_Width;
        public string EP_Unit;
        public object EP_Alignment;

        public ColumnProperties() { }
        public ColumnProperties(Type cType, string sName, string sCaption, float fWidth, string sUnit, object eAlignment)
        {
            DataType = cType;
            ColumnName = sName;
            Caption = sCaption;

            EP_Width = fWidth;
            EP_Unit = sUnit;
            EP_Alignment = eAlignment;
        }
    }
}
