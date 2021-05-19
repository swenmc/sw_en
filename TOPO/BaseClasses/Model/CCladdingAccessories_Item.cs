using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class CCladdingAccessories_Item : CEntity3D
    {
        public int m_count;

        public double m_length;

        public DATABASE.DTO.CCladdingAccessories_Item_Properties m_ItemProp;

        public CCladdingAccessories_Item()
        { }

        public CCladdingAccessories_Item(string name)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties(name);
        }

        public CCladdingAccessories_Item(string name, int count)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties(name);
            m_count = count;
        }

        public CCladdingAccessories_Item(string name, double length)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties(name);
            m_length = length;
        }
    }
}