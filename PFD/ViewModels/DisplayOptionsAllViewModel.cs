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
    public class DisplayOptionsAllViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private List<DisplayOptionsViewModel> m_DisplayOptionsList;

        public bool IsSetFromCode = false;

        public List<DisplayOptionsViewModel> DisplayOptionsList
        {
            get
            {
                if (m_DisplayOptionsList == null) m_DisplayOptionsList = new List<DisplayOptionsViewModel>();
                return m_DisplayOptionsList;
            }

            set
            {
                m_DisplayOptionsList = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------




        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DisplayOptionsAllViewModel()
        {
            IsSetFromCode = true;
            


            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(DisplayOptionsAllViewModel newVM)
        {
            IsSetFromCode = true;

            DisplayOptionsList = newVM.DisplayOptionsList;

            IsSetFromCode = false;
        }

        
    }
}