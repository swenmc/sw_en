using System;

namespace BaseClasses
{
    [Serializable]
    public class CMemberEccentricity : CEntity
    {
        private float m_fy_local;

        public float MFy_local
        {
            get { return m_fy_local; }
            set { m_fy_local = value; }
        }

        private float m_fz_local;

        public float MFz_local
        {
            get { return m_fz_local; }
            set { m_fz_local = value; }
        }

        public CMemberEccentricity()
        {

        }

        public CMemberEccentricity(float fy_local, float fz_local)
        {
            MFy_local = fy_local;
            MFz_local = fz_local;
        }
    }
}
