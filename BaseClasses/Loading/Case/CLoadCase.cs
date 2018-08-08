using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    [Serializable]
    public class CLoadCase : CObject
    {
        //----------------------------------------------------------------------------

        /*
        // Nepouzije sa, pretoze kazda kombinacia ma vlastny faktor pre dany LC
        private float m_fFactor;

        public float Factor
        {
            get { return m_fFactor; }
            set { m_fFactor = value; }
        }
        */

        private ELCType m_eType;

        public ELCType Type
        {
            get { return m_eType; }
            set { m_eType = value; }
        }

        private ELCMainDirection m_eMainDirection;

        public ELCMainDirection MainDirection
        {
            get { return m_eMainDirection; }
            set { m_eMainDirection = value; }
        }

        public List<CNLoad> NodeLoadsList;
        public List<CMLoad> MemberLoadsList;
        public List<CSLoad_Free> SurfaceLoadsList;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CLoadCase()
        {

        }

        public CLoadCase(int id_temp, string name_temp, ELCType type_temp, ELCMainDirection MainDirection_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCType type_temp, List<CNLoad> NodeLoads_temp, List<CMLoad> MemberLoads_temp, List<CSLoad_Free> SurfaceLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = ELCMainDirection.eGeneral;
            NodeLoadsList = NodeLoads_temp;
            MemberLoadsList = MemberLoads_temp;
            SurfaceLoadsList = SurfaceLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCType type_temp, List<CNLoad> NodeLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = ELCMainDirection.eGeneral;
            NodeLoadsList = NodeLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CMLoad> MemberLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            MemberLoadsList = MemberLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCType type_temp, List<CSLoad_Free> SurfaceLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = ELCMainDirection.eGeneral;
            SurfaceLoadsList = SurfaceLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CMLoad> MemberLoads_temp, List<CSLoad_Free> SurfaceLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            MemberLoadsList = MemberLoads_temp;
            SurfaceLoadsList = SurfaceLoads_temp;
        }
    }
}
