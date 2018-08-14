using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;

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

        private bool m_bUseColorScaleRedAndBlue;

        public bool UseColorScaleRedAndBlue
        {
            get { return m_bUseColorScaleRedAndBlue; }
            set { m_bUseColorScaleRedAndBlue = value; }
        }

        ELoadCoordSystem eLoadCS;
        ELoadDir eLoadDirection;
        float[] fX_coordinates;
        float fX_dimension_max;
        float fY_dimension;
        float fY2_dimension;
        float[,] fValues;

        public bool bDrawPositiveValueOnPlusLocalZSide;
        public bool bChangePositionForNegativeValue;

        public CSLoad_FreeUniformGroup(
               ELoadCoordSystem eLoadCS_temp,
               ELoadDir eLoadDirection_temp,
               CPoint pControlPoint_temp,
               float[] fX_coordinates_temp,
               float fX_dimension_max_temp,
               float fY_dimension_temp,
               float[,] fValues_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               bool bDrawPositiveValueOnPlusLocalZSide_temp,
               bool bChangePositionForNegativeValue_temp,
               bool bIsDisplayed,
               float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            eLoadCS = eLoadCS_temp;
            eLoadDirection = eLoadDirection_temp;
            m_pControlPoint = pControlPoint_temp;
            fX_coordinates = fX_coordinates_temp;
            fX_dimension_max = fX_dimension_max_temp;
            fY_dimension = fY_dimension_temp;
            fY2_dimension = 0f; // Rectangle
            fValues = fValues_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;

            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            BIsDisplayed = bIsDisplayed;

            CreateParticularLoads();
        }

        public CSLoad_FreeUniformGroup(
               ELoadCoordSystem eLoadCS_temp,
               ELoadDir eLoadDirection_temp,
               CPoint pControlPoint_temp,
               float[] fX_coordinates_temp,
               float fX_dimension_max_temp,
               float fY_dimension_temp,
               float fY2_dimension_temp,
               float[,] fValues_temp,
               float m_fRotationX_deg_temp,
               float m_fRotationY_deg_temp,
               float m_fRotationZ_deg_temp,
               bool bDrawPositiveValueOnPlusLocalZSide_temp,
               bool bChangePositionForNegativeValue_temp,
               bool bIsDisplayed,
               float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            eLoadCS = eLoadCS_temp;
            eLoadDirection = eLoadDirection_temp;
            m_pControlPoint = pControlPoint_temp;
            fX_coordinates = fX_coordinates_temp;
            fX_dimension_max = fX_dimension_max_temp;
            fY_dimension = fY_dimension_temp;
            fY2_dimension = fY2_dimension_temp; // Trapezoidal shape
            fValues = fValues_temp;
            RotationX_deg = m_fRotationX_deg_temp;
            RotationY_deg = m_fRotationY_deg_temp;
            RotationZ_deg = m_fRotationZ_deg_temp;

            bDrawPositiveValueOnPlusLocalZSide = bDrawPositiveValueOnPlusLocalZSide_temp;
            bChangePositionForNegativeValue = bChangePositionForNegativeValue_temp;
            BIsDisplayed = bIsDisplayed;

            CreateParticularLoads();
        }

        public void CreateParticularLoads()
        {
            // TODO - Ondrej - pripravit koncept pre farby zatazenia ale aj vseobecne v programe pre pruty, prierezy, materialy atd
            // TODO vymysliet nejaky obecny koncept (nech sa mame s cim hrat), rozdielne farby zatazenia podla segmentov, rozdielne farby podla hodnot, podla znamienka hodnoty a pod
            // TODO umoznit farby uzivatelsky nastavovat
            // TODO definovat zoznam farieb, asi by to mohlo byt niekde inde globalne dostupne, moze sa to hodit
            UseColorScaleRedAndBlue = true;

            LoadList = new List<CSLoad_FreeUniform>(1);

            int indexDirection = 1; // 0-3

            float segmentStart_x_coordinate;
            float segment_x_dimension;

            for (int i = 0; i < fX_coordinates.Length - 1; i++)
            {
                if ((fX_coordinates[i + 1]) <= fX_dimension_max)
                {
                    segmentStart_x_coordinate = fX_coordinates[i]; // Coordinate of segment start
                    segment_x_dimension = fX_coordinates[i + 1] - fX_coordinates[i]; // Length of particular segment

                    CPoint pControlPoint_segment = new CPoint();
                    pControlPoint_segment.X = segmentStart_x_coordinate;
                    pControlPoint_segment.Y = 0;
                    pControlPoint_segment.Z = 0;

                    float fY_dimension_temp1 = fY_dimension; // Rectangle
                    float fY_dimension_temp2 = fY_dimension; // Rectangle

                    // Trapezoidal shape
                    if (!MathF.d_equal(fY2_dimension, 0.0f))
                    {
                        CalculateYCoordinatesOfSegment(segmentStart_x_coordinate, segment_x_dimension, out fY_dimension_temp1, out fY_dimension_temp2);
                    }

                    if (!MathF.d_equal(fY2_dimension,0) && fX_coordinates[i + 1] > 0.5f * fX_dimension_max) // Segment in the middle
                    {
                        // Create object in LCS (x - direction with changing values of load)
                        // 5 points
                        float fY_dimension_temp1_unused;
                        float fY3_dimension_temp; // Bod na pravej strane
                        CalculateYCoordinatesOfSegment(0.5f * fX_dimension_max, segmentStart_x_coordinate + segment_x_dimension - 0.5f * fX_dimension_max, out fY_dimension_temp1_unused, out fY3_dimension_temp);
                        LoadList.Add(new CSLoad_FreeUniform(eLoadCS, eLoadDirection, pControlPoint_segment, segment_x_dimension, fY3_dimension_temp, 0.5f * fX_dimension_max - segmentStart_x_coordinate, fY2_dimension, fY_dimension_temp1, fValues[indexDirection, i], 0, 0, 0, GetColorBySegmentIDAndValueSign(i, fValues[indexDirection, i]), bDrawPositiveValueOnPlusLocalZSide, bChangePositionForNegativeValue, false, BIsDisplayed, FTime));
                    }
                    else
                    {
                        // Create object in LCS (x - direction with changing values of load)
                        // 4 points
                        LoadList.Add(new CSLoad_FreeUniform(eLoadCS, eLoadDirection, pControlPoint_segment, segment_x_dimension, fY_dimension_temp1, fY_dimension_temp2, fValues[indexDirection, i], 0, 0, 0, GetColorBySegmentIDAndValueSign(i, fValues[indexDirection, i]), bDrawPositiveValueOnPlusLocalZSide, bChangePositionForNegativeValue, true, BIsDisplayed, FTime));
                    }
                }
                else
                {
                    segmentStart_x_coordinate = fX_coordinates[i]; // Coordinate of segment start
                    segment_x_dimension = fX_dimension_max - fX_coordinates[i]; // Length of particular segment

                    CPoint pControlPoint_segment = new CPoint();
                    pControlPoint_segment.X = segmentStart_x_coordinate;
                    pControlPoint_segment.Y = 0;
                    pControlPoint_segment.Z = 0;

                    float fY_dimension_temp1 = fY_dimension; // Rectangle
                    float fY_dimension_temp2 = fY_dimension; // Rectangle

                    // Trapezoidal shape
                    if (!MathF.d_equal(fY2_dimension, 0.0f))
                    {
                        CalculateYCoordinatesOfSegment(segmentStart_x_coordinate, segment_x_dimension, out fY_dimension_temp1, out fY_dimension_temp2);
                    }

                    // Create object in LCS (x - direction with changing values of load)
                    LoadList.Add(new CSLoad_FreeUniform(eLoadCS, eLoadDirection, pControlPoint_segment, segment_x_dimension, fY_dimension_temp1, fY_dimension_temp2, fValues[indexDirection, i], 0, 0, 0, GetColorBySegmentIDAndValueSign(i, fValues[indexDirection, i]), bDrawPositiveValueOnPlusLocalZSide, bChangePositionForNegativeValue, true, BIsDisplayed, FTime));

                    break; // Finish cycle after adding of last segment, we dont need to continue per whole list of fX_coordinates
                }
            }
        }

        private void CalculateYCoordinatesOfSegment(float fSegmentStart_x_coordinate, float fSegmentStart_x_dimension, out float fY_dimension_temp1, out float fY_dimension_temp2)
        {
            if (fSegmentStart_x_coordinate < 0.5f * fX_dimension_max)  // Half of symmetric gable roof
            {
                // Left side of building
                fY_dimension_temp1 = fY_dimension + (fSegmentStart_x_coordinate * ((fY2_dimension - fY_dimension) / (0.5f * fX_dimension_max)));
                fY_dimension_temp2 = fY_dimension + ((fSegmentStart_x_coordinate + fSegmentStart_x_dimension) * ((fY2_dimension - fY_dimension) / (0.5f * fX_dimension_max)));
            }
            else
            {
                // Right side of building
                fY_dimension_temp1 = fY2_dimension - ((fSegmentStart_x_coordinate - (0.5f * fX_dimension_max)) * ((fY2_dimension - fY_dimension) / (0.5f * fX_dimension_max)));
                fY_dimension_temp2 = fY2_dimension - (((fSegmentStart_x_coordinate + fSegmentStart_x_dimension) - (0.5f * fX_dimension_max)) * ((fY2_dimension - fY_dimension) / (0.5f * fX_dimension_max)));
            }
        }

        public Color GetColorBySegmentIDAndValueSign(int iSegmentID, float fValue)
        {
            List<Color> ColorListGeneral = new List<Color>(6);
            ColorListGeneral.Add(Colors.Cyan);
            ColorListGeneral.Add(Colors.Fuchsia);
            ColorListGeneral.Add(Colors.Gold);
            ColorListGeneral.Add(Colors.IndianRed);
            ColorListGeneral.Add(Colors.Lime);
            ColorListGeneral.Add(Colors.Magenta);

            List<Color> ColorListPalleteOfRed = new List<Color>(6);
            ColorListPalleteOfRed.Add(Color.FromRgb(190, 0, 40));
            ColorListPalleteOfRed.Add(Color.FromRgb(230, 30, 30));
            ColorListPalleteOfRed.Add(Color.FromRgb(250, 80, 40));
            ColorListPalleteOfRed.Add(Color.FromRgb(250, 140, 60));
            ColorListPalleteOfRed.Add(Color.FromRgb(255, 180, 70));
            ColorListPalleteOfRed.Add(Color.FromRgb(255, 220, 118));

            List<Color> ColorListPalleteOfBlue = new List<Color>(6);
            ColorListPalleteOfBlue.Add(Color.FromRgb(10, 80, 160));
            ColorListPalleteOfBlue.Add(Color.FromRgb(30, 110, 180));
            ColorListPalleteOfBlue.Add(Color.FromRgb(70, 150, 200));
            ColorListPalleteOfBlue.Add(Color.FromRgb(110, 170, 220));
            ColorListPalleteOfBlue.Add(Color.FromRgb(160, 200, 220));
            ColorListPalleteOfBlue.Add(Color.FromRgb(200, 220, 240));

            Color loadcolor = new Color();

            if(UseColorScaleRedAndBlue)
            {
                if (fValue > 0)
                    loadcolor = ColorListPalleteOfRed[iSegmentID];
                else
                    loadcolor = ColorListPalleteOfBlue[iSegmentID];
            }
            else
            {
                loadcolor = ColorListGeneral[iSegmentID];
            }
            return loadcolor;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            m_Material3DGraphics = new DiffuseMaterial();
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_fOpacity = 0.3f;
            m_Material3DGraphics.Brush.Opacity = m_fOpacity;

            Model3DGroup model_gr = new Model3DGroup();

            Transform3DGroup trOfWholeLoadGroup = CreateTransformCoordGroupOfLoadGroup(); // Transform whole group of particular load segments
            foreach (CSLoad_FreeUniform load in LoadList)
            {
                // TODO - Ondrej - toto sa da asi urobit krajsie :) mam tu 2 riesenia
                // v LCS load group potrebujeme kazdy segment len posunut v smere x
                // pre prvy sposob sa vyrobi nova TranslateTransform3D a ta sa prida to skupiny tempTransformGroupOfLoadSegment
                // pre druhy sposob sa pouzije povodna Transform3DGroup segmentu a ta sa prida do skupiny tempTransformGroupOfLoadSegment
                // potom sa do skupiny tempTransformGroupOfLoadSegment prida celkova Transform3DGroup pre celu LoadGroup
                // Nastavi sa vysledna tempTransformGroupOfLoadSegment

                Model3DGroup tempModelOfLoadSegment;
                Transform3DGroup tempTransformGroupOfLoadSegment = new Transform3DGroup();

                // Create segment 3D model
                tempModelOfLoadSegment = load.CreateM_3D_G_Load();

                bool bUseAlternativeSolution = false; // true - create new translate transformation for replacement in x, false - use Transform3DGroup of segment

                if (bUseAlternativeSolution)
                {
                    // Create new translate transformation for segment (posun v smere x)
                    TranslateTransform3D translateinx = new TranslateTransform3D(load.m_pControlPoint.X, 0, 0);

                    // Add segment translate transformation within LCS of group - pouzije sa nova TranslateTransform3D segmentu (obsahuje len posun)
                    tempTransformGroupOfLoadSegment.Children.Add(translateinx);
                }
                else
                {
                    // Add segment transformation within LCS of group - pouzije sa povodna Transform3DGroup segmentu (obsahuje posun ale aj rotacie)
                    tempTransformGroupOfLoadSegment.Children.Add(tempModelOfLoadSegment.Transform);
                }

                // Add transformation of whole group
                tempTransformGroupOfLoadSegment.Children.Add(trOfWholeLoadGroup);

                // Set final transofrmation to the segment
                tempModelOfLoadSegment.Transform = tempTransformGroupOfLoadSegment;

                model_gr.Children.Add(tempModelOfLoadSegment); // Add particular segment model to the model group in local coordinates
            }
            return model_gr;
        }

        public Transform3DGroup CreateTransformCoordGroupOfLoadGroup()
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
