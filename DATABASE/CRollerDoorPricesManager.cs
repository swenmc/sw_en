using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATH;

namespace DATABASE
{
    public static class CRollerDoorPricesManager
    {
        public static float GetRollerDoorPrice(float doorWidth, float doorHeight, bool bIsAluZincColor)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            bool bIsDomesticModelSeries = true;

            float fAreaLimit;
            float fWidthLimit;
            float fHeightLimit;

            float[,] Table;

            if (bIsDomesticModelSeries)
            {
                Table = new float[4, 6];

                fAreaLimit = 10.25f;
                fWidthLimit = 3.21f;
                fHeightLimit = 3.21f;
            }
            else
            {
                Table = new float[12, 12];

                fAreaLimit = 31.01f;
                fWidthLimit = 6.01f;
                fHeightLimit = 6.01f;
            }

            if (doorWidth * doorHeight > fAreaLimit || doorWidth > fWidthLimit || doorHeight > fHeightLimit)
                throw new Exception(
                    "Door size is invalid." + "\n" +
                    "Width: " + doorWidth.ToString("F2") + " m" + "\n" +
                    "Height: " + doorHeight.ToString("F2") + " m");

            // Connect to database
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName;

                if(bIsDomesticModelSeries)
                {
                    if (bIsAluZincColor)
                        sTableName = "rollerDoorDomPrices_ALUZINC_NZD";
                    else
                        sTableName = "rollerDoorDomPrices_COLOR_NZD";
                }
                else
                {
                    if (bIsAluZincColor)
                        sTableName = "rollerShutterPrices_ALUZINC_NZD";
                    else
                        sTableName = "rollerShutterPrices_COLOR_NZD";
                }

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);

                using (reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        if (bIsDomesticModelSeries)
                        {
                            Table[i, 0] = float.Parse(reader["DoorWidth"].ToString(), nfi);
                            Table[i, 1] = float.Parse(reader["w24"].ToString(), nfi);
                            Table[i, 2] = float.Parse(reader["w26"].ToString(), nfi);
                            Table[i, 3] = float.Parse(reader["w28"].ToString(), nfi);
                            Table[i, 4] = float.Parse(reader["w30"].ToString(), nfi);
                            Table[i, 5] = float.Parse(reader["w32"].ToString(), nfi);
                        }
                        else
                        {
                            Table[i, 0] = float.Parse(reader["DoorWidth"].ToString(), nfi);
                            Table[i, 1] = float.Parse(reader["w30"].ToString(), nfi);
                            Table[i, 2] = float.Parse(reader["w33"].ToString(), nfi);
                            Table[i, 3] = float.Parse(reader["w36"].ToString(), nfi);
                            Table[i, 4] = float.Parse(reader["w39"].ToString(), nfi);
                            Table[i, 5] = float.Parse(reader["w42"].ToString(), nfi);
                            Table[i, 6] = float.Parse(reader["w45"].ToString(), nfi);
                            Table[i, 7] = float.Parse(reader["w48"].ToString(), nfi);
                            Table[i, 8] = float.Parse(reader["w51"].ToString(), nfi);
                            Table[i, 9] = float.Parse(reader["w54"].ToString(), nfi);
                            Table[i, 10] = float.Parse(reader["w57"].ToString(), nfi);
                            Table[i, 11] = float.Parse(reader["w60"].ToString(), nfi);
                        }
                        i++;
                    }
                }

                reader.Close();
            }

            // Bilinear Interpolation
            float[] arrayHeaderColumnValues;

            if (bIsDomesticModelSeries)
                arrayHeaderColumnValues = new float[] { 2.4f, 2.6f, 2.8f, 3.0f, 3.2f };
            else
                arrayHeaderColumnValues = new float[] { 3f, 3.3f, 3.6f, 3.9f, 4.2f, 4.5f, 4.8f, 5.1f, 5.4f, 5.7f, 6f };

            return MATH.ARRAY.ArrayF.GetBilinearInterpolationValuePositive(Table, arrayHeaderColumnValues, doorHeight, doorWidth);

            // TODO - zapracovat extra doplnky
            /*
            WINDLOCK GUIDES -Steel          $300   EXTRA
            REMOTES X2 / EYEBEAMS           $240   EXTRA
            BIRDPROOFING 75MM                $75   PER 1.5M
            CRATE UPTO 4.0                  $300   EXTRA
            CRATE OVER 4.0                  $450   EXTRA
            */
        }
    }
}