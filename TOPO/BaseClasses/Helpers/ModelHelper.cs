
using MATH;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace BaseClasses.Helpers
{
    public static class ModelHelper
    {

        public static Point3D GetPoint3D(this CNode node)
        {
            return new Point3D(node.X, node.Y, node.Z);
        }

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

        // Roof
        public static CMember[] GetRoofViewMembers(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                // Coordinates are higher than (eave) wall height
                if (((m.NodeStart.Z >= model.fH1_frame || MathF.d_equal(m.NodeStart.Z, model.fH1_frame)) &&
                    ((m.NodeEnd.Z >= model.fH1_frame) || MathF.d_equal(m.NodeEnd.Z, model.fH1_frame))) &&
                    (
                    m.EMemberType == EMemberType_FS.eER || // Edge rafter
                    m.EMemberType == EMemberType_FS.eMR || // Main rafter
                    m.EMemberType == EMemberType_FS.eEP || // Eave purlin
                    m.EMemberType == EMemberType_FS.eP ||  // Purlin
                    m.EMemberType == EMemberType_FS.ePB    // Purlin Block
                    )
                    )
                    members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetRoofViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (n.Z >= model.fH1_frame || MathF.d_equal(n.Z, model.fH1_frame)) nodes.Add(n);
            }
            return nodes.ToArray();
        }

        // Columns
        public static CMember[] GetColumnsViewMembers(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                if ((MathF.d_equal(m.NodeStart.Z, 0) || MathF.d_equal(m.NodeEnd.Z, 0)) &&
                    (m.EMemberType == EMemberType_FS.eMC || // Main frame column
                    m.EMemberType == EMemberType_FS.eEC ||  // Edge frame column
                    m.EMemberType == EMemberType_FS.eC ||   // General Column
                    m.EMemberType == EMemberType_FS.eWP ||  // Wind post
                    m.EMemberType == EMemberType_FS.eDT ||  // Roller door trimmer
                    m.EMemberType == EMemberType_FS.eDF))   // Personnel door trimmer (frame column)
                    members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetColumnsViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (MathF.d_equal(n.Z, 0)) nodes.Add(n);
            }
            return nodes.ToArray();
        }

        // Intermediate frame (main frame - 2nd gridline)
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
    }
}
