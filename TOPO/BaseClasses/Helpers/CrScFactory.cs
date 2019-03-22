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
        

        public static CSC GetCrSc(string SectionName)
        {
            if(sectionsProps == null) sectionsProps = CSectionManager.LoadSectionProperties();

            CrScProperties s = null;
            sectionsProps.TryGetValue(SectionName, out s);
            if (s == null) return null;
            if (SectionName == "Box 63020")
            {
                return new CCrSc_3_63020_BOX((float)s.h, (float)s.b, (float)s.t_min, (float)s.t_max);
            }
            else if (SectionName == "C 50020")
            {
                new CCrSc_3_50020_C((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "C 27095")
            {
                new CCrSc_3_270XX_C((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "C 270115")
            {
                new CCrSc_3_270XX_C_NESTED((float)s.h, (float)s.b, (float)s.t_min);
            }
            else if (SectionName == "Box 10075")
            {
                new CCrSc_3_270XX_C_NESTED((float)s.h, (float)s.b, (float)s.t_min);
            }

            return null;            
        }
    }
}
