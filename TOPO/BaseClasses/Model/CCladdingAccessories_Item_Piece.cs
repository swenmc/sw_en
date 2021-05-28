using DATABASE.DTO;
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

        private CCladdingAccessories_Item_Piece_Properties m_ItemProp;

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

        public CCladdingAccessories_Item_Piece_Properties ItemProp
        {
            get
            {
                return m_ItemProp;
            }

            set
            {
                m_ItemProp = value;
            }
        }

        public CCladdingAccessories_Item_Piece()
        { }

        public CCladdingAccessories_Item_Piece(string name)
        {
            Name = name;            
            ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemPiece(name);
        }

        public CCladdingAccessories_Item_Piece(string name, int count, string note = "")
        {
            Name = name;            
            ItemProp = DATABASE.CCladdingAccessoriesManager.GetItemProperties_ItemPiece(name);
            Count = count;
            Note = note;
        }
        public CCladdingAccessories_Item_Piece(CCladdingAccessories_Fixing_Properties fixing, int count)
        {
            Name = fixing.Name;
            ItemProp = new CCladdingAccessories_Item_Piece_Properties()
            {
                ID = fixing.ID,
                Mass_kg = fixing.Mass_kg,
                Name = fixing.Name,
                Price_PPKG_NZD = fixing.Price_PPKG_NZD,
                Price_PPP_NZD = fixing.Price_PPP_NZD,
                Standard = fixing.Standard
            };
            Count = count;
        }
    }
}