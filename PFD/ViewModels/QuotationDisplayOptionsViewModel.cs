﻿using BaseClasses.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    [Serializable]
    public class QuotationDisplayOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_DisplayMembers;
        private bool m_DisplayPlates;
        private bool m_DisplayConnectors;
        private bool m_DisplayBoltNuts;
        private bool m_DisplayCladding;
        private bool m_DisplayFibreglass;
        private bool m_DisplayRoofNetting;
        private bool m_DisplayDoorsAndWindows;
        private bool m_DisplayGutters;
        private bool m_DisplayDownpipe;
        private bool m_DisplayFlashing;
        private bool m_DisplayPackers;
        private bool m_DisplayCladdingAccesories;
        private bool m_CalculateFreightAuto;
        

        public bool DisplayMembers
        {
            get
            {
                return m_DisplayMembers;
            }

            set
            {
                m_DisplayMembers = value;
                NotifyPropertyChanged("DisplayMembers");
            }
        }

        public bool DisplayPlates
        {
            get
            {
                return m_DisplayPlates;
            }

            set
            {
                m_DisplayPlates = value;
                NotifyPropertyChanged("DisplayPlates");
            }
        }

        public bool DisplayConnectors
        {
            get
            {
                return m_DisplayConnectors;
            }

            set
            {
                m_DisplayConnectors = value;
                NotifyPropertyChanged("DisplayConnectors");
            }
        }

        public bool DisplayBoltNuts
        {
            get
            {
                return m_DisplayBoltNuts;
            }

            set
            {
                m_DisplayBoltNuts = value;
                NotifyPropertyChanged("DisplayBoltNuts");
            }
        }

        public bool DisplayCladding
        {
            get
            {
                return m_DisplayCladding;
            }

            set
            {
                m_DisplayCladding = value;
                NotifyPropertyChanged("DisplayCladding");
            }
        }

        public bool DisplayFibreglass
        {
            get
            {
                return m_DisplayFibreglass;
            }

            set
            {
                m_DisplayFibreglass = value;
                NotifyPropertyChanged("DisplayFibreglass");
            }
        }

        public bool DisplayRoofNetting
        {
            get
            {
                return m_DisplayRoofNetting;
            }

            set
            {
                m_DisplayRoofNetting = value;
                NotifyPropertyChanged("DisplayRoofNetting");
            }
        }

        public bool DisplayDoorsAndWindows
        {
            get
            {
                return m_DisplayDoorsAndWindows;
            }

            set
            {
                m_DisplayDoorsAndWindows = value;
                NotifyPropertyChanged("DisplayDoorsAndWindows");
            }
        }

        public bool DisplayGutters
        {
            get
            {
                return m_DisplayGutters;
            }

            set
            {
                m_DisplayGutters = value;
                NotifyPropertyChanged("DisplayGutters");
            }
        }

        public bool DisplayDownpipe
        {
            get
            {
                return m_DisplayDownpipe;
            }

            set
            {
                m_DisplayDownpipe = value;
                NotifyPropertyChanged("DisplayDownpipe");
            }
        }

        public bool DisplayFlashing
        {
            get
            {
                return m_DisplayFlashing;
            }

            set
            {
                m_DisplayFlashing = value;
                NotifyPropertyChanged("DisplayFlashing");
            }
        }

        public bool DisplayPackers
        {
            get
            {
                return m_DisplayPackers;
            }

            set
            {
                m_DisplayPackers = value;
                NotifyPropertyChanged("DisplayPackers");
            }
        }

        public bool DisplayCladdingAccesories
        {
            get
            {
                return m_DisplayCladdingAccesories;
            }

            set
            {
                m_DisplayCladdingAccesories = value;
                NotifyPropertyChanged("DisplayCladdingAccesories");
            }
        }

        public bool CalculateFreightAuto
        {
            get
            {
                return m_CalculateFreightAuto;
            }

            set
            {
                m_CalculateFreightAuto = value;
                NotifyPropertyChanged("CalculateFreightAuto");
            }
        }

        public QuotationDisplayOptionsViewModel() { }

        public QuotationDisplayOptionsViewModel(bool checkAll)
        {
            DisplayMembers = checkAll;
            DisplayPlates = checkAll;
            DisplayConnectors = checkAll;
            DisplayBoltNuts = checkAll;
            DisplayCladding = checkAll;
            DisplayFibreglass = checkAll;
            DisplayRoofNetting = checkAll;
            DisplayDoorsAndWindows = checkAll;
            DisplayGutters = checkAll;
            DisplayDownpipe = checkAll;
            DisplayFlashing = checkAll;
            DisplayPackers = checkAll;
            DisplayCladdingAccesories = checkAll;

            CalculateFreightAuto = checkAll;
        }


        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(QuotationDisplayOptionsViewModel vm)
        {
            if (vm == null) return;

            DisplayMembers = vm.DisplayMembers;
            DisplayPlates = vm.DisplayPlates;
            DisplayConnectors = vm.DisplayConnectors;
            DisplayBoltNuts = vm.DisplayBoltNuts;
            DisplayCladding = vm.DisplayCladding;
            DisplayFibreglass = vm.DisplayFibreglass;
            DisplayRoofNetting = vm.DisplayRoofNetting;
            DisplayDoorsAndWindows = vm.DisplayDoorsAndWindows;
            DisplayGutters = vm.DisplayGutters;
            DisplayDownpipe = vm.DisplayDownpipe;
            DisplayFlashing = vm.DisplayFlashing;
            DisplayPackers = vm.DisplayPackers;
            DisplayCladdingAccesories = vm.DisplayCladdingAccesories;
            CalculateFreightAuto = vm.CalculateFreightAuto;
        }

    }
}
