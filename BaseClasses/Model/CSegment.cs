using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    // Class definition - segment of member
    // Straigth or curved part of member between two nodes (internal nodes or end nodes of member)
    // As default one segment exist for one member defined by two nodes, segment could be straight or curved
    // It means that simple straiht member properties are identical to straight segment
    // For polynomial members list of segments exists

    [Serializable]
    public class CSegment : CMember
    {
        public CSegment()
        { 
        
        }
    }
}
