using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class CCladdingAccessories_Item_Length : CEntity3D
    {
        public double m_length;

        public DATABASE.DTO.CCladdingAccessories_Item_Length_Properties m_ItemProp;

        public CCladdingAccessories_Item_Length()
        { }

        public CCladdingAccessories_Item_Length(string name)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Length_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemLength(name);
        }

        public CCladdingAccessories_Item_Length(string name, double length)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Length_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemLength(name);
            m_length = length;
        }
    }
}