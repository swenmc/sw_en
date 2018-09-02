using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using CRSC;
using MATERIAL;

namespace DATABASE
{
    public static class CMaterialManager
    {
        public static void SetMaterialValuesFromDatabase(CMat[] materials)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();

                foreach (CMat_03_00 mat in materials)
                {
                    //??? predpokladam,ze na zaklade nejakej property z mat treba vybrat z DB a nie natvrdo 1
                    int ID = 1;
                    string stringcommand = "Select * from materialSteelAS4600 where ID = '" + ID + "'";

                    using (SQLiteCommand command = new SQLiteCommand(stringcommand, conn))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mat.Standard = reader["Standard"].ToString();
                                mat.Name = /*mat.Grade =*/ reader["Grade"].ToString();
                                int intervals = reader.GetInt32(reader.GetOrdinal("iNumberOfIntervals"));
                                mat.Note = reader["note"].ToString();

                                if (intervals == 1)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                }
                                else if (intervals == 2)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                    mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                    mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                    mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                }
                                else if (intervals == 3)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                    mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                    mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                    mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                    mat.m_ft_interval[2] = reader.GetFloat(reader.GetOrdinal("t3"));
                                    mat.m_ff_yk[2] = reader.GetFloat(reader.GetOrdinal("f_y3")) * 1e+6f;
                                    mat.m_ff_u[2] = reader.GetFloat(reader.GetOrdinal("f_u3")) * 1e+6f;
                                }
                                else if (intervals == 4)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                    mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                    mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                    mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                    mat.m_ft_interval[2] = reader.GetFloat(reader.GetOrdinal("t3"));
                                    mat.m_ff_yk[2] = reader.GetFloat(reader.GetOrdinal("f_y3")) * 1e+6f;
                                    mat.m_ff_u[2] = reader.GetFloat(reader.GetOrdinal("f_u3")) * 1e+6f;
                                    mat.m_ft_interval[3] = reader.GetFloat(reader.GetOrdinal("t4"));
                                    mat.m_ff_yk[3] = reader.GetFloat(reader.GetOrdinal("f_y4")) * 1e+6f;
                                    mat.m_ff_u[3] = reader.GetFloat(reader.GetOrdinal("f_u4")) * 1e+6f;
                                }
                            } //end while
                        } //end reader
                    }
                }
            }
        }
    }
}
