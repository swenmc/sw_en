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
                                if (MemberLiesOnSurfaceLoadPlane(l, m, loadGroupTransform))
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

            // Coordinate system LCS
            Vector3D vLCS = new Vector3D(1, 1, 1);

            // Member coordinate system LCS in GCS
            Transform3DGroup memberTransformGroupLCS_to_GCS = new Transform3DGroup();

            // Surface coordinate system LCS in GCS
            Transform3DGroup loadTransformGroupLCS_to_GCS = new Transform3DGroup();

            // Member load direction
            Vector3D vMemberLoadDirection;

            bool bIsVerzia1 = true;
            if (bIsVerzia1)
            {
                // VERZIA 1

                // Member coordinate system LCS in GCS
                memberTransformGroupLCS_to_GCS = m.CreateTransformCoordGroup(m, true);
                Vector3D vMemberLCSinGCS = GetTransformedVector(vLCS, memberTransformGroupLCS_to_GCS); // Toto "vyzera spravne", ale este treba poskusat rozne moznosti

                // Surface coordinate system LCS in GCS
                loadTransformGroupLCS_to_GCS = GetSurfaceLoadTransformFromLCSToGCS(l, loadGroupTransform);
                Vector3D vLoadLCSinGCS = GetTransformedVector(vLCS, loadTransformGroupLCS_to_GCS);

                // Vector LCS of member in LCS of surface
                // Pozname poziciu LCS plochy a LCS pruta voci GCS,
                // Transformujeme LCS pruta do LCS plochy - To Ondrej - myslis ze to mozem urobit takto???
                Matrix3D matrixLCSMemberToLCSSurface = GetLocalToGlobalTransformMatrix(vLoadLCSinGCS, vMemberLCSinGCS);
                Vector3D vLCSMemberInLCSSurface = Vector3D.Multiply(vLCS, matrixLCSMemberToLCSSurface);

                // Surface Load Direction Vector
                l.SetLoadDirectionVector(l.fValue); // Set vector depending on value

                // Vystupny vektor zloziek zatazenia pruta
                // Vektor smeru zatazenia v ploche transformujeme do systemu LCS pruta v LCS systeme plochy - To Ondrej - myslis ze to mozem urobit takto???
                Matrix3D matrixLoadDirectionVectorToMember = GetLocalToGlobalTransformMatrix(vLCSMemberInLCSSurface, l.LoadDirectionVector);
                vMemberLoadDirection = Vector3D.Multiply(vLCS, matrixLoadDirectionVectorToMember);
            }
            else
            {
                // VERZIA 2

                // Surface Load Direction Vector
                l.SetLoadDirectionVector(l.fValue); // Set vector depending on value

                // Vector LCS of member in LCS of surface
                // Member coordinate system LCS in surface coordinate system
                Vector3D vLCSMemberInLCSSurface = GetTransformedVector(vLCS, inverseTrans); // TO ONDREJ - Tu je asi problem v tom ze inverseTransform nezohladnuje pootocenie pruta okolo vlastnej osi x v LCS, pretoze to nebolo robene geometrickou transformaciou modelu ale este pred vytvorenim 3D modelu

                // Vynasobime zlozky vektora smeru zatazenia plochy s vektorom LCS pruta v pozicii voci LCS plochy
                // Vobec neviem ci je to takto spravne :)))))

                // Vystupny vektor zloziek zatazenia pruta
                vMemberLoadDirection = new Vector3D(
                l.LoadDirectionVector.X * vLCSMemberInLCSSurface.X,
                l.LoadDirectionVector.Y * vLCSMemberInLCSSurface.Y,
                l.LoadDirectionVector.Z * vLCSMemberInLCSSurface.Z);




                // POKUSY

                // Member coordinate system LCS in GCS
                memberTransformGroupLCS_to_GCS = m.CreateTransformCoordGroup(m, true);

                // Surface coordinate system LCS in GCS
                loadTransformGroupLCS_to_GCS = GetSurfaceLoadTransformFromLCSToGCS(l, loadGroupTransform);

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
            }

            ELoadDirection eMemberLoadDirection;
            float fMemberLoadValueSign;

            // Nastavime znamienko a smer generovaneho zatazenia na prute
            l.GetLoadDirectionAndValueSign(vMemberLoadDirection, out fMemberLoadValueSign, out eMemberLoadDirection);

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
            bool bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x;

            if (MathF.d_equal(pStartLCS.X, pEndLCS.X))
            {
                bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x = false;
                pStartLCS.X -= fDist / 2;
                pEndLCS.X += fDist / 2;
            }
            else
            {
                bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x = true;
                pStartLCS.Y -= fDist / 2;
                pEndLCS.Y += fDist / 2;
            }

            Point p1r2 = Drawing3D.GetPoint_IgnoreZ(pStartLCS);
            Point p2r2 = Drawing3D.GetPoint_IgnoreZ(pEndLCS);

            Rect loadRect = new Rect(p1r1, p2r1); // Rectangle defined in LCS of surface load
            Rect memberRect = new Rect(p1r2, p2r2); // To Ondrej Tu bol problem - vracia to napriklad obdlznik s nulovym rozmerom Height ak ma prut globalne Y suradnice rovnake, dorobil som podmienku if (MathF.d_equal(pStartLCS.X, pEndLCS.X))

            Rect intersection = Drawing3D.GetRectanglesIntersection(loadRect, memberRect);

            double dMemberLoadStartCoordinate_x_axis;
            double dIntersectionLengthInMember_x_axis;
            double dIntersectionLengthInMember_yz_axis;

            // TODO - tu bude potrebne zapravoat ako je x plochy a x pruta vzajomne pootocene, ak o 180 stupnov, tak bude treba prehodit vsetko L - x
            if (bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x)
            {
                if(memberRect.Left - loadRect.Left > 0)
                    dMemberLoadStartCoordinate_x_axis = 0; // Prut zacina za plochou
                else
                    dMemberLoadStartCoordinate_x_axis = loadRect.Left - memberRect.Left; // Prut zacina pred plochou

                // Opacny smer osi pruta x voci osi x load surface
                if (pStartLCS.X > pEndLCS.X)
                {
                    dMemberLoadStartCoordinate_x_axis = m.FLength - intersection.Width;

                    if (loadRect.Width >= pStartLCS.X)
                        dMemberLoadStartCoordinate_x_axis = 0;
                }

                dIntersectionLengthInMember_x_axis = intersection.Width;   // Length of applied load
                dIntersectionLengthInMember_yz_axis = intersection.Height; // Tributary width
            }
            else
            {
                if (memberRect.Top - loadRect.Top > 0)
                    dMemberLoadStartCoordinate_x_axis = 0; // Prut zacina za plochou
                else
                    dMemberLoadStartCoordinate_x_axis = loadRect.Top - memberRect.Top;  // Prut zacina pred plochou

                // Opacny smer osi pruta x voci osi x load surface
                if (pStartLCS.Y > pEndLCS.Y)
                {
                    dMemberLoadStartCoordinate_x_axis = m.FLength - intersection.Height;

                    if (loadRect.Height >= pStartLCS.Y)
                        dMemberLoadStartCoordinate_x_axis = 0;
                }

                dIntersectionLengthInMember_x_axis = intersection.Height; // Length of applied load
                dIntersectionLengthInMember_yz_axis = intersection.Width; // Tributary width
            }

            if (intersection == Rect.Empty)
            {
                return;
            }
            else if (MathF.d_equal(dIntersectionLengthInMember_x_axis, m.FLength)) // Intersection in x direction of member is same as member length - generate uniform load per whole member length
            {
                float fq = fMemberLoadValueSign * Math.Abs(l.fValue) * (float)dIntersectionLengthInMember_yz_axis; // Load Value
                lc.MemberLoadsList.Add(new CMLoad_21(iLoadID, fq, m, EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, eMemberLoadDirection, true, 0));
                iLoadID += 1;
            }
            else
            {
                //nie som si isty,ci to je spravne
                float fq = (float)(fMemberLoadValueSign * Math.Abs(l.fValue) * dIntersectionLengthInMember_yz_axis); // Load Value
                float faA = (float)dMemberLoadStartCoordinate_x_axis; // Load start point on member (absolute coordinate x)
                float fs = (float)dIntersectionLengthInMember_x_axis; // Load segment length on member (absolute coordinate x)

                lc.MemberLoadsList.Add(new CMLoad_24(iLoadID, fq, faA, fs, m, EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, eMemberLoadDirection, true, 0));
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
        public static Transform3DGroup GetSurfaceLoadTransformFromLCSToGCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        {
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(load.CreateTransformCoordGroup());
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }
            return trans;
        }

        public static Vector3D GetTransformedVector(Vector3D v, Transform3D transformation)
        {
            Vector3D v_out = new Vector3D();

            v_out = transformation.Transform(v);

            return v_out;
        }

        public static Vector3D GetTransformedVector(Vector3D v, GeneralTransform3D transformation)
        {
            Vector3D v_out = new Vector3D();
            Point3D p_out = new Point3D();

            p_out = transformation.Transform(new Point3D(v.X, v.Y, v.Z));

            // Set output vector
            v_out.X = p_out.X;
            v_out.Y = p_out.Y;
            v_out.Z = p_out.Z;

            return v_out;
        }

        public static Matrix3D GetLocalToGlobalTransformMatrix(Vector3D vGlobalVector, Vector3D vLocalVector)
        {
            Vector3D X1 = new Vector3D(vGlobalVector.X, 0, 0);
            Vector3D X2 = new Vector3D(0, vGlobalVector.Y, 0);
            Vector3D X3 = new Vector3D(0, 0, vGlobalVector.Z);

            Vector3D X1_LCS = new Vector3D(vLocalVector.X, 0, 0);
            Vector3D X2_LCS = new Vector3D(0, vLocalVector.Y, 0);
            Vector3D X3_LCS = new Vector3D(0, 0, vLocalVector.Z);

            // This matrix will transform points from the rotated axis to the global
            Matrix3D LocalToGlobalTransformMatrix = new Matrix3D(
                Vector3D.DotProduct(X1, X1_LCS),
                Vector3D.DotProduct(X1, X2_LCS),
                Vector3D.DotProduct(X1, X3_LCS),
                0,
                Vector3D.DotProduct(X2, X1_LCS),
                Vector3D.DotProduct(X2, X2_LCS),
                Vector3D.DotProduct(X2, X3_LCS),
                0,
                Vector3D.DotProduct(X3, X1_LCS),
                Vector3D.DotProduct(X3, X2_LCS),
                Vector3D.DotProduct(X3, X3_LCS),
                0,

                0,
                0,
                0,
                1);

            return LocalToGlobalTransformMatrix;
        }

        public static Matrix3D GetGlobalToLocalTransformMatrix(Vector3D vGlobalVector, Vector3D vLocalVector)
        {
            Vector3D X1 = new Vector3D(vGlobalVector.X, 0, 0);
            Vector3D X2 = new Vector3D(0, vGlobalVector.Y, 0);
            Vector3D X3 = new Vector3D(0, 0, vGlobalVector.Z);

            Vector3D X1_LCS = new Vector3D(vLocalVector.X, 0, 0);
            Vector3D X2_LCS = new Vector3D(0, vLocalVector.Y, 0);
            Vector3D X3_LCS = new Vector3D(0, 0, vLocalVector.Z);

            // This matrix will transform points from the global system back to the rotated axis
            Matrix3D GlobalToLocalTransformMatrix = new Matrix3D(
                Vector3D.DotProduct(X1_LCS, X1),
                Vector3D.DotProduct(X1_LCS, X2),
                Vector3D.DotProduct(X1_LCS, X3),
                0,
                Vector3D.DotProduct(X2_LCS, X1),
                Vector3D.DotProduct(X2_LCS, X2),
                Vector3D.DotProduct(X2_LCS, X3),
                0,
                Vector3D.DotProduct(X3_LCS, X1),
                Vector3D.DotProduct(X3_LCS, X2),
                Vector3D.DotProduct(X3_LCS, X3),
                0,

                0,
                0,
                0,
                1);

            return GlobalToLocalTransformMatrix;
        }
    }
}
