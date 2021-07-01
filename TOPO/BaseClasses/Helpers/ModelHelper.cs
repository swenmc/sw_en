
using BaseClasses.GraphObj;
using MATH;
using System;
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
            return GetMembersInDistance(model, model.fL_tot_centerline, 1); // smer Y
        }

        public static CNode[] GetBackViewNodes(CModel model)
        {
            return GetNodesInDistance(model, model.fL_tot_centerline, 1); // smer Y
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
            return GetMembersInDistance(model, model.fW_frame_centerline, 0); // smer X
        }

        public static CNode[] GetRightViewNodes(CModel model)
        {
            return GetNodesInDistance(model, model.fW_frame_centerline, 0); // smer X
        }

        // Roof
        public static List<CMember> GetRoofViewMembers(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                // Coordinates are higher than (eave) wall height
                if (/*((m.NodeStart.Z >= model.fH1_frame_centerline || MathF.d_equal(m.NodeStart.Z, model.fH1_frame_centerline)) &&
                    ((m.NodeEnd.Z >= model.fH1_frame_centerline) || MathF.d_equal(m.NodeEnd.Z, model.fH1_frame_centerline))) &&*/
                    (
                    m.EMemberType == EMemberType_FS.eER || // Edge rafter
                    m.EMemberType == EMemberType_FS.eMR || // Main rafter
                    m.EMemberType == EMemberType_FS.eEP || // Eave purlin
                    m.EMemberType == EMemberType_FS.eP ||  // Purlin
                    m.EMemberType == EMemberType_FS.ePB || // Purlin Block
                    m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoof ||
                    m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoofCanopy
                    )
                    )
                    members.Add(m);
            }
            return members;
        }

        public static List<CNode> GetRoofViewNodes(CModel model)
        {
            List<CNode> nodes = new List<CNode>();

            // TODO - prepracovat ak su pridane canopies alebo je model monopitch tak neplati ze vyberame uzly s Z vyssie ako fH1_frame_centerline
            // Prebrat zoznam nodes z koncovych uzlov vyssie naplneneho zoznamu prutov members
            foreach (CNode n in model.m_arrNodes)
            {
                if (n.Z >= model.fH1_frame_centerline || MathF.d_equal(n.Z, model.fH1_frame_centerline)) nodes.Add(n);
            }
            return nodes;
        }

        public static List<CNode> GetNodesFromMembers(CMember[] members, bool removeDuplicit = true)
        {
            List<CNode> nodes = new List<CNode>();

            foreach (CMember m in members)
            {
                if (m.NodeStart != null)
                {
                    if (removeDuplicit) { if (!nodes.Contains(m.NodeStart)) nodes.Add(m.NodeStart); }
                    else nodes.Add(m.NodeStart);
                }

                if (m.NodeEnd != null)
                {
                    if (removeDuplicit) { if (!nodes.Contains(m.NodeEnd)) nodes.Add(m.NodeEnd); }
                    else nodes.Add(m.NodeEnd);
                }
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

        //WithoutCanopies
        public static CMember[] GetMembersIgnoreCanopies(CModel model)
        {
            List<CMember> members = new List<CMember>();

            foreach (CMember m in model.m_arrMembers)
            {
                if (m.EMemberTypePosition == EMemberType_FS_Position.MainRafterCanopy ||
                    m.EMemberTypePosition == EMemberType_FS_Position.EdgeRafterCanopy ||
                    m.EMemberTypePosition == EMemberType_FS_Position.EdgePurlinCanopy ||
                    m.EMemberTypePosition == EMemberType_FS_Position.PurlinCanopy ||
                    m.EMemberTypePosition == EMemberType_FS_Position.BracingBlockPurlinsCanopy ||
                    m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoofCanopy) continue;

                members.Add(m);
            }
            return members.ToArray();
        }

        public static CCladding GetCladdingBasedOnView(CModel model, EViewCladdingFilters view)
        {
            CCladding source = model.m_arrGOCladding[0];
            source.fibreglassSheetCollection = null; //dufam,ze takto nezrusim v povodnom tie FibreglassProperties

            CCladding cladding = source.Clone(); // To Ondrej - sme si isti ze toto Clone funguje spravne pre vsetky properties objektov ktore su v cladding ???
            cladding.Clear();
            if (view == EViewCladdingFilters.CLADDING_FRONT)
            {
                cladding.listOfCladdingSheetsFrontWall = source.listOfCladdingSheetsFrontWall;
                cladding.listOfFibreGlassSheetsWallFront = source.listOfFibreGlassSheetsWallFront;
            }
            else if (view == EViewCladdingFilters.CLADDING_BACK)
            {
                cladding.listOfCladdingSheetsBackWall = source.listOfCladdingSheetsBackWall;
                cladding.listOfFibreGlassSheetsWallBack = source.listOfFibreGlassSheetsWallBack;
            }
            else if (view == EViewCladdingFilters.CLADDING_LEFT)
            {
                cladding.listOfCladdingSheetsLeftWall = source.listOfCladdingSheetsLeftWall;
                cladding.listOfFibreGlassSheetsWallLeft = source.listOfFibreGlassSheetsWallLeft;
            }
            else if (view == EViewCladdingFilters.CLADDING_RIGHT)
            {
                cladding.listOfCladdingSheetsRightWall = source.listOfCladdingSheetsRightWall;
                cladding.listOfFibreGlassSheetsWallRight = source.listOfFibreGlassSheetsWallRight;
            }
            else if (view == EViewCladdingFilters.CLADDING_ROOF)
            {
                cladding.listOfCladdingSheetsRoofRight = source.listOfCladdingSheetsRoofRight;
                cladding.listOfFibreGlassSheetsRoofRight = source.listOfFibreGlassSheetsRoofRight;

                if (model.eKitset == EModelType_FS.eKitsetGableRoofEnclosed)
                {
                    cladding.listOfCladdingSheetsRoofLeft = source.listOfCladdingSheetsRoofLeft;
                    cladding.listOfFibreGlassSheetsRoofLeft = source.listOfFibreGlassSheetsRoofLeft;
                }
                else
                {
                    cladding.listOfCladdingSheetsRoofLeft = null;
                    cladding.listOfFibreGlassSheetsRoofLeft = null;
                }
            }

            return cladding;
        }

        public static List<CStructure_Door> GetDoorsForViewCladding(CModel model, EViewCladdingFilters view)
        {
            if (model == null) return null;
            if (model.m_arrGOStrDoors == null) return null;

            if (view == EViewCladdingFilters.CLADDING_FRONT)
            {
                return model.m_arrGOStrDoors.Where(d => d.Side == "Front").ToList();
            }
            else if (view == EViewCladdingFilters.CLADDING_BACK)
            {
                return model.m_arrGOStrDoors.Where(d => d.Side == "Back").ToList();
            }
            else if (view == EViewCladdingFilters.CLADDING_LEFT)
            {
                return model.m_arrGOStrDoors.Where(d => d.Side == "Left").ToList();
            }
            else if (view == EViewCladdingFilters.CLADDING_RIGHT)
            {
                return model.m_arrGOStrDoors.Where(d => d.Side == "Right").ToList();
            }            
            else return null;
        }

        //public static List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Front(CModel model)
        //{
        //    return model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall;
        //}
        //public static List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Back(CModel model)
        //{
        //    return model.m_arrGOCladding[0].listOfCladdingSheetsBackWall;
        //}
        //public static List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Left(CModel model)
        //{
        //    return model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall;
        //}
        //public static List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Right(CModel model)
        //{
        //    return model.m_arrGOCladding[0].listOfCladdingSheetsRightWall;
        //}
        //public static List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Roof_Right(CModel model)
        //{
        //    return model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight;
        //}
        //public static List<CCladdingOrFibreGlassSheet> GetCladdingSheets_Roof_Left(CModel model)
        //{
        //    return model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft;
        //}

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
                return GetMembersInDistance(model, model.L1_Bays[0], iDirectionCode); //bay number 1
            else
                return GetMembersInDistanceInterval(model, 0, model.L1_Bays[0], iDirectionCode, false, true, true); //bay number 1
        }

        public static CNode[] GetMiddleFrameNodes(CModel model, bool bAddGirtsAndPurlins = true)
        {
            int iDirectionCode = 1; // iDirectionCode 0- direction X, 1-direction Y, 2 - direction Z

            if (!bAddGirtsAndPurlins)
                return GetNodesInDistance(model, model.L1_Bays[0], iDirectionCode); //bay number 1 
            else
                return GetNodesInDistanceInterval(model, 0, model.L1_Bays[0], iDirectionCode, false, true); //bay number 1
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

        public static CMember[] GetMembersInDistance(CModel model, double coordinate, int iDirectionCode, params EMemberType_FS[] types)
        {
            CMember[] membersAll = GetMembersInDistance(model, coordinate, iDirectionCode);

            List<CMember> members = new List<CMember>();
            foreach (CMember m in membersAll)
            {
                foreach (EMemberType_FS t in types)
                {
                    if (m.EMemberType == t)
                        members.Add(m);
                }
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
            foreach (CConnectionJointTypes j in model.m_arrConnectionJoints)
            {
                foreach (CMember m in membersList)
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

            if (iDirectionCode == (int)EGCSDirection.X) // Direction X
                return n.X;
            if (iDirectionCode == (int)EGCSDirection.Y) // Direction Y
                return n.Y;
            else //if (iDirectionCode == 2) // Direction Z
                return n.Z;
        }

        private static double SetPointCoordinateForSpecificDirection(Point3D p, int iDirectionCode)
        {
            // Funckia vrati suradnicu bodu pre specificky smer GCS ktory chceme uvazovat

            if (iDirectionCode == (int)EGCSDirection.X) // Direction X
                return p.X;
            if (iDirectionCode == (int)EGCSDirection.Y) // Direction Y
                return p.Y;
            else //if (iDirectionCode == 2) // Direction Z
                return p.Z;
        }

        public static int GetFirstBayWithoutDoors(ObservableCollection<DoorProperties> doors, string side)
        {
            int freeBay = 1;
            if (doors != null)
            {
                IEnumerable<DoorProperties> doorsOnSide = doors.Where(d => d.sBuildingSide == side);
                foreach (DoorProperties door in doorsOnSide.OrderBy(d => d.iBayNumber))
                {
                    if (freeBay == door.iBayNumber) freeBay++;
                }
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

        public static bool IsNodesLocationIdentical(CNode n1, CNode n2)
        {
            if (MathF.d_equal(n1.X, n2.X) && MathF.d_equal(n1.Y, n2.Y) && MathF.d_equal(n1.Z, n2.Z))
                return true;
            else
                return false;
        }



        

        public static float GetBaysWidthUntil(int bayIndex, IList<CBayInfo> bayWidths)
        {
            float w = 0;
            for (int i = 0; i < bayIndex; i++)
            {
                if (i >= bayWidths.Count) continue;
                w += bayWidths[i].Width;
            }
            return w;
        }

        public static float GetBaysWidthUntil(int bayIndex, float bayWidth)
        {
            float w = 0;
            for (int i = 0; i < bayIndex; i++)
            {
                w += bayWidth;
            }
            return w;
        }

        public static double GetVerticalCoordinate(string sBuildingSide, EModelType_FS eKitset, double width, double leftHeight, double x, float fRoofPitch_deg)
        {
            if(sBuildingSide == "Front" || sBuildingSide == "Back")
            {
                if (eKitset == EModelType_FS.eKitsetMonoRoofEnclosed)
                {
                    if (sBuildingSide == "Back")
                        return leftHeight + x * Math.Tan(-fRoofPitch_deg * Math.PI / 180);
                    else
                        return leftHeight + x * Math.Tan(fRoofPitch_deg * Math.PI / 180);
                }
                else if (x < 0.5f * width)
                    return leftHeight + x * Math.Tan(fRoofPitch_deg * Math.PI / 180);
                else
                    return leftHeight + (width - x) * Math.Tan(fRoofPitch_deg * Math.PI / 180);
            }
            else // (sBuildingSide == "Left" || sBuildingSide == "Right") // a Roof
               return leftHeight;
        }

        public static double GetMaximumAvailable_FG_SheetTopCoordinate(string side, // Strana budovy
            EModelType_FS eModelType,
            float fRoofPitch_deg,
            //double bottomEdge_z,
            double width, // Sirka steny budovy, pre ktoru generujeme sheets
            //double height_middle_basic_aux, // Stred budovy gable roof - roof ridge
            double height_middle_basic_aux_total, // Stred budovy gable roof - roof ridge
            double height_left_basic, // Vyska steny vlavo
            double claddingWidthModular,
            int sheetIndex) // index pre sheet ktoreho vysku zistujeme
        {
            if (sheetIndex < 0)
                throw new Exception("Invalid sheet index!");

            // Predpokladame ze FB su obdlzniky a na hornom okraji sa nebudu zrezavat sikmo pre front a back wall ale vzdy nad nimi bude este aj plech

            int iNumberOfWholeSheets = (int)(width / claddingWidthModular);
            //double dWidthOfWholeSheets = iNumberOfWholeSheets * claddingWidthModular;
            //double dLastSheetWidth = width - dWidthOfWholeSheets; // Last Sheet
            int iNumberOfOriginalSheetsOnSide = iNumberOfWholeSheets + 1; // Celkovy pocet povodnych sheets

            // Zakladne hodnoty pre obdlznik
            //int iNumberOfEdges = 4;
            // TODO - overit ci sa ma v height pocitat s bottomEdge_z
            double height_left = height_left_basic;
            double height_right = height_left_basic;
            double height_toptip = height_left_basic;
            //double tipCoordinate_x = 0.5 * (sheetIndex == iNumberOfOriginalSheetsOnSide - 1 ? dLastSheetWidth : claddingWidthModular);

            if (side == "Front" || side == "Back")
            {
                // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                height_left = GetVerticalCoordinate(side, eModelType, width, height_left_basic, sheetIndex * claddingWidthModular, fRoofPitch_deg);
                height_right = GetVerticalCoordinate(side, eModelType, width, height_left_basic, (sheetIndex + 1) * claddingWidthModular, fRoofPitch_deg);
                height_toptip = 0.5 * (height_left + height_right);
                //tipCoordinate_x = 0.5 * claddingWidthModular;

                if (sheetIndex == iNumberOfOriginalSheetsOnSide - 1)
                {
                    // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                    height_right = GetVerticalCoordinate(side, eModelType, width, height_left_basic, width, fRoofPitch_deg);
                    height_toptip = 0.5 * (height_left + height_right);
                    //tipCoordinate_x = 0.5 * dLastSheetWidth;
                }

                if (eModelType == EModelType_FS.eKitsetGableRoofEnclosed &&
                   sheetIndex * claddingWidthModular < 0.5 * width &&
                   (sheetIndex + 1) * claddingWidthModular > 0.5 * width)
                {
                    //iNumberOfEdges = 5;
                    // TODO - overit ci sa ma v height pocitat s bottomEdge_z
                    height_toptip = height_middle_basic_aux_total; //-bottomEdge_z + height_middle_basic_aux; // Stred budovy gable roof - roof ridge
                    //tipCoordinate_x = 0.5 * width - sheetIndex * claddingWidthModular;
                }
            }

            return Math.Min(Math.Min(height_left, height_right), height_toptip);
        }



        
    }
}
