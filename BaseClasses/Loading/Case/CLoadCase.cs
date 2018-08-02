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

        // TODO - zmenit na ENUM
        private string m_Type;

        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public List<CNLoad> NodeLoadsList;
        public List<CMLoad> MemberLoadsList;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CLoadCase()
        {

        }

        public CLoadCase(int id_temp, string name_temp, string type_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
        }

        public CLoadCase(int id_temp, string name_temp, string type_temp, List<CNLoad> NodeLoads_temp, List<CMLoad> MemberLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            NodeLoadsList = NodeLoads_temp;
            MemberLoadsList = MemberLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, string type_temp, List<CMLoad> MemberLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            MemberLoadsList = MemberLoads_temp;
        }

        public CLoadCase(int id_temp, string name_temp, string type_temp, List<CNLoad> NodeLoads_temp)
        {
            ID = id_temp;
            Name = name_temp;
            Type = type_temp;
            NodeLoadsList = NodeLoads_temp;
        }
    }
}
