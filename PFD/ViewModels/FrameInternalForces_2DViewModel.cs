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

        public FrameInternalForces_2DViewModel()
        {
            IFTypeIndex = 0;
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
