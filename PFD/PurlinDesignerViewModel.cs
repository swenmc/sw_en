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
        private float MLength_L;
        private float MTributaryWidth_B;
        private float MTributaryArea_A;
        private int MCrossSectionIndex;
        private float MPurlinSelfWeight_gp;
        private float MCladdingSelfWeight_gc;
        private float MAdditionalDeadLoad_g;
        private float MLiveLoad_q;
        private float MSnowLoad_s;
        private float MWindLoadInternalPressure_pimin;
        private float MWindLoadInternalPressure_pimax;
        private float MWindLoadExternalPressure_pemin;
        private float MWindLoadExternalPressure_pemax;

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
        private float MBendingCapacity_Mb;
        private float MShearCapacity_Vw;

        private float MBracingLength_Lb;

        private float MDeflectionUpwind_Delta;
        private float MDeflectionDownwind_Delta;

        private float MDeflectionLimit_Delta_lim;

        private float MDesignRatioDeflection_eta;

        //-------------------------------------------------------------------------------------------------------------
        public float Length_L
        {
            get
            {
                return MLength_L;
            }
            set
            {
                MLength_L = value;

                NotifyPropertyChanged("Length_L");
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
                MTributaryWidth_B = value;

                NotifyPropertyChanged("TributaryWidth_B");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float TributaryArea
        {
            get
            {
                return MTributaryArea_A;
            }
            set
            {
                MTributaryArea_A = value;

                NotifyPropertyChanged("TributaryArea");
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

                NotifyPropertyChanged("IExternalPressure_peminl");
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
        public float BracingLength_Lb
        {
            get
            {
                return MBracingLength_Lb;
            }
            set
            {
                MBracingLength_Lb = value;

                NotifyPropertyChanged("BracingLength_Lb");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionUpwind_Delta
        {
            get
            {
                return MDeflectionUpwind_Delta;
            }
            set
            {
                MDeflectionUpwind_Delta = value;

                NotifyPropertyChanged("DeflectionUpwind_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionDownwind_Delta
        {
            get
            {
                return MDeflectionDownwind_Delta;
            }
            set
            {
                MDeflectionDownwind_Delta = value;

                NotifyPropertyChanged("DeflectionDownwind_Delta");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DeflectionLimit_Delta_lim
        {
            get
            {
                return MDeflectionLimit_Delta_lim;
            }
            set
            {
                MDeflectionLimit_Delta_lim = value;

                NotifyPropertyChanged("DeflectionLimit_Delta_lim");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DesignRatioDeflection_eta
        {
            get
            {
                return MDesignRatioDeflection_eta;
            }
            set
            {
                MDesignRatioDeflection_eta = value;

                NotifyPropertyChanged("DesignRatioDeflection_eta");
            }
        }

        public PurlinDesignerViewModel()
        {
            // Set default
            Length_L = 6f;
            TributaryWidth_B = 2f;

            CrossSectionIndex = 0;

            CladdingSelfWeight_gc = 0.05f;
            AdditionalDeadLoad_g = 0.10f;

            LiveLoad_q = 0.25f;
            SnowLoad_s = 0;

            WindLoadInternalPressure_pimin = -0.3f;
            WindLoadInternalPressure_pimax = 0;

            WindLoadExternalPressure_pemin = -0.9f;
            WindLoadExternalPressure_pemax = 0.3f;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void HandleComponentParamsViewPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    this.PropertyChanged(sender, e);
        //}
    }
}
