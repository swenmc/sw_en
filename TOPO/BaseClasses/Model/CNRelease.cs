using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CNRelease : CEntity3D
    {
        //---------------------------------------------------------------------------------
        private CNode m_Node;  // Musi sa vymazat z tejto triedy a ostane len list of nodes
        private CMember m_Member;  // Musi sa vymazat z tejto triedy a ostane len list of member
        public int [] m_iMembCollection; // List / Collection of members IDs where release is defined
        public int [] m_iNodeCollection; // List / Collection of nodes IDs where release is defined (need for local stiffeness matrix)
        public bool m_nRelease1;  // true - release in start point of member, false - release in end point
        public int m_eNDOF;
        public bool?[] m_bRestrain; // DOF is rigid - 1, DOF is free - 0

        //---------------------------------------------------------------------------------
        public CNode Node
        {
            get { return m_Node; }
            set { m_Node = value; }
        }
        public CMember Member
        {
            get { return m_Member; }
            set { m_Member = value; }
        }

        //---------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------
        public CNRelease(CNode node, CMember member)
        {
            m_Node = node;
            m_Member = member;
            BIsDisplayed = true;
        }

        public CNRelease(CNode node)
        {
            m_Node = node;
            BIsDisplayed = true;
        }

        public CNRelease(int eNDOF, bool?[] bRestrain, int fTime)
        {
            m_eNDOF = eNDOF;
            m_bRestrain = bRestrain;
            FTime = fTime;
            BIsDisplayed = true;
            FTime = fTime;
        }

        public CNRelease(int eNDOF, CNode Node, bool?[] bRestrain, int fTime)
        {
            m_eNDOF = eNDOF;
            m_Node = Node;
            m_bRestrain = bRestrain;
            BIsDisplayed = true;
            FTime = fTime;
        }

        public CNRelease(int eNDOF, CNode Node, CMember Member, bool?[] bRestrain, int fTime)
        {
            m_eNDOF = eNDOF;
            m_Node = Node;
            m_Member = Member;
            m_bRestrain = bRestrain;
            BIsDisplayed = true;
            FTime = fTime;
        }

        //---------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------

        public Model3DGroup CreateM_3D_G_MNRelease()
        {
            // Rozpracovat vykreslovanie pre rozne typy klbov

            Model3DGroup model_gr = new Model3DGroup();

            SolidColorBrush brush_Sphere = new SolidColorBrush(Color.FromRgb(200, 250, 250));

            SolidColorBrush brushX = new SolidColorBrush(Color.FromRgb(150, 0, 0));
            SolidColorBrush brushY = new SolidColorBrush(Color.FromRgb(0, 50, 0));
            SolidColorBrush brushZ = new SolidColorBrush(Color.FromRgb(0, 0, 100));

            brush_Sphere.Opacity = 0.8f;

            // Auxialiary - to call VOLUME functions
            CVolume volaux = new CVolume();
            //GeometryModel3D GeomModel3Daux = new GeometryModel3D();
            //RotateTransform3D RotateTrans3D = new RotateTransform3D();
            TranslateTransform3D Translate3D = new TranslateTransform3D();

            float fr_s = 0.05f; // Radius of basic shape - sphere
            float fa = 3 * fr_s; // Dimension - translation DOF dimension
            float fb = 0.1f * fa; // Dimension of base sides
            float fa_c = 2.5f * fr_s; // Dimension of cylinder - height of cylinder
            float fr_c = 0.3f * fr_s; // Radius of cylinder (rotational DOF)

            if (m_bRestrain[(int)ENSupportType.eNST_Ux] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Uy] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Uz] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Rx] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Ry] == true &&
                m_bRestrain[(int)ENSupportType.eNST_Rz] == true) // Total restraint
            {
                // No release - nothing to display
                return null;
            }
            else
            {
                // Basic sphere
                model_gr.Children.Add(volaux.CreateM_3D_G_Volume_Sphere(new Point3D(0, 0, 0), fr_s, new DiffuseMaterial(brush_Sphere)));

                // Rotational release around local x-axis
                if (m_bRestrain[(int)ENSupportType.eNST_Rx] == false)
                {
                    GeometryModel3D GeomModel3_RX = new GeometryModel3D();
                    GeomModel3_RX = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), 73, fr_c, fa_c, new DiffuseMaterial(brushX));

                    Transform3DGroup Trans3DGroup = new Transform3DGroup();
                    RotateTransform3D RotateTrans3D_AUX = new RotateTransform3D();
                    TranslateTransform3D Translate3D_AUX;

                    // Rotate around y-axis
                    RotateTrans3D_AUX.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);
                    Trans3DGroup.Children.Add(RotateTrans3D_AUX);
                    Translate3D_AUX = new TranslateTransform3D(-0.5f * fa_c, 0f, 0f);
                    Trans3DGroup.Children.Add(Translate3D_AUX);
                    GeomModel3_RX.Transform = Trans3DGroup;
                    model_gr.Children.Add(GeomModel3_RX); // Add object to release model group
                }

                // Rotational release around local y-axis
                if (m_bRestrain[(int)ENSupportType.eNST_Ry] == false)
                {
                    GeometryModel3D GeomModel3_RY = new GeometryModel3D();
                    GeomModel3_RY = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), fr_c, fa_c, new DiffuseMaterial(brushY));

                    Transform3DGroup Trans3DGroup = new Transform3DGroup();
                    RotateTransform3D RotateTrans3D_AUX = new RotateTransform3D();
                    TranslateTransform3D Translate3D_AUX;

                    // Rotate around y-axis
                    RotateTrans3D_AUX.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90);
                    Trans3DGroup.Children.Add(RotateTrans3D_AUX);
                    Translate3D_AUX = new TranslateTransform3D(0, 0.5f * fa_c, 0f);
                    Trans3DGroup.Children.Add(Translate3D_AUX);
                    GeomModel3_RY.Transform = Trans3DGroup;
                    model_gr.Children.Add(GeomModel3_RY); // Add object to release model group
                }

                // Rotational release around local z-axis
                if (m_bRestrain[(int)ENSupportType.eNST_Rz] == false)
                {
                    GeometryModel3D GeomModel3_RZ = new GeometryModel3D();
                    TranslateTransform3D Translate3D_AUX;
                    GeomModel3_RZ = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0, 0, 0), fr_c, fa_c, new DiffuseMaterial(brushZ));
                    Translate3D_AUX = new TranslateTransform3D(0, 0, -0.5f * fa_c);
                    GeomModel3_RZ.Transform = Translate3D_AUX;
                    model_gr.Children.Add(GeomModel3_RZ); // Add object to release model group
                }

                // Translation release in local x-axis
                if (m_bRestrain[(int)ENSupportType.eNST_Ux] == false)
                {
                    GeometryModel3D GeomModel3_UX1 = new GeometryModel3D();
                    GeomModel3_UX1 = volaux.CreateM_3D_G_Volume_8Edges(fa, fb, fb, new DiffuseMaterial(brushX));
                    TranslateTransform3D Translate3D_UX1;
                    Translate3D_UX1 = new TranslateTransform3D(-0.5f * fa, -0.5f * fa - 0.5f * fb, -0.5f * fb);
                    GeomModel3_UX1.Transform = Translate3D_UX1;
                    model_gr.Children.Add(GeomModel3_UX1);

                    GeometryModel3D GeomModel3_UX2 = new GeometryModel3D();
                    GeomModel3_UX2 = volaux.CreateM_3D_G_Volume_8Edges(fa, fb, fb, new DiffuseMaterial(brushX));
                    TranslateTransform3D Translate3D_UX2;
                    Translate3D_UX2 = new TranslateTransform3D(-0.5f * fa, +0.5f * fa - 0.5f * fb, -0.5f * fb);
                    GeomModel3_UX2.Transform = Translate3D_UX2;
                    model_gr.Children.Add(GeomModel3_UX2);
                }

                // Translation release in local y-axis
                if (m_bRestrain[(int)ENSupportType.eNST_Uy] == false)
                {

                    // NEFUNGUJE !!!

                    /*
                    RotateTransform3D RotateTrans3D_AUX = new RotateTransform3D();
                    RotateTrans3D_AUX.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 90);

                    GeometryModel3D GeomModel3_UY1 = new GeometryModel3D();
                    GeomModel3_UY1 = volaux.CreateM_3D_G_Volume_8Edges(fa, 0.02f, 0.02f, new DiffuseMaterial(brushY));
                    TranslateTransform3D Translate3D_UY1;
                    Translate3D_UY1 = new TranslateTransform3D(-0.1f, - 0.5f * fa, -0.01f);
                    Transform3DGroup Trans3DGroup1 = new Transform3DGroup();
                    Trans3DGroup1.Children.Add(RotateTrans3D_AUX);
                    Trans3DGroup1.Children.Add(Translate3D_UY1);
                    GeomModel3_UY1.Transform = Trans3DGroup1;
                    model_gr.Children.Add(GeomModel3_UY1);

                    GeometryModel3D GeomModel3_UY2 = new GeometryModel3D();
                    GeomModel3_UY2 = volaux.CreateM_3D_G_Volume_8Edges(fa, 0.02f, 0.02f, new DiffuseMaterial(brushY));
                    TranslateTransform3D Translate3D_UY2;
                    Translate3D_UY2 = new TranslateTransform3D(0.5f * fa + 0.1f, -0.5f * fa, -0.01f);
                    Transform3DGroup Trans3DGroup2 = new Transform3DGroup();
                    Trans3DGroup2.Children.Add(RotateTrans3D_AUX);
                    Trans3DGroup2.Children.Add(Translate3D_UY2);
                    GeomModel3_UY2.Transform = Trans3DGroup2;
                    model_gr.Children.Add(GeomModel3_UY2);
                    */

                    /*
                    GeometryModel3D GeomModel3_UY1 = new GeometryModel3D();
                    GeomModel3_UY1 = volaux.CreateM_3D_G_Volume_8Edges(0.02f, fa, 0.02f, new DiffuseMaterial(brushY));
                    TranslateTransform3D Translate3D_UY1;
                    Translate3D_UY1 = new TranslateTransform3D(-0.1f, -0.5f * fa, -0.01f);
                    GeomModel3_UY1.Transform = Translate3D_UY1;
                    model_gr.Children.Add(GeomModel3_UY1);

                    GeometryModel3D GeomModel3_UY2 = new GeometryModel3D();
                    GeomModel3_UY2 = volaux.CreateM_3D_G_Volume_8Edges(0.02f, fa, 0.02f, new DiffuseMaterial(brushY));
                    TranslateTransform3D Translate3D_UY2;
                    Translate3D_UY2 = new TranslateTransform3D(fa - 0.1f, -0.5f * fa, -0.01f);
                    GeomModel3_UY2.Transform = Translate3D_UY2;
                    model_gr.Children.Add(GeomModel3_UY2);
                    */

                    GeometryModel3D GeomModel3_UY1 = new GeometryModel3D();
                    GeomModel3_UY1 = volaux.CreateM_3D_G_Volume_8Edges(new Point3D(-0.5f * fa - 0.5f * fb, -0.5f * fa, -0.5f * fb), fb, fa, fb, new DiffuseMaterial(brushY));
                    model_gr.Children.Add(GeomModel3_UY1);

                    GeometryModel3D GeomModel3_UY2 = new GeometryModel3D();
                    GeomModel3_UY2 = volaux.CreateM_3D_G_Volume_8Edges(new Point3D(0.5f * fa - 0.5f * fb, -0.5f * fa, -0.5f * fb), fb, fa, fb, new DiffuseMaterial(brushY));
                    model_gr.Children.Add(GeomModel3_UY2);
                }

                // Translation release in local z-axis
                if (m_bRestrain[(int)ENSupportType.eNST_Uz] == false)
                {
                    RotateTransform3D RotateTrans3D_AUX = new RotateTransform3D();
                    RotateTrans3D_AUX.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);

                    GeometryModel3D GeomModel3_UZ1 = new GeometryModel3D();
                    GeomModel3_UZ1 = volaux.CreateM_3D_G_Volume_8Edges(fa, fb, fb, new DiffuseMaterial(brushZ));
                    TranslateTransform3D Translate3D_UZ1;
                    Translate3D_UZ1 = new TranslateTransform3D(-0.5f * fb, -0.5f * fa - 0.5f * fb, 0.5f * fa);
                    Transform3DGroup Trans3DGroup1 = new Transform3DGroup();
                    Trans3DGroup1.Children.Add(RotateTrans3D_AUX);
                    Trans3DGroup1.Children.Add(Translate3D_UZ1);
                    GeomModel3_UZ1.Transform = Trans3DGroup1;
                    model_gr.Children.Add(GeomModel3_UZ1);

                    GeometryModel3D GeomModel3_UZ2 = new GeometryModel3D();
                    GeomModel3_UZ2 = volaux.CreateM_3D_G_Volume_8Edges(fa, fb, fb, new DiffuseMaterial(brushZ));
                    TranslateTransform3D Translate3D_UZ2;
                    Translate3D_UZ2 = new TranslateTransform3D(-0.5f * fb, +0.5f * fa - 0.5f * fb, 0.5f * fa);
                    Transform3DGroup Trans3DGroup2 = new Transform3DGroup();
                    Trans3DGroup2.Children.Add(RotateTrans3D_AUX);
                    Trans3DGroup2.Children.Add(Translate3D_UZ2);
                    GeomModel3_UZ2.Transform = Trans3DGroup2;
                    model_gr.Children.Add(GeomModel3_UZ2);
                }


            }

            // Set position of release at member in LCS

            if (m_Node == m_Member.NodeStart) // Release at start of member
            {
                Translate3D = new TranslateTransform3D(fa, 0f, 0f);
            }
            else // Release at end of member
            {
                Translate3D = new TranslateTransform3D(m_Member.FLength - fa, 0f, 0f);
            }

            // Set release position in LCS of member
            model_gr.Transform = Translate3D;

            return model_gr;
        }
    }
}
