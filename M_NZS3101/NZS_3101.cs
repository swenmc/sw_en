using MATH;
using MATH.ARRAY;
using System;
using BaseClasses;


namespace M_NZS_3101
{
    public class NZS_3101
    {
        public float Eq_17_1____(float fN_asterix, float fPhi, float fN_n)
        {
            return fN_asterix / (fPhi * fN_n); // Eq. (17-1) // fN Design ratio
        }
        public float Eq_17_2____(float fV_asterix, float fPhi, float fV_n)
        {
            return fV_asterix / (fPhi * fV_n); // Eq. (17-2) // fV Design ratio
        }
        public float Eq_17564___(bool bIsDuctileSteelInTension)
        {
            if (bIsDuctileSteelInTension)
                return 0.75f; // Eq. (17-3)
            else
                return 0.65f; // Eq. (17-4) (17-4(a)) (17-4(b))
        }
        public float Eq_17566___(float fN_asterix, float fPhi_N, float fN_n, float fV_asterix, float fPhi_V, float fV_n)
        {
            if (fV_asterix <= 0.2f * fPhi_V * fV_n)
                return fN_asterix / (fPhi_N * fN_n); // 17.5.6.6 (a)
            else if (fN_asterix <= 0.2f * fPhi_N * fN_n)
                return fV_asterix / (fPhi_V * fV_n); // 17.5.6.6 (b)
            else
                return (fN_asterix / (fPhi_N * fN_n) + fV_asterix / (fPhi_V * fV_n)) / 1.2f; // 17.5.6.6 (c) Eq. (17-5)
        }

        // TODO - rovnice z NZS 3101 - IN WORK
    }
}
