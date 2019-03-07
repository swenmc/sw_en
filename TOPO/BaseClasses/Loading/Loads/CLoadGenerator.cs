using BaseClasses.GraphObj.Objects_3D;
using MATH;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace BaseClasses
{
    public static class CLoadGenerator
    {
        static bool bDebugging = false; // Console output


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
                                if (MemberLiesOnSurfaceLoadPlane(l, m, loadGroupTransform)) // TO Ondrej - tu bol parameter loadGroupTransform = null, ale potom to nikdy nenaslo prut na ploche pretoze dielcia plocha surface load v ramci group bola v LCS, dal som tam ako parameter tu transformaciu, skus sa na to pozriet ci je to spravne
                                {
                                    if (bDebugging) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                    if(m.BIsDisplayed) // TODO - tu by mala byt podmienka ci je prut aktivny pre vypocet (nie len ci je zobrazeny) potrebujeme doriesit co s prutmi, ktore boli v mieste kde sa vlozili dvere, zatial som ich nemazal, lebo som si nebol isty ci by mi sedeli ID pre generovanie zatazenia, chcel som ich len deaktivovat
                                        GenerateMemberLoad(lc, l, m, loadGroupTransform, fDist, ref iLoadID);
                                }
                                else { /*System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}");*/ continue; }

                            }
                        }
                        else if (csload is CSLoad_FreeUniform)
                        {
                            CSLoad_FreeUniform l = (CSLoad_FreeUniform)csload;

                            if (MemberLiesOnSurfaceLoadPlane(l, m, null))
                            {
                                if(bDebugging) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                if (m.BIsDisplayed) // TODO - tu by mala byt podmienka ci je prut aktivny pre vypocet (nie len ci je zobrazeny) potrebujeme doriesit co s prutmi, ktore boli v mieste kde sa vlozili dvere, zatial som ich nemazal, lebo som si nebol isty ci by mi sedeli ID pre generovanie zatazenia, chcel som ich len deaktivovat
                                    GenerateMemberLoad(lc, l, m, null, fDist, ref iLoadID);
                            }
                            else { /*System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}");*/ continue; }

                        }

                    } //foreach member
                } //foreach surface load in load case
            } //foreach loadcase
        }


        private static bool MemberLiesOnSurfaceLoadPlane(CSLoad_FreeUniform l, CMember m, Transform3DGroup loadGroupTransform)
        {
            l.PointsGCS = GetSurfaceLoadCoordinates_GCS(l, loadGroupTransform); // Positions in global coordinate system GCS

            if (l.PointsGCS.Count < 2) { return false; }

            return Drawing3D.MemberLiesOnPlane(l.PointsGCS[0], l.PointsGCS[1], l.PointsGCS[2], m);
        }

        private static void GenerateMemberLoad(CLoadCase lc, CSLoad_FreeUniform l, CMember m, Transform3DGroup loadGroupTransform, float fDist, ref int iLoadID)
        {
            GeneralTransform3D inverseTrans = GetSurfaceLoadTransformFromGCSToLCS(l, loadGroupTransform);
            Point3D pStart = new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z);
            Point3D pEnd = new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z);

            Point3D pStartLCS = inverseTrans.Transform(pStart); // To Ondrej: Tu sa sice vratia nejake suradnice pruta v LCS plochy ale nie som si isty ci sa spravne uvazuje pootocenie medzi osou x plochy a x pruta !!!
            Point3D pEndLCS = inverseTrans.Transform(pEnd);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // MC 7.3.2019: To Ondrej - moje "analyticke" pochody a pokusy :) - prosim zatial nemazat

            // LCS axes vector
            Vector3D vLCS_X = new Vector3D(1, 0, 0);
            Vector3D vLCS_Y = new Vector3D(0, 1, 0);
            Vector3D vLCS_Z = new Vector3D(0, 0, 1);

            Transform3DGroup memberTransformGroupLCS_to_GCS = new Transform3DGroup();
            Transform3DGroup loadTransformGroupLCS_to_GCS = new Transform3DGroup();

            // Zaciatok LCS pruta v GCS
            Point3D pMemberLCSOrigin = pStart;
            // Smerove vektory LCS os pruta v GCS
            Vector3D vMember_X = GetTransformedVector(vLCS_X, memberTransformGroupLCS_to_GCS);
            Vector3D vMember_Y = GetTransformedVector(vLCS_Y, memberTransformGroupLCS_to_GCS);
            Vector3D vMember_Z = GetTransformedVector(vLCS_Z, memberTransformGroupLCS_to_GCS);

            // Smerovy vektor pruta v GCS
            Vector3D vMember = new Vector3D(m.Delta_X, m.Delta_Y, m.Delta_Z);

            // Zaciatok LCS plochy zatazenia v GCS
            Point3D pSurfaceLCSOrigin = new Point3D(l.PointsGCS[0].X, l.PointsGCS[0].Y, l.PointsGCS[0].Z);
            // Smerove vektory LCS os plochy v GCS
            Vector3D vLoad_X = GetTransformedVector(vLCS_X, loadTransformGroupLCS_to_GCS);
            Vector3D vLoad_Y = GetTransformedVector(vLCS_Y, loadTransformGroupLCS_to_GCS);
            Vector3D vLoad_Z = GetTransformedVector(vLCS_Z, loadTransformGroupLCS_to_GCS);

            /*
            // Smerove vektory hran plochy
            Vector3D vLoad_X = new Vector3D(l.PointsGCS[1].X - l.PointsGCS[0].X, l.PointsGCS[l.PointsGCS.Count - 1].Y - l.PointsGCS[0].Y, 1);
            Vector3D vLoad_Y = new Vector3D(l.PointsGCS[1].X - l.PointsGCS[0].X, l.PointsGCS[l.PointsGCS.Count - 1].Y - l.PointsGCS[0].Y, 1);
            Vector3D vLoad_Z = new Vector3D(l.PointsGCS[1].X - l.PointsGCS[0].X, l.PointsGCS[l.PointsGCS.Count - 1].Y - l.PointsGCS[0].Y, 1);
            */


            // Podla vzajomneho pootocenia a orientacie osovych systemov pruta a plochy urcime smer a znamienko zatazenia pruta
            // Je treba rozlisovat ze zatazenie moze byt zadane v GCS alebo LCS

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            // Surface load - local coordinate system

            // Rectangular

            //            4 [0,cY]  _____________  3 [cX,cY]
            //                     |             |
            //                     |             |
            // ^ y                 |             |
            // |                   |_____________|
            // |          1 [0,0]                  2 [cX,0]
            // o----->x

            // Trapezoidal
            //                            4 [cX2,cY2]
            //                           /\
            //                          /  \
            //                         /    \
            //                        /      \
            //                       /        \
            //            5 [0,cY1] /          \  3 [cX1,cY1]
            //                     |            |
            // ^ y                 |            |
            // |                   |____________|
            // |          1 [0,0]                  2 [cX1,0]
            // o----->x

            //tu si nie som uplne isty,ci je dany 1. a 3. bodom (obdlznik je definovany 2 bodmi, bottomleft a topright)
            // To Ondrej - ak tomu rozumiem spravne tak rectangle Rect je definovany v lavotocivom systeme x,y s [0,0] v TopLeft,
            // zatial co body plochy su definovane v pravotocivom systeme x,y s [0,0] BottomLeft
            // Neviem ci to nemoze robit problemy

            Point p1r1 = Drawing3D.GetPoint_IgnoreZ(l.SurfaceDefinitionPoints[0]);
            Point p2r1 = Drawing3D.GetPoint_IgnoreZ(l.SurfaceDefinitionPoints[2]);

            // TO Ondrej - Tento rozmer musi byt vzdy kolmy na lokalnu osu x pruta
            if (MathF.d_equal(pStartLCS.X, pEndLCS.X))
            {
                pStartLCS.X -= fDist / 2;
                pEndLCS.X += fDist / 2;
            }
            else
            {
                pStartLCS.Y -= fDist / 2;
                pEndLCS.Y += fDist / 2;
            }

            Point p1r2 = Drawing3D.GetPoint_IgnoreZ(pStartLCS);
            Point p2r2 = Drawing3D.GetPoint_IgnoreZ(pEndLCS);

            Rect loadRect = new Rect(p1r1, p2r1); // Rectangle defined in LCS of surface load
            Rect memberRect = new Rect(p1r2, p2r2); // To Ondrej Tu bol problem - vracia to napriklad obdlznik s nulovym rozmerom Height ak ma prut globalne Y suradnice rovnake, dorobil som podmienku if (MathF.d_equal(pStartLCS.X, pEndLCS.X))

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // MC 7.3.2019: Moje pokusy - prosim zatial nemazat

            // Load direction and load orientation depends on LCSs relationship
            // Ak normala plochy smeruje rovnakym smerom ako osa z LCS pruta tak je typ a orientacia (znamienko hodnoty zatazenia) rovnake
            // Treba sa s tym pohrat, moze to byt v GCS alebo LCS podla toho v ktorom systeme je definovane zatazenie plochy
            EMLoadDirPCC1 eMemberLoadDirection = EMLoadDirPCC1.eMLD_PCC_FZV_MYU; // Default

            Vector3D surfaceNormal_GCS = Drawing3D.GetSurfaceNormalVector(l.PointsGCS[0], l.PointsGCS[1], l.PointsGCS[l.PointsGCS.Count - 1]);

            if(MathF.d_equal(surfaceNormal_GCS.Z, 1, 0.0001))
            {
                eMemberLoadDirection = EMLoadDirPCC1.eMLD_PCC_FZV_MYU;
            }
            else if(MathF.d_equal(surfaceNormal_GCS.Y, 1, 0.0001) && MathF.d_equal(m.Delta_Y, 0, 0.0001))
            {
                eMemberLoadDirection = EMLoadDirPCC1.eMLD_PCC_FYU_MZV;
            }
            else if (MathF.d_equal(surfaceNormal_GCS.X, 1, 0.0001) && MathF.d_equal(m.Delta_X, 0, 0.0001))
            {
                eMemberLoadDirection = EMLoadDirPCC1.eMLD_PCC_FYU_MZV;
            }
            /*
            else
            {
                eMemberLoadDirection = EMLoadDirPCC1.eMLD_PCC_FXX_MXX; // ????????
            }*/

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Rect intersection = Drawing3D.GetRectanglesIntersection(loadRect, memberRect);
            if (intersection == Rect.Empty)
            {
                return;
            }
            else if (intersection == memberRect)
            {
                float fq = l.fValue * fDist; // Load Value
                lc.MemberLoadsList.Add(new CMLoad_21(iLoadID, fq, m, ELoadCoordSystem.eLCS, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, eMemberLoadDirection, true, 0));
                iLoadID += 1;
            }
            else
            {
                //nie som si isty,ci to je spravne
                float fq = (float)(l.fValue * intersection.Height); // Load Value
                float faA = (float)(memberRect.Width - intersection.Width); // Load start point on member (absolute coordinate x)
                float fs = (float)intersection.Width; // Load segment length on member (absolute coordinate x)

                lc.MemberLoadsList.Add(new CMLoad_24(iLoadID, fq, faA, fs, m, ELoadCoordSystem.eLCS, EMLoadTypeDistr.eMLT_QUF_PG_24, EMLoadType.eMLT_F, eMemberLoadDirection, true, 0));
                iLoadID += 1;
            }
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

        public static Vector3D GetTransformedVector(Vector3D v, Transform3D transformation)
        {
            Vector3D v_out = new Vector3D();

            // TODO Ondrej - Tu by som chcel transformovat suradnice vektora podla toho ako bol objekt (mebmer, surface) transformovany z LCS do GCS
            // Da sa to nejako vymysliet ??? :)))
            // Napriklad nejaky fake 3D objekt, ktory by mal vybrany bod so suradnicami vektora a po transformacii by sa nove suradnice toho bodu nastavili do vektora

            return v_out;
        }
    }
}
