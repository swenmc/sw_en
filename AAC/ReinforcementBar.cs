using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses;
using BaseClasses.GraphObj;
using _3DTools;
using MATH;
using MATERIAL;
using CRSC;

namespace AAC
{
    public class ReinforcementBar : Object3DModel
    {
        public MATERIAL.CMat_03_00 Reinforcement;
        public CRSC.CCrSc_3_09 Cross_Section;

        public float fL = 0.0f;   // Bar Length

        public ReinforcementBar() { }

        public ReinforcementBar(float fd_temp, float fL_temp, MATERIAL.CMat_03_00 Reinforcement_temp)
        {
            Reinforcement = new MATERIAL.CMat_03_00();
            Reinforcement = Reinforcement_temp;
            Cross_Section = new CRSC.CCrSc_3_09(fd_temp);
            fL = fL_temp;
        }


    }
}
