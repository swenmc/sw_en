﻿
using MATH;
using System.Collections.Generic;

namespace BaseClasses.Helpers
{
    public static class ModelHelper
    {
        public static CMember[] GetFrontViewMembers(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                if (MathF.d_equal(m.NodeStart.Y, 0) && MathF.d_equal(m.NodeEnd.Y, 0)) members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetFrontViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (MathF.d_equal(n.Y, 0)) nodes.Add(n);
            }
            return nodes.ToArray();
        }

        public static CMember[] GetBackViewMembers(CModel model)
        {
            return GetMembersInYDistance(model, model.fL_tot);
        }

        public static CNode[] GetBackViewNodes(CModel model)
        {
            return GetNodesInYDistance(model, model.fL_tot);
        }

        public static CMember[] GetMiddleFrameMembers(CModel model)
        {
            return GetMembersInYDistance(model, model.fL1_frame);
        }

        public static CNode[] GetMiddleFrameNodes(CModel model)
        {
            return GetNodesInYDistance(model, model.fL1_frame);
        }

        public static CMember[] GetMembersInYDistance(CModel model, double length)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                if (MathF.d_equal(m.NodeStart.Y, length) && MathF.d_equal(m.NodeEnd.Y, length)) members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetNodesInYDistance(CModel model, double length)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (MathF.d_equal(n.Y, length)) nodes.Add(n);
            }
            return nodes.ToArray();
        }


        public static CMember[] GetLeftViewMembers(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                if (MathF.d_equal(m.NodeStart.X, 0) && MathF.d_equal(m.NodeEnd.X, 0)) members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetLeftViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (MathF.d_equal(n.X, 0)) nodes.Add(n);
            }
            return nodes.ToArray();
        }

        public static CMember[] GetRightViewMembers(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                if (MathF.d_equal(m.NodeStart.X, model.fW_frame) && MathF.d_equal(m.NodeEnd.X, model.fW_frame)) members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetRightViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (MathF.d_equal(n.X, model.fW_frame)) nodes.Add(n);
            }
            return nodes.ToArray();
        }





    }
}
