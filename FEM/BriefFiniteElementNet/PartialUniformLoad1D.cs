using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Elements;
using MATH;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a uniform load with specified magnitude which is applying to an <see cref="FrameElement2Node"/> element.
    /// </summary>
    [Serializable]
    //[Obsolete("still in development")]
    public class PartialUniformLoad1D : Load1D
    {
        #region Members

        private double magnitude;
        private double startOffset;
        private double endOffset;

        /// <summary>
        /// Gets or sets the magnitude.
        /// </summary>
        /// <remarks>
        /// Value is magnitude of distributed load, the unit is [N/m]
        /// </remarks>
        /// <value>
        /// The magnitude of distributed load along the member.
        /// </value>
        public double Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }

        /// <summary>
        /// Gets or sets the start of load range
        /// </summary>
        /// <value>
        /// The offset from start node(s) regarding isoparametric coordination system (in range [-1,1])
        /// </value>
        public double StarIsoLocation
        {
            get { return startOffset; }
            set { startOffset = value; }
        }

        /// <summary>
        /// Gets or sets the end of load range
        /// </summary>
        /// <value>
        /// The offset from end node(s) regarding isoparametric coordination system (in range [-1,1])
        /// </value>
        public double EndIsoLocation
        {
            get { return endOffset; }
            set { endOffset = value; }
        }

        #endregion

        #region Methods

        public Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            if (element is FrameElement2Node)
            {
                var frElm = element as FrameElement2Node;

                var l = (frElm.EndNode.Location - frElm.StartNode.Location).Length;

                var w = GetLocalDistributedLoad(element as Element1D);

                var localEndForces = new Force[2];

                // Simply supported beam
                // https://www.linsgroup.com/MECHANICAL_DESIGN/Beam/beam_formula.htm

                /*
                var a =0;
                var b = 0;
                var c = 0;

                // 1 - start, 2 - end

                var R1x = (w.X * b / (2 * l)) * (2 * c + b);
                var R2x = (w.X * b / (2 * l)) * (2 * a + b);

                var R1y = (w.Y * b / (2 * l)) * (2 * c + b);
                var R2y = (w.Y * b / (2 * l)) * (2 * a + b);
                */

                // Both end fixed beam
                // https://www.engineersedge.com/beam_bending/beam_bending52.htm
                var a = l / 2 + StarIsoLocation * l / 2;
                var c = (EndIsoLocation - StarIsoLocation) * l / 2;
                var b = a + c;
                var e = l - b;
                var d = l - a / 2 - b / 2;

                var RAX = Get_RA(w.X, a, b, c, d, l);
                var RBX = Get_RB(w.X, a, b, c, d, l);

                var RAY = Get_RA(w.Y, a, b, c, d, l);
                var RBY = Get_RB(w.Y, a, b, c, d, l);

                var RAZ = Get_RA(w.Z, a, b, c, d, l);
                var RBZ = Get_RB(w.Z, a, b, c, d, l);

                var MAY = Get_MA(w.Z, a, b, c, d, l);
                var MBY = Get_MB(w.Z, a, b, c, d, l);

                var MAZ = Get_MA(w.Y, a, b, c, d, l);
                var MBZ = Get_MB(w.Y, a, b, c, d, l);

                localEndForces[0] = new Force(RAX, RAY, RAZ, 0, MAY, -MAZ);
                localEndForces[1] = new Force(RBX, RBY, RBZ, 0, MBY, -MBZ);

                localEndForces = CalcUtil.ApplyReleaseMatrixToEndForces(frElm, localEndForces);//applying release matrix to end forces

                for (var i = 0; i < element.Nodes.Length; i++)
                {
                    var frc = localEndForces[i];

                    localEndForces[i] =
                        new Force(
                            frElm.TransformLocalToGlobal(frc.Forces),
                            frElm.TransformLocalToGlobal(frc.Moments));
                }

                return localEndForces;
            }

            return element.GetGlobalEquivalentNodalLoads(this);
        }

        private double Get_RA(double w, double a, double b, double c , double d, double l)
        {
            return w * c / (4 * l * l) * (12 * d * d - 8 * d * d * d / l + 2 * b * c * c / l - c * c * c / l - c * c);
        }

        private double Get_RB(double w, double a, double b, double c, double d, double l)
        {
            var RA = Get_RA(w, a, b, c, d, l);
            return w * c - RA;
        }

        private double Get_MA(double w, double a, double b, double c, double d, double l)
        {
            return (w * c / (24 * l)) * (24 * d * d * d / l - 6 * b * c * c / l + 3 * c * c * c / l + 4 * c * c - 24 * d * d);
        }

        private double Get_MB(double w, double a, double b, double c, double d, double l)
        {
            return (w * c / (24 * l)) * (24 * d * d * d / l - 6 * b * c * c / l + 3 * c * c * c / l + 2 * c * c - 48 * d * d + 24 * d * l);
        }

        /// <summary>
        /// Gets the local distributed load.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <returns></returns>
        /// <remarks>
        /// Gets a vector that its components shows Wx, Wy and Wz in local coordination system of <see cref="elm"/>
        /// </remarks>
        private Vector GetLocalDistributedLoad(Element1D elm)
        {
            if (coordinationSystem == CoordinationSystem.Local)
            {
                return new Vector(
                    direction == LoadDirection.X ? this.magnitude : 0,
                    direction == LoadDirection.Y ? this.magnitude : 0,
                    direction == LoadDirection.Z ? this.magnitude : 0);
            }

            if (elm is FrameElement2Node)
            {
                var frElm = elm as FrameElement2Node;

                var w = new Vector();


                var globalVc = new Vector(
                    direction == LoadDirection.X ? this.magnitude : 0,
                    direction == LoadDirection.Y ? this.magnitude : 0,
                    direction == LoadDirection.Z ? this.magnitude : 0);

                w = frElm.TransformGlobalToLocal(globalVc);

                return w;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the internal force based on only this load at defined position <see cref="x"/>.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            if (elm is FrameElement2Node)
            {
                var frElm = elm as FrameElement2Node;

                var l = (frElm.EndNode.Location - frElm.StartNode.Location).Length;
                var w = GetLocalDistributedLoad(elm);

                var gf1 = -GetGlobalEquivalentNodalLoads(elm)[0];
                var f1 = new Force();

                f1.Forces = frElm.TransformGlobalToLocal(gf1.Forces);
                f1.Moments = frElm.TransformGlobalToLocal(gf1.Moments);

                //Console.WriteLine("Start f1:\t" + f1);

                var f2 = new Force(new Vector(w.X * x, w.Y * x, w.Z * x), new Vector());

                /*
                var buf = f1.Move(new Point(0, 0, 0), new Point(x, 0, 0)) +
                          f2.Move(new Point(x/2, 0, 0), new Point(x, 0, 0));
                */

                var gf3 = -GetGlobalEquivalentNodalLoads(elm)[1];
                var f3 = new Force();

                f3.Forces = frElm.TransformGlobalToLocal(gf3.Forces);
                f3.Moments = frElm.TransformGlobalToLocal(gf3.Moments);

                //Console.WriteLine("End f3:\t" + f3);

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // POKUS 5.2.2019 - VYPOCET VNUTORNYCH SIL

                var buf = new Force();

                bool bPodla_XLS = false;
                if (bPodla_XLS)
                {
                    // Absolute coordinate of load start and end point (b) and (e)
                    // b - distance of load start from member start point
                    // e - distance of load end from member start point
                    double b = StarIsoLocation < 0 ? (StarIsoLocation - (-1)) * l / 2 : l / 2 + StarIsoLocation * l / 2;
                    double e = EndIsoLocation < 0 ? (EndIsoLocation - (-1)) * l / 2 : l / 2 + EndIsoLocation * l / 2;

                    // Loading functions for each uniform or distributed load evaluated at distance = x from left end of member:

                    // TODO MI ZATIAL NEFUNGUJE
                    buf.Fx = GetForceAt_x(x, b, e, w.X, w.X);
                    buf.Fy = GetForceAt_x(x, b, e, w.Y, w.Y);
                    buf.Fz = GetForceAt_x(x, b, e, w.Z, w.Z);

                    buf.Mx = GetMomentAt_x(x, b, e, 0, 0); // ?? torzne zatazenie okolo osi x pruta na casti pruta (zavisi od podopretia v kruteni)
                    buf.My = GetMomentAt_x(x, b, e, w.Z, w.Z);
                    buf.Mz = GetMomentAt_x(x, b, e, w.Y, w.Y);
                }
                else
                {
                    var a = l / 2 + StarIsoLocation * l / 2;
                    var c = (EndIsoLocation - StarIsoLocation) * l / 2;
                    var b = a + c;
                    var e = l - b;
                    var d = l - a / 2 - b / 2;

                    buf.Fx = -GetForceAt_x2(x, a, b, c, d, l, w.X, -f1.Fx, -f3.Fx);
                    buf.Fy = -GetForceAt_x2(x, a, b, c, d, l, w.Y, -f1.Fy, -f3.Fy);
                    buf.Fz = -GetForceAt_x2(x, a, b, c, d, l, w.Z, -f1.Fz, -f3.Fz);

                    buf.Mx = -GetMomentAt_x2(x, a, b, c, d, l, 0, -f1.Fx, f1.Mx); // ?? torzne zatazenie okolo osi x pruta na casti pruta (zavisi od podopretia v kruteni)
                    buf.My = -GetMomentAt_x2(x, a, b, c, d, l, w.Z, -f1.Fz, f1.My);
                    buf.Mz = -GetMomentAt_x2(x, a, b, c, d, l, w.Y, -f1.Fy, f1.Mz);
                }

                //Console.WriteLine(buf);

                return -buf;
            }

            throw new NotImplementedException();
        }

        // Podla XLS - frame
        // Toto mi zatial velmi nefunguje - treba to konfrontovat s XLS vypoctom
        private double GetForceAt_x(double x, double b, double e, double wb, double we)
        {
            double Fvx;

            /*
            if(x >= e)
            -wb * (x - b - (x - e)) - 1 / 2 * (we - wb) / (e - b) * ((x - b) ^ 2 - (x - e) ^ 2) + (we - wb) * (x - e);
            else
            -wb * (x - b) + -1 / 2 * (we - wb) / (e - b) * (x - b) ^ 2;
            */

            if (x >= e)
            {
                Fvx = -wb * (x - b - (x - e)) + -1 / 2 * (we - wb) / (e - b) * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + (we - wb) * (x - e);
                //Fmx = -wb / 2 * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + -1 / 6 * (we - wb) / (e - b) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + (we - wb) / 2 * MathF.Pow2(x - e);
                //FPhix = -wb / (6 * E * I) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + -1 / (24 * E * I) * (we - wb) / (e - b) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + (we - wb) / (6 * E * I) * MathF.Pow3(x - e);
                //FDeltax = -wb / (24 * E * I) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + -1 / (120 * E * I) * (we - wb) / (e - b) * (MathF.Pow5(x - b) - MathF.Pow5(x - e)) + (we - wb) / (24 * E * I) * MathF.Pow4(x - e);
            }
            else if (x >= b)
            {
                Fvx = -wb * (x - b) + -1 / 2 * (we - wb) / (e - b) * MathF.Pow2(x - b);
                //Fmx = -wb / 2 * MathF.Pow2(x - b) + -1 / 6 * (we - wb) / (e - b) * MathF.Pow3(x - b) - MathF.Pow3(x - e);
                //FPhix = -wb / (6 * E * I) * MathF.Pow3(x - b) + -1 / (24 * E * I) * (we - wb) / (e - b) * MathF.Pow4(x - b);
                //FDeltax = -wb / (24 * E * I) * MathF.Pow4(x - b) + -1 / (120 * E * I) * (we - wb) / (e - b) * MathF.Pow5(x - b);
            }
            else
            {
                Fvx = 0;
                //Fmx = 0;
                //FPhix = 0;
                //FDeltax = 0;
            }

            return Fvx;
        }
        private double GetMomentAt_x(double x, double b, double e, double wb, double we)
        {
            double Fmx;

            /*
            if (x >= e)
            { -wb / 2 * ((x - b) ^ 2 - (x - e) ^ 2) + (-1 / 6 * (we - wb) / (e - b) * ((x - b) ^ 3 - (x - e) ^ 3) + (we - wb) / 2 * (x - e) ^ 2);
            }
            else
            {
                -wb / 2 * (x - b) ^ 2+(-1 / 6 * (we - wb) / (e - b) * (x - b) ^ 3; // Rozdiel v popise rovnice a v rovnici v xls
            }
            */

            if (x >= e)
            {
                //Fvx = -wb * (x - b - (x - e)) + -1 / 2 * (we - wb) / (e - b) * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + (we - wb) * (x - e);
                Fmx = -wb / 2 * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + -1 / 6 * (we - wb) / (e - b) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + (we - wb) / 2 * MathF.Pow2(x - e);
                //FPhix = -wb / (6 * E * I) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + -1 / (24 * E * I) * (we - wb) / (e - b) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + (we - wb) / (6 * E * I) * MathF.Pow3(x - e);
                //FDeltax = -wb / (24 * E * I) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + -1 / (120 * E * I) * (we - wb) / (e - b) * (MathF.Pow5(x - b) - MathF.Pow5(x - e)) + (we - wb) / (24 * E * I) * MathF.Pow4(x - e);
            }
            else if (x >= b)
            {
                //Fvx = -wb * (x - b) + -1 / 2 * (we - wb) / (e - b) * MathF.Pow2(x - b);
                Fmx = -wb / 2 * MathF.Pow2(x - b) + -1 / 6 * (we - wb) / (e - b) * MathF.Pow3(x - b) /*- MathF.Pow3(x - e) ?????*/; // Rozdiel v popise rovnice a v rovnici v xls
                //FPhix = -wb / (6 * E * I) * MathF.Pow3(x - b) + -1 / (24 * E * I) * (we - wb) / (e - b) * MathF.Pow4(x - b);
                //FDeltax = -wb / (24 * E * I) * MathF.Pow4(x - b) + -1 / (120 * E * I) * (we - wb) / (e - b) * MathF.Pow5(x - b);
            }
            else
            {
                //Fvx = 0;
                Fmx = 0;
                //FPhix = 0;
                //FDeltax = 0;
            }

            return Fmx;
        }

        // Podla webu - both end fixed beam
        // https://www.engineersedge.com/beam_bending/beam_bending52.htm
        private double GetForceAt_x2(double x, double a, double b, double c, double d, double l, double w, double Ra, double Rb)
        {
            /*
            double Ra = Get_RA(w, a, b, c, d, l); // TODO - namiesto tychto hodnot by sa mali asi napojit zlozky vektora f1, pozor na znamienka
            double Rb = Get_RB(w, a, b, c, d, l); // TODO - namiesto tychto hodnot by sa mali asi napojit zlozky vektora f1, pozor na znamienka
            */

            if (x <= a)
                return Ra;
            else if (x < b)
                return Ra - w * (x - a);
            else
                return -Rb;
        }
        private double GetMomentAt_x2(double x, double a, double b, double c, double d, double l, double w , double Ra, double Ma)
        {
            /*
            double Ra = Get_RA(w, a, b, c, d, l); // TODO - namiesto tychto hodnot by sa mali asi napojit zlozky vektora f1, pozor na znamienka
            double Ma = -Get_MA(w, a, b, c, d, l); // TODO - namiesto tychto hodnot by sa mali asi napojit zlozky vektora f1, pozor na znamienka
            */

            if (x < a)
                return -Ma + Ra * x;
            else if (x < b)
                return -Ma + Ra * x - w / 2 * MathF.Pow2(x - a);
            else
                return -Ma + Ra * x - w * c * (x - l + d);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialUniformLoad1D"/> class.
        /// </summary>
        /// <param name="magnitude">The magnitude of load.</param>
        /// <param name="direction">The direction of load.</param>
        /// <param name="sys">The coordination system type.</param>
        /// <param name="cse">The load case.</param>
        public PartialUniformLoad1D(double magnitude, double starIsoLocation, double endIsoLocation, LoadDirection direction, CoordinationSystem sys, LoadCase cse)
        {
            this.magnitude = magnitude;
            this.startOffset = starIsoLocation;
            this.endOffset = endIsoLocation;
            this.coordinationSystem = sys;
            this.direction = direction;
            this.Case = cse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialUniformLoad1D"/> class.
        /// </summary>
        /// <param name="magnitude">The magnitude.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="sys">The system.</param>
        public PartialUniformLoad1D(double magnitude, double starIsoLocation, double endIsoLocation, LoadDirection direction, CoordinationSystem sys)
        {
            this.magnitude = magnitude;
            this.startOffset = starIsoLocation;
            this.endOffset = endIsoLocation;
            this.coordinationSystem = sys;
            this.direction = direction;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PartialUniformLoad1D"/> class.
        /// </summary>
        public PartialUniformLoad1D()
        {
        }

        #endregion

        #region Serialization stuff and constructor

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("magnitude", magnitude);
            info.AddValue("starOffset", StarIsoLocation);
            info.AddValue("endOffset", EndIsoLocation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialUniformLoad1D"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected PartialUniformLoad1D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.magnitude = info.GetDouble("magnitude");
            this.startOffset = info.GetDouble("starOffset");
            this.endOffset = info.GetDouble("endOffset");
        }

        #endregion
    }
}
