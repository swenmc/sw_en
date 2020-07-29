using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                m_Width = value;
                NotifyPropertyChanged("Width");
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

               
    }
}
