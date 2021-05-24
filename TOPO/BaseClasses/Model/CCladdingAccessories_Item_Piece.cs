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
        private int count;

        public int Count { get => count; set => count = value; }

        private string note;

        public string Note { get => note; set => note = value; }

        public DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties m_ItemProp;

        public CCladdingAccessories_Item_Piece()
        { }

        public CCladdingAccessories_Item_Piece(string name)
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemPiece(name);
        }

        public CCladdingAccessories_Item_Piece(string name, int count, string note = "")
        {
            Name = name;
            m_ItemProp = new DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties();
            m_ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemPiece(name);
            Count = count;
            Note = note;
        }
    }
}