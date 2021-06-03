﻿using BaseClasses;
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
        private bool m_IsCrossBracedLeft;
        private bool m_IsCrossBracedRight;

        private double m_DefaultWidth;
        private int m_BaysNum;

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
                if (!IsSetFromCode)
                {
                    SetLeftDefaults();
                }
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
                if (!IsSetFromCode)
                {
                    SetRightDefaults();
                }
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
                if (Left) ValidateWidth(value);
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
                if (Right) ValidateWidth(value);
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
                if (Left) ValidatePurlinCount(value);
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
                if (Right) ValidatePurlinCount(value);
                m_PurlinCountRight = value;
                NotifyPropertyChanged("PurlinCountRight");
            }
        }

        public bool IsCrossBracedLeft
        {
            get
            {
                return m_IsCrossBracedLeft;
            }

            set
            {
                m_IsCrossBracedLeft = value;
                NotifyPropertyChanged("IsCrossBracedLeft");
            }
        }

        public bool IsCrossBracedRight
        {
            get
            {
                return m_IsCrossBracedRight;
            }

            set
            {
                m_IsCrossBracedRight = value;
                NotifyPropertyChanged("IsCrossBracedRight");
            }
        }

        public double DefaultWidth
        {
            get
            {
                return m_DefaultWidth;
            }

            set
            {
                m_DefaultWidth = value;
                if (m_DefaultWidth < 1) m_DefaultWidth = 1;
                if (m_DefaultWidth > 6) m_DefaultWidth = 6;
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
        public CanopiesOptionsViewModel(int baysNum, float width)
        {
            IsSetFromCode = true;
            m_BaysNum = baysNum;

            DefaultWidth = Math.Round(width * 0.2, 1); //20% from width

            initBays(baysNum);

            bool debug = false;

            ObservableCollection<CCanopiesInfo> items = new ObservableCollection<CCanopiesInfo>();

            for (int i = 1; i <= baysNum; i++)
            {
                CCanopiesInfo ci = new CCanopiesInfo(i, false, false, 0, 0, 0, 0, false, false, DefaultWidth);

                // TODO Ondrej - !!!!! len v debugu !!!!! Nastavit debug na true a if pre debug
                if (debug)
                {
                    // Default - docasne pridavam pre ucely vyvoja a rychlejsieho testovania
                    if (i == 1) // Left - 1st bay
                        ci = new CCanopiesInfo(i, true, false, 6, 0, 3, 0, true, false, DefaultWidth);

                    if (i == 2 || i == 3) // Right - 2nd and 3rd bay
                        ci = new CCanopiesInfo(i, false, true, 0, 3, 0, 2, false, true, DefaultWidth);
                }

                items.Add(ci);
            }

            CanopiesList = items;

            Left = false;
            Right = false;
            IsCrossBracedLeft = false;
            IsCrossBracedRight = false;

            IsSetFromCode = false;
        }

        public void Update(int baysNum, float width)
        {
            if (baysNum == m_BaysNum) return; //no change

            IsSetFromCode = true;

            DefaultWidth = Math.Round(width * 0.2, 1); //20% from width

            initBays(baysNum);

            ObservableCollection<CCanopiesInfo> items = new ObservableCollection<CCanopiesInfo>();
            for (int i = 1; i <= baysNum; i++)
            {
                CCanopiesInfo ci = CanopiesList.ElementAtOrDefault(i - 1);
                if(ci == null) ci = new CCanopiesInfo(i, false, false, 0, 0, 0, 0, false, false, DefaultWidth);

                items.Add(ci);
            }
            CanopiesList = items;
            IsSetFromCode = false;

            m_BaysNum = baysNum;
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

        private void SetLeftDefaults()
        {
            if (Left)
            {
                WidthLeft = DefaultWidth;
                PurlinCountLeft = 2;
                IsCrossBracedLeft = true;
            }
            else
            {
                WidthLeft = 0;
                PurlinCountLeft = 0;
                IsCrossBracedLeft = false;
            }
        }
        private void SetRightDefaults()
        {
            if (Right)
            {
                WidthRight = DefaultWidth;
                PurlinCountRight = 2;
                IsCrossBracedRight = true;
            }
            else
            {
                WidthRight = 0;
                PurlinCountRight = 0;
                IsCrossBracedRight = false;
            }
        }

        private void ValidateWidth(double value)
        {
            if (value <= 0)
                throw new ArgumentException("Width must be greater than 0 [m].");
        }
        private void ValidatePurlinCount(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Purlin count must be greater than 0.");
        }

        public bool HasCanopies()
        {
            if (CanopiesList == null) return false;

            foreach (CCanopiesInfo ci in CanopiesList)
            {
                if (ci.Left || ci.Right) return true;
            }

            return false;
        }

        public bool HasCanopiesCrossBracing()
        {
            if (CanopiesList == null) return false;

            foreach (CCanopiesInfo ci in CanopiesList)
            {
                if (ci.IsCrossBracedLeft || ci.IsCrossBracedRight) return true;
            }

            return false;
        }

        //todo Mato 659, prosim o kontrolu podmienok
        public bool HasCanopiesPurlin()
        {
            if (CanopiesList == null) return false;

            foreach (CCanopiesInfo ci in CanopiesList)
            {
                // Zistime ci je pocet generovanych purlins vacsi ako 0
                // Do istej miery je to zbytocna podmienka a bool, lebo vzdy musi existovat aspon jedna purlin ak chceme generovat bracing block pre purlin canopy
                if (ci.PurlinCountLeft > 0 || ci.PurlinCountRight > 0) return true;
            }

            return false;
        }
        public bool HasCanopiesMainRafter()
        {
            if (CanopiesList == null) return false;

            for (int i=0; i < CanopiesList.Count - 1; i++)
            {
                if (CanopiesList[i].Left && CanopiesList[i + 1].Left) return true;
                if (CanopiesList[i].Right && CanopiesList[i + 1].Right) return true;
            }

            return false;
        }
        public int GetMaxPurlinCount()
        {
            int purlinCount = 0;
            if (CanopiesList == null) return purlinCount;

            foreach (CCanopiesInfo ci in CanopiesList)
            {
                if (ci.Left && ci.PurlinCountLeft > purlinCount) purlinCount = ci.PurlinCountLeft;
                if (ci.Right && ci.PurlinCountRight > purlinCount) purlinCount = ci.PurlinCountRight;
            }
            return purlinCount;
        }

        public void ClearCanopies()
        {
            foreach (CCanopiesInfo ci in CanopiesList)
            {
                ci.IsSetFromCode = true;
                ci.Left = false;
                ci.WidthLeft = 0;
                ci.PurlinCountLeft = 0;
                ci.IsCrossBracedLeft = false;

                ci.Right = false;
                ci.WidthRight = 0;
                ci.PurlinCountRight = 0;
                ci.IsCrossBracedRight = false;
                
                ci.IsSetFromCode = false;
            }
        }

        public void SetViewModel(CanopiesOptionsViewModel vm)
        {
            if (vm == null) return;
            
            CanopiesList = vm.CanopiesList;
            SelectedCanopiesIndex = vm.SelectedCanopiesIndex;

            Bays = vm.Bays;
            BayFrom = vm.BayFrom;
            BayTo = vm.BayTo;
            Left = vm.Left;
            Right = vm.Right;
            WidthLeft = vm.WidthLeft;
            WidthRight = vm.WidthRight;
            PurlinCountLeft = vm.PurlinCountLeft;
            PurlinCountRight = vm.PurlinCountRight;
            IsCrossBracedLeft = vm.IsCrossBracedLeft;
            IsCrossBracedRight = vm.IsCrossBracedRight;

            DefaultWidth = vm.DefaultWidth;
        }

        public double CalculateCanopiesBargeLength()
        {
            double len = 0;
            foreach (CCanopiesInfo canopy in CanopiesList)
            {
                if (canopy.Left)
                {
                    CCanopiesInfo previousCanopy = GetPreviousNeighboringCanopyLeft(canopy);
                    if (previousCanopy == null)
                        len += canopy.WidthLeft;

                    CCanopiesInfo nextCanopy = GetNextNeighboringCanopyLeft(canopy);
                    if (nextCanopy == null)
                        len += canopy.WidthLeft;
                    else
                        len += Math.Abs(nextCanopy.WidthLeft - canopy.WidthLeft);
                }

                if (canopy.Right)
                {
                    CCanopiesInfo previousCanopy = GetPreviousNeighboringCanopyRight(canopy);
                    if (previousCanopy == null)
                        len += canopy.WidthRight;

                    CCanopiesInfo nextCanopy = GetNextNeighboringCanopyRight(canopy);
                    if (nextCanopy == null)
                        len += canopy.WidthRight;
                    else
                        len += Math.Abs(nextCanopy.WidthRight - canopy.WidthRight);
                }
            }
            return len;
        }
        private CCanopiesInfo GetPreviousNeighboringCanopyLeft(CCanopiesInfo canopy)
        {
            bool hasPreviousCanopy = ModelHelper.IsNeighboringLeftCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1));
            if (hasPreviousCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1);
            else return null;
        }
        private CCanopiesInfo GetPreviousNeighboringCanopyRight(CCanopiesInfo canopy)
        {
            bool hasPreviousCanopy = ModelHelper.IsNeighboringRightCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1));
            if (hasPreviousCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1);
            else return null;
        }
        private CCanopiesInfo GetNextNeighboringCanopyLeft(CCanopiesInfo canopy)
        {
            bool hasNextCanopy = ModelHelper.IsNeighboringLeftCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1));
            if (hasNextCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1);
            else return null;
        }
        private CCanopiesInfo GetNextNeighboringCanopyRight(CCanopiesInfo canopy)
        {
            bool hasNextCanopy = ModelHelper.IsNeighboringRightCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1));
            if (hasNextCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1);
            else return null;
        }
    }
}