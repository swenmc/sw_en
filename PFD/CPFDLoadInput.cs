using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CPFDLoadInput : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
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
        private int MSiteSubsoilClassIndex;
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

                DatabaseLocations dlocation = new DatabaseLocations(MLocationIndex);

                // TODO nastavit podla location dalsie parametre, vytvorit na to databazu

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
                if (value < 1 || value > 5)
                    throw new ArgumentException("Importance level must be between 1 and 5");
                MDesignLife = value;

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
        public int SiteSubsoilClassIndex
        {
            get
            {
                return MSiteSubsoilClassIndex;
            }

            set
            {
                if (value < 0 || value > 4)
                    throw new ArgumentException("Site subsoil class must be between A and E");
                MSiteSubsoilClassIndex = value;

                NotifyPropertyChanged("SubsoilClassIndex");
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
        public CPFDLoadInput(int locationIndex)
        {
            LocationIndex = locationIndex;

            //test ci sa to vyplna
            DesignLife = 20;
            AnnualProbabilityULS = 10;
            AnnualProbabilitySLS = 11;

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
