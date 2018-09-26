using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
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
        private float MAnnualProbabilityULS_Snow;
        private float MAnnualProbabilityULS_Wind;
        private float MAnnualProbabilityULS_EQ;
        private float MAnnualProbabilitySLS;
        private float MSiteElevation;
        private int MSnowRegionIndex;
        private int MExposureCategory;
        private int MWindRegionIndex;
        private int MTerrainCategoryIndex;
        private int MAngleWindDirectionIndex;
        private int MSiteSubSoilClassIndex;
        private float MFaultDistanceDmin;
        private float MFaultDistanceDmax;
        private float MZoneFactorZ;
        private float MPeriodAlongXDirectionTx;
        private float MPeriodAlongYDirectionTy;
        private float MSpectralShapeFactorChTx;
        private float MSpectralShapeFactorChTy;
        private float MAdditionalDeadActionRoof;
        private float MAdditionalDeadActionWall;
        private float MImposedActionRoof;

        // Not in GUI
        private float MDesignLife_Value;
        private float MR_ULS_Snow;
        private float MR_ULS_Wind;
        private float MR_ULS_EQ;
        private float MR_SLS;
        private ERoofExposureCategory MEExposureCategory;
        private EWindRegion MEWindRegion;
        private ESiteSubSoilClass MESiteSubSoilClass;

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

                SetDesignLifeValueFromDatabaseValues();
                SetAnnualProbabilityValuesFromDatabaseValues();

                NotifyPropertyChanged("DesignLifeIndex");
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
        public float SiteElevation
        {
            get
            {
                return MSiteElevation;
            }

            set
            {
                if (value < 0 || value > 2000)
                    throw new ArgumentException("Site elevation must be between 0 and 2000 meters");
                MSiteElevation = value;

                NotifyPropertyChanged("SiteElevation");
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
        public int ExposureCategoryIndex
        {
            get
            {
                return MExposureCategory;
            }

            set
            {
                MExposureCategory = value;

                MEExposureCategory = (ERoofExposureCategory)MExposureCategory;

                NotifyPropertyChanged("ExposureCategoryIndex");
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

                WindRegion = (EWindRegion)MWindRegionIndex;

                NotifyPropertyChanged("WindRegionIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int TerrainCategoryIndex
        {
            get
            {
                return MTerrainCategoryIndex;
            }

            set
            {
                MTerrainCategoryIndex = value;

                NotifyPropertyChanged("TerrainCategoryIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int AngleWindDirectionIndex
        {
            get
            {
                return MAngleWindDirectionIndex;
            }

            set
            {
                MAngleWindDirectionIndex = value;

                NotifyPropertyChanged("AngleWindDirectionIndex");
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

                SiteSubSoilClass = (ESiteSubSoilClass)MSiteSubSoilClassIndex;

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
        public float AdditionalDeadActionRoof
        {
            get
            {
                return MAdditionalDeadActionRoof;
            }

            set
            {
                if (value < 0.0f || value > 10.00f)
                    throw new ArgumentException("Additional dead action value must be between 0.0 and 10.0 kN/m²");
                MAdditionalDeadActionRoof = value;

                NotifyPropertyChanged("AdditionalDeadActionRoof");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AdditionalDeadActionWall
        {
            get
            {
                return MAdditionalDeadActionWall;
            }

            set
            {
                if (value < 0.0f || value > 10.00f)
                    throw new ArgumentException("Additional dead action value must be between 0.0 and 10.0 kN/m²");
                MAdditionalDeadActionWall = value;

                NotifyPropertyChanged("AdditionalDeadActionWall");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ImposedActionRoof
        {
            get
            {
                return MImposedActionRoof;
            }

            set
            {
                if (value < 0.0f || value > 5.00f)
                    throw new ArgumentException("Imposed action value must be between 0.0 and 5.0 kN/m²");
                MImposedActionRoof = value;

                NotifyPropertyChanged("ImposedActionRoof");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignLife_Value
        {
            get
            {
                return MDesignLife_Value;
            }

            set
            {
                MDesignLife_Value = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float R_ULS_Snow
        {
            get
            {
                return MR_ULS_Snow;
            }

            set
            {
                MR_ULS_Snow = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float R_ULS_Wind
        {
            get
            {
                return MR_ULS_Wind;
            }

            set
            {
                MR_ULS_Wind = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float R_ULS_EQ
        {
            get
            {
                return MR_ULS_EQ;
            }

            set
            {
                MR_ULS_EQ = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float R_SLS
        {
            get
            {
                return MR_SLS;
            }

            set
            {
                MR_SLS = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public EWindRegion WindRegion
        {
            get
            {
                return MEWindRegion;
            }

            set
            {
                MEWindRegion = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public ESiteSubSoilClass SiteSubSoilClass
        {
            get
            {
                return MESiteSubSoilClass;
            }

            set
            {
                MESiteSubSoilClass = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public ERoofExposureCategory ExposureCategory
        {
            get
            {
                return MEExposureCategory;
            }

            set
            {
                MEExposureCategory = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDLoadInput(loadInputComboboxIndexes sloadInputComboBoxes, loadInputTextBoxValues sloadInputTextBoxes)
        {
            // Set default location
            LocationIndex = sloadInputComboBoxes.LocationIndex;
            DesignLifeIndex = sloadInputComboBoxes.DesignLifeIndex;
            ImportanceClassIndex = sloadInputComboBoxes.ImportanceLevelIndex;
            SiteSubSoilClassIndex = sloadInputComboBoxes.SiteSubSoilClassIndex;
            TerrainCategoryIndex = sloadInputComboBoxes.TerrainCategoryIndex;
            AngleWindDirectionIndex = sloadInputComboBoxes.AngleWindDirectionIndex;

            SiteElevation = sloadInputTextBoxes.SiteElevation;
            FaultDistanceDmin = sloadInputTextBoxes.FaultDistanceDmin_km;
            FaultDistanceDmax = sloadInputTextBoxes.FaultDistanceDmax_km;
            PeriodAlongXDirectionTx = sloadInputTextBoxes.PeriodAlongXDirectionTx;
            PeriodAlongYDirectionTy = sloadInputTextBoxes.PeriodAlongYDirectionTy;
            AdditionalDeadActionRoof = sloadInputTextBoxes.AdditionalDeadActionRoof;
            AdditionalDeadActionWall = sloadInputTextBoxes.AdditionalDeadActionWall;
            ImposedActionRoof = sloadInputTextBoxes.ImposedActionRoof;

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
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

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
                        SpectralShapeFactorChTx = float.Parse(sSpectralShapeFactorChTx, nfi);
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
                        SpectralShapeFactorChTy = float.Parse(sSpectralShapeFactorChTy, nfi);
                    }
                }

                reader.Close();
            }
        }

        protected void SetDesignLifeValueFromDatabaseValues()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;
                string sTableName = "ASNZS1170_Tab3_3_DWL";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + DesignLifeIndex + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DesignLife_Value = float.Parse(reader["time_in_years"].ToString(), nfi);
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
                        // Co tak v databaze dat spravny typ ???
                        // TO Ondrej - potrebujem najst conventor ktory to urobi automaticky a nebude tam pridavat ako prvy stlpec svoje ID
                        // Teraz pouzivam convertor ktory vsetko nastavi ako default na string

                        SnowRegionIndex = int.Parse(reader["snow_zone"].ToString()); //reader.GetInt32(reader.GetOrdinal("snow_zone"));
                        WindRegionIndex = int.Parse(reader["wind_zone"].ToString()); //reader.GetInt32(reader.GetOrdinal("wind_zone"));
                        WindRegion = (EWindRegion)WindRegionIndex;

                        int iMultiplier_M_lee_ID; // Could be empty
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("windMultiplierM_lee")))
                            {
                                iMultiplier_M_lee_ID = int.Parse(reader["windMultiplierM_lee"].ToString());

                                // TODO nacitat data pre index fMultiplier_M_lee_ID z databazy - tabulka multiplierM_lee, vzdialenost (zone_min a zone_max)
                            }
                        }
                        catch (ArgumentNullException) { iMultiplier_M_lee_ID = -1; }

                        int iRainZone = int.Parse(reader["rain_zone"].ToString());
                        int iCorrosionZone = int.Parse(reader["corrosion_zone"].ToString());

                        // Site elevation
                        SiteElevation = float.Parse(reader["E_average_m"].ToString());

                        // Earthquake
                        ZoneFactorZ = float.Parse(reader["eqFactorZ"].ToString(), nfi);

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("D_min_km")))
                            {
                                FaultDistanceDmin = float.Parse(reader["D_min_km"].ToString());
                            }
                        }
                        catch (ArgumentNullException) { FaultDistanceDmin = 9999.00f; }

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("D_max_km")))
                            {
                                FaultDistanceDmax = float.Parse(reader["D_max_km"].ToString());
                            }
                        }
                        catch (ArgumentNullException) { FaultDistanceDmax = 9999.00f; }
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
                // TODO Mato - Ak je to OK, tak koment vymazat

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
                        string sAnnualProbabilitySLS1 = reader["SLS1"].ToString();
                        string sAnnualProbabilitySLS2 = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("SLS2")))
                            {
                                sAnnualProbabilitySLS2 = reader["SLS2"].ToString();
                            }
                        }
                        catch (ArgumentNullException) { }

                        AnnualProbabilityULS_Wind = (float)FractionConverter.Convert(sAnnualProbabilityULS_Wind);
                        AnnualProbabilityULS_Snow = (float)FractionConverter.Convert(sAnnualProbabilityULS_Snow);
                        AnnualProbabilityULS_EQ = (float)FractionConverter.Convert(sAnnualProbabilityULS_EQ);
                        AnnualProbabilitySLS = (float)FractionConverter.Convert(sAnnualProbabilitySLS1);

                        //TODO Martin - doriesit SLS1 a SLS2
                        //AnnualProbabilitySLS2 = (float)FractionConverter.Convert(sAnnualProbabilitySLS2);

                        R_ULS_Wind = float.Parse(reader["R_ULS_Wind_inyears"].ToString());
                        R_ULS_Snow = float.Parse(reader["R_ULS_Snow_inyears"].ToString());
                        R_ULS_EQ = float.Parse(reader["R_ULS_Earthquake_inyears"].ToString());

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_SLS1_inyears")))
                            {
                                R_SLS = float.Parse(reader["R_SLS1_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException) { }
                    }
                }

                reader.Close();
            }
        }
    }
}
