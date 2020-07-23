
using BaseClasses.GraphObj;
using MATH;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public static List<CMember> GetRoofViewMembers(CModel model)
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
            return members;
        }

        public static List<CNode> GetRoofViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CNode n in model.m_arrNodes)
            {
                if (n.Z >= model.fH1_frame || MathF.d_equal(n.Z, model.fH1_frame)) nodes.Add(n);
            }
            return nodes;
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

            //task 600
            if (!bAddGirtsAndPurlins)
                //return GetMembersInDistance(model, model.fL1_frame, iDirectionCode);
                return GetMembersInDistance(model, model.L1_Bays[0], iDirectionCode);
            else
                //return GetMembersInDistanceInterval(model, 0, model.fL1_frame, iDirectionCode, false, true, true);
                return GetMembersInDistanceInterval(model, 0, model.L1_Bays[0], iDirectionCode, false, true, true);
        }

        public static CNode[] GetMiddleFrameNodes(CModel model, bool bAddGirtsAndPurlins = true)
        {
            int iDirectionCode = 1; // iDirectionCode 0- direction X, 1-direction Y, 2 - direction Z

            //task 600
            if (!bAddGirtsAndPurlins)
                //return GetNodesInDistance(model, model.fL1_frame, iDirectionCode);
                return GetNodesInDistance(model, model.L1_Bays[0], iDirectionCode);
            else
                //return GetNodesInDistanceInterval(model, 0, model.fL1_frame, iDirectionCode, false, true);
                return GetNodesInDistanceInterval(model, 0, model.L1_Bays[0], iDirectionCode, false, true);
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

        // Foundation points
        public static List<Point3D> GetFootingPadsPoints(CModel model)
        {
            List<Point3D> list = null;

            if (model.m_arrFoundations != null && model.m_arrFoundations.Count > 0) // Check that some foundations exists
            {
                list = new List<Point3D>();
                // Refaktorovat s inymi castami v Drawing3D.cs

                foreach (CFoundation f in model.m_arrFoundations)
                {
                    GeometryModel3D model3D = f.Visual_Object;

                    if (f.Visual_Object == null) // In case that foundation exist but geometry is not generated
                        model3D = f.Visual_Object = f.CreateGeomModel3D(System.Windows.Media.Colors.Gray, 0.2f); // TODO zaviest opacity ako parameter

                    MeshGeometry3D mesh3D = (MeshGeometry3D)model3D.Geometry; // TO Ondrej - toto su podla mna uplne zakladna mesh a body geometrie zakladu, nemali by sme pracovat uz s transformovanymi ????

                    foreach (Point3D point3D in mesh3D.Positions)
                    {
                        // TO Ondrej - dve moznosti ako ziskat transformaciu zakladu
                        // 1
                        Transform3DGroup trans = f.GetFoundationTransformGroup_Complete();

                        // 2
                        //Transform3DGroup trans = new Transform3DGroup();
                        //trans.Children.Add(model3D.Transform);

                        Point3D p = trans.Transform(point3D); // Transformujeme povodny bod
                        list.Add(p);
                    }
                }
            }

            return list;
        }

        public static List<Point3D> GetPointsInDistance(List<Point3D> sourcePoints, double coordinate, int iDirectionCode)
        {
            List<Point3D> points = new List<Point3D>();

            foreach (Point3D p in sourcePoints)
            {
                double dPointCoordinate = SetPointCoordinateForSpecificDirection(p, iDirectionCode);
                if (MathF.d_equal(dPointCoordinate, coordinate)) points.Add(p);
            }

            if (points.Count == 0) return null;

            return points;
        }

        public static List<Point3D> GetPointsInDistanceInterval(List<Point3D> sourcePoints, double startPosition, double endPosition, int iDirectionCode, bool bIncludingStart = false, bool bIncludingEnd = false/*, bool bIncludingPartial = true*/)
        {
            float fLimit = 0.0001f; // Limit pre uzatvoreny interval (0.1 mm)

            if (bIncludingStart) startPosition -= fLimit;
            if (bIncludingEnd) endPosition += fLimit;

            List<Point3D> points = new List<Point3D>();

            foreach (Point3D p in sourcePoints)
            {
                double PointCoordinate = SetPointCoordinateForSpecificDirection(p, iDirectionCode);

                if ((!bIncludingStart && !bIncludingEnd) && (PointCoordinate > startPosition && PointCoordinate < endPosition)) // Node in interval - otvoreny interval
                    points.Add(p);
                else if (bIncludingStart && (PointCoordinate >= startPosition && PointCoordinate < endPosition))
                    points.Add(p);
                else if (bIncludingEnd && (PointCoordinate > startPosition && PointCoordinate <= endPosition))
                    points.Add(p);
                else if ((bIncludingStart && bIncludingEnd) && (PointCoordinate >= startPosition && PointCoordinate <= endPosition)) // Uzavrety interval
                    points.Add(p);
            }

            if (points.Count == 0) return null;

            return points;
        }

        public static List<CConnectionJointTypes> GetRelatedJoints(CModel model, CMember[] members)
        {
            // TODO Ondrej - potrebujeme najst v globalnom modeli vsetky spoje ktore prisluchaju zvolenym prutom pre filter
            // Este by bolo dobre vylepsit to tak ze vieme urcit presne suradnicu uzla spoja, aby sme vybrali len spoje v rovine pohladu pre ktore chceme teoreticky zobrazit znacky detailov
            List<CConnectionJointTypes> joints = new List<CConnectionJointTypes>();

            List<CMember> membersList = new List<CMember>(members); // Konvertujeme z pola na list, aby sme mohli v zozname vyhladavat

            // Najdi vsetky spoje na prutoch modelu
            foreach(CConnectionJointTypes j in model.m_arrConnectionJoints)
            {
                foreach(CMember m in membersList)
                {
                    if (j.m_Node.ID == m.NodeStart.ID || j.m_Node.ID == m.NodeEnd.ID)
                    {
                        if (!joints.Contains(j))
                            joints.Add(j); // Nepridavat uz pridane spoje, spoj musi byt v zozname len raz
                    }

                    //// uzol spoja je zaciatocny alebo koncovy uzol niektoreho z prutov
                    //// main member spoja je v zozname prutov, secondary member spoja je v zozname prutov
                    //if ((j.m_Node == m.NodeStart || j.m_Node == m.NodeEnd) &&
                    //    (membersList.Contains(j.m_MainMember) &&
                    //    (j.m_SecondaryMembers == null || (j.m_SecondaryMembers != null && membersList.Contains(j.m_SecondaryMembers[0])))))
                    //{
                    //    if(!joints.Contains(j))
                    //        joints.Add(j); // Nepridavat uz pridane spoje, spoj musi byt v zozname len raz
                    //}
                }
            }

            return joints;
        }

        private static float SetNodeCoordinateForSpecificDirection(CNode n, int iDirectionCode)
        {
            // Funckia vrati suradnicu uzla pre specificky smer GCS ktory chceme uvazovat

            if(iDirectionCode == 0) // Direction X
                return n.X;
            if (iDirectionCode == 1) // Direction Y
                return n.Y;
            else //if (iDirectionCode == 2) // Direction Z
                return n.Z;
        }

        private static double SetPointCoordinateForSpecificDirection(Point3D p, int iDirectionCode)
        {
            // Funckia vrati suradnicu bodu pre specificky smer GCS ktory chceme uvazovat

            if (iDirectionCode == 0) // Direction X
                return p.X;
            if (iDirectionCode == 1) // Direction Y
                return p.Y;
            else //if (iDirectionCode == 2) // Direction Z
                return p.Z;
        }

        public static int GetFirstBayWithoutDoors(ObservableCollection<DoorProperties> doors, string side)
        {
            int freeBay = 1;
            IEnumerable<DoorProperties> doorsOnSide = doors.Where(d => d.sBuildingSide == side);
            foreach (DoorProperties door in doorsOnSide.OrderBy(d => d.iBayNumber))
            {
                if (freeBay == door.iBayNumber) freeBay++;
            }
            return freeBay;
        }

        public static List<CSlabPerimeter> GetDifferentPerimetersWithoutRebates(List<CSlabPerimeter> perimeters)
        {
            List<CSlabPerimeter> differentPerimeters = new List<CSlabPerimeter>();
            if (perimeters.Count == 0) return differentPerimeters;

            differentPerimeters.Add(perimeters[0]);
            for (int i = 1; i < perimeters.Count; i++)
            {
                if (differentPerimeters.Exists(p => p.Equals(perimeters[i]))) continue;
                else differentPerimeters.Add(perimeters[i]);
            }
            return differentPerimeters;
        }
        public static List<CSlabPerimeter> GetDifferentPerimetersWithRebates(List<CSlabPerimeter> perimeters)
        {
            List<CSlabPerimeter> differentPerimeters = new List<CSlabPerimeter>();
            if (perimeters.Count == 0) return differentPerimeters;

            differentPerimeters.Add(perimeters[0]);
            for (int i = 1; i < perimeters.Count; i++)
            {
                if (differentPerimeters.Exists(p => p.EqualsWithRebates(perimeters[i]))) continue;
                else differentPerimeters.Add(perimeters[i]);
            }
            return differentPerimeters;
        }

        public static List<CDimension> GetClonedDimensions(CDimension[] Dimensions)
        {
            // To Ondrej - pokus o opravu prepisu dat pre plate ak spustim export opakovane
            List<CDimension> clonedDimensions = new List<CDimension>();
            if (Dimensions != null)
            {
                foreach (CDimension d in Dimensions)
                {
                    CDimension dimensionClone = d.GetClonedDimension();
                    clonedDimensions.Add(dimensionClone);
                }
            }
            return clonedDimensions;
        }

        public static List<CLine2D> GetClonedLines(CLine2D[] lines)
        {
            List<CLine2D> clonedLines = new List<CLine2D>();
            if (lines != null)
            {
                foreach (CLine2D l in lines)
                {
                    CLine2D lineClone = l.Clone();
                    clonedLines.Add(lineClone);
                }
            }
            return clonedLines;
        }

        public static float GetModelMaxLength(CModel model, DisplayOptions sDisplayOptions)
        {
            float fModel_Length_X = 0;
            float fModel_Length_Y = 0;
            float fModel_Length_Z = 0;
            Drawing3D.GetModelCentreWithoutCrsc(model, sDisplayOptions, out fModel_Length_X, out fModel_Length_Y, out fModel_Length_Z);

            float maxSize = MathF.Max(fModel_Length_X, fModel_Length_Y, fModel_Length_Z);
            return maxSize;
        }
    }
}
