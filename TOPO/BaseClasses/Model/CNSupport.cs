using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CNSupport : CEntity3D
    {
        //----------------------------------------------------------------------------
        public int[] m_iNodeCollection; // List / Collection of nodes IDs where support is defined [First member index is 0]
        private CNode m_Node; // Musi sa vymazat z tejto triedy a ostane len list of nodes

        // Restraints - list of node degreess of freedom
        // false - 0 - free DOF
        // true - 1 - restrained (rigid)

        public int m_eNDOF;
        public bool[] m_bRestrain; // Array of boolean values, UX, UY, UZ, RX, RY, RZ

        //----------------------------------------------------------------------------
        public CNode Node
        {
            get { return m_Node; }
            set { m_Node = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CNSupport(int eNDOF)
        {
            m_bRestrain = new bool[(int)eNDOF];
        }

        public CNSupport(int eNDOF, int iSupport_ID, CNode Node, bool[] bRestrain, int fTime)
        {
            m_eNDOF = eNDOF;
            m_bRestrain = new bool[(int)eNDOF];
            ID = iSupport_ID;
            m_Node = Node;
            m_bRestrain = bRestrain;
            BIsDisplayed = true;
            FTime = fTime;
        }

        public CNSupport(int eNDOF, int iSupport_ID,CNode Node, bool[] bRestrain, bool bIsDisplayed, int fTime)
        {
            m_eNDOF = eNDOF;
            m_bRestrain = new bool[(int)eNDOF];
            ID = iSupport_ID;
            m_Node = Node;
            m_bRestrain = bRestrain;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        public Model3DGroup CreateM_3D_G_NSupport()
        {
            // Rozpracovat vykreslovanie pre rozne typy podpor

            Model3DGroup model_gr = new Model3DGroup();

            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(20, 250, 20));
            brush.Opacity = 0.8f;

            // We need to transform CNode to Point3D
            //Point3D pTopEdge = new Point3D(Node.X, Node.Y, Node.Z);

            // Auxialiary - to call VOLUME functions
            CVolume volaux = new CVolume();
            GeometryModel3D GeomModel3Daux = new GeometryModel3D();
            RotateTransform3D RotateTrans3D = new RotateTransform3D();
            TranslateTransform3D Translate3D = new TranslateTransform3D();

            float fa = 0.2f; // Dimension in x / y
            float fh = 0.1f; // Dimension in z

            if (m_bRestrain[(int)ENSupportType.eNST_Ux] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Uy] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Uz] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Rx] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Ry] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Rz] == false) // No restraint
            {
                return null;
            }

            if (m_bRestrain[(int)ENSupportType.eNST_Ux] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Uy] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Uz] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Rx] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Ry] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Rz] == true) // Total restraint
            {
                GeomModel3Daux = volaux.CreateM_3D_G_Volume_8Edges(new Point3D(- 0.5f * fa, - 0.5f * fa, - 0.5f * fa),fa, fa, fa, new DiffuseMaterial(brush));
                model_gr.Children.Add(GeomModel3Daux);
            }
            else if (m_bRestrain[(int)ENSupportType.eNST_Ux] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Uy] == false &&
                m_bRestrain[(int)ENSupportType.eNST_Uz] == false && (
                m_bRestrain[(int)ENSupportType.eNST_Rx] == true ||
                m_bRestrain[(int)ENSupportType.eNST_Ry] == true ||
                m_bRestrain[(int)ENSupportType.eNST_Rz] == true)) // Only rotational restraints
            {
                GeometryModel3D GeomModel3_RX = new GeometryModel3D();
                GeomModel3_RX = volaux.CreateM_3D_G_Volume_8Edges(new Point3D(- 0.5f * fa,  - 0.125f * fa,  - 0.125f * fa), fa, 0.25f * fa, 0.25f * fa, new DiffuseMaterial(brush));

                if (m_bRestrain[(int)ENSupportType.eNST_Rx] == true) // Rotational restraint around x-axis
                {
                    // LCS and GCS of support are identical, we can add model to modelgroup
                    model_gr.Children.Add(GeomModel3_RX);
                }

                if (m_bRestrain[(int)ENSupportType.eNST_Ry] == true) // Rotational restraint around y-axis
                {
                    GeometryModel3D GeomModel3_RY = new GeometryModel3D();
                    GeomModel3_RY = GeomModel3_RX;

                    // Rotate model around z-axis
                    RotateTrans3D.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 90);
                    GeomModel3_RY.Transform = RotateTrans3D;
                    model_gr.Children.Add(GeomModel3_RY);
                }

                if (m_bRestrain[(int)ENSupportType.eNST_Rz] == true) // Rotational restraint around z-axis
                {
                    GeometryModel3D GeomModel3_RZ = new GeometryModel3D();
                    GeomModel3_RZ = GeomModel3_RX;

                    // Rotate model around y-axis
                    RotateTrans3D.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);
                    GeomModel3_RZ.Transform = RotateTrans3D;
                    model_gr.Children.Add(GeomModel3_RZ);
                }
            }
            else if (m_bRestrain[(int)ENSupportType.eNST_Uz] == true)
            {
                if (m_bRestrain[(int)ENSupportType.eNST_Ux] == false)
                {
                    GeomModel3Daux = volaux.CreateM_3D_G_Volume_8Edges(fa, 0.01f, 0.01f, new DiffuseMaterial(brush));
                    Translate3D = new TranslateTransform3D(- 0.5f * fa + 0.005f, - 0.5f * fa + 0.005f, - fh - 0.05f);
                    GeomModel3Daux.Transform = Translate3D;
                    model_gr.Children.Add(GeomModel3Daux);

                    GeomModel3Daux = volaux.CreateM_3D_G_Volume_8Edges(fa, 0.01f, 0.01f, new DiffuseMaterial(brush));
                    Translate3D = new TranslateTransform3D(- 0.5f * fa + 0.005f, + 0.5f * fa - 0.005f, - fh - 0.05f);
                    GeomModel3Daux.Transform = Translate3D;
                    model_gr.Children.Add(GeomModel3Daux);
                }

                if (m_bRestrain[(int)ENSupportType.eNST_Uy] == false)
                {
                    // Rotate around z-axis
                    RotateTrans3D.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 90);
                    Transform3DGroup Trans3DGroup_1 = new Transform3DGroup();
                    Transform3DGroup Trans3DGroup_2 = new Transform3DGroup();
                    Trans3DGroup_1.Children.Add(RotateTrans3D);
                    Trans3DGroup_2.Children.Add(RotateTrans3D);

                    GeomModel3Daux = volaux.CreateM_3D_G_Volume_8Edges(fa, 0.01f, 0.01f, new DiffuseMaterial(brush));
                    Translate3D = new TranslateTransform3D( - 0.5f * fa + 0.005f,  - 0.5f * fa + 0.005f, - fh - 0.05f);
                    Trans3DGroup_1.Children.Add(Translate3D);
                    GeomModel3Daux.Transform = Trans3DGroup_1;
                    model_gr.Children.Add(GeomModel3Daux);

                    GeomModel3Daux = volaux.CreateM_3D_G_Volume_8Edges(fa, 0.01f, 0.01f, new DiffuseMaterial(brush));
                    Translate3D = new TranslateTransform3D( + 0.5f * fa - 0.005f,  - 0.5f * fa + 0.005f, - fh - 0.05f);
                    Trans3DGroup_2.Children.Add(Translate3D);
                    GeomModel3Daux.Transform = Trans3DGroup_2;
                    model_gr.Children.Add(GeomModel3Daux);
                }

                if (m_bRestrain[(int)ENSupportType.eNST_Rx] == false && m_bRestrain[(int)ENSupportType.eNST_Ry] == false)
                {
                    model_gr.Children.Add(volaux.CreateM_G_M_3D_Volume_5Edges(new Point3D(0, 0, 0), fa, fh, new DiffuseMaterial(brush)));
                    model_gr.Children.Add(volaux.CreateM_3D_G_Volume_Sphere(new Point3D(0, 0, 0), 0.04f, new DiffuseMaterial(brush)));

                    if (m_bRestrain[(int)ENSupportType.eNST_Rz] == false)
                    {
                        model_gr.Children.Add(Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, - 1.5f * fh), 0.25f * fa, 0.5f * fh, new DiffuseMaterial(brush)));
                    }
                }
                else if (m_bRestrain[(int)ENSupportType.eNST_Rx] == true || m_bRestrain[(int)ENSupportType.eNST_Ry])
                {
                    // Support in point 0,0,0
                    model_gr.Children.Add(volaux.CreateM_G_M_3D_Volume_6Edges_CN(new Point3D(0, 0, 0), fa, fh, new DiffuseMaterial(brush)));
                    GeomModel3Daux = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), 0.20f * fa, fa, new DiffuseMaterial(brush));
                    RotateTrans3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
                    Translate3D = new TranslateTransform3D(new Vector3D(0, 0.5f * fa, 0));

                    Transform3DGroup Trans3DGroup = new Transform3DGroup();
                    Trans3DGroup.Children.Add(RotateTrans3D);
                    Trans3DGroup.Children.Add(Translate3D);
                    GeomModel3Daux.Transform = Trans3DGroup;

                    model_gr.Children.Add(GeomModel3Daux);

                    if (m_bRestrain[(int)ENSupportType.eNST_Ry])
                    {
                        RotateTrans3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 90));
                        //Translate3D = new TranslateTransform3D(new Vector3D(0, 0, 0));

                        Transform3DGroup Trans3DGroup2 = new Transform3DGroup();
                        Trans3DGroup2.Children.Add(RotateTrans3D);
                        //Trans3DGroup2.Children.Add(Translate3D);

                        model_gr.Transform = Trans3DGroup2;
                    }
                }
                else
                { }
            }

            // Translate support from 0,0,0 to supported point coordinates in GCS
            Translate3D = new TranslateTransform3D(new Vector3D(Node.X, Node.Y, Node.Z));
            model_gr.Transform = Translate3D;

            return model_gr;
        }

    } // End of CNSupport class

    public class CCompare_NSupportID : IComparer
    {
        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
        public int Compare(object x, object y)
        {
            return ((CNSupport)x).ID - ((CNSupport)y).ID;
        }
    }
}
