using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CPFDViewModel
    {

        //float fb; // 3000 - 100000 mm
        //float fL; // 3000 - 150000 mm
        //float fh; // 2000 -  50000 mm (h1)
        //float fL1;
        //int iFrNo; // 2 - 30
        //float fRoofPitch_radians; // (zadavane v stupnoch - limity stupne 3 - 50 deg)
        //float fh2;
        //float fdist_girt; // 500 - 5000 mm
        //float fdist_purlin; // 500 - 5000 mm
        //float fdist_frontcolumn; // 1000 - 10000 mm
        //float fdist_girt_bottom; // 1000 - 10000 mm

        private double MGableWidth;
        private double MLength;
        private double MWallHeight;
        private double MRoofPitch;
        private int MFrames;
        private double MGirtDistance;
        private double MPurlinDistance;
        private double MColumnDistance;

        public double GableWidth
        {
            get
            {
                return MGableWidth;
            }
            set
            {
                if (value < 3000 || value > 100000)
                    throw new ArgumentException("Cable Width must be between 3000 and 100000 [mm]");
                MGableWidth = value;
            }
        }

        public double Length
        {
            get
            {
                return MLength;
            }

            set
            {
                if (value < 3000 || value > 150000)
                    throw new ArgumentException("Length must be between 3000 and 150000 [mm]");
                MLength = value;
            }
        }

        public double WallHeight
        {
            get
            {
                return MWallHeight;
            }

            set
            {
                if (value < 2000 || value > 50000)
                    throw new ArgumentException("Wall Height must be between 2000 and 50000 [mm]");
                MWallHeight = value;
            }
        }

        public double RoofPitch
        {
            get
            {
                return MRoofPitch;
            }

            set
            {
                if (value < 3 || value > 50)
                    throw new ArgumentException("Roof Pitch must be between 2 and 50 degrees");
                MRoofPitch = value;
            }
        }

        public int Frames
        {
            get
            {
                return MFrames;
            }

            set
            {
                if (value < 2 || value > 30)
                    throw new ArgumentException("Frames must be between 2 and 30");
                MFrames = value;
            }
        }

        public double GirtDistance
        {
            get
            {
                return MGirtDistance;
            }

            set
            {
                if (value < 500 || value > 5000)
                    throw new ArgumentException("Girt distance must be between 500 and 5000 [mm]");
                MGirtDistance = value;
            }
        }

        public double PurlinDistance
        {
            get
            {
                return MPurlinDistance;
            }

            set
            {
                if (value < 500 || value > 5000)
                    throw new ArgumentException("Purlin distance must be between 500 and 5000 [mm]");
                MPurlinDistance = value;
            }
        }

        public double ColumnDistance
        {
            get
            {
                return MColumnDistance;
            }

            set
            {
                if (value < 1000 || value > 10000)
                    throw new ArgumentException("Column distance must be between 1000 and 10000 [mm]");
                MColumnDistance = value;
            }
        }

        public CPFDViewModel()
        {
            // Todo - Ondrej - toto by sa malo nacitat podla defaultnej hodnoty indexu nastavenej v comboboxe

            /* Set default values */
            MGableWidth = 8000;
            MLength = 12000;
            MWallHeight = 3600;
            MRoofPitch = 15;
            MFrames = 4;
            MGirtDistance = 1000;
            MPurlinDistance = 1000;
            MColumnDistance = 2000;
        }
    }
}
