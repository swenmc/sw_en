using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;

namespace SBD
{
    public class CSBDViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private float MLength;
        private float MLoadqy;
        private float MLoadqz;

        //-------------------------------------------------------------------------------------------------------------
        public float Length
        {
            get
            {
                return MLength;
            }
            set
            {
                if (value < 0.1 || value > 30)
                    throw new ArgumentException("Length must be between 0.1 and 30 [m]");
                MLength = value;

                NotifyPropertyChanged("Length");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Loadqy
        {
            get
            {
                return MLoadqy;
            }

            set
            {
                if (value < 0.01 || value > 30)
                    throw new ArgumentException("Value must be between 0.01 and 30 [kN/m]");
                MLoadqy = value;


                NotifyPropertyChanged("Loadqy");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Loadqz
        {
            get
            {
                return MLoadqz;
            }

            set
            {
                if (value < 0.01 || value > 30)
                    throw new ArgumentException("Value must be between 0.01 and 30 [kN/m]");
                MLoadqz = value;


                NotifyPropertyChanged("Loadqz");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CSBDViewModel()
        {
            Length = 5;
            Loadqy = 0.2f;
            Loadqz = 2;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
