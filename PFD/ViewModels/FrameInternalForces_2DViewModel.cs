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
                //"N", "Vz", "Vy", "T", "My", "Mz"
                if (MIFTypeIndex <= 2) MIFTypeUnit = "kN";
                else MIFTypeUnit = "kNm";
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

        public FrameInternalForces_2DViewModel()
        {
            IFTypeIndex = 4;
            InternalForceScale_user = 1;

            list_IFTypes = new List<string>() { "N", "Vz", "Vy", "T", "My", "Mz" };            
        }


        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
