using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CLoadCaseView
    {
        int m_loadCaseID;
        string m_loadCaseName;
        string m_loadCaseType;

        public int LoadCaseID
        {
            get
            {
                return m_loadCaseID;
            }

            set
            {
                m_loadCaseID = value;
            }
        }

        public string LoadCaseName
        {
            get
            {
                return m_loadCaseName;
            }

            set
            {
                m_loadCaseName = value;
            }
        }

        public string LoadCaseType
        {
            get
            {
                return m_loadCaseType;
            }

            set
            {
                m_loadCaseType = value;
            }
        }

        public CLoadCaseView(int lcID, string lcName, string lcType)
        {
            LoadCaseID = lcID;
            LoadCaseName = lcName;
            LoadCaseType = lcType;
        }

        
    }
}
