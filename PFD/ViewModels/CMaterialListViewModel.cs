using BaseClasses;
using BaseClasses.Results;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PFD.ViewModels
{
    public class CMaterialListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<MaterialListMember> m_MembersMatList;
        public List<MaterialListMember> MembersMaterialList
        {
            get
            {
                return m_MembersMatList;
            }

            set
            {
                m_MembersMatList = value;
                NotifyPropertyChanged("MembersMaterialList");
            }
        }

        int iNumberOfDecimalPlacesLength = 2;
        int iNumberOfDecimalPlacesPlateDim = 3;
        int iNumberOfDecimalPlacesArea = 3;
        int iNumberOfDecimalPlacesVolume = 3;
        int iNumberOfDecimalPlacesMass = 3;
        int iNumberOfDecimalPlacesPrice = 3;

        float fCFS_PricePerKg_Members_Material = 0.3f;     // NZD / kg
        float fCFS_PricePerKg_Plates_Material = 0.4f;      // NZD / kg
        float fCFS_PricePerKg_Members_Manufacture = 0.2f;  // NZD / kg
        float fCFS_PricePerKg_Plates_Manufacture = 0.3f;   // NZD / kg
        
        //float fCFS_PricePerKg_Plates_Total = fCFS_PricePerKg_Plates_Material + fCFS_PricePerKg_Plates_Manufacture;           // NZD / kg
        float fTEK_PricePerPiece_Screws_Total = 0.05f;         // NZD / piece


        public CMaterialListViewModel(CModel_PFD model)
        {
            CDatabaseComponents databaseCopm = new CDatabaseComponents();

            List<MaterialListMember> membersMatList = new List<MaterialListMember>();

            float fCFS_PricePerKg_Members_Total = fCFS_PricePerKg_Members_Material + fCFS_PricePerKg_Members_Manufacture;        // NZD / kg

            int iLastItemIndex = 0; // Index of last row for previous cross-section

            // For each cross-section shape / size add one row
            for (int i = 0; i < model.m_arrCrSc.Length; i++)
            {
                List<CMember> assignedMembersList = model.GetListOfMembersWithCrsc(model.m_arrCrSc[i]);
                if (assignedMembersList.Count > 0) // Cross-section is assigned (to the one or more members)
                {
                    List<CMember> ListOfMemberGroups = new List<CMember>();

                    for (int j = 0; j < assignedMembersList.Count; j++) // Each member in the list
                    {
                        if (assignedMembersList[j].BIsSelectedForMaterialList)
                        {
                            // Define current member properties
                            string sPrefix = databaseCopm.arr_Member_Types_Prefix[(int)assignedMembersList[j].EMemberType, 0];
                            string sCrScName = model.m_arrCrSc[i].Name_short;
                            int iQuantity = 1;
                            string sMaterialName = model.m_arrCrSc[i].m_Mat.Name;
                            float fLength = assignedMembersList[j].FLength_real;
                            float fMassPerLength = (float)(model.m_arrCrSc[i].A_g * model.m_arrCrSc[i].m_Mat.m_fRho);
                            float fMassPerPiece = fLength * fMassPerLength;
                            float fTotalLength = iQuantity * fLength;
                            float fTotalMass = fTotalLength * fMassPerLength;
                            float fTotalPrice = fTotalMass * fCFS_PricePerKg_Members_Total;

                            bool bMemberwasAdded = false; // Member was added to the group

                            if (j > 0) // If it not first item
                            {
                                for (int k = 0; k < ListOfMemberGroups.Count; k++) // For each group of members check if current member has same prefix and same length as some already created -  // Add Member to the group or create new one
                                {
                                    if ((databaseCopm.arr_Member_Types_Prefix[(int)ListOfMemberGroups[k].EMemberType, 0] == databaseCopm.arr_Member_Types_Prefix[(int)assignedMembersList[j].EMemberType, 0]) &&
                                    (MathF.d_equal(ListOfMemberGroups[k].FLength_real, assignedMembersList[j].FLength_real)))
                                    {
                                        // Add member to the one from already created groups
                                        membersMatList[iLastItemIndex + k].Quantity += 1;
                                        membersMatList[iLastItemIndex + k].TotalLength = Math.Round(membersMatList[iLastItemIndex + k].Quantity * membersMatList[iLastItemIndex + k].Length, iNumberOfDecimalPlacesLength); // Recalculate total length of all members in the group
                                        membersMatList[iLastItemIndex + k].TotalMass = Math.Round(membersMatList[iLastItemIndex + k].TotalLength * membersMatList[iLastItemIndex + k].MassPerLength, iNumberOfDecimalPlacesMass); // Recalculate total weight of all members in the group
                                        membersMatList[iLastItemIndex + k].TotalPrice = Math.Round(membersMatList[iLastItemIndex + k].TotalMass * fCFS_PricePerKg_Members_Total, iNumberOfDecimalPlacesPrice); // Recalculate total price of all members in the group

                                        //listMemberQuantity[iLastItemIndex + k] += 1; // Add one member (piece) to the quantity
                                        //listMemberTotalLength[iLastItemIndex + k] = Math.Round(listMemberQuantity[iLastItemIndex + k] * dlistMemberLength[iLastItemIndex + k], iNumberOfDecimalPlacesLength); // Recalculate total length of all members in the group
                                        //listMemberTotalMass[iLastItemIndex + k] = Math.Round(listMemberTotalLength[iLastItemIndex + k] * dlistMemberMassPerLength[iLastItemIndex + k], iNumberOfDecimalPlacesMass); // Recalculate total weight of all members in the group
                                        //listMemberTotalPrice[iLastItemIndex + k] = Math.Round(listMemberTotalMass[iLastItemIndex + k] * fCFS_PricePerKg_Members_Total, iNumberOfDecimalPlacesPrice); // Recalculate total price of all members in the group

                                        bMemberwasAdded = true;
                                        break;
                                    }

                                }
                            }

                            if (j == 0 || !bMemberwasAdded) // Create new group (new row) (different length /prefix of member or first item in list of members assigned to the cross-section)
                            {
                                MaterialListMember mlm = new MaterialListMember();
                                mlm.Prefix = sPrefix;
                                mlm.CrScName = sCrScName;
                                mlm.Quantity = iQuantity;
                                mlm.MaterialName = sMaterialName;
                                mlm.Length = (Math.Round(fLength, iNumberOfDecimalPlacesLength));
                                mlm.MassPerLength = (Math.Round(fMassPerLength, iNumberOfDecimalPlacesMass));
                                mlm.MassPerPiece = (Math.Round(fMassPerPiece, iNumberOfDecimalPlacesMass));
                                mlm.TotalLength = (Math.Round(fTotalLength, iNumberOfDecimalPlacesLength));
                                mlm.TotalMass = (Math.Round(fTotalMass, iNumberOfDecimalPlacesMass));
                                mlm.TotalPrice = (Math.Round(fTotalPrice, iNumberOfDecimalPlacesPrice));
                                membersMatList.Add(mlm);
                                // Add first member in the group to the list of member groups
                                ListOfMemberGroups.Add(assignedMembersList[j]);
                            }
                        }
                    }

                    iLastItemIndex += ListOfMemberGroups.Count; // Index of last row for previous cross-section
                }
            }

            // Check Data
            double dTotalMembersLength_Model = 0, dTotalMembersLength_Table = 0;
            double dTotalMembersVolume_Model = 0, dTotalMembersVolume_Table = 0;
            double dTotalMembersMass_Model = 0, dTotalMembersMass_Table = 0;
            double dTotalMembersPrice_Model = 0, dTotalMembersPrice_Table = 0;
            int iTotalMembersNumber_Model = 0, iTotalMembersNumber_Table = 0;

            foreach (CMember member in model.m_arrMembers)
            {
                if (member.BIsSelectedForMaterialList)
                {
                    dTotalMembersLength_Model += member.FLength_real;
                    dTotalMembersVolume_Model += member.CrScStart.A_g * member.FLength_real;
                    dTotalMembersMass_Model += member.CrScStart.A_g * member.FLength_real * member.CrScStart.m_Mat.m_fRho;
                    dTotalMembersPrice_Model += member.CrScStart.A_g * member.FLength_real * member.CrScStart.m_Mat.m_fRho * fCFS_PricePerKg_Members_Total;
                    iTotalMembersNumber_Model += 1;
                }
            }

            for (int i = 0; i < membersMatList.Count; i++)
            {
                dTotalMembersLength_Table += membersMatList[i].Length * membersMatList[i].Quantity;
                //dTotalMembersVolume_Table += member.CrScStart.A_g * dlistMemberLength[i]; // TODO - pridat funkciu, ktora podla nazvu prierezu vrati jeho parametre
                dTotalMembersMass_Table += membersMatList[i].TotalMass;
                dTotalMembersPrice_Table += membersMatList[i].TotalPrice;
                iTotalMembersNumber_Table += membersMatList[i].Quantity;
            }

            dTotalMembersLength_Model = Math.Round(dTotalMembersLength_Model, iNumberOfDecimalPlacesLength);
            dTotalMembersLength_Table = Math.Round(dTotalMembersLength_Table, iNumberOfDecimalPlacesLength);
            dTotalMembersMass_Model = Math.Round(dTotalMembersMass_Model, iNumberOfDecimalPlacesMass);
            dTotalMembersMass_Table = Math.Round(dTotalMembersMass_Table, iNumberOfDecimalPlacesMass);

            if (!MathF.d_equal(dTotalMembersLength_Model, dTotalMembersLength_Table) ||
                !MathF.d_equal(dTotalMembersMass_Model, dTotalMembersMass_Table) ||
                (iTotalMembersNumber_Model != iTotalMembersNumber_Table)) // Error
                MessageBox.Show(
                "Total length of members in model " + dTotalMembersLength_Model + " m" + "\n" +
                "Total length of members in table " + dTotalMembersLength_Table + " m" + "\n" +
                "Total weight of members in model " + dTotalMembersMass_Model + " kg" + "\n" +
                "Total weight of members in table " + dTotalMembersMass_Table + " kg" + "\n" +
                "Total number of members in model " + iTotalMembersNumber_Model + "\n" +
                "Total number of members in table " + iTotalMembersNumber_Table + "\n");

            // Prepare output format (last row is empty)
            for (int i = 0; i < membersMatList.Count; i++)
            {
                // Change output data format
                membersMatList[i].LengthStr = membersMatList[i].Length.ToString();
                membersMatList[i].MassPerLengthStr = membersMatList[i].MassPerLength.ToString();
                membersMatList[i].MassPerPieceStr = membersMatList[i].MassPerPiece.ToString();
            }

            // Add Sum
            MaterialListMember sum = new MaterialListMember();
            sum.Prefix = "Total:";
            //sum.CrScName = "";
            sum.Quantity = iTotalMembersNumber_Table;
            //sum.MaterialName.Add("");
            //sum.Length.Add(""); // Empty cell
            //sum.MassPerLength.Add(""); // Empty cell
            //sum.MassPerPiece.Add(""); // Empty cell
            sum.TotalLength = dTotalMembersLength_Table;
            sum.TotalMass = dTotalMembersMass_Table;
            sum.TotalPrice = dTotalMembersPrice_Table;

            membersMatList.Add(sum);

            MembersMaterialList = membersMatList;
        }


        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
