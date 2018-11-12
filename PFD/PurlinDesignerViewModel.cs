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
    public class PurlinDesignerViewModel
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        public SQLiteConnection conn;
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

        public PurlinDesignerViewModel()
        {

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
