using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public static class CLoadGenerator
    {

        //// TODO - Ondrej, pripravit staticku triedu a metody pre generovanie member load zo surface load v zlozke Loading
        //float fMemberLoadValue = ((CSLoad_FreeUniform)m_arrLoadCases[01].SurfaceLoadsList[0]).fValue * fDist_Purlin;

        //    foreach (CMember m in listOfPurlins)
        //    {
        //        m_arrLoadCases[01].MemberLoadsList = new List<CMLoad>();
        //        m_arrLoadCases[01].MemberLoadsList.Add(new CMLoad_21(fMemberLoadValue, m, EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
        //    }

        //public static void GenerateMemberLoads(CLoadCase[] m_arrLoadCases, List<CMember> members, float fDist)
        //{
        //    foreach (CLoadCase lc in m_arrLoadCases)
        //    {
        //        int iLoadID_Temp = 1;
        //        foreach (CSLoad_Free csload in lc.SurfaceLoadsList)
        //        {
        //            if (csload is CSLoad_FreeUniform)
        //            {
        //                float fMemberLoadValue = ((CSLoad_FreeUniform)csload).fValue * fDist;
        //                foreach (CMember m in members)
        //                {
        //                    lc.MemberLoadsList.Add(new CMLoad_21(iLoadID_Temp, fMemberLoadValue, m, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
        //                    iLoadID_Temp += 1;
        //                }
        //            }
        //            if (csload is CSLoad_FreeUniformGroup)
        //            {
        //                CSLoad_FreeUniformGroup group = (CSLoad_FreeUniformGroup)csload;
        //                foreach (CSLoad_FreeUniform csloadFree in group.LoadList)
        //                {
        //                    float fMemberLoadValue = csloadFree.fValue * fDist;
        //                    foreach (CMember m in members)
        //                    {
        //                        lc.MemberLoadsList.Add(new CMLoad_21(iLoadID_Temp, fMemberLoadValue, m, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
        //                        iLoadID_Temp += 1;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void GenerateMemberLoads(CLoadCase loadCase, List<CMember> members, float fDist)
        //{
        //    foreach (CSLoad_Free csload in loadCase.SurfaceLoadsList)
        //    {
        //        float fMemberLoadValue = ((CSLoad_FreeUniform)csload).fValue * fDist;
        //        foreach (CMember m in members)
        //        {
        //            loadCase.MemberLoadsList.Add(new CMLoad_21(fMemberLoadValue, m, EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
        //        }
        //    }
        //}


        public static void GenerateMemberLoads(CLoadCase[] m_arrLoadCases, List<CMember> members, float fDist)
        {
            foreach (CLoadCase lc in m_arrLoadCases)
            {
                int iLoadID = 0;
                int c = 0;
                foreach (CSLoad_Free csload in lc.SurfaceLoadsList)
                {
                    c++;
                    foreach (CMember m in members)
                    {
                        if (csload is CSLoad_FreeUniformGroup)
                        {
                            Transform3DGroup loadGroupTransform = ((CSLoad_FreeUniformGroup)csload).CreateTransformCoordGroupOfLoadGroup();
                            foreach (CSLoad_FreeUniform l in ((CSLoad_FreeUniformGroup)csload).LoadList)
                            {
                                if (MemberLiesOnSurfaceLoadPlane(l, m, null)) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");
                                else { System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}"); continue; }

                                GeneralTransform3D inverseTrans = GetSurfaceLoadTransformFromGCSToLCS(l, loadGroupTransform);
                                Point3D pStart = new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z);
                                Point3D pEnd = new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z);

                                Point3D pStartLCS = inverseTrans.Transform(pStart);
                                Point3D pEndLCS = inverseTrans.Transform(pEnd);

                                if(Drawing3D.LineLiesOnPlane(l.SurfaceDefinitionPoints[0], l.SurfaceDefinitionPoints[1], l.SurfaceDefinitionPoints[2], pStartLCS, pEndLCS))
                                    System.Diagnostics.Trace.WriteLine($"SUPER. LIES ON PLANE IN LCS TOO. LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                //ak by bol cely pod tou surface load
                                float fMemberLoadValue = l.fValue * fDist;
                                lc.MemberLoadsList.Add(new CMLoad_21(iLoadID, fMemberLoadValue, m, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
                                iLoadID += 1;

                                
                            }
                        }
                        else if (csload is CSLoad_FreeUniform)
                        {
                            CSLoad_FreeUniform l = (CSLoad_FreeUniform)csload;
                            if(MemberLiesOnSurfaceLoadPlane(l, m, null)) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");
                            else { System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}"); continue; }
                            
                            GeneralTransform3D inverseTrans = GetSurfaceLoadTransformFromGCSToLCS(l, null);
                            Point3D pStart = new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z);
                            Point3D pEnd = new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z);

                            Point3D pStartLCS = inverseTrans.Transform(pStart);
                            Point3D pEndLCS = inverseTrans.Transform(pEnd);

                            if (Drawing3D.LineLiesOnPlane(l.SurfaceDefinitionPoints[0], l.SurfaceDefinitionPoints[1], l.SurfaceDefinitionPoints[2], pStartLCS, pEndLCS))
                                System.Diagnostics.Trace.WriteLine($"SUPER. LIES ON PLANE IN LCS TOO. LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                            //ak by bol cely pod tou surface load
                            float fMemberLoadValue = l.fValue * fDist;
                            lc.MemberLoadsList.Add(new CMLoad_21(iLoadID, fMemberLoadValue, m, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
                            iLoadID += 1;
                        }

                    } //foreach member
                } //foreach surface load in load case
            } //foreach loadcase
        }


        private static bool MemberLiesOnSurfaceLoadPlane(CSLoad_FreeUniform l, CMember m, Transform3DGroup loadGroupTransform)
        {
            l.PointsGCS = GetSurfaceLoadCoordinates_GCS(l, loadGroupTransform); // Positions in global coordinate system GCS
            if (l.PointsGCS.Count < 2) { System.Diagnostics.Trace.WriteLine("ERROR in method MemberLiesOnSurfaceLoadPlane. Surface points collection must have 3 points. LoadID: " + l.ID); return false; }
            
            return Drawing3D.MemberLiesOnPlane(l.PointsGCS[0], l.PointsGCS[1], l.PointsGCS[2], m);
        }

        //public static List<Point3D> GetSurfaceLoadCoordinates_GCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        //{
        //    Model3DGroup gr = load.CreateM_3D_G_Load();
        //    if (gr.Children.Count < 1) return new List<Point3D>();

        //    Transform3DGroup trans = new Transform3DGroup();
        //    trans.Children.Add(gr.Transform);
        //    if (groupTransform != null)
        //    {
        //        trans.Children.Add(groupTransform);
        //    }

        //    List<Point3D> transPoints = new List<Point3D>();
        //    foreach (Point3D p in load.SurfaceDefinitionPoints)
        //        transPoints.Add(trans.Transform(p));

        //    return transPoints;
        //}
        public static List<Point3D> GetSurfaceLoadCoordinates_GCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        {
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(load.CreateTransformCoordGroup());
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }

            List<Point3D> transPoints = new List<Point3D>();
            foreach (Point3D p in load.SurfaceDefinitionPoints)
                transPoints.Add(trans.Transform(p));

            return transPoints;
        }

        public static GeneralTransform3D GetSurfaceLoadTransformFromGCSToLCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        {
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(load.CreateTransformCoordGroup());
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }
            return trans.Inverse;
        }

    }
}
