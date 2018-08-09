using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CSLoad_FreeUniform : CSLoad_Free
    {
        Point3DCollection pSurfacePoints;

        float fValue;

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public CSLoad_FreeUniform(
            ELoadCoordSystem eLoadCS_temp,
            ELoadDir eLoadDirection_temp,
            CPoint pControlPoint_temp,
            Point3DCollection pSurfacePoints_temp,
            float fValue_temp,
            bool bIsDisplayed,
            float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            if (pSurfacePoints_temp == null || pSurfacePoints_temp.Count < 3)
                throw new ArgumentException("Invalid Point Collection"); // Invalid collection data, unable to define surface, pridat kontrolu totoznych bodov v kolekcii

            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDirection = eLoadDirection_temp;
            m_pControlPoint = pControlPoint_temp;
            pSurfacePoints = pSurfacePoints_temp;
            fValue = fValue_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = true;  // TODO - uzivatelsky nastavitelne
            m_Color = Colors.OrangeRed;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
                m_Color = Colors.LightSkyBlue;
        }

        // Constructor used for rectangular surface (4 edges)
        public CSLoad_FreeUniform(
               ELoadCoordSystem eLoadCS_temp,
               ELoadDir eLoadDirection_temp,
               CPoint pControlPoint_temp,
               float fX_dimension,
               float fY_dimension,
               float fValue_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               bool bIsDisplayed,
               float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDirection = eLoadDirection_temp;
            m_pControlPoint = pControlPoint_temp;

            pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY_dimension, 0), new Point3D(0, fY_dimension, 0) };

            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            bool bUseDifferentColorForNegativeValue = true;  // TODO - uzivatelsky nastavitelne
            m_Color = Colors.OrangeRed;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
                m_Color = Colors.LightSkyBlue;
        }

        // Constructor used for rectangular surface (4 edges)
        public CSLoad_FreeUniform(
               ELoadCoordSystem eLoadCS_temp,
               ELoadDir eLoadDirection_temp,
               CPoint pControlPoint_temp,
               float fX_dimension,
               float fY_dimension,
               float fValue_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               Color color_temp,
               bool bIsDisplayed,
               float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDirection = eLoadDirection_temp;
            m_pControlPoint = pControlPoint_temp;

            pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY_dimension, 0), new Point3D(0, fY_dimension, 0) };

            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
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

        // Constructor used for Gable Roof Building - wall (5 edges)
        public CSLoad_FreeUniform(
               ELoadCoordSystem eLoadCS_temp,
               ELoadDir eLoadDirection_temp,
               CPoint pControlPoint_temp,
               float fX_dimension,
               float fY1_dimension,
               float fY2_dimension,
               float fValue_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               Color color_temp,
               bool bIsDisplayed,
               float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDirection = eLoadDirection_temp;
            m_pControlPoint = pControlPoint_temp;

            pSurfacePoints = new Point3DCollection { new Point3D(0, 0, 0), new Point3D(fX_dimension, 0, 0), new Point3D(fX_dimension, fY1_dimension, 0), new Point3D(0.5f * fX_dimension, fY2_dimension, 0), new Point3D(0, fY1_dimension, 0) };

            fValue = fValue_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;
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

        public override Model3DGroup CreateM_3D_G_Load()
        {
            m_Material3DGraphics = new DiffuseMaterial();
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_fOpacity = 0.3f;
            m_Material3DGraphics.Brush.Opacity = m_fOpacity;

            Model3DGroup model_gr = new Model3DGroup();

            if (Math.Abs(fValue) > 0) // Create and determine model data only in load value is not zero
            {
                CVolume volume = new CVolume();

                bool bChangePositionForNegativeValue = true;
                float fValueFor3D = fValue * m_fDisplayin3DRatio; // Load value to display as 3D graphical object (1 kN = 1 m)

                double fz_coord = m_pControlPoint.Z;

                if (bChangePositionForNegativeValue && fValue < 0)
                    fz_coord = m_pControlPoint.Z - fValueFor3D;

                Point3DCollection pSurfacePoints_h = new Point3DCollection(pSurfacePoints.Count);

                float fy = 0.0f;

                // Todo limit 35 stupnov je tak trosku riskantny, asi by to malo byt zadane jednoznacne ci sa maju skosit hrany kvadra, resp ze normala plochy zviera s osou Z uhol mensi nez 90 stupnov
                if (ELoadCS == ELoadCoordSystem.eGCS && ELoadDirection == ELoadDir.eLD_Z && Math.Abs(RotationX_deg) < 35) // Load defined in GCS in global Z direction, rotation is less than 35 deg in absolute value - so we know that it is roof pitch angle
                {
                    // Move coordinates in y-direction
                    fy = (float)Math.Tan(RotationX_deg / 180 * Math.PI) * fValueFor3D;
                }

                // Set point coordinates
                for (int i = 0; i < pSurfacePoints.Count; i++)
                {
                    Point3D pa = new Point3D();
                    pa = pSurfacePoints[i];

                    pa.Y += fy; // fy is currently used only for GCS and Z direction
                    pa.Z = fValueFor3D;
                    pSurfacePoints_h.Add(pa);
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
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }
    }
}
