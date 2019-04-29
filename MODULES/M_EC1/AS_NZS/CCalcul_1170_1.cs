using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_1
    {
        public float fDeadLoadTotal_Roof;
        public float fDeadLoadTotal_Wall;
        public float fImposedLoadTotal_Roof;

        public float fDeadLoad_Roof;
        public float fDeadLoad_Wall;

        public CCalcul_1170_1(
               float fMass_Roof,
               float fMass_Wall,
               float fAdditionalDeadActionRoof,
               float fAdditionalDeadActionWall,
               float fImposedActionRoof,
               float fg_acceleration)
        {
            fDeadLoad_Roof = fMass_Roof * fg_acceleration; // Change from mass kg/m^2 to weight
            fDeadLoad_Wall = fMass_Wall * fg_acceleration; // Change from mass kg/m^2 to weight

            // Additional dead loads
            // Additional dead load - roof
            float fDeadLoadAdditional_Roof = fAdditionalDeadActionRoof * 1000f; // change units from kN/m^2 to N/m^2
            float fDeadLoadAdditional_Wall = fAdditionalDeadActionWall * 1000f; // change units from kN/m^2 to N/m^2

            // Additional roof imposed load
            float fImposedLoad_Roof = fImposedActionRoof * 1000f; // change units from kN/m^2 to N/m^2

            // Set output variables [N / m^2]
            fDeadLoadTotal_Roof = fDeadLoad_Roof + fDeadLoadAdditional_Roof;
            fDeadLoadTotal_Wall = fDeadLoad_Wall + fDeadLoadAdditional_Wall;
            fImposedLoadTotal_Roof = fImposedLoad_Roof;
        }
    }
}
