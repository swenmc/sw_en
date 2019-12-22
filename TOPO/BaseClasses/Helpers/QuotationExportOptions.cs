using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.Helpers
{
    public class QuotationExportOptions
    {
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

        public bool DisplayMembers
        {
            get
            {
                return m_DisplayMembers;
            }

            set
            {
                m_DisplayMembers = value;
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
            }
        }

        public QuotationExportOptions()
        {

        }

        public QuotationExportOptions(QuotationDisplayOptions opts)
        {
            DisplayMembers = opts.DisplayMembers;
            DisplayPlates = opts.DisplayPlates;
            DisplayConnectors = opts.DisplayConnectors;
            DisplayBoltNuts = opts.DisplayBoltNuts;
            DisplayCladding = opts.DisplayCladding;
            DisplayFibreglass = opts.DisplayFibreglass;
            DisplayRoofNetting = opts.DisplayRoofNetting;
            DisplayDoorsAndWindows = opts.DisplayDoorsAndWindows;
            DisplayGutters = opts.DisplayGutters;
            DisplayDownpipe = opts.DisplayDownpipe;
            DisplayFlashing = opts.DisplayFlashing;

        }

    }
}
