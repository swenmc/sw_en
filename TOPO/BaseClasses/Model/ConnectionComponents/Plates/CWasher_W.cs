using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CWasher_W : CPlate
    {
        public CWasher_W(string sName_temp,
            Point3D controlpoint,
            //float fbx_1_temp,
            //float fhy_1_temp,
            //float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_W;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 4;
            INoPoints2Dfor3D = 8;
            ITotNoPointsin3D = 14;

            m_pControlPoint = controlpoint;
            //m_fbX1 = fbx_1_temp;
            //m_fhY1 = fhy_1_temp;
            //Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_Mat.Name = "Q235"; // ???? TODO zapracovat do databazy ??? // AS/NZS1111

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            //arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            //Calc_Coord3D();

            if (sName_temp != null)
            {
                DATABASE.DTO.CRectWasher_W_Properties prop = DATABASE.CWashersManager.GetPlate_W_Properties(sName_temp);

                Width_bx = (float)prop.dim1x;
                Height_hy = (float)prop.dim2y;
                Ft = (float)prop.thickness;
                fArea = (float)prop.Area;
                fCuttingRouteDistance = 2 * Width_bx + 2 * Height_hy;
                fSurface = 2 * fArea + fCuttingRouteDistance * Ft;
                fVolume = (float)prop.Volume;
                fMass = (float)prop.Mass;
                Price_PPKG_NZD = prop.Price_PPKG_NZD;
                Price_PPP_NZD = prop.Price_PPP_NZD;
            }

            //UpdatePlateData(screwArrangement);
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            // Outline Points
            // Outline Edges
            float fRadiusOut = 0.2f;
            float fRadiusIn = 0.1f;

            short iEdgesOutBasic = 4;
            short iNumberOfSegmentsPerSideOut = 4;
            int iEdgeOut = iEdgesOutBasic * iNumberOfSegmentsPerSideOut;
            int iEdgesInBasic = iEdgeOut;

            float fAngleBasic_rad = MathF.fPI / iEdgesOutBasic;

            List<Point> pointsOutBasic = Geom2D.GetPolygonPointCoord_CW(fRadiusOut, iEdgesOutBasic);
            List<Point> pointsInBasic = Geom2D.GetPolygonPointCoord_CW(fRadiusIn, (short)iEdgesInBasic);

            List<Point> pointsOut = Geom2D.GetPolygonPointsIncludingIntermediateOnSides_CW(fRadiusOut, iEdgesOutBasic, iNumberOfSegmentsPerSideOut); // Stvorec (TODO - zamysliet sa ako dorobit vynimku pre obldznik, uhol medzi uhloprieckami nie je rovnaky)

            // Pre kombinaciu stvorca a kruhu musime stvorec musime body pootocit tak aby bol prvy bod v y = 0 a uprostred strany stvorca, nie rohovy bod
            // Naprv presunieme body tak aby bol prvy bod zoznamu bod ktory je uprostred strany stvorca
            ChangeFirstNodeOfinList(2, ref pointsOut);

            // Nasledne pootocime body okolo [0,0] o 45 stupnov proti smeru hodinovych ruciciek
            Geom2D.TransformPositions_CCW_rad(0, 0, fAngleBasic_rad, ref pointsOut);

            List<Point> pointsIn = pointsInBasic; // Kruh vo vnutri

            PointsOut2D = pointsOutBasic.ToArray();
        }

        // TODO Ondrej, pozri sa na to - asi to vies urobit lepsie
        public void ChangeFirstNodeOfinList(int iOffsetCount, ref List<Point> points)
        {
            List<Point> offSet = points.GetRange(0, iOffsetCount); // Zo zaciatku vyberieme prvky v pocte iOffsetCount a presunieme ich na koniec listu
            points.AddRange(offSet); // Pridame polozky na koniec zoznamu
            points.RemoveRange(0, iOffsetCount); // Vymazeme polozky zo zaciatku zoznamu
        }
    }
}
