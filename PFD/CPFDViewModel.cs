using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CPFDViewModel
    {

        //float fb; // 3 - 100 m
        //float fL; // 3 - 150 m
        //float fh; // 2 -  50 m (h1)
        //float fL1;
        //int iFrNo; // 2 - 50
        //float fRoofPitch_radians; // (zadavane v stupnoch - limity stupne 3 - 50 deg)
        //float fh2;
        //float fdist_girt; // 0.5 - 5 m
        //float fdist_purlin; // 0.5 - 5 m
        //float fdist_frontcolumn; // 1 - 10 m
        //float fdist_girt_bottom; // 1 - 10 m

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
                if (value < 3 || value > 100)
                    throw new ArgumentException("Cable Width must be between 3 and 100 [m]");
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
                if (value < 3 || value > 150)
                    throw new ArgumentException("Length must be between 3 and 150 [m]");
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
                if (value < 2 || value > 50)
                    throw new ArgumentException("Wall Height must be between 2 and 50 [m]");
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
                // Todo - Ondrej - pada ak zadam napr 60 stupnov, tato kontrola ma prebehnut len ak je hodnota zadana do text boxu uzivatelsky, ak sa prepocita z inych hodnot (napr. b), tak by sa mala zobrazit
                // aj ked je nevalidna a malo by vypisat nizsie uvedene varovanie, model by sa nemal prekreslit kym nie su vsetky hodnoty validne
                //if (value < 3 || value > 50)
                //    throw new ArgumentException("Roof Pitch must be between 2 and 50 degrees");
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
                if (value < 2 || value > 50)
                    throw new ArgumentException("Number of frames must be between 2 and 50");
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
                if (value < 0.5 || value > 5)
                    throw new ArgumentException("Girt distance must be between 0.5 and 5 [m]");
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
                if (value < 0.5 || value > 5)
                    throw new ArgumentException("Purlin distance must be between 0.5 and 5 [m]");
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
                if (value < 1 || value > 10)
                    throw new ArgumentException("Column distance must be between 1 and 10 [m]");
                MColumnDistance = value;
            }
        }

        public CPFDViewModel()
        {
            // Todo - Ondrej - toto by sa malo nacitat podla defaultnej hodnoty indexu nastavenej v comboboxe

            /* Set default values */
            MGableWidth = 8;
            MLength = 12;
            MWallHeight = 3.6f;
            MRoofPitch = 11;
            MFrames = 4;
            MGirtDistance = 1;
            MPurlinDistance = 1;
            MColumnDistance = 2;
        }
    }
}
