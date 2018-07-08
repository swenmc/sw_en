using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using MATH;
using _3DTools;

namespace BaseClasses
{
    [Serializable]
    public class CMember : CEntity3D
    {
        private CNode    m_NodeStart;

        public CNode NodeStart
        {
          get { return m_NodeStart; }
          set { m_NodeStart = value; }
        }
        private CNode    m_NodeEnd;

        public CNode NodeEnd
        {
          get { return m_NodeEnd; }
          set { m_NodeEnd = value; }
        }

        private Point3D m_PointStart;

        public Point3D PointStart
        {
            get { return m_PointStart; }
            set { m_PointStart = value; }
        }
        private Point3D m_PointEnd;

        public Point3D PointEnd
        {
            get { return m_PointEnd; }
            set { m_PointEnd = value; }
        }

        private CNRelease m_cnRelease1;

        public CNRelease CnRelease1
        {
          get { return m_cnRelease1; }
          set { m_cnRelease1 = value; }
        }
        private CNRelease m_cnRelease2;

        public CNRelease CnRelease2
        {
          get { return m_cnRelease2; }
          set { m_cnRelease2 = value; }
        }

        private CCrSc m_CrScStart;

        public CCrSc CrScStart
        {
            get { return m_CrScStart; }
            set { m_CrScStart = value; }
        }

        private CCrSc m_CrScEnd;

        public CCrSc CrScEnd
        {
            get { return m_CrScEnd; }
            set { m_CrScEnd = value; }
        }

        public EMemberType_FormSteel eMemberType_FS;

        // Priemet do osi GCS - rozdiel suradnic v GCS
        private double dDelta_X;

        public double Delta_X
        {
            get { return dDelta_X; }
            set { dDelta_X = value; }
        }

        private double dDelta_Y;

        public double Delta_Y
        {
            get { return dDelta_Y; }
            set { dDelta_Y = value; }
        }

        private double dDelta_Z;

        public double Delta_Z
        {
            get { return dDelta_Z; }
            set { dDelta_Z = value; }
        }

        private float m_fLength;

        public float FLength
        {
          get { return m_fLength; }
          set { m_fLength = value; }
        }

        private float m_fLength_real;

        public float FLength_real
        {
            get { return m_fLength_real; }
            set { m_fLength_real = value; }
        }

        private float m_fAlignment_Start; // Positive value means elongation of beam

        public float FAlignment_Start
        {
            get { return m_fAlignment_Start; }
            set { m_fAlignment_Start = value; }
        }

        private float m_fAlignment_End; // Positive value means elongation of beam

        public float FAlignment_End
        {
            get { return m_fAlignment_End; }
            set { m_fAlignment_End = value; }
        }

        public double m_dTheta_x;

        public double DTheta_x
        {
            get { return m_dTheta_x; }
            set { m_dTheta_x = value; }
        }



        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        // Constructor 1
        public CMember()
        {
            m_NodeStart = new CNode();
            m_NodeEnd = new CNode();
            m_cnRelease1 = null;
            m_cnRelease2 = null;
        }
        // Constructor 2
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            int fTime)
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            FTime = fTime;

            Fill_Basic();
        }

        // Constructor 3
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            bool haveRelease1,
            bool haveRelease2,
            int fTime)
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            if (haveRelease1)
                m_cnRelease1 = new CNRelease(iNode1);
            if (haveRelease2)
                m_cnRelease2 = new CNRelease(iNode2);
            FTime = fTime;

            Fill_Basic();
        }
        // Constructor 4
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            CCrSc objCrSc1,
            int fTime
            )
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            m_CrScStart = objCrSc1;
            FTime = fTime;

            Fill_Basic();
        }

        // Constructor 5
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            CCrSc objCrSc1,
            float fAligment1,
            float fAligment2,
            float fTheta_x = 0,
            int fTime = 0
            )
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            m_CrScStart = objCrSc1;
            FAlignment_Start = fAligment1;
            FAlignment_End = fAligment2;
            DTheta_x = fTheta_x;
            FTime = fTime;

            Fill_Basic();
        }

        // Constructor 6
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            CCrSc objCrSc1,
            EMemberType_FormSteel eMemberType,
            float fAligment1,
            float fAligment2,
            float fTheta_x = 0,
            int fTime = 0
            )
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            m_CrScStart = objCrSc1;
            eMemberType_FS = eMemberType;
            FAlignment_Start = fAligment1;
            FAlignment_End = fAligment2;
            DTheta_x = fTheta_x;
            FTime = fTime;

            Fill_Basic();
        }

        // Constructor 7
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            CCrSc objCrSc1,
            CCrSc objCrSc2,
            float fAligment1,
            float fAligment2,
            int fTime
            )
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            m_CrScStart = objCrSc1;
            m_CrScEnd = objCrSc2;
            FAlignment_Start = fAligment1;
            FAlignment_End = fAligment2;
            FTime = fTime;

            Fill_Basic();
        }

        // Constructor 8
        public CMember(
            int iLine_ID,
            CNode iNode1,
            CNode iNode2,
            CCrSc objCrSc1,
            CCrSc objCrSc2,
            EMemberType_FormSteel eMemberType,
            float fAligment1,
            float fAligment2,
            int fTime
            )
        {
            ID = iLine_ID;
            m_NodeStart = iNode1;
            m_NodeEnd = iNode2;
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            m_CrScStart = objCrSc1;
            m_CrScEnd = objCrSc2;
            eMemberType_FS = eMemberType;
            FAlignment_Start = fAligment1;
            FAlignment_End = fAligment2;
            FTime = fTime;

            Fill_Basic();
        }


        //Fill basic data
        public void Fill_Basic()
        {
            // Temporary !!!!!!!!!!!!!!!!!!!!!! Member Length for 3D
            // Theroretical length of member (between definition nodes)

            // Priemet do osi GCS - rozdiel suradnic v GCS

            FLength = (float)Math.Sqrt((float)Math.Pow(m_NodeEnd.X - m_NodeStart.X, 2f) + (float)Math.Pow(m_NodeEnd.Y - m_NodeStart.Y, 2f) + (float)Math.Pow(m_NodeEnd.Z - m_NodeStart.Z, 2f));
            FLength_real = FAlignment_Start + FLength + FAlignment_End; // Real length of member

            Delta_X = m_NodeEnd.X - m_NodeStart.X;
            Delta_Y = m_NodeEnd.Y - m_NodeStart.Y;
            Delta_Z = m_NodeEnd.Z - m_NodeStart.Z;

            SetStartPoint3DCoord();
            SetEndPoint3DCoord();

            // Add created member to the list of members in cross-section object
            if (m_CrScStart != null)
                m_CrScStart.AssignedMembersList.Add(this);

            if (m_CrScEnd != null)
                m_CrScEnd.AssignedMembersList.Add(this);
        }

        public void SetStartPoint3DCoord()
        {
            m_PointStart.X = m_NodeStart.X;
            m_PointStart.Y = m_NodeStart.Y;
            m_PointStart.Z = m_NodeStart.Z;

            Point3DCollection temp = new Point3DCollection();
            Point3D tempPoint = new Point3D(-m_fAlignment_Start,0,0);
            temp.Add(tempPoint);

            TransformMember_LCStoGCS(EGCS.eGCSLeftHanded, m_PointStart, Delta_X, Delta_Y, Delta_Z, m_dTheta_x, temp);

            m_PointStart = temp[0];
        }
        public void SetEndPoint3DCoord()
        {
            m_PointEnd.X = m_NodeEnd.X;
            m_PointEnd.Y = m_NodeEnd.Y;
            m_PointEnd.Z = m_NodeEnd.Z;

            Point3DCollection temp = new Point3DCollection();
            Point3D tempPoint = new Point3D(m_fAlignment_End, 0, 0);
            temp.Add(tempPoint);

            TransformMember_LCStoGCS(EGCS.eGCSLeftHanded, m_PointEnd, Delta_X, Delta_Y, Delta_Z, m_dTheta_x, temp);

            m_PointEnd = temp[0];
        }

        public Model3DGroup getM_3D_G_Member(EGCS eGCS, SolidColorBrush brushFrontSide, SolidColorBrush brushShell, SolidColorBrush brushBackSide)
        {
            Model3DGroup MObject3DModel = new Model3DGroup(); // Whole member

            GeometryModel3D modelFrontSide = new GeometryModel3D();
            GeometryModel3D modelShell = new GeometryModel3D();
            GeometryModel3D modelBackSide = new GeometryModel3D();

            getG_M_3D_Member(eGCS, brushFrontSide, brushShell, brushBackSide, out modelFrontSide, out  modelShell, out modelBackSide);

            MObject3DModel.Children.Add((Model3D)modelFrontSide);
            MObject3DModel.Children.Add((Model3D)modelShell);
            MObject3DModel.Children.Add((Model3D)modelBackSide);

            return MObject3DModel;
        }

        public GeometryModel3D getG_M_3D_Member(EGCS eGCS, SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            MeshGeometry3D mesh = getMeshMemberGeometry3DFromCrSc(eGCS, CrScStart, CrScEnd, DTheta_x); // Mesh one member

            model.Geometry = mesh;

            model.Material = new DiffuseMaterial(brush);  // Set MemberModel Material

            return model;
        }

        public void getG_M_3D_Member(EGCS eGCS, SolidColorBrush brushFrontSide, SolidColorBrush brushShell, SolidColorBrush brushBackSide, 
            out GeometryModel3D modelFrontSide, out GeometryModel3D modelShell, out GeometryModel3D modelBackSide)
        {
            modelFrontSide = new GeometryModel3D();
            modelShell = new GeometryModel3D();
            modelBackSide = new GeometryModel3D();

            MeshGeometry3D meshFrontSide = new MeshGeometry3D();
            MeshGeometry3D meshShell= new MeshGeometry3D();
            MeshGeometry3D meshBackSide = new MeshGeometry3D();

            getMeshMemberGeometry3DFromCrSc_1(eGCS, CrScStart, CrScEnd, DTheta_x, out meshFrontSide, out meshShell, out meshBackSide);

            modelFrontSide.Geometry = meshFrontSide;
            modelShell.Geometry = meshShell;
            modelBackSide.Geometry = meshBackSide;

            // Set MemberModel parts Material
            modelFrontSide.Material = new DiffuseMaterial(brushFrontSide);
            modelShell.Material = new DiffuseMaterial(brushShell);
            modelBackSide.Material = new DiffuseMaterial(brushBackSide);
        }

        private MeshGeometry3D getMeshMemberGeometry3DFromCrSc(EGCS eGCS, CCrSc obj_CrScA, CCrSc obj_CrScB, double dTheta_x)
        {
            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();

            // Realna dlzka prvku // Length of member - straigth segment of member
            // Prečo je záporná ???
            // double m_dLength = -Math.Sqrt(Math.Pow(m_dDelta_X, 2) + Math.Pow(m_dDelta_Y, 2) + Math.Pow(m_dDelta_Z, 2));
            //double m_dLength = Math.Sqrt(Math.Pow(Delta_X, 2) + Math.Pow(Delta_Y, 2) + Math.Pow(Delta_Z, 2));

            // Number of Points per section
            short iNoCrScPoints2D;
            float fy, fz;
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
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                        mesh.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }
                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);

                            mesh.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                        }
                        else
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);

                            mesh.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                        }
                    }
                }
                else
                {
                    // Exception
                }
            }
            else if (obj_CrScA.INoPointsOut == obj_CrScA.INoPointsIn) // Closed cross-section with same number out ouside and insdide definiton points
            {
                // Tubes , Polygonal Hollow Sections
                iNoCrScPoints2D = (short)(2 * obj_CrScA.INoPointsOut); // Twice number of one surface

                // Tube, regular hollow sections
                // TU

                // Start
                if (obj_CrScA.CrScPointsOut != null) // Check that data are available
                {
                    // OutSide Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsOut; j++)
                    {
                        // X - start, Y, Z
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);

                        mesh.Positions.Add(new Point3D(-FAlignment_Start,fy, fz));
                    }
                }
                else
                {
                    // Exception
                }

                if (obj_CrScA.CrScPointsIn != null) // Check that data are available
                {
                    // Inside Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsIn; j++)
                    {
                        // X - start, Y, Z
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);

                        mesh.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }
                }
                else
                {
                    // Exception
                }

                // End
                if (obj_CrScA.CrScPointsOut != null) // Check that data are available
                {
                    // OutSide Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsOut; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);

                            mesh.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                        }
                        else
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);

                            mesh.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                        }
                    }
                }
                else
                {
                    // Exception
                }

                if (obj_CrScA.CrScPointsIn != null) // Check that data are available
                {
                    // Inside Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsIn; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);

                            mesh.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                        }
                        else
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScB.CrScPointsIn[j, 0], obj_CrScB.CrScPointsIn[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScB.CrScPointsIn[j, 0], obj_CrScB.CrScPointsIn[j, 1], dTheta_x);

                            mesh.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                        }
                    }
                }
                else
                {
                    // Exception
                }
            }
            else
            {
                // Exception
                // Closed cross-section with different number out ouside and insdide definiton points

                iNoCrScPoints2D = 0; // Temp
            }

            // Dislay data in the output window

            string sOutput = "Before transformation \n\n"; // create temporary string

            for (int i = 0; i < 2 * iNoCrScPoints2D; i++) // for all mesh positions (start and end of member, number of edge points of whole member = 2 * number in one section)
            {
                Point3D p3D = mesh.Positions[i]; // Get mesh element/item (returns Point3D)

                sOutput += "Node ID: " + i.ToString();
                sOutput += "\t"; // New Tab between columns
                sOutput += p3D.X.ToString("0.0000");
                sOutput += "\t"; // New Tab between columns
                sOutput += p3D.Y.ToString("0.0000");
                sOutput += "\t"; // New Tab between columns
                sOutput += p3D.Z.ToString("0.0000");
                sOutput += "\t"; // New Tab between columns

                sOutput += "\n"; // New row
            }

            if (BIsDebugging)
                System.Console.Write(sOutput); // Write in console window

            // Transform coordinates
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, m_dTheta_x, mesh.Positions);

            // Mesh Triangles - various cross-sections shapes defined
            mesh.TriangleIndices = obj_CrScA.TriangleIndices;

            // Dislay data in the output window

            sOutput = null;
            sOutput = "After transformation \n\n"; // create temporary string

            for (int i = 0; i < 2 * iNoCrScPoints2D; i++) // for all mesh positions (start and end of member, number of edge points of whole member = 2 * number in one section)
            {
                Point3D p3D = mesh.Positions[i]; // Get mesh element/item (returns Point3D)

                sOutput += "Node ID: " + i.ToString();
                sOutput += "\t"; // New Tab between columns
                sOutput += p3D.X.ToString("0.0000");
                sOutput += "\t"; // New Tab between columns
                sOutput += p3D.Y.ToString("0.0000");
                sOutput += "\t"; // New Tab between columns
                sOutput += p3D.Z.ToString("0.0000");
                sOutput += "\t"; // New Tab between columns

                sOutput += "\n"; // New row
            }

            if (BIsDebugging)
                System.Console.Write(sOutput); // Write in console window

            // Change mesh triangle indices
            // Change orientation of normals

            //if (eGCS == EGCS.eGCSLeftHanded)
            //{
            bool bIndicesCW = true; // Clockwise or counter-clockwise system

            if(bIndicesCW)
              ChangeIndices(mesh.TriangleIndices);
            //}
            return mesh;
        }

        private void ChangeIndices(Int32Collection TriangleIndices)
        {
            if (TriangleIndices != null && TriangleIndices.Count > 0)
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
        }

        private void getMeshMemberGeometry3DFromCrSc_1(EGCS eGCS, CCrSc obj_CrScA, CCrSc obj_CrScB, double dTheta_x, out MeshGeometry3D meshFrontSide, out MeshGeometry3D meshShell, out MeshGeometry3D meshBackSide)
        {
            // Separate mesh for front, back and shell surfaces of member

            meshFrontSide = new MeshGeometry3D();
            meshShell = new MeshGeometry3D();
            meshBackSide = new MeshGeometry3D();

            meshFrontSide.Positions = new Point3DCollection();
            meshShell.Positions = new Point3DCollection();
            meshBackSide.Positions = new Point3DCollection();

            // Number of Points per section
            short iNoCrScPoints2D;
            float fy, fz;
            // Points 2D Coordinate Array
            if (obj_CrScA.IsShapeSolid) // Solid I,U,Z,HL,L, ..............
            {
                iNoCrScPoints2D = obj_CrScA.ITotNoPoints; // Depends on Section Type
                // Fill Mesh Positions for Start and End Section of Element - Defines Edge Points of Element

                if (obj_CrScA.CrScPointsOut != null) // Check that data are available
                {
                    // Rotate local y and z coordinates

                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - start, Y, Z
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);

                        meshFrontSide.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                        meshShell.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }

                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);

                            meshBackSide.Positions.Add(new Point3D(FLength + FAlignment_End,fy, fz)); // Constant size member
                            meshShell.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                        else
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);

                            meshBackSide.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                            meshShell.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                    }
                }
                else
                {
                    // Exception
                }
            }
            else if (obj_CrScA.INoPointsOut == obj_CrScA.INoPointsIn) // Closed cross-section with same number out ouside and insdide definiton points
            {
                // Tubes , Polygonal Hollow Sections
                iNoCrScPoints2D = (short)(2 * obj_CrScA.INoPointsOut); // Twice number of one surface

                // Tube, regular hollow sections
                // TU

                // Start Point
                if (obj_CrScA.CrScPointsOut != null) // Check that data are available
                {
                    // OutSide Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsOut; j++)
                    {
                        // X - start, Y, Z
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);

                        meshFrontSide.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                        meshShell.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }
                }
                else
                {
                    // Exception
                }

                if (obj_CrScA.CrScPointsIn != null) // Check that data are available
                {
                    // Inside Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsIn; j++)
                    {
                        // X - start, Y, Z
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);

                        meshFrontSide.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                        meshShell.Positions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }
                }
                else
                {
                    // Exception
                }

                // End Point
                if (obj_CrScA.CrScPointsOut != null) // Check that data are available
                {
                    // OutSide Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsOut; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsOut[j, 0], obj_CrScA.CrScPointsOut[j, 1], dTheta_x);

                            meshBackSide.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                            meshShell.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                        else
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScB.CrScPointsOut[j, 0], obj_CrScB.CrScPointsOut[j, 1], dTheta_x);

                            meshBackSide.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                            meshShell.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                    }
                }
                else
                {
                    // Exception
                }

                if (obj_CrScA.CrScPointsIn != null) // Check that data are available
                {
                    // Inside Radius Points
                    for (int j = 0; j < obj_CrScA.INoPointsIn; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScA.CrScPointsIn[j, 0], obj_CrScA.CrScPointsIn[j, 1], dTheta_x);

                            meshBackSide.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                            meshShell.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                        else
                        {
                            // Rotate about local x-axis
                            fy = (float)Geom2D.GetRotatedPosition_x_CCW(obj_CrScB.CrScPointsIn[j, 0], obj_CrScB.CrScPointsIn[j, 1], dTheta_x);
                            fz = (float)Geom2D.GetRotatedPosition_y_CCW(obj_CrScB.CrScPointsIn[j, 0], obj_CrScB.CrScPointsIn[j, 1], dTheta_x);

                            meshBackSide.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                            meshShell.Positions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                    }
                }
                else
                {
                    // Exception
                }
            }
            else
            {
                // Exception
                // Closed cross-section with different number out ouside and insdide definiton points

                iNoCrScPoints2D = 0; // Temp
            }

            // Transform coordinates
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y,NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, m_dTheta_x, meshFrontSide.Positions); // Posun voci povodnemu definicnemu uzlu pruta
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, m_dTheta_x, meshShell.Positions);
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, m_dTheta_x, meshBackSide.Positions);

            // Mesh Triangles - various cross-sections shapes defined
            //mesh.TriangleIndices = obj_CrScA.TriangleIndices;
            meshFrontSide.TriangleIndices = obj_CrScA.TriangleIndicesFrontSide;
            meshShell.TriangleIndices = obj_CrScA.TriangleIndicesShell;
            meshBackSide.TriangleIndices = obj_CrScA.TriangleIndicesBackSide;

            // Change mesh triangle indices
            // Change orientation of normals
            bool bIndicesCW = false; // Clockwise or counter-clockwise system

            if(bIndicesCW)
            {
                ChangeIndices(meshFrontSide.TriangleIndices);
                ChangeIndices(meshShell.TriangleIndices);
                ChangeIndices(meshBackSide.TriangleIndices);
            }
        }

        public Point3DCollection TransformMember_LCStoGCS(EGCS eGCS, Point3D pA, double dDeltaX, double dDeltaY, double dDeltaZ, double dTheta_x, Point3DCollection pointsCollection)
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

            if (BIsDebugging)
            {
                // Temporary console output
                System.Console.Write("\n" + "Lengths - projection of element into global coordinate system:\n");
                System.Console.Write("Length - global X-axis:\t" + dDeltaX.ToString("0.000") + "\n"); // Write length in X-axis
                System.Console.Write("Length - global Y-axis:\t" + dDeltaY.ToString("0.000") + "\n"); // Write length in Y-axis
                System.Console.Write("Length - global Z-axis:\t" + dDeltaZ.ToString("0.000") + "\n\n"); // Write length in Z-axis
            }

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

            if (BIsDebugging)
            {
                // Temporary console output
                System.Console.Write("\n" + "Rotation angles:\n");
                System.Console.Write("Rotation about global X-axis:\t" + dAlphaX.ToString("0.000") + "rad\t " + (dAlphaX * 180.0f / MathF.fPI).ToString("0.0") + "deg \n"); // Write rotation about X-axis
                System.Console.Write("Rotation about global Y-axis:\t" + dBetaY.ToString("0.000") + "rad\t " + (dBetaY * 180.0f / MathF.fPI).ToString("0.0") + "deg \n"); // Write rotation about Y-axis
                System.Console.Write("Rotation about global Z-axis:\t" + dGammaZ.ToString("0.000") + "rad\t " + (dGammaZ * 180.0f / MathF.fPI).ToString("0.0") + "deg \n"); // Write rotation about Z-axis
                System.Console.Write("\n" + "Auxiliary rotation angles - graphics:\n");
                System.Console.Write("Rotation about global Y-axis:\t" + dBetaY_aux.ToString("0.000") + "rad\t " + (dBetaY_aux * 180.0f / MathF.fPI).ToString("0.0") + "deg \n"); // Write auxiliary rotation about Y-axis
                System.Console.Write("Rotation about global Z-axis:\t" + dGammaZ_aux.ToString("0.000") + "rad\t " + (dGammaZ_aux * 180.0f / MathF.fPI).ToString("0.0") + "deg \n\n"); // Write auxiliary rotation about Z-axis
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

            theta_x = 0.0f; // TODO - otocenie pruta okolo lokalnej osi v GCS nefunguje spravne ??? 

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

        public void GetRotationAngles(out double dAlphaX, out double dBetaY, out double dGammaZ, out double dBetaY_aux, out double dGammaZ_aux)
        {
            Point3D pA = new Point3D (NodeStart.X, NodeStart.Y, NodeStart.Z);
            Point3D pB = new Point3D(NodeEnd.X, NodeEnd.Y, NodeEnd.Z);

            double dDelta_X = pB.X - pA.X;
            double dDelta_Y = pB.Y - pA.Y;
            double dDelta_Z = pB.Z - pA.Z;

            // Uhly pootocenia LCS okolo osi GCS
            // Angles
            dAlphaX = Geom2D.GetAlpha2D_CW(dDelta_Y, dDelta_Z);
            dBetaY = Geom2D.GetAlpha2D_CW_2(dDelta_X, dDelta_Z); // !!! Pre pootocenie okolo Y su pouzite ine kvadranty !!!
            dGammaZ = Geom2D.GetAlpha2D_CW(dDelta_X, dDelta_Y);

            double dLength_XY = 0;

            if (!MathF.d_equal(dDelta_X, 0.0) || !MathF.d_equal(dDelta_Y, 0.0))
               dLength_XY = Math.Sqrt(Math.Pow(dDelta_X, 2) + Math.Pow(dDelta_Y, 2));

            // Auxialiary angles for members graphics
            dBetaY_aux = Geom2D.GetAlpha2D_CW_3(dDelta_X, dDelta_Z, Math.Sqrt(Math.Pow(dLength_XY, 2) + Math.Pow(dDelta_Z, 2)));
            dGammaZ_aux = dGammaZ;

            if (Math.PI / 2 < dBetaY && dBetaY < 1.5 * Math.PI)
            {
                if (dGammaZ < Math.PI)
                    dGammaZ_aux = dGammaZ + Math.PI;
                else
                    dGammaZ_aux = dGammaZ - Math.PI;
            }
        }



        //refaktoring
        public ScreenSpaceLines3D CreateWireFrame(float x, EGCS eGCS = EGCS.eGCSLeftHanded, Color? color = null, double thickness = 1.0)
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();
            Color wireFrameColor = Color.FromRgb(60, 60, 60);
            if (color != null) wireFrameColor = (Color)color;
            wireFrame.Color = wireFrameColor;
            wireFrame.Thickness = thickness;

            // Todo Prepracovat pre vnutornu a vonkajsiu outline
            // Zjednotit s AAC panel

            float fy, fz;

            if (CrScStart.CrScPointsOut != null && CrScStart.CrScPointsOut.Length > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsOut.Length / 2 - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    if (i < CrScStart.CrScPointsOut.Length / 2 - CrScStart.INoAuxPoints - 1)
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints + 1, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i + 1, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints + 1, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i + 1, 1], DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }
                    else // Last line
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0, 1], DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }

                    // Add points
                    wireFrame.Points.Add(pi);
                    wireFrame.Points.Add(pj);
                }
            }

            if (CrScStart.CrScPointsIn != null && CrScStart.CrScPointsIn.Length > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsIn.Length / 2 - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    if (i < CrScStart.CrScPointsIn.Length / 2 - CrScStart.INoAuxPoints - 1)
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints + 1, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i + 1, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints + 1, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i + 1, 1], DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }
                    else // Last line
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0, 1], DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0, 1], DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }

                    // Add points
                    wireFrame.Points.Add(pi);
                    wireFrame.Points.Add(pj);
                }
            }

            // Transform coordinates from LCS to GCS
            Point3D p_temp = new Point3D();
            p_temp.X = NodeStart.X;
            p_temp.Y = NodeStart.Y;
            p_temp.Z = NodeStart.Z;

            TransformMember_LCStoGCS(eGCS, p_temp, Delta_X, Delta_Y, Delta_Z, m_dTheta_x, wireFrame.Points);

            return wireFrame;
        }

        public ScreenSpaceLines3D CreateWireFrameLateral(EGCS eGCS = EGCS.eGCSLeftHanded, Color? color = null, double thickness = 1.0)
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();            
            Color wireFrameColor = Color.FromRgb(60, 60, 60);
            if (color != null) wireFrameColor = (Color)color;
            wireFrame.Color = wireFrameColor;
            wireFrame.Thickness = thickness;

            // Todo Prepracovat pre vnutornu a vonkajsiu outline
            // Zjednotit s AAC panel

            float fy, fz;

            if (CrScStart.CrScPointsOut != null && CrScStart.CrScPointsOut.Length > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsOut.Length / 2 - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                    pi = new Point3D(- FAlignment_Start, fy, fz);

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                    pj = new Point3D(FLength + FAlignment_End, fy, fz);

                    // Add points
                    wireFrame.Points.Add(pi);
                    wireFrame.Points.Add(pj);
                }
            }

            if (CrScStart.CrScPointsIn != null && CrScStart.CrScPointsIn.Length > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsIn.Length / 2 - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                    pi = new Point3D(-FAlignment_Start, fy, fz);

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints, 0], CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i, 1], DTheta_x);

                    pj = new Point3D(FLength + FAlignment_End, fy, fz);

                    // Add points
                    wireFrame.Points.Add(pi);
                    wireFrame.Points.Add(pj);
                }
            }

            // Transform coordinates from LCS to GCS
            Point3D p_temp = new Point3D();
            p_temp.X = NodeStart.X;
            p_temp.Y = NodeStart.Y;
            p_temp.Z = NodeStart.Z;

            TransformMember_LCStoGCS(eGCS, p_temp, Delta_X, Delta_Y, Delta_Z, m_dTheta_x, wireFrame.Points);

            return wireFrame;
        }


        public void CalculateMemberLimits(out float fTempMax_X, out float fTempMin_X, out float fTempMax_Y, out float fTempMin_Y, out float fTempMax_Z, out float fTempMin_Z)
        {
            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;
            fTempMax_Z = float.MinValue;
            fTempMin_Z = float.MaxValue;
            
            if (CrScStart.CrScPointsOut != null) // Some cross-section points exist
            {
                // Maximum X - coordinate
                fTempMax_X = NodeStart.X;
                // Minimum X - coordinate
                fTempMin_X = NodeEnd.X;

                for (int i = 0; i < CrScStart.CrScPointsOut.Length / 2; i++)
                {
                    // Maximum Y - coordinate
                    if (CrScStart.CrScPointsOut[i, 0] > fTempMax_Y)
                        fTempMax_Y = CrScStart.CrScPointsOut[i, 0];
                    // Minimum Y - coordinate
                    if (CrScStart.CrScPointsOut[i, 0] < fTempMin_Y)
                        fTempMin_Y = CrScStart.CrScPointsOut[i, 0];
                    // Maximum Z - coordinate
                    if (CrScStart.CrScPointsOut[i, 1] > fTempMax_Z)
                        fTempMax_Z = CrScStart.CrScPointsOut[i, 1];
                    // Minimum Z - coordinate
                    if (CrScStart.CrScPointsOut[i, 1] < fTempMin_Z)
                        fTempMin_Z = CrScStart.CrScPointsOut[i, 1];
                }
            }
        }
        
        protected Point3D RotatePoint_POKUSY(EGCS eGCS, Point3D pA, Point3D p, double alphaX, double betaY, double gamaZ, double dDeltaX, double dDeltaY, double dDeltaZ)
        {
            Point3D p3Drotated = new Point3D();

            /* Commented 25.5.2013
            // Left Handed

            p3Drotated.X = pA.X + p.X;
            p3Drotated.Y = pA.Y + (p.Y * Math.Cos(alphaX) - p.Z * Math.Sin(alphaX));
            p3Drotated.Z = pA.Z + (p.Y * Math.Sin(alphaX) + p.Z * Math.Cos(alphaX));

            p3Drotated.X = pA.X + (p.X * Math.Cos(betaY) + p.Z * Math.Sin(betaY));
            p3Drotated.Y = pA.Y + p.Y;
            p3Drotated.Z = pA.Z + (-p.X * Math.Sin(betaY) + p.Z * Math.Cos(betaY));

            p3Drotated.X = pA.X + (p.X * Math.Cos(gamaZ) - p.Y * Math.Sin(gamaZ));
            p3Drotated.Y = pA.Y + (p.X * Math.Sin(gamaZ) + p.Y * Math.Cos(gamaZ));
            p3Drotated.Z = pA.Z + p.Z;
            */

            // Left Handed
            //http://sk.wikipedia.org/wiki/Trojrozmern%C3%A1_projekcia#D.C3.A1ta_nevyhnutn.C3.A9_pre_projekciu
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

             cos (betaY)   0     sin (betaY)    0
             0             1     0              0
            -sin (betaY)   0     cos (betaY)    0
             0             0     0              1
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
            0      cos (alphaX)    sin (alphaX)       0
            0     -sin (alphaX)    cos (alphaX)       0
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

            /*
            Point3D pTemp1 = new Point3D();
            Point3D pTemp2 = new Point3D();

            if (eGCS == EGCS.eGCSLeftHanded)
            {
                // Left handed

                pTemp1.X = p.X;
                pTemp1.Y = (Math.Cos(alphaX) * p.Y) - (Math.Sin(alphaX) * p.Z);
                pTemp1.Z = (Math.Sin(alphaX) * p.Y) + (Math.Cos(alphaX) * p.Z);

                pTemp2.X = (Math.Cos(betaY) * pTemp1.X) + (Math.Sin(betaY) * pTemp1.Z);
                pTemp2.Y = pTemp1.Y;
                pTemp2.Z = (-Math.Sin(betaY) * pTemp1.X) + (Math.Cos(betaY) * pTemp1.Z);

                p3Drotated.X = pA.X + ((Math.Cos(gamaZ) * pTemp2.X) - (Math.Sin(gamaZ) * pTemp2.Y));
                p3Drotated.Y = pA.Y + ((Math.Sin(gamaZ) * pTemp2.X) + (Math.Cos(gamaZ) * pTemp2.Y));
                p3Drotated.Z = pA.Z + pTemp2.Z;
            }
            else
            {
                // Right handed

                pTemp1.X = p.X;
                pTemp1.Y = (Math.Cos(alphaX) * p.Y) + (Math.Sin(alphaX) * p.Z);
                pTemp1.Z = (-Math.Sin(alphaX) * p.Y) + (Math.Cos(alphaX) * p.Z);

                pTemp2.X = (Math.Cos(betaY) * pTemp1.X) - (Math.Sin(betaY) * pTemp1.Z);
                pTemp2.Y = pTemp1.Y;
                pTemp2.Z = (Math.Sin(betaY) * pTemp1.X) + (Math.Cos(betaY) * pTemp1.Z);

                p3Drotated.X = pA.X + ((Math.Cos(gamaZ) * pTemp2.X) + (Math.Sin(gamaZ) * pTemp2.Y));
                p3Drotated.Y = pA.Y + ((-Math.Sin(gamaZ) * pTemp2.X) + (Math.Cos(gamaZ) * pTemp2.Y));
                p3Drotated.Z = pA.Z + pTemp2.Z;
            }
            */

            // In case that member is parallel to global axis should be rotated only once
            if (dDeltaX < 0 && MathF.d_equal(dDeltaY, 0.0) && MathF.d_equal(dDeltaZ, 0.0))      // Parallel to X-axis with negative orientation
                betaY = 0; // Do not rotate about Y-axis
            else if (MathF.d_equal(dDeltaX, 0.0) && dDeltaY < 0 && MathF.d_equal(dDeltaZ, 0.0)) // Parallel to Y-axis with negative orientation
                alphaX = 0; // Do not rotate about X-axis
            else if (MathF.d_equal(dDeltaX, 0.0) && MathF.d_equal(dDeltaY, 0.0) && dDeltaZ < 0) // Parallel to Z-axis with negative orientation
                betaY = 0; // Do not rotate about Y-axis
            else
            {
                // No action - General position of member in space
            }

            // Cumulative 3D rotation and translation
            // Temp - pokus 1
            // Rotate around x, y, z
            double ax = Math.Cos(betaY) * Math.Cos(gamaZ);
            double ay = (Math.Cos(gamaZ) * Math.Sin(alphaX) * Math.Sin(betaY)) - (Math.Cos(alphaX) * Math.Sin(gamaZ));
            double az = (Math.Cos(alphaX) * Math.Cos(gamaZ) * Math.Sin(betaY)) + (Math.Sin(alphaX) * Math.Sin(gamaZ));

            p3Drotated.X = ((Math.Cos(betaY) * Math.Cos(gamaZ)) * p.X + ((Math.Cos(gamaZ) * Math.Sin(alphaX) * Math.Sin(betaY)) - (Math.Cos(alphaX) * Math.Sin(gamaZ))) * p.Y + ((Math.Cos(alphaX) * Math.Cos(gamaZ) * Math.Sin(betaY)) + (Math.Sin(alphaX) * Math.Sin(gamaZ))) * p.Z);

            double bx = Math.Cos(betaY) * Math.Sin(gamaZ);
            double by = (Math.Cos(alphaX) * Math.Cos(gamaZ)) + (Math.Sin(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ));
            double bz = (-Math.Cos(gamaZ) * Math.Sin(alphaX)) + (Math.Cos(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ));

            p3Drotated.Y = ((Math.Cos(betaY) * Math.Sin(gamaZ)) * p.X + ((Math.Cos(alphaX) * Math.Cos(gamaZ)) + (Math.Sin(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ))) * p.Y + ((-Math.Cos(gamaZ) * Math.Sin(alphaX)) + (Math.Cos(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ))) * p.Z);

            double cx = -Math.Sin(betaY);
            double cy = Math.Cos(betaY) * Math.Sin(alphaX);
            double cz = Math.Cos(alphaX) * Math.Cos(betaY);

            p3Drotated.Z = ((-Math.Sin(betaY)) * p.X + (Math.Cos(betaY) * Math.Sin(alphaX)) * p.Y + (Math.Cos(alphaX) * Math.Cos(betaY)) * p.Z);


            // Temp - pokus 2

            if (gamaZ < Math.PI)
                gamaZ += Math.PI;
            else if (gamaZ > Math.PI)
                gamaZ -= Math.PI;


            // X Rotation
            ax = 1 * p.X + 0 * p.Y + 0 * p.Z;
            ay = 0 * p.X + Math.Cos(alphaX) * p.Y - Math.Sin(alphaX) * p.Z;
            az = Math.Sin(alphaX) * p.X + Math.Cos(alphaX) * p.Y + 0 * p.Z;

            // Y Rotation
            bx = Math.Cos(betaY) * ax + 0 * ay + Math.Sin(betaY) * az;
            by = 0 * ax + 1 * ay + 0 * az;
            bz = -Math.Sin(betaY) * ax + 0 * ay + Math.Cos(betaY) * az;

            // Z Rotation
            cx = Math.Cos(gamaZ) * bx - Math.Sin(gamaZ) * by + 0 * bz;
            cy = Math.Sin(gamaZ) * bx + Math.Cos(gamaZ) * by + 0 * bz;
            cz = 0 * bx + 0 * by + 1 * bz;

            p3Drotated.X = pA.X + cx;
            p3Drotated.Y = pA.Y + cy;
            p3Drotated.Z = pA.Z + cz;

            p3Drotated.X = pA.X + ((Math.Cos(betaY) * Math.Cos(gamaZ)) * p.X + ((Math.Cos(gamaZ) * Math.Sin(alphaX) * Math.Sin(betaY)) - (Math.Cos(alphaX) * Math.Sin(gamaZ))) * p.Y + ((Math.Cos(alphaX) * Math.Cos(gamaZ) * Math.Sin(betaY)) + (Math.Sin(alphaX) * Math.Sin(gamaZ))) * p.Z);
            p3Drotated.Y = pA.Y + ((Math.Cos(betaY) * Math.Sin(gamaZ)) * p.X + ((Math.Cos(alphaX) * Math.Cos(gamaZ)) + (Math.Sin(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ))) * p.Y + ((-Math.Cos(gamaZ) * Math.Sin(alphaX)) + (Math.Cos(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ))) * p.Z);
            p3Drotated.Z = pA.Z + ((-Math.Sin(betaY)) * p.X + (Math.Cos(betaY) * Math.Sin(alphaX)) * p.Y + (Math.Cos(alphaX) * Math.Cos(betaY)) * p.Z);

            // Rotate around z, y, x
            /*
            p3Drotated.X = pA.X + ((Math.Cos(betaY) * Math.Cos(gamaZ)) * p.X + (-Math.Cos(betaY) * Math.Sin(gamaZ)) * p.Y + (Math.Sin(betaY)) * p.Z);
            p3Drotated.Y = pA.Y + (((Math.Cos(alphaX) * Math.Sin(gamaZ)) + (Math.Sin(alphaX)*Math.Sin(betaY) * Math.Cos(gamaZ))) * p.X + ((Math.Cos(alphaX) * Math.Cos(gamaZ)) - (Math.Sin(alphaX) * Math.Sin(betaY) * Math.Sin(gamaZ))) * p.Y + (-Math.Sin(alphaX) * Math.Cos(betaY)) * p.Z);
            p3Drotated.Z = pA.Z + (((Math.Sin(alphaX) * Math.Sin(gamaZ)) - (Math.Cos(alphaX) * Math.Sin(betaY) * Math.Cos(gamaZ))) * p.X + ((Math.Sin(alphaX) * Math.Cos(gamaZ)) + (Math.Cos(alphaX) * Math.Sin(betaY) * Math.Sin (gamaZ))) * p.Y + (Math.Cos(alphaX) * Math.Cos(betaY)) * p.Z);
            */

            // Right-handed cumulative ???
            // http://scipp.ucsc.edu/~haber/ph216/rotation_12.pdf
            /*
            p3Drotated.X = pA.X + (((Math.Cos(alphaX) * Math.Cos(betaY) * Math.Cos(gamaZ)) - (Math.Sin(alphaX) * Math.Sin(gamaZ))) * p.X + (-Math.Cos(alphaX) * Math.Cos(betaY) * Math.Sin(gamaZ) - (Math.Sin(alphaX) * Math.Cos(gamaZ))) * p.Y + (Math.Cos(alphaX) * Math.Sin(betaY)) * p.Z);
            p3Drotated.Y = pA.Y + (((Math.Sin(alphaX) * Math.Cos(betaY) * Math.Cos(gamaZ)) + (Math.Cos(alphaX) * Math.Sin(gamaZ))) * p.X + (-Math.Sin(alphaX) * Math.Cos(betaY) * Math.Sin(gamaZ) + (Math.Cos(alphaX) * Math.Cos(gamaZ))) * p.Y + (Math.Sin(alphaX) * Math.Sin(betaY)) * p.Z);
            p3Drotated.Z = pA.Z + ((-Math.Sin(betaY) * Math.Cos(gamaZ)) * p.X + (Math.Sin(betaY) * Math.Sin(gamaZ)) * p.Y + (Math.Cos(betaY)) * p.Z);
            */

            //
            // http://inside.mines.edu/~gmurray/ArbitraryAxisRotation/
            /*
            p3Drotated.X = pA.X + ((Math.Cos(gamaZ) * Math.Cos(betaY)) * p.X + (-Math.Sin(gamaZ) * Math.Cos(betaY)) * p.Y + Math.Sin(betaY) * p.Z);
            p3Drotated.Y = pA.Y + (((Math.Cos(gamaZ) * Math.Sin(betaY) * Math.Sin(alphaX)) + (Math.Sin(gamaZ) * Math.Cos(alphaX))) * p.X + ((Math.Cos(gamaZ) * Math.Cos(alphaX)) - (Math.Sin(gamaZ) * Math.Sin(betaY) * Math.Sin(alphaX))) * p.Y + (-Math.Cos(betaY) * Math.Sin(alphaX)) * p.Z);
            p3Drotated.Z = pA.Z + (((Math.Sin(gamaZ) * Math.Sin(alphaX)) - (Math.Cos(gamaZ) * Math.Sin(betaY) * Math.Cos(alphaX))) * p.X + ((Math.Sin(gamaZ) * Math.Sin(betaY) * Math.Cos(alphaX)) + (Math.Sin(alphaX) * Math.Cos(gamaZ))) * p.Y + (Math.Cos(betaY) * Math.Cos(alphaX)) * p.Z);
            */


            return p3Drotated;


            // Mozno by som mal zapracovat toto
            //http://mathworld.wolfram.com/EulerAngles.html
        }

    } // End of Class CMember

    /*
    public class CCompare_MemberID : IComparer
    {
        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
        public int Compare(object x, object y)
        {
            return ((CMember)x).ID - ((CMember)y).ID;
        }
    }
    */
}
