using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CTEKScrewProp
    {
        public int ID;
        public int gauge;
        public double threadDiameter;
        public double shankDiameter;
        public double shankLength;
        public string threadType1;
        public int threadsPerInch1;
        public string threadType2;
        public int threadsPerInch2;
        public string threadType3;
        public int threadsPerInch3;
        public double headSizeInch;
        public double headSize;
        public double headAgonThickness;
        public double headTotalThickness;
        public double washerSize;
        public double washerThickness;
        public double preDrillHoleDiameter_3mmthickness;
        public double shearStrength_N;
        public double axialTensileStrength_N;
        public double torsionalStrength_Nm;
        public double mass_kg;
        public double price_PPP_NZD;
        public double price_PPKG_NZD;

        public CTEKScrewProp() { }
    }
}
