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

            CreateAllViewModelsWithDefaults();

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


        private void CreateAllViewModelsWithDefaults()
        {
            m_DisplayOptionsList = new List<DisplayOptionsViewModel>();

            for (int i = 0; i <= (int)EDisplayOptionsTypes.Layouts_Foundations; i++)
            {
                m_DisplayOptionsList.Add(new DisplayOptionsViewModel());
            }

            InitViewModelsDefaults();
        }

        private void InitViewModelsDefaults()
        {
            //TODO Mato - 701
            //tu sa budu nastavovat defaulty pre vsetky displayOptions
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightAmbient = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightAmbient = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightAmbient = true;

        }

    }
}