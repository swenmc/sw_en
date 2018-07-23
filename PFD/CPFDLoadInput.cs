using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;
using System.Globalization;

namespace PFD
{
    public class CPFDLoadInput : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        public SQLiteConnection conn;
        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MLocationIndex;
        private int MDesignLifeIndex;
        private int MImportanceClassIndex; // Clause A3—Building importance levels
        private float MAnnualProbabilityULS_Wind;
        private float MAnnualProbabilityULS_Snow;
        private float MAnnualProbabilityULS_EQ;
        private float MAnnualProbabilitySLS;
        private int MSnowRegionIndex;
        private int MWindRegionIndex;
        private int MTerrainRoughnessIndex;
        private int MSiteSubSoilClassIndex;
        private float MFaultDistanceDmin;
        private float MFaultDistanceDmax;
        private float MZoneFactorZ;
        private float MPeriodAlongXDirectionTx;
        private float MPeriodAlongYDirectionTy;
        private float MSpectralShapeFactorChTx;
        private float MSpectralShapeFactorChTy;

        //-------------------------------------------------------------------------------------------------------------
        public int LocationIndex
        {
            get
            {
                return MLocationIndex;
            }

            set
            {
                MLocationIndex = value;

                IsSetFromCode = true;

                SetLocationDependentDataFromDatabaseValues();

                IsSetFromCode = false;

                NotifyPropertyChanged("LocationIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int DesignLifeIndex
        {
            get
            {
                return MDesignLifeIndex;
            }
            set
            {
                MDesignLifeIndex = value;

                SetAnnualProbabilityValuesFromDatabaseValues();

                NotifyPropertyChanged("DesignLife");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int ImportanceClassIndex
        {
            get
            {
                return MImportanceClassIndex;
            }

            set
            {
                MImportanceClassIndex = value;

                SetAnnualProbabilityValuesFromDatabaseValues();

                NotifyPropertyChanged("ImportanceClassIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS_Wind
        {
            get
            {
                return MAnnualProbabilityULS_Wind;
            }

            set
            {
                MAnnualProbabilityULS_Wind = value;

                NotifyPropertyChanged("AnnualProbabilityULS_Wind");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS_Snow
        {
            get
            {
                return MAnnualProbabilityULS_Snow;
            }

            set
            {
               MAnnualProbabilityULS_Snow = value;

               NotifyPropertyChanged("AnnualProbabilityULS_Snow");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS_EQ
        {
            get
            {
                return MAnnualProbabilityULS_EQ;
            }

            set
            {
                MAnnualProbabilityULS_EQ = value;

                NotifyPropertyChanged("AnnualProbabilityULS_EQ");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilitySLS
        {
            get
            {
                return MAnnualProbabilitySLS;
            }

            set
            {
                MAnnualProbabilitySLS = value;

                NotifyPropertyChanged("AnnualProbabilitySLS");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int SnowRegionIndex
        {
            get
            {
                return MSnowRegionIndex;
            }

            set
            {
                MSnowRegionIndex = value;

                NotifyPropertyChanged("SnowRegionIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WindRegionIndex
        {
            get
            {
                return MWindRegionIndex;
            }

            set
            {
                MWindRegionIndex = value;

                NotifyPropertyChanged("WindRegionIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int TerrainRoughnessIndex
        {
            get
            {
                return MTerrainRoughnessIndex;
            }

            set
            {
                MTerrainRoughnessIndex = value;

                NotifyPropertyChanged("TerrainRoughnessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int SiteSubSoilClassIndex
        {
            get
            {
                return MSiteSubSoilClassIndex;
            }

            set
            {
                if (value < 0 || value > 4)
                    throw new ArgumentException("Site subsoil class must be between A and E");
                MSiteSubSoilClassIndex = value;

                SetSpectralShapeFactorsFromDatabaseValues();

                NotifyPropertyChanged("SiteSubSoilClassIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FaultDistanceDmin
        {
            get
            {
                return MFaultDistanceDmin;
            }

            set
            {
                MFaultDistanceDmin = value;

                NotifyPropertyChanged("FaultDistanceDmin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FaultDistanceDmax
        {
            get
            {
                return MFaultDistanceDmax;
            }

            set
            {
                MFaultDistanceDmax = value;

                NotifyPropertyChanged("FaultDistanceDmax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ZoneFactorZ
        {
            get
            {
                return MZoneFactorZ;
            }

            set
            {
                if (value < 0.01f || value > 0.90f)
                    throw new ArgumentException("Zone factor must be between 0.01 and 0.90");
                MZoneFactorZ = value;

                NotifyPropertyChanged("ZoneFactorZ");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PeriodAlongXDirectionTx
        {
            get
            {
                return MPeriodAlongXDirectionTx;
            }

            set
            {
                if (value < 0.00f || value > 4.50f)
                    throw new ArgumentException("Period along X-direction Tx must be between 0.00 and 4.50 seconds");
                MPeriodAlongXDirectionTx = value;

                SetSpectralShapeFactorsFromDatabaseValues();

                NotifyPropertyChanged("PeriodAlongXDirectionTx");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PeriodAlongYDirectionTy
        {
            get
            {
                return MPeriodAlongYDirectionTy;
            }

            set
            {
                if (value < 0.00f || value > 4.50f)
                    throw new ArgumentException("Period along Y-direction Ty must be between 0.00 and 4.50 seconds");
                MPeriodAlongYDirectionTy = value;

                SetSpectralShapeFactorsFromDatabaseValues();

                NotifyPropertyChanged("PeriodAlongYDirectionTy");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SpectralShapeFactorChTx
        {
            get
            {
                return MSpectralShapeFactorChTx;
            }

            set
            {
                if (value < 0.15f || value > 3.00f)
                    throw new ArgumentException("Spectral shape factor Ch T must be between 0.15 and 3.0");
                MSpectralShapeFactorChTx = value;

                NotifyPropertyChanged("SpectralShapeFactorChTx");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SpectralShapeFactorChTy
        {
            get
            {
                return MSpectralShapeFactorChTy;
            }

            set
            {
                if (value < 0.15f || value > 3.00f)
                    throw new ArgumentException("Spectral shape factor Ch T must be between 0.15 and 3.0");
                MSpectralShapeFactorChTy = value;

                NotifyPropertyChanged("SpectralShapeFactorChTy");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDLoadInput(loadInputComboboxIndexes sloadInput)
        {
            // Set default location
            LocationIndex = sloadInput.LocationIndex;
            DesignLifeIndex = sloadInput.DesignLifeIndex;
            SiteSubSoilClassIndex = sloadInput.SiteSubSoilClassIndex;
            ImportanceClassIndex = sloadInput.ImportanceLevelIndex;
            TerrainRoughnessIndex = sloadInput.TerrainRoughnessIndex;
            FaultDistanceDmin = sloadInput.FaultDistanceDmin;
            FaultDistanceDmax = sloadInput.FaultDistanceDmax;
            PeriodAlongXDirectionTx = sloadInput.PeriodAlongXDirectionTx;
            PeriodAlongYDirectionTy = sloadInput.PeriodAlongYDirectionTy;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetSpectralShapeFactorsFromDatabaseValues()
        {
            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName = "SiteSubSoilClass";
                string sSiteSubSoilClass = "";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + SiteSubSoilClassIndex + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sSiteSubSoilClass = reader["class"].ToString();
                    }
                }

                sTableName = "ASNZS1170_5_Tab3_1_SSF";
                string sPeriodT = PeriodAlongXDirectionTx.ToString();
                string sSpectralShapeFactorChTx = "";

                command = new SQLiteCommand("Select * from " + sTableName + " where periodT = '" + sPeriodT + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sSpectralShapeFactorChTx = reader[sSiteSubSoilClass].ToString();
                        SpectralShapeFactorChTx = float.Parse(sSpectralShapeFactorChTx);
                    }
                }

                sPeriodT = PeriodAlongYDirectionTy.ToString();
                string sSpectralShapeFactorChTy = "";

                command = new SQLiteCommand("Select * from " + sTableName + " where periodT = '" + sPeriodT + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sSpectralShapeFactorChTy = reader[sSiteSubSoilClass].ToString();
                        SpectralShapeFactorChTy = float.Parse(sSpectralShapeFactorChTy);
                    }
                }

                reader.Close();
            }
        }

        protected void SetLocationDependentDataFromDatabaseValues()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;
                string sTableName = "nzLocations";
                int cityID = LocationIndex;

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + cityID + "'", conn);
                
                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Co tak v databaze dat spravny typ ???
                        SnowRegionIndex = int.Parse(reader["snow_zone"].ToString()); //reader.GetInt32(reader.GetOrdinal("snow_zone"));
                        WindRegionIndex = int.Parse(reader["wind_zone"].ToString()); //reader.GetInt32(reader.GetOrdinal("wind_zone"));

                        // TODO - Ondrej osetrit pripady ked nie je v databaze vyplnena hodnota
                        //23.7.2018 O.P.

                        int fMultiplier_M_lee_ID = 0; // Could be empty
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("windMultiplierM_lee")))
                            {
                                fMultiplier_M_lee_ID = reader.GetInt32(reader.GetOrdinal("windMultiplierM_lee"));
                            } 
                            
                        }
                        catch (ArgumentNullException) { }

                        int iRainZone = int.Parse(reader["rain_zone"].ToString()); //reader.GetInt32(reader.GetOrdinal("rain_zone"));
                        int iCorrosionZone = int.Parse(reader["corrosion_zone"].ToString()); //reader.GetInt32(reader.GetOrdinal("corrosion_zone"));

                        // Earthquake
                        ZoneFactorZ = float.Parse(reader["rain_zone"].ToString(), nfi); //reader.GetFloat(reader.GetOrdinal("eqFactorZ"));

                        try
                        {
                            //FaultDistanceDmin = float.Parse(reader["D_min_km"].ToString());
                            //FaultDistanceDmax = float.Parse(reader["D_max_km"].ToString());
                        }
                        catch (ArgumentNullException) { }
                    }
                }

                reader.Close();
            }

            SetSpectralShapeFactorsFromDatabaseValues();
        }

        protected void SetAnnualProbabilityValuesFromDatabaseValues()
        {
            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                /*
                AnnualProbabilityULS_Wind = 1f / 250f;
                AnnualProbabilityULS_Snow = 1f / 250f;
                AnnualProbabilityULS_EQ = 1f / 250f;
                AnnualProbabilitySLS = 1f / 25f;
                */

                conn.Open();
                SQLiteDataReader reader = null;
                string sTableName = "ASNZS1170_Tab3_3_APOE";

                // TODO - Ondrej - SQL subquery in database
                // vybrat vsetky riadky s designWorkingLife_ID a uz vybranych riadkov vybrat vsetky riadky s 
                // s uvedenym importanceLevel_ID
                // vysledkom dotazu ma byt jeden riadok, pricom hodnoty apoeULS_xxx a SLS1 sa zapisu do premennych

                SQLiteCommand command = new SQLiteCommand(
                    "Select * from " +
                    " ( " +
                    "Select * from " + sTableName + " where designWorkingLife_ID = '" + DesignLifeIndex +
                    "')," +
                    " ( " +
                    "Select * from " + sTableName + " where importanceLevel_ID = '" + ImportanceClassIndex +
                    "')"
                    , conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //SnowRegionIndex = int.Parse(reader["snow_zone"].ToString());
                        //WindRegionIndex = int.Parse(reader["wind_zone"].ToString());

                        // TODO - Ondrej osetrit pripady ked nie je v databaze vyplnena hodnota
                        // Osetrit ako nacitat zlomok zadany ako string v databaze do float
                        // Bolo by super ak by sa zlomok v textboxe zobrazoval ako zlomok a potom sa previedol na float az vo vypocte
                        // uzivatel by mohol zadavat do textboxu zlomok x / y alebo priamo float

                        // Pridana trieda "FractionConverter.cs" - presunut ???

                        /*
                        AnnualProbabilityULS_Wind = float.Parse(reader["apoeULS_Wind"].ToString());
                        AnnualProbabilityULS_Snow = float.Parse(reader["apoeULS_Snow"].ToString());
                        AnnualProbabilityULS_EQ = float.Parse(reader["apoeULS_Earthquake"].ToString());
                        AnnualProbabilitySLS = float.Parse(reader["SLS1"].ToString());
                        float AnnualProbabilitySLS_2 = float.Parse(reader["SLS2"].ToString());
                        */
                        string sAnnualProbabilityULS_Wind = reader["apoeULS_Wind"].ToString();
                        string sAnnualProbabilityULS_Snow = reader["apoeULS_Snow"].ToString();
                        string sAnnualProbabilityULS_EQ = reader["apoeULS_Earthquake"].ToString();
                        string sAnnualProbabilitySLS = reader["SLS1"].ToString();

                        AnnualProbabilityULS_Wind = (float)FractionConverter.Convert(sAnnualProbabilityULS_Wind);
                        AnnualProbabilityULS_Snow = (float)FractionConverter.Convert(sAnnualProbabilityULS_Snow);
                        AnnualProbabilityULS_EQ = (float)FractionConverter.Convert(sAnnualProbabilityULS_EQ);
                        AnnualProbabilitySLS = (float)FractionConverter.Convert(sAnnualProbabilitySLS);
                    }
                }

                reader.Close();
            }
        }
    }
}
