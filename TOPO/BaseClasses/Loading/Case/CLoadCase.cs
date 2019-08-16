using System;
using System.Collections.Generic;

namespace BaseClasses
{
    [Serializable]
    public class CLoadCase : CObject
    {
        //----------------------------------------------------------------------------


        // Nepouzije sa, pretoze kazda kombinacia ma vlastny faktor pre dany LC
        // Pouzije sa, lebo sa pri danej kombinacii nastavi tento faktor
        private float m_fFactor;

        public float Factor
        {
            get { return m_fFactor; }
            set { m_fFactor = value; }
        }


        private ELCType m_eType;

        public ELCType Type
        {
            get { return m_eType; }
            set { m_eType = value; }
        }

        private ELCGTypeForLimitState m_Type_LS;

        public ELCGTypeForLimitState MType_LS
        {
            get { return m_Type_LS; }
            set { m_Type_LS = value; }
        }

        private ELCMainDirection m_eMainDirection;

        public ELCMainDirection MainDirection
        {
            get { return m_eMainDirection; }
            set { m_eMainDirection = value; }
        }
              

        private List<CNLoad> MNodeLoadsList;
        private List<CMLoad> MMemberLoadsList;
        private List<CSLoad_Free> MSurfaceLoadsList;


        public List<CNLoad> NodeLoadsList
        {
            get
            {
                if (MNodeLoadsList == null) MNodeLoadsList = new List<CNLoad>();
                return MNodeLoadsList;
            }

            set
            {
                MNodeLoadsList = value;
            }
        }

        public List<CMLoad> MemberLoadsList
        {
            get
            {
                if (MMemberLoadsList == null) MMemberLoadsList = new List<CMLoad>();
                return MMemberLoadsList;
            }

            set
            {
                MMemberLoadsList = value;
            }
        }

        public List<CSLoad_Free> SurfaceLoadsList
        {
            get
            {
                if (MSurfaceLoadsList == null) MSurfaceLoadsList = new List<CSLoad_Free>();
                return MSurfaceLoadsList;
            }

            set
            {
                MSurfaceLoadsList = value;
            }
        }
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CLoadCase()
        {

        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, ELCMainDirection MainDirection_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, List<CNLoad> NodeLoads_temp, List<CMLoad> MemberLoads_temp, List<CSLoad_Free> SurfaceLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            MType_LS = eType_LS_temp;
            Type = type_temp;
            MainDirection = ELCMainDirection.eGeneral;
            NodeLoadsList = NodeLoads_temp;
            MemberLoadsList = MemberLoads_temp;
            SurfaceLoadsList = SurfaceLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CNLoad> NodeLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            MType_LS = eType_LS_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            NodeLoadsList = NodeLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CMLoad> MemberLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            MemberLoadsList = MemberLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CSLoad_Free> SurfaceLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            MType_LS = eType_LS_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            SurfaceLoadsList = SurfaceLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CNLoad> NodeLoads_temp, List<CMLoad> MemberLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            MType_LS = eType_LS_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            NodeLoadsList = NodeLoads_temp;
            MemberLoadsList = MemberLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, ELCGTypeForLimitState eType_LS_temp, ELCType type_temp, ELCMainDirection MainDirection_temp, List<CMLoad> MemberLoads_temp, List<CSLoad_Free> SurfaceLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            MType_LS = eType_LS_temp;
            Type = type_temp;
            MainDirection = MainDirection_temp;
            MemberLoadsList = MemberLoads_temp;
            SurfaceLoadsList = SurfaceLoads_temp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            CLoadCase c = (CLoadCase)obj;
            if (c.ID == this.ID) return true;
            else return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {

            // TODO: write your implementation of GetHashCode() here
            //throw new NotImplementedException();
            return base.GetHashCode();
        }
    }
}
