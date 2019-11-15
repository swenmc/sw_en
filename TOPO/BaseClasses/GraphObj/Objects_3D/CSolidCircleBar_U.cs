using MATH;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class CSolidCircleBar_U
    {
        public float m_fDiameter;
        DiffuseMaterial m_mat;

        //short iTotNoPoints = 13; // 1 auxialiary node in centroid / stredovy bod

        public CSolidCircleBar_U()
        { }

        public static Model3DGroup CreateM_3D_G_Volume_U_Bar(Point3D solidControlEdge, short nPoints, float fDiameter, DiffuseMaterial mat)
        {
            Model3DGroup models = new Model3DGroup();

            float cylinderVerticalLeft_Length = 0.075f;
            float cylinderHorizontal_Length = 0.6f;
            float cylinderVerticalRight_Length = 0.075f;

            // Kreslime v rovine XZ
            Point3D cylinderVerticalLeft_ControlPoint = new Point3D(0, 0, -cylinderVerticalLeft_Length);
            Point3D cylinderHorizontal_ControlPoint = new Point3D(0, 0, -cylinderVerticalLeft_Length);
            Point3D cylinderVerticalRight_ControlPoint = new Point3D(cylinderHorizontal_Length, 0, -cylinderVerticalLeft_Length);

            GeometryModel3D cylinderVerticalLeft = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalLeft_ControlPoint, 13, 0.5f * fDiameter, cylinderVerticalLeft_Length, mat, 2);
            GeometryModel3D cylinderHorizontal = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderHorizontal_ControlPoint, 13, 0.5f * fDiameter, cylinderHorizontal_Length, mat,0, false, false);
            GeometryModel3D cylinderVerticalRight = Cylinder.CreateM_G_M_3D_Volume_Cylinder(cylinderVerticalRight_ControlPoint, 13, 0.5f * fDiameter, cylinderVerticalRight_Length, mat, 2);

            models.Children.Add((Model3D)cylinderVerticalLeft);
            models.Children.Add((Model3D)cylinderHorizontal);
            models.Children.Add((Model3D)cylinderVerticalRight);

            return models;
        }
    }
}
