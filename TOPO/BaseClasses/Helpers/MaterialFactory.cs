using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.Helpers
{
    public static class MaterialFactory
    {
        private static Dictionary<string, CMaterialProperties> materialsProps = null;


        public static CMat GetMaterial(string materialName)
        {
            if (materialsProps == null) materialsProps = CMaterialManager.LoadMaterialPropertiesDict();

            CMaterialProperties s = null;
            materialsProps.TryGetValue(materialName, out s);
            if (s == null) return null;
            if (materialName == "G550")
            {
                return new CMat_03_00(1, "G550", 200e+9f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            }
            else if (materialName == "50020_C")
            {
                return new CMat_03_00(1, "G550", 200e+9f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            }
            

            return null;
        }
    }
}
