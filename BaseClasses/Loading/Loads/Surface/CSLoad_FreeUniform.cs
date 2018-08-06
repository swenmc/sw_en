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

        public CSLoad_FreeUniform(ELoadCoordSystem eLoadCS_temp, CPoint pControlPoint_temp, Point3DCollection pSurfacePoints_temp, float fValue_temp, bool bIsDisplayed, float fTime) : base(eLoadCS_temp, bIsDisplayed, fTime)
        {
            if (pSurfacePoints_temp == null || pSurfacePoints_temp.Count < 3)
                throw new ArgumentException("Invalid Point Collection"); // Invalid collection data, unable to define surface, pridat kontrolu totoznych bodov v kolekcii

            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            m_pControlPoint = pControlPoint_temp;
            pSurfacePoints = pSurfacePoints_temp;
            fValue = fValue_temp;
            BIsDisplayed = BIsDisplayed;
            FTime = fTime;

            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.Cyan;
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_Material3DGraphics.Brush.Opacity = 0.9f;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();

            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.Cyan;
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_Material3DGraphics.Brush.Opacity = m_fOpacity = 0.9f;

            CVolume volume = new CVolume();

            double fz_coord = fValue > 0 ? m_pControlPoint.Z : m_pControlPoint.Z - fValue;
            GeometryModel3D model = volume.CreateM_G_M_3D_Volume_nEdges(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, fz_coord), pSurfacePoints, fValue, m_Material3DGraphics);

            model_gr.Children.Add(model);

            // TODO - zapracovat presun a pootocenie zatazenia z LCS do GCS

            // Trasnform position of load
            TranslateTransform3D translate = new TranslateTransform3D(0, 0, 0);

            AxisAngleRotation3D p = new AxisAngleRotation3D(new Vector3D(0, 1, 0), -15);
            RotateTransform3D rotateTransform = new RotateTransform3D(p, new Point3D(0,0,4));

            // Add the transform to a Transform3DGroup
            Transform3DGroup loadTransform3DGroup = new Transform3DGroup();
            loadTransform3DGroup.Children.Add(translate);
            loadTransform3DGroup.Children.Add(rotateTransform);

            // Set the Transform property of the GeometryModel to the Transform3DGroup
            model_gr.Transform = loadTransform3DGroup;

            return model_gr;
        }
    }
}
