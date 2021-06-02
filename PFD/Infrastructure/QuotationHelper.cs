using BaseClasses;
using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
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
        public static ColumnProperties colProp_Flashing = new ColumnProperties(typeof(String), "Flashing", "Flashing", 22f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Packer = new ColumnProperties(typeof(String), "Packer", "Packer", 22f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Material = new ColumnProperties(typeof(String), "Material", "Material", 12.5f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Coating = new ColumnProperties(typeof(String), "Coating", "Coating", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_Color = new ColumnProperties(typeof(String), "Color", "Colour", 10f, null, AlignmentX.Left);
        public static ColumnProperties colProp_ColorName = new ColumnProperties(typeof(String), "ColorName", "Colour Name", 12.5f, null, AlignmentX.Left);

        // S jednotkami
        public static ColumnProperties colProp_Count = new ColumnProperties(typeof(Int32), "Count", "Count", 7.5f, "[-]", AlignmentX.Right);
        public static ColumnProperties colProp_Width_m = new ColumnProperties(typeof(String), "Width_m", "Width", 7.5f, "[m]", AlignmentX.Right);
        public static ColumnProperties colProp_Height_m = new ColumnProperties(typeof(String), "Height_m", "Height", 7.5f, "[m]", AlignmentX.Right);
        public static ColumnProperties colProp_Length_m = new ColumnProperties(typeof(String), "Length_m", "Length", 7.5f, "[m]", AlignmentX.Right);
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
        public static ColumnProperties colProp_Note = new ColumnProperties(typeof(String), "Note", "Note", 20f, "[-]", AlignmentX.Left);

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
                colProp_Packer,

                colProp_Material,
                colProp_Coating,
                colProp_Color,
                colProp_ColorName,

                colProp_Count,
                colProp_Width_m,
                colProp_Height_m,
                colProp_Length_m,
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
                colProp_TotalPrice_NZD,

                colProp_Note
        };

        private const float fTEK_PricePerPiece_Screws_Total = 0.15f;     // NZD / piece / !!! priblizna cena - nezohladnuje priemer skrutky
        private const float fAnchor_PricePerLength = 30; // NZD / m - !!! priblizna cena - nezohladnuje priemer tyce

        public static void AddPlateToQuotation(CPlate plate, List<QuotationItem> quotation, int iQuantity, float fCFS_PricePerKg_Plates_Total)
        {
            if (plate == null) return;

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


        public static void AddConnector(CConnector connector, List<QuotationItem> quotation, int iQuantity)
        {
            string sPrefix = "";
            string name = "";
            float fTotalMass = iQuantity * connector.Mass;
            float fUnitPrice = connector.Price_PPP_NZD;

            if (connector.Price_PPP_NZD > 0)
                fUnitPrice = connector.Price_PPP_NZD;
            else
            {
                if (connector is CScrew) fUnitPrice = fTEK_PricePerPiece_Screws_Total;
                else if (connector is CAnchor) fUnitPrice = (fAnchor_PricePerLength * connector.Length);
            }

            if (connector is CScrew)
            {
                sPrefix = connector.Name;
                int iGauge = ((CScrew)connector).Gauge;
                name = $"{iGauge}g x {Math.Round(connector.Length * 1000, 0)}"; // Display in [mm] (value * 1000)
            }
            else if (connector is CAnchor)
            {
                sPrefix = connector.Prefix;
                name = $"{connector.Name} x {Math.Round(connector.Length * 1000, 0)}"; // Display in [mm] (value * 1000)
            }

            float fTotalPrice = iQuantity * fUnitPrice;

            QuotationItem item = quotation.FirstOrDefault(q => q.Prefix == sPrefix &&
                        MathF.d_equal(q.Diameter_thread, connector.Diameter_thread) &&
                        MathF.d_equal(q.Length, connector.Length) &&
                        MathF.d_equal(q.MassPerPiece, connector.Mass));

            if (item != null)
            {
                item.Quantity += iQuantity; // Add one connector (piece) to the quantity
                item.TotalMass = item.Quantity * item.MassPerPiece;// Recalculate total mass of all connectors in the group
                item.TotalPrice = item.Quantity * item.PricePerPiece;// Recalculate total price of all connectors in the group
            }
            else
            {
                QuotationItem newItem = new QuotationItem
                {
                    Prefix = sPrefix,
                    Quantity = iQuantity,
                    MaterialName = connector.m_Mat.Name,
                    Diameter_thread = connector.Diameter_thread,
                    Length = connector.Length,
                    Name = name,
                    MassPerPiece = connector.Mass,
                    TotalMass = fTotalMass,
                    PricePerPiece = fUnitPrice,
                    TotalPrice = fTotalPrice
                };
                quotation.Add(newItem);
            }
        }

        public static DataSet GetTablePlates(CModel model, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            const float fCFS_PricePerKg_Plates_Material = 1.698f;    // NZD / kg
            const float fCFS_PricePerKg_Plates_Manufacture = 0.0f;   // NZD / kg
            float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg

            // Plates
            List<QuotationItem> quotation = GetPlatesQuotation(model, fCFS_PricePerKg_Plates_Total);

            //System.Diagnostics.Trace.WriteLine("Joints SelectedForMaterialList count: " + count);

            // Check Data
            double dTotalPlatesArea_Model = 0, dTotalPlatesArea_Table = 0;
            double dTotalPlatesVolume_Model = 0, dTotalPlatesVolume_Table = 0;
            double dTotalPlatesMass_Model = 0, dTotalPlatesMass_Table = 0;
            double dTotalPlatesPrice_Model = 0, dTotalPlatesPrice_Table = 0;
            int iTotalPlatesNumber_Model = 0, iTotalPlatesNumber_Table = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            {
                if (joint.BIsSelectedForMaterialList)
                {
                    // Set plates and connectors data
                    if (joint.m_arrPlates == null || joint.m_arrPlates.Length == 0) continue;

                    foreach (CPlate plate in joint.m_arrPlates)
                    {
                        dTotalPlatesArea_Model += plate.fArea;
                        dTotalPlatesVolume_Model += plate.fArea * plate.Ft;
                        dTotalPlatesMass_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho;

                        if (plate.Price_PPKG_NZD > 0)
                            dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * plate.Price_PPKG_NZD;
                        else
                        {
                            dTotalPlatesPrice_Model += plate.fArea * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;

                            if (plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_J || plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_K) // Consider overal rectangular dimensions for knee and apex plates
                            {
                                dTotalPlatesPrice_Model += plate.Width_bx_Stretched * plate.Height_hy_Stretched * plate.Ft * plate.m_Mat.m_fRho * fCFS_PricePerKg_Plates_Total;
                            }
                        }

                        iTotalPlatesNumber_Model += 1;
                    }
                }
            }

            foreach (QuotationItem item in quotation)
            {
                dTotalPlatesArea_Table += item.Area * item.Quantity;
                dTotalPlatesVolume_Table += item.Area * item.Quantity * item.Ft;
                dTotalPlatesMass_Table += item.TotalMass;
                dTotalPlatesPrice_Table += item.TotalPrice;
                iTotalPlatesNumber_Table += item.Quantity;
            }
            dBuildingMass += dTotalPlatesMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalPlatesPrice_Table;

            // Create Table
            DataTable table = new DataTable("Plates");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Prefix.ColumnName, QuotationHelper.colProp_Prefix.DataType);
            table.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            table.Columns.Add(QuotationHelper.colProp_Material.ColumnName, QuotationHelper.colProp_Material.DataType);
            table.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Height_m.ColumnName, QuotationHelper.colProp_Height_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            table.Columns.Add(QuotationHelper.colProp_Area_m2.ColumnName, QuotationHelper.colProp_Area_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Prefix.ColumnName] = item.Prefix;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_Material.ColumnName] = item.MaterialName;
                    row[QuotationHelper.colProp_Width_m.ColumnName] = item.Width_bx.ToString("F2");
                    row[QuotationHelper.colProp_Height_m.ColumnName] = item.Height_hy.ToString("F2");
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (1000 * item.Ft).ToString("F3"); // milimeters
                    row[QuotationHelper.colProp_Area_m2.ColumnName] = item.Area.ToString("F2");
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = item.TotalArea.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Prefix.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalPlatesNumber_Table;
            row[QuotationHelper.colProp_Material.ColumnName] = "";
            row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            row[QuotationHelper.colProp_Height_m.ColumnName] = "";
            row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
            row[QuotationHelper.colProp_Area_m2.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = dTotalPlatesArea_Table.ToString("F2");
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalPlatesMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalPlatesPrice_Table.ToString("F2");
            table.Rows.Add(row);

            return ds;
        }

        public static List<QuotationItem> GetPlatesQuotation(CModel model, float fCFS_PricePerKg_Plates_Total)
        {
            List<QuotationItem> quotation = new List<QuotationItem>();

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                joint.SetJointIsSelectedForMaterialListAccordingToMember();

                if (!joint.BIsSelectedForMaterialList) continue;
                if (joint.m_arrPlates == null || joint.m_arrPlates.Length == 0) continue; // Spoj neobsahuje plates (napr. U001)

                foreach (CPlate plate in joint.m_arrPlates) // For each plate
                {
                    // Nastavime parametre plechu z databazy - TO Ondrej - toto by sa malo diat uz asi pri vytvarani plechov
                    // Nie vsetky plechy budu mat parametre definovane v databaze
                    // !!!! Treba doriesit presne rozmery pri vytvarani plates a zaokruhlovanie

                    #region Base Plate
                    // TO Ondrej Blok1 Plate START
                    // ----------------------------------------------------------------------------------------------------------------------------------------
                    try
                    {
                        plate.SetParams(plate.Name, plate.m_ePlateSerieType_FS);
                    }
                    catch { };

                    QuotationHelper.AddPlateToQuotation(plate, quotation, 1, fCFS_PricePerKg_Plates_Total);


                    // TO Ondrej Blok1 Plate END
                    // ----------------------------------------------------------------------------------------------------------------------------------------
                    #endregion

                    //temp
                    // Anchors - WASHERS
                    // TO Mato - nieco som skusal... chcelo by to asi mat jeden objekt na tieto veci a nie zoznamy kade tade
                    //rovnako je asi problem,ze to nijako negrupujem...ale tak potreboval by som vediet na zaklade coho sa to bude grupovat

                    // To Ondrej - K prvej vete nemam vyhrady. Urob ako sa to ma.
                    // Zgrupovat to treba podla prefixu, ale kedze to este nie je dotiahnute tak porovnavam aj rozmery a plochu uz pridanych plates alebo washers s aktualnym
                    // Vyrobil som 3 bloky kodu, resp. regiony
                    // Jeden pre base plate, jeden washer plate top a jeden pre washer bearing
                    // Funguje to tak ze sa v bloku nastavia parametre aktualnej plate / washer (pocet, rozmery cena, celkove pocty a cena atd)
                    // Potom sa prechadza cyklus cez vsetky uz vytvorene riadky, resp ListOfPlateGroups a porovnava sa ci je aktualny objekt rovnaky ako niektory uz pridany do skupiny
                    // Porovnava sa prefix, rozmery a plocha (ak by sme boli dosledni tak pre plates by sa este malo porovnat screw arrangement, anchor arrangement)
                    // Ak sa zisti ze rovnaky plate/ washer uz bol pridany tak sa aktualizuju celkove parametre, celkovy pocet, celkova plocha, celkova hmotnost
                    // Ak sa zisti ze taky plech v skupine este nie je alebo je to uplne prvy plech v cykle tak sa vyrobi novy zaznam

                    // Dalo by sa to napriklad refaktorovat a urobit z toho jedna funkcia
                    // ListOfPlateGroups by som asi zrusil, lebo tam nemame moznost nastavit pocet plechov v ramci skupiny
                    // Ak tomu rozumiem spravne chces na to pouzit List<PlateView> a odstranit jednotlive zoznamy podla stplcov
                    // Kazdopadne zase sa dostavame k tomu, ze to mame vselijako, niekde samostatne zoznamy pre jednotlive stlpce, inde zoznam objektov s properties, ktore odpovedaju jednemu riadku.

                    if (plate is CConCom_Plate_B_basic)
                    {
                        CConCom_Plate_B_basic plateB = (CConCom_Plate_B_basic)plate;

                        if (plateB.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                        {
                            CAnchor anchor = plateB.AnchorArrangement.Anchors.FirstOrDefault();
                            int anchorsNum = plateB.AnchorArrangement.Anchors.Length;

                            #region Washer Plate Top
                            // TO Ondrej Blok2 Washer Plate Top START
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            // Plate Top Washer
                            try
                            {
                                if (anchor.WasherPlateTop != null)
                                    anchor.WasherPlateTop.SetParams(anchor.WasherPlateTop.Name, anchor.WasherPlateTop.m_ePlateSerieType_FS);
                            }
                            catch { };

                            QuotationHelper.AddPlateToQuotation(anchor.WasherPlateTop, quotation, anchorsNum, fCFS_PricePerKg_Plates_Total);


                            // TO Ondrej Blok2 Washer Plate Top END
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            #endregion

                            #region Washer Bearing 
                            // TO Ondrej Blok3 Washer Bearing START
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            // Bearing Washer
                            try
                            {
                                if (anchor.WasherBearing != null)
                                    anchor.WasherBearing.SetParams(anchor.WasherBearing.Name, anchor.WasherBearing.m_ePlateSerieType_FS);
                            }
                            catch { };

                            QuotationHelper.AddPlateToQuotation(anchor.WasherBearing, quotation, anchorsNum, fCFS_PricePerKg_Plates_Total);

                            // TO Ondrej Blok3 Washer Bearing END
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            #endregion
                        }
                    }
                    //end temp
                }
            }

            return quotation;
        }

        public static List<CPlate> GetDifferentPlates(CModel model)
        {
            List<CPlate> plates = new List<CPlate>();
            List<QuotationItem> quotation = new List<QuotationItem>();
            int quotationCount = 0;

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                joint.SetJointIsSelectedForMaterialListAccordingToMember();

                if (!joint.BIsSelectedForMaterialList) continue;
                if (joint.m_arrPlates == null || joint.m_arrPlates.Length == 0) continue; // Spoj neobsahuje plates (napr. U001)

                foreach (CPlate plate in joint.m_arrPlates) // For each plate
                {

                    #region Base Plate
                    // TO Ondrej Blok1 Plate START
                    // ----------------------------------------------------------------------------------------------------------------------------------------
                    try
                    {
                        plate.SetParams(plate.Name, plate.m_ePlateSerieType_FS);
                    }
                    catch { };

                    quotationCount = quotation.Count;
                    QuotationHelper.AddPlateToQuotation(plate, quotation, 1, 0);
                    if (quotation.Count > quotationCount) { plates.Add(plate); }

                    // TO Ondrej Blok1 Plate END
                    // ----------------------------------------------------------------------------------------------------------------------------------------
                    #endregion

                    if (plate is CConCom_Plate_B_basic)
                    {
                        CConCom_Plate_B_basic plateB = (CConCom_Plate_B_basic)plate;

                        if (plateB.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                        {
                            CAnchor anchor = plateB.AnchorArrangement.Anchors.FirstOrDefault();
                            int anchorsNum = plateB.AnchorArrangement.Anchors.Length;

                            #region Washer Plate Top
                            // TO Ondrej Blok2 Washer Plate Top START
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            // Plate Top Washer
                            try
                            {
                                if (anchor.WasherPlateTop != null)
                                    anchor.WasherPlateTop.SetParams(anchor.WasherPlateTop.Name, anchor.WasherPlateTop.m_ePlateSerieType_FS);
                            }
                            catch { };

                            quotationCount = quotation.Count;
                            QuotationHelper.AddPlateToQuotation(anchor.WasherPlateTop, quotation, anchorsNum, 0);
                            if (quotation.Count > quotationCount) { plates.Add(anchor.WasherPlateTop); }

                            // TO Ondrej Blok2 Washer Plate Top END
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            #endregion

                            #region Washer Bearing 
                            // TO Ondrej Blok3 Washer Bearing START
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            // Bearing Washer
                            try
                            {
                                if (anchor.WasherBearing != null)
                                    anchor.WasherBearing.SetParams(anchor.WasherBearing.Name, anchor.WasherBearing.m_ePlateSerieType_FS);
                            }
                            catch { };

                            quotationCount = quotation.Count;
                            QuotationHelper.AddPlateToQuotation(anchor.WasherBearing, quotation, anchorsNum, 0);
                            if (quotation.Count > quotationCount) { plates.Add(anchor.WasherBearing); }
                            // TO Ondrej Blok3 Washer Bearing END
                            // ----------------------------------------------------------------------------------------------------------------------------------------
                            #endregion
                        }
                    }
                    //end temp
                }
            }

            return plates;
        }

        public static DataSet GetTableConnectors(CModel model, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            // Connectors
            // TASK 422
            // Neviem ci je to stastne ale chcel som usetrit datagridy a dat vsetky spojovacie prostriedky (rozne typy) do jednej tabulky            
            // Anchors + screws

            List<QuotationItem> quotation = new List<QuotationItem>();

            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                joint.SetJointIsSelectedForMaterialListAccordingToMember();

                if (!joint.BIsSelectedForMaterialList) continue;

                if (joint.m_arrPlates != null && joint.m_arrPlates.Length > 0) // Spoj obsahuje plates
                {
                    foreach (CPlate plate in joint.m_arrPlates) // For each plate
                    {
                        // Screws
                        if (plate.ScrewArrangement != null && plate.ScrewArrangement.Screws != null)
                        {
                            QuotationHelper.AddConnector(plate.ScrewArrangement.Screws.FirstOrDefault(), quotation, plate.ScrewArrangement.Screws.Length);
                        }

                        // Anchors
                        if (plate is CConCom_Plate_B_basic)
                        {
                            CConCom_Plate_B_basic plateB = (CConCom_Plate_B_basic)plate;

                            if (plateB.AnchorArrangement != null) // Base plate - obsahuje anchor arrangement
                            {
                                // TASK 422
                                // Doplnit data pre anchors
                                // Refaktorovat anchors a screws
                                // Pre Quantity asi zaviest Count a zjednotit nazov stlpca pre pocet vsade

                                // Pre screws - gauge + dlzka (14g - 38)
                                // Pre anchors  - name + dlzka (M16 - 330)

                                // Prefix | Quantity |     Material     | Size    |   Mass per Piece [kg] | Total Mass [kg] | Unit Price [NZD / piece] | Total Price [NZD]
                                // TEK    |     1515 | Class 3 / 4 / B8 |  14g-38 |                 0.052 |
                                // Anchor |       65 |              8.8 | M16-330 |                 2.241 |

                                QuotationHelper.AddConnector(plateB.AnchorArrangement.Anchors.FirstOrDefault(), quotation, plateB.AnchorArrangement.Anchors.Length);
                            }
                        }
                    }
                }

                if(joint.ConnectorGroups != null && joint.ConnectorGroups.Count > 0)
                {
                    CConnector screw = null;
                    int screwsCount = 0;
                    foreach (CConnectorGroup connectorGroup in joint.ConnectorGroups)
                    {
                        foreach (CConnector connector in connectorGroup.Connectors)
                        {
                            // Screws
                            if (connector is CScrew) // TODO Nemalo by sa posuzovat podla prveho objektu, mozu tam byt rozne, ale zatial to necham tak. Mal to byt cyklus cez vsetky polozky a roztriedit ich podla typu
                            {
                                screw = connector;
                            }
                            else throw new Exception("Not implemented type of connector.");
                        }
                    }
                    if(screw != null) QuotationHelper.AddConnector(screw, quotation, screwsCount);
                }
            }

            // Check Data
            //double dTotalConnectorsMass_Model = 0, dTotalConnectorsMass_Table = 0;
            //double dTotalConnectorsPrice_Model = 0, dTotalConnectorsPrice_Table = 0;
            //int iTotalConnectorsNumber_Model = 0, iTotalConnectorsNumber_Table = 0;

            ////toto sa mi zda,ze netreba
            //foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints)
            //{
            //    if (!joint.BIsSelectedForMaterialList) continue;

            //    foreach (CPlate plate in joint.m_arrPlates)
            //    {
            //        // Set connectors data
            //        if (plate.ScrewArrangement.Screws != null)
            //        {
            //            foreach (CConnector connector in plate.ScrewArrangement.Screws)
            //            {
            //                dTotalConnectorsMass_Model += connector.Mass;

            //                if (connector.Price_PPP_NZD > 0)
            //                    dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
            //                else
            //                    dTotalConnectorsPrice_Model += fTEK_PricePerPiece_Screws_Total;

            //                iTotalConnectorsNumber_Model += 1;
            //            }
            //        }

            //        if (plate is CConCom_Plate_B_basic)
            //        {
            //            CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

            //            if (basePlate.AnchorArrangement.Anchors != null)
            //            {
            //                foreach (CConnector connector in basePlate.AnchorArrangement.Anchors)
            //                {
            //                    dTotalConnectorsMass_Model += connector.Mass;

            //                    if (connector.Price_PPP_NZD > 0)
            //                        dTotalConnectorsPrice_Model += connector.Price_PPP_NZD;
            //                    else
            //                        dTotalConnectorsPrice_Model += (fAnchor_PricePerLength * connector.Length);

            //                    iTotalConnectorsNumber_Model += 1;
            //                }
            //            }
            //        }
            //    }
            //}

            double dTotalConnectorsMass_Table = 0;
            double dTotalConnectorsPrice_Table = 0;
            int iTotalConnectorsNumber_Table = 0;
            foreach (QuotationItem item in quotation)
            {
                dTotalConnectorsMass_Table += item.TotalMass;
                dTotalConnectorsPrice_Table += item.TotalPrice;
                iTotalConnectorsNumber_Table += item.Quantity;
            }

            //To Mato...toto tu treba???
            // TO Ondrej
            // Tie kontroly znikli pre to aby som mal istotu ze som vsetko spravne pridal z modelu do zoznamov
            // V debugu by sa nam to mohlo zist aby sme nic nevynechali ani neodfiltrovali

            //if (!MathF.d_equal(dTotalConnectorsMass_Model, dTotalConnectorsMass_Table) ||
            //        (iTotalConnectorsNumber_Model != iTotalConnectorsNumber_Table)) // Error
            //    MessageBox.Show(
            //    "Total weight of connectors in model " + dTotalConnectorsMass_Model + " kg" + "\n" +
            //    "Total weight of connectors in table " + dTotalConnectorsMass_Table + " kg" + "\n" +
            //    "Total number of connectors in model " + iTotalConnectorsNumber_Model + "\n" +
            //    "Total number of connectors in table " + iTotalConnectorsNumber_Table + "\n");

            //// Prepare output format (last row is empty)
            //for (int i = 0; i < listConnectorPrefix.Count; i++)
            //{
            //    // Change output data format
            //    listConnectorMassPerPiece.Add(dlistConnectorMassPerPiece[i].ToString("F2"));
            //}

            dBuildingMass += dTotalConnectorsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalConnectorsPrice_Table;

            // Create Table
            DataTable dt = new DataTable("Connectors (Anchors, Bolts, Screws, Rivets)");

            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Prefix.ColumnName, QuotationHelper.colProp_Prefix.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Material.ColumnName, QuotationHelper.colProp_Material.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Size.ColumnName, QuotationHelper.colProp_Size.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);
            DataRow row;
            foreach (QuotationItem item in quotation)
            {
                row = dt.NewRow();
                try
                {
                    row[QuotationHelper.colProp_Prefix.ColumnName] = item.Prefix;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_Material.ColumnName] = item.MaterialName;
                    row[QuotationHelper.colProp_Size.ColumnName] = item.Name;
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            // Add Sum
            row = dt.NewRow();
            row[QuotationHelper.colProp_Prefix.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalConnectorsNumber_Table;
            row[QuotationHelper.colProp_Material.ColumnName] = "";
            row[QuotationHelper.colProp_Size.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalConnectorsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalConnectorsPrice_Table.ToString("F2");
            dt.Rows.Add(row);

            return ds;
            //Datagrid_Connectors.ItemsSource = ds.Tables[0].AsDataView();
            //Datagrid_Connectors.Loaded += Datagrid_Connectors_Loaded;
        }

        public static DataSet GetTableCladdingSheets(CModel model, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            const float fCFS_PricePerKg_CladdingSheets_Material = 1.698f;    // NZD / kg
            const float fCFS_PricePerKg_CladdingSheets_Manufacture = 0.0f;   // NZD / kg
            float fCFS_PricePerKg_CladdingSheets_Total = fCFS_PricePerKg_CladdingSheets_Material + fCFS_PricePerKg_CladdingSheets_Manufacture;           // NZD / kg

            // Cladding Sheets
            List<QuotationItem> quotation = GetCladdingSheetsQuotation(model, fCFS_PricePerKg_CladdingSheets_Total);

            // Check Data
            double dTotalCladdingSheetsArea_Model = 0, dTotalCladdingSheetsArea_Table = 0;
            double dTotalCladdingSheetsVolume_Model = 0, dTotalCladdingSheetsVolume_Table = 0;
            double dTotalCladdingSheetsMass_Model = 0, dTotalCladdingSheetsMass_Table = 0;
            double dTotalCladdingSheetsPrice_Model = 0, dTotalCladdingSheetsPrice_Table = 0;
            int iTotalCladdingSheetsNumber_Model = 0, iTotalCladdingSheetsNumber_Table = 0;

            if (model.m_arrGOCladding == null) return null;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) throw new Exception("Cladding is empty.");

            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets()) 
            {
                if (!sheet.BIsSelectedForMaterialList) continue;

                // Do vykazu materialu jednotlivych sheets davame plochu brutto vratane overlap - k diskusii s NZ
                dTotalCladdingSheetsArea_Model += sheet.Area_brutto_Real;
                dTotalCladdingSheetsVolume_Model += sheet.Volume_brutto_Real;
                dTotalCladdingSheetsMass_Model += sheet.Mass_brutto_Real;

                if (sheet.Price_PPKG_NZD > 0)
                    dTotalCladdingSheetsPrice_Model += sheet.Mass_brutto_Real * sheet.Price_PPKG_NZD;
                else
                    dTotalCladdingSheetsPrice_Model += sheet.Mass_brutto_Real * fCFS_PricePerKg_CladdingSheets_Total;

                iTotalCladdingSheetsNumber_Model += 1;
            }

            foreach (QuotationItem item in quotation)
            {
                dTotalCladdingSheetsArea_Table += item.Area * item.Quantity;
                dTotalCladdingSheetsVolume_Table += item.Area * item.Quantity * item.Ft;
                dTotalCladdingSheetsMass_Table += item.TotalMass;
                dTotalCladdingSheetsPrice_Table += item.TotalPrice;
                iTotalCladdingSheetsNumber_Table += item.Quantity;
            }
            dBuildingMass += dTotalCladdingSheetsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalCladdingSheetsPrice_Table;

            // Create Table
            DataTable table = new DataTable("CladdingSheets");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Prefix.ColumnName, QuotationHelper.colProp_Prefix.DataType);
            table.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            table.Columns.Add(QuotationHelper.colProp_Material.ColumnName, QuotationHelper.colProp_Material.DataType);
            table.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Length_m.ColumnName, QuotationHelper.colProp_Length_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            table.Columns.Add(QuotationHelper.colProp_Area_m2.ColumnName, QuotationHelper.colProp_Area_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Prefix.ColumnName] = item.Prefix;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_Material.ColumnName] = item.MaterialName;
                    row[QuotationHelper.colProp_Width_m.ColumnName] = item.Width_bx.ToString("F2");
                    row[QuotationHelper.colProp_Length_m.ColumnName] = item.Length.ToString("F2");
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (1000 * item.Ft).ToString("F3"); // milimeters
                    row[QuotationHelper.colProp_Area_m2.ColumnName] = item.Area.ToString("F2");
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = item.TotalArea.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Prefix.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalCladdingSheetsNumber_Table;
            row[QuotationHelper.colProp_Material.ColumnName] = "";
            row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            row[QuotationHelper.colProp_Length_m.ColumnName] = "";
            row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
            row[QuotationHelper.colProp_Area_m2.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = dTotalCladdingSheetsArea_Table.ToString("F2");
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalCladdingSheetsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalCladdingSheetsPrice_Table.ToString("F2");
            table.Rows.Add(row);

            return ds;
        }

        public static List<QuotationItem> GetCladdingSheetsQuotation(CModel model, float fCFS_PricePerKg_CladdingSheets_Total)
        {
            List<QuotationItem> quotation = new List<QuotationItem>();
            if (model.m_arrGOCladding == null) return quotation;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return quotation;

            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets())
            {
                if (!sheet.BIsSelectedForMaterialList) continue;

                QuotationHelper.AddSheetToQuotation(sheet, quotation, 1, fCFS_PricePerKg_CladdingSheets_Total);
            }

            return quotation;
        }

        public static DataSet GetTableFibreglassSheets(CModel model, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            const float fCFS_PricePerKg_FibreglassSheets_Material = 9.500f;    // NZD / kg
            const float fCFS_PricePerKg_FibreglassSheets_Manufacture = 0.0f;   // NZD / kg
            float fCFS_PricePerKg_FibreglassSheets_Total = fCFS_PricePerKg_FibreglassSheets_Material + fCFS_PricePerKg_FibreglassSheets_Manufacture;           // NZD / kg

            // Cladding Sheets
            List<QuotationItem> quotation = GetFibreglassSheetsQuotation(model, fCFS_PricePerKg_FibreglassSheets_Total);

            // Check Data
            double dTotalFibreglassSheetsArea_Model = 0, dTotalFibreglassSheetsArea_Table = 0;
            double dTotalFibreglassSheetsVolume_Model = 0, dTotalFibreglassSheetsVolume_Table = 0;
            double dTotalFibreglassSheetsMass_Model = 0, dTotalFibreglassSheetsMass_Table = 0;
            double dTotalFibreglassSheetsPrice_Model = 0, dTotalFibreglassSheetsPrice_Table = 0;
            int iTotalFibreglassSheetsNumber_Model = 0, iTotalFibreglassSheetsNumber_Table = 0;

            if (model.m_arrGOCladding == null) return null;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) throw new Exception("Cladding is empty.");

            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets())
            {
                if (!sheet.BIsSelectedForMaterialList) continue;

                // Do vykazu materialu jednotlivych sheets davame plochu brutto vratane overlap - k diskusii s NZ
                dTotalFibreglassSheetsArea_Model += sheet.Area_brutto_Real;
                dTotalFibreglassSheetsVolume_Model += sheet.Volume_brutto_Real;
                dTotalFibreglassSheetsMass_Model += sheet.Mass_brutto_Real;

                if (sheet.Price_PPKG_NZD > 0)
                    dTotalFibreglassSheetsPrice_Model += sheet.Mass_brutto_Real * sheet.Price_PPKG_NZD;
                else
                    dTotalFibreglassSheetsPrice_Model += sheet.Mass_brutto_Real * fCFS_PricePerKg_FibreglassSheets_Total;

                iTotalFibreglassSheetsNumber_Model += 1;
            }

            foreach (QuotationItem item in quotation)
            {
                dTotalFibreglassSheetsArea_Table += item.Area * item.Quantity;
                dTotalFibreglassSheetsVolume_Table += item.Area * item.Quantity * item.Ft;
                dTotalFibreglassSheetsMass_Table += item.TotalMass;
                dTotalFibreglassSheetsPrice_Table += item.TotalPrice;
                iTotalFibreglassSheetsNumber_Table += item.Quantity;
            }
            dBuildingMass += dTotalFibreglassSheetsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalFibreglassSheetsPrice_Table;

            // Create Table
            DataTable table = new DataTable("FibreglassSheets");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Prefix.ColumnName, QuotationHelper.colProp_Prefix.DataType);
            table.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            //table.Columns.Add(QuotationHelper.colProp_Material.ColumnName, QuotationHelper.colProp_Material.DataType);
            table.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Length_m.ColumnName, QuotationHelper.colProp_Length_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            table.Columns.Add(QuotationHelper.colProp_Area_m2.ColumnName, QuotationHelper.colProp_Area_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Prefix.ColumnName] = item.Prefix;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    //row[QuotationHelper.colProp_Material.ColumnName] = item.MaterialName;
                    row[QuotationHelper.colProp_Width_m.ColumnName] = item.Width_bx.ToString("F2");
                    row[QuotationHelper.colProp_Length_m.ColumnName] = item.Length.ToString("F2");
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (1000 * item.Ft).ToString("F3"); // milimeters
                    row[QuotationHelper.colProp_Area_m2.ColumnName] = item.Area.ToString("F2");
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = item.TotalArea.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Prefix.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalFibreglassSheetsNumber_Table;
            //row[QuotationHelper.colProp_Material.ColumnName] = "";
            row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            row[QuotationHelper.colProp_Length_m.ColumnName] = "";
            row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
            row[QuotationHelper.colProp_Area_m2.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = dTotalFibreglassSheetsArea_Table.ToString("F2");
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalFibreglassSheetsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalFibreglassSheetsPrice_Table.ToString("F2");
            table.Rows.Add(row);

            return ds;
        }

        public static List<QuotationItem> GetFibreglassSheetsQuotation(CModel model, float fCFS_PricePerKg_FibreglassSheets_Total)
        {
            List<QuotationItem> quotation = new List<QuotationItem>();
            if (model.m_arrGOCladding == null) return quotation;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return quotation;
            
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetFibreglassSheets())
            {
                if (!sheet.BIsSelectedForMaterialList) continue;

                QuotationHelper.AddSheetToQuotation(sheet, quotation, 1, fCFS_PricePerKg_FibreglassSheets_Total);
            }

            return quotation;
        }



        public static DataSet GetTableRoofNetting(float fRoofArea, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            List<CPlaneItemProperties> listOfProperties = CPlaneItemManager.LoadPlaneItemsProperties("RoofNetting");

            // Roof Netting and Sisalation
            // Roof Sisalation Foil
            // Roof Safe Net
            float fRoofSisalationFoilPrice_PSM_NZD = (float)listOfProperties[0].Price1_PPSM_NZD; // Cena roof foil za 1 m^2
            float fRoofSafeNetPrice_PSM_NZD = (float)listOfProperties[1].Price1_PPSM_NZD; // Cena roof net za 1 m^2

            float fRoofSisalationFoilUnitMass_SM = (float)listOfProperties[0].Mass_kg_m2;
            float fRoofSafeNetUnitMass_SM = (float)listOfProperties[1].Mass_kg_m2;

            float fRoofSisalationFoilPrice_Total_NZD = fRoofArea * fRoofSisalationFoilPrice_PSM_NZD;
            float fRoofSafeNetPrice_Total_NZD = fRoofArea * fRoofSafeNetPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("Roof Netting");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Component.ColumnName, QuotationHelper.colProp_Component.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_SM.ColumnName, QuotationHelper.colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            AddSurfaceItemRow(dt,
                        QuotationHelper.colProp_Component.ColumnName,
                        listOfProperties[0].Name,
                        fRoofArea,
                        fRoofSisalationFoilUnitMass_SM,
                        fRoofSisalationFoilUnitMass_SM * fRoofArea,
                        fRoofSisalationFoilPrice_PSM_NZD,
                        fRoofSisalationFoilPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            AddSurfaceItemRow(dt,
                        QuotationHelper.colProp_Component.ColumnName,
                        listOfProperties[1].Name,
                        fRoofArea,
                        fRoofSafeNetUnitMass_SM,
                        fRoofSafeNetUnitMass_SM * fRoofArea,
                        fRoofSafeNetPrice_PSM_NZD,
                        fRoofSafeNetPrice_Total_NZD,
                        ref SumTotalArea,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            DataRow row;
            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[QuotationHelper.colProp_Component.ColumnName] = "Total:";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = "";
                row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);
            }
            return ds;
        }

        public static DataSet GetTableDoorsAndWindows(CPFDViewModel vm, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST, 
            out float fTotalAreaOfOpennings)
        {
            fTotalAreaOfOpennings = 0;

            List<COpeningProperties> listOfOpenings = new List<COpeningProperties>();

            if (vm._doorsAndWindowsVM != null && QuotationHelper.DisplayDoorsAndWindowsTable(vm))
            {
                foreach (DoorProperties dp in vm._doorsAndWindowsVM.DoorBlocksProperties)
                {
                    fTotalAreaOfOpennings += dp.fDoorsWidth * dp.fDoorsHeight;

                    listOfOpenings.Add(new COpeningProperties(dp.sDoorType, dp.fDoorsWidth, dp.fDoorsHeight, dp.CoatingColor.ID, dp.Serie));
                }

                foreach (WindowProperties wp in vm._doorsAndWindowsVM.WindowBlocksProperties)
                {
                    fTotalAreaOfOpennings += wp.fWindowsWidth * wp.fWindowsHeight;

                    listOfOpenings.Add(new COpeningProperties("Window", wp.fWindowsWidth, wp.fWindowsHeight, wp.CoatingColor.ID, null));
                }
            }

            // TODO Ondrej

            // Refaktorovat kody
            // Skus to popozerat a pripadne nejako zautomatizovat
            // V principe mame 2 typy poloziek
            // 1 - definovane dlzkou (flashings, gutters, mozno sa da uvazovat aj fibreglass)
            // 2 - definovene plochou (doors, windows, roof netting)

            List<COpeningProperties> groupedOpenings = new List<COpeningProperties>();
            foreach (COpeningProperties op in listOfOpenings)
            {
                if (groupedOpenings.Contains(op))
                {
                    COpeningProperties grOP = groupedOpenings[groupedOpenings.IndexOf(op)];
                    grOP.Count++;
                }
                else groupedOpenings.Add(op);
            }

            // Create Table
            DataTable dt = new DataTable("Doors and Windows");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Opening.ColumnName, QuotationHelper.colProp_Opening.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Height_m.ColumnName, QuotationHelper.colProp_Height_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Area_m2.ColumnName, QuotationHelper.colProp_Area_m2.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_SM.ColumnName, QuotationHelper.colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            int SumCount = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (COpeningProperties prop in groupedOpenings)
            {
                AddOpeningItemRow(dt,
                            QuotationHelper.colProp_Opening.ColumnName,
                            prop.Type,
                            prop.fWidth,
                            prop.fHeight,
                            prop.Count,
                            prop.Area,
                            prop.Area * prop.Count,
                            prop.UnitMass_SM,
                            prop.UnitMass_SM * prop.Area,
                            prop.Price_PPSM_NZD,
                            prop.Price_PPP_NZD,
                            prop.Price_PPSM_NZD * prop.Area * prop.Count,
                            ref SumCount,
                            ref SumTotalArea,
                            ref SumTotalMass,
                            ref SumTotalPrice);
            }

            DataRow row;
            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[QuotationHelper.colProp_Opening.ColumnName] = "Total:";
                row[QuotationHelper.colProp_Width_m.ColumnName] = "";
                row[QuotationHelper.colProp_Height_m.ColumnName] = "";
                row[QuotationHelper.colProp_Count.ColumnName] = SumCount.ToString();
                row[QuotationHelper.colProp_Area_m2.ColumnName] = "";
                row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = "";
                row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
                row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                return ds;
            }
            else // Tabulka je prazdna - nezobrazime ju
            {
                return null;
            }
        }

        public static DataSet GetTableCladding(CPFDViewModel vm, float fTotalAreaOfOpennings, float fFibreGlassArea_Walls, float fFibreGlassArea_Roof, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            // Plocha stien bez otvorov a fibre glass
            float fWallArea_Total_Netto = vm.TotalWallArea - fTotalAreaOfOpennings - fFibreGlassArea_Walls;  //float fWallArea_Total,

            // Plocha strechy bez fibre glass
            float fRoofArea_Total_Netto = vm.TotalRoofAreaInclCanopies - fFibreGlassArea_Roof;    //float fRoofArea,

            CoatingColour prop_RoofCladdingColor = vm._claddingOptionsVM.RoofCladdingColors.ElementAtOrDefault(vm._claddingOptionsVM.RoofCladdingColorIndex);
            CoatingColour prop_WallCladdingColor = vm._claddingOptionsVM.WallCladdingColors.ElementAtOrDefault(vm._claddingOptionsVM.WallCladdingColorIndex);

            float fRoofCladdingUnitMass_kg_m2 = (float)(vm._claddingOptionsVM.RoofCladdingCoilProps.mass_kg_lm / vm._claddingOptionsVM.RoofCladdingProps.widthModular_m);
            float fWallCladdingUnitMass_kg_m2 = (float)(vm._claddingOptionsVM.WallCladdingCoilProps.mass_kg_lm / vm._claddingOptionsVM.WallCladdingProps.widthModular_m);

            float fRoofCladdingPrice_PSM_NZD = (float)(vm._claddingOptionsVM.RoofCladdingCoilProps.price_PPLM_NZD / vm._claddingOptionsVM.RoofCladdingProps.widthModular_m);
            float fWallCladdingPrice_PSM_NZD = (float)(vm._claddingOptionsVM.WallCladdingCoilProps.price_PPLM_NZD / vm._claddingOptionsVM.WallCladdingProps.widthModular_m);

            float fRoofCladdingPrice_Total_NZD = fRoofArea_Total_Netto * fRoofCladdingPrice_PSM_NZD;
            float fWallCladdingPrice_Total_NZD = fWallArea_Total_Netto * fWallCladdingPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("Cladding");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Cladding.ColumnName, QuotationHelper.colProp_Cladding.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Coating.ColumnName, QuotationHelper.colProp_Coating.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Color.ColumnName, QuotationHelper.colProp_Color.DataType);
            dt.Columns.Add(QuotationHelper.colProp_ColorName.ColumnName, QuotationHelper.colProp_ColorName.DataType);
            //dt.Columns.Add("TotalLength", typeof(String)); // Dalo by sa spocitat ak podelime plochu sirkou profilu
            dt.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_SM.ColumnName, QuotationHelper.colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            // double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;

            if (fRoofArea_Total_Netto > 0 && vm.ModelHasRoof()) // Roof Cladding   //CModelHelper.ModelHasCladding_Roof(vm.Model)
            {
                row = dt.NewRow();

                float fUnitMass = fRoofCladdingUnitMass_kg_m2;
                float totalMass = fRoofArea_Total_Netto * fUnitMass;
                try
                {
                    row[QuotationHelper.colProp_Cladding.ColumnName] = vm._claddingOptionsVM.RoofCladding;
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (vm._claddingOptionsVM.RoofCladdingProps.thicknessCore_m * 1000).ToString("F2"); // mm
                    row[QuotationHelper.colProp_Coating.ColumnName] = vm._claddingOptionsVM.RoofCladdingCoating;
                    row[QuotationHelper.colProp_Color.ColumnName] = prop_RoofCladdingColor.CodeHEX;
                    row[QuotationHelper.colProp_ColorName.ColumnName] = prop_RoofCladdingColor.Name;
                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = fRoofArea_Total_Netto.ToString("F2");
                    SumTotalArea += fRoofArea_Total_Netto;

                    row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = fRoofCladdingPrice_PSM_NZD.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = fRoofCladdingPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fRoofCladdingPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (fWallArea_Total_Netto > 0 && vm.ModelHasWalls()) // Wall Cladding  // CModelHelper.ModelHasCladding_Wall(vm.Model)
            {
                row = dt.NewRow();

                float fUnitMass = fWallCladdingUnitMass_kg_m2;
                float totalMass = fWallArea_Total_Netto * fUnitMass;
                try
                {
                    row[QuotationHelper.colProp_Cladding.ColumnName] = vm._claddingOptionsVM.WallCladding;
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (vm._claddingOptionsVM.WallCladdingProps.thicknessCore_m * 1000).ToString("F2"); // mm
                    row[QuotationHelper.colProp_Coating.ColumnName] = vm._claddingOptionsVM.WallCladdingCoating;
                    row[QuotationHelper.colProp_Color.ColumnName] = prop_WallCladdingColor.CodeHEX;
                    row[QuotationHelper.colProp_ColorName.ColumnName] = prop_WallCladdingColor.Name;

                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = fWallArea_Total_Netto.ToString("F2");
                    SumTotalArea += fWallArea_Total_Netto;

                    row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = fWallCladdingPrice_PSM_NZD.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = fWallCladdingPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fWallCladdingPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[QuotationHelper.colProp_Cladding.ColumnName] = "Total:";
                row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
                row[QuotationHelper.colProp_Coating.ColumnName] = "";
                row[QuotationHelper.colProp_Color.ColumnName] = "";
                row[QuotationHelper.colProp_ColorName.ColumnName] = "";
                //row["TotalLength"] = SumTotalLength.ToString("F2");
                row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = "";
                row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);
            }
            return ds;
        }

        public static DataSet GetTableFibreglass(CPFDViewModel vm, float fFibreGlassArea_Roof, float fFibreGlassArea_Walls, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            string roofFibreglassThickness = vm._claddingOptionsVM.RoofFibreglassThicknessTypes.ElementAtOrDefault(vm._claddingOptionsVM.RoofFibreglassThicknessIndex);
            string wallFibreglassThickness = vm._claddingOptionsVM.WallFibreglassThicknessTypes.ElementAtOrDefault(vm._claddingOptionsVM.WallFibreglassThicknessIndex);

            CFibreglassProperties prop_RoofFibreglass = new CFibreglassProperties();
            prop_RoofFibreglass = CFibreglassManager.GetFibreglassProperties($"{vm._claddingOptionsVM.RoofCladding}-{roofFibreglassThickness}");

            CFibreglassProperties prop_WallFibreglass = new CFibreglassProperties();
            prop_WallFibreglass = CFibreglassManager.GetFibreglassProperties($"{vm._claddingOptionsVM.WallCladding}-{wallFibreglassThickness}");

            float fRoofFibreGlassPrice_PSM_NZD = (float)prop_RoofFibreglass.price_PPSM_NZD; // Cena roof fibreglass za 1 m^2
            float fWallFibreGlassPrice_PSM_NZD = (float)prop_WallFibreglass.price_PPSM_NZD; // Cena wall fibreglass za 1 m^2

            float fRoofFibreGlassUnitMass_SM = (float)prop_RoofFibreglass.mass_kg_m2;
            float fWallFibreGlassUnitMass_SM = (float)prop_WallFibreglass.mass_kg_m2;

            float fRoofFibreGlassPrice_Total_NZD = fFibreGlassArea_Roof * fRoofFibreGlassPrice_PSM_NZD;
            float fWallFibreGlassPrice_Total_NZD = fFibreGlassArea_Walls * fWallFibreGlassPrice_PSM_NZD;

            // Create Table
            DataTable dt = new DataTable("Fibreglass");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Fibreglass.ColumnName, QuotationHelper.colProp_Fibreglass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalLength_m.ColumnName, QuotationHelper.colProp_TotalLength_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalArea_m2.ColumnName, QuotationHelper.colProp_TotalArea_m2.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_SM.ColumnName, QuotationHelper.colProp_UnitMass_SM.DataType); // kg / m^2
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_SM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalArea = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;

            if (fFibreGlassArea_Roof > 0 && vm.ModelHasRoof()) // Roof Cladding  //CModelHelper.ModelHasFibreglass_Roof(vm.Model)
            {
                row = dt.NewRow();

                float totalLength = fFibreGlassArea_Roof / (float)prop_RoofFibreglass.widthModular_m;
                float fUnitMass = fRoofFibreGlassUnitMass_SM;
                float totalMass = fFibreGlassArea_Roof * fUnitMass;
                try
                {
                    row[QuotationHelper.colProp_Fibreglass.ColumnName] = vm._claddingOptionsVM.RoofCladding;
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (prop_RoofFibreglass.thickness_m * 1000).ToString("F2"); // mm
                    row[QuotationHelper.colProp_Width_m.ColumnName] = prop_RoofFibreglass.widthModular_m.ToString("F2");
                    row[QuotationHelper.colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = fFibreGlassArea_Roof.ToString("F2");
                    SumTotalArea += fFibreGlassArea_Roof;

                    row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = fRoofFibreGlassPrice_PSM_NZD.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = fRoofFibreGlassPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fRoofFibreGlassPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (fFibreGlassArea_Walls > 0 && vm.ModelHasWalls()) // Wall Cladding  //CModelHelper.ModelHasFibreglass_Wall(vm.Model)
            {
                row = dt.NewRow();

                float totalLength = fFibreGlassArea_Walls / (float)prop_WallFibreglass.widthModular_m;
                float fUnitMass = fWallFibreGlassUnitMass_SM;
                float totalMass = fFibreGlassArea_Walls * fUnitMass;
                try
                {
                    row[QuotationHelper.colProp_Fibreglass.ColumnName] = vm._claddingOptionsVM.WallCladding;
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (prop_WallFibreglass.thickness_m * 1000).ToString("F2"); // mm
                    row[QuotationHelper.colProp_Width_m.ColumnName] = prop_WallFibreglass.widthModular_m.ToString("F2");
                    row[QuotationHelper.colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = fFibreGlassArea_Walls.ToString("F2");
                    SumTotalArea += fFibreGlassArea_Walls;

                    row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = fUnitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = fWallFibreGlassPrice_PSM_NZD.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = fWallFibreGlassPrice_Total_NZD.ToString("F2");
                    SumTotalPrice += fWallFibreGlassPrice_Total_NZD;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            if (SumTotalPrice > 0)
            {
                dBuildingMass += SumTotalMass;
                dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

                // Last row
                row = dt.NewRow();
                row[QuotationHelper.colProp_Fibreglass.ColumnName] = "Total:";
                row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
                row[QuotationHelper.colProp_Width_m.ColumnName] = "";
                row[QuotationHelper.colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
                row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = SumTotalArea.ToString("F2");
                row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = "";
                row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
                row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = "";
                row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
                dt.Rows.Add(row);

                return ds;
            }
            else // Tabulka je prazdna - nezobrazime ju
            {
                return null;
            }
        }

        public static DataSet GetTableGutters(CPFDViewModel vm, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            //-----------------------------------------------------------------------------------
            // TO Ondrej - toto by som presunul do CPFDViewModel
            float fGuttersTotalLength = 0;

            if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                fGuttersTotalLength = 1 * vm.RoofLength_Y; // na jednej hrane strechy (podla toho ci je mensia H1 alebo H2), ale pre dlzku gutter to nehra rolu
            }
            else if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                fGuttersTotalLength = 2 * vm.RoofLength_Y; // na dvoch okrajoch strechy
            }
            else
            {
                // Exception - not implemented
                fGuttersTotalLength = 0;
            }

            //toto tu je len preto ak by sa nahodou neupdatoval gutters total length pri zmene modelu (mozno je aj lepsie to mat az tu)
            //_pfdVM.Gutters[0].Length_total = fGuttersTotalLength;
            //-----------------------------------------------------------------------------------

            // Create Table
            DataTable dt = new DataTable("Gutters");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Gutter.ColumnName, QuotationHelper.colProp_Gutter.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Color.ColumnName, QuotationHelper.colProp_Color.DataType);
            dt.Columns.Add(QuotationHelper.colProp_ColorName.ColumnName, QuotationHelper.colProp_ColorName.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalLength_m.ColumnName, QuotationHelper.colProp_TotalLength_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (CAccessories_LengthItemProperties gutter in vm._doorsAndWindowsVM.Gutters)
            {
                gutter.Length_total = fGuttersTotalLength; // To Ondrej - moze to byt takto?

                AddLengthItemRow(dt,
                        QuotationHelper.colProp_Gutter.ColumnName,
                        gutter.Name,
                        gutter.Thickness / 1000, // from [mm] to [m]
                        gutter.Width_total,
                        gutter.CoatingColor,
                        gutter.Length_total,
                        gutter.Mass_kg_lm,
                        gutter.Mass_kg_lm * gutter.Length_total,
                        gutter.Price_PPLM_NZD,
                        gutter.Price_PPLM_NZD * gutter.Length_total,
                        ref SumTotalLength,
                        ref SumTotalMass,
                        ref SumTotalPrice);

            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            //if (dt.Rows.Count > 1) // Len ak su v tabulke rozne typy gutters // Zatial komentujem, dal by sa tym usetrit jeden riadok
            //{
            // Last row
            DataRow row;
            row = dt.NewRow();
            row[QuotationHelper.colProp_Gutter.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
            row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            row[QuotationHelper.colProp_Color.ColumnName] = "";
            row[QuotationHelper.colProp_ColorName.ColumnName] = "";
            row[QuotationHelper.colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);
            //}

            return ds;
        }

        public static DataSet GetTableDownpipes(CPFDViewModel vm, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            //-----------------------------------------------------------------------------------
            // TO Ondrej - toto by som presunul do CPFDViewModel
            // Zatial bude natvrdo jeden riadok s poctom zvodov, prednastavenou dlzkou ako vyskou steny a farbou, rovnaky default ako gutter
            CAccessories_DownpipeProperties downpipe = vm._doorsAndWindowsVM.Downpipes[0];
            float fDownpipesTotalLength = 0;

            if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                fDownpipesTotalLength = downpipe.CountOfDownpipePoints * Math.Min(vm.WallHeightOverall, vm.Height_H2_Overall); // Pocet zvodov krat mensia z vysok stien vlavo a vpravo (H1 alebo H2)
            }
            else if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                fDownpipesTotalLength = downpipe.CountOfDownpipePoints * vm.WallHeightOverall; // Pocet zvodov krat vyska steny
            }
            else
            {
                // Exception - not implemented
                fDownpipesTotalLength = 0;
            }

            downpipe.Length_total = fDownpipesTotalLength;
            //------------------------------------------------------------------------------------

            double fDownpipesTotalMass = fDownpipesTotalLength * downpipe.Mass_kg_lm;
            double fDownpipesTotalPrice = fDownpipesTotalLength * downpipe.Price_PPLM_NZD;

            // Create Table
            DataTable dt = new DataTable("Downpipes");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Downpipe.ColumnName, QuotationHelper.colProp_Downpipe.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Diameter_mm.ColumnName, QuotationHelper.colProp_Diameter_mm.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Color.ColumnName, QuotationHelper.colProp_Color.DataType);
            dt.Columns.Add(QuotationHelper.colProp_ColorName.ColumnName, QuotationHelper.colProp_ColorName.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalLength_m.ColumnName, QuotationHelper.colProp_TotalLength_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            DataRow row;

            if (fDownpipesTotalLength > 0 && fDownpipesTotalPrice > 0) // Add new row only if length and price are more than zero
            {
                row = dt.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Downpipe.ColumnName] = downpipe.Name;
                    row[QuotationHelper.colProp_Diameter_mm.ColumnName] = (downpipe.Diameter * 1000f).ToString("F2"); // mm
                    row[QuotationHelper.colProp_Color.ColumnName] = downpipe.CoatingColor.CodeHEX;
                    row[QuotationHelper.colProp_ColorName.ColumnName] = downpipe.CoatingColor.Name;
                    row[QuotationHelper.colProp_TotalLength_m.ColumnName] = fDownpipesTotalLength.ToString("F2");
                    SumTotalLength += fDownpipesTotalLength;

                    row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = downpipe.Mass_kg_lm.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = fDownpipesTotalMass.ToString("F2");
                    SumTotalMass += fDownpipesTotalMass;

                    row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = downpipe.Price_PPLM_NZD.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = fDownpipesTotalPrice.ToString("F2");
                    SumTotalPrice += fDownpipesTotalPrice;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            //if (dt.Rows.Count > 1) // Len ak su v tabulke rozne typy downpipes // Zatial komentujem, dal by sa tym usetrit jeden riadok
            //{
            // Last row
            row = dt.NewRow();
            row[QuotationHelper.colProp_Downpipe.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Diameter_mm.ColumnName] = "";
            row[QuotationHelper.colProp_Color.ColumnName] = "";
            row[QuotationHelper.colProp_ColorName.ColumnName] = "";
            row[QuotationHelper.colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);
            //}

            return ds;
        }

        public static DataSet GetTableFlashing(CPFDViewModel vm, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            vm.CountFlashings();
            vm.SetFlashingsLengths();

            // Create Table
            DataTable dt = new DataTable("Flashings");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Flashing.ColumnName, QuotationHelper.colProp_Flashing.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Color.ColumnName, QuotationHelper.colProp_Color.DataType);
            dt.Columns.Add(QuotationHelper.colProp_ColorName.ColumnName, QuotationHelper.colProp_ColorName.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalLength_m.ColumnName, QuotationHelper.colProp_TotalLength_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            foreach (CAccessories_LengthItemProperties fl in vm._doorsAndWindowsVM.Flashings)
            {
                AddLengthItemRow(dt,
                            QuotationHelper.colProp_Flashing.ColumnName,
                            fl.Name,
                            fl.Thickness / 1000, //from [mm] to [m]
                            fl.Width_total,
                            fl.CoatingColor,
                            fl.Length_total,
                            fl.Mass_kg_lm,
                            fl.Mass_kg_lm * fl.Length_total,
                            fl.Price_PPLM_NZD,
                            fl.Price_PPLM_NZD * fl.Length_total,
                            ref SumTotalLength,
                            ref SumTotalMass,
                            ref SumTotalPrice);
            }

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            // Last row
            DataRow row;
            row = dt.NewRow();
            row[QuotationHelper.colProp_Flashing.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
            row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            row[QuotationHelper.colProp_Color.ColumnName] = "";
            row[QuotationHelper.colProp_ColorName.ColumnName] = "";
            row[QuotationHelper.colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
            dt.Rows.Add(row);

            return ds;
        }

        public static DataSet GetTablePackers(float fRollerDoorLintelFlashing_TotalLength, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            // Create Table
            DataTable dt = new DataTable("Packers");
            // Create Table Rows
            dt.Columns.Add(QuotationHelper.colProp_Packer.ColumnName, QuotationHelper.colProp_Packer.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Thickness_mm.ColumnName, QuotationHelper.colProp_Thickness_mm.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Width_m.ColumnName, QuotationHelper.colProp_Width_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_Color.ColumnName, QuotationHelper.colProp_Color.DataType);
            dt.Columns.Add(QuotationHelper.colProp_ColorName.ColumnName, QuotationHelper.colProp_ColorName.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalLength_m.ColumnName, QuotationHelper.colProp_TotalLength_m.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            dt.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            dt.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(dt);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);

            double SumTotalLength = 0;
            double SumTotalMass = 0;
            double SumTotalPrice = 0;

            CAccessories_LengthItemProperties packer = new CAccessories_LengthItemProperties("Roller Door Channel Packer 70x1 mm", "Packers", fRollerDoorLintelFlashing_TotalLength, 23);

            AddLengthItemRow(dt,
                            QuotationHelper.colProp_Packer.ColumnName,
                            packer.Name,
                            packer.Thickness / 1000, //from [mm] to [m]
                            packer.Width_total,
                            packer.CoatingColor,
                            packer.Length_total,
                            packer.Mass_kg_lm,
                            packer.Mass_kg_lm * packer.Length_total,
                            packer.Price_PPLM_NZD,
                            packer.Price_PPLM_NZD * packer.Length_total,
                            ref SumTotalLength,
                            ref SumTotalMass,
                            ref SumTotalPrice);

            dBuildingMass += SumTotalMass;
            dBuildingNetPrice_WithoutMargin_WithoutGST += SumTotalPrice;

            //// Last row
            //DataRow row;
            //row = dt.NewRow();
            //row[QuotationHelper.colProp_Packer.ColumnName] = "Total:";
            //row[QuotationHelper.colProp_Thickness_mm.ColumnName] = "";
            //row[QuotationHelper.colProp_Width_m.ColumnName] = "";
            //row[QuotationHelper.colProp_Color.ColumnName] = "";
            //row[QuotationHelper.colProp_ColorName.ColumnName] = "";
            //row[QuotationHelper.colProp_TotalLength_m.ColumnName] = SumTotalLength.ToString("F2");
            //row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            //row[QuotationHelper.colProp_TotalMass.ColumnName] = SumTotalMass.ToString("F2");
            //row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            //row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = SumTotalPrice.ToString("F2");
            //dt.Rows.Add(row);
            if (SumTotalLength > 0 && SumTotalPrice > 0) return ds;
            else return null;
        }



        private static void AddSurfaceItemRow(DataTable dt,
            string itemColumnName,
            string itemName,
            double totalArea,
            double unitMass,
            double totalMass,
            double unitPrice,
            double price,
            ref double SumTotalArea,
            ref double SumTotalMass,
            ref double SumTotalPrice)
        {
            if (totalArea > 0 && price > 0) // Add new row only if area and price are more than zero
            {
                DataRow row;

                row = dt.NewRow();

                try
                {
                    row[itemColumnName] = itemName;

                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = totalArea.ToString("F2");
                    SumTotalArea += totalArea;

                    row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = unitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = unitPrice.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }

        private static void AddOpeningItemRow(DataTable dt,
                string itemColumnName,
                string itemName,
                double width,
                double height,
                int count,
                double area,
                double totalArea,
                double unitMass,
                double totalMass,
                double unitPrice_PPSM,
                double unitPrice_PPP,
                double price,
                ref int SumCount,
                ref double SumTotalArea,
                ref double SumTotalMass,
                ref double SumTotalPrice)
        {
            if (totalArea > 0 && price > 0) // Add new row only if area and price are more than zero
            {
                DataRow row;

                row = dt.NewRow();

                try
                {
                    row[itemColumnName] = itemName;

                    row[QuotationHelper.colProp_Width_m.ColumnName] = width.ToString("F2");
                    row[QuotationHelper.colProp_Height_m.ColumnName] = height.ToString("F2");
                    row[QuotationHelper.colProp_Count.ColumnName] = count.ToString();
                    SumCount += count;

                    row[QuotationHelper.colProp_Area_m2.ColumnName] = area.ToString("F2");

                    row[QuotationHelper.colProp_TotalArea_m2.ColumnName] = totalArea.ToString("F2");
                    SumTotalArea += totalArea;

                    row[QuotationHelper.colProp_UnitMass_SM.ColumnName] = unitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_SM_NZD.ColumnName] = unitPrice_PPSM.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = unitPrice_PPP.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }

        private static void AddLengthItemRow(DataTable dt,
            string itemColumnName,
            string itemName,
            double thickness_m,
            double width,
            CoatingColour coatingColor,
            double totalLength,
            double unitMass,
            double totalMass,
            double unitPrice,
            double price,
            ref double SumTotalLength,
            ref double SumTotalMass,
            ref double SumTotalPrice)
        {
            if (totalLength > 0 && price > 0) // Add new row only if length and price are more than zero
            {
                DataRow row;

                row = dt.NewRow();

                try
                {
                    row[itemColumnName] = itemName;
                    row[QuotationHelper.colProp_Thickness_mm.ColumnName] = (thickness_m * 1000).ToString("F2"); // mm
                    row[QuotationHelper.colProp_Width_m.ColumnName] = width.ToString("F2");
                    row[QuotationHelper.colProp_Color.ColumnName] = coatingColor.CodeHEX;
                    row[QuotationHelper.colProp_ColorName.ColumnName] = coatingColor.Name;
                    row[QuotationHelper.colProp_TotalLength_m.ColumnName] = totalLength.ToString("F2");
                    SumTotalLength += totalLength;

                    row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = unitMass.ToString("F2");

                    row[QuotationHelper.colProp_TotalMass.ColumnName] = totalMass.ToString("F2");
                    SumTotalMass += totalMass;

                    row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = unitPrice.ToString("F2");

                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = price.ToString("F2");
                    SumTotalPrice += price;
                }
                catch (ArgumentOutOfRangeException) { }
                dt.Rows.Add(row);
            }
        }



        public static List<CCladdingAccessories_Item_Piece> GetQuotationCladdingAccessoriesItems_Piece(List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_PieceFromPartList)
        {
            List<CCladdingAccessories_Item_Piece> items = new List<CCladdingAccessories_Item_Piece>();

            Dictionary<int, CCladdingAccessories_Fixing_Properties> dict_Fixings = CCladdingAccessoriesManager.GetFixingPropertiesDict();
            foreach (CCladdingAccessories_Item_Piece item in claddingAccessoriesItems_PieceFromPartList)
            {
                if (item.ItemProp.IsFixingItem)
                {
                    foreach (int fixingID in item.ItemProp.FixingIDsArray)
                    {
                        CCladdingAccessories_Fixing_Properties f = null;
                        dict_Fixings.TryGetValue(fixingID, out f);
                        if (f != null)
                        {
                            CCladdingAccessories_Item_Piece fixingExists = items.FirstOrDefault(i => i.Name == f.Name);
                            if (fixingExists == null) items.Add(new CCladdingAccessories_Item_Piece(f, item.Count));
                            else fixingExists.Count += item.Count;
                        }                        
                    }
                }
                else items.Add(item);
            }

            return items;
        }



        // Add cladding or fibreglass sheet
        public static void AddSheetToQuotation(CCladdingOrFibreGlassSheet sheet, List<QuotationItem> quotation, int iQuantity, float fCFS_PricePerKg_CladdingSheets_Total)
        {
            if (sheet == null) return;

            // Do vykazu materialu jednotlivych sheets davame plochu brutto vratane overlap - k diskusii s NZ
            float fMassPerPiece = sheet.Mass_brutto_Real;
            float fPricePerPiece = sheet.Price_PPKG_NZD > 0 ? (float)sheet.Price_PPKG_NZD * fMassPerPiece : fCFS_PricePerKg_CladdingSheets_Total * fMassPerPiece;

            QuotationItem qItem = quotation.FirstOrDefault(q => q.Prefix == sheet.Prefix &&
                    MathF.d_equal(q.Width_bx, sheet.Width) &&
                    MathF.d_equal(q.Length, sheet.LengthTotal_Real) &&
                    MathF.d_equal(q.Ft, sheet.Ft) &&
                    MathF.d_equal(q.Area, sheet.Area_brutto_Real));
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
                    Prefix = sheet.Prefix,
                    Quantity = iQuantity,
                    Width_bx = (float)sheet.Width,
                    Length = (float)sheet.LengthTotal_Real,
                    Ft = sheet.Ft,
                    MaterialName = sheet.m_Mat.Name,
                    Area = (float)sheet.Area_brutto_Real,
                    MassPerPiece = fMassPerPiece,
                    PricePerPiece = fPricePerPiece,
                    TotalArea = iQuantity * (float)sheet.Area_brutto_Real,
                    TotalMass = iQuantity * fMassPerPiece,
                    TotalPrice = iQuantity * fPricePerPiece
                };
                quotation.Add(item);
            }
        }

        public static bool DisplayCladdingTable(CPFDViewModel vm)
        {
            //return vm._modelOptionsVM.EnableCladding && CModelHelper.ModelHasCladding(vm.Model);
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts();
        }
        public static bool DisplayFibreglassTable(CPFDViewModel vm)
        {
            //return vm._modelOptionsVM.EnableCladding && CModelHelper.ModelHasFibreglass(vm.Model);
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts() && vm._claddingOptionsVM.HasFibreglass();
        }
        public static bool DisplayDoorsAndWindowsTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts();
        }
        public static bool DisplayFlashingsTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts();
        }
        public static bool DisplayGuttersTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasRoof();
        }        
        public static bool DisplayDownpipesTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasRoof();
        }
        public static bool DisplayRoofNettingTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasRoof();
        }
        public static bool DisplayPackersTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts() && vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.ModelHasRollerDoor() && vm._doorsAndWindowsVM.AreBothRollerDoorHeaderFlashings();
        }

        public static bool DisplayCladdingAccesoriesTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts() && vm._doorsAndWindowsVM != null;
        }
    }
}