﻿using BaseClasses;
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

            float fRoofSideLength = 0;
 
            if (model is CModel_PFD_01_MR)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame_overall - model.fH1_frame_overall) + MathF.Pow2(model.fW_frame_overall)); // Dlzka hrany strechy
            }
            else if (model is CModel_PFD_01_GR)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame_overall - model.fH1_frame_overall) + MathF.Pow2(0.5f * model.fW_frame_overall)); // Dlzka hrany strechy
            }
            else
            {
                // Exception - not implemented
                fRoofSideLength = 0;
            }

            float fRoofRidgeFlashing_TotalLength = 0;
            float fWallCornerFlashing_TotalLength = 0;
            float fBargeFlashing_TotalLength = 0;

            if (model is CModel_PFD_01_MR)
            {
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 2 * model.fH1_frame_overall + 2 * model.fH2_frame_overall;
                fBargeFlashing_TotalLength = 2 * fRoofSideLength;
            }
            else if (model is CModel_PFD_01_GR)
            {
                fRoofRidgeFlashing_TotalLength = model.fL_tot_overall;
                fWallCornerFlashing_TotalLength = 4 * model.fH1_frame_overall;
                fBargeFlashing_TotalLength = 4 * fRoofSideLength;
            }
            else
            {
                // Exception - not implemented
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 0;
                fBargeFlashing_TotalLength = 0;
            }

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
