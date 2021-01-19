using BaseClasses;
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
    [Serializable]
    public class SolverOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        // Load Combination - options
        private bool MDeterminateCombinationResultsByFEMSolver;
        private bool MUseFEMSolverCalculationForSimpleBeam;
        private bool MDeterminateMemberLocalDisplacementsForULS;

        private bool MMultiCoreCalculation;
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

        public bool MultiCoreCalculation
        {
            get
            {
                return MMultiCoreCalculation;
            }

            set
            {
                MMultiCoreCalculation = value;
                NotifyPropertyChanged("MultiCoreCalculation");
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

            MultiCoreCalculation = true;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public void SetViewModel(SolverOptionsViewModel vm)
        {
            if (vm == null) return;
            
            DeterminateCombinationResultsByFEMSolver = vm.DeterminateCombinationResultsByFEMSolver;
            UseFEMSolverCalculationForSimpleBeam = vm.UseFEMSolverCalculationForSimpleBeam;
            DeterminateMemberLocalDisplacementsForULS = vm.DeterminateMemberLocalDisplacementsForULS;

            MultiCoreCalculation = vm.MultiCoreCalculation;
        }


    }
}