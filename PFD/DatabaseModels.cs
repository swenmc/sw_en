using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class DatabaseModels
    {
        //mena premmennych treba zvolit tak aby bolo mozne im rozumiet b,L,L1,h,FrNo  to nema sancu nikto rozkodovat co znamena
        public float fb; // Building Width (X-axis)
        public float fL; // Building Length (Y-axis)
        public float fL1; // Bay Length / Distance between frames (Y-axis)
        public float fh; // Building Wall Height (Z-axis)
        public int iFrNo; // Number of Frames
        public float fRoof_Pitch_deg; // Building roof pitch
        public float fdist_girt; // Distance between girts (Z-axis)
        public float fdist_purlin; // Distance between purlins (X-axis in roof pitch slope)
        public float fdist_frontcolumn; // Distance between front columns (X-axis)
        public float fdist_girt_bottom; // Distance between bottom girt and column slab (Z-axis)
        public float fRakeAngleFrontFrame_deg; // Angle between X-axis and first frame
        public float fRakeAngleBackFrame_deg; // Angle between X-axis and last frame

        public DatabaseModels()
        { }

        public DatabaseModels(int iSelectedIndex)
        {
            fb = DatabaseManager.GetValueFromDatabasebyRowID("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "width", iSelectedIndex+1);
            fL = DatabaseManager.GetValueFromDatabasebyRowID("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "length", iSelectedIndex+1);
            fh = DatabaseManager.GetValueFromDatabasebyRowID("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "wall_height", iSelectedIndex+1);
            iFrNo = (int)DatabaseManager.GetValueFromDatabasebyRowID("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "iFrames", iSelectedIndex+1);
            fL1 = fL / (iFrNo - 1);
            fRoof_Pitch_deg = 15;
            fdist_girt = 0.25f * fL1;
            fdist_purlin = 0.25f * fL1;
            fdist_frontcolumn = 0.5f * fL1;
            fdist_girt_bottom = 0.3f; // Distance from concrete foundation to the centerline
            fRakeAngleFrontFrame_deg = 0.0f; // Angle between first frame and global X-axis
            fRakeAngleBackFrame_deg = 0.0f; // Angle between last frame and global X-axis

            
        }
     }
}























































