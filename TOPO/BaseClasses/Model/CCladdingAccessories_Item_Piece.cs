using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class CCladdingAccessories_Item_Piece : CEntity3D
    {
        public int m_count;

        public DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties m_ItemProp;

        public CCladdingAccessories_Item_Piece()
        { }

        public CCladdingAccessories_Item_Piece(string name)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemPiece(name);
        }

        public CCladdingAccessories_Item_Piece(string name, int count)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemPiece(name);
            m_count = count;
        }
    }
}