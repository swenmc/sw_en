using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CStringsManager
    {
        public static List<DataExportTables> LoadBasicGeometryParameters()
        {
            return LoadStringsTable("BasicGeometry");
        }

        public static List<DataExportTables> LoadBasicLoadParameters()
        {
            return LoadStringsTable("BasicLoadParameters");
        }

        public static List<DataExportTables> LoadDeadLoadParameters_AS1170_1()
        {
            return LoadStringsTable("AS1170_1_DL");
        }

        public static List<DataExportTables> LoadServiceLoadParameters_AS1170_1()
        {
            return LoadStringsTable("AS1170_1_SL");
        }

        public static List<DataExportTables> LoadImposedLoadParameters_AS1170_1()
        {
            return LoadStringsTable("AS1170_1_IL");
        }

        public static List<DataExportTables> LoadWindLoadParameters_AS1170_2()
        {
            return LoadStringsTable("AS1170_2");
        }

        public static List<DataExportTables> LoadSnowLoadParameters_AS1170_3()
        {
            return LoadStringsTable("AS1170_3");
        }

        public static List<DataExportTables> LoadSeismicLoadParameters_NZS1170_5()
        {
            return LoadStringsTable("NZS1170_5");
        }
        public static List<DataExportTables> LoadAll()
        {
            List<DataExportTables> allItems = new List<DataExportTables>();

            allItems.AddRange(LoadStringsTable("BasicGeometry"));
            allItems.AddRange(LoadStringsTable("BasicLoadParameters"));        
            allItems.AddRange(LoadStringsTable("AS1170_1_DL"));
            allItems.AddRange(LoadStringsTable("AS1170_1_SL"));
            allItems.AddRange(LoadStringsTable("AS1170_1_IL"));
            allItems.AddRange(LoadStringsTable("AS1170_2"));
            allItems.AddRange(LoadStringsTable("AS1170_3"));
            allItems.AddRange(LoadStringsTable("NZS1170_5"));

            return allItems;
        }
        public static Dictionary<string, DataExportTables> GetAllDict()
        {
            List<DataExportTables> allItems = LoadAll();
            Dictionary<string, DataExportTables> dict = new Dictionary<string, DataExportTables>();
            foreach (DataExportTables d in allItems)
            {
                dict.Add(d.Identificator, d);
            }
            return dict;
        }


        private static List<DataExportTables> LoadStringsTable(string tableName)
        {
            List<DataExportTables> items = new List<DataExportTables>();
            DataExportTables data = null;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["StringsSQLiteDB"].ConnectionString))
            {
                conn.Open();

                /*
                // TO Ondrej - pokus zmazat riadky s null ID
                if (true)
                {
                    string queryString = $"Delete from " + tableName + " where ID = NULL";
                    SQLiteCommand command_delete = new SQLiteCommand(queryString, conn);
                    string queryString2 = $"Delete from " + tableName + " where ID = ''";
                    SQLiteCommand command_delete2 = new SQLiteCommand(queryString2, conn);
                    string queryString3 = $"Delete from " + tableName + " where ID IS NULL";
                    SQLiteCommand command_delete3 = new SQLiteCommand(queryString3, conn);
                }*/


                SQLiteCommand command = new SQLiteCommand($"Select * from {tableName}", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data = GetDataForExport(reader);
                        if (data != null) items.Add(data);
                    }
                }
            }
            return items;
        }

        private static DataExportTables GetDataForExport(SQLiteDataReader reader)
        {
            // To Mato: v databaze ak je ID,tak tam nemaju co hladat NULL hodnoty
            // To Ondrej: Mam taky mensi problem s convertorom http://converttosqlite.com/convert
            // Ak je v xls nejako dotknuta bunka na prazdnom riadku pod tabulkou, tak to vsetky tie prazdne riadky konvertuje do sql
            // Blbe je ze v tom editore SQL co mam to neviem zmazat hromadne, ale len po jednotlivych riadkoch
            // Chcel som to zmazat cez SQL prikaz vid vyssie ale nezadarilo sa.
            // Mozes mi urobit taku utilitku ze ked zmenim databazove subory, tak pri spusteni programu mi to automaticky odmaze zo vsetkych databaz a vsetkych tabuliek

            if (reader.IsDBNull(reader.GetOrdinal("ID"))) return null;

            DataExportTables data = new DataExportTables();
            data.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            data.Description_ENU_USA = reader["Description_ENU_USA"].ToString();
            data.Description_CSY = reader["Description_CSY"].ToString();
            data.Description_SVK = reader["Description_SVK"].ToString();
            data.Symbol = reader["Symbol"].ToString();
            data.Identificator = reader["Identificator"].ToString();

            // To Ondrej
            // V databaze je novy stlpec UnitIdentificator
            // Jeho hodnota zodpoveda stlpcu UnitIdentificator v novej tabulke QuantityLibrary
            // V tejto tabulke su jednotky, v ktorych sa pocita v programe, jednotky ktore sa maju zobrazovat v GUI a v reporte, faktory na prepocet jednotiek a pocty desatinnych miest
            // Mrkni na to ci by to takto mohlo fungovat
            // Do buducna je este otazka ako by sme to urobili pre palce, stopy, libry, unce a dalsie imperialne jednotky, ktore su popularne v GB a v USA

            data.Unit = reader["Unit"].ToString();
            data.UnitIdentificator = reader["UnitIdentificator"].ToString();            
            
            return data;
        }
        
    }
}
