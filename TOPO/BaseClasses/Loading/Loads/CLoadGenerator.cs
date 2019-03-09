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
    public struct MemberLoadParameters
    {
        public ELoadDirection eMemberLoadDirection;
        public float fSurfaceLoadValueFactor;
        public float fMemberLoadValueSign;
    }

    public static class CLoadGenerator
    {
        static bool bDebugging = false; // Console output

        public static void GenerateMemberLoads(CLoadCase[] m_arrLoadCases, List<CMember> allMembersInModel)
        {
            foreach (CLoadCase lc in m_arrLoadCases)
            {
                int iLoadID = 0;
                int c = 0;
                foreach (CSLoad_Free csload in lc.SurfaceLoadsList)
                {
                    c++;
                    foreach (CMember m in allMembersInModel)
                    {
                        foreach (FreeSurfaceLoadsMemberTypeData mtypedata in csload.listOfLoadedMemberTypeData)
                        {
                            if (m.EMemberType == mtypedata.memberType) // Prut je rovnakeho typu ako je niektory z typov prutov zo skupiny typov ktoru plocha zatazuje
                            {
                                if(m.EMemberType == EMemberType_FS.eEP)
                                {

                                }

                                if (csload is CSLoad_FreeUniformGroup)
                                {
                                    Transform3DGroup loadGroupTransform = ((CSLoad_FreeUniformGroup)csload).CreateTransformCoordGroupOfLoadGroup();
                                    foreach (CSLoad_FreeUniform l in ((CSLoad_FreeUniformGroup)csload).LoadList)
                                    {
                                        if (MemberLiesOnSurfaceLoadPlane(l, m, loadGroupTransform)) // Prut lezi na ploche
                                        {
                                            if (bDebugging) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                            if (m.BIsDisplayed) // TODO - tu by mala byt podmienka ci je prut aktivny pre vypocet (nie len ci je zobrazeny) potrebujeme doriesit co s prutmi, ktore boli v mieste kde sa vlozili dvere, zatial som ich nemazal, lebo som si nebol isty ci by mi sedeli ID pre generovanie zatazenia, chcel som ich len deaktivovat
                                                GenerateMemberLoad(lc, l, m, loadGroupTransform, mtypedata.fLoadingWidth, ref iLoadID);
                                        }
                                        else { /*System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}");*/ continue; }
                                    }
                                }
                                else if (csload is CSLoad_FreeUniform)
                                {
                                    CSLoad_FreeUniform l = (CSLoad_FreeUniform)csload;

                                    if (MemberLiesOnSurfaceLoadPlane(l, m, null)) // Prut lezi na ploche
                                    {
                                        if (bDebugging) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                        if (m.BIsDisplayed) // TODO - tu by mala byt podmienka ci je prut aktivny pre vypocet (nie len ci je zobrazeny) potrebujeme doriesit co s prutmi, ktore boli v mieste kde sa vlozili dvere, zatial som ich nemazal, lebo som si nebol isty ci by mi sedeli ID pre generovanie zatazenia, chcel som ich len deaktivovat
                                            GenerateMemberLoad(lc, l, m, null, mtypedata.fLoadingWidth, ref iLoadID);
                                    }
                                    else { /*System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}");*/ continue; }
                                }
                            } // member type is included in group of types
                        } //foreach memberType in group of types loaded by surface load
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
            // Transformacia pruta do LCS plochy
            GeneralTransform3D inverseTrans = GetSurfaceLoadTransformFromGCSToLCS(l, loadGroupTransform);
            Point3D pStart = new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z);
            Point3D pEnd = new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z);

            Point3D pStartLCS = inverseTrans.Transform(pStart);
            Point3D pEndLCS = inverseTrans.Transform(pEnd);

            // Transformacia bodov plochy do LCS pruta
            Transform3DGroup trans = m.CreateTransformCoordGroup(m, true);
            GeneralTransform3D inverseTrans2 = trans.Inverse;
            List<Point3D> surfaceDefPointsGCS = GetSurfaceLoadCoordinates_GCS(l, loadGroupTransform);

            List<Point3D> surfaceDefPointsLCSMember = new List<Point3D>();
            foreach (Point3D p in surfaceDefPointsGCS) surfaceDefPointsLCSMember.Add(inverseTrans.Transform(p));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
            // Load direction transformation

            // Surface Load Direction Vector
            l.SetLoadDirectionVector(l.fValue); // Set vector depending on value

            // Member coordinate system LCS in GCS
            Transform3DGroup memberTransformGroupLCS_to_GCS = m.CreateTransformCoordGroup(m, true);

            // Surface coordinate system LCS in GCS
            Transform3DGroup loadTransformGroupLCS_to_GCS = GetSurfaceLoadTransformFromLCSToGCS(l, loadGroupTransform);

            // Surface load direction vector in GCS
            Vector3D vLoadDirectioninGCS = GetTransformedVector(l.LoadDirectionVector, loadTransformGroupLCS_to_GCS);
            Vector3D vloadDirectioninLCS = ((Transform3D)(memberTransformGroupLCS_to_GCS.Inverse)).Transform(vLoadDirectioninGCS);

            // Ak nie su vsetky osi pruta kolme na osi plochy, moze nastat pripad ze je potrebne vygenerovat viac zatazeni
            // (tj. zatazenie z plochy je potrebne rozlozit do viacerych smerov na prute, vznike teda viacero objektov member load)
            // Zistime ktore zlozky su ine nez 0 a ma sa pre ne generovat zatazenie

            List<MemberLoadParameters> listMemberLoadParams = new List<MemberLoadParameters>();

            if(!MathF.d_equal(Math.Abs(vloadDirectioninLCS.X),0, 0.001))
            {
                MemberLoadParameters parameters_LCS_X = new MemberLoadParameters();

                parameters_LCS_X.fSurfaceLoadValueFactor = (float)vloadDirectioninLCS.X;
                parameters_LCS_X.eMemberLoadDirection = ELoadDirection.eLD_X;
                parameters_LCS_X.fMemberLoadValueSign = vloadDirectioninLCS.X < 0.0 ? -1 : 1;

                listMemberLoadParams.Add(parameters_LCS_X);
            }

            if (!MathF.d_equal(Math.Abs(vloadDirectioninLCS.Y),0, 0.001))
            {
                MemberLoadParameters parameters_LCS_Y = new MemberLoadParameters();

                parameters_LCS_Y.fSurfaceLoadValueFactor = (float)vloadDirectioninLCS.Y;
                parameters_LCS_Y.eMemberLoadDirection = ELoadDirection.eLD_Y;
                parameters_LCS_Y.fMemberLoadValueSign = vloadDirectioninLCS.Y < 0.0 ? -1 : 1;

                listMemberLoadParams.Add(parameters_LCS_Y);
            }

            if (!MathF.d_equal(Math.Abs(vloadDirectioninLCS.Z),0, 0.001))
            {
                MemberLoadParameters parameters_LCS_Z = new MemberLoadParameters();

                parameters_LCS_Z.fSurfaceLoadValueFactor = (float)vloadDirectioninLCS.Z;
                parameters_LCS_Z.eMemberLoadDirection = ELoadDirection.eLD_Z;
                parameters_LCS_Z.fMemberLoadValueSign = vloadDirectioninLCS.Z < 0.0 ? -1 : 1;

                listMemberLoadParams.Add(parameters_LCS_Z);
            }

            // Validation
            if(listMemberLoadParams.Count == 0) // Nepodarilo sa vygenerovat ziadne zatazenie pruta
            {
                throw new Exception("Error. Member load can't be generated.");
            }


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

            // TODO - toto by sa asi tiez dalo nejako pekne vyriesit cez Vector LCS pruta v LCS plochy, resp opacne
            // TODO - tu bude potrebne zapravoat ako je x plochy a x pruta vzajomne pootocene, ak o 180 stupnov, tak bude treba prehodit vsetko L - x
            if (bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x) // x pruta a x plochy su na jednej priamke
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
            else  // x pruta a x plochy nie na jednej priamke
            {
                if (memberRect.Top - loadRect.Top > 0)
                    dMemberLoadStartCoordinate_x_axis = 0; // Prut zacina za plochou
                else
                    dMemberLoadStartCoordinate_x_axis = loadRect.Top - memberRect.Top;  // Prut zacina pred plochou

                // Opacny smer osi pruta x voci osi y load surface
                if (pStartLCS.Y > pEndLCS.Y)
                {
                    dMemberLoadStartCoordinate_x_axis = m.FLength - intersection.Height;

                    if (loadRect.Height >= pStartLCS.Y)
                        dMemberLoadStartCoordinate_x_axis = 0;
                }

                dIntersectionLengthInMember_x_axis = intersection.Height; // Length of applied load
                dIntersectionLengthInMember_yz_axis = intersection.Width; // Tributary width
            }

            foreach (MemberLoadParameters loadparam in listMemberLoadParams)
            {
                if (intersection == Rect.Empty)
                {
                    return;
                }
                else if (MathF.d_equal(dIntersectionLengthInMember_x_axis, m.FLength)) // Intersection in x direction of member is same as member length - generate uniform load per whole member length
                {
                    float fq = loadparam.fMemberLoadValueSign * Math.Abs(l.fValue * loadparam.fSurfaceLoadValueFactor) * (float)dIntersectionLengthInMember_yz_axis; // Load Value
                    lc.MemberLoadsList.Add(new CMLoad_21(iLoadID, fq, m, EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, loadparam.eMemberLoadDirection, true, 0));
                    iLoadID += 1;
                }
                else
                {
                    //nie som si isty,ci to je spravne
                    float fq = (float)(loadparam.fMemberLoadValueSign * Math.Abs(l.fValue * loadparam.fSurfaceLoadValueFactor) * dIntersectionLengthInMember_yz_axis); // Load Value
                    float faA = (float)dMemberLoadStartCoordinate_x_axis; // Load start point on member (absolute coordinate x)
                    float fs = (float)dIntersectionLengthInMember_x_axis; // Load segment length on member (absolute coordinate x)

                    lc.MemberLoadsList.Add(new CMLoad_24(iLoadID, fq, faA, fs, m, EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, loadparam.eMemberLoadDirection, true, 0));
                    iLoadID += 1;
                }
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
