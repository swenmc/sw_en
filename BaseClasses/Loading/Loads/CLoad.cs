using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    [Serializable]
    abstract public class CLoad : CEntity3D
    {
        public CLoad()
        { }

        // Model of arrow or moment curve in LCS
        // Arrow in z-axis
        // Moment around y-axis
        public Model3DGroup CreateM_3D_G_SimpleLoad(Point3D p, ENLoadType eLoadType, Color cColor, float fValue, float fOpacity, DiffuseMaterial material)
        {
            Model3DGroup model_gr = new Model3DGroup();

            if (eLoadType == ENLoadType.eNLT_Fx || eLoadType == ENLoadType.eNLT_Mx)
            {
                cColor = Color.FromRgb(100, 40, 40);

                if (fValue < 0.0f)
                    cColor = Color.FromRgb(150, 20, 20);
            }
            else if (eLoadType == ENLoadType.eNLT_Fy || eLoadType == ENLoadType.eNLT_My)
            {
                cColor = Color.FromRgb(40, 100, 40);

                if (fValue < 0.0f)
                    cColor = Color.FromRgb(20, 150, 20);
            }
            else //if (NLoadType == ENLoadType.eNLT_Fz || NLoadType == ENLoadType.eNLT_Mz)
            {
                cColor = Color.FromRgb(40, 40, 100);

                if (fValue < 0.0f)
                    cColor = Color.FromRgb(20, 20, 150);
            }

            fOpacity = 0.9f;
            material.Brush = new SolidColorBrush(cColor);
            material.Brush.Opacity = fOpacity;
            material.AmbientColor = cColor;
            material.Color = cColor;

            if (eLoadType == ENLoadType.eNLT_Fx || eLoadType == ENLoadType.eNLT_Fy || eLoadType == ENLoadType.eNLT_Fz) // Force
            {
                // Tip (cone height id 20% from force value)
                StraightLineArrow3D arrow = new StraightLineArrow3D(Math.Abs(fValue));
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
                CurvedLineArrow3D cArrowArc = new CurvedLineArrow3D(new Point3D(0, 0, 0), Math.Abs(fValue), cColor, fOpacity);
                model_gr.Children.Add(cArrowArc.GetTorus3DGroup());  // Add curved segment (arc)

                // Tip (cone height is 20% from moment value)
                Arrow3DTip cArrowTip = new Arrow3DTip(0.2f * Math.Abs(fValue));

                GeometryModel3D modelTip = new GeometryModel3D();
                MeshGeometry3D meshTip = new MeshGeometry3D();

                meshTip.Positions = cArrowTip.GetArrowPoints();
                meshTip.TriangleIndices = cArrowTip.GetArrowIndices();
                modelTip.Geometry = meshTip;
                modelTip.Material = material;

                TranslateTransform3D TranslateArrowTip = new TranslateTransform3D(Math.Abs(fValue), 0, 0);

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
