using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.ViewModels
{
    public class FrameInternalForces_2DViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        private int MIFTypeIndex;
        private string MIFTypeUnit;
        private double MInternalForceScale_user;

        private bool MShowLabels;
        private bool MShowEndValues;
        private bool MShowExtremeValues;
        private bool MShowEverySecondSection;
        private bool MShowEveryThirdSection;
        private bool MShowMembers;
        private bool MShowAll;
        private bool MShowUnits;
        private int MNumberOfDecimalPlacesIndex;
        private int MNumberOfDecimalPlaces;
        private List<int> MListDecimalPlaces;
        private int MFontSizeIndex;
        private int MFontSize;
        private List<int> MListFontSize;

        List<string> list_IFTypes;

        public int IFTypeIndex
        {
            get
            {
                return MIFTypeIndex;
            }

            set
            {
                MIFTypeIndex = value;
                //"N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz"
                if (MIFTypeIndex <= 2) MIFTypeUnit = "kN";
                else if(MIFTypeIndex <= 6) MIFTypeUnit = "kNm";
                else MIFTypeUnit = "mm";
                NotifyPropertyChanged("IFTypeIndex");
            }
        }

        public List<string> List_IFTypes
        {
            get
            {
                return list_IFTypes;
            }

            set
            {
                list_IFTypes = value;
            }
        }

        public string IFTypeUnit
        {
            get
            {
                return MIFTypeUnit;
            }

            set
            {
                MIFTypeUnit = value;
            }
        }

        public double InternalForceScale_user
        {
            get
            {
                return MInternalForceScale_user;
            }

            set
            {
                MInternalForceScale_user = value;
                //do not allow negative zoom
                if (MInternalForceScale_user < 0) MInternalForceScale_user = 0;

                NotifyPropertyChanged("InternalForceScale_user");
            }
        }

        public bool ShowExtremeValues
        {
            get
            {
                return MShowExtremeValues;
            }

            set
            {
                MShowExtremeValues = value;
                NotifyPropertyChanged("ShowExtremeValues");
            }
        }

        public bool ShowEndValues
        {
            get
            {
                return MShowEndValues;
            }

            set
            {
                MShowEndValues = value;
                NotifyPropertyChanged("ShowEndValues");
            }
        }

        public bool ShowEverySecondSection
        {
            get
            {
                return MShowEverySecondSection;
            }

            set
            {
                MShowEverySecondSection = value;
                NotifyPropertyChanged("ShowEverySecondSection");
            }
        }

        public bool ShowEveryThirdSection
        {
            get
            {
                return MShowEveryThirdSection;
            }

            set
            {
                MShowEveryThirdSection = value;
                NotifyPropertyChanged("ShowEveryThirdSection");
            }
        }

        public bool ShowAll
        {
            get
            {
                return MShowAll;
            }

            set
            {
                MShowAll = value;
                NotifyPropertyChanged("ShowAll");
            }
        }

        public bool ShowMembers
        {
            get
            {
                return MShowMembers;
            }

            set
            {
                MShowMembers = value;
                NotifyPropertyChanged("ShowMembers");
            }
        }

        public bool ShowLabels
        {
            get
            {
                return MShowLabels;
            }

            set
            {
                MShowLabels = value;
                NotifyPropertyChanged("ShowLabels");
            }
        }

        public bool ShowUnits
        {
            get
            {
                return MShowUnits;
            }

            set
            {
                MShowUnits = value;
                NotifyPropertyChanged("ShowUnits");
            }
        }

        public int NumberOfDecimalPlacesIndex
        {
            get
            {
                return MNumberOfDecimalPlacesIndex;
            }

            set
            {
                MNumberOfDecimalPlacesIndex = value;
                NumberOfDecimalPlaces = ListDecimalPlaces[MNumberOfDecimalPlacesIndex];
                NotifyPropertyChanged("NumberOfDecimalPlacesIndex");
            }
        }
        public int NumberOfDecimalPlaces
        {
            get
            {
                return MNumberOfDecimalPlaces;
            }

            set
            {
                MNumberOfDecimalPlaces = value;
            }
        }

        public List<int> ListDecimalPlaces
        {
            get
            {
                return MListDecimalPlaces;
            }

            set
            {
                MListDecimalPlaces = value;
            }
        }

        public int FontSizeIndex
        {
            get
            {
                return MFontSizeIndex;
            }

            set
            {
                MFontSizeIndex = value;
                FontSize = ListFontSizes[MFontSizeIndex];
                NotifyPropertyChanged("FontSizeIndex");
            }
        }
        public int FontSize
        {
            get
            {
                return MFontSize;
            }

            set
            {
                MFontSize = value;
            }
        }

        public List<int> ListFontSizes
        {
            get
            {
                return MListFontSize;
            }

            set
            {
                MListFontSize = value;
            }
        }

        public FrameInternalForces_2DViewModel()
        {
            IFTypeIndex = 4;
            InternalForceScale_user = 1;

            list_IFTypes = new List<string>() { "N", "Vz", "Vy", "T", "My", "Mz", "δy", "δz" };

            MListDecimalPlaces = new List<int>() { 0, 1, 2, 3 };
            NumberOfDecimalPlacesIndex = 2;
            MListFontSize = new List<int>() {10, 12, 14, 16, 20 };
            FontSizeIndex = 1;

            ShowExtremeValues = true;
            ShowMembers = true;
            ShowLabels = true;
            ShowUnits = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
