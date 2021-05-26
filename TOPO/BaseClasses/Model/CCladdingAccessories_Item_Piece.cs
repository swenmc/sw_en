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
        private int m_Count;
        private string m_Note;

        public DATABASE.DTO.CCladdingAccessories_Item_Piece_Properties m_ItemProp;

        public int Count
        {
            get
            {
                return m_Count;
            }

            set
            {
                m_Count = value;
            }
        }

        public string Note
        {
            get
            {
                return m_Note;
            }

            set
            {
                m_Note = value;
            }
        }

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