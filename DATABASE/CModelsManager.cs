using DATABASE.DTO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;

namespace DATABASE
{
    public static class CModelsManager
    {
        public static List<CKitsetGableRoofEnclosed> LoadModelKitsetGableRoofEnclosed()
        {
            CKitsetGableRoofEnclosed model;
            List<CKitsetGableRoofEnclosed> items = new List<CKitsetGableRoofEnclosed>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["ModelsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from KitsetGableRoofEnclosed", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        model = new CKitsetGableRoofEnclosed();
                        model.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        model.ModelName = reader["modelName"].ToString();
                        model.Width = reader["width"].ToString();
                        model.Length = reader["length"].ToString();
                        model.Wall_height = reader["wall_height"].ToString();
                        model.Distance_L1 = reader["distance_L1"].ToString();
                        model.IFrames = reader["iFrames"].ToString();
                        model.C_array_code = reader["c_array_code"].ToString();
                        model.MainColumn = reader["mainColumn"].ToString();
                        model.MainRafter = reader["mainRafter"].ToString();
                        model.EdgeColumn = reader["edgeColumn"].ToString();
                        model.EdgeRafter = reader["edgeRafter"].ToString();
                        model.EdgePurlin = reader["edgePurlin"].ToString();
                        model.Girt = reader["girt"].ToString();
                        model.Purlin = reader["purlin"].ToString();
                        model.ColumnFrontSide = reader["columnFrontSide"].ToString();
                        model.ColumnBackSide = reader["columnBackSide"].ToString();
                        model.GirtFrontSide = reader["girtFrontSide"].ToString();
                        model.GirtBackSide = reader["girtBackSide"].ToString();
                        model.DoorFrame = reader["doorFrame"].ToString();
                        model.DoorTrimmer = reader["doorTrimmer"].ToString();
                        model.DoorLintel = reader["doorLintel"].ToString();
                        model.WindowFrame = reader["windowFrame"].ToString();

                        model.RafterFlyBracingEveryXXPurlin = reader["iRafterFlyBracingEveryXXPurlin"].ToString();
                        model.EdgePurlin_ILS_Number = reader["edgePurlin_ILS_Number"].ToString();
                        model.Girt_ILS_Number = reader["girt_ILS_Number"].ToString();
                        model.Purlin_ILS_Number = reader["purlin_ILS_Number"].ToString();
                        model.GirtFrontSide_ILS_Number = reader["girtFrontSide_ILS_Number"].ToString();
                        model.GirtBackSide_ILS_Number = reader["girtBackSide_ILS_Number"].ToString();

                        items.Add(model);
                    }
                }
            }
            return items;
        }
        public static CKitsetGableRoofEnclosed LoadModelKitsetGableRoofEnclosed(int ID)
        {
            CKitsetGableRoofEnclosed model = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["ModelsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from KitsetGableRoofEnclosed WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new CKitsetGableRoofEnclosed();
                        model.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        model.ModelName = reader["modelName"].ToString();
                        model.Width = reader["width"].ToString();
                        model.Length = reader["length"].ToString();
                        model.Wall_height = reader["wall_height"].ToString();
                        model.Distance_L1 = reader["distance_L1"].ToString();
                        model.IFrames = reader["iFrames"].ToString();
                        model.C_array_code = reader["c_array_code"].ToString();
                        model.MainColumn = reader["mainColumn"].ToString();
                        model.MainRafter = reader["mainRafter"].ToString();
                        model.EdgeColumn = reader["edgeColumn"].ToString();
                        model.EdgeRafter = reader["edgeRafter"].ToString();
                        model.EdgePurlin = reader["edgePurlin"].ToString();
                        model.Girt = reader["girt"].ToString();
                        model.Purlin = reader["purlin"].ToString();
                        model.ColumnFrontSide = reader["columnFrontSide"].ToString();
                        model.ColumnBackSide = reader["columnBackSide"].ToString();
                        model.GirtFrontSide = reader["girtFrontSide"].ToString();
                        model.GirtBackSide = reader["girtBackSide"].ToString();
                        model.DoorFrame = reader["doorFrame"].ToString();
                        model.DoorTrimmer = reader["doorTrimmer"].ToString();
                        model.DoorLintel = reader["doorLintel"].ToString();
                        model.WindowFrame = reader["windowFrame"].ToString();

                        model.BracingBlockGirts = reader["bracingBlockGirts"].ToString();
                        model.BracingBlockPurlins = reader["bracingBlockPurlins"].ToString();
                        model.BracingBlocksGirtsFrontSide = reader["bracingBlocksGirtsFrontSide"].ToString();
                        model.BracingBlocksGirtsBackSide = reader["bracingBlocksGirtsBackSide"].ToString();

                        model.RafterFlyBracingEveryXXPurlin = reader["iRafterFlyBracingEveryXXPurlin"].ToString();
                        model.EdgePurlin_ILS_Number = reader["edgePurlin_ILS_Number"].ToString();
                        model.Girt_ILS_Number = reader["girt_ILS_Number"].ToString();
                        model.Purlin_ILS_Number = reader["purlin_ILS_Number"].ToString();
                        model.GirtFrontSide_ILS_Number = reader["girtFrontSide_ILS_Number"].ToString();
                        model.GirtBackSide_ILS_Number = reader["girtBackSide_ILS_Number"].ToString();
                    }
                }
            }
            return model;
        }

        private static Dictionary<int, CComponentPrefixes> DictComponentPrefixes;
        private static void LoadModelComponents()
        {
            DictComponentPrefixes = new Dictionary<int, CComponentPrefixes>();
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["ModelsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from componentPrefixes", conn);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CComponentPrefixes component = new CComponentPrefixes();
                        component.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        component.ComponentPrefix = reader["componentPrefix"].ToString();
                        component.ComponentName = reader["componentName"].ToString();
                        component.ComponentColorCodeRGB = reader["defaultColorRGB"].ToString();
                        component.ComponentColorCodeHEX = reader["defaultColorHEX"].ToString();
                        component.ComponentColorName = reader["defaultColorName"].ToString();
                        DictComponentPrefixes.Add(component.ID, component);
                    }
                }
            }
        }

        public static CComponentPrefixes GetModelComponent(int ID)
        {
            CComponentPrefixes component = null;
            if (DictComponentPrefixes == null) LoadModelComponents();
            DictComponentPrefixes.TryGetValue(ID, out component);
            return component;
        }
    }
}