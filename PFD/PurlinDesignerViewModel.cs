using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using MATH;
using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;

namespace PFD
{
    public class PurlinDesignerViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        //public SQLiteConnection conn;
        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region private fields
        private float MLength_L;
        private float MBracingLength_Lb;
        private float MTributaryWidth_B;
        private float MTributaryArea_A;
        private int MCrossSectionIndex;
        private float MArea_Ag;
        private float MMomentOfInertia_Ix;
        private int MMaterialIndex;
        private float MYieldStrength_fy;
        private float MTensileStrength_fu;
        private float MPurlinSelfWeight_gp;
        private float MCladdingSelfWeight_gc;
        private float MAdditionalDeadLoad_g;
        private float MLiveLoad_q;
        private float MSnowLoad_s;
        private float MWindLoadInternalPressure_pimin;
        private float MWindLoadInternalPressure_pimax;
        private float MWindLoadExternalPressure_pemin;
        private float MWindLoadExternalPressure_pemax;

        private float MWindLoadUpwind_puwl;
        private float MWindLoadDownwind_pdwl;

        private float MCladdingSelfWeight_gcl;
        private float MAdditionalDeadLoad_gl;
        private float MLiveLoad_ql;
        private float MSnowLoad_sl;
        private float MInternalPressure_piminl;
        private float MInternalPressure_pimaxl;
        private float MExternalPressure_peminl;
        private float MExternalPressure_pemaxl;

        private float MBendingMomentUpwind_M_asterix;
        private float MShearForceUpwind_V_asterix;
        private float MBendingMomentDownwind_M_asterix;
        private float MShearForceDownwind_V_asterix;

        private float MBendingCapacity_Ms;
        private float MElasticBucklingMoment_Mo;
        private float MNominalMemberCapacity_Mbe;
        private float MElasticBucklingMoment_Mol;
        private float MNominalMemberCapacity_Mbl;
        private float MElasticBucklingMoment_Mod;
        private float MNominalMemberCapacity_Mbd;
        private float MBendingCapacity_Mb;
        private float MShearCapacity_Vy; // Yield shear force
        private float MShearCapacity_Vw;

        private float MDesignRatioStrength_eta;

        private float MDeflectionGQ_Delta;
        private float MDeflectionGQLimitFraction;
        private float MDeflectionGQLimit_Delta_lim;
        private float MDesignRatioDeflectionGQ_eta;
        private float MDeflection_W_Upwind_Delta;
        private float MDeflection_SW_Downwind_Delta;
        private float MDeflectionSWLimitFraction;
        private float MDeflectionLimit_SW_Delta_lim;
        private float MDesignRatio_SW_Deflection_eta;

        private float MDeflectionTotalUpwind_Delta;
        private float MDeflectionTotalDownwind_Delta;
        private float MDeflectionTotalLimitFraction;
        private float MDeflectionTotalLimit_Delta_lim;
        private float MDesignRatioDeflectionTotal_eta;
        #endregion


        #region Properties
        //-------------------------------------------------------------------------------------------------------------
        public float Length_L
        {
            get
            {
                return MLength_L;
            }
            set
            {
                if (value < 0.5 || value > 50)
                    throw new ArgumentException("Purlin length must be between 0.5 and 50 [m]");

                MLength_L = value;

                if (MLength_L < MBracingLength_Lb)
                {
                    BracingLength_Lb = MLength_L;
                    MessageBox.Show($"Distance of bracing blocks was changed to {MLength_L}.");
                }
                
                NotifyPropertyChanged("Length_L");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BracingLength_Lb
        {
            get
            {
                return MBracingLength_Lb;
            }
            set
            {
                if (value < 0.5)
                {
                    MBracingLength_Lb = 0.5f;
                }
                else if (value > Length_L)
                {
                    MBracingLength_Lb = Length_L;
                }
                else MBracingLength_Lb = value;
                //throw new ArgumentException("Distance of bracing blocks must be between 0.5 and " + Math.Round(Length_L,3).ToString() + " [m]");
                
                NotifyPropertyChanged("BracingLength_Lb");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float TributaryWidth_B
        {
            get
            {
                return MTributaryWidth_B;
            }
            set
            {
                if (value < 0.5 || value > 10)
                    throw new ArgumentException("Tributary width must be between 0.5 and 10 [m]");

                MTributaryWidth_B = value;

                NotifyPropertyChanged("TributaryWidth_B");
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
        public int CrossSectionIndex
        {
            get
            {
                return MCrossSectionIndex;
            }
            set
            {
                MCrossSectionIndex = value;

                NotifyPropertyChanged("CrossSectionIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Area_Ag
        {
            get
            {
                return MArea_Ag;
            }
            set
            {
                MArea_Ag = value;

                NotifyPropertyChanged("Area_Ag");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float MomentOfInertia_Ix
        {
            get
            {
                return MMomentOfInertia_Ix;
            }
            set
            {
                MMomentOfInertia_Ix = value;

                NotifyPropertyChanged("MomentOfInertia_Ix");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int MaterialIndex
        {
            get
            {
                return MMaterialIndex;
            }
            set
            {
                MMaterialIndex = value;

                NotifyPropertyChanged("MaterialIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float YieldStrength_fy
        {
            get
            {
                return MYieldStrength_fy;
            }
            set
            {
                MYieldStrength_fy = value;

                NotifyPropertyChanged("YieldStrength_fy");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float TensileStrength_fu
        {
            get
            {
                return MTensileStrength_fu;
            }
            set
            {
                MTensileStrength_fu = value;

                NotifyPropertyChanged("TensileStrength_fu");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PurlinSelfWeight_gp
        {
            get
            {
                return MPurlinSelfWeight_gp;
            }
            set
            {
                MPurlinSelfWeight_gp = value;

                NotifyPropertyChanged("PurlinSelfWeight_gp");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float CladdingSelfWeight_gc
        {
            get
            {
                return MCladdingSelfWeight_gc;
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Load value must be between 0 and 1 [kN/m²]");

                MCladdingSelfWeight_gc = value;

                NotifyPropertyChanged("CladdingSelfWeight_gc");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AdditionalDeadLoad_g
        {
            get
            {
                return MAdditionalDeadLoad_g;
            }
            set
            {
                if (value < 0 || value > 5)
                    throw new ArgumentException("Load value must be between 0 and 5 [kN/m²]");

                MAdditionalDeadLoad_g = value;

                NotifyPropertyChanged("AdditionalDeadLoad_g");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LiveLoad_q
        {
            get
            {
                return MLiveLoad_q;
            }
            set
            {
                if (value < 0 || value > 5)
                    throw new ArgumentException("Load value must be between 0 and 5 [kN/m²]");

                MLiveLoad_q = value;

                NotifyPropertyChanged("LiveLoad_q");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SnowLoad_s
        {
            get
            {
                return MSnowLoad_s;
            }
            set
            {
                if (value < 0 || value > 10)
                    throw new ArgumentException("Load value must be between 0 and 10 [kN/m²]");

                MSnowLoad_s = value;

                NotifyPropertyChanged("SnowLoad_s");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindLoadInternalPressure_pimin
        {
            get
            {
                return MWindLoadInternalPressure_pimin;
            }
            set
            {
                if (value < -2 || value > 2)
                    throw new ArgumentException("Load value must be between -2 and 2 [kN/m²]");

                MWindLoadInternalPressure_pimin = value;

                NotifyPropertyChanged("WindLoadInternalPressure_pimin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindLoadInternalPressure_pimax
        {
            get
            {
                return MWindLoadInternalPressure_pimax;
            }
            set
            {
                if (value < -2 || value > 2)
                    throw new ArgumentException("Load value must be between -2 and 2 [kN/m²]");

                MWindLoadInternalPressure_pimax = value;

                NotifyPropertyChanged("WindLoadInternalPressure_pimax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindLoadExternalPressure_pemin
        {
            get
            {
                return MWindLoadExternalPressure_pemin;
            }
            set
            {
                if (value < -5 || value > 5)
                    throw new ArgumentException("Load value must be between -5 and 5 [kN/m²]");

                MWindLoadExternalPressure_pemin = value;

                NotifyPropertyChanged("WindLoadExternalPressure_pemin");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindLoadExternalPressure_pemax
        {
            get
            {
                return MWindLoadExternalPressure_pemax;
            }
            set
            {
                if (value < -5 || value > 5)
                    throw new ArgumentException("Load value must be between -5 and 5 [kN/m²]");

                MWindLoadExternalPressure_pemax = value;

                NotifyPropertyChanged("WindLoadExternalPressure_pemax");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float CladdingSelfWeight_gcl
        {
            get
            {
                return MCladdingSelfWeight_gcl;
            }
            set
            {
                MCladdingSelfWeight_gcl = value;

                NotifyPropertyChanged("CladdingSelfWeight_gcl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float AdditionalDeadLoad_gl
        {
            get
            {
                return MAdditionalDeadLoad_gl;
            }
            set
            {
                MAdditionalDeadLoad_gl = value;

                NotifyPropertyChanged("AdditionalDeadLoad_gl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LiveLoad_ql
        {
            get
            {
                return MLiveLoad_ql;
            }
            set
            {
                MLiveLoad_ql = value;

                NotifyPropertyChanged("LiveLoad_ql");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SnowLoad_sl
        {
            get
            {
                return MSnowLoad_sl;
            }
            set
            {
                MSnowLoad_sl = value;

                NotifyPropertyChanged("SnowLoad_sl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float InternalPressure_piminl
        {
            get
            {
                return MInternalPressure_piminl;
            }
            set
            {
                MInternalPressure_piminl = value;

                NotifyPropertyChanged("InternalPressure_piminl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float InternalPressure_pimaxl
        {
            get
            {
                return MInternalPressure_pimaxl;
            }
            set
            {
                MInternalPressure_pimaxl = value;

                NotifyPropertyChanged("InternalPressure_pimaxl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ExternalPressure_peminl
        {
            get
            {
                return MExternalPressure_peminl;
            }
            set
            {
                MExternalPressure_peminl = value;

                NotifyPropertyChanged("ExternalPressure_peminl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ExternalPressure_pemaxl
        {
            get
            {
                return MExternalPressure_pemaxl;
            }
            set
            {
                MExternalPressure_pemaxl = value;

                NotifyPropertyChanged("ExternalPressure_pemaxl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindLoadUpwind_puwl
        {
            get
            {
                return MWindLoadUpwind_puwl;
            }
            set
            {
                MWindLoadUpwind_puwl = value;

                NotifyPropertyChanged("WindLoadUpwind_puwl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WindLoadDownwind_pdwl
        {
            get
            {
                return MWindLoadDownwind_pdwl;
            }
            set
            {
                MWindLoadDownwind_pdwl = value;

                NotifyPropertyChanged("WindLoadDownwind_pdwl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BendingMomentUpwind_M_asterix
        {
            get
            {
                return MBendingMomentUpwind_M_asterix;
            }
            set
            {
                MBendingMomentUpwind_M_asterix = value;

                NotifyPropertyChanged("BendingMomentUpwind_M_asterix");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ShearForceUpwind_V_asterix
        {
            get
            {
                return MShearForceUpwind_V_asterix;
            }
            set
            {
                MShearForceUpwind_V_asterix = value;

                NotifyPropertyChanged("ShearForceUpwind_V_asterix");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BendingMomentDownwind_M_asterix
        {
            get
            {
                return MBendingMomentDownwind_M_asterix;
            }
            set
            {
                MBendingMomentDownwind_M_asterix = value;

                NotifyPropertyChanged("BendingMomentDownwind_M_asterix");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ShearForceDownwind_V_asterix
        {
            get
            {
                return MShearForceDownwind_V_asterix;
            }
            set
            {
                MShearForceDownwind_V_asterix = value;

                NotifyPropertyChanged("ShearForceDownwind_V_asterix");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BendingCapacity_Ms
        {
            get
            {
                return MBendingCapacity_Ms;
            }
            set
            {
                MBendingCapacity_Ms = value;

                NotifyPropertyChanged("BendingCapacity_Ms");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ElasticBucklingMoment_Mo
        {
            get
            {
                return MElasticBucklingMoment_Mo;
            }
            set
            {
                MElasticBucklingMoment_Mo = value;

                NotifyPropertyChanged("ElasticBucklingMoment_Mo");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float NominalMemberCapacity_Mbe
        {
            get
            {
                return MNominalMemberCapacity_Mbe;
            }
            set
            {
                MNominalMemberCapacity_Mbe = value;

                NotifyPropertyChanged("NominalMemberCapacity_Mbe");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ElasticBucklingMoment_Mol
        {
            get
            {
                return MElasticBucklingMoment_Mol;
            }
            set
            {
                MElasticBucklingMoment_Mol = value;

                NotifyPropertyChanged("ElasticBucklingMoment_Mol");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float NominalMemberCapacity_Mbl
        {
            get
            {
                return MNominalMemberCapacity_Mbl;
            }
            set
            {
                MNominalMemberCapacity_Mbl = value;

                NotifyPropertyChanged("NominalMemberCapacity_Mbl");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ElasticBucklingMoment_Mod
        {
            get
            {
                return MElasticBucklingMoment_Mod;
            }
            set
            {
                MElasticBucklingMoment_Mod = value;

                NotifyPropertyChanged("ElasticBucklingMoment_Mod");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float NominalMemberCapacity_Mbd
        {
            get
            {
                return MNominalMemberCapacity_Mbd;
            }
            set
            {
                MNominalMemberCapacity_Mbd = value;

                NotifyPropertyChanged("NominalMemberCapacity_Mbd");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BendingCapacity_Mb
        {
            get
            {
                return MBendingCapacity_Mb;
            }
            set
            {
                MBendingCapacity_Mb = value;

                NotifyPropertyChanged("BendingCapacity_Mb");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ShearCapacity_Vy // Yield shear force
        {
            get
            {
                return MShearCapacity_Vy;
            }
            set
            {
                MShearCapacity_Vy = value;

                NotifyPropertyChanged("ShearCapacity_Vy");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ShearCapacity_Vw
        {
            get
            {
                return MShearCapacity_Vw;
            }
            set
            {
                MShearCapacity_Vw = value;

                NotifyPropertyChanged("ShearCapacity_Vw");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignRatioStrength_eta
        {
            get
            {
                return MDesignRatioStrength_eta;
            }
            set
            {
                MDesignRatioStrength_eta = value;

                NotifyPropertyChanged("DesignRatioStrength_eta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        // DEFLECTIONS
        //-------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionGQ_Delta
        {
            get
            {
                return MDeflectionGQ_Delta;
            }
            set
            {
                MDeflectionGQ_Delta = value;

                NotifyPropertyChanged("DeflectionGQ_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionGQLimitFraction
        {
            get
            {
                return MDeflectionGQLimitFraction;
            }
            set
            {
                if (value < 50 || value > 1000)
                    throw new ArgumentException("Fraction denominator value must be between 50 and 1000 [-]");

                MDeflectionGQLimitFraction = value;

                NotifyPropertyChanged("DeflectionGQLimitFraction");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionGQLimit_Delta_lim
        {
            get
            {
                return MDeflectionGQLimit_Delta_lim;
            }
            set
            {
                MDeflectionGQLimit_Delta_lim = value;

                NotifyPropertyChanged("DeflectionGQLimit_Delta_lim");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignRatioDeflectionGQ_eta
        {
            get
            {
                return MDesignRatioDeflectionGQ_eta;
            }
            set
            {
                MDesignRatioDeflectionGQ_eta = value;

                NotifyPropertyChanged("DesignRatioDeflectionGQ_eta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Deflection_W_Upwind_Delta
        {
            get
            {
                return MDeflection_W_Upwind_Delta;
            }
            set
            {
                MDeflection_W_Upwind_Delta = value;

                NotifyPropertyChanged("Deflection_W_Upwind_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Deflection_SW_Downwind_Delta
        {
            get
            {
                return MDeflection_SW_Downwind_Delta;
            }
            set
            {
                MDeflection_SW_Downwind_Delta = value;

                NotifyPropertyChanged("Deflection_SW_Downwind_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionSWLimitFraction
        {
            get
            {
                return MDeflectionSWLimitFraction;
            }
            set
            {
                if (value < 50 || value > 1000)
                    throw new ArgumentException("Fraction denominator value must be between 50 and 1000 [-]");

                MDeflectionSWLimitFraction = value;

                NotifyPropertyChanged("DeflectionSWLimitFraction");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionLimit_SW_Delta_lim
        {
            get
            {
                return MDeflectionLimit_SW_Delta_lim;
            }
            set
            {
                MDeflectionLimit_SW_Delta_lim = value;

                NotifyPropertyChanged("DeflectionLimit_SW_Delta_lim");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignRatio_SW_Deflection_eta
        {
            get
            {
                return MDesignRatio_SW_Deflection_eta;
            }
            set
            {
                MDesignRatio_SW_Deflection_eta = value;

                NotifyPropertyChanged("DesignRatio_SW_Deflection_eta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionTotalUpwind_Delta
        {
            get
            {
                return MDeflectionTotalUpwind_Delta;
            }
            set
            {
                MDeflectionTotalUpwind_Delta = value;

                NotifyPropertyChanged("DeflectionTotalUpwind_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionTotalDownwind_Delta
        {
            get
            {
                return MDeflectionTotalDownwind_Delta;
            }
            set
            {
                MDeflectionTotalDownwind_Delta = value;

                NotifyPropertyChanged("DeflectionTotalDownwind_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionTotalLimitFraction
        {
            get
            {
                return MDeflectionTotalLimitFraction;
            }
            set
            {
                if (value < 50 || value > 1000)
                    throw new ArgumentException("Fraction denominator value must be between 50 and 1000 [-]");

                MDeflectionTotalLimitFraction = value;

                NotifyPropertyChanged("DeflectionTotalLimitFraction");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionTotalLimit_Delta_lim
        {
            get
            {
                return MDeflectionTotalLimit_Delta_lim;
            }
            set
            {
                MDeflectionTotalLimit_Delta_lim = value;

                NotifyPropertyChanged("DeflectionTotalLimit_Delta_lim");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignRatioDeflectionTotal_eta
        {
            get
            {
                return MDesignRatioDeflectionTotal_eta;
            }
            set
            {
                MDesignRatioDeflectionTotal_eta = value;

                NotifyPropertyChanged("DesignRatioDeflectionTotal_eta");
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public PurlinDesignerViewModel()
        {
            // Set default
            Length_L = 6f;
            BracingLength_Lb = 2f;

            TributaryWidth_B = 2f;

            CrossSectionIndex = 1;
            MaterialIndex = 11; // Default AS1397 - Grade G550‡ (database row ID 12)

            CladdingSelfWeight_gc = 0.05f; // kN/m^2
            AdditionalDeadLoad_g = 0.10f; // kN/m^2

            LiveLoad_q = 0.25f; // kN/m^2
            SnowLoad_s = 0; // kN/m^2

            // TODO - napojit vypocet z Wind Load Calculator a po spusteni zapisat hodnoty do Purlin Designer
            WindLoadInternalPressure_pimin = -0.3f; // kN/m^2
            WindLoadInternalPressure_pimax = 0; // kN/m^2

            WindLoadExternalPressure_pemin = -1.0f; // kN/m^2
            WindLoadExternalPressure_pemax = 0.15f; // kN/m^2

            DeflectionGQLimitFraction = 300f;
            DeflectionSWLimitFraction = 120f;
            DeflectionTotalLimitFraction = 150f;

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
