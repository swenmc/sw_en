using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public abstract class CAnchorArrangement_Circular : CAnchorArrangement
    {
        public CAnchorArrangement_Circular()
        { }

        // Not implemented
        public CAnchorArrangement_Circular(int iAnchorNumber_temp, CAnchor referenceAnchor_temp) : base(iAnchorNumber_temp, referenceAnchor_temp)
        {
            IHolesNumber = iAnchorNumber_temp;
            referenceAnchor = referenceAnchor_temp;
            holesCentersPointsfor3D = new Point[IHolesNumber];

            RadiusAngle = 360; // Circle total angle to generate holes
        }
    }
}
