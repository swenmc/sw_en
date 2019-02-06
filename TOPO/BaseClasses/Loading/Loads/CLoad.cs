using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    abstract public class CLoad : CEntity3D
    {
        private ELoadCoordSystem m_eLoadCS;

        public ELoadCoordSystem ELoadCS
        {
            get { return m_eLoadCS; }
            set { m_eLoadCS = value; }
        }

        // TODO - zapracovat do GUI option uzivatelske nastavenie, aku velkost v 3D zobrazeni ma mat 1kN , 1 kN / m, 1 kN / m2 (rozne typy zatazenia, bodove, liniove, plosne)
        private float m_fDisplayin3DRatio; // Load value is in N, N/m, N/m2. Display unit is meter, so 1kN = 1 m in display units, 1000 N = 1 m, therefore is fDisplayRatio = 1/1000
        public float Displayin3DRatio
        {
            get { return m_fDisplayin3DRatio; }
            set { m_fDisplayin3DRatio = value; }
        }

        public CLoad()
        { }

        // Model of arrow or moment curve in LCS
        // Arrow in z-axis
        // Moment around y-axis
        public Model3DGroup CreateM_3D_G_SimpleLoad(Point3D p, ENLoadType eLoadType, Color cColor, float fValue, float fOpacity, DiffuseMaterial material, float fDisplayin3D_ratio = 0.001f)
        {
            Displayin3DRatio = fDisplayin3D_ratio;

            float fValueFor3D = fValue * Displayin3DRatio; // Load value to display as 3D graphical object (1 kN = 1 m, fValue is in [N] so for 1000 N = 1 m, display ratio = 1/1000)

            Model3DGroup model_gr = new Model3DGroup();

            if (eLoadType == ENLoadType.eNLT_Fx || eLoadType == ENLoadType.eNLT_Mx)
            {
                // Red
                cColor = Color.FromRgb(250, 0, 200);

                if (fValue < 0.0f)
                    cColor = Color.FromRgb(200, 0, 200);
            }
            else if (eLoadType == ENLoadType.eNLT_Fy || eLoadType == ENLoadType.eNLT_My)
            {
                // Green
                cColor = Color.FromRgb(0, 250, 200);

                if (fValue < 0.0f)
                    cColor = Color.FromRgb(0, 200, 200);
            }
            else //if (NLoadType == ENLoadType.eNLT_Fz || NLoadType == ENLoadType.eNLT_Mz)
            {
                // Blue
                cColor = Color.FromRgb(0, 200, 250);

                if (fValue < 0.0f)
                    cColor = Color.FromRgb(0, 200, 200);
            }

            fOpacity = 0.9f;
            material.Brush = new SolidColorBrush(cColor);
            material.Brush.Opacity = fOpacity;
            material.AmbientColor = cColor;
            material.Color = cColor;

            if (eLoadType == ENLoadType.eNLT_Fx || eLoadType == ENLoadType.eNLT_Fy || eLoadType == ENLoadType.eNLT_Fz) // Force
            {
                // Tip (cone height id 20% from force value)
                StraightLineArrow3D arrow = new StraightLineArrow3D(Math.Abs(fValueFor3D));
                GeometryModel3D model = new GeometryModel3D();
                MeshGeometry3D mesh = new MeshGeometry3D();

                mesh.Positions = arrow.GetArrowPoints();
                mesh.TriangleIndices = arrow.GetArrowIndices();
                model.Geometry = mesh;
                model.Material = material;
                model_gr.Children.Add(model);  // Add traight arrow
            }
            else // Moment
            {
                // Arc
                CurvedLineArrow3D cArrowArc = new CurvedLineArrow3D(new Point3D(0, 0, 0), Math.Abs(fValueFor3D), cColor, fOpacity);
                model_gr.Children.Add(cArrowArc.GetTorus3DGroup());  // Add curved segment (arc)

                // Tip (cone height is 20% from moment value)
                Arrow3DTip cArrowTip = new Arrow3DTip(0.2f * Math.Abs(fValueFor3D));

                GeometryModel3D modelTip = new GeometryModel3D();
                MeshGeometry3D meshTip = new MeshGeometry3D();

                meshTip.Positions = cArrowTip.GetArrowPoints();
                meshTip.TriangleIndices = cArrowTip.GetArrowIndices();
                modelTip.Geometry = meshTip;
                modelTip.Material = material;

                TranslateTransform3D TranslateArrowTip = new TranslateTransform3D(Math.Abs(fValueFor3D), 0, 0);

                // Translate model points from LCS of Arrow Tip to LCS of load
                modelTip.Transform = TranslateArrowTip;
                model_gr.Children.Add(modelTip); // Add tip model to arrow model group
            }

            // Transform (rotate and translate) load geometry model from LCS to GCS

            RotateTransform3D RotateModel = new RotateTransform3D();

            // Original force model is in z-axis
            // Original moment model is about y-axis

            Vector3D v = new Vector3D();
            double dRotationAngle = 0; // In degrees

            if (eLoadType == ENLoadType.eNLT_Fx)
            {
                v.Y = 1;
                v.X = v.Z = 0;

                dRotationAngle = -90;

                if (fValue < 0.0f) // If negative value change direction
                {
                    dRotationAngle = 90;
                }
            }
            else if (eLoadType == ENLoadType.eNLT_Fy)
            {
                v.X = 1;
                v.Y = v.Z = 0;

                dRotationAngle = 90;

                if (fValue < 0.0f) // If negative value change direction
                {
                    dRotationAngle = -90;
                }
            }
            else if (eLoadType == ENLoadType.eNLT_Fz)
            {
                v.X = 1;
                v.Y = v.Z = 0;

                dRotationAngle = 180;

                if (fValue < 0.0f) // If negative value change direction
                {
                    v.X = v.Y = v.Z = 0; // No Rotation
                    dRotationAngle = 0;
                }
            }
            else if (eLoadType == ENLoadType.eNLT_Mx)
            {
                v.Z = 1;
                v.X = v.Y = 0;

                dRotationAngle = 90;

                if (fValue < 0.0f) // If negative value change direction
                {
                    dRotationAngle = -90;
                }
            }
            else if (eLoadType == ENLoadType.eNLT_My)
            {
                v.X = v.Y = v.Z = 0; // No Rotation

                if (fValue < 0.0f) // If negative value change direction
                {
                    v.Z = 1;
                    v.X = v.Z = 0;

                    dRotationAngle = 180;
                }
            }
            else //if(NLoadType == ENLoadType.eNLT_Mz)
            {
                v.X = 1;
                v.Y = v.Z = 0;

                dRotationAngle = 90;

                if (fValue < 0.0f) // If negative value change direction
                {
                    dRotationAngle = -90;
                }
            }

            RotateModel.Rotation = new AxisAngleRotation3D(v, dRotationAngle);
            TranslateTransform3D TranslateModel = new TranslateTransform3D(p.X, p.Y, p.Z);

            Transform3DGroup Rottransgroup = new Transform3DGroup();
            Rottransgroup.Children.Add(RotateModel);
            Rottransgroup.Children.Add(TranslateModel);

            // Translate model from LCS to GCS
            model_gr.Transform = Rottransgroup;

            return model_gr;
        }

        virtual public Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();
            return model_gr;
        }
    }
}
