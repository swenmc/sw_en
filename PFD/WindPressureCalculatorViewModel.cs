using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using System.Windows.Media;
using System.Collections.ObjectModel;
using MATH;
using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;
using System.Windows;

namespace PFD
{
    public class WindPressureCalculatorViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        public SQLiteConnection conn;
        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MDesignLifeIndex;
        private int MImportanceClassIndex; // Clause A3—Building importance levels
        private float MAnnualProbabilityULS_Wind;
        private float MAnnualProbabilitySLS;
        private float MSiteElevation;
        private int MWindRegionIndex;
        private int MTerrainCategoryIndex;
        private int MAngleWindDirectionIndex;

        private float MLeeMultiplier_Mlee;

        private float MTopographicMultiplier_Mt;
        private float MHillShapeMultiplier_Mh;
        private float MShieldingMultiplier_Ms;
        private float MWindDirectionMultiplier_Md;
        private float MTerrainHeightMultiplier_Mzcat;
        private float MTributaryArea_A;
        private float MAreaReductionFactor_Ka;
        private float MLocalPressureFactor_Kl;
        private float MPorousCladdingReductionFactor_Kp;
        private float MCombinationFactorExternalPressures_Kce;
        private float MCombinationFactorExternalPressures_Kci;

        private float MInternalPressureCoefficient_Cpimin;
        private float MInternalPressureCoefficient_Cpimax;
        private float MExternalPressureCoefficient_Cpemin;
        private float MExternalPressureCoefficient_Cpemax;
        private float MAerodynamicShapeFactor_Cfigimin;
        private float MAerodynamicShapeFactor_Cfigimax;
        private float MAerodynamicShapeFactor_Cfigemin;
        private float MAerodynamicShapeFactor_Cfigemax;

        private float MGableWidth;
        private float MLength;
        private float MWallHeight;
        private float MRoofPitch_deg;

        private float MAverageStructureHeight_h;

        // Not in GUI
        private float MDesignLife_Value;

        private float MR_ULS_Wind;
        private float MR_SLS;
        private EWindRegion MEWindRegion;

        private float MApexHeigth_H_2;

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

                // TODO Ondrej - napojit, aby to bolo zjednotene, pouzit funkcie v CPFDLoadInput

                //SetDesignLifeValueFromDatabaseValues();
                //SetAnnualProbabilityValuesFromDatabaseValues();

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

                // TODO Ondrej - napojit, aby to bolo zjednotene, pouzit funkcie v CPFDLoadInput

                //SetAnnualProbabilityValuesFromDatabaseValues();

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
        public float LeeMultiplier_Mlee
        {
            get
            {
                return MLeeMultiplier_Mlee;
            }

            set
            {
                MLeeMultiplier_Mlee = value;

                NotifyPropertyChanged("LeeMultiplier_Mlee");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float HillShapeMultiplier_Mh
        {
            get
            {
                return MHillShapeMultiplier_Mh;
            }

            set
            {
                MHillShapeMultiplier_Mh = value;

                NotifyPropertyChanged("HillShapeMultiplier_Mh");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float TopographicMultiplier_Mt
        {
            get
            {
                return MTopographicMultiplier_Mt;
            }

            set
            {
                MTopographicMultiplier_Mt = value;

                NotifyPropertyChanged("TopographicMultiplier_Mt");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ShieldingMultiplier_Ms
        {
            get
            {
                return MShieldingMultiplier_Ms;
            }

            set
            {
                MShieldingMultiplier_Ms = value;

                NotifyPropertyChanged("ShieldingMultiplier_Ms");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindDirectionMultiplier_Md
        {
            get
            {
                return MWindDirectionMultiplier_Md;
            }

            set
            {
                MWindDirectionMultiplier_Md = value;

                NotifyPropertyChanged("WindDirectionMultiplier_Md");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float TerrainHeightMultiplier_Mzcat
        {
            get
            {
                return MTerrainHeightMultiplier_Mzcat;
            }

            set
            {
                MTerrainHeightMultiplier_Mzcat = value;

                NotifyPropertyChanged("TerrainHeightMultiplier_Mzcat");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float TributaryArea_A
        {
            get
            {
                return MTributaryArea_A;
            }

            set
            {
                MTributaryArea_A = value;

                NotifyPropertyChanged("TributaryArea_A");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AreaReductionFactor_Ka
        {
            get
            {
                return MAreaReductionFactor_Ka;
            }

            set
            {
                MAreaReductionFactor_Ka = value;

                NotifyPropertyChanged("AreaReductionFactor_Ka");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LocalPressureFactor_Kl
        {
            get
            {
                return MLocalPressureFactor_Kl;
            }

            set
            {
                MLocalPressureFactor_Kl = value;

                NotifyPropertyChanged("LocalPressureFactor_Kl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PorousCladdingReductionFactor_Kp
        {
            get
            {
                return MPorousCladdingReductionFactor_Kp;
            }

            set
            {
                MPorousCladdingReductionFactor_Kp = value;

                NotifyPropertyChanged("PorousCladdingReductionFactor_Kp");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float CombinationFactorExternalPressures_Kce
        {
            get
            {
                return MCombinationFactorExternalPressures_Kce;
            }

            set
            {
                MCombinationFactorExternalPressures_Kce = value;

                NotifyPropertyChanged("CombinationFactorExternalPressures_Kce");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float CombinationFactorExternalPressures_Kci
        {
            get
            {
                return MCombinationFactorExternalPressures_Kci;
            }

            set
            {
                MCombinationFactorExternalPressures_Kci = value;

                NotifyPropertyChanged("CombinationFactorExternalPressures_Kci");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float InternalPressureCoefficient_Cpimin
        {
            get
            {
                return MInternalPressureCoefficient_Cpimin;
            }

            set
            {
                MInternalPressureCoefficient_Cpimin = value;

                NotifyPropertyChanged("InternalPressureCoefficient_Cpimin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float InternalPressureCoefficient_Cpimax
        {
            get
            {
                return MInternalPressureCoefficient_Cpimax;
            }

            set
            {
                MInternalPressureCoefficient_Cpimax = value;

                NotifyPropertyChanged("InternalPressureCoefficient_Cpimax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ExternalPressureCoefficient_Cpemin
        {
            get
            {
                return MExternalPressureCoefficient_Cpemin;
            }

            set
            {
                MExternalPressureCoefficient_Cpemin = value;

                NotifyPropertyChanged("ExternalPressureCoefficient_Cpemin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ExternalPressureCoefficient_Cpemax
        {
            get
            {
                return MExternalPressureCoefficient_Cpemax;
            }

            set
            {
                MExternalPressureCoefficient_Cpemax = value;

                NotifyPropertyChanged("ExternalPressureCoefficient_Cpemax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AerodynamicShapeFactor_Cfigimin
        {
            get
            {
                return MAerodynamicShapeFactor_Cfigimin;
            }

            set
            {
                MAerodynamicShapeFactor_Cfigimin = value;

                NotifyPropertyChanged("AerodynamicShapeFactor_Cfigimin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AerodynamicShapeFactor_Cfigimax
        {
            get
            {
                return MAerodynamicShapeFactor_Cfigimax;
            }

            set
            {
                MAerodynamicShapeFactor_Cfigimax = value;

                NotifyPropertyChanged("AerodynamicShapeFactor_Cfigimax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AerodynamicShapeFactor_Cfigemin
        {
            get
            {
                return MAerodynamicShapeFactor_Cfigemin;
            }

            set
            {
                MAerodynamicShapeFactor_Cfigemin = value;

                NotifyPropertyChanged("AerodynamicShapeFactor_Cfigemin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AerodynamicShapeFactor_Cfigemax
        {
            get
            {
                return MAerodynamicShapeFactor_Cfigemax;
            }

            set
            {
                MAerodynamicShapeFactor_Cfigemax = value;

                NotifyPropertyChanged("AerodynamicShapeFactor_Cfigemax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float GableWidth
        {
            get
            {
                return MGableWidth;
            }

            set
            {
                MGableWidth = value;

                NotifyPropertyChanged("GableWidth");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Length
        {
            get
            {
                return MLength;
            }

            set
            {
                MLength = value;

                NotifyPropertyChanged("Length");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WallHeight
        {
            get
            {
                return MWallHeight;
            }

            set
            {
                MWallHeight = value;

                NotifyPropertyChanged("WallHeight");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RoofPitch_deg
        {
            get
            {
                return MRoofPitch_deg;
            }

            set
            {
                MRoofPitch_deg = value;

                NotifyPropertyChanged("RoofPitch_deg");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AverageStructureHeight_h
        {
            get
            {
                return MAverageStructureHeight_h;
            }

            set
            {
                MAverageStructureHeight_h = value;

                NotifyPropertyChanged("AverageStructureHeight_h");
            }
        }

        // Not in GUI
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
        public float ApexHeigth_H_2
        {
            get
            {
                return MApexHeigth_H_2;
            }

            set
            {
                MApexHeigth_H_2 = value;
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
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public WindPressureCalculatorViewModel()
        {
            // Set default

            DesignLifeIndex = 4;
            ImportanceClassIndex = 1;

            SiteElevation = 10; // m
            WindRegionIndex = 6;
            TerrainCategoryIndex = 3;
            AngleWindDirectionIndex = 90;

            LeeMultiplier_Mlee = 1.0f;

            TributaryArea_A = 16f; // m^2

            LocalPressureFactor_Kl = 1.5f;
            PorousCladdingReductionFactor_Kp = 1.0f;

            MCombinationFactorExternalPressures_Kce = 0.8f;
            MCombinationFactorExternalPressures_Kci = 1.0f;

            GableWidth = 20f; // m
            Length = 40f; // m
            WallHeight = 6f; // m
            RoofPitch_deg = 8f; // deg

            float fRoofPitch_rad = RoofPitch_deg / 180f * MathF.fPI;
            float fPedimentHeight_H1toH2 = 0.5f * GableWidth * (float)Math.Tan(fRoofPitch_rad);
            ApexHeigth_H_2 = WallHeight + fPedimentHeight_H1toH2; // Apex Height

            // Vyska pre urcenie faktorov zatazenia vetrom (h) pre portal frame building
            AverageStructureHeight_h = WallHeight + 0.5f * fPedimentHeight_H1toH2; // Wall Height + 0.5 * (Gable Roof Apex Height - Wall Height)

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleComponentParamsViewPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.PropertyChanged(sender, e);
        }
    }
}
