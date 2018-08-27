using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class CEntityGroup : CEntity3D
    {
        public CEntityGroup()
        { }

        public CEntityGroup(int ID_temp, string sName_temp, float fTime_temp)
        {
            ID = ID_temp;
            Name = sName_temp;
            FTime = fTime_temp;
        }
    }
}
