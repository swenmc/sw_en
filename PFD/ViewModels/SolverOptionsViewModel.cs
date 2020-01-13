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
    public class SolverOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        // Load Combination - options
        private bool MDeterminateCombinationResultsByFEMSolver;
        private bool MUseFEMSolverCalculationForSimpleBeam;
        private bool MDeterminateMemberLocalDisplacementsForULS;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        public bool DeterminateCombinationResultsByFEMSolver
        {
            get
            {
                return MDeterminateCombinationResultsByFEMSolver;
            }

            set
            {
                MDeterminateCombinationResultsByFEMSolver = value;
                
                NotifyPropertyChanged("DeterminateCombinationResultsByFEMSolver");
            }
        }

        public bool UseFEMSolverCalculationForSimpleBeam
        {
            get
            {
                return MUseFEMSolverCalculationForSimpleBeam;
            }

            set
            {
                MUseFEMSolverCalculationForSimpleBeam = value;
                
                NotifyPropertyChanged("UseFEMSolverCalculationForSimpleBeam");
            }
        }

        public bool DeterminateMemberLocalDisplacementsForULS
        {
            get
            {
                return MDeterminateMemberLocalDisplacementsForULS;
            }

            set
            {
                MDeterminateMemberLocalDisplacementsForULS = value;
                
                NotifyPropertyChanged("DeterminateMemberLocalDisplacementsForULS");
            }
        }



        public bool IsSetFromCode = false;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public SolverOptionsViewModel()
        {
            IsSetFromCode = true;

            DeterminateCombinationResultsByFEMSolver = false;
            UseFEMSolverCalculationForSimpleBeam = false;
            DeterminateMemberLocalDisplacementsForULS = false;

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