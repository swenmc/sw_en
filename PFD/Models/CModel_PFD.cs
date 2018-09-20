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
        eMainColumn = 0,
        eRafter = 1,
        eEavesPurlin = 2,
        eGirtWall = 3,
        ePurlin = 4,
        eFrontColumn = 5,
        eBackColumn = 6,
        eFrontGirt = 7,
        eBackGirt = 8
    }

    public class CModel_PFD : CExample
    {
        public List<CEntity3D> componentList;


        
    }
}
