using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_TR_SHEETING : CSO
    {
        public CCrSc_3_TR_SHEETING() { }
        public CCrSc_3_TR_SHEETING(int iID_temp, float fh, float fb, float ft, Color color_temp)
        {
            ID = iID_temp;
        }

        public virtual void CalcCrSc_Coord() { }

    }
}
