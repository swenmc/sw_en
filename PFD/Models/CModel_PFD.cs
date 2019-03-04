using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;

namespace PFD
{
    enum EMemberGroupNames
    {
        eMainColumn = 0,    // Internal frame
        eRafter = 1,        // Internal frame
        eMainColumn_EF = 2, // Edge frame
        eRafter_EF = 3,     // Edge frame
        eEavesPurlin = 4,
        eGirtWall = 5,
        ePurlin = 6,
        eFrontColumn = 7,
        eBackColumn = 8,
        eFrontGirt = 9,
        eBackGirt = 10
    }

    public class CModel_PFD : CExample
    {
        public List<CEntity3D> componentList;
    }
}
