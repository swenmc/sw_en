using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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

            ITotNoPointsin2D = 4 + 12;
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
    }
}
