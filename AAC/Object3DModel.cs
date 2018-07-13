using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses;
using BaseClasses.GraphObj;
using _3DTools;
using MATH;

namespace AAC
{
    public class Object3DModel
    {

        public Object3DModel()
        { }

        public Model3DGroup GetMemberModel(EGCS eGCS, bool bIsTransparent, Point3D NodeStart, Point3D NodeEnd, CCrSc Cross_Section, SolidColorBrush brushFrontSide, SolidColorBrush brushShell, SolidColorBrush brushBackSide, MaterialGroup qOuterMaterial)
        {
            Model3DGroup MObject3DModel = new Model3DGroup(); // Whole member

            GeometryModel3D modelFrontSide = new GeometryModel3D();
            GeometryModel3D modelShell = new GeometryModel3D();
            GeometryModel3D modelBackSide = new GeometryModel3D();

            // Note: Same cross-section in the start and end of bar
            getG_M_3D_Member(eGCS, NodeStart, NodeEnd, Cross_Section, Cross_Section, brushFrontSide, brushShell, brushBackSide, out modelFrontSide, out modelShell, out modelBackSide);

            if (bIsTransparent && qOuterMaterial != null)
            {
                modelFrontSide.Material = qOuterMaterial;  // Set transparent material
                modelShell.Material = qOuterMaterial;
                modelBackSide.Material = qOuterMaterial;
            }

            MObject3DModel.Children.Add((Model3D)modelFrontSide);
            MObject3DModel.Children.Add((Model3D)modelShell);
            MObject3DModel.Children.Add((Model3D)modelBackSide);

            return MObject3DModel;
        }

        public void getG_M_3D_Member(EGCS eGCS, Point3D NodeStart, Point3D NodeEnd, CCrSc CrScStart, CCrSc CrScEnd, SolidColorBrush brushFrontSide, SolidColorBrush brushShell, SolidColorBrush brushBackSide, out GeometryModel3D modelFrontSide, out GeometryModel3D modelShell, out GeometryModel3D modelBackSide)
        {
            // We need to transform CNode to Point3D
            Point3D mpA = new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z); // Start point - class Point3D
            Point3D mpB = new Point3D(NodeEnd.X, NodeEnd.Y, NodeEnd.Z); // End point - class Point3D
            // Angle of rotation about local x-axis
            double DTheta_x = 0; // Temporary

            modelFrontSide = new GeometryModel3D();
            modelShell = new GeometryModel3D();
            modelBackSide = new GeometryModel3D();

            MeshGeometry3D meshFrontSide = new MeshGeometry3D();
            MeshGeometry3D meshShell = new MeshGeometry3D();
            MeshGeometry3D meshBackSide = new MeshGeometry3D();

            getMeshMemberGeometry3DFromCrSc_1(eGCS, CrScStart, CrScEnd, mpA, mpB, DTheta_x, out meshFrontSide, out meshShell, out meshBackSide);

            modelFrontSide.Geometry = meshFrontSide;
            modelShell.Geometry = meshShell;
            modelBackSide.Geometry = meshBackSide;

            // Set MemberModel parts Material
            modelFrontSide.Material = new DiffuseMaterial(brushFrontSide);
            modelShell.Material = new DiffuseMaterial(brushShell);
            modelBackSide.Material = new DiffuseMaterial(brushBackSide);
        }

        private void getMeshMemberGeometry3DFromCrSc_1(EGCS eGCS, CCrSc obj_CrScA, CCrSc obj_CrScB, Point3D mpA, Point3D mpB, double dTheta_x, out MeshGeometry3D meshFrontSide, out MeshGeometry3D meshShell, out MeshGeometry3D meshBackSide)
        {
            meshFrontSide = new MeshGeometry3D();
            meshShell = new MeshGeometry3D();
            meshBackSide = new MeshGeometry3D();

            meshFrontSide.Positions = new Point3DCollection();
            meshShell.Positions = new Point3DCollection();
            meshBackSide.Positions = new Point3DCollection();

            // Main Nodes of Member
            Point3D pA = mpA;
            Point3D pB = mpB;

            // Priemet do osi GCS - rozdiel suradnic v GCS
            double dDelta_X = pB.X - pA.X;
            double dDelta_Y = pB.Y - pA.Y;
            double dDelta_Z = pB.Z - pA.Z;

            // Realna dlzka prvku // Length of member - straigth segment of member
            // Prečo je záporná ???
            // double m_dLength = -Math.Sqrt(Math.Pow(m_dDelta_X, 2) + Math.Pow(m_dDelta_Y, 2) + Math.Pow(m_dDelta_Z, 2));
            double m_dLength = Math.Sqrt(Math.Pow(dDelta_X, 2) + Math.Pow(dDelta_Y, 2) + Math.Pow(dDelta_Z, 2));

            // Number of Points per section
            int iNoCrScPoints2D;
            // Points 2D Coordinate Array
            if (obj_CrScA.IsShapeSolid) // Solid I,U,Z,HL,L, ..............
            {
                iNoCrScPoints2D = obj_CrScA.ITotNoPoints; // Depends on Section Type
                // Fill Mesh Positions for Start and End Section of Element - Defines Edge Points of Element

                if (obj_CrScA.CrScPointsOut != null) // Check that data are available
                {
                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - start, Y, Z
                        meshFrontSide.Positions.Add(new Point3D(0, obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1]));
                        meshShell.Positions.Add(new Point3D(0, obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1]));
                    }

                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            meshBackSide.Positions.Add(new Point3D(m_dLength, obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1])); // Constant size member
                            meshShell.Positions.Add(new Point3D(m_dLength, obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1]));
                        }
                        else
                        {
                            meshBackSide.Positions.Add(new Point3D(m_dLength, obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1])); // Tapered member
                            meshShell.Positions.Add(new Point3D(m_dLength, obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1]));
                        }
                    }
                }
                else
                {
                    // Exception
                }
            }

            // Transform coordinates
            TransformMember_LCStoGCS(eGCS, pA, pB, dDelta_X, dDelta_Y, dDelta_Z, dTheta_x, meshFrontSide.Positions);
            TransformMember_LCStoGCS(eGCS, pA, pB, dDelta_X, dDelta_Y, dDelta_Z, dTheta_x, meshShell.Positions);
            TransformMember_LCStoGCS(eGCS, pA, pB, dDelta_X, dDelta_Y, dDelta_Z, dTheta_x, meshBackSide.Positions);

            // Mesh Triangles - various cross-sections shapes defined
            //mesh.TriangleIndices = obj_CrScA.TriangleIndices;
            meshFrontSide.TriangleIndices = obj_CrScA.TriangleIndicesFrontSide;
            meshShell.TriangleIndices = obj_CrScA.TriangleIndicesShell;
            meshBackSide.TriangleIndices = obj_CrScA.TriangleIndicesBackSide;

            // Change mesh triangle indices
            // Change orientation of normals
            bool bIndicesCW = true; // Clockwise or counter-clockwise system

            if (bIndicesCW)
            {
                ChangeIndices(meshFrontSide.TriangleIndices);
                ChangeIndices(meshShell.TriangleIndices);
                ChangeIndices(meshBackSide.TriangleIndices);
            }
        }

        private void ChangeIndices(Int32Collection TriangleIndices)
        {
            int iSecond = 1;
            int iThird = 2;

            int iTIcount = TriangleIndices.Count;
            for (int i = 0; i < iTIcount / 3; i++)
            {
                int iTI_2 = TriangleIndices[iSecond];
                int iTI_3 = TriangleIndices[iThird];

                TriangleIndices[iThird] = iTI_2;
                TriangleIndices[iSecond] = iTI_3;

                iSecond += 3;
                iThird += 3;
            }
        }

        public Point3DCollection TransformMember_LCStoGCS(EGCS eGCS, Point3D pA, Point3D pB, double dDeltaX, double dDeltaY, double dDeltaZ, double dTheta_x, Point3DCollection pointsCollection)
        {
            // Returns transformed coordinates of member nodes

            // Angles
            double dAlphaX = 0, dBetaY = 0, dGammaZ = 0;

            // Priemet do rovin GCS - dlzka priemetu do roviny
            double dLength_XY = 0,
                   dLength_YZ = 0,
                   dLength_XZ = 0;

            if (!MathF.d_equal(dDeltaX, 0.0) || !MathF.d_equal(dDeltaY, 0.0))
                dLength_XY = Math.Sqrt(Math.Pow(dDeltaX, 2) + Math.Pow(dDeltaY, 2));

            if (!MathF.d_equal(dDeltaY, 0.0) || !MathF.d_equal(dDeltaZ, 0.0))
                dLength_YZ = Math.Sqrt(Math.Pow(dDeltaY, 2) + Math.Pow(dDeltaZ, 2));

            if (!MathF.d_equal(dDeltaX, 0.0) || !MathF.d_equal(dDeltaZ, 0.0))
                dLength_XZ = Math.Sqrt(Math.Pow(dDeltaX, 2) + Math.Pow(dDeltaZ, 2));

            // Uhly pootocenia LCS okolo osi GCS
            // Angles
            dAlphaX = Geom2D.GetAlpha2D_CW(dDeltaY, dDeltaZ);
            dBetaY = Geom2D.GetAlpha2D_CW_2(dDeltaX, dDeltaZ); // !!! Pre pootocenie okolo Y su pouzite ine kvadranty !!!
            dGammaZ = Geom2D.GetAlpha2D_CW(dDeltaX, dDeltaY);

            // Auxialiary angles for members graphics
            double dBetaY_aux = Geom2D.GetAlpha2D_CW_3(dDeltaX, dDeltaZ, Math.Sqrt(Math.Pow(dLength_XY, 2) + Math.Pow(dDeltaZ, 2)));
            double dGammaZ_aux = dGammaZ;
            if (Math.PI / 2 < dBetaY && dBetaY < 1.5 * Math.PI)
            {
                if (dGammaZ < Math.PI)
                    dGammaZ_aux = dGammaZ + Math.PI;
                else
                    dGammaZ_aux = dGammaZ - Math.PI;
            }

            for (int i = 0; i < pointsCollection.Count; i++)
            {
                pointsCollection[i] = RotateTranslatePoint3D(eGCS, pA, pointsCollection[i], dBetaY_aux, dGammaZ_aux, dTheta_x, dDeltaX, dDeltaY, dDeltaZ);
            }

            return pointsCollection;
        }

        public Point3D RotateTranslatePoint3D(EGCS eGCS, Point3D pA, Point3D p, double betaY, double gamaZ, double theta_x, double dDeltaX, double dDeltaY, double dDeltaZ)
        {
            Point3D p3Drotated = new Point3D();

            //http://sk.wikipedia.org/wiki/Trojrozmern%C3%A1_projekcia#D.C3.A1ta_nevyhnutn.C3.A9_pre_projekciu

            // Left Handed
            // * Where (alphaX) represents the rotation about the X axis, (betaY) represents the rotation about the Y axis, and (gamaZ) represents the rotation about the Z axis
            /*
            X Rotation *

            1     0                0                  0
            0     cos (alphaX)    -sin (alphaX)       0
            0     sin (alphaX)     cos (alphaX)       0
            0     0                0                  1
            */

            /*
            Y Rotation *

            cos (betaY)    0     sin (betaY)    0
            0              1     0              0
            -sin (betaY)   0     cos (betaY)    0
            0              0     0              1
            */

            /*
            Z Rotation *

            cos (gamaZ)     -sin (gamaZ)     0      0
            sin (gamaZ)      cos (gamaZ)     0      0
            0                 0              1      0
            0                 0              0      1
            */

            ////////////////////////////////////////////////////////////////////////////////////////////
            // Right Handed

            /*
            X Rotation *

            1           0             0               0
            0     cos (alphaX)     sin (alphaX)       0
            0    -sin (alphaX)     cos (alphaX)       0
            0           0             0               1
            */

            /*
            Y Rotation *

            cos (betaY)    0    -sin (betaY)    0
            0              1     0              0
            sin (betaY)    0     cos (betaY)    0
            0              0     0              1
            */

            /*
            Z Rotation *

             cos (gamaZ)     sin (gamaZ)      0      0
            -sin (gamaZ)     cos (gamaZ)      0      0
             0                 0              1      0
             0                 0              0      1
            */

            Point3D pTemp1 = new Point3D();
            Point3D pTemp2 = new Point3D();

            if (eGCS == EGCS.eGCSLeftHanded)
            {
                // Left-handed

                // Rotate about Y-axis
                pTemp1.X = (Math.Cos(betaY) * p.X) + (Math.Sin(betaY) * p.Z);
                pTemp1.Y = p.Y;
                pTemp1.Z = (-Math.Sin(betaY) * p.X) + (Math.Cos(betaY) * p.Z);

                // Rotate about Z-axis
                pTemp2.X = (Math.Cos(gamaZ) * pTemp1.X) - (Math.Sin(gamaZ) * pTemp1.Y);
                pTemp2.Y = (Math.Sin(gamaZ) * pTemp1.X) + (Math.Cos(gamaZ) * pTemp1.Y);
                pTemp2.Z = pTemp1.Z;

                // Translate
                pTemp1.X = pA.X + pTemp2.X;
                pTemp1.Y = pA.Y + pTemp2.Y;
                pTemp1.Z = pA.Z + pTemp2.Z;

                // Set output point data
                p3Drotated.X = pTemp1.X;
                p3Drotated.Y = pTemp1.Y;
                p3Drotated.Z = pTemp1.Z;

                // Rotate about local x-axis
                if (!MathF.d_equal(theta_x, 0.0))
                {
                    double c = Math.Cos(theta_x);
                    double s = Math.Sin(theta_x);
                    double t = 1 - c;

                    p3Drotated.X = (t * Math.Pow(dDeltaX, 2) + c) * pTemp1.X + (t * dDeltaX * dDeltaY - s * dDeltaZ) * pTemp1.Y + (t * dDeltaX * dDeltaZ + s * dDeltaY) * pTemp1.Z;
                    p3Drotated.Y = (t * dDeltaX * dDeltaY + s * dDeltaZ) * pTemp1.X + (t * Math.Pow(dDeltaY, 2) + c) * pTemp1.Y + (t * dDeltaY * dDeltaZ - s * dDeltaX) * pTemp1.Z;
                    p3Drotated.Z = (t * dDeltaX * dDeltaZ - s * dDeltaY) * pTemp1.X + (t * dDeltaY * dDeltaZ + s * dDeltaX) * pTemp1.Y + (t * Math.Pow(dDeltaZ, 2) + c) * pTemp1.Z;
                }
            }
            else
            {
                // Right-handed

                // Rotate about Y-axis
                pTemp1.X = (Math.Cos(betaY) * p.X) - (Math.Sin(betaY) * p.Z);
                pTemp1.Y = p.Y;
                pTemp1.Z = (Math.Sin(betaY) * p.X) + (Math.Cos(betaY) * p.Z);

                // Rotate about Z-axis
                pTemp2.X = (Math.Cos(gamaZ) * pTemp1.X) + (Math.Sin(gamaZ) * pTemp1.Y);
                pTemp2.Y = (-Math.Sin(gamaZ) * pTemp1.X) + (Math.Cos(gamaZ) * pTemp1.Y);
                pTemp2.Z = pTemp1.Z;

                // Translate
                pTemp1.X = pA.X + pTemp2.X;
                pTemp1.Y = pA.Y + pTemp2.Y;
                pTemp1.Z = pA.Z + pTemp2.Z;

                // Set output point data
                p3Drotated.X = pTemp1.X;
                p3Drotated.Y = pTemp1.Y;
                p3Drotated.Z = pTemp1.Z;

                // Rotate about local x-axis
                if (!MathF.d_equal(theta_x, 0.0))
                {
                    double c = Math.Cos(theta_x);
                    double s = Math.Sin(theta_x);
                    double t = 1 - c;

                    p3Drotated.X = (t * Math.Pow(dDeltaX, 2) + c) * pTemp1.X + (t * dDeltaX * dDeltaY + s * dDeltaZ) * pTemp1.Y + (t * dDeltaX * dDeltaZ - s * dDeltaY) * pTemp1.Z;
                    p3Drotated.Y = (t * dDeltaX * dDeltaY - s * dDeltaZ) * pTemp1.X + (t * Math.Pow(dDeltaY, 2) + c) * pTemp1.Y + (t * dDeltaY * dDeltaZ + s * dDeltaX) * pTemp1.Z;
                    p3Drotated.Z = (t * dDeltaX * dDeltaZ + s * dDeltaY) * pTemp1.X + (t * dDeltaY * dDeltaZ - s * dDeltaX) * pTemp1.Y + (t * Math.Pow(dDeltaZ, 2) + c) * pTemp1.Z;
                }

            }

            return p3Drotated;
        }


        // Unused functions
        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateRectangle(
              Point3D point1, Point3D point2,
              Point3D point3, Point3D point4,
              DiffuseMaterial DiffMat)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);
            mesh.Positions.Add(point4);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            return new GeometryModel3D(mesh, DiffMat);
        }

        //--------------------------------------------------------------------------------------------
        public GeometryModel3D CreateTriangle(
              Point3D point1, Point3D point2,
              Point3D point3,
              DiffuseMaterial DiffMat)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);


            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            return new GeometryModel3D(mesh, DiffMat);
        }
    }
}
