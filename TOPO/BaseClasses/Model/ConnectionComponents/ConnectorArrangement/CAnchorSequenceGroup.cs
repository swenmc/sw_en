using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CAnchorSequenceGroup : CConnectorSequenceGroup
    {
        public CAnchorSequenceGroup()
        {
            ListSequence = new List<CConnectorSequence>();
        }
    }
}
