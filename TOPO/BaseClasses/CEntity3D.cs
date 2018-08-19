using _3DTools;
using BaseClasses.GraphObj;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    //[Serializable]
    // Base class of all topological model entities
    abstract public class CEntity3D : CEntity
    {
        /*
        Model3DGroup mObject3DModel = new Model3DGroup();

        public Model3DGroup MObject3DModel
        {
            get { return mObject3DModel; }
            set { mObject3DModel = value; }
        }
        */

        public CPoint m_pControlPoint = new CPoint();
        public CMat m_Mat;
        public DiffuseMaterial m_Material3DGraphics;
        public string m_Shape;

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CEntity3D() { }

        public Model3DGroup Transform3D_OnMemberEntity_fromLCStoGCS(Model3DGroup modelgroup_original, CMember member)
        {
            // Objekty na prute s x <> 0
            // Modelgroup musime pridat ako child do novej modelgroup inak sa "Transform" definovane z 0,0,0 do LCS pruta prepise "Transform" z LCS do GCS

            Model3DGroup modelgroup_out = new Model3DGroup();
            modelgroup_out.Children.Add(modelgroup_original);
            modelgroup_out.Transform = CreateTransformCoordGroup(member);

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

        public Transform3DGroup CreateTransformCoordGroup(CMember member)
        {
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
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);

            return Trans3DGroup;
        }
    }
}
