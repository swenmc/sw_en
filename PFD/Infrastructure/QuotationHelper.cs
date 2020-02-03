using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PFD
{
    public static class QuotationHelper
    {
        // Tu by som chcel pre stlpce, ktore sa casto opakuju ale najlepsie pre vsetky
        // vyrobit defaultne, ktore sa budu vsade pouzivat, aby sa nestalo ze niekde nieco zmenime a niekde zabudneme        

        static string sEP_Width = "Width";
        static string sEP_Unit = "Unit";
        static string sEP_Align = "Align";

        // Bez jednotiek
        public static ColumnProperties colProp_Prefix = new ColumnProperties(typeof(String), "Prefix", "Prefix", 7.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Name = new ColumnProperties(typeof(String), "Name", "Name", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Item = new ColumnProperties(typeof(String), "Item", "Item", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Component = new ColumnProperties(typeof(String), "Component", "Component", 15f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Size = new ColumnProperties(typeof(String), "Size", "Size", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Description = new ColumnProperties(typeof(String), "Description", "Description", 15f, null, AlignmentX.Left);

        // Specificke nazvy (mozno by sa to dalo nahradit ako item / description alebo nieco podobne)
        public static ColumnProperties colProp_CrossSection = new ColumnProperties(typeof(String), "Crsc", "Cross-section", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Cladding = new ColumnProperties(typeof(String), "Cladding", "Cladding", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Fibreglass = new ColumnProperties(typeof(String), "Fibreglass", "Fibreglass", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Opening = new ColumnProperties(typeof(String), "Opening", "Opening", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Gutter = new ColumnProperties(typeof(String), "Gutter", "Gutter", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Downpipe = new ColumnProperties(typeof(String), "Downpipe", "Downpipe", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Flashing = new ColumnProperties(typeof(String), "Flashing", "Flashing", 20f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Material = new ColumnProperties(typeof(String), "Material", "Material", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Coating = new ColumnProperties(typeof(String), "Coating", "Coating", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Color = new ColumnProperties(typeof(String), "Color", "Colour", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_ColorName = new ColumnProperties(typeof(String), "ColorName", "Colour Name", 12.5f, null, AlignmentX.Left);

        // S jednotkami
        public static ColumnProperties colProp_Count = new ColumnProperties(typeof(Int32), "Count", "Count", 7.5f, "[-]", AlignmentX.Right);
        public static ColumnProperties colProp_Width_m = new ColumnProperties(typeof(String), "Width_m", "Width", 7.5f, "[m]", AlignmentX.Right);
        public static ColumnProperties colProp_Height_m = new ColumnProperties(typeof(String), "Height_m", "Height", 7.5f, "[m]", AlignmentX.Right);
        public static ColumnProperties colProp_Thickness_m = new ColumnProperties(typeof(String), "Thickness_m", "Thickness", 7.5f, "[m]", AlignmentX.Right);
        public static ColumnProperties colProp_Thickness_mm = new ColumnProperties(typeof(String), "Thickness_mm", "Thickness", 7.5f, "[mm]", AlignmentX.Right);
        public static ColumnProperties colProp_Diameter_mm = new ColumnProperties(typeof(String), "Diameter_mm", "Diameter", 7.5f, "[mm]", AlignmentX.Right);
        public static ColumnProperties colProp_Area_m2 = new ColumnProperties(typeof(String), "Area_m2", "Area", 10f, "[m²]", AlignmentX.Right);
        public static ColumnProperties colProp_UnitMass_LM = new ColumnProperties(typeof(String), "UnitMass_LM", "Unit Mass", 10f, "[kg/m]", AlignmentX.Right);
        public static ColumnProperties colProp_UnitMass_SM = new ColumnProperties(typeof(String), "UnitMass_SM", "Unit Mass", 10f, "[kg/m²]", AlignmentX.Right);
        public static ColumnProperties colProp_UnitMass_P = new ColumnProperties(typeof(String), "UnitMass_P", "Unit Mass", 10f, "[kg/piece]", AlignmentX.Right);
        public static ColumnProperties colProp_UnitPrice_LM_NZD = new ColumnProperties(typeof(String), "UnitPrice_LM_NZD", "Unit Price", 0f, "[NZD/m]", AlignmentX.Right);
        public static ColumnProperties colProp_UnitPrice_SM_NZD = new ColumnProperties(typeof(String), "UnitPrice_SM_NZD", "Unit Price", 0f, "[NZD/m²]", AlignmentX.Right);
        public static ColumnProperties colProp_UnitPrice_P_NZD = new ColumnProperties(typeof(String), "UnitPrice_P_NZD", "Unit Price", 0f, "[NZD/piece]", AlignmentX.Right);
        public static ColumnProperties colProp_TotalLength_m = new ColumnProperties(typeof(Decimal), "TotalLength_m", "Total Length", 10f, "[m]", AlignmentX.Right);
        public static ColumnProperties colProp_TotalArea_m2 = new ColumnProperties(typeof(Decimal), "TotalArea_m2", "Total Area", 10f, "[m²]", AlignmentX.Right);
        public static ColumnProperties colProp_TotalMass = new ColumnProperties(typeof(String), "TotalMass", "Total Mass", 10f, "[kg]", AlignmentX.Right);
        public static ColumnProperties colProp_TotalPrice_NZD = new ColumnProperties(typeof(String), "TotalPrice_NZD", "Price", 0f, "[NZD]", AlignmentX.Right);

        static List<ColumnProperties> colPropList = new List<ColumnProperties>()
            {
                colProp_Prefix,
                colProp_Name,
                colProp_Component,
                colProp_Size,
                colProp_Description,

                colProp_CrossSection,
                colProp_Cladding,
                colProp_Fibreglass,
                colProp_Opening,
                colProp_Gutter,
                colProp_Downpipe,
                colProp_Flashing,

                colProp_Material,
                colProp_Coating,
                colProp_Color,
                colProp_ColorName,

                colProp_Count,
                colProp_Width_m,
                colProp_Height_m,
                colProp_Thickness_m,
                colProp_Thickness_mm,
                colProp_Diameter_mm,
                colProp_Area_m2,
                colProp_UnitMass_LM,
                colProp_UnitMass_SM,
                colProp_UnitMass_P,
                colProp_UnitPrice_LM_NZD,
                colProp_UnitPrice_SM_NZD,
                colProp_UnitPrice_P_NZD,
                colProp_TotalLength_m,
                colProp_TotalArea_m2,
                colProp_TotalMass,
                colProp_TotalPrice_NZD
    };

    public static void AddPlateToQuotation(CPlate plate, List<QuotationItem> quotation, int iQuantity, float fCFS_PricePerKg_Plates_Total)
        {
            float fMassPerPiece = plate.fArea * plate.Ft * plate.m_Mat.m_fRho;
            float fPricePerPiece = plate.Price_PPKG_NZD > 0 ? (float)plate.Price_PPKG_NZD * fMassPerPiece : fCFS_PricePerKg_Plates_Total * fMassPerPiece;

            if (plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_J || plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_K) // Consider overal rectangular dimensions for knee and apex plates
            {
                fPricePerPiece = plate.Width_bx_Stretched * plate.Height_hy_Stretched * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;
            }

            QuotationItem qItem = quotation.FirstOrDefault(q => q.Prefix == plate.Name && MathF.d_equal(q.Width_bx, plate.Width_bx) &&
                    MathF.d_equal(q.Height_hy, plate.Height_hy) &&
                    MathF.d_equal(q.Ft, plate.Ft) &&
                    MathF.d_equal(q.Area, plate.fArea));
            if (qItem != null) //this quotation exists
            {
                qItem.Quantity += iQuantity;
                qItem.TotalArea = qItem.Quantity * qItem.Area;
                qItem.TotalMass = qItem.Quantity * qItem.MassPerPiece;
                qItem.TotalPrice = qItem.Quantity * qItem.PricePerPiece;
            }
            else //quotation item does not exist = add to collection
            {
                QuotationItem item = new QuotationItem
                {
                    Prefix = plate.Name,
                    Quantity = iQuantity,
                    Width_bx = plate.Width_bx,
                    Height_hy = plate.Height_hy,
                    Ft = plate.Ft,
                    MaterialName = plate.m_Mat.Name,
                    Area = plate.fArea,
                    MassPerPiece = fMassPerPiece,
                    PricePerPiece = fPricePerPiece,
                    TotalArea = iQuantity * plate.fArea,
                    TotalMass = iQuantity * fMassPerPiece,
                    TotalPrice = iQuantity * fPricePerPiece
                };
                quotation.Add(item);
            }
        }

        public static void SetDataTableColumnProperties(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
                SetColumnProperties(dt, GetColumnProperties(c));
        }

        private static void SetColumnProperties(DataTable dt, ColumnProperties properties)
        {
            // Set Column Caption
            dt.Columns[properties.ColumnName].Caption = properties.Caption;

            // Set Extended properties
            if (properties.EP_Unit != null && properties.EP_Unit.Length > 1)
                dt.Columns[properties.ColumnName].ExtendedProperties.Add(sEP_Unit, properties.EP_Unit);

            dt.Columns[properties.ColumnName].ExtendedProperties.Add(sEP_Width, properties.EP_Width);
            dt.Columns[properties.ColumnName].ExtendedProperties.Add(sEP_Align, properties.EP_Alignment);
        }

        private static ColumnProperties GetColumnProperties(DataColumn c)
        {
            ColumnProperties prop = null;

            if (colPropList != null)
            {
                prop = new ColumnProperties();
                prop = colPropList.Find(x => x.ColumnName == c.ColumnName);
            }

            return prop;
        }

    }
}
