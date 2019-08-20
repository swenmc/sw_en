
using MATH;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace BaseClasses.Helpers
{
    public static class ModelHelper
    {
        // TO Ondrej
        // TODO - zapracoval som vseobecnejsie funkcie  GetMembersInDistanceInterval a GetNodesInDistanceInterval
        // Mali by fungovat pre rozne smery, takze nimi mozno nahradit niektore z nizsie uvedenych funkcii kde sa porovnavaju specificke suradnice uzlov natvrdo napr. m.NodeStart.Y
        public static Point3D GetPoint3D(this CNode node)
        {
            return new Point3D(node.X, node.Y, node.Z);
        }

        public static CMember[] GetFrontViewMembers(CModel model)
        {
            return GetMembersInDistance(model, 0, 1); // smer Y
        }

        public static CNode[] GetFrontViewNodes(CModel model)
        {
            return GetNodesInDistance(model, 0, 1); // smer Y
        }

        public static CMember[] GetBackViewMembers(CModel model)
        {
            return GetMembersInDistance(model, model.fL_tot, 1); // smer Y
        }

        public static CNode[] GetBackViewNodes(CModel model)
        {
            return GetNodesInDistance(model, model.fL_tot, 1); // smer Y
        }

        public static CMember[] GetLeftViewMembers(CModel model)
        {
            return GetMembersInDistance(model, 0, 0); // smer X
        }

        public static CNode[] GetLeftViewNodes(CModel model)
        {
            return GetNodesInDistance(model, 0, 0); // smer X
        }

        public static CMember[] GetRightViewMembers(CModel model)
        {
            return GetMembersInDistance(model, model.fW_frame, 0); // smer X
        }

        public static CNode[] GetRightViewNodes(CModel model)
        {
            return GetNodesInDistance(model, model.fW_frame, 0); // smer X
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

        public static List<CFoundation> GetColumnsViewFoundations(CModel model)
        {
            return model.m_arrFoundations;
        }

        public static List<CSlab> GetColumnsViewSlabs(CModel model)
        {
            return model.m_arrSlabs;
        }

        // Intermediate frame (main frame - 2nd gridline)
        public static CMember[] GetMiddleFrameMembers(CModel model, bool bAddGirtsAndPurlins = true)
        {
            int iDirectionCode = 1; // iDirectionCode 0- direction X, 1-direction Y, 2 - direction Z

            if (!bAddGirtsAndPurlins)
                return GetMembersInDistance(model, model.fL1_frame, iDirectionCode);
            else
                return GetMembersInDistanceInterval(model, 0, model.fL1_frame, iDirectionCode, false, true, true);
        }

        public static CNode[] GetMiddleFrameNodes(CModel model, bool bAddGirtsAndPurlins = true)
        {
            int iDirectionCode = 1; // iDirectionCode 0- direction X, 1-direction Y, 2 - direction Z

            if (!bAddGirtsAndPurlins)
                return GetNodesInDistance(model, model.fL1_frame, iDirectionCode);
            else
                return GetNodesInDistanceInterval(model, 0, model.fL1_frame, iDirectionCode, false, true);
        }

        public static CMember[] GetMembersInDistance(CModel model, double coordinate, int iDirectionCode)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                float fNodeStartCoordinate = SetNodeCoordinateForSpecificDirection(m.NodeStart, iDirectionCode);
                float fNodeEndCoordinate = SetNodeCoordinateForSpecificDirection(m.NodeEnd, iDirectionCode);

                // Porovname suradnice a typ prvku (dopracovat typ prvku)
                if ((MathF.d_equal(fNodeStartCoordinate, coordinate) && MathF.d_equal(fNodeEndCoordinate, coordinate))) members.Add(m);
            }
            return members.ToArray();
        }

        public static CMember[] GetMembersInDistance(CModel model, double coordinate, int iDirectionCode, EMemberType_FS type)
        {
            CMember[] membersAll = GetMembersInDistance(model, coordinate, iDirectionCode);

            List<CMember> members = new List<CMember>();
            foreach (CMember m in membersAll)
            {
                if (m.EMemberType == type)
                    members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetNodesInDistance(CModel model, double coordinate, int iDirectionCode)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                float fNodeCoordinate = SetNodeCoordinateForSpecificDirection(n, iDirectionCode);
                if (MathF.d_equal(fNodeCoordinate, coordinate)) nodes.Add(n);
            }
            return nodes.ToArray();
        }

        public static CMember[] GetMembersInDistanceInterval(CModel model, double startPosition, double endPosition, int iDirectionCode, bool bIncludingStart = false, bool bIncludingEnd = false, bool bIncludingPartial = true)
        {
            // iDirectionCode 0- direction X, 1-direction Y, 2 - direction Z
            // TODO - dopracovat selekciu podla typu pruta

            float fLimit = 0.0001f; // Limit pre uzatvoreny interval (0.1 mm)

            if (bIncludingStart) startPosition -= fLimit;
            if (bIncludingEnd) endPosition += fLimit;

            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                float fNodeStartCoordinate = SetNodeCoordinateForSpecificDirection(m.NodeStart, iDirectionCode);
                float fNodeEndCoordinate = SetNodeCoordinateForSpecificDirection(m.NodeEnd, iDirectionCode);

                if (!bIncludingPartial) // Selektuje len pruty ktore su cele v otvorenom alebo uzavretom intervale
                {
                    if ((!bIncludingStart && !bIncludingEnd) && (fNodeStartCoordinate > startPosition && fNodeEndCoordinate > startPosition) && (fNodeStartCoordinate < endPosition && fNodeEndCoordinate < endPosition)) // Whole member in interval - otvoreny interval
                        members.Add(m);
                    else if (bIncludingStart && ((fNodeStartCoordinate >= startPosition && fNodeEndCoordinate >= startPosition) && (fNodeStartCoordinate < endPosition && fNodeEndCoordinate < endPosition)))
                        members.Add(m);
                    else if (bIncludingEnd && ((fNodeStartCoordinate > startPosition && fNodeEndCoordinate > startPosition) && (fNodeStartCoordinate <= endPosition && fNodeEndCoordinate <= endPosition)))
                        members.Add(m);
                    else if ((bIncludingStart && bIncludingEnd) && ((fNodeStartCoordinate >= startPosition && fNodeEndCoordinate >= startPosition) && (fNodeStartCoordinate <= endPosition && fNodeEndCoordinate <= endPosition))) // Uzavrety interval
                        members.Add(m);
                }
                else // Selektuje pruty ktore maju aspon jeden uzol v intervale
                {
                    if ((!bIncludingStart && !bIncludingEnd) && ((fNodeStartCoordinate > startPosition || fNodeEndCoordinate > startPosition) && (fNodeStartCoordinate < endPosition || fNodeEndCoordinate < endPosition))) // Some node in interval - otvoreny interval
                        members.Add(m);
                    else if (bIncludingStart && ((fNodeStartCoordinate >= startPosition || fNodeEndCoordinate >= startPosition) && (fNodeStartCoordinate < endPosition || fNodeEndCoordinate < endPosition)))
                        members.Add(m);
                    else if (bIncludingEnd && ((fNodeStartCoordinate > startPosition || fNodeEndCoordinate > startPosition) && (fNodeStartCoordinate <= endPosition || fNodeEndCoordinate <= endPosition)))
                        members.Add(m);
                    else if ((bIncludingStart && bIncludingEnd) && ((fNodeStartCoordinate >= startPosition || fNodeEndCoordinate >= startPosition) && (fNodeStartCoordinate <= endPosition || fNodeEndCoordinate <= endPosition))) // Uzavrety interval
                        members.Add(m);
                }
            }

            return members.ToArray();
        }

        public static CMember[] GetMembersInDistanceInterval(CModel model, double startPosition, double endPosition, int iDirectionCode, EMemberType_FS type, bool bIncludingStart = false, bool bIncludingEnd = false, bool bIncludingPartial = true)
        {
            CMember[] membersAll = GetMembersInDistanceInterval(model, startPosition, endPosition, iDirectionCode, bIncludingStart, bIncludingEnd, bIncludingPartial);

            List<CMember> members = new List<CMember>();
            foreach (CMember m in membersAll)
            {
                if (m.EMemberType == type)
                    members.Add(m);
            }
            return members.ToArray();
        }

        public static CNode[] GetNodesInDistanceInterval(CModel model, double startPosition, double endPosition, int iDirectionCode, bool bIncludingStart = false, bool bIncludingEnd = false/*, bool bIncludingPartial = true*/)
        {
            float fLimit = 0.0001f; // Limit pre uzatvoreny interval (0.1 mm)

            if (bIncludingStart) startPosition -= fLimit;
            if (bIncludingEnd) endPosition += fLimit;

            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                float fNodeCoordinate = SetNodeCoordinateForSpecificDirection(n, iDirectionCode);

                if ((!bIncludingStart && !bIncludingEnd) && (fNodeCoordinate > startPosition && fNodeCoordinate < endPosition)) // Node in interval - otvoreny interval
                    nodes.Add(n);
                else if (bIncludingStart && (fNodeCoordinate >= startPosition && fNodeCoordinate < endPosition))
                    nodes.Add(n);
                else if (bIncludingEnd && (fNodeCoordinate > startPosition && fNodeCoordinate <= endPosition))
                    nodes.Add(n);
                else if ((bIncludingStart && bIncludingEnd) && (fNodeCoordinate >= startPosition && fNodeCoordinate <= endPosition)) // Uzavrety interval
                    nodes.Add(n);
            }

            return nodes.ToArray();
        }

        private static float SetNodeCoordinateForSpecificDirection(CNode n, int iDirectionCode)
        {
            // Funckia vrati suradnicu uzla pre specificky smer GCS ktory chceme uvazovat

            if(iDirectionCode == 0) // Direction X
                return n.X;
            if (iDirectionCode == 1) // Direction Y
                return n.Y;
            else //if (iDirectionCode == 2) // Direction ZX
                return n.Z;
        }
    }
}
