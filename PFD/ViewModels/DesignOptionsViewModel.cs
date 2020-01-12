﻿using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace PFD
{
    public class DesignOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        // Displacement / Deflection Limits
        private float MVerticalDisplacementLimitDenominator_Rafter_PL;
        private float MVerticalDisplacementLimitDenominator_Rafter_TL;
        private float MHorizontalDisplacementLimitDenominator_Column_TL;
        private float MVerticalDisplacementLimitDenominator_Purlin_PL;
        private float MVerticalDisplacementLimitDenominator_Purlin_TL;
        private float MHorizontalDisplacementLimitDenominator_Girt_TL;

        private bool MShearDesignAccording334; // Use shear design according to 3.3.4 or 7
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        // Displacement / Deflection Limits
        public float VerticalDisplacementLimitDenominator_Rafter_PL
        {
            get
            {
                return MVerticalDisplacementLimitDenominator_Rafter_PL;
            }

            set
            {
                MVerticalDisplacementLimitDenominator_Rafter_PL = value;
                
                NotifyPropertyChanged("VerticalDisplacementLimitDenominator_Rafter_PL");
            }
        }

        public float VerticalDisplacementLimitDenominator_Rafter_TL
        {
            get
            {
                return MVerticalDisplacementLimitDenominator_Rafter_TL;
            }

            set
            {
                MVerticalDisplacementLimitDenominator_Rafter_TL = value;
                
                NotifyPropertyChanged("VerticalDisplacementLimitDenominator_Rafter_TL");
            }
        }

        public float HorizontalDisplacementLimitDenominator_Column_TL
        {
            get
            {
                return MHorizontalDisplacementLimitDenominator_Column_TL;
            }

            set
            {
                MHorizontalDisplacementLimitDenominator_Column_TL = value;
                
                NotifyPropertyChanged("HorizontalDisplacementLimitDenominator_Column_TL");
            }
        }

        public float VerticalDisplacementLimitDenominator_Purlin_PL
        {
            get
            {
                return MVerticalDisplacementLimitDenominator_Purlin_PL;
            }

            set
            {
                MVerticalDisplacementLimitDenominator_Purlin_PL = value;
                
                NotifyPropertyChanged("VerticalDisplacementLimitDenominator_Purlin_PL");
            }
        }

        public float VerticalDisplacementLimitDenominator_Purlin_TL
        {
            get
            {
                return MVerticalDisplacementLimitDenominator_Purlin_TL;
            }

            set
            {
                MVerticalDisplacementLimitDenominator_Purlin_TL = value;
                
                NotifyPropertyChanged("VerticalDisplacementLimitDenominator_Purlin_TL");
            }
        }

        public float HorizontalDisplacementLimitDenominator_Girt_TL
        {
            get
            {
                return MHorizontalDisplacementLimitDenominator_Girt_TL;
            }

            set
            {
                MHorizontalDisplacementLimitDenominator_Girt_TL = value;
                
                NotifyPropertyChanged("HorizontalDisplacementLimitDenominator_Girt_TL");
            }
        }

        public bool ShearDesignAccording334
        {
            get
            {
                return MShearDesignAccording334;
            }

            set
            {
                MShearDesignAccording334 = value;
                
                NotifyPropertyChanged("ShearDesignAccording334");
            }
        }


        public bool IsSetFromCode = false;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DesignOptionsViewModel()
        {
            IsSetFromCode = true;

            // Displacement / Deflection Limits            
            MVerticalDisplacementLimitDenominator_Rafter_PL = 300;
            MVerticalDisplacementLimitDenominator_Rafter_TL = 250;
            MHorizontalDisplacementLimitDenominator_Column_TL = 150;
            MVerticalDisplacementLimitDenominator_Purlin_PL = 300;
            MVerticalDisplacementLimitDenominator_Purlin_TL = 150;
            MHorizontalDisplacementLimitDenominator_Girt_TL = 150;

            MShearDesignAccording334 = false;

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