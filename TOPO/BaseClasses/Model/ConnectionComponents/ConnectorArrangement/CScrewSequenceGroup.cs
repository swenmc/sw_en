using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class CScrewSequenceGroup
    {
        private List<CScrewSequence> m_listScrewSequence;

        public List<CScrewSequence> ListScrewSequence
        {
            get
            {
                if (m_listScrewSequence == null) m_listScrewSequence = new List<CScrewSequence>();
                return m_listScrewSequence;
            }

            set
            {
                m_listScrewSequence = value;
            }
        }

        public CScrewSequenceGroup()
        {
            ListScrewSequence = new List<CScrewSequence>();
        }
    }
}
