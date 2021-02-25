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
    public class ModelOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private bool m_BracingEverySecondRowOfGirts;
        private bool m_BracingEverySecondRowOfPurlins;

        // Tato premenna urcuje aky bude detail spoja a ake budu excentricity prutov, resp. zakladovych patiek
        private bool m_WindPostUnderRafter; // Poloha wind post pod rafterom alebo za nim, moze byt nastavitelne, mohlo by byt aj automaticke podla velkosti prierezu

        private bool m_UseStraightReinforcementBars;
        private bool m_UpdateAutomatically;
        private bool m_VariousCrossSections;

        private bool m_VariousBayWidths;
        private bool m_EnableAccessories;
        private bool m_EnableJoints;
        private bool m_EnableFootings;
        private bool m_EnableCrossBracing;
        private bool m_EnableCanopies;
        private bool m_EnableCladding;
        private bool m_DisplayIndividualCladdingSheets;

        private bool m_SameColorsDoor;
        private bool m_SameColorsFlashings;
        private bool m_SameColorsGutters;
        private bool m_SameColorsDownpipes;
        private bool m_SameColorsFGD;

        private bool m_CenterlinesDimensions;
        private bool m_OverallDimensions;

        private bool m_UseMainColumnFlyBracingPlates;
        private bool m_UseRafterFlyBracingPlates;

        public bool BracingEverySecondRowOfGirts
        {
            get
            {
                return m_BracingEverySecondRowOfGirts;
            }

            set
            {
                m_BracingEverySecondRowOfGirts = value;

                NotifyPropertyChanged("BracingEverySecondRowOfGirts");
            }
        }

        public bool BracingEverySecondRowOfPurlins
        {
            get
            {
                return m_BracingEverySecondRowOfPurlins;
            }

            set
            {
                m_BracingEverySecondRowOfPurlins = value;
                
                NotifyPropertyChanged("BracingEverySecondRowOfPurlins");
            }
        }

        public bool WindPostUnderRafter
        {
            get
            {
                return m_WindPostUnderRafter;
            }

            set
            {
                m_WindPostUnderRafter = value;

                NotifyPropertyChanged("WindPostUnderRafter");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool UseStraightReinforcementBars
        {
            get
            {
                return m_UseStraightReinforcementBars;
            }

            set
            {
                m_UseStraightReinforcementBars = value;
                
                NotifyPropertyChanged("UseStraightReinforcementBars");
            }
        }

        public bool UpdateAutomatically
        {
            get
            {
                return m_UpdateAutomatically;
            }

            set
            {
                m_UpdateAutomatically = value;
                NotifyPropertyChanged("UpdateAutomatically");
            }
        }

        public bool VariousCrossSections
        {
            get
            {
                return m_VariousCrossSections;
            }
            set
            {
                m_VariousCrossSections = value;
                NotifyPropertyChanged("VariousCrossSections");
            }
        }

        public bool SameColorsDoor
        {
            get
            {
                return m_SameColorsDoor;
            }

            set
            {
                m_SameColorsDoor = value;
                NotifyPropertyChanged("SameColorsDoor");
            }
        }

        public bool SameColorsFlashings
        {
            get
            {
                return m_SameColorsFlashings;
            }

            set
            {
                m_SameColorsFlashings = value;
                NotifyPropertyChanged("SameColorsFlashings");
            }
        }

        public bool SameColorsGutters
        {
            get
            {
                return m_SameColorsGutters;
            }

            set
            {
                m_SameColorsGutters = value;
                NotifyPropertyChanged("SameColorsGutters");
            }
        }

        public bool SameColorsDownpipes
        {
            get
            {
                return m_SameColorsDownpipes;
            }

            set
            {
                m_SameColorsDownpipes = value;
                NotifyPropertyChanged("SameColorsDownpipes");
            }
        }

        public bool SameColorsFGD
        {
            get
            {
                return m_SameColorsFGD;
            }

            set
            {
                m_SameColorsFGD = value;
                IsSetFromCode = true;
                SameColorsFlashings = m_SameColorsFGD;
                SameColorsGutters = m_SameColorsFGD;
                SameColorsDownpipes = m_SameColorsFGD;                
                IsSetFromCode = false;
                NotifyPropertyChanged("SameColorsFGD");
            }
        }


        public bool CenterlinesDimensions
        {
            get
            {
                return m_CenterlinesDimensions;
            }

            set
            {
                if (value == false && m_OverallDimensions == false) return;

                m_CenterlinesDimensions = value;
                NotifyPropertyChanged("CenterlinesDimensions");
            }
        }

        public bool OverallDimensions
        {
            get
            {
                return m_OverallDimensions;
            }

            set
            {
                if (value == false && m_CenterlinesDimensions == false) return;

                m_OverallDimensions = value;
                NotifyPropertyChanged("OverallDimensions");
            }
        }

        public bool UseMainColumnFlyBracingPlates
        {
            get
            {
                return m_UseMainColumnFlyBracingPlates;
            }

            set
            {
                m_UseMainColumnFlyBracingPlates = value;
                NotifyPropertyChanged("UseMainColumnFlyBracingPlates");
            }
        }

        public bool UseRafterFlyBracingPlates
        {
            get
            {
                return m_UseRafterFlyBracingPlates;
            }

            set
            {
                m_UseRafterFlyBracingPlates = value;
                NotifyPropertyChanged("UseRafterFlyBracingPlates");
            }
        }

        public bool VariousBayWidths
        {
            get
            {
                return m_VariousBayWidths;
            }

            set
            {
                m_VariousBayWidths = value;
                NotifyPropertyChanged("VariousBayWidths");
            }
        }

        public bool EnableAccessories
        {
            get
            {
                return m_EnableAccessories;
            }

            set
            {
                m_EnableAccessories = value;
                NotifyPropertyChanged("EnableAccessories");
            }
        }

        public bool EnableJoints
        {
            get
            {
                return m_EnableJoints;
            }

            set
            {
                m_EnableJoints = value;
                NotifyPropertyChanged("EnableJoints");
            }
        }

        public bool EnableFootings
        {
            get
            {
                return m_EnableFootings;
            }

            set
            {
                m_EnableFootings = value;
                NotifyPropertyChanged("EnableFootings");
            }
        }

        public bool EnableCrossBracing
        {
            get
            {
                return m_EnableCrossBracing;
            }

            set
            {
                m_EnableCrossBracing = value;
                NotifyPropertyChanged("EnableCrossBracing");
            }
        }

        public bool EnableCanopies
        {
            get
            {
                return m_EnableCanopies;
            }

            set
            {
                m_EnableCanopies = value;
                NotifyPropertyChanged("EnableCanopies");
            }
        }

        public bool EnableCladding
        {
            get
            {
                return m_EnableCladding;
            }

            set
            {
                m_EnableCladding = value;
                NotifyPropertyChanged("EnableCladding");
            }
        }
        public bool DisplayIndividualCladdingSheets
        {
            get
            {
                return m_DisplayIndividualCladdingSheets;
            }

            set
            {
                m_DisplayIndividualCladdingSheets = value;
                NotifyPropertyChanged("DisplayIndividualCladdingSheets");
            }
        }

        public bool IsSetFromCode = false;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public ModelOptionsViewModel()
        {
            IsSetFromCode = true;

            m_BracingEverySecondRowOfGirts = true;
            m_BracingEverySecondRowOfPurlins = true;

            m_WindPostUnderRafter = false;

            m_UseStraightReinforcementBars = false;

            m_UpdateAutomatically = false;

            m_VariousCrossSections = false;

            m_VariousBayWidths = false;
            m_EnableAccessories = true;
            m_EnableJoints = true;
            m_EnableFootings = true;
            m_EnableCrossBracing = true;
            m_EnableCanopies = false;
            m_EnableCladding = true;
            m_DisplayIndividualCladdingSheets = true;

            m_SameColorsDoor = false;
            m_SameColorsFGD = false;
            m_SameColorsFlashings = false;
            m_SameColorsGutters = false;
            m_SameColorsDownpipes = false;

            m_CenterlinesDimensions = false;
            m_OverallDimensions = true;

            m_UseMainColumnFlyBracingPlates = true;
            m_UseRafterFlyBracingPlates = true;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(ModelOptionsViewModel vm)
        {
            if (vm == null) return;

            BracingEverySecondRowOfGirts = vm.BracingEverySecondRowOfGirts;
            BracingEverySecondRowOfPurlins = vm.BracingEverySecondRowOfPurlins;

            WindPostUnderRafter = vm.WindPostUnderRafter;

            UseStraightReinforcementBars = vm.UseStraightReinforcementBars;

            UpdateAutomatically = vm.UpdateAutomatically;

            VariousCrossSections = vm.VariousCrossSections;

            VariousBayWidths = vm.VariousBayWidths;
            EnableAccessories = vm.EnableAccessories;
            EnableJoints = vm.EnableJoints;
            EnableFootings = vm.EnableFootings;
            EnableCrossBracing = vm.EnableCrossBracing;
            EnableCanopies = vm.EnableCanopies;
            EnableCladding = vm.EnableCladding;
            DisplayIndividualCladdingSheets = vm.DisplayIndividualCladdingSheets;

            SameColorsDoor = vm.SameColorsDoor;
            SameColorsFGD = vm.SameColorsFGD;
            SameColorsFlashings = vm.SameColorsFlashings;
            SameColorsGutters = vm.SameColorsGutters;
            SameColorsDownpipes = vm.SameColorsDownpipes;

            CenterlinesDimensions = vm.CenterlinesDimensions;
            OverallDimensions = vm.OverallDimensions;

            UseMainColumnFlyBracingPlates = vm.UseMainColumnFlyBracingPlates;
            UseRafterFlyBracingPlates = vm.UseRafterFlyBracingPlates;
        }
    }
}