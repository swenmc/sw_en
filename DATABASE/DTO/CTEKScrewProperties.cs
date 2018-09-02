using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CTEKScrewProperties
    {
        public int ID;
        public string gauge;
        public string threadDiameter;
        public string shankDiameter;
        public string threadType1;
        public string threadsPerInch1;
        public string threadType2;
        public string threadsPerInch2;
        public string threadType3;
        public string threadsPerInch3;
        public string headSizeInch;
        public string headSizemm;
        public string washerSizemm;
        public string washerThicknessmm;

        public CTEKScrewProperties() { }
    }
}
