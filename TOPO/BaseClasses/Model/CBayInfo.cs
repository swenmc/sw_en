using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BaseClasses
{
    [Serializable]
    public class CBayInfo : INotifyPropertyChanged
    {        
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool MIsSetFromCode;

        private int m_BayNumber;
        private int m_BayIndex;
        private float m_Width;
        private float m_Width_old;



        public bool IsSetFromCode
        {
            get
            {
                return MIsSetFromCode;
            }

            set
            {
                MIsSetFromCode = value;
                //NotifyPropertyChanged("IsSetFromCode");
            }
        }


        public int BayNumber
        {
            get
            {
                return m_BayNumber;
            }

            set
            {
                m_BayNumber = value;
                if (m_BayNumber < 1) m_BayIndex = 0;
                else m_BayIndex = m_BayNumber - 1;
                NotifyPropertyChanged("BayNumber");
            }
        }
        public int BayIndex
        {
            get
            {
                return m_BayIndex;
            }

            set
            {
                m_BayIndex = value;
            }
        }
        public float Width
        {
            get
            {
                return m_Width;
            }

            set
            {
                m_Width_old = m_Width;
                m_Width = value;

                if (m_Width <= 0)
                {
                    m_Width = m_Width_old;
                    MessageBox.Show("Wrong input value.");
                }                

                NotifyPropertyChanged("Width");
            }
        }

        public float Width_old
        {
            get
            {
                return m_Width_old;
            }

            set
            {
                m_Width_old = value;
            }
        }

        public CBayInfo(int bayNumber, float width)
        {
            MIsSetFromCode = false;

            m_BayNumber = bayNumber;
            m_BayIndex = bayNumber - 1;
            m_Width = width;            
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ValidateWidth(float maxWidth)
        {
            if (Width <= 0 || Width >= maxWidth) return false;
            else return true;
        }

        public void UndoWidth()
        {
            Width = Width_old;
        }
               
    }
}
