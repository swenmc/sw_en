﻿using BaseClasses.GraphObj;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace BaseClasses
{
    [Serializable]
    public class CSLoad_FreeUniform : CSLoad_Free
    {
        public float fValue;
        bool bIsFourPointBase;
        public bool bDrawPositiveValueOnPlusLocalZSide;
        public bool bChangePositionForNegativeValue;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public CSLoad_FreeUniform(
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData_temp,
            ELoadCoordSystem eLoadCS_temp,
            ELoadDirection eLoadDirection_temp,
            Point3D pControlPoint_temp,
            Point3DCollection pSurfacePoints_temp,
            float fValue_temp,
            bool bDrawPositiveValueOnPlusLocalZSide_temp,
            bool bChangePositionForNegativeValue_temp,
            bool bIsDisplayed,
            float fTime) : base(listOfLoadedMemberTypeData_temp, eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            if (pSurfacePoints_temp == null || pSurfacePoints_temp.Count < 3)
                throw new ArgumentException("Invalid Point Collection"); // Invalid collection data, unable to define surface, pridat kontrolu totoznych bodov v kolekcii

            listOfLoadedMemberTypeData = listOfLoadedMemberTypeData_temp;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDir = eLoadDirection_temp;
            ControlPoint = pControlPoint_temp;
            pSurfacePoints = pSurfacePoints_temp;
            SurfaceDefinitionPoints = new Point3DCollection(pSurfacePoints_temp);
            fValue = fValue_temp;
            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = true;  // TODO - uzivatelsky nastavitelne
            m_Color = Colors.OrangeRed;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
                m_Color = Colors.LightSkyBlue;
        }

        // Constructor used for rectangular surface (4 edges)
        public CSLoad_FreeUniform(
               List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData_temp,
               ELoadCoordSystem eLoadCS_temp,
               ELoadDirection eLoadDirection_temp,
               Point3D pControlPoint_temp,
               float fX_dimension,
               float fY_dimension,
               float fValue_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               bool bDrawPositiveValueOnPlusLocalZSide_temp,
               bool bChangePositionForNegativeValue_temp,
               bool bIsDisplayed,
               float fTime) : base(listOfLoadedMemberTypeData_temp, eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            listOfLoadedMemberTypeData = listOfLoadedMemberTypeData_temp;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDir = eLoadDirection_temp;
            ControlPoint = pControlPoint_temp;

            pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY_dimension, 0), new Point3D(0, fY_dimension, 0) };
            SurfaceDefinitionPoints = new Point3DCollection(pSurfacePoints);

            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = true;  // TODO - uzivatelsky nastavitelne
            m_Color = Colors.OrangeRed;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
                m_Color = Colors.LightSkyBlue;
        }

        // Constructor used for rectangular surface (4 edges)
        public CSLoad_FreeUniform(
               List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData_temp,
               ELoadCoordSystem eLoadCS_temp,
               ELoadDirection eLoadDirection_temp,
               Point3D pControlPoint_temp,
               float fX_dimension,
               float fY_dimension,
               float fValue_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               Color color_temp,
               bool bDrawPositiveValueOnPlusLocalZSide_temp,
               bool bChangePositionForNegativeValue_temp,
               bool bIsDisplayed,
               float fTime) : base(listOfLoadedMemberTypeData_temp, eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            listOfLoadedMemberTypeData = listOfLoadedMemberTypeData_temp;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDir = eLoadDirection_temp;
            ControlPoint = pControlPoint_temp;

            pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY_dimension, 0), new Point3D(0, fY_dimension, 0) };

            SurfaceDefinitionPoints = new Point3DCollection(pSurfacePoints);
            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = false; // TODO - uzivatelsky nastavitelne
            m_Color = color_temp;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
            {
                // TODO Ondrej - zapracovat nejaky algoritmus, ktory zmeni farbu m_Color, ak je hodnota negativna
                // Nastavit napriklad nejaky posun v skale alebo zaviest skalu farieb pre kladne (odtiene cervenej) a zaporne (odtiene modrej) hodnoty a z tych potom vyberat
                m_Color = Colors.LightSkyBlue;
            }
        }

        // Constructor used for Gable Roof Building - wall (5 edges, trapezoidal segment)
        // Constructor used for Gable Roof Building - wall (4 edges, trapezoidal segment)

        // Symmetric 5 points
        public CSLoad_FreeUniform(
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData_temp,
        ELoadCoordSystem eLoadCS_temp,
        ELoadDirection eLoadDirection_temp,
        Point3D pControlPoint_temp,
        float fX_dimension,
        float fY1_dimension,
        float fY2_dimension, // Tip coordinates
        float fValue_temp,
        float m_fRotationX_deg_temp,
        float m_fRotationY_deg_temp,
        float m_fRotationZ_deg_temp,
        Color color_temp,
        bool bDrawPositiveValueOnPlusLocalZSide_temp,
        bool bChangePositionForNegativeValue_temp,
        bool bIsFourPointBase_temp,
        bool bIsDisplayed,
        float fTime) : base(listOfLoadedMemberTypeData_temp, eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            listOfLoadedMemberTypeData = listOfLoadedMemberTypeData_temp;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDir = eLoadDirection_temp;
            ControlPoint = pControlPoint_temp;
            bIsFourPointBase = bIsFourPointBase_temp;

            if (bIsFourPointBase)
                pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY2_dimension, 0), new Point3D(0, fY1_dimension, 0) };
            else
                pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY1_dimension, 0), new Point3D(0.5f * fX_dimension, fY2_dimension, 0), new Point3D(0, fY1_dimension, 0) };

            SurfaceDefinitionPoints = new Point3DCollection(pSurfacePoints);
            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            bIsFourPointBase = bIsFourPointBase_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = false; // TODO - uzivatelsky nastavitelne
            m_Color = color_temp;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
            {
                // TODO Ondrej - zapracovat nejaky algoritmus, ktory zmeni farbu m_Color, ak je hodnota negativna
                // Nastavit napriklad nejaky posun v skale alebo zaviest skalu farieb pre kladne (odtiene cervenej) a zaporne (odtiene modrej) hodnoty a z tych potom vyberat
                m_Color = Colors.LightSkyBlue;
            }
        }

        // Asymetric 5 points
        public CSLoad_FreeUniform(
               List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData_temp,
               ELoadCoordSystem eLoadCS_temp,
               ELoadDirection eLoadDirection_temp,
               Point3D pControlPoint_temp,
               float fX_dimension,
               float fY1_dimension,
               float fX2_dimension, // Tip coordinates
               float fY2_dimension, // Tip coordinates
               float fY3_dimension,
               float fValue_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               Color color_temp,
               bool bDrawPositiveValueOnPlusLocalZSide_temp,
               bool bChangePositionForNegativeValue_temp,
               bool bIsFourPointBase_temp,
               bool bIsDisplayed,
               float fTime) : base(listOfLoadedMemberTypeData_temp, eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            listOfLoadedMemberTypeData = listOfLoadedMemberTypeData_temp;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDir = eLoadDirection_temp;
            ControlPoint = pControlPoint_temp;
            bIsFourPointBase = bIsFourPointBase_temp;

            if (bIsFourPointBase)
                pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY2_dimension, 0), new Point3D(0, fY1_dimension, 0) };
            else
                pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY1_dimension, 0), new Point3D(fX2_dimension, fY2_dimension, 0), new Point3D(0, fY3_dimension, 0) };

            SurfaceDefinitionPoints = new Point3DCollection(pSurfacePoints);
            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            bIsFourPointBase = bIsFourPointBase_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = false; // TODO - uzivatelsky nastavitelne
            m_Color = color_temp;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
            {
                // TODO Ondrej - zapracovat nejaky algoritmus, ktory zmeni farbu m_Color, ak je hodnota negativna
                // Nastavit napriklad nejaky posun v skale alebo zaviest skalu farieb pre kladne (odtiene cervenej) a zaporne (odtiene modrej) hodnoty a z tych potom vyberat
                m_Color = Colors.LightSkyBlue;
            }
        }

        public override Model3DGroup CreateM_3D_G_Load(float fDisplayin3DRatio)
        {
            m_Material3DGraphics = new DiffuseMaterial();
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_fOpacity = 0.5f;
            m_Material3DGraphics.Brush.Opacity = m_fOpacity;

            Model3DGroup model_gr = new Model3DGroup();

            if (Math.Abs(fValue) > 0) // Create and determine model data only in load value is not zero
            {
                CVolume volume = new CVolume();

                float fValueFor3D = Math.Abs(fValue) * fDisplayin3DRatio; // Load value to display as 3D graphical object, height of prism (local z-axis) (1 kN = 1 m)

                float fz_coordTop = fValueFor3D;
                float fz_coordBottom = 0;

                if (fValue > 0 && !bDrawPositiveValueOnPlusLocalZSide || (fValue < 0 && bDrawPositiveValueOnPlusLocalZSide && bChangePositionForNegativeValue))
                {
                    fz_coordTop = 0;
                    fz_coordBottom = -fValueFor3D;
                }

                pSurfacePoints_h = new Point3DCollection(pSurfacePoints.Count);

                float fy = 0.0f;

                // Todo limit 35 stupnov je tak trosku riskantny, asi by to malo byt zadane jednoznacne ci sa maju skosit hrany kvadra, resp ze normala plochy zviera s osou Z uhol mensi nez 90 stupnov
                if (ELoadCS == ELoadCoordSystem.eGCS && ELoadDir == ELoadDirection.eLD_Z && Math.Abs(RotationX_deg) < 35) // Load defined in GCS in global Z direction, rotation is less than 35 deg in absolute value - so we know that it is roof pitch angle
                {
                    // Move coordinates in y-direction
                    fy = (float)(Math.Tan(RotationX_deg / 180 * Math.PI) * fValueFor3D);
                }

                // Set point coordinates
                for (int i = 0; i < pSurfacePoints.Count; i++)
                {
                    // Top
                    Point3D pTop = new Point3D();
                    pTop = pSurfacePoints[i];
                    pTop.Z = fz_coordTop;

                    if(bDrawPositiveValueOnPlusLocalZSide || (!bDrawPositiveValueOnPlusLocalZSide && bChangePositionForNegativeValue))
                        pTop.Y += fy; // fy is currently used only for GCS and Z direction

                    pSurfacePoints_h.Add(pTop);

                    // Bottom
                    Point3D pBottom = new Point3D();
                    pBottom = pSurfacePoints[i];
                    pBottom.Z = fz_coordBottom;

                    /*
                    if (bDrawPositiveValueOnPlusLocalZSide)
                        pBottom.Y -= fy; // fy is currently used only for GCS and Z direction
                    */

                    pSurfacePoints[i] = pBottom;
                }

                //GeometryModel3D model = volume.CreateM_G_M_3D_Volume_nEdges(new Point3D(0, 0, 0), pSurfacePoints, fValue, m_Material3DGraphics);
                GeometryModel3D model = volume.CreateM_G_M_3D_Volume_nEdges(new Point3D(0, 0, 0), pSurfacePoints, pSurfacePoints_h, m_Material3DGraphics);

                model_gr.Children.Add(model);

                // Create Transform3DGroup
                Transform3DGroup loadTransform3DGroup = CreateTransformCoordGroup();

                // Set the Transform property of the GeometryModel to the Transform3DGroup
                model_gr.Transform = loadTransform3DGroup;
            }
            return model_gr;
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate model from its LCS to GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), RotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), RotationZ_deg); // Rotation in degrees

            // Move 0,0,0 to control point in GCS
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }
    }
}
