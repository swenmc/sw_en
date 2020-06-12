using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    [Serializable]
    public class CrossBracingProperties : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_iBayNumber;
        private int m_iBayNumber_old;

        private ObservableCollection<int> m_Bays;

        private bool m_bWallLeftSide;
        private bool m_bWallRigthSide;
        private bool m_bRoof;

        private int m_iEveryXXPurlin;

        public bool IsSetFromCode = false;

        public int iBayNumber
        {
            get
            {
                return m_iBayNumber;
            }

            set
            {
                m_iBayNumber_old = m_iBayNumber;
                m_iBayNumber = value;
                NotifyPropertyChanged("iBayNumber");
            }
        }

        public bool bWallLeftSide
        {
            get
            {
                return m_bWallLeftSide;
            }

            set
            {
                m_bWallLeftSide = value;
                NotifyPropertyChanged("bWallLeftSide");
            }
        }

        public bool bWallRightSide
        {
            get
            {
                return m_bWallRightSide;
            }

            set
            {
                m_bWallRightSide = value;
                NotifyPropertyChanged("bWallRightSide");
            }
        }

        public bool bRoof
        {
            get
            {
                return m_bRoof;
            }

            set
            {
                m_bRoof = value;
                NotifyPropertyChanged("bRoof");
            }
        }

        public int iEveryXXPurlin
        {
            get
            {
                return m_iEveryXXPurlin;
            }

            set
            {
                m_iEveryXXPurlin = value;
                NotifyPropertyChanged("iEveryXXPurlin");
            }
        }

        public ObservableCollection<int> Bays
        {
            get
            {
                if (m_Bays == null) m_Bays = new ObservableCollection<int>();
                return m_Bays;
            }

            set
            {
                m_Bays = value;
                if(m_Bays != null) NotifyPropertyChanged("Bays");
            }
        }

        public int iBayNumber_old
        {
            get
            {
                return m_iBayNumber_old;
            }

            set
            {
                m_iBayNumber_old = value;
            }
        }

        public CrossBracingProperties()
        {
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Validate()
        {
            bool isValid = true;

            // TODO doplnit

            return isValid;
        }

        public bool ValidateBays()
        {
            if (iBayNumber <= Bays.Count) return true;
            else return false;
        }
    }
}
