using _3DTools;
using BaseClasses.GraphObj;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    // Base class of all topological model entities
    abstract public class CEntity3D : CEntity
    {
        bool m_bIsDisplayed;

        public bool BIsDisplayed
        {
            get { return m_bIsDisplayed; }
            set { m_bIsDisplayed = value; }
        }

        bool m_bIsSelectedForDesign;

        public bool BIsSelectedForDesign
        {
            get { return m_bIsSelectedForDesign; }
            set { m_bIsSelectedForDesign = value; }
        }

        bool m_bIsSelectedForMaterialList;

        public bool BIsSelectedForMaterialList
        {
            get { return m_bIsSelectedForMaterialList; }
            set { m_bIsSelectedForMaterialList = value; }
        }

        public Point3D ControlPoint = new Point3D();

        public CMat m_Mat;
        [NonSerialized]
        public DiffuseMaterial m_Material3DGraphics;
        public string m_Shape;

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CEntity3D()
        {
            // Set as default property that object is displayed
            m_bIsDisplayed = true;
            // Set as default property that object is selected for design
            m_bIsSelectedForDesign = true;
            // Set as default property that object is selected for material list
            m_bIsSelectedForMaterialList = true;
        }

        public Model3DGroup Transform3D_OnMemberEntity_fromLCStoGCS(Model3DGroup modelgroup_original, CMember member, bool bConsiderRotationAboutLCS_xAxis = false)
        {
            // Objekty na prute s x <> 0
            // Modelgroup musime pridat ako child do novej modelgroup inak sa "Transform" definovane z 0,0,0 do LCS pruta prepise "Transform" z LCS do GCS

            Model3DGroup modelgroup_out = new Model3DGroup();
            modelgroup_out.Children.Add(modelgroup_original);
            modelgroup_out.Transform = CreateTransformCoordGroup(member, bConsiderRotationAboutLCS_xAxis);

            // Return transformed model group
            return modelgroup_out;
        }
        public void Transform3D_OnMemberEntity_fromLCStoGCS_ChangeOriginal(Model3DGroup modelgroup_original, CMember member)
        {
            // Objekty na prute s x <> 0
            // Modelgroup musime pridat ako child do novej modelgroup inak sa "Transform" definovane z 0,0,0 do LCS pruta prepise "Transform" z LCS do GCS
            Transform3DGroup trg = CreateTransformCoordGroup(member);
            if (modelgroup_original.Transform == null) modelgroup_original.Transform = trg;
            else
            {
                Transform3DGroup gr = new Transform3DGroup();
                gr.Children.Add(modelgroup_original.Transform);
                gr.Children.Add(trg);
                modelgroup_original.Transform = gr;
            }
        }

        public ScreenSpaceLines3D Transform3D_OnMemberEntity_fromLCStoGCS(ScreenSpaceLines3D wireframeModel_original, CMember member)
        {
            ScreenSpaceLines3D wireframeModel_out = new ScreenSpaceLines3D();
            wireframeModel_out = wireframeModel_original;
            wireframeModel_out.Transform = CreateTransformCoordGroup(member);

            // Return transformed model
            return wireframeModel_out;
        }
        public void Transform3D_OnMemberEntity_fromLCStoGCS(ref List<ScreenSpaceLines3D> wireframeModel_group, CMember member)
        {
            foreach (ScreenSpaceLines3D wireframeModel in wireframeModel_group)
            {
                wireframeModel.Transform = CreateTransformCoordGroup(member);
            }
        }

        public Transform3DGroup CreateTransformCoordGroup(CMember member, bool bConsiderRotationAboutLCS_xAxis = false)
        {
            // TATO TRANSFORMACIA V DEFAULT IGNORUJE POOTOCENIE LCS PRUTA / RESP PRIEREZU - treba to nejako elegantne domysliet

            double dAlphaX = 0;
            double dBetaY = 0;
            double dGammaZ = 0;
            double dBetaY_aux = 0;
            double dGammaZ_aux = 0;

            member.GetRotationAngles(out dAlphaX, out dBetaY, out dGammaZ, out dBetaY_aux, out dGammaZ_aux);

            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), dBetaY_aux * 180 / Math.PI);
            RotateTrans3D_AUX_Y.CenterX = 0;
            RotateTrans3D_AUX_Y.CenterY = 0;
            RotateTrans3D_AUX_Y.CenterZ = 0;

            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), dGammaZ_aux * 180 / Math.PI);
            RotateTrans3D_AUX_Z.CenterX = 0;
            RotateTrans3D_AUX_Z.CenterY = 0;
            RotateTrans3D_AUX_Z.CenterZ = 0;

            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(member.NodeStart.X, member.NodeStart.Y, member.NodeStart.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();

            RotateTransform3D RotateTrans3D_LCSx = new RotateTransform3D();

            RotateTrans3D_LCSx.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), member.DTheta_x * 180 / Math.PI);
            RotateTrans3D_LCSx.CenterX = 0;
            RotateTrans3D_LCSx.CenterY = 0;
            RotateTrans3D_LCSx.CenterZ = 0;

            if (bConsiderRotationAboutLCS_xAxis)
                Trans3DGroup.Children.Add(RotateTrans3D_LCSx);

            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);

            return Trans3DGroup;
        }
    }
}
