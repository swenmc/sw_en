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
        float m_fRotationX_deg;
        float m_fRotationY_deg;
        float m_fRotationZ_deg;

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
        }

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
            m_fRotationX_deg = m_fRotationX_deg_temp;
            m_fRotationY_deg = m_fRotationY_deg_temp;
            m_fRotationZ_deg = m_fRotationZ_deg_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;
        }

        public void Set3DModelColorAndMaterial()
        {
            bool bUseDifferentColorForNegativeValue = true;

            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.OrangeRed;

            if (bUseDifferentColorForNegativeValue && fValue < 0)
                m_Color = Colors.LightSkyBlue;

            m_Material3DGraphics = new DiffuseMaterial();
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_fOpacity = 0.3f;
            m_Material3DGraphics.Brush.Opacity = m_fOpacity;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Set3DModelColorAndMaterial();

            Model3DGroup model_gr = new Model3DGroup();
            CVolume volume = new CVolume();

            bool bChangePositionForNegativeValue = true;

            double fz_coord = m_pControlPoint.Z;

            if(bChangePositionForNegativeValue && fValue < 0)
               fz_coord = m_pControlPoint.Z - fValue;

            GeometryModel3D model = volume.CreateM_G_M_3D_Volume_nEdges(new Point3D(0,0,0), pSurfacePoints, fValue, m_Material3DGraphics);

            if (ELoadCS == ELoadCoordSystem.eGCS && ELoadDirection == ELoadDir.eLD_Z) // Load defined in GCS in global Z direction
            {
                // Move Coordinates in y direction
                float fy = (float)Math.Tan(m_fRotationX_deg / 180 * Math.PI) * fValue;



            }


            model_gr.Children.Add(model);

             // Create Transform3DGroup
            Transform3DGroup loadTransform3DGroup = CreateTransformCoordGroup();

            // Set the Transform property of the GeometryModel to the Transform3DGroup
            model_gr.Transform = loadTransform3DGroup;

            return model_gr;
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate model from its LCS to GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), m_fRotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_fRotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ_deg); // Rotation in degrees

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
