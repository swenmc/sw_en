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
        private float MDesignLife;
        private int MImportanceClass; // Clause A3—Building importance levels
        private float MAnnualProbability_R_ULS;
        private float MAnnualProbability_R_SLS;
        private int MSnowRegionIndex;
        private int MWindRegionIndex;
        private int MTerrainRoughnessIndex;
        private int MSiteSubSoilClassIndex;
        private float MProximityToFault;
        private float MZoneFactorZ;
        private float MPeriodAlongXDirectionTx;
        private float MPeriodAlongYDirectionTy;
        private float MSpectralShapeFactorChT;

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

                    reader.Close();
                }

                DesignLife = 20f;
                ImportanceClassIndex = 0;
                AnnualProbabilityULS = 1f / 250f;
                AnnualProbabilitySLS = 1f / 500f;
                TerrainRoughnessIndex = 0;
                SiteSubSoilClassIndex = 0;
                ProximityToFault = 4000f;
                PeriodAlongXDirectionTx = 0.4f;
                PeriodAlongYDirectionTy = 0.4f;
                SpectralShapeFactorChT = 1f;

                IsSetFromCode = false;

                NotifyPropertyChanged("LocationIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignLife
        {
            get
            {
                return MDesignLife;
            }
            set
            {
                if (value < 1 || value > 100)
                    throw new ArgumentException("Design Life must be between 1 and 100 years");
                MDesignLife = value;

                NotifyPropertyChanged("DesignLife");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int ImportanceClassIndex
        {
            get
            {
                return MImportanceClass;
            }

            set
            {
                if (value + 1 < 1 || value + 1 > 5) // Add one because it is indexed from 0
                    throw new ArgumentException("Importance level must be between 1 and 5");
                MImportanceClass = value;

                NotifyPropertyChanged("ImportanceClassIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AnnualProbabilityULS
        {
            get
            {
                return MAnnualProbability_R_ULS;
            }

            set
            {
               MAnnualProbability_R_ULS = value;

               NotifyPropertyChanged("AnnualProbabilityULS");
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
        public float SpectralShapeFactorChT
        {
            get
            {
                return MSpectralShapeFactorChT;
            }

            set
            {
                if (value < 0.50f || value > 5.00f)
                    throw new ArgumentException("Spectral shape factor Ch T must be between 0.5 and 5.0");
                MSpectralShapeFactorChT = value;

                NotifyPropertyChanged("SpectralShapeFactorChT");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDLoadInput(int locationCombobox)
        {
            // Set default location
            LocationIndex = locationCombobox;

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
