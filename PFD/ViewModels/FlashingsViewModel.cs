using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace PFD
{
    public class FlashingsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        private List<CAccessories_LengthItemProperties> m_Flashings;




        public bool IsSetFromCode = false;

        public List<CAccessories_LengthItemProperties> Flashings
        {
            get
            {
                return m_Flashings;
            }

            set
            {
                m_Flashings = value;
            }
        }


        

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FlashingsViewModel(CModel model)
        {            
            IsSetFromCode = false;

            float fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame - model.fH1_frame) + MathF.Pow2(0.5f * model.fW_frame)); // Dlzka hrany strechy

            float fRoofRidgeFlashing_TotalLength = model.fL_tot;
            float fWallCornerFlashing_TotalLength = 4 * model.fH1_frame;
            float fBargeFlashing_TotalLength = 4 * fRoofSideLength;

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            float fPADoorTrimmerFlashing_TotalLength = 0;
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

            Flashings = new List<CAccessories_LengthItemProperties>(); 
            //List<CoatingColour> colors = CCoatingColorManager.LoadColours("TrapezoidalSheetingSQLiteDB"); // Temporary - malo by byt nastavovane z GUI

            Flashings.Add(new CAccessories_LengthItemProperties("Roof Ridge", "Flashings", fRoofRidgeFlashing_TotalLength, 2));
            Flashings.Add(new CAccessories_LengthItemProperties("Wall Corner", "Flashings", fWallCornerFlashing_TotalLength, 2));
            Flashings.Add(new CAccessories_LengthItemProperties("Barge", "Flashings", fBargeFlashing_TotalLength, 2));
            Flashings.Add(new CAccessories_LengthItemProperties("Roller Door Trimmer", "Flashings", fRollerDoorTrimmerFlashing_TotalLength, 4));
            Flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header", "Flashings", fRollerDoorLintelFlashing_TotalLength, 4));
            Flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header Cap", "Flashings", fRollerDoorLintelCapFlashing_TotalLength, 4));
            Flashings.Add(new CAccessories_LengthItemProperties("PA Door Trimmer", "Flashings", fPADoorTrimmerFlashing_TotalLength, 18));
            Flashings.Add(new CAccessories_LengthItemProperties("PA Door Header", "Flashings", fPADoorLintelFlashing_TotalLength, 18));
            Flashings.Add(new CAccessories_LengthItemProperties("Window", "Flashings", fWindowFlashing_TotalLength, 9));

        }

        

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
