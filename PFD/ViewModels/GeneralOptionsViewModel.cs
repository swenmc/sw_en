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
    public class GeneralOptionsViewModel : INotifyPropertyChanged
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

        public bool IsSetFromCode = false;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public GeneralOptionsViewModel()
        {
            IsSetFromCode = true;

            m_BracingEverySecondRowOfGirts = true;
            m_BracingEverySecondRowOfPurlins = true;

            m_WindPostUnderRafter = false;

            m_UseStraightReinforcementBars = false;

            m_UpdateAutomatically = false;

            m_VariousCrossSections = false;

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