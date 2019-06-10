using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class CIntermediateTransverseSupport:CEntity3D
    {
        private float m_fx_position_rel;
        public float Fx_position_rel
        {
            get { return m_fx_position_rel; }
            set { m_fx_position_rel = value; }
        }

        private float m_fx_position_abs;
        public float Fx_position_abs
        {
            get { return m_fx_position_abs; }
            set { m_fx_position_abs = value; }
        }

        EITSType m_eType;
        public EITSType Type
        {
            get { return m_eType; }
            set { m_eType = value; }
        }

        public CIntermediateTransverseSupport() { }

        public CIntermediateTransverseSupport(int id, EITSType etype, int fTime = 0)
        {
            ID = id;
            Type = etype;

            FTime = fTime;
        }        
        public CIntermediateTransverseSupport(int id, EITSType etype, float fx_rel, float fx_abs, int fTime = 0)
        {
            ID = id;
            Type = etype;
            Fx_position_rel = fx_rel;
            Fx_position_abs = fx_abs;
            FTime = fTime;
        }
    }
}
