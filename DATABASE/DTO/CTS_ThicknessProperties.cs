using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CTS_ThicknessProperties
    {
        // TODO - implementovat properties

        public int ID;
        public double thicknessCore;
        public double thicknessTot;
        public int[] claddingIDs;
        public int[] coatingIDs;
        public int[] coil_IDs;

        public CTS_ThicknessProperties() { }
    }
}
