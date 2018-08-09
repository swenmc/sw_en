using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CSLoad_FreeUniformGroup : CSLoad_Free
    {
        private List<CSLoad_FreeUniform> m_loadList;

        public List<CSLoad_FreeUniform> LoadList
        {
            get { return m_loadList; }
            set { m_loadList = value; }
        }

        private List<Color> m_colorList;

        public List<Color> ColorList
        {
            get { return m_colorList; }
            set { m_colorList = value; }
        }

        public CSLoad_FreeUniformGroup(
            ELoadCoordSystem eLoadCS_temp,
            ELoadDir eLoadDirection_temp,
            CPoint pControlPoint_temp,
            float[] fX_coordinates,
            float fX_dimension_max,
            float fY_dimension,
            float[,] fValues,
            float m_fRotationX_deg_temp,
            float m_fRotationY_deg_temp,
            float m_fRotationZ_deg_temp,
            bool bIsDisplayed,
            float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            m_pControlPoint = pControlPoint_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;

            // TODO definovat zoznam farieb
            ColorList = new List<Color>(6);
            ColorList.Add(Colors.Cyan);
            ColorList.Add(Colors.Fuchsia);
            ColorList.Add(Colors.Gold);
            ColorList.Add(Colors.IndianRed);
            ColorList.Add(Colors.Lime);
            ColorList.Add(Colors.Magenta);

            LoadList = new List<CSLoad_FreeUniform>(1);

            int indexDirection = 1; // 0-3

            float segmentStart_x_coordinate;
            float segment_x_dimension;
            CPoint pControlPoint_segment = new CPoint();

            for (int i = 0; i < fX_coordinates.Length - 1; i++)
            {
                if ((fX_coordinates[i + 1]) <= fX_dimension_max)
                {
                    segmentStart_x_coordinate = fX_coordinates[i]; // Coordinate of segment start
                    segment_x_dimension = fX_coordinates[i + 1] - fX_coordinates[i]; // Length of particular segment

                    pControlPoint_segment.X = segmentStart_x_coordinate;
                    pControlPoint_segment.Y = 0;
                    pControlPoint_segment.Z = 0;

                    // Create object in LCS (x - direction with changing values of load)
                    LoadList.Add(new CSLoad_FreeUniform(eLoadCS_temp, eLoadDirection_temp, pControlPoint_segment, segment_x_dimension, fY_dimension, fValues[indexDirection, i], 0, 0, 0, ColorList[i], bIsDisplayed, fTime));
                }
                else
                {
                    segmentStart_x_coordinate = fX_coordinates[i]; // Coordinate of segment start
                    segment_x_dimension = fX_dimension_max - fX_coordinates[i]; // Length of particular segment

                    pControlPoint_segment.X = segmentStart_x_coordinate;
                    pControlPoint_segment.Y = 0;
                    pControlPoint_segment.Z = 0;

                    // Create object in LCS (x - direction with changing values of load)
                    LoadList.Add(new CSLoad_FreeUniform(eLoadCS_temp, eLoadDirection_temp, pControlPoint_segment, segment_x_dimension, fY_dimension, fValues[indexDirection, i], 0, 0, 0, ColorList[i], bIsDisplayed, fTime));

                    break; // Finish cycle after adding of last segment
                }
            }
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            m_Material3DGraphics = new DiffuseMaterial();
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_fOpacity = 0.3f;
            m_Material3DGraphics.Brush.Opacity = m_fOpacity;

            Model3DGroup model_gr = new Model3DGroup();

            foreach (CSLoad_FreeUniform load in LoadList)
                model_gr.Children.Add(load.CreateM_3D_G_Load()); // Add particular segment model to the model group in local coordinates

            model_gr.Transform = CreateTransformCoordGroup(); // Transform whole group of particular load segments
            return model_gr;
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate model from its LCS to GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), RotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), RotationZ_deg); // Rotation in degrees

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
