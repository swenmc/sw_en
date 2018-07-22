using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;

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
        private float MAnnualProbability_R_ULS_Wind;
        private float MAnnualProbability_R_ULS_Snow;
        private float MAnnualProbability_R_ULS_EQ;
        private float MAnnualProbability_R_SLS;
        private int MSnowRegionIndex;
        private int MWindRegionIndex;
        private int MTerrainRoughnessIndex;
        private int MSiteSubSoilClassIndex;
        private float MProximityToFault;
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

                // Connect to database
                using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
                {
                    conn.Open();
                    SQLiteDataReader reader = null;
                    string sTableName = "nzLocations";
                    int cityID = MLocationIndex;

                    SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + cityID + "'", conn);

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SnowRegionIndex = int.Parse(reader["snow_zone"].ToString());
                            WindRegionIndex = int.Parse(reader["wind_zone"].ToString());

                            // TODO - Ondrej osetrit pripady ked nie je v databaze vyplnena hodnota

                            int fMultiplier_M_lee_ID; // Could be empty
                            try
                            {
                                //fMultiplier_M_lee_ID = int.Parse(reader["windMultiplierM_lee"].ToString());
                            }
                            catch (ArgumentNullException) { }

                            int iRainZone = int.Parse(reader["rain_zone"].ToString());
                            int iCorrosionZone = int.Parse(reader["corrosion_zone"].ToString());

                            // Earthquake
                            ZoneFactorZ = float.Parse(reader["eqFactorZ"].ToString());

                            float fD_min_km;  // Could be empty
                            float fD_max_km;  // Could be empty

                            try
                            {
                                //fD_min_km = float.Parse(reader["D_min_km"].ToString());
                                //fD_max_km = float.Parse(reader["D_max_km"].ToString());
                            }
                            catch (ArgumentNullException) { }
                        }
                    }

                    sTableName = "SiteSubSoilClass";
                    string sSiteSubSoilClass ="";

                    command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + MSiteSubSoilClassIndex + "'", conn);

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sSiteSubSoilClass = reader["class"].ToString();
                        }
                    }

                    sTableName = "ASNZS1170_5_Tab3_1_SSF";
                    string sPeriodT = MPeriodAlongXDirectionTx.ToString();
                    string sSpectralShapeFactorChTx = "";

                    command = new SQLiteCommand("Select * from " + sTableName + " where periodT = '" + sPeriodT + "'", conn);

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sSpectralShapeFactorChTx = reader[sSiteSubSoilClass].ToString();
                            MSpectralShapeFactorChTx = float.Parse(sSpectralShapeFactorChTx);
                        }
                    }

                    sPeriodT = MPeriodAlongYDirectionTy.ToString();
                    string sSpectralShapeFactorChTy = "";

                    command = new SQLiteCommand("Select * from " + sTableName + " where periodT = '" + sPeriodT + "'", conn);

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sSpectralShapeFactorChTy = reader[sSiteSubSoilClass].ToString();
                            MSpectralShapeFactorChTy = float.Parse(sSpectralShapeFactorChTy);
                        }
                    }

                    reader.Close();
                }

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

                    SQLiteCommand command = new SQLiteCommand(
                        "Select * from " +
                        " ( " +
                        "Select * from " + sTableName + " where designWorkingLife_ID = '" + MDesignLifeIndex +
                        "')," +
                        " ( " +
                        "Select * from " + sTableName + " where importanceLevel_ID = '" + MImportanceClassIndex +
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

                NotifyPropertyChanged("ImportanceClassIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS_Wind
        {
            get
            {
                return MAnnualProbability_R_ULS_Wind;
            }

            set
            {
                MAnnualProbability_R_ULS_Wind = value;

                NotifyPropertyChanged("AnnualProbabilityULS_Wind");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS_Snow
        {
            get
            {
                return MAnnualProbability_R_ULS_Snow;
            }

            set
            {
               MAnnualProbability_R_ULS_Snow = value;

               NotifyPropertyChanged("AnnualProbabilityULS_Snow");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS_EQ
        {
            get
            {
                return MAnnualProbability_R_ULS_EQ;
            }

            set
            {
                MAnnualProbability_R_ULS_EQ = value;

                NotifyPropertyChanged("AnnualProbabilityULS_EQ");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilitySLS
        {
            get
            {
                return MAnnualProbability_R_SLS;
            }

            set
            {
                MAnnualProbability_R_SLS = value;

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

                NotifyPropertyChanged("SiteSubSoilClassIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ProximityToFault
        {
            get
            {
                return MProximityToFault;
            }

            set
            {
                if (value < 1000 || value > 50000)
                    throw new ArgumentException("Proximity to fault must be between 1000 and 50000 meters");
                MProximityToFault = value;

                NotifyPropertyChanged("ProximityToFault");
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
                    throw new ArgumentException("Zone factor must be between 0.01 and 0.90 meters");
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
                return MSpectralShapeFactorChTx;
            }

            set
            {
                if (value < 0.15f || value > 3.00f)
                    throw new ArgumentException("Spectral shape factor Ch T must be between 0.15 and 3.0");
                MSpectralShapeFactorChTx = value;

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
            ProximityToFault = sloadInput.ProximityToFault;
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
    }
}
