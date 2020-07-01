using CRSC;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaseClasses.Helpers
{
    public static class CrScFactory
    {
        private static Dictionary<string, CrScProperties> sectionsProps = null;

        public static CCrSc_TW GetCrSc(string SectionName)
        {
            if(sectionsProps == null) sectionsProps = CSectionManager.LoadSectionProperties();

            CrScProperties s = null;
            sectionsProps.TryGetValue(SectionName, out s);
            if (s == null) return null;

            if (SectionName == "10075")
            {
                return new CCrSc_3_10075_BOX((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "27055" || SectionName == "27095" || SectionName == "270115")
            {
                return new CCrSc_3_270XX_C((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "27055n" || SectionName == "27095n" || SectionName == "270115n")
            {
                return new CCrSc_3_270XX_C_NESTED((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "270115btb")
            {
                return new CCrSc_3_270XX_C_BACK_TO_BACK((float)s.h, (float)s.b, 0.020f, (float)s.t_min);
            }
            else if (SectionName == "50020")
            {
                return new CCrSc_3_50020_C((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "50020n")
            {
                return new CCrSc_3_50020_C_NESTED((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "63020" || SectionName == "63020s1" || SectionName == "63020s2")
            {
                return new CCrSc_3_63020_BOX((float)s.h, (float)s.b, (float)s.t_min, s.n_stiff);
            }
            else if (SectionName == "1x50x1" || SectionName == "2x50x1" || SectionName == "3x50x1" || SectionName == "4x50x1" ||
                     SectionName == "1x100x1" || SectionName == "2x100x1" || SectionName == "3x100x1" || SectionName == "4x100x1")
            {
                return new CCrSc_3_FLAT(Int32.Parse(SectionName.Substring(0,1)), (float)s.h, (float)s.t_min);
            }
            else
            {
                throw new Exception("Cross-section with name " + SectionName + " is not defined.");
            }
        }
    }
}
