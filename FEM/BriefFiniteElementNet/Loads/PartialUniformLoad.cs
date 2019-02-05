using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Loads
{
    [Serializable]
    //[Obsolete("still in development")]
    public class PartialUniformLoad:Load
    {
        public PartialUniformLoad(LoadCase @case) : base(@case)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="case"></param>
        /// <param name="direction">the direction of load</param>
        /// <param name="magnitude">magnitude of load</param>
        /// <param name="coordinationSystem">e coordination system that <see cref="Direction"/> is defined in</param>
        public PartialUniformLoad(LoadCase @case, Vector direction, double magnitude, CoordinationSystem coordinationSystem) : base(@case)
        {
            _direction = direction;
            _magnitude = magnitude;
            _coordinationSystem = coordinationSystem;
        }

        private double _startOffset;
        private double _endOffset;
        private Vector _direction;
        private double _magnitude;
        private CoordinationSystem _coordinationSystem;

        /// <summary>
        /// Sets or gets the direction of load
        /// </summary>
        public Vector Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Sets or gets the magnitude of load
        /// </summary>
        public double Magnitude
        {
            get { return _magnitude; }
            set { _magnitude = value; }
        }


        /// <summary>
        /// Gets or sets the coordination system that <see cref="Direction"/> is defined in.
        /// </summary>
        /// <value>
        /// The coordination system.
        /// </value>
        public CoordinationSystem CoordinationSystem
        {
            get
            {
                return _coordinationSystem;
            }

            set
            {
                _coordinationSystem = value;
            }
        }

        /// <summary>
        /// Gets or sets the start of load range
        /// </summary>
        /// <value>
        /// The offset from start node(s) regarding isoparametric coordination system (in rangle [-1,1])
        /// </value>
        public double StarIsoLocation
        {
            get { return _startOffset; }
            set { _startOffset = value; }
        }

        /// <summary>
        /// Gets or sets the end of load range
        /// </summary>
        /// <value>
        /// The offset from end node(s) regarding isoparametric coordination system (in rangle [-1,1])
        /// </value>
        public double EndIsoLocation
        {
            get { return _endOffset; }
            set { _endOffset = value; }
        }




        #region Constructor

        public PartialUniformLoad()
        {
        }

        #endregion


        #region Deserialization Constructor

        protected PartialUniformLoad(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _direction = (Vector)info.GetValue("_direction", typeof(Vector));
            _magnitude = (double)info.GetValue("_magnitude", typeof(double));
            _coordinationSystem = (CoordinationSystem)(int)info.GetValue("_coordinationSystem", typeof(int));
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_direction", _direction);
            info.AddValue("_magnitude", _magnitude);
            info.AddValue("_coordinationSystem", (int)_coordinationSystem);
        }

        #endregion

        public double[] GetMagnitudesAt(params double[] isoCoords)
        {
            // TODO preverit co sa zapisuje do ktorej premennej (absolutne alebo relativne hodnoty)
            var buf = new double[isoCoords.Length];

            var n = isoCoords.Length;

            for (var i = 0; i < n; i++)
            {
                // TODO - toto som prerobil ale asi sa to nepouziva, navyse neviem ci je to spravne
                var startOffset = this.StarIsoLocation;
                var endOffset = this.EndIsoLocation;
                var Mag = this.Magnitude;

                var xi0 = -1 + startOffset;
                var xi1 = 1 - endOffset;

                if(isoCoords[i] >= xi0 && isoCoords[i] <= xi1)
                    buf[i] = Mag;
                else
                    buf[i] = 0;
            }

            return buf;
        }
    }
}
