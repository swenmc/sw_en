using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class MemberInternalForces
    {
        int _memberID;
        double[] _xLocations;
        List<basicInternalForces> _internalForces;
        List<basicDeflections> _deflections;

        public int MemberID
        {
            get
            {
                return _memberID;
            }

            set
            {
                _memberID = value;
            }
        }

        public double[] XLocations
        {
            get
            {
                return _xLocations;
            }

            set
            {
                _xLocations = value;
            }
        }

        public List<basicInternalForces> InternalForces
        {
            get
            {
                return _internalForces;
            }

            set
            {
                _internalForces = value;
            }
        }

        public List<basicDeflections> Deflections
        {
            get
            {
                return _deflections;
            }

            set
            {
                _deflections = value;
            }
        }

        public MemberInternalForces(int memberID)
        {
            MemberID = memberID;
            InternalForces = new List<basicInternalForces>();
            Deflections = new List<basicDeflections>();
        }

        public MemberInternalForces(int memberID, double[] xLocations, List<basicInternalForces> internalForces)
        {
            MemberID = memberID;
            XLocations = xLocations;
            InternalForces = internalForces;
        }
    }
}
