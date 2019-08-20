using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CMeshesManager
    {
        public static Dictionary<string, CMeshProperties> DictMeshProperties;

        public static List<CMeshProperties> LoadMeshesProperties_List()
        {
            CMeshProperties properties = null;
            List<CMeshProperties> items = new List<CMeshProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MeshesRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from reinforcing_meshes", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetMeshProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static Dictionary<string, CMeshProperties> LoadMeshesProperties_Dictionary()
        {
            CMeshProperties prop = null;
            Dictionary<string, CMeshProperties> items = new Dictionary<string, CMeshProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MeshesRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from reinforcing_meshes", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prop = GetMeshProperties(reader);
                        items.Add(prop.Name, prop);
                    }
                }
            }
            return items;
        }

        private static void LoadMeshesPropertiesDictionary()
        {
            DictMeshProperties = new Dictionary<string, CMeshProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MeshesRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from reinforcing_meshes", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CMeshProperties properties = GetMeshProperties(reader);
                        DictMeshProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CMeshProperties GetMeshProperties(string name)
        {
            if (DictMeshProperties == null) LoadMeshesPropertiesDictionary();

            CMeshProperties prop = null;
            DictMeshProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CMeshProperties GetMeshProperties(int id)
        {
            CMeshProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MeshesRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from reinforcing_meshes WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMeshProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CMeshProperties GetMeshProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMeshProperties properties = new CMeshProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["name"].ToString();
            properties.MaterialName = reader["material_name"].ToString();
            properties.WireDiameter = reader["wire_diameter_mm"].ToString() == "" ? double.NaN : double.Parse(reader["wire_diameter_mm"].ToString(), nfi) / 1000f;
            properties.CentersDistance = reader["centers_mm"].ToString() == "" ? double.NaN : double.Parse(reader["centers_mm"].ToString(), nfi) / 1000f;

            return properties;
        }
    }
}