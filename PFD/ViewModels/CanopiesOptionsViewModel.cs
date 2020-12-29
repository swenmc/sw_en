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
using System.Collections.ObjectModel;

namespace PFD
{
    [Serializable]
    public class CanopiesOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
                
        ObservableCollection<CCanopiesInfo> m_CanopiesList;
        private int m_SelectedCanopiesIndex;
        
        private List<int> m_Bays;
        private int m_BayFrom;
        private int m_BayTo;
        private bool m_Left;
        private bool m_Right;
        private double m_WidthLeft;
        private double m_WidthRight;
        private int m_PurlinCountLeft;
        private int m_PurlinCountRight;


        public ObservableCollection<CCanopiesInfo> CanopiesList
        {
            get
            {
                return m_CanopiesList;
            }

            set
            {
                m_CanopiesList = value;
                foreach (CCanopiesInfo ci in CanopiesList)
                {
                    ci.PropertyChanged += canopiesItem_PropertyChanged;
                }
                NotifyPropertyChanged("CanopiesList");
            }
        }

        public int SelectedCanopiesIndex
        {
            get
            {
                return m_SelectedCanopiesIndex;
            }

            set
            {
                m_SelectedCanopiesIndex = value;
                NotifyPropertyChanged("SelectedCanopiesIndex");
            }
        }

        public List<int> Bays
        {
            get
            {
                return m_Bays;
            }

            set
            {
                m_Bays = value;
                if (m_Bays != null && m_Bays.Count > 1)
                {
                    BayFrom = m_Bays.First();
                    BayTo = m_Bays.Last();
                }
                
                NotifyPropertyChanged("Bays");
            }
        }

        public int BayFrom
        {
            get
            {
                return m_BayFrom;
            }

            set
            {
                m_BayFrom = value;
                NotifyPropertyChanged("BayFrom");
            }
        }

        public int BayTo
        {
            get
            {
                return m_BayTo;
            }

            set
            {
                m_BayTo = value;
                NotifyPropertyChanged("BayTo");
            }
        }

        public bool Left
        {
            get
            {
                return m_Left;
            }

            set
            {
                m_Left = value;
                NotifyPropertyChanged("Left");
            }
        }

        public bool Right
        {
            get
            {
                return m_Right;
            }

            set
            {
                m_Right = value;
                NotifyPropertyChanged("Right");
            }
        }
        public double WidthLeft
        {
            get
            {
                return m_WidthLeft;
            }

            set
            {
                m_WidthLeft = value;
                NotifyPropertyChanged("WidthLeft");
            }
        }
        public double WidthRight
        {
            get
            {
                return m_WidthRight;
            }

            set
            {
                m_WidthRight = value;
                NotifyPropertyChanged("WidthRight");
            }
        }

        public int PurlinCountLeft
        {
            get
            {
                return m_PurlinCountLeft;
            }

            set
            {
                m_PurlinCountLeft = value;
                NotifyPropertyChanged("PurlinCountLeft");
            }
        }
        public int PurlinCountRight
        {
            get
            {
                return m_PurlinCountRight;
            }

            set
            {
                m_PurlinCountRight = value;
                NotifyPropertyChanged("PurlinCountRight");
            }
        }



        private void canopiesItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(PropertyChanged != null) PropertyChanged(sender, e);
            //NotifyPropertyChanged("CrossBracingItem_PropertyChanged");
        }

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
       

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CanopiesOptionsViewModel(int baysNum)
        {
            IsSetFromCode = true;            

            initBays(baysNum);

            ObservableCollection<CCanopiesInfo> items = new ObservableCollection<CCanopiesInfo>();

            for (int i = 1; i <= baysNum; i++)
            {
                CCanopiesInfo ci = new CCanopiesInfo(i, false, false, 0,0,0,0);
                items.Add(ci);
            }

            CanopiesList = items;

            Left = false;
            Right = false;
            

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void initBays(int baysNum)
        {
            List<int> bays = new List<int>(baysNum);
            for (int i = 1; i <= baysNum; i++)
            {
                bays.Add(i);
            }
            Bays = bays;
        }

        
        

    }
}