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
        private static Dictionary<string, CMatProperties> materialsProps = null;

        public static CMat_03_00 GetMaterial(string materialName)
        {
            if (materialsProps == null) materialsProps = CMaterialManager.LoadMaterialProperties();

            CMatProperties s = null;
            materialsProps.TryGetValue(materialName, out s);
            if (s == null) return null;

            CMat_03_00 material = new CMat_03_00(1, s.Grade, 200e+9f, 0.3f); // Create steel material
            CMaterialManager.SetMaterialProperties(s, ref material);

            return material;
        }
    }
}
