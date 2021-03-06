﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Elements;
using MATH;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a Concentrated load (contains both force and moment in 3d space) which is applying to an <see cref="Element1D"/> body.
    /// </summary>
    [Serializable]
    public class ConcentratedLoad1D : Load1D
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D"/> class.
        /// </summary>
        public ConcentratedLoad1D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D" /> class.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="distanseFromStartNode">The distance from start node.</param>
        /// <param name="coordinationSystem">The coordination system.</param>
        /// <param name="cse">The load case.</param>
        public ConcentratedLoad1D(Force force, double distanseFromStartNode, CoordinationSystem coordinationSystem, LoadCase cse)
        {
            this.force = force;
            this.distanseFromStartNode = distanseFromStartNode;
            this.coordinationSystem = coordinationSystem;
            this.Case = cse;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D"/> class.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="distanseFromStartNode">The distance from start node.</param>
        /// <param name="coordinationSystem">The coordination system.</param>
        public ConcentratedLoad1D(Force force, double distanseFromStartNode, CoordinationSystem coordinationSystem)
        {
            this.force = force;
            this.distanseFromStartNode = distanseFromStartNode;
            this.coordinationSystem = coordinationSystem;
        }

        #endregion

        #region Properties

        private Force force;

        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        /// <remarks>Concentrated force consist of force in 3 directions and moments in three dimentions</remarks>
        /// <value>
        /// The concentrated force.
        /// </value>
        public Force Force
        {
            get { return force; }
            set { force = value; }
        }

        /// <summary>
        /// Gets or sets the distance of concentrated load from start node.
        /// </summary>
        /// <value>
        /// The distance of the <see cref="ConcentratedLoad1D"/> from start node.
        /// </value>
        public double DistanseFromStartNode
        {
            get { return distanseFromStartNode; }
            set { distanseFromStartNode = value; }
        }


        private double distanseFromStartNode;

        #endregion

        #region Methods

        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            if (elm is FrameElement2Node)
            {
                var e = elm as FrameElement2Node;

                var localForce = this.force;

                if (this.coordinationSystem == CoordinationSystem.Global)
                {
                    var tmp = e.TransformGlobalToLocal(localForce.Forces, localForce.Moments);

                    localForce.Forces = tmp[0];
                    localForce.Moments = tmp[1];
                }

                var l = (e.EndNode.Location - e.StartNode.Location).Length;

                var l1 = distanseFromStartNode;
                var l2 = l - l1;

                var mymy1 = localForce.My*l2/(l*l)*(l2 - 2*l1);
                var myfz1 = 6*localForce.My*l1*l2/(l*l*l);

                var mzmz1 = localForce.Mz*l2/(l*l)*(l2 - 2*l1);
                var mzfy1 = -6*localForce.Mz*l1*l2/(l*l*l);

                var fzmy1 = -localForce.Fz*l1*l2*l2/(l*l);
                var fzfz1 = localForce.Fz*l2*l2/(l*l*l)*(3*l1 + l2);

                var fymz1 = localForce.Fy*l1*l2*l2/(l*l);
                var fyfy1 = localForce.Fy*l2*l2/(l*l*l)*(3*l1 + l2);


                var fxfx1 = localForce.Fx*l2/l;
                var mxmx1 = localForce.Mx*l2/l;

                var f1 = -new Force(
                    fxfx1,
                    mzfy1 + fyfy1,
                    fzfz1 + myfz1,
                    mxmx1,
                    mymy1 + fzmy1,
                    mzmz1 + fymz1
                    );

                if (x < this.distanseFromStartNode)
                {
                    var v1 = new Vector(x, 0, 0);
                    var buf = -f1.Move(v1);

                    return buf;
                }

                else
                {
                    var v1 = new Vector(x, 0, 0);
                    var v2 = new Vector(x - this.distanseFromStartNode, 0, 0);

                    var buf = -f1.Move(v1) - this.force.Move(v2);

                    return buf;
                }
            }

            throw new NotImplementedException();
        }

        public override Displacement GetLocalDeformationAt_MC(Element1D elm, double x, bool bConsiderNodalDisplacementOnly = false, bool bConsiderNodalDisplacement = false)
        {
            if (elm is FrameElement2Node)
            {
                var frElm = elm as FrameElement2Node;

                var localForce = this.force;

                if (this.coordinationSystem == CoordinationSystem.Global)
                {
                    var tmp = frElm.TransformGlobalToLocal(localForce.Forces, localForce.Moments);

                    localForce.Forces = tmp[0];
                    localForce.Moments = tmp[1];
                }

                var l = (frElm.EndNode.Location - frElm.StartNode.Location).Length;

                var l1 = distanseFromStartNode;
                var l2 = l - l1;

                var gStartDisp = frElm.StartNode.GetNodalDisplacement();
                var gEndDisp = frElm.EndNode.GetNodalDisplacement();

                var lStartDisp = new Displacement(
                    frElm.TransformGlobalToLocal(gStartDisp.Displacements),
                    frElm.TransformGlobalToLocal(gStartDisp.Rotations));

                var lEndDisp = new Displacement(
                    frElm.TransformGlobalToLocal(gEndDisp.Displacements),
                    frElm.TransformGlobalToLocal(gEndDisp.Rotations));

                // Linear displacement in x-location due to displacement of element end nodes
                double dx_linear_Atx = GetDisplacementAt_x_linear(lEndDisp.DX, lStartDisp.DX, x, l);
                double dy_linear_Atx = GetDisplacementAt_x_linear(lEndDisp.DY, lStartDisp.DY, x, l);
                double dz_linear_Atx = GetDisplacementAt_x_linear(lEndDisp.DZ, lStartDisp.DZ, x, l);
                double rx_linear_Atx = GetDisplacementAt_x_linear(lEndDisp.RX, lStartDisp.RX, x, l);
                double ry_linear_Atx = GetDisplacementAt_x_linear(lEndDisp.RY, lStartDisp.RY, x, l);
                double rz_linear_Atx = GetDisplacementAt_x_linear(lEndDisp.RZ, lStartDisp.RZ, x, l);

                var lDisp_x_linear = new Displacement(
                    dx_linear_Atx,
                    dy_linear_Atx,
                    dz_linear_Atx,
                    rx_linear_Atx,
                    ry_linear_Atx,
                    rz_linear_Atx);

                var buf = new Displacement();

                // bool bConsiderNodalDisplacement = false; // Nodal displacement transformed to LCS

                if (bConsiderNodalDisplacement)
                {
                    buf.DX = lDisp_x_linear.DX;
                    buf.DY = lDisp_x_linear.DY + (bConsiderNodalDisplacementOnly ? 0 : GetDeflectionAt_x2(x, l1, l2, l, localForce.Fy, frElm.E, frElm.Iz));
                    buf.DZ = lDisp_x_linear.DZ + (bConsiderNodalDisplacementOnly ? 0 : GetDeflectionAt_x2(x, l1, l2, l, localForce.Fz, frElm.E, frElm.Iy));
                    buf.RX = lDisp_x_linear.RX;
                    buf.RY = lDisp_x_linear.RY; // TODO - not implemented
                    buf.RZ = lDisp_x_linear.RZ; // TODO - not implemented
                }
                else
                {
                    buf.DX = 0;
                    buf.DY = GetDeflectionAt_x2(x, l1, l2, l, localForce.Fy, frElm.E, frElm.Iz);
                    buf.DZ = GetDeflectionAt_x2(x, l1, l2, l, localForce.Fz, frElm.E, frElm.Iy);
                    buf.RX = 0;
                    buf.RY = 0; // TODO - not implemented
                    buf.RZ = 0; // TODO - not implemented
                }

                return -buf;
            }

            throw new NotImplementedException();
        }

        private double GetDeflectionAt_x2(double x, double l1, double l2, double l, double f_value, double E, double I)
        {
            return (-f_value / (6f * E * I)) * (l2 * ((MathF.Pow3(x) / l) - ((l1 * l2 * x / l) * (2*l -l1)) - MathF.Pow3(x - l1)));
        }

        // TODO - refactoring
        private double GetDisplacementAt_x_linear(double dstart, double dend, double x, double l)
        {
            // Linear interpolation (l and x are always positive)
            return MATH.ARRAY.ArrayF.GetLinearInterpolationValuePositive(0, l, dstart, dend, x);
        }

        public Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            if (element is FrameElement2Node)
            {
                var e = element as FrameElement2Node;

                var localForce = this.force;

                if (this.coordinationSystem == CoordinationSystem.Global)
                {
                    var tmp = e.TransformGlobalToLocal(localForce.Forces, localForce.Moments);

                    localForce.Forces = tmp[0];
                    localForce.Moments = tmp[1];
                }

                var buf = new Force[2];

                buf[0] = new Force();

                var l = (e.EndNode.Location - e.StartNode.Location).Length;

                var a = distanseFromStartNode;
                var b = l - a;

                var mymy1 = localForce.My*b/(l*l)*(b - 2*a);
                var mymy2 = localForce.My*a/(l*l)*(a - 2*b);

                var myfz1 = 6*localForce.My*a*b/(l*l*l);
                var myfz2 = -myfz1;


                var mzmz1 = localForce.Mz*b/(l*l)*(b - 2*a);
                var mzmz2 = localForce.Mz*a/(l*l)*(a - 2*b);

                var mzfy1 = -6*localForce.Mz*a*b/(l*l*l);
                var mzfy2 = -mzfy1;


                var fzmy1 = -localForce.Fz*a*b*b/(l*l);
                var fzmy2 = localForce.Fz*a*a*b/(l*l);

                var fzfz1 = localForce.Fz*b*b/(l*l*l)*(3*a + b);
                var fzfz2 = localForce.Fz*a*a/(l*l*l)*(3*b + a);


                var fymz1 = localForce.Fy*a*b*b/(l*l);
                var fymz2 = -localForce.Fy*a*a*b/(l*l);

                var fyfy1 = localForce.Fy*b*b/(l*l*l)*(3*a + b);
                var fyfy2 = localForce.Fy*a*a/(l*l*l)*(3*b + a);


                var fxfx1 = localForce.Fx*b/l;
                var fxfx2 = localForce.Fx*a/l;

                var mxmx1 = localForce.Mx*b/l;
                var mxmx2 = localForce.Mx*a/l;

                var f1 = new Force(
                    fxfx1,
                    mzfy1 + fyfy1,
                    fzfz1 + myfz1,
                    mxmx1,
                    mymy1 + fzmy1,
                    mzmz1 + fymz1
                    );

                var f2 = new Force(
                    fxfx2,
                    mzfy2 + fyfy2,
                    fzfz2 + myfz2,
                    mxmx2,
                    mymy2 + fzmy2,
                    mzmz2 + fymz2
                    );

                var localEndForces = new Force[2];
                localEndForces[0] = f1;
                localEndForces[1] = f2;

                localEndForces = CalcUtil.ApplyReleaseMatrixToEndForces(e, localEndForces);//applying release matrix to end forces

                var vecs = new Vector[] { localEndForces[0].Forces, localEndForces[0].Moments, localEndForces[1].Forces, localEndForces[1].Moments };
                var tvecs = e.TransformLocalToGlobal(vecs);

                buf[0] = new Force(tvecs[0], tvecs[1]);
                buf[1] = new Force(tvecs[2], tvecs[3]);

                return buf;
            }

            throw new NotImplementedException();
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
            info.AddValue("distanseFromStartNode", distanseFromStartNode);
            info.AddValue("force", force);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected ConcentratedLoad1D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.distanseFromStartNode = info.GetDouble("distanseFromStartNode");
            this.force = (Force)info.GetValue("force",typeof(Force));
        }

        #endregion
    }
}