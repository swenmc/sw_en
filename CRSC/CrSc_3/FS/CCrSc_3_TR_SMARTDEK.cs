using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_TR_SMARTDEK : CCrSc_3_TR_SHEETING
    {
        public CCrSc_3_TR_SMARTDEK() { }
        public CCrSc_3_TR_SMARTDEK(int iID_temp, float fh, float fb, float ft, Color color_temp)
        {
            ID = iID_temp;
        }

        public override void CalcCrSc_Coord()
        {
        }
    }
}
