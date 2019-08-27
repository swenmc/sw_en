using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    abstract public class CLoad : CEntity3D
    {
        private ELoadType m_eLoadType_FMTS;

        public ELoadType ELoadType_FMTS
        {
            get { return m_eLoadType_FMTS; }
            set { m_eLoadType_FMTS = value; }
        }

        private ELoadCoordSystem m_eLoadCS;

        public ELoadCoordSystem ELoadCS
        {
            get { return m_eLoadCS; }
            set { m_eLoadCS = value; }
        }

        private ELoadDirection m_eLoadDir;

        public ELoadDirection ELoadDir
        {
            get { return m_eLoadDir; }
            set { m_eLoadDir = value; }
        }

        private Vector3D m_LoadDirectionVector; // Vektor zohladnuje smer a znamienko zatazenia

        public Vector3D LoadDirectionVector
        {
            get { return m_LoadDirectionVector; }
            set { m_LoadDirectionVector = value; }
        }

        // TODO - zapracovat do GUI option uzivatelske nastavenie, aku velkost v 3D zobrazeni ma mat 1kN , 1 kN / m, 1 kN / m2 (rozne typy zatazenia, bodove, liniove, plosne)
        //private float m_fDisplayin3DRatio; // Load value is in N, N/m, N/m2. Display unit is meter, so 1kN = 1 m in display units, 1000 N = 1 m, therefore is fDisplayRatio = 1/1000
        //public float Displayin3DRatio
        //{
        //    get { return m_fDisplayin3DRatio; }
        //    set { m_fDisplayin3DRatio = value; }
        //}

        public CLoad()
        { }

        // Funkcia urci z enumu pre smer zatazenia a znamienka hodnoty zatazenia vektor smeru zatazenia
        public void SetLoadDirectionVector(ELoadDirection eld, float fLoadValue, out Vector3D vOutloadDirectionVector)
        {
            int ix_LCSorGCS = 0;
            int iy_LCSorGCS = 0;
            int iz_LCSorGCS = 0;

            if (eld == ELoadDirection.eLD_X)
            {
                ix_LCSorGCS = fLoadValue < 0.0f ? -1 : 1;
            }
            else if (eld == ELoadDirection.eLD_Y)
            {
                iy_LCSorGCS = fLoadValue < 0.0f ? -1 : 1;
            }
            else if (eld == ELoadDirection.eLD_Z)
            {
                iz_LCSorGCS = fLoadValue < 0.0f ? -1 : 1;
            }
            else
            {
                // Un-defined direction
            }

            // Validacia
            if (ix_LCSorGCS == 0 && iy_LCSorGCS == 0 && iz_LCSorGCS == 0)
            {
                // Vektor nie je definovany - zatazenie ma nulovu hodnotu alebo sa nejedna o silove zatazenie alebo je definicia zatazenia chybna

                if (MATH.MathF.d_equal(fLoadValue, 0)) // Ak je zatazenie nulove nastavime default ako kladny smer z
                    iz_LCSorGCS = 1;

                if (ELoadType_FMTS == ELoadType.eLT_F || ELoadType_FMTS == ELoadType.eLT_M) // Silove alebo momentove zatazenie musi mat urceny vektor
                    throw new Exception("Load direction is not defined.");
            }

            // Set load direction vector
            vOutloadDirectionVector = new Vector3D(ix_LCSorGCS, iy_LCSorGCS, iz_LCSorGCS);
        }

        public void SetLoadDirectionVector(float fLoadValue)
        {
            Vector3D vOutloadDirectionVector;
            SetLoadDirectionVector(ELoadDir, fLoadValue, out vOutloadDirectionVector);

            LoadDirectionVector = vOutloadDirectionVector;
        }

        // Funkcia urci z vektora smeru zatazenia smer a znamienko pre hodnotu zatazenia
        public void GetLoadDirectionAndValueSign(Vector3D vLoadDirectionVector, out float fLoadValueSignFactor, out ELoadDirection eld)
        {
            // Validacia
            if (MATH.MathF.d_equal(vLoadDirectionVector.X, 0) &&
                MATH.MathF.d_equal(vLoadDirectionVector.Y, 0) &&
                MATH.MathF.d_equal(vLoadDirectionVector.Z, 0))
            {
                // Vektor nie je definovany - zatazenie ma nulovu hodnotu alebo sa nejedna o silove zatazenie alebo je definicia zatazenia chybna
                fLoadValueSignFactor = 1;
                eld = ELoadDirection.eLD_Z;

                if (ELoadType_FMTS == ELoadType.eLT_F || ELoadType_FMTS == ELoadType.eLT_M) // Silove alebo momentove zatazenie musi mat urceny vektor
                    throw new Exception("Load direction is not defined.");
            }

            double x_LCSorGCS = vLoadDirectionVector.X;
            double y_LCSorGCS = vLoadDirectionVector.Y;
            double z_LCSorGCS = vLoadDirectionVector.Z;

            if (!MATH.MathF.d_equal(x_LCSorGCS, 0))
            {
                eld = ELoadDirection.eLD_X;
                fLoadValueSignFactor = x_LCSorGCS < 0 ? -1 : 1;
            }
            else if (!MATH.MathF.d_equal(y_LCSorGCS, 0))
            {
                eld = ELoadDirection.eLD_Y;
                fLoadValueSignFactor = y_LCSorGCS < 0 ? -1 : 1;
            }
            else if (!MATH.MathF.d_equal(z_LCSorGCS, 0))
            {
                eld = ELoadDirection.eLD_Z;
                fLoadValueSignFactor = z_LCSorGCS < 0 ? -1 : 1;
            }
            else
            {
                // Un-defined direction
                fLoadValueSignFactor = 1;
                eld = ELoadDirection.eLD_Z;
            }
        }

        public void GetLoadDirectionAndValueSign(out float fLoadValueSignFactor, out ELoadDirection eld)
        {
            GetLoadDirectionAndValueSign(LoadDirectionVector, out fLoadValueSignFactor, out eld);
        }

        public void SetLoadDirectionAndValueSign(out float fLoadValueSignFactor)
        {
            ELoadDirection eld;
            GetLoadDirectionAndValueSign(out fLoadValueSignFactor, out eld);
            ELoadDir = eld; // Set load direction
        }

        // Model of arrow or moment curve in LCS
        // Arrow in z-axis
        // Moment around y-axis
        public Model3DGroup CreateM_3D_G_SimpleLoad(Point3D p, ENLoadType eLoadType, Color cColor, float fValue, float fOpacity, DiffuseMaterial material, float fDisplayin3D_ratio)
        {
            //Displayin3DRatio = fDisplayin3D_ratio;

            float fValueFor3D = fValue * fDisplayin3D_ratio; // Load value to display as 3D graphical object (1 kN = 1 m, fValue is in [N] so for 1000 N = 1 m, display ratio = 1/1000)

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
                // Tip (cone height is 20% from force value)
                StraightLineArrow3D arrow = new StraightLineArrow3D(new Point3D(0,0,0),Math.Abs(fValueFor3D));
                GeometryModel3D model = new GeometryModel3D();
                MeshGeometry3D mesh = new MeshGeometry3D();

                mesh.Positions = arrow.ArrowPoints;
                mesh.TriangleIndices = arrow.GetArrowIndices();
                model.Geometry = mesh;
                model.Material = material;
                model_gr.Children.Add(model);  // Add straight arrow
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

        virtual public Model3DGroup CreateM_3D_G_Load(float fDisplayin3D_ratio)
        {
            Model3DGroup model_gr = new Model3DGroup();
            return model_gr;
        }

        virtual public Model3DGroup CreateM_3D_G_Load(bool bConsiderCrossSectionDimensions, float fDisplayin3D_ratio)
        {
            Model3DGroup model_gr = new Model3DGroup();
            return model_gr;
        }
    }
}
