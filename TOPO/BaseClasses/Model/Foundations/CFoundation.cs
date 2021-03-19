﻿using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // Class CFoundation
    [Serializable]
    public class CFoundation : CVolume
    {
        public EFoundationType eFoundationType;
        public CNode m_Node;
        public EMemberType_FS_Position m_ColumnMemberTypePosition;

        private float m_EccentricityBasicColumn_x; //y
        private float m_EccentricityBasicColumn_y; //z
        private float m_Eccentricity_x;
        private float m_Eccentricity_y;
        private float m_RotationAboutZ_deg;

        //private bool m_UseStraightReinforcementBars; // Tu to nema zmysel nastavovat lebo do objektu uz vstupuju reference bars, takze uz pri ich vytvoreni / zmene musi byt jasne ci maju byt priame alebo U

        private CReinforcementBar m_Reference_Top_Bar_x;
        private CReinforcementBar m_Reference_Top_Bar_y;
        private CReinforcementBar m_Reference_Bottom_Bar_x;
        private CReinforcementBar m_Reference_Bottom_Bar_y;

        private List<CReinforcementBar> m_Top_Bars_x;
        private List<CReinforcementBar> m_Top_Bars_y;
        private List<CReinforcementBar> m_Bottom_Bars_x;
        private List<CReinforcementBar> m_Bottom_Bars_y;

        private int m_Count_Top_Bars_x;
        private int m_Count_Top_Bars_y;
        private int m_Count_Bottom_Bars_x;
        private int m_Count_Bottom_Bars_y;

        private float m_fDistanceOfBars_Top_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Top_y_SpacingInxDirection;
        private float m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Bottom_y_SpacingInxDirection;

        private float m_fConcreteCover;

        private CJointDesignDetails m_DesignDetails;
        public CJointDesignDetails DesignDetails
        {
            get { return m_DesignDetails; }
            set { m_DesignDetails = value; }
        }

        public float EccentricityBasicColumn_x
        {
            get
            {
                return m_EccentricityBasicColumn_x;
            }

            set
            {
                m_EccentricityBasicColumn_x = value;
            }
        }

        public float EccentricityBasicColumn_y
        {
            get
            {
                return m_EccentricityBasicColumn_y;
            }

            set
            {
                m_EccentricityBasicColumn_y = value;
            }
        }

        public float Eccentricity_x
        {
            get
            {
                return m_Eccentricity_x;
            }

            set
            {
                m_Eccentricity_x = value;
            }
        }

        public float Eccentricity_y
        {
            get
            {
                return m_Eccentricity_y;
            }

            set
            {
                m_Eccentricity_y = value;
            }
        }

        public float RotationAboutZ_deg
        {
            get
            {
                return m_RotationAboutZ_deg;
            }

            set
            {
                m_RotationAboutZ_deg = value;
            }
        }

        /*
        public bool UseStraightReinforcementBars
        {
            get
            {
                return m_UseStraightReinforcementBars;
            }

            set
            {
                m_UseStraightReinforcementBars = value;
            }
        }
        */

        public CReinforcementBar Reference_Top_Bar_x
        {
            get
            {
                return m_Reference_Top_Bar_x;
            }

            set
            {
                m_Reference_Top_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_y
        {
            get
            {
                return m_Reference_Top_Bar_y;
            }

            set
            {
                m_Reference_Top_Bar_y = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_x
        {
            get
            {
                return m_Reference_Bottom_Bar_x;
            }

            set
            {
                m_Reference_Bottom_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_y
        {
            get
            {
                return m_Reference_Bottom_Bar_y;
            }

            set
            {
                m_Reference_Bottom_Bar_y = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_x
        {
            get
            {
                return m_Top_Bars_x;
            }

            set
            {
                m_Top_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_y
        {
            get
            {
                return m_Top_Bars_y;
            }

            set
            {
                m_Top_Bars_y = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_x
        {
            get
            {
                return m_Bottom_Bars_x;
            }

            set
            {
                m_Bottom_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_y
        {
            get
            {
                return m_Bottom_Bars_y;
            }

            set
            {
                m_Bottom_Bars_y = value;
            }
        }

        public int Count_Top_Bars_x
        {
            get
            {
                return m_Count_Top_Bars_x;
            }

            set
            {
                m_Count_Top_Bars_x = value;
            }
        }

        public int Count_Top_Bars_y
        {
            get
            {
                return m_Count_Top_Bars_y;
            }

            set
            {
                m_Count_Top_Bars_y = value;
            }
        }

        public int Count_Bottom_Bars_x
        {
            get
            {
                return m_Count_Bottom_Bars_x;
            }

            set
            {
                m_Count_Bottom_Bars_x = value;
            }
        }

        public int Count_Bottom_Bars_y
        {
            get
            {
                return m_Count_Bottom_Bars_y;
            }

            set
            {
                m_Count_Bottom_Bars_y = value;
            }
        }

        public float DistanceOfBars_Top_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Top_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_y_SpacingInxDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_y_SpacingInxDirection = value;
            }
        }

        public float ConcreteCover
        {
            get
            {
                return m_fConcreteCover;
            }

            set
            {
                m_fConcreteCover = value;
            }
        }

        public CFoundation()
        {
        }

        // Rectangular prism
        public CFoundation(int iFoundation_ID,
            EFoundationType eType,
            CNode node,
            MATERIAL.CMat_02_00 materialConcrete,
            EMemberType_FS_Position memberTypePosition,
            string sName,
            string descriptionText,
            //Point3D pControlEdgePoint,
            float fX,
            float fY,
            float fZ,
            float feBasicColumn_x,//y
            float feBasicColumn_y,//y
            float ex,
            float ey,
            float rotationAboiutZInDeg,
            float fConcreteCover,
            CReinforcementBar refTopBar_x,
            CReinforcementBar refTopBar_y,
            CReinforcementBar refBottomBar_x,
            CReinforcementBar refBottomBar_y,
            int iNumberOfBarsTop_x,
            int iNumberOfBarsTop_y,
            int iNumberOfBarsBottom_x,
            int iNumberOfBarsBottom_y,
            //Color volColor,
            float fvolOpacity,
            bool bIsDisplayed,
            float fTime)
        {
            ID = iFoundation_ID;
            eFoundationType = eType;
            m_Node = node; // Note that is assigned to the foundation / footing pad
            m_Mat = materialConcrete;
            m_ColumnMemberTypePosition = memberTypePosition;
            Name = sName;
            Text = descriptionText;
            //m_pControlPoint = pControlEdgePoint;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_fVolume = fX * fY * fZ; // !!! PLATI LEN PRE KVADER
            m_EccentricityBasicColumn_x = feBasicColumn_x;
            m_EccentricityBasicColumn_y = feBasicColumn_y;
            m_Eccentricity_x = ex;
            m_Eccentricity_y = ey;
            m_RotationAboutZ_deg = rotationAboiutZInDeg;
            m_fConcreteCover = fConcreteCover;
            m_Reference_Top_Bar_x = refTopBar_x;
            m_Reference_Top_Bar_y = refTopBar_y;
            m_Reference_Bottom_Bar_x = refBottomBar_x;
            m_Reference_Bottom_Bar_y = refBottomBar_y;
            m_Count_Top_Bars_x = iNumberOfBarsTop_x;
            m_Count_Top_Bars_y = iNumberOfBarsTop_y;
            m_Count_Bottom_Bars_x = iNumberOfBarsBottom_x;
            m_Count_Bottom_Bars_y = iNumberOfBarsBottom_y;
            //m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            // Set control point coordinates - joint node [0,0,0]
            //m_pControlPoint.X = -0.5 * fX + ex;
            //m_pControlPoint.Y = -0.5 * fY + ey;
            //m_pControlPoint.Z = -fZ;
            SetControlPoint();

            CreateReinforcementBars();

            SetTextPoint(0.2f);
        }

        public void SetTextPoint(float textSize)
        {
            // V systeme GCS
            //float fCornerPointOffsetX = 0.3f;
            //float fCornerPointOffsetY = 0.2f;

            float fOffsetX = m_fDim1 + m_EccentricityBasicColumn_x + m_Eccentricity_x + textSize;
            float fOffsetY = m_fDim2 + m_EccentricityBasicColumn_y + m_Eccentricity_y + textSize;
            float fOffsetFromPlane = m_fDim3 + 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

            PointText = new Point3D()
            {
                X = fOffsetX,
                Y = fOffsetY,
                Z = fOffsetFromPlane
            };
        }

        public void SetControlPoint()
        {
            ControlPoint = new Point3D()
            {
                X = -0.5 * m_fDim1 + m_EccentricityBasicColumn_x + m_Eccentricity_x,
                Y = -0.5 * m_fDim2 + m_EccentricityBasicColumn_y + m_Eccentricity_y,
                Z = -m_fDim3
            };
        }

        //public /*override*/ GeometryModel3D CreateGeomModel3D()
        //{
        //    return CreateGeomModel3D(new SolidColorBrush(m_volColor_2));
        //}

        //public /*override*/ GeometryModel3D CreateGeomModel3D(Color colorBrush)
        //{
        //    return CreateGeomModel3D(new SolidColorBrush(colorBrush));
        //}

        public /*override*/ GeometryModel3D CreateGeomModel3D(Color color, float fOpacity)
        {
            m_volColor_2 = color;
            Visual_Object = CreateGeomModel3D(new SolidColorBrush(m_volColor_2), fOpacity);
            return Visual_Object;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(SolidColorBrush brush, float fOpacity)
        {
            brush.Opacity = fOpacity; // Set brush opacity // TODO - mozeme nejako vypesit a prepojit s GUI, aby to bol piamo parameter zadavany spolu s farbou zakladu

            GeometryModel3D model = new GeometryModel3D();

            // TODO - pohrat sa s nastavenim farieb
            DiffuseMaterial qDiffTrans = new DiffuseMaterial(brush);
            SpecularMaterial qSpecTrans = new SpecularMaterial(brush/*new SolidColorBrush(Color.FromArgb(210, 210, 210, 210))*/, 90.0);

            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTrans);
            qOuterMaterial.Children.Add(qSpecTrans);

            if (eFoundationType == EFoundationType.ePad)
            {
                // Create foundation block - origin [0,0,0]
                CVolume volume = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, new Point3D(0,0,0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial, true, 0);
                //model = volume.CreateGM_3D_Volume_8Edges(volume);
                model = volume.CreateM_3D_G_Volume_8Edges(new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial);

                // Set the Transform property of the GeometryModel to the Transform3DGroup
                // Nastavim vyslednu transformaciu
                model.Transform = GetFoundationTransformGroup_Complete();

                // Naplnime pole bodov wireFrame
                // TODO - Ondrej - chcelo by to nejako elegantne zjednotit u vsetkych objektov ktore maju 3D geometriu kde a ako ziskavat wireframe
                // TODO Ondrej - tu chyba v tom ze beriem pozicie z povodneho zakladu nie z posunuteho do finalnej pozicie
                volume.SetWireFramePoints_Volume(model);
                WireFramePoints = volume.WireFramePoints;
            }
            else
            {
                throw new NotImplementedException();
            }
            Visual_Object = model;
            
            return model;
        }

        private float GetDistanceBetweenReinforcementBars(float footingPadWidth, int iNumberOfBarsPerSection, float fBarDiameter, float fPerpendicularBarDiameter, float fConcreteCover, bool bIsPerpendicularStraightBar)
        {
            // Odpocitavam 2 priemery kolmych prutov, kedze sa ocakavaju aj zvisle casti prutov, ak je vystuz len horizontalna ma sa odpocitat len jeden priemer
            int iNumberOfDiameters = bIsPerpendicularStraightBar ? 0 : 2;
            return (footingPadWidth - 2 * fConcreteCover - iNumberOfDiameters * fPerpendicularBarDiameter - fBarDiameter) / (iNumberOfBarsPerSection - 1);
        }

        public void CreateReinforcementBars()
        {
            // Fill 4 list of reinforcement bars
            m_fDistanceOfBars_Top_x_SpacingInyDirection = 0;
            m_Top_Bars_x = null;
            m_fDistanceOfBars_Top_y_SpacingInxDirection = 0;
            m_Top_Bars_y = null;
            m_fDistanceOfBars_Bottom_x_SpacingInyDirection = 0;
            m_Bottom_Bars_x = null;
            m_fDistanceOfBars_Bottom_y_SpacingInxDirection = 0;
            m_Bottom_Bars_y = null;

            if (Count_Top_Bars_x > 0)
            {
                m_fDistanceOfBars_Top_x_SpacingInyDirection = GetDistanceBetweenReinforcementBars(m_fDim2, Count_Top_Bars_x, Reference_Top_Bar_x.Diameter, Reference_Top_Bar_y.Diameter, ConcreteCover, Reference_Top_Bar_y is CReinforcementBarStraight);
                m_Top_Bars_x = GetReinforcementBarsOneLayer(true, m_Count_Top_Bars_x, Reference_Top_Bar_x, m_fDistanceOfBars_Top_x_SpacingInyDirection);
            }

            if (Count_Top_Bars_y > 0)
            {
                m_fDistanceOfBars_Top_y_SpacingInxDirection = GetDistanceBetweenReinforcementBars(m_fDim1, Count_Top_Bars_y, Reference_Top_Bar_y.Diameter, Reference_Top_Bar_x.Diameter, ConcreteCover, Reference_Top_Bar_x is CReinforcementBarStraight);
                m_Top_Bars_y = GetReinforcementBarsOneLayer(false, m_Count_Top_Bars_y, Reference_Top_Bar_y, m_fDistanceOfBars_Top_y_SpacingInxDirection);
            }

            if (Count_Bottom_Bars_x > 0)
            {
                m_fDistanceOfBars_Bottom_x_SpacingInyDirection = GetDistanceBetweenReinforcementBars(m_fDim2, Count_Bottom_Bars_x, Reference_Bottom_Bar_x.Diameter, Reference_Bottom_Bar_y.Diameter, ConcreteCover, Reference_Bottom_Bar_y is CReinforcementBarStraight);
                m_Bottom_Bars_x = GetReinforcementBarsOneLayer(true, m_Count_Bottom_Bars_x, Reference_Bottom_Bar_x, m_fDistanceOfBars_Bottom_x_SpacingInyDirection);
            }

            if (Count_Bottom_Bars_y > 0)
            {
                m_fDistanceOfBars_Bottom_y_SpacingInxDirection = GetDistanceBetweenReinforcementBars(m_fDim1, Count_Bottom_Bars_y, Reference_Bottom_Bar_y.Diameter, Reference_Bottom_Bar_x.Diameter, ConcreteCover, Reference_Bottom_Bar_x is CReinforcementBarStraight);
                m_Bottom_Bars_y = GetReinforcementBarsOneLayer(false, m_Count_Bottom_Bars_y, Reference_Bottom_Bar_y, m_fDistanceOfBars_Bottom_y_SpacingInxDirection);
            }
        }

        public List<CReinforcementBar> GetReinforcementBarsOneLayer(bool bBarIsInXDirection, int iCount_Bars_x, CReinforcementBar referenceBar, float fDistanceOfBars)
        {
            // Create liest of one layer of bar objects
            if (iCount_Bars_x > 1)
            {
                List<CReinforcementBar> list = new List<CReinforcementBar>();

                double cp_X_coordinate = referenceBar.ControlPoint.X; // Set first bar control point
                double cp_Y_coordinate = referenceBar.ControlPoint.Y;

                for (int i = 0; i < iCount_Bars_x; i++)
                {
                    // Nastavit control point pre reinforcement bar - zohladnit posun voci [0,0,0]
                    // TODO - zohladnit pootocenie celeho zakladu

                    // m_pControlPoint bod kam je vlozene [0,0,0] celeho zakladu v GCS
                    // referenceBar.ControlPoint bod kam je vlozena prva tyc do zakladu v LCS zakladu
                    //Point3D controlPoint = new Point3D(i + 1,
                    //    m_pControlPoint.X + cp_X_coordinate,
                    //    m_pControlPoint.Y + cp_Y_coordinate,
                    //    m_pControlPoint.Z + referenceBar.ControlPoint.Z,
                    //    0);

                    Point3D controlPoint = new Point3D(
                        ControlPoint.X + cp_X_coordinate,
                        ControlPoint.Y + cp_Y_coordinate,
                        ControlPoint.Z + referenceBar.ControlPoint.Z);

                    if (referenceBar is CReinforcementBar_U)
                    {
                        CReinforcementBar_U refBar = (CReinforcementBar_U)referenceBar;

                        list.Add(new CReinforcementBar_U(i + 1,
                            "500E",
                            referenceBar.Name,
                            bBarIsInXDirection,
                            controlPoint,
                            referenceBar.TotalLength, // Length
                            refBar.ArcRadiusNet,
                            referenceBar.Diameter, // Diameter
                            //referenceBar.m_volColor_2,
                            referenceBar.Opacity,
                            refBar.IsTop_U,
                            referenceBar.BIsDisplayed,
                            referenceBar.FTime));
                    }
                    else
                    {
                        list.Add(new CReinforcementBarStraight(i + 1,
                             "500E",
                             referenceBar.Name,
                             bBarIsInXDirection,
                             controlPoint,
                             referenceBar.TotalLength, // Length
                             referenceBar.Diameter, // Diameter
                             //referenceBar.m_volColor_2,
                             referenceBar.Opacity,
                             referenceBar.BIsDisplayed,
                             referenceBar.FTime));
                    }

                    // Set next bar control point coordinates
                    if (bBarIsInXDirection) // Change control Point Coordinate Y
                    {
                        cp_Y_coordinate += fDistanceOfBars;
                    }
                    else // Change control Point Coordinate X
                    {
                        cp_X_coordinate += fDistanceOfBars;
                    }
                }

                return list;
            }
            else
            { return null; }
        }

        public Transform3DGroup GetFoundationTransformGroup()
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            // Rotate about Z axis - otocime patku okolo [0,0,0]
            // Create and apply a transformation that rotates the object.
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 1);
            myAxisAngleRotation3d.Angle = RotationAboutZ_deg;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            // Add the rotation transform to a Transform3DGroup
            myTransform3DGroup.Children.Add(myRotateTransform3D);

            // Presun celehho zakladu do GCS
            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D_GCS = new TranslateTransform3D(m_Node.X, m_Node.Y, m_Node.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D_GCS);

            return myTransform3DGroup;
        }

        public Transform3DGroup GetFoundationTransformGroup_Complete()
        {
            // Tato cast prva translate zlozka transformacie sa na reinforcement bars nepouzije, lebo pri vytvarani vyztuze sa uz uvazuje upraveny m_pControlPoint
            // Model Transformation
            // Apply multiple transformations to the object.
            // Presun v ramci LCS (tak ze [0,0,0] bude v mieste, kde je joinNode
            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            Transform3DGroup myTransform3DGroup = new Transform3DGroup();
            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D);

            // Pridam transformaciu zakladu do GCS
            myTransform3DGroup.Children.Add(GetFoundationTransformGroup());

            return myTransform3DGroup;
        }
    }
}
