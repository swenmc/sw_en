using _3DTools;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_KD : CPlate
    {
        public CConCom_Plate_KD()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KD(string sName_temp, GraphObj.CPoint controlpoint, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float fl_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            // TODO
        }
    }
}
