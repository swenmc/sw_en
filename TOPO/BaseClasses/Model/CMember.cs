using _3DTools;
using BaseClasses.Helpers;
using CRSC;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMember : CEntity3D
    {
        private CNode m_NodeStart;

        public CNode NodeStart
        {
          get { return m_NodeStart; }
          set { m_NodeStart = value; }
        }
        private CNode m_NodeEnd;

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

        private CMemberEccentricity m_EccentricityStart;

        public CMemberEccentricity EccentricityStart
        {
            get { return m_EccentricityStart; }
            set { m_EccentricityStart = value; }
        }

        private CMemberEccentricity m_EccentricityEnd;

        public CMemberEccentricity EccentricityEnd
        {
            get { return m_EccentricityEnd; }
            set { m_EccentricityEnd = value; }
        }

        private List<CIntermediateTransverseSupport> m_IntermediateTransverseSupportGroup;
        public List<CIntermediateTransverseSupport> IntermediateTransverseSupportGroup
        {
            get
            {
                return m_IntermediateTransverseSupportGroup;
            }

            set
            {
                m_IntermediateTransverseSupportGroup = value;
            }
        }

        private List<CSegment_LTB> m_LTBSegmentGroup;
        public List<CSegment_LTB> LTBSegmentGroup // Group of LTB segments at member (default - 1 segment)
        {
            get
            {
                return m_LTBSegmentGroup;
            }

            set
            {
                m_LTBSegmentGroup = value;
            }
        }

        private EMemberType_FS eMemberType_FS;

        public EMemberType_FS EMemberType
        {
            get { return eMemberType_FS; }
            set { eMemberType_FS = value; }
        }
        private EMemberType_FS_Position eMemberType_DB;

        public EMemberType_FS_Position EMemberTypePosition
        {
            get { return eMemberType_DB; }
            set { eMemberType_DB = value; }
        }

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

        private double m_dTheta_x;

        public double DTheta_x
        {
            get { return m_dTheta_x; }
            set { m_dTheta_x = value; }
        }

        // Member results - list of values in each Load Case
        private List<designBucklingLengthFactors> m_sBucklingLengthFactors;

        public List<designBucklingLengthFactors> MBucklingLengthFactors
        {
            get { return m_sBucklingLengthFactors; }
            set { m_sBucklingLengthFactors = value; }
        }

        private List<designMomentValuesForCb> m_sMomentValuesforCb;

        public List<designMomentValuesForCb> MMomentValuesforCb
        {
            get { return m_sMomentValuesforCb; }
            set { m_sMomentValuesforCb = value; }
        }

        private List<basicInternalForces[]> m_sBIF_x;

        public List<basicInternalForces[]> MBIF_x
        {
            get { return m_sBIF_x; }
            set { m_sBIF_x = value; }
        }

        private List<basicDeflections[]> m_sBDef_x;

        public List<basicDeflections[]> MBDef_x
        {
            get { return m_sBDef_x; }
            set { m_sBDef_x = value; }
        }

        private List<Point3D> MWireFramePoints;
        public List<Point3D> WireFramePoints
        {
            get
            {
                if (MWireFramePoints == null) MWireFramePoints = new List<Point3D>();
                return MWireFramePoints;
            }

            set
            {
                MWireFramePoints = value;
            }
        }

        private List<CLoad> MLoads;
        public List<CLoad> Loads
        {
            get
            {
                if (MLoads == null) MLoads = new List<CLoad>();
                return MLoads;
            }

            set
            {
                MLoads = value;
            }
        }

        private Color m_color;
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }
        
        private List<CNode> m_IntermediateNodes;
        public List<CNode> IntermediateNodes
        {
            get
            {
                if (m_IntermediateNodes == null) m_IntermediateNodes = new List<CNode>();
                return m_IntermediateNodes;
            }

            set
            {
                m_IntermediateNodes = value;
            }
        }
        //public List<Point3D> WireFramePoints;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        // Constructor 1
        public CMember()
        {
            m_NodeStart = new CNode(0,0,0,0,0);
            m_NodeEnd = new CNode(1,1,0,0,0);
            m_cnRelease1 = null;
            m_cnRelease2 = null;
            ID = 0;
            m_CrScStart = null;
            m_CrScEnd =null;
            //eMemberType_FS;
            EccentricityStart = new CMemberEccentricity(0,0);
            EccentricityEnd = new CMemberEccentricity(0, 0);
            FAlignment_Start = 0;
            FAlignment_End = 0;
            FTime = 0;

            Fill_Basic();
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
            EMemberType_FS eMemberType,
            EMemberType_FS_Position eMemberType_Position,
            CMemberEccentricity objEccentricityStart,
            CMemberEccentricity objEccentricityEnd,
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
            eMemberType_DB = eMemberType_Position;
            EccentricityStart = objEccentricityStart;
            EccentricityEnd = objEccentricityEnd;
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
            EMemberType_FS eMemberType,
            CMemberEccentricity objEccentricityStart,
            CMemberEccentricity objEccentricityEnd,
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

            EccentricityStart = objEccentricityStart;
            EccentricityEnd = objEccentricityEnd;
            FAlignment_Start = fAligment1;
            FAlignment_End = fAligment2;
            FTime = fTime;

            Fill_Basic();
        }


        //Fill basic data
        public void Fill_Basic()
        {
            // Theroretical length of member (between definition nodes) - used for calculation
            FLength = (float)Math.Sqrt((float)Math.Pow(m_NodeEnd.X - m_NodeStart.X, 2f) + (float)Math.Pow(m_NodeEnd.Y - m_NodeStart.Y, 2f) + (float)Math.Pow(m_NodeEnd.Z - m_NodeStart.Z, 2f));

            // Real length of member (displayed in graphics, used for material list)
            FLength_real = FAlignment_Start + FLength + FAlignment_End;
            
            // Priemet do osi GCS - rozdiel suradnic v GCS
            Delta_X = m_NodeEnd.X - m_NodeStart.X;
            Delta_Y = m_NodeEnd.Y - m_NodeStart.Y;
            Delta_Z = m_NodeEnd.Z - m_NodeStart.Z;

            SetStartPoint3DCoord();
            SetEndPoint3DCoord();

            // Add created member to the list of members in cross-section object
            //if (m_CrScStart != null)
            //    m_CrScStart.AssignedMembersList.Add(this);

            //if (m_CrScEnd != null)
            //    m_CrScEnd.AssignedMembersList.Add(this);

            // Set member name according to enum value of member type
            CComponentPrefixes component = CModelsManager.GetModelComponent((int)EMemberType + 1);

            Prefix = component.ComponentPrefix;
            Name = component.ComponentName;

            // Member is generated
            BIsGenerated = true;
            // Set as default property that member should be displayed
            BIsDisplayed = true;
        }

        public void SetStartPoint3DCoord()
        {
            m_PointStart.X = m_NodeStart.X;
            m_PointStart.Y = m_NodeStart.Y;
            m_PointStart.Z = m_NodeStart.Z;

            Point3DCollection temp = new Point3DCollection();
            Point3D tempPoint = new Point3D(-m_fAlignment_Start,0,0); // skratenie je ma zapornu hodnotu, predlzenie ma kladnu hodnotu
            temp.Add(tempPoint);

            // Riadiaci uzol/bod pre translaciu bodov pruta je vzdy NodeStart
            TransformMember_LCStoGCS(EGCS.eGCSLeftHanded, new Point3D(m_NodeStart.X, m_NodeStart.Y, m_NodeStart.Z), Delta_X, Delta_Y, Delta_Z, m_dTheta_x, temp);

            m_PointStart = temp[0];
        }
        public void SetEndPoint3DCoord()
        {
            m_PointEnd.X = m_NodeEnd.X;
            m_PointEnd.Y = m_NodeEnd.Y;
            m_PointEnd.Z = m_NodeEnd.Z;

            Point3DCollection temp = new Point3DCollection();
            Point3D tempPoint = new Point3D(m_fAlignment_End, 0, 0); // skratenie je ma zapornu hodnotu, predlzenie ma kladnu hodnotu
            temp.Add(tempPoint);

            // Riadiaci uzol/bod pre translaciu bodov pruta je vzdy NodeStart
            TransformMember_LCStoGCS(EGCS.eGCSLeftHanded, new Point3D(m_NodeStart.X, m_NodeStart.Y, m_NodeStart.Z), Delta_X, Delta_Y, Delta_Z, m_dTheta_x, temp);

            m_PointEnd = temp[0];
        }

        public Model3DGroup getM_3D_G_Member(EGCS eGCS, SolidColorBrush brushFrontSide, SolidColorBrush brushShell, SolidColorBrush brushBackSide, bool bUseDiffuseMaterial, bool bUseEmissiveMaterial)
        {
            Model3DGroup MObject3DModel = new Model3DGroup(); // Whole member

            GeometryModel3D modelFrontSide = new GeometryModel3D();
            GeometryModel3D modelShell = new GeometryModel3D();
            GeometryModel3D modelBackSide = new GeometryModel3D();

            getG_M_3D_Member(eGCS, brushFrontSide, brushShell, brushBackSide, bUseDiffuseMaterial, bUseEmissiveMaterial, out modelFrontSide, out  modelShell, out modelBackSide);

            MObject3DModel.Children.Add(modelFrontSide);
            MObject3DModel.Children.Add(modelBackSide);
            MObject3DModel.Children.Add(modelShell);

            return MObject3DModel;
        }

        public GeometryModel3D getG_M_3D_Member(EGCS eGCS, SolidColorBrush brush, bool bUseDiffuseMaterial, bool bUseEmissiveMaterial)
        {
            GeometryModel3D model = new GeometryModel3D();

            MeshGeometry3D mesh;

            getMeshMemberGeometry3DFromCrSc_One(eGCS, CrScStart, CrScEnd, DTheta_x, out mesh); // Mesh one member

            model.Geometry = mesh;

            MaterialGroup materialGroup = new MaterialGroup();

            if (bUseDiffuseMaterial || bUseEmissiveMaterial)
            {
                if (bUseDiffuseMaterial)
                    materialGroup.Children.Add(new DiffuseMaterial(brush));

                if (bUseEmissiveMaterial)
                    materialGroup.Children.Add(new EmissiveMaterial(brush));
            }
            else
            {
                throw new Exception("Exception - material is not valid");

                // Temporary - default in case that material si not defined
                //bUseDiffuseMaterial = true;
                //materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.YellowGreen)));
            }

            model.Material = materialGroup;
            return model;
        }

        public void getG_M_3D_Member(EGCS eGCS, SolidColorBrush brushFrontSide, SolidColorBrush brushShell, SolidColorBrush brushBackSide, bool bUseDiffuseMaterial, bool bUseEmissiveMaterial,
            out GeometryModel3D modelFrontSide, out GeometryModel3D modelShell, out GeometryModel3D modelBackSide)
        {
            modelFrontSide = new GeometryModel3D();
            modelBackSide = new GeometryModel3D();
            modelShell = new GeometryModel3D();

            MeshGeometry3D meshFrontSide = new MeshGeometry3D();
            MeshGeometry3D meshBackSide = new MeshGeometry3D();
            MeshGeometry3D meshShell = new MeshGeometry3D();

            getMeshMemberGeometry3DFromCrSc_Three(eGCS, CrScStart, CrScEnd, DTheta_x, out meshFrontSide, out meshShell, out meshBackSide);

            modelFrontSide.Geometry = meshFrontSide;
            modelBackSide.Geometry = meshBackSide;
            modelShell.Geometry = meshShell;

            modelFrontSide.Material = new EmissiveMaterial(brushFrontSide);
            modelBackSide.Material = new EmissiveMaterial(brushBackSide);
            modelShell.Material = new EmissiveMaterial(brushShell);

            MaterialGroup materialGroupFrontSide = new MaterialGroup();
            MaterialGroup materialGroupBackSide = new MaterialGroup();
            MaterialGroup materialGroupShell = new MaterialGroup();

            if (bUseDiffuseMaterial || bUseEmissiveMaterial)
            {
                if (bUseDiffuseMaterial)
                {
                    materialGroupFrontSide.Children.Add(new DiffuseMaterial(brushFrontSide));
                    materialGroupBackSide.Children.Add(new DiffuseMaterial(brushBackSide));
                    materialGroupShell.Children.Add(new DiffuseMaterial(brushShell));
                }

                if (bUseEmissiveMaterial)
                {
                    materialGroupFrontSide.Children.Add(new EmissiveMaterial(brushFrontSide));
                    materialGroupBackSide.Children.Add(new EmissiveMaterial(brushBackSide));
                    materialGroupShell.Children.Add(new EmissiveMaterial(brushShell));
                }
            }
            else
            {
                throw new Exception("Exception - material is not valid");
            }

            modelFrontSide.Material = materialGroupFrontSide;
            modelBackSide.Material = materialGroupBackSide;
            modelShell.Material = materialGroupShell;
        }

        // TODO Ondrej - 25/07/2018 refaktorovat a optimalizovat metody
        // getMeshMemberGeometry3DFromCrSc_One (fast rendering, one color)
        // getMeshMemberGeometry3DFromCrSc_Three (distinguished colors)

        private void getMeshMemberGeometry3DFromCrSc_One(EGCS eGCS, CCrSc obj_CrScA, CCrSc obj_CrScB, double dTheta_x, out MeshGeometry3D mesh)
        {
            // All in one mesh
            mesh = new MeshGeometry3D();

            // Kvoli optimalizacii potrebne este inicializovat velkost kolekcie
            Point3DCollection meshPositions = new Point3DCollection();

            // Number of Points per section
            int iNoCrScPoints2D;
            double fy, fz;

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

                        // Set original value to the temporary variable
                        fy = obj_CrScA.CrScPointsOut[j].X;
                        fz = obj_CrScA.CrScPointsOut[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityStart != null)
                        {
                            fy += EccentricityStart.MFy_local;
                            fz += EccentricityStart.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                        meshPositions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }
                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScA.CrScPointsOut[j].X;
                            fz = obj_CrScA.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                        }
                        else
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScB.CrScPointsOut[j].X;
                            fz = obj_CrScB.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
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

                        // Set original value to the temporary variable
                        fy = obj_CrScA.CrScPointsOut[j].X;
                        fz = obj_CrScA.CrScPointsOut[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityStart != null)
                        {
                            fy += EccentricityStart.MFy_local;
                            fz += EccentricityStart.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                        meshPositions.Add(new Point3D(-FAlignment_Start,fy, fz));
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

                        // Set original value to the temporary variable
                        fy = obj_CrScA.CrScPointsIn[j].X;
                        fz = obj_CrScA.CrScPointsIn[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityStart != null)
                        {
                            fy += EccentricityStart.MFy_local;
                            fz += EccentricityStart.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                        meshPositions.Add(new Point3D(-FAlignment_Start, fy, fz));
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
                            // Set original value to the temporary variable
                            fy = obj_CrScA.CrScPointsOut[j].X;
                            fz = obj_CrScA.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                        }
                        else
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScB.CrScPointsOut[j].X;
                            fz = obj_CrScB.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
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
                            // Set original value to the temporary variable
                            fy = obj_CrScA.CrScPointsIn[j].X;
                            fz = obj_CrScA.CrScPointsIn[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                        }
                        else
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScB.CrScPointsIn[j].X;
                            fz = obj_CrScB.CrScPointsIn[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
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
            
            if (BIsDebugging)
            {
                // Dislay data in the output window
                string sOutput = "Before transformation \n\n"; // create temporary string
                for (int i = 0; i < 2 * iNoCrScPoints2D; i++) // for all mesh positions (start and end of member, number of edge points of whole member = 2 * number in one section)
                {
                    Point3D p3D = meshPositions[i]; // Get mesh element/item (returns Point3D)

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
                System.Console.Write(sOutput); // Write in console window
            }

            // Transform coordinates
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, m_dTheta_x, meshPositions);

            meshPositions.Freeze();
            mesh.Positions = meshPositions;
            // Mesh Triangles - various cross-sections shapes defined
            mesh.TriangleIndices = obj_CrScA.TriangleIndices;

            if (obj_CrScA.WireFrameIndices != null) // Validation of cross-section wireframe data
            {
                foreach (int n in obj_CrScA.WireFrameIndices)
                {
                    WireFramePoints.Add(meshPositions[n]);
                }
            }
            else
            {
                // TODO - zatial disabled, je potrebne najprv prerobit prierezy a doplnit definiciu wireframe
                //throw new ArgumentNullException();
            }

            if (BIsDebugging)
            {
                // Dislay data in the output window
                string sOutput = null;
                sOutput = "After transformation \n\n"; // create temporary string
                for (int i = 0; i < 2 * iNoCrScPoints2D; i++) // for all mesh positions (start and end of member, number of edge points of whole member = 2 * number in one section)
                {
                    Point3D p3D = meshPositions[i]; // Get mesh element/item (returns Point3D)

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
                System.Console.Write(sOutput); // Write in console window
            }
        }

        private void getMeshMemberGeometry3DFromCrSc_Three(EGCS eGCS, CCrSc obj_CrScA, CCrSc obj_CrScB, double dTheta_x, out MeshGeometry3D meshFrontSide, out MeshGeometry3D meshShell, out MeshGeometry3D meshBackSide)
        {
            // Separate mesh for front, back and shell surfaces of member
            meshFrontSide = new MeshGeometry3D();
            meshBackSide = new MeshGeometry3D();
            meshShell = new MeshGeometry3D();

            Point3DCollection meshFrontSidePositions = new Point3DCollection();
            Point3DCollection meshBackSidePositions = new Point3DCollection();
            Point3DCollection meshShellPositions = new Point3DCollection();

            // Number of Points per section
            int iNoCrScPoints2D;
            double fy, fz;

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

                        // Set original value to the temporary variable
                        fy = obj_CrScA.CrScPointsOut[j].X;
                        fz = obj_CrScA.CrScPointsOut[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityStart != null)
                        {
                            fy += EccentricityStart.MFy_local;
                            fz += EccentricityStart.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                        meshFrontSidePositions.Add(new Point3D(-FAlignment_Start, fy, fz));
                        meshShellPositions.Add(new Point3D(-FAlignment_Start, fy, fz));
                    }

                    for (int j = 0; j < iNoCrScPoints2D; j++)
                    {
                        // X - end, Y, Z
                        if (obj_CrScB == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScA.CrScPointsOut[j].X;
                            fz = obj_CrScA.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshBackSidePositions.Add(new Point3D(FLength + FAlignment_End,fy, fz)); // Constant size member
                            meshShellPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                        else
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScB.CrScPointsOut[j].X;
                            fz = obj_CrScB.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshBackSidePositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                            meshShellPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
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

                        // Set original value to the temporary variable
                        fy = obj_CrScA.CrScPointsOut[j].X;
                        fz = obj_CrScA.CrScPointsOut[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityStart != null)
                        {
                            fy += EccentricityStart.MFy_local;
                            fz += EccentricityStart.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                        meshFrontSidePositions.Add(new Point3D(-FAlignment_Start, fy, fz));
                        meshShellPositions.Add(new Point3D(-FAlignment_Start, fy, fz));
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

                        // Set original value to the temporary variable
                        fy = obj_CrScA.CrScPointsIn[j].X;
                        fz = obj_CrScA.CrScPointsIn[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityStart != null)
                        {
                            fy += EccentricityStart.MFy_local;
                            fz += EccentricityStart.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                        meshFrontSidePositions.Add(new Point3D(-FAlignment_Start, fy, fz));
                        meshShellPositions.Add(new Point3D(-FAlignment_Start, fy, fz));
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
                            // Set original value to the temporary variable
                            fy = obj_CrScA.CrScPointsOut[j].X;
                            fz = obj_CrScA.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshBackSidePositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                            meshShellPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                        else
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScB.CrScPointsOut[j].X;
                            fz = obj_CrScB.CrScPointsOut[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshBackSidePositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                            meshShellPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
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
                            // Set original value to the temporary variable
                            fy = obj_CrScA.CrScPointsIn[j].X;
                            fz = obj_CrScA.CrScPointsIn[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshBackSidePositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Constant size member
                            meshShellPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                        }
                        else
                        {
                            // Set original value to the temporary variable
                            fy = obj_CrScB.CrScPointsIn[j].X;
                            fz = obj_CrScB.CrScPointsIn[j].Y;

                            // Set Member Eccentricity
                            if (EccentricityEnd != null)
                            {
                                fy += EccentricityEnd.MFy_local;
                                fz += EccentricityEnd.MFz_local;
                            }

                            // Rotate about local x-axis
                            Geom2D.TransformPositions_CCW_rad(0f, 0f, dTheta_x, ref fy, ref fz);

                            meshBackSidePositions.Add(new Point3D(FLength + FAlignment_End, fy, fz)); // Tapered member
                            meshShellPositions.Add(new Point3D(FLength + FAlignment_End, fy, fz));
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
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y,NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, dTheta_x, meshFrontSidePositions); // Posun voci povodnemu definicnemu uzlu pruta
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, dTheta_x, meshBackSidePositions);
            TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), dDelta_X, dDelta_Y, dDelta_Z, dTheta_x, meshShellPositions);

            meshFrontSidePositions.Freeze();
            meshBackSidePositions.Freeze();
            meshShellPositions.Freeze();
            meshFrontSide.Positions = meshFrontSidePositions;
            meshBackSide.Positions = meshBackSidePositions;
            meshShell.Positions = meshShellPositions;

            // Mesh Triangles - various cross-sections shapes defined
            meshFrontSide.TriangleIndices = obj_CrScA.TriangleIndicesFrontSide;
            meshBackSide.TriangleIndices = obj_CrScA.TriangleIndicesBackSide;
            meshShell.TriangleIndices = obj_CrScA.TriangleIndicesShell;

            // Maximum index of position (point) used in collection
            int iFrontSideMaxIndex = MathF.Max(meshFrontSide.TriangleIndices);
            int iBackSideMaxIndex = MathF.Max(meshBackSide.TriangleIndices);
            int iShellMaxIndex = MathF.Max(meshShell.TriangleIndices);

            // Pre back side odpocitat zo vsetkych indices celkovy pocet bodov na prednej strane, odpocitat pre prierez len raz
            if (iBackSideMaxIndex > iFrontSideMaxIndex)
            {
                for (int i = 0; i < meshBackSide.TriangleIndices.Count; i++)
                    meshBackSide.TriangleIndices[i] -= obj_CrScA.ITotNoPoints;

                // Nastavit novu hodnotu
                iBackSideMaxIndex = MathF.Max(meshBackSide.TriangleIndices);
            }

            // Validation
            // Number of points in front and back side must be equal
            // Number of indices in front and back side must be equal
            // Number of points in shell must be front + back side
            // Number of indices in shell must be front and back
            if (meshFrontSide.Positions.Count != meshBackSide.Positions.Count ||
                meshFrontSide.TriangleIndices.Count != meshBackSide.TriangleIndices.Count ||
                meshShell.Positions.Count != (meshFrontSide.Positions.Count + meshBackSide.Positions.Count) ||
                meshShell.TriangleIndices.Count != (meshShell.Positions.Count * 3) ||
                iFrontSideMaxIndex != (meshFrontSide.Positions.Count - 1) ||
                iBackSideMaxIndex != (meshBackSide.Positions.Count - 1) ||
                iShellMaxIndex != (meshShell.Positions.Count - 1))
            {
               throw new Exception("Invalid number of positions or incides!");
            }

            // To Ondrej - urcite to tu potrebujeme nastavovat ???
            foreach (int n in obj_CrScA.WireFrameIndices)
            {
                WireFramePoints.Add(meshShellPositions[n]);
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

            // Tieto priemety sa nepouzili - zakomentovane
            /*
            if (!MathF.d_equal(dDeltaY, 0.0) || !MathF.d_equal(dDeltaZ, 0.0))
                dLength_YZ = Math.Sqrt(Math.Pow(dDeltaY, 2) + Math.Pow(dDeltaZ, 2));

            if (!MathF.d_equal(dDeltaX, 0.0) || !MathF.d_equal(dDeltaZ, 0.0))
                dLength_XZ = Math.Sqrt(Math.Pow(dDeltaX, 2) + Math.Pow(dDeltaZ, 2));
            */

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

            if (CrScStart.CrScPointsOut != null && CrScStart.CrScPointsOut.Count > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsOut.Count - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    if (i < CrScStart.CrScPointsOut.Count - CrScStart.INoAuxPoints - 1)
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints + 1].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i + 1].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints + 1].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i + 1].Y, DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }
                    else // Last line
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + 0].Y, DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }

                    // Add points
                    wireFrame.Points.Add(pi);
                    wireFrame.Points.Add(pj);
                }
            }

            if (CrScStart.CrScPointsIn != null && CrScStart.CrScPointsIn.Count > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsIn.Count - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    if (i < CrScStart.CrScPointsIn.Count - CrScStart.INoAuxPoints - 1)
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints + 1].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i + 1].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints + 1].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i + 1].Y, DTheta_x);

                        pj = new Point3D(x, fy, fz);
                    }
                    else // Last line
                    {
                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                        pi = new Point3D(x, fy, fz);

                        // Rotate about local x-axis
                        fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0].Y, DTheta_x);
                        fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + 0].Y, DTheta_x);

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

            if (CrScStart.CrScPointsOut != null && CrScStart.CrScPointsOut.Count > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsOut.Count - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                    pi = new Point3D(- FAlignment_Start, fy, fz);

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsOut[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsOut[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                    pj = new Point3D(FLength + FAlignment_End, fy, fz);

                    // Add points
                    wireFrame.Points.Add(pi);
                    wireFrame.Points.Add(pj);
                }
            }

            if (CrScStart.CrScPointsIn != null && CrScStart.CrScPointsIn.Count > 0)
            {
                for (int i = 0; i < CrScStart.CrScPointsIn.Count - CrScStart.INoAuxPoints; i++)
                {
                    Point3D pi = new Point3D();
                    Point3D pj = new Point3D();

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);

                    pi = new Point3D(-FAlignment_Start, fy, fz);

                    // Rotate about local x-axis
                    fy = (float)Geom2D.GetRotatedPosition_x_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);
                    fz = (float)Geom2D.GetRotatedPosition_y_CCW_rad(CrScStart.CrScPointsIn[i + CrScStart.INoAuxPoints].X, CrScStart.CrScPointsIn[CrScStart.INoAuxPoints + i].Y, DTheta_x);

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

        public Point3DCollection GetRealExternalOutlinePointsTransformedToGCS(EGCS eGCS = EGCS.eGCSLeftHanded)
        {
            // Lateral Points
            Point3DCollection points = new Point3DCollection();

            // TODO - refaktoring - skopirovane z funkcie getMeshMemberGeometry3DFromCrSc_Three a upravene parametre
            // Zmazane vnutorne body a rozlisenie medzi solid a closed cross-section

            double fy, fz;

            // Fill Mesh Positions for Start and End Section of Element - Defines Edge Points of Element

            if (CrScStart.CrScPointsOut != null) // Check that data are available
            {
                // Rotate local y and z coordinates

                for (int j = 0; j < CrScStart.INoPointsOut; j++)
                {
                    // X - start, Y, Z

                    // Set original value to the temporary variable
                    fy = CrScStart.CrScPointsOut[j].X;
                    fz = CrScStart.CrScPointsOut[j].Y;

                    // Set Member Eccentricity
                    if (EccentricityStart != null)
                    {
                        fy += EccentricityStart.MFy_local;
                        fz += EccentricityStart.MFz_local;
                    }

                    // Rotate about local x-axis
                    Geom2D.TransformPositions_CCW_rad(0f, 0f, DTheta_x, ref fy, ref fz);

                    points.Add(new Point3D(-FAlignment_Start, fy, fz));
                }

                for (int j = 0; j < CrScStart.INoPointsOut; j++)
                {
                    // X - end, Y, Z
                    if (CrScEnd == null /*|| zistit ci su objekty rovnakeho typu - triedy */)  // Check that data of second cross-section are available
                    {
                        // Set original value to the temporary variable
                        fy = CrScStart.CrScPointsOut[j].X;
                        fz = CrScStart.CrScPointsOut[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityEnd != null)
                        {
                            fy += EccentricityEnd.MFy_local;
                            fz += EccentricityEnd.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, DTheta_x, ref fy, ref fz);

                        points.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                    }
                    else
                    {
                        // Set original value to the temporary variable
                        fy = CrScEnd.CrScPointsOut[j].X;
                        fz = CrScEnd.CrScPointsOut[j].Y;

                        // Set Member Eccentricity
                        if (EccentricityEnd != null)
                        {
                            fy += EccentricityEnd.MFy_local;
                            fz += EccentricityEnd.MFz_local;
                        }

                        // Rotate about local x-axis
                        Geom2D.TransformPositions_CCW_rad(0f, 0f, DTheta_x, ref fy, ref fz);

                        points.Add(new Point3D(FLength + FAlignment_End, fy, fz));
                    }
                }
            }
            else
            {
                // Exception
            }

            // Transform coordinates from LCS to GCS
            return TransformMember_LCStoGCS(eGCS, new Point3D(NodeStart.X, NodeStart.Y, NodeStart.Z), Delta_X, Delta_Y, Delta_Z, m_dTheta_x, points);
        }

        public void CalculateMemberLimits(out double fTempMax_X, out double fTempMin_X, out double fTempMax_Y, out double fTempMin_Y, out double fTempMax_Z, out double fTempMin_Z)
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

                for (int i = 0; i < CrScStart.CrScPointsOut.Count; i++)
                {
                    // Maximum Y - coordinate
                    if (CrScStart.CrScPointsOut[i].X > fTempMax_Y)
                        fTempMax_Y = CrScStart.CrScPointsOut[i].X;
                    // Minimum Y - coordinate
                    if (CrScStart.CrScPointsOut[i].X < fTempMin_Y)
                        fTempMin_Y = CrScStart.CrScPointsOut[i].X;
                    // Maximum Z - coordinate
                    if (CrScStart.CrScPointsOut[i].Y > fTempMax_Z)
                        fTempMax_Z = CrScStart.CrScPointsOut[i].Y;
                    // Minimum Z - coordinate
                    if (CrScStart.CrScPointsOut[i].Y < fTempMin_Z)
                        fTempMin_Z = CrScStart.CrScPointsOut[i].Y;
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

        public CMemberGroup GetMemberGroupFromList(List<CMemberGroup> listOfGroups)
        {
            // TODO - toto vyhladavanie podla skupiny je dost kostrbate, lepsi by bol ENUM EMemberGroupNames
            for (int i = 0; i < listOfGroups.Count; i++)
            {
                for (int j = 0; j < listOfGroups[i].ListOfMembers.Count; j++)
                {
                    if (listOfGroups[i].ListOfMembers[j] == this)
                        return listOfGroups[i];
                }
            }

            return null; // Not found
        }


        private void SetMemberType()
        {
            switch (eMemberType_FS)
            {
                case EMemberType_FS.eMC: eMemberType_FS = EMemberType_FS.eMC;
                    break;
            }
            
        }


        public bool IsIntermediateNode(CNode node)
        {
            if (node == null) return false;
            if (NodeStart == null) return false;
            if (NodeEnd == null) return false;

            if (node == NodeStart) return false;   // ak je pociatocny, tak nie je Intermediate Node
            if (node == NodeEnd) return false; //ak je koncovy, tak nie je Intermediate Node

            double d = R3.distanceToSegment(new R3(node.X, node.Y, node.Z), new R3(NodeStart.X, NodeStart.Y, NodeStart.Z), new R3(NodeEnd.X, NodeEnd.Y, NodeEnd.Z));
            if (MathF.d_equal(d, 0, 0.0001)) return true;
            else return false;
        }

    } // End of Class CMember
}
