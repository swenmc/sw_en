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
    public class CFootingInputVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        private Dictionary<string, CMatPropertiesRC> m_ConcreteGrades;
        private Dictionary<string, CMatPropertiesRF> m_ReinforcementGrades;
        private Dictionary<int, CReinforcementBarProperties> m_ReinforcementBars;
        private Dictionary<string, CMeshProperties> m_ReinforcementMeshGrades;

        private List<string> m_ConcreteGradesList;
        private List<string> m_AggregateSizesList;
        private List<string> m_ReinforcementGradesList;
        private List<string> m_ReinforcementBarsList;
        private List<string> m_ReinforcementBarsCountList;
        private List<string> m_ReinforcementMeshGradesList;

        private List<CComboColor> m_ColorList;

        private int m_FootingPadMemberTypeIndex;
        private string m_ConcreteGrade;
        private string m_AggregateSize;
        private float m_ConcreteDensity;
        private string m_ReinforcementGrade;
        private string m_ReinforcementMeshGrade;

        private bool m_UseStraightReinforcementBars;

        private string m_LongReinTop_x_No;
        private string m_LongReinTop_x_Phi;
        private float m_LongReinTop_x_distance_s_y;
        private int m_LongReinTop_x_ColorIndex;
        public Color LongReinTop_x_Color;

        private string m_LongReinTop_y_No;
        private string m_LongReinTop_y_Phi;
        private float m_LongReinTop_y_distance_s_x;
        private int m_LongReinTop_y_ColorIndex;
        public Color LongReinTop_y_Color;

        private string m_LongReinBottom_x_No;
        private string m_LongReinBottom_x_Phi;
        private float m_LongReinBottom_x_distance_s_y;
        private int m_LongReinBottom_x_ColorIndex;
        public Color LongReinBottom_x_Color;

        private string m_LongReinBottom_y_No;
        private string m_LongReinBottom_y_Phi;
        private float m_LongReinBottom_y_distance_s_x;
        private int m_LongReinBottom_y_ColorIndex;
        public Color LongReinBottom_y_Color;

        private float m_FootingPadSize_x_Or_a;
        private float m_FootingPadSize_y_Or_b;
        private float m_FootingPadSize_z_Or_h;

        private float m_Eccentricity_ex;
        private float m_Eccentricity_ey;

        private float m_SoilReductionFactor_Phi;
        private float m_SoilReductionFactorEQ_Phi;

        private float m_SoilBearingCapacity;
        private float m_ConcreteCover;

        // Floor
        private float m_FloorSlabThickness;
        private float m_MeshConcreteCover;

        private int m_NumberOfSawCutsInDirectionX;
        private int m_NumberOfSawCutsInDirectionY;
        private float m_FirstSawCutPositionInDirectionX;
        private float m_FirstSawCutPositionInDirectionY;
        private float m_SawCutsSpacingInDirectionX;
        private float m_SawCutsSpacingInDirectionY;
        private float m_CutWidth;
        private float m_CutDepth;

        private int m_NumberOfControlJointsInDirectionX;
        private int m_NumberOfControlJointsInDirectionY;
        private float m_FirstControlJointPositionInDirectionX;
        private float m_FirstControlJointPositionInDirectionY;
        private float m_ControlJointsSpacingInDirectionX;
        private float m_ControlJointsSpacingInDirectionY;
        private string m_DowelDiameter; // TODO - zapracovat databazu dowels a vyberat konkretny kolik z databazy
        private float m_DowelLength;
        private float m_DowelSpacing;

        private bool m_IsEnabledFirstSawCutPositionInDirectionX;
        private bool m_IsEnabledFirstSawCutPositionInDirectionY;
        private bool m_IsEnabledSawCutsSpacingInDirectionX;
        private bool m_IsEnabledSawCutsSpacingInDirectionY;

        private bool m_IsEnabledFirstControlJointPositionInDirectionX;
        private bool m_IsEnabledFirstControlJointPositionInDirectionY;
        private bool m_IsEnabledControlJointsSpacingInDirectionX;
        private bool m_IsEnabledControlJointsSpacingInDirectionY;

        private float m_PerimeterDepth_LRSide;
        private float m_PerimeterWidth_LRSide;
        private float m_StartersLapLength_LRSide;
        private float m_StartersSpacing_LRSide;
        private string m_Starters_Phi_LRSide;
        private string m_Longitud_Reinf_TopAndBotom_Phi_LRSide;
        private string m_Longitud_Reinf_Intermediate_Phi_LRSide;
        private int m_Longitud_Reinf_Intermediate_Count_LRSide;

        private float m_RebateWidth_LRSide;

        private float m_PerimeterDepth_FBSide;
        private float m_PerimeterWidth_FBSide;
        private float m_StartersLapLength_FBSide;
        private float m_StartersSpacing_FBSide;
        private string m_Starters_Phi_FBSide;
        private string m_Longitud_Reinf_TopAndBotom_Phi_FBSide;
        private string m_Longitud_Reinf_Intermediate_Phi_FBSide;
        private int m_Longitud_Reinf_Intermediate_Count_FBSide;

        private float m_RebateWidth_FBSide;

        private List<CFoundation> listOfSelectedTypePads;
        private Dictionary<string, Tuple<CFoundation, CConnectionJointTypes>> m_DictFootings;

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        public Dictionary<string, CMatPropertiesRC> ConcreteGrades
        {
            get
            {
                return m_ConcreteGrades;
            }

            set
            {
                m_ConcreteGrades = value;
                NotifyPropertyChanged("ConcreteGrades");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public Dictionary<string, CMatPropertiesRF> ReinforcementGrades
        {
            get
            {
                return m_ReinforcementGrades;
            }

            set
            {
                m_ReinforcementGrades = value;
                NotifyPropertyChanged("ReinforcementGrades");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public Dictionary<string, CMeshProperties> ReinforcementMeshGrades
        {
            get
            {
                return m_ReinforcementMeshGrades;
            }

            set
            {
                m_ReinforcementMeshGrades = value;
                NotifyPropertyChanged("ReinforcementMeshGrades");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public Dictionary<int, CReinforcementBarProperties> ReinforcementBars
        {
            get
            {
                return m_ReinforcementBars;
            }

            set
            {
                m_ReinforcementBars = value;
                NotifyPropertyChanged("ReinforcementBars");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<string> ConcreteGradesList
        {
            get
            {
                return m_ConcreteGradesList;
            }

            set
            {
                m_ConcreteGradesList = value;
                NotifyPropertyChanged("ConcreteGradesList");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<string> AggregateSizesList
        {
            get
            {
                return m_AggregateSizesList;
            }

            set
            {
                m_AggregateSizesList = value;
                NotifyPropertyChanged("AggregateSizesList");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<string> ReinforcementGradesList
        {
            get
            {
                return m_ReinforcementGradesList;
            }

            set
            {
                m_ReinforcementGradesList = value;
                NotifyPropertyChanged("ReinforcementGradesList");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<string> ReinforcementMeshGradesList
        {
            get
            {
                return m_ReinforcementMeshGradesList;
            }

            set
            {
                m_ReinforcementMeshGradesList = value;
                NotifyPropertyChanged("ReinforcementMeshGradesList");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<string> ReinforcementBarsList
        {
            get
            {
                return m_ReinforcementBarsList;
            }

            set
            {
                m_ReinforcementBarsList = value;
                NotifyPropertyChanged("ReinforcementBarsList");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<string> ReinforcementBarsCountList
        {
            get
            {
                return m_ReinforcementBarsCountList;
            }

            set
            {
                m_ReinforcementBarsCountList = value;
                NotifyPropertyChanged("ReinforcementBarsCountList");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<CComboColor> ColorList
        {
            get
            {
                return m_ColorList;
            }

            set
            {
                m_ColorList = value;
                NotifyPropertyChanged("ComboboxColors");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int FootingPadMemberTypeIndex
        {
            get
            {
                return m_FootingPadMemberTypeIndex;
            }

            set
            {
                m_FootingPadMemberTypeIndex = value;

                IsSetFromCode = true;
                UpdateValuesInGUIFromSelectedFootingPad();
                IsSetFromCode = false;

                NotifyPropertyChanged("FootingPadMemberTypeIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string ConcreteGrade
        {
            get
            {
                return m_ConcreteGrade;
            }

            set
            {
                m_ConcreteGrade = value;
                NotifyPropertyChanged("ConcreteGrade");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string AggregateSize
        {
            get
            {
                return m_AggregateSize;
            }

            set
            {
                m_AggregateSize = value;
                NotifyPropertyChanged("AggregateSize");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ConcreteDensity
        {
            get
            {
                return m_ConcreteDensity;
            }

            set
            {
                if (value < 1800 || value > 2800)
                    throw new ArgumentException("Concrete density must be between 1800 and 2800 [kg/m³]");

                m_ConcreteDensity = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                NotifyPropertyChanged("ConcreteDensity");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string ReinforcementGrade
        {
            get
            {
                return m_ReinforcementGrade;
            }

            set
            {
                m_ReinforcementGrade = value;
                NotifyPropertyChanged("ReinforcementGrade");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string ReinforcementMeshGrade
        {
            get
            {
                return m_ReinforcementMeshGrade;
            }

            set
            {
                m_ReinforcementMeshGrade = value;
                //_model.m_arrSlabs.First().MeshGradeName = m_ReinforcementMeshGrade;
                NotifyPropertyChanged("ReinforcementMeshGrade");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool UseStraightReinforcementBars
        {
            get
            {
                return m_UseStraightReinforcementBars;
            }

            set
            {
                m_UseStraightReinforcementBars = value;
                _pfdVM.RecreateFoundations = true;

                //if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("UseStraightReinforcementBars");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinTop_x_No
        {
            get
            {
                return m_LongReinTop_x_No;
            }

            set
            {
                m_LongReinTop_x_No = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    if (m_LongReinTop_x_No == "None")
                        pad.Count_Top_Bars_x = 0;
                    else
                        pad.Count_Top_Bars_x = Int32.Parse(LongReinTop_x_No);
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinTop_x_No");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinTop_x_Phi
        {
            get
            {
                return m_LongReinTop_x_Phi;
            }

            set
            {
                m_LongReinTop_x_Phi = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    // Dim 1 je polomer valca
                    pad.Reference_Top_Bar_x.Diameter = float.Parse(LongReinTop_x_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinTop_x_Phi");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LongReinTop_x_distance_s_y
        {
            get
            {
                return m_LongReinTop_x_distance_s_y;
            }

            set
            {
                m_LongReinTop_x_distance_s_y = value;

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI(); // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika

                NotifyPropertyChanged("LongReinTop_x_distance_s_y");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int LongReinTop_x_ColorIndex
        {
            get
            {
                return m_LongReinTop_x_ColorIndex;
            }

            set
            {
                m_LongReinTop_x_ColorIndex = value;

                List<CComboColor> listOfMediaColours = CComboBoxHelper.ColorList;

                LongReinTop_x_Color = listOfMediaColours[m_LongReinTop_x_ColorIndex].Color.Value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.Reference_Top_Bar_x.ColorBar = LongReinTop_x_Color;
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinTop_x_ColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinTop_y_No
        {
            get
            {
                return m_LongReinTop_y_No;
            }

            set
            {
                m_LongReinTop_y_No = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    if (m_LongReinTop_y_No == "None")
                        pad.Count_Top_Bars_y = 0;
                    else
                        pad.Count_Top_Bars_y = Int32.Parse(LongReinTop_y_No);
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinTop_y_No");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinTop_y_Phi
        {
            get
            {
                return m_LongReinTop_y_Phi;
            }

            set
            {
                m_LongReinTop_y_Phi = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    // Dim 1 je polomer valca
                    pad.Reference_Top_Bar_y.Diameter = float.Parse(LongReinTop_y_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinTop_y_Phi");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LongReinTop_y_distance_s_x
        {
            get
            {
                return m_LongReinTop_y_distance_s_x;
            }

            set
            {
                m_LongReinTop_y_distance_s_x = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI(); // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika
                NotifyPropertyChanged("LongReinTop_y_distance_s_x");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int LongReinTop_y_ColorIndex
        {
            get
            {
                return m_LongReinTop_y_ColorIndex;
            }

            set
            {
                m_LongReinTop_y_ColorIndex = value;

                List<CComboColor> listOfMediaColours = CComboBoxHelper.ColorList;

                LongReinTop_y_Color = listOfMediaColours[m_LongReinTop_y_ColorIndex].Color.Value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.Reference_Top_Bar_y.ColorBar = LongReinTop_y_Color;
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinTop_y_ColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinBottom_x_No
        {
            get
            {
                return m_LongReinBottom_x_No;
            }

            set
            {
                m_LongReinBottom_x_No = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    if (m_LongReinBottom_x_No == "None")
                        pad.Count_Bottom_Bars_x = 0;
                    else
                        pad.Count_Bottom_Bars_x = Int32.Parse(LongReinBottom_x_No);
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinBottom_x_No");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinBottom_x_Phi
        {
            get
            {
                return m_LongReinBottom_x_Phi;
            }

            set
            {
                m_LongReinBottom_x_Phi = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    // Dim 1 je polomer valca
                    pad.Reference_Bottom_Bar_x.Diameter = float.Parse(LongReinBottom_x_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinBottom_x_Phi");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LongReinBottom_x_distance_s_y
        {
            get
            {
                return m_LongReinBottom_x_distance_s_y;
            }

            set
            {
                m_LongReinBottom_x_distance_s_y = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();  // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika
                NotifyPropertyChanged("LongReinBottom_x_distance_s_y");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int LongReinBottom_x_ColorIndex
        {
            get
            {
                return m_LongReinBottom_x_ColorIndex;
            }

            set
            {
                m_LongReinBottom_x_ColorIndex = value;

                List<CComboColor> listOfMediaColours = CComboBoxHelper.ColorList;

                LongReinBottom_x_Color = listOfMediaColours[m_LongReinBottom_x_ColorIndex].Color.Value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.Reference_Bottom_Bar_x.ColorBar = LongReinBottom_x_Color;
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinBottom_x_ColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinBottom_y_No
        {
            get
            {
                return m_LongReinBottom_y_No;
            }

            set
            {
                m_LongReinBottom_y_No = value;


                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    if (m_LongReinBottom_y_No == "None")
                        pad.Count_Bottom_Bars_y = 0;
                    else
                        pad.Count_Bottom_Bars_y = Int32.Parse(LongReinBottom_y_No);
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinBottom_y_No");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string LongReinBottom_y_Phi
        {
            get
            {
                return m_LongReinBottom_y_Phi;
            }

            set
            {
                m_LongReinBottom_y_Phi = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    // Dim 1 je polomer valca
                    pad.Reference_Bottom_Bar_y.Diameter = float.Parse(LongReinBottom_y_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinBottom_y_Phi");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float LongReinBottom_y_distance_s_x
        {
            get
            {
                return m_LongReinBottom_y_distance_s_x;
            }

            set
            {
                m_LongReinBottom_y_distance_s_x = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();  // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika
                NotifyPropertyChanged("LongReinBottom_y_distance_s_x");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int LongReinBottom_y_ColorIndex
        {
            get
            {
                return m_LongReinBottom_y_ColorIndex;
            }

            set
            {
                m_LongReinBottom_y_ColorIndex = value;

                List<CComboColor> listOfMediaColours = CComboBoxHelper.ColorList;

                LongReinBottom_y_Color = listOfMediaColours[m_LongReinBottom_y_ColorIndex].Color.Value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.Reference_Bottom_Bar_y.ColorBar = LongReinBottom_y_Color;
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("LongReinBottom_y_ColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FootingPadSize_x_Or_a
        {
            get
            {
                return m_FootingPadSize_x_Or_a;
            }

            set
            {
                if (value < 0.4f || value > 5f)
                    throw new ArgumentException("Footing pad size must be between 0.4 and 5 [m]");

                m_FootingPadSize_x_Or_a = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.m_fDim1 = FootingPadSize_x_Or_a;
                    pad.SetControlPoint();
                }

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                NotifyPropertyChanged("FootingPadSize_x_Or_a");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FootingPadSize_y_Or_b
        {
            get
            {
                return m_FootingPadSize_y_Or_b;
            }

            set
            {
                if (value < 0.4f || value > 5f)
                    throw new ArgumentException("Footing pad size must be between 0.4 and 5 [m]");

                m_FootingPadSize_y_Or_b = value;
                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.m_fDim2 = FootingPadSize_y_Or_b;
                    pad.SetControlPoint();
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                NotifyPropertyChanged("FootingPadSize_y_Or_b");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FootingPadSize_z_Or_h
        {
            get
            {
                return m_FootingPadSize_z_Or_h;
            }

            set
            {
                if (value < 0.45f || value > 2f)
                    throw new ArgumentException("AS 2870 - Footing pad size must be between 0.45 and 2 [m]"); // TODO napojit na tabulku normy

                m_FootingPadSize_z_Or_h = value;
                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.m_fDim3 = FootingPadSize_z_Or_h;
                    pad.SetControlPoint();
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                NotifyPropertyChanged("FootingPadSize_z_Or_h");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Eccentricity_ex
        {
            get
            {
                return m_Eccentricity_ex;
            }

            set
            {
                if (value < -0.5f * FootingPadSize_x_Or_a || value > 0.5f * FootingPadSize_x_Or_a)
                    throw new ArgumentException("Eccentricity must be between -x/2 = " + string.Format("{0:0.000}", -0.5f * FootingPadSize_x_Or_a) +
                                                                          "and x/2 = " + string.Format("{0:0.000}", 0.5f * FootingPadSize_x_Or_a) + " [m]");

                m_Eccentricity_ex = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                NotifyPropertyChanged("Eccentricity_ex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Eccentricity_ey
        {
            get
            {
                return m_Eccentricity_ey;
            }

            set
            {
                if (value < -0.5f * FootingPadSize_y_Or_b || value > 0.5f * FootingPadSize_y_Or_b)
                    throw new ArgumentException("Eccentricity must be between -y/2= " + string.Format("{0:0.000}", -0.5f * FootingPadSize_y_Or_b) +
                                                                          "and y/2= " + string.Format("{0:0.000}", 0.5f * FootingPadSize_y_Or_b) + "[m]");

                m_Eccentricity_ey = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                NotifyPropertyChanged("Eccentricity_ey");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SoilReductionFactor_Phi
        {
            get
            {
                return m_SoilReductionFactor_Phi;
            }

            set
            {
                if (value < 0.3f || value > 1f)
                    throw new ArgumentException("Soil reduction factor must be between 0.3 and 1 [-]");

                m_SoilReductionFactor_Phi = value;
                NotifyPropertyChanged("SoilReductionFactor_Phi");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SoilReductionFactorEQ_Phi
        {
            get
            {
                return m_SoilReductionFactorEQ_Phi;
            }

            set
            {
                if (value < 0.3f || value > 1f)
                    throw new ArgumentException("Soil reduction factor must be between 0.3 and 1 [-]");

                m_SoilReductionFactorEQ_Phi = value;
                NotifyPropertyChanged("SoilReductionFactorEQ_Phi");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SoilBearingCapacity
        {
            get
            {
                return m_SoilBearingCapacity;
            }

            set
            {
                if (value < 30f || value > 800f)
                    throw new ArgumentException("Soil bearing capacity must be between 30 and 800 [kPa]");

                m_SoilBearingCapacity = value;
                NotifyPropertyChanged("SoilBearingCapacity");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ConcreteCover
        {
            get
            {
                return m_ConcreteCover;
            }

            set
            {
                if (value < 10f || value > 200f)
                    throw new ArgumentException("Concrete cover must be between 10 and 200 [mm]");

                m_ConcreteCover = value;

                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.ConcreteCover = ConcreteCover / 1000f;
                }
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();

                NotifyPropertyChanged("ConcreteCover");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float MeshConcreteCover
        {
            get
            {
                return m_MeshConcreteCover;
            }

            set
            {
                if (value < 10f || value > 0.5 * m_FloorSlabThickness)
                    throw new ArgumentException("Concrete cover must be between 10 [mm] and 50% of slab thickness");

                m_MeshConcreteCover = value;

                //if (IsSetFromCode == false) UpdateValuesInGUI();
                //_model.m_arrSlabs.First().ConcreteCover = m_MeshConcreteCover / 1000;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("MeshConcreteCover");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FloorSlabThickness
        {
            get
            {
                return m_FloorSlabThickness;
            }

            set
            {
                if (value < 50f || value > 500f)
                    throw new ArgumentException("Floor slab thickness must be between 50 and 500 [mm]");

                m_FloorSlabThickness = value;
                //_model.m_arrSlabs.First().m_fDim3 = m_FloorSlabThickness / 1000;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("FloorSlabThickness");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int NumberOfSawCutsInDirectionX
        {
            get
            {
                return m_NumberOfSawCutsInDirectionX;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Number of saw cuts must be between 0 and 50 [-]");

                m_NumberOfSawCutsInDirectionX = value;

                if(m_NumberOfSawCutsInDirectionX <= 0)
                    m_FirstSawCutPositionInDirectionX = 0;

                if (m_NumberOfSawCutsInDirectionX <= 1)
                    m_SawCutsSpacingInDirectionX = 0;

                SetIsEnabledFirstSawCutPositionInDirectionX();
                //SetIsEnabledSawCutsSpacingInDirectionX();
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("NumberOfSawCutsInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int NumberOfSawCutsInDirectionY
        {
            get
            {
                return m_NumberOfSawCutsInDirectionY;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Number of saw cuts must be between 0 and 50 [-]");

                m_NumberOfSawCutsInDirectionY = value;

                if (m_NumberOfSawCutsInDirectionY <= 0)
                    m_FirstSawCutPositionInDirectionY = 0;

                if (m_NumberOfSawCutsInDirectionY <= 1)
                    m_SawCutsSpacingInDirectionY = 0;

                SetIsEnabledFirstSawCutPositionInDirectionY();
                //SetIsEnabledSawCutsSpacingInDirectionY();
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("NumberOfSawCutsInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FirstSawCutPositionInDirectionX
        {
            get
            {
                return m_FirstSawCutPositionInDirectionX;
            }

            set
            {
                if (value < 0f || value > 10)
                    throw new ArgumentException("Position of saw cut must be between 0.2 and 10 [m]");

                m_FirstSawCutPositionInDirectionX = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("FirstSawCutPositionInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FirstSawCutPositionInDirectionY
        {
            get
            {
                return m_FirstSawCutPositionInDirectionY;
            }

            set
            {
                if (value < 0.0f || value > 10)
                    throw new ArgumentException("Position of saw cut must be between 0.0 and 10 [m]");

                m_FirstSawCutPositionInDirectionY = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("FirstSawCutPositionInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SawCutsSpacingInDirectionX
        {
            get
            {
                return m_SawCutsSpacingInDirectionX;
            }

            set
            {
                if (value < 0f || value > 10)
                    throw new ArgumentException("Spacing of saw cuts must be between 0 and 10 [m]");

                m_SawCutsSpacingInDirectionX = value;

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("SawCutsSpacingInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float SawCutsSpacingInDirectionY
        {
            get
            {
                return m_SawCutsSpacingInDirectionY;
            }

            set
            {
                if (value < 0f || value > 10)
                    throw new ArgumentException("Spacing of saw cuts must be between 0 and 10 [m]");

                m_SawCutsSpacingInDirectionY = value;

                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("SawCutsSpacingInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float CutWidth
        {
            get
            {
                return m_CutWidth;
            }
            set
            {
                m_CutWidth = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("CutWidth");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float CutDepth
        {
            get
            {
                return m_CutDepth;
            }
            set
            {
                m_CutDepth = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("CutDepth");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int NumberOfControlJointsInDirectionX
        {
            get
            {
                return m_NumberOfControlJointsInDirectionX;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Number of control joints must be between 0 and 50 [-]");

                m_NumberOfControlJointsInDirectionX = value;

                if (m_NumberOfControlJointsInDirectionX <= 0)
                    m_FirstControlJointPositionInDirectionX = 0;

                if (m_NumberOfControlJointsInDirectionX <= 1)
                    m_ControlJointsSpacingInDirectionX = 0;

                SetIsEnabledFirstControlJointPositionInDirectionX();
                //SetIsEnabledControlJointsSpacingInDirectionX();
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("NumberOfControlJointsInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int NumberOfControlJointsInDirectionY
        {
            get
            {
                return m_NumberOfControlJointsInDirectionY;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Number of control joints must be between 0 and 50 [-]");

                m_NumberOfControlJointsInDirectionY = value;


                if (m_NumberOfControlJointsInDirectionY <= 0)
                    m_FirstControlJointPositionInDirectionY = 0;

                if (m_NumberOfControlJointsInDirectionY <= 1)
                    m_ControlJointsSpacingInDirectionY = 0;

                SetIsEnabledFirstControlJointPositionInDirectionY();
                //SetIsEnabledControlJointsSpacingInDirectionY();
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("NumberOfControlJointsInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FirstControlJointPositionInDirectionX
        {
            get
            {
                return m_FirstControlJointPositionInDirectionX;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Position of control joint must be between 0 and 50 [m]");

                m_FirstControlJointPositionInDirectionX = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("FirstControlJointPositionInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FirstControlJointPositionInDirectionY
        {
            get
            {
                return m_FirstControlJointPositionInDirectionY;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Position of control joint must be between 0 and 50 [m]");

                m_FirstControlJointPositionInDirectionY = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("FirstControlJointPositionInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ControlJointsSpacingInDirectionX
        {
            get
            {
                return m_ControlJointsSpacingInDirectionX;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Spacing of control joints must be between 0 and 50 [m]");

                m_ControlJointsSpacingInDirectionX = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("ControlJointsSpacingInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ControlJointsSpacingInDirectionY
        {
            get
            {
                return m_ControlJointsSpacingInDirectionY;
            }

            set
            {
                if (value < 0f || value > 50)
                    throw new ArgumentException("Spacing of saw control joints must be between 0 and 50 [m]");

                m_ControlJointsSpacingInDirectionY = value;
                if (IsSetFromCode == false) UpdateSelectedFootingPadsValuesFromGUI();
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("ControlJointsSpacingInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string DowelDiameter
        {
            get
            {
                return m_DowelDiameter;
            }
            set
            {
                m_DowelDiameter = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("DowelDiameter");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DowelLength
        {
            get
            {
                return m_DowelLength;
            }
            set
            {
                m_DowelLength = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("DowelLength");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float DowelSpacing
        {
            get
            {
                return m_DowelSpacing;
            }
            set
            {
                m_DowelSpacing = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("DowelSpacing");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledFirstSawCutPositionInDirectionX
        {
            get
            {
                return m_IsEnabledFirstSawCutPositionInDirectionX;
            }

            set
            {
                m_IsEnabledFirstSawCutPositionInDirectionX = value;
                NotifyPropertyChanged("IsEnabledFirstSawCutPositionInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledFirstSawCutPositionInDirectionY
        {
            get
            {
                return m_IsEnabledFirstSawCutPositionInDirectionY;
            }

            set
            {
                m_IsEnabledFirstSawCutPositionInDirectionY = value;
                NotifyPropertyChanged("IsEnabledFirstSawCutPositionInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledSawCutsSpacingInDirectionX
        {
            get
            {
                return m_IsEnabledSawCutsSpacingInDirectionX;
            }

            set
            {
                m_IsEnabledSawCutsSpacingInDirectionX = value;
                NotifyPropertyChanged("IsEnabledSawCutsSpacingInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledSawCutsSpacingInDirectionY
        {
            get
            {
                return m_IsEnabledSawCutsSpacingInDirectionY;
            }

            set
            {
                m_IsEnabledSawCutsSpacingInDirectionY = value;
                NotifyPropertyChanged("IsEnabledSawCutsSpacingInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledFirstControlJointPositionInDirectionX
        {
            get
            {
                return m_IsEnabledFirstControlJointPositionInDirectionX;
            }

            set
            {
                m_IsEnabledFirstControlJointPositionInDirectionX = value;
                NotifyPropertyChanged("IsEnabledFirstControlJointPositionInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledFirstControlJointPositionInDirectionY
        {
            get
            {
                return m_IsEnabledFirstControlJointPositionInDirectionY;
            }

            set
            {
                m_IsEnabledFirstControlJointPositionInDirectionY = value;
                NotifyPropertyChanged("IsEnabledFirstControlJointPositionInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledControlJointsSpacingInDirectionX
        {
            get
            {
                return m_IsEnabledControlJointsSpacingInDirectionX;
            }

            set
            {
                m_IsEnabledControlJointsSpacingInDirectionX = value;
                NotifyPropertyChanged("IsEnabledControlJointsSpacingInDirectionX");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public bool IsEnabledControlJointsSpacingInDirectionY
        {
            get
            {
                return m_IsEnabledControlJointsSpacingInDirectionY;
            }

            set
            {
                m_IsEnabledControlJointsSpacingInDirectionY = value;
                NotifyPropertyChanged("IsEnabledControlJointsSpacingInDirectionY");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterDepth_LRSide
        {
            get
            {
                return m_PerimeterDepth_LRSide;
            }

            set
            {
                if (value < 450 || value > 1000)
                    throw new ArgumentException("AS 2870 - Perimeter depth must be between 450 and 1000 [mm]"); // TODO napojit na tabulku normy

                m_PerimeterDepth_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("PerimeterDepth_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterWidth_LRSide
        {
            get
            {
                return m_PerimeterWidth_LRSide;
            }

            set
            {
                m_PerimeterWidth_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("PerimeterWidth_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersLapLength_LRSide
        {
            get
            {
                return m_StartersLapLength_LRSide;
            }

            set
            {
                m_StartersLapLength_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("StartersLapLength_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersSpacing_LRSide
        {
            get
            {
                return m_StartersSpacing_LRSide;
            }

            set
            {
                m_StartersSpacing_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("StartersSpacing_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string Starters_Phi_LRSide
        {
            get
            {
                return m_Starters_Phi_LRSide;
            }

            set
            {
                m_Starters_Phi_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Starters_Phi_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateWidth_LRSide
        {
            get
            {
                return m_RebateWidth_LRSide;
            }

            set
            {
                m_RebateWidth_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("RebateWidth_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string Longitud_Reinf_TopAndBotom_Phi_LRSide
        {
            get
            {
                return m_Longitud_Reinf_TopAndBotom_Phi_LRSide;
            }

            set
            {
                m_Longitud_Reinf_TopAndBotom_Phi_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Longitud_Reinf_TopAndBotom_Phi_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string Longitud_Reinf_Intermediate_Phi_LRSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Phi_LRSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Phi_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Longitud_Reinf_Intermediate_Phi_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int Longitud_Reinf_Intermediate_Count_LRSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Count_LRSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Count_LRSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Longitud_Reinf_Intermediate_Count_LRSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterDepth_FBSide
        {
            get
            {
                return m_PerimeterDepth_FBSide;
            }

            set
            {
                if (value < 450 || value > 1000)
                    throw new ArgumentException("AS 2870 - Perimeter depth must be between 450 and 1000 [mm]"); // TODO napojit na tabulku normy

                m_PerimeterDepth_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("PerimeterDepth_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterWidth_FBSide
        {
            get
            {
                return m_PerimeterWidth_FBSide;
            }

            set
            {
                m_PerimeterWidth_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("PerimeterWidth_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersLapLength_FBSide
        {
            get
            {
                return m_StartersLapLength_FBSide;
            }

            set
            {
                m_StartersLapLength_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("StartersLapLength_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersSpacing_FBSide
        {
            get
            {
                return m_StartersSpacing_FBSide;
            }

            set
            {
                m_StartersSpacing_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("StartersSpacing_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string Starters_Phi_FBSide
        {
            get
            {
                return m_Starters_Phi_FBSide;
            }

            set
            {
                m_Starters_Phi_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Starters_Phi_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateWidth_FBSide
        {
            get
            {
                return m_RebateWidth_FBSide;
            }

            set
            {
                m_RebateWidth_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("RebateWidth_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string Longitud_Reinf_TopAndBotom_Phi_FBSide
        {
            get
            {
                return m_Longitud_Reinf_TopAndBotom_Phi_FBSide;
            }

            set
            {
                m_Longitud_Reinf_TopAndBotom_Phi_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Longitud_Reinf_TopAndBotom_Phi_FBSide");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public string Longitud_Reinf_Intermediate_Phi_FBSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Phi_FBSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Phi_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Longitud_Reinf_Intermediate_Phi_FBSide");
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        public int Longitud_Reinf_Intermediate_Count_FBSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Count_FBSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Count_FBSide = value;
                //UpdateFloorSlabModelFromGUI();
                NotifyPropertyChanged("Longitud_Reinf_Intermediate_Count_FBSide");
            }
        }

        public Dictionary<string, Tuple<CFoundation, CConnectionJointTypes>> DictFootings
        {
            get
            {
                if (m_DictFootings == null)
                {
                    m_DictFootings = new Dictionary<string, Tuple<CFoundation, CConnectionJointTypes>>();
                    CFoundation pad = GetFootingPad(EMemberType_FS_Position.MainColumn);
                    CConnectionJointTypes joint = GetBaseJointForSelectedNode(pad.m_Node);
                    m_DictFootings.Add("Main Column", Tuple.Create<CFoundation, CConnectionJointTypes>(pad, joint));

                    pad = GetFootingPad(EMemberType_FS_Position.EdgeColumn);
                    joint = GetBaseJointForSelectedNode(pad.m_Node);
                    m_DictFootings.Add("Edge Column", Tuple.Create<CFoundation, CConnectionJointTypes>(pad, joint));

                    pad = GetFootingPad(EMemberType_FS_Position.ColumnFrontSide);
                    joint = GetBaseJointForSelectedNode(pad.m_Node);
                    m_DictFootings.Add("Wind Post - Front", Tuple.Create<CFoundation, CConnectionJointTypes>(pad, joint));

                    pad = GetFootingPad(EMemberType_FS_Position.ColumnBackSide);
                    joint = GetBaseJointForSelectedNode(pad.m_Node);
                    m_DictFootings.Add("Wind Post - Back", Tuple.Create<CFoundation, CConnectionJointTypes>(pad, joint));
                }

                return m_DictFootings;
            }
        }

        CPFDViewModel _pfdVM;
        //CModel_PFD_01_GR _model;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CFootingInputVM(CPFDViewModel pfdVM)
        {
            IsSetFromCode = true;
            _pfdVM = pfdVM;
            //_model = pfdVM.Model as CModel_PFD_01_GR;
            // Fill dictionaries
            ConcreteGrades = CMaterialManager.LoadMaterialPropertiesRC();
            ReinforcementGrades = CMaterialManager.LoadMaterialPropertiesRF();
            ReinforcementBars = CReinforcementBarManager.LoadReiforcementBarProperties();
            ReinforcementMeshGrades = CMeshesManager.LoadMeshesProperties_Dictionary();

            // To Ondrej - asi by som mal urobit zoznamy objektov vlastnosti/properties priamo v Database Manager
            // v Database Manager mame niekde dictionary, niekde list of properties, neviem ci nam to fakt oboje treba a ci by to nemalo byt jednotne vsade jedno alebo druhe (dictionary alebo list of properties objects)
            // Convert dictionary keys to list of strings - used for combobox items
            ConcreteGradesList = ConcreteGrades.Keys.ToList();
            ReinforcementGradesList = ReinforcementGrades.Keys.ToList();
            List<int> rcBarsDiameters = ReinforcementBars.Keys.ToList();
            ReinforcementBarsList = rcBarsDiameters.ConvertAll<string>(x => x.ToString());
            ReinforcementMeshGradesList = ReinforcementMeshGrades.Keys.ToList();

            // Zoznam poctov vyztuznych tyci pre jeden smer (None alebo 2 - 30)
            ReinforcementBarsCountList = GetReinforcementBarsCountList();

            // Zoznam priemerov kameniva
            AggregateSizesList = new List<string>() { "2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "24", "28", "32", "64", "128", "256" };

            // Zoznam farieb
            ColorList = CComboBoxHelper.ColorList;

            // Set default GUI
            FootingPadMemberTypeIndex = 1;
            
            ConcreteGrade = "30"; // MPa
            AggregateSize = "20"; // mm

            ConcreteDensity = 2300f; // kg / m^3
            ReinforcementGrade = "500E"; // 500 MPa
            ReinforcementMeshGrade = "SE92DE"; // SE92


            //To Mato - toto su tie soil capacity parametre?
            SoilReductionFactor_Phi = 0.5f;
            SoilReductionFactorEQ_Phi = 0.8f;

            SoilBearingCapacity = 200f; // kPa (konverovat kPa na Pa)

            // ---------------------------------------------------------------------------------------------------
            // To Ondrej - tieto hodnoty by sa mali prevziat z vygenerovaneho CModel_PFD_01_GR
            // Alebo sa tu nastavia a podla toho sa vyrobi model ???

            UpdateFloorSlabViewModelFromModel();

            //CFoundation pad = GetSelectedFootingPad();
            //FootingPadSize_x_Or_a = pad.m_fDim1;
            //FootingPadSize_y_Or_b = pad.m_fDim2;
            //FootingPadSize_z_Or_h = pad.m_fDim3;

            // TO ONDREJ S tymito excentricitami je trosku problem
            // Pre rovnaky typ patiek sa im pri vyslednom zobrazeni meni sa im znamienko podla toho ako je otocena patka
            // podla toho ci sme na lavej alebo pravej strane budovy

            //Eccentricity_ex = pad.Eccentricity_x;  //toto nenastavujem lebo bolo zaporne a hned sa to zrube na validacii
            //Eccentricity_ey = pad.Eccentricity_y;
            Eccentricity_ex = 0; // m
            Eccentricity_ey = 0; // m

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SetIsEnabledFirstSawCutPositionInDirectionX()
        {
            if (m_NumberOfSawCutsInDirectionX <= 0)
                IsEnabledFirstSawCutPositionInDirectionX = false;
            else
                IsEnabledFirstSawCutPositionInDirectionX = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SetIsEnabledFirstSawCutPositionInDirectionY()
        {
            if (m_NumberOfSawCutsInDirectionY <= 0)
                IsEnabledFirstSawCutPositionInDirectionY = false;
            else
                IsEnabledFirstSawCutPositionInDirectionY = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        //private void SetIsEnabledSawCutsSpacingInDirectionX()
        //{
        //    if (m_NumberOfSawCutsInDirectionX <= 1)
        //        IsEnabledSawCutsSpacingInDirectionX = false;
        //    else
        //        IsEnabledSawCutsSpacingInDirectionX = true;
        //}

        //-------------------------------------------------------------------------------------------------------------
        //private void SetIsEnabledSawCutsSpacingInDirectionY()
        //{
        //    if (m_NumberOfSawCutsInDirectionY <= 1)
        //        IsEnabledSawCutsSpacingInDirectionY = false;
        //    else
        //        IsEnabledSawCutsSpacingInDirectionY = true;
        //}

        //-------------------------------------------------------------------------------------------------------------
        private void SetIsEnabledFirstControlJointPositionInDirectionX()
        {
            if (m_NumberOfControlJointsInDirectionX <= 0)
                IsEnabledFirstControlJointPositionInDirectionX = false;
            else
                IsEnabledFirstControlJointPositionInDirectionX = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SetIsEnabledFirstControlJointPositionInDirectionY()
        {
            if (m_NumberOfControlJointsInDirectionY <= 0)
                IsEnabledFirstControlJointPositionInDirectionY = false;
            else
                IsEnabledFirstControlJointPositionInDirectionY = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        //private void SetIsEnabledControlJointsSpacingInDirectionX()
        //{
        //    if (m_NumberOfControlJointsInDirectionX <= 1)
        //        IsEnabledControlJointsSpacingInDirectionX = false;
        //    else
        //        IsEnabledControlJointsSpacingInDirectionX = true;
        //}

        //-------------------------------------------------------------------------------------------------------------
        //private void SetIsEnabledControlJointsSpacingInDirectionY()
        //{
        //    if (m_NumberOfControlJointsInDirectionY <= 1)
        //        IsEnabledControlJointsSpacingInDirectionY = false;
        //    else
        //        IsEnabledControlJointsSpacingInDirectionY = true;
        //}

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public CFoundation GetSelectedFootingPad()
        {
            // Select type of footing pads that match with selected footing pad of member type in GUI
            listOfSelectedTypePads = new List<CFoundation>(); //all pads in list should be the same!

            EMemberType_FS_Position memberType = GetSelectedFootingPadMemberType();

            for (int i = 0; i < _pfdVM.Model.m_arrFoundations.Count; i++)
            {
                if (memberType == _pfdVM.Model.m_arrFoundations[i].m_ColumnMemberTypePosition)
                    listOfSelectedTypePads.Add(_pfdVM.Model.m_arrFoundations[i]);
            }

            // All pads in list should be the same!
            CFoundation pad = listOfSelectedTypePads.FirstOrDefault();

            return pad;
        }

        public CConnectionJointTypes GetBaseJointForSelectedNode(CNode node)
        {
            // Vrati spoj typu base plate pre uzol selektovanej patky

            for (int i = 0; i < _pfdVM.Model.m_arrConnectionJoints.Count; i++)
            {
                if (node == _pfdVM.Model.m_arrConnectionJoints[i].m_Node && _pfdVM.Model.m_arrConnectionJoints[i].m_arrPlates[0] is CConCom_Plate_B_basic)
                {
                    return _pfdVM.Model.m_arrConnectionJoints[i];
                }
            }

            return null; // Error - joint wasn't found
        }

        public CSlab GetFloorSlab()
        {
            return _pfdVM.Model.m_arrSlabs.FirstOrDefault();
        }

        public CFoundation GetFootingPad(EMemberType_FS_Position memberType)
        {
            for (int i = 0; i < _pfdVM.Model.m_arrFoundations.Count; i++)
            {
                if (memberType == _pfdVM.Model.m_arrFoundations[i].m_ColumnMemberTypePosition) return _pfdVM.Model.m_arrFoundations[i];
            }
            return null;
        }

        private EMemberType_FS_Position GetSelectedFootingPadMemberType()
        {
            EMemberType_FS_Position memberType;
            if (FootingPadMemberTypeIndex == 0) // TODO - porovnavam s indexom v comboboxe 0-3, asi by bolo istejsie zobrazovat v comboboxe items naviazane na EMemberType_FS_Position, aby sa to neznicilo ked co comboboxu pridam nejaky dalsi typ alebo zmenim poradie
                memberType = EMemberType_FS_Position.MainColumn;
            else if (FootingPadMemberTypeIndex == 1)
                memberType = EMemberType_FS_Position.EdgeColumn;
            else if (FootingPadMemberTypeIndex == 2)
                memberType = EMemberType_FS_Position.ColumnFrontSide;
            else if (FootingPadMemberTypeIndex == 3)
                memberType = EMemberType_FS_Position.ColumnBackSide;
            else
            {
                throw new Exception("Not defined member type!");
            }
            return memberType;
        }

        private List<string> GetReinforcementBarsCountList()
        {
            List<string> list = new List<string>();

            list.Add("None"); // count of bars = 0, unused reinforcement

            int iMinimumNumberOfBars = 2;
            int iMaximumNumberOfBars = 30;

            for (int i = iMinimumNumberOfBars; i <= iMaximumNumberOfBars; i++)
                list.Add(i.ToString());

            return list;
        }

        private void UpdateModelFootingPads()
        {
            foreach (CFoundation pad in listOfSelectedTypePads)
            {
                pad.m_fDim1 = FootingPadSize_x_Or_a;
                pad.m_fDim2 = FootingPadSize_y_Or_b;
                pad.m_fDim3 = FootingPadSize_z_Or_h;
            }
        }

        //set GUI values from Model
        public void UpdateFloorSlabViewModelFromModel()
        {
            //FloorSlabThickness = 125; // mm 0.125f; m
            FloorSlabThickness = _pfdVM.Model.m_arrSlabs.First().m_fDim3 * 1000;
            MeshConcreteCover = _pfdVM.Model.m_arrSlabs.First().ConcreteCover * 1000f;
            ReinforcementMeshGrade = _pfdVM.Model.m_arrSlabs.First().MeshGradeName;

            // Saw Cuts
            FirstSawCutPositionInDirectionX = _pfdVM.Model.m_arrSlabs.First().FirstSawCutPositionInDirectionX;
            FirstSawCutPositionInDirectionY = _pfdVM.Model.m_arrSlabs.First().FirstSawCutPositionInDirectionY;
            NumberOfSawCutsInDirectionX = _pfdVM.Model.m_arrSlabs.First().NumberOfSawCutsInDirectionX;
            NumberOfSawCutsInDirectionY = _pfdVM.Model.m_arrSlabs.First().NumberOfSawCutsInDirectionY;
            SawCutsSpacingInDirectionX = _pfdVM.Model.m_arrSlabs.First().SawCutsSpacingInDirectionX;
            SawCutsSpacingInDirectionY = _pfdVM.Model.m_arrSlabs.First().SawCutsSpacingInDirectionY;
            CutWidth = _pfdVM.Model.m_arrSlabs.First().ReferenceSawCut.CutWidth * 1000;
            CutDepth = _pfdVM.Model.m_arrSlabs.First().ReferenceSawCut.CutDepth * 1000;

            // Control Joints
            FirstControlJointPositionInDirectionX = _pfdVM.Model.m_arrSlabs.First().FirstControlJointPositionInDirectionX;
            FirstControlJointPositionInDirectionY = _pfdVM.Model.m_arrSlabs.First().FirstControlJointPositionInDirectionY;
            NumberOfControlJointsInDirectionX = _pfdVM.Model.m_arrSlabs.First().NumberOfControlJointsInDirectionX;
            NumberOfControlJointsInDirectionY = _pfdVM.Model.m_arrSlabs.First().NumberOfControlJointsInDirectionY;
            ControlJointsSpacingInDirectionX = _pfdVM.Model.m_arrSlabs.First().ControlJointsSpacingInDirectionX;
            ControlJointsSpacingInDirectionY = _pfdVM.Model.m_arrSlabs.First().ControlJointsSpacingInDirectionY;
            DowelDiameter = (_pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.ReferenceDowel.Diameter_shank * 1000).ToString("F0");
            DowelLength = _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.ReferenceDowel.Length * 1000;
            DowelSpacing = _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.DowelSpacing * 1000;

            // Perimeters
            // TODO - nemuseli by sa pouzivat indexy, ale dalo by sa vyhladavat left, right, front, back podla 
            // parametra m_BuildingSide v objekte CSlabPerimeter

            // Index 0 / first - lava strana
            PerimeterDepth_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().PerimeterDepth * 1000;
            PerimeterWidth_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().PerimeterWidth * 1000;
            StartersLapLength_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().StartersLapLength * 1000;
            StartersSpacing_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().StartersSpacing * 1000;
            Starters_Phi_LRSide = (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().Starters_Phi * 1000).ToString("F0");
            Longitud_Reinf_TopAndBotom_Phi_LRSide = (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().Longitud_Reinf_TopAndBotom_Phi * 1000).ToString("F0");
            Longitud_Reinf_Intermediate_Phi_LRSide = (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().Longitud_Reinf_Intermediate_Phi * 1000).ToString("F0");
            Longitud_Reinf_Intermediate_Count_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().Longitud_Reinf_Intermediate_Count;

            // Len ak existuju roller doors, resp. rebate na lavej alebo pravej strane floor slab
            if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().SlabRebates != null)
                RebateWidth_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().SlabRebates.First().RebateWidth * 1000;
            else if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[1].SlabRebates != null)
                RebateWidth_LRSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[1].SlabRebates.First().RebateWidth * 1000;
            else
                RebateWidth_LRSide = 0;

            // Index 2 - predna strana
            PerimeterDepth_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].PerimeterDepth * 1000;
            PerimeterWidth_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].PerimeterWidth * 1000;
            StartersLapLength_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].StartersLapLength * 1000;
            StartersSpacing_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].StartersSpacing * 1000;
            Starters_Phi_FBSide = (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].Starters_Phi * 1000).ToString("F0");
            Longitud_Reinf_TopAndBotom_Phi_FBSide = (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].Longitud_Reinf_TopAndBotom_Phi * 1000).ToString("F0");
            Longitud_Reinf_Intermediate_Phi_FBSide = (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].Longitud_Reinf_Intermediate_Phi * 1000).ToString("F0");
            Longitud_Reinf_Intermediate_Count_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].Longitud_Reinf_Intermediate_Count;

            // Len ak existuju roller doors, resp. rebate na prednej alebo zadnej strane floor slab
            if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].SlabRebates != null)
                RebateWidth_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].SlabRebates.First().RebateWidth * 1000;
            else if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[3].SlabRebates != null)
                RebateWidth_FBSide = _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[3].SlabRebates.First().RebateWidth * 1000;
            else
                RebateWidth_FBSide = 0;
        }

        //GUI was changed, so update Model

        // TO ONDREJ - TAK SOM SA NAKONIEC DOPATRAL K INEJ METODE, KTORA ROBI TO ISTE 
        // NASTAVUJE HODNOTY Z GUI DO REALNYCH OBJEKTOV
        // Pozri UC_FootingInput.xaml.cs UpdateModelProperties(), line 175
        // Prosim pozri sa na to a skus to zjednotit aby sme to mali pre footing pads a floor slab podobne

        public void UpdateFloorSlabModelFromGUI_XXX()
        {
            foreach (CSlab slab in _pfdVM.Model.m_arrSlabs)
            {
                // TAKTO BY SME PRISTUPOVALI A MENILI JEDNOTLIVE PARAMETRE
                /*
                foreach (CSlabPerimeter perimeter in slab.PerimeterBeams)
                {
                    foreach (CSlabRebate rebate in perimeter.SlabRebates)
                    {
                        if(perimeter.BuildingSide == "Front" || perimeter.BuildingSide == "Back")                        
                            rebate.RebateWidth = RebateWidth_FBSide / 1000;
                        else rebate.RebateWidth = RebateWidth_LRSide / 1000;
                    }
                }
                */

                // NASTAVIME PROPERTIES PRE SLAB A VSETKY OBJEKTY V SLAB PREGENERUJEME (VYGENERUJEME UPLNE NANOVO

                _pfdVM.Model.m_arrSlabs.First().m_fDim3 = FloorSlabThickness / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().ConcreteCover = MeshConcreteCover / 1000f;
                _pfdVM.Model.m_arrSlabs.First().MeshGradeName = ReinforcementMeshGrade;

                // Saw Cuts
                _pfdVM.Model.m_arrSlabs.First().FirstSawCutPositionInDirectionX = FirstSawCutPositionInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().FirstSawCutPositionInDirectionY = FirstSawCutPositionInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().NumberOfSawCutsInDirectionX = NumberOfSawCutsInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().NumberOfSawCutsInDirectionY = NumberOfSawCutsInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().SawCutsSpacingInDirectionX = SawCutsSpacingInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().SawCutsSpacingInDirectionY = SawCutsSpacingInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().ReferenceSawCut.CutWidth = CutWidth / 1000f;
                _pfdVM.Model.m_arrSlabs.First().ReferenceSawCut.CutDepth = CutDepth / 1000f;

                // Control Joints
                _pfdVM.Model.m_arrSlabs.First().FirstControlJointPositionInDirectionX = FirstControlJointPositionInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().FirstControlJointPositionInDirectionY = FirstControlJointPositionInDirectionY; 
                _pfdVM.Model.m_arrSlabs.First().NumberOfControlJointsInDirectionX = NumberOfControlJointsInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().NumberOfControlJointsInDirectionY = NumberOfControlJointsInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().ControlJointsSpacingInDirectionX =  ControlJointsSpacingInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().ControlJointsSpacingInDirectionY = ControlJointsSpacingInDirectionY; 
                //_pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.ReferenceDowel.Diameter_shank = (float.Parse(DowelDiameter)) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.ReferenceDowel.Length = DowelLength / 1000f;
                _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.DowelSpacing =DowelSpacing / 1000;

                // Perimeters

                // Left / Right Side
                //_pfdVM.Model.m_arrSlabs.First().PerimeterDepth_LRSide = PerimeterDepth_LRSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().PerimeterWidth_LRSide = PerimeterWidth_LRSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().StartersLapLength_LRSide = StartersLapLength_LRSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().StartersSpacing_LRSide = StartersSpacing_LRSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().Starters_Phi_LRSide = (float.Parse(Starters_Phi_LRSide)) / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_TopAndBotom_Phi_LRSide = (float.Parse(Longitud_Reinf_TopAndBotom_Phi_LRSide)) / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Phi_LRSide = (float.Parse(Longitud_Reinf_Intermediate_Phi_LRSide)) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Count_LRSide =  Longitud_Reinf_Intermediate_Count_LRSide;

                // Rebate
                _pfdVM.Model.m_arrSlabs.First().RebateWidth_LRSide = RebateWidth_LRSide / 1000f;

                // Front / Back Side

                //_pfdVM.Model.m_arrSlabs.First().PerimeterDepth_FBSide = PerimeterDepth_FBSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().PerimeterWidth_FBSide = PerimeterWidth_FBSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().StartersLapLength_FBSide = StartersLapLength_FBSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().StartersSpacing_FBSide = StartersSpacing_FBSide / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().Starters_Phi_FBSide = (float.Parse(Starters_Phi_FBSide)) / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_TopAndBotom_Phi_FBSide = (float.Parse(Longitud_Reinf_TopAndBotom_Phi_FBSide)) / 1000f;
                //_pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Phi_FBSide = (float.Parse(Longitud_Reinf_Intermediate_Phi_FBSide)) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Count_FBSide = Longitud_Reinf_Intermediate_Count_FBSide;

                // Rebate
                _pfdVM.Model.m_arrSlabs.First().RebateWidth_FBSide = RebateWidth_FBSide / 1000f;

                // PREGENERUJ OBJEKTY V SLAB
                _pfdVM.Model.m_arrSlabs.First().SetControlPoint();
                _pfdVM.Model.m_arrSlabs.First().SetTextPoint();
                _pfdVM.Model.m_arrSlabs.First().CreateSawCuts();
                _pfdVM.Model.m_arrSlabs.First().CreateControlJoints();
                _pfdVM.Model.m_arrSlabs.First().CreateMesh();
                _pfdVM.Model.m_arrSlabs.First().CreatePerimeters();
                _pfdVM.Model.m_arrSlabs.First().SetDescriptionText();
            }
        }

        //vyrobil som novu metodu, lebo toto podla mna mala robit metoda, ktoru sme mali nizsie UpdateValuesInGUI
        //ak zmenim index pre Footing Pad type sa musia v GUI prestavit vsetky hodnoty podla toho co ma dany objekt v modeli
        //metoda sluzi na nastavenie prvkov v GUI podla Foooting pad v modeli
        private void UpdateValuesInGUIFromSelectedFootingPad()
        {
            CFoundation pad = GetSelectedFootingPad();

            //toto vsetko treba nastavit z objektu CFoundation pad do GUI

            //TODO- Mato skontrolovat a nastavit
            //Concrete Grade
            ConcreteGrade = pad.m_Mat.Name;
            //Max. Aggregate Size
            //ConcreteDensity
            ConcreteDensity = pad.m_Mat.m_fRho;
            //Reinforcement Grade
            //SoilReductionFactor_Phi = pad.SoilReductionFactor_Phi;
            //SoilReductionFactorEQ_Phi = pad.SoilReductionFactorEQ_Phi;
            //SoilBearingCapacity = pad.SoilBearingCapacity;
            ConcreteCover = pad.ConcreteCover * 1000; // mm

            FootingPadSize_x_Or_a = pad.m_fDim1;
            FootingPadSize_y_Or_b = pad.m_fDim2;
            FootingPadSize_z_Or_h = pad.m_fDim3;

            //Eccentricity_ex
            //Eccentricity_ey
            //LongReinTop_x_No
            //LongReinTop_x_Phi
            //LongReinTop_x_distance_s_y
            //LongReinTop_x_ColorIndex
            //LongReinTop_y_No
            //LongReinTop_y_Phi
            //LongReinTop_y_distance_s_x
            //LongReinTop_y_ColorIndex

            //LongReinBottom_x_No
            //LongReinBottom_x_Phi
            //LongReinBottom_x_distance_s_y
            //LongReinBottom_x_ColorIndex

            //LongReinBottom_y_No
            //LongReinBottom_y_Phi
            //LongReinBottom_y_distance_s_x
            //LongReinBottom_y_ColorIndex


            //to co bolo // TO Ondrej K comu je ta poznamka :) ??????????????????

            LongReinTop_x_distance_s_y = 0;
            LongReinTop_y_distance_s_x = 0;
            LongReinBottom_x_distance_s_y = 0;
            LongReinBottom_y_distance_s_x = 0;

            LongReinTop_x_No = pad.Count_Top_Bars_x == 0 ? "None" : pad.Count_Top_Bars_x.ToString();
            LongReinTop_x_Phi = (pad.Reference_Top_Bar_x.Diameter * 1000f).ToString();

            LongReinTop_y_No = pad.Count_Top_Bars_y == 0 ? "None" : pad.Count_Top_Bars_y.ToString();
            LongReinTop_y_Phi = (pad.Reference_Top_Bar_y.Diameter * 1000f).ToString();

            LongReinBottom_x_No = pad.Count_Bottom_Bars_x == 0 ? "None" : pad.Count_Bottom_Bars_x.ToString();
            LongReinBottom_x_Phi = (pad.Reference_Bottom_Bar_x.Diameter * 1000f).ToString();

            LongReinBottom_y_No = pad.Count_Bottom_Bars_y == 0 ? "None" : pad.Count_Bottom_Bars_y.ToString();
            LongReinBottom_y_Phi = (pad.Reference_Bottom_Bar_y.Diameter * 1000f).ToString();

            if (LongReinTop_x_No != "None")
                LongReinTop_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, int.Parse(LongReinTop_x_No), float.Parse(LongReinTop_x_Phi) * 0.001f, pad.Count_Top_Bars_y > 0 ? float.Parse(LongReinTop_y_Phi) * 0.001f : 0, ConcreteCover * 0.001f, false);

            if (LongReinTop_y_No != "None")
                LongReinTop_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, int.Parse(LongReinTop_y_No), float.Parse(LongReinTop_y_Phi) * 0.001f, pad.Count_Top_Bars_x > 0 ? float.Parse(LongReinTop_x_Phi) * 0.001f :0, ConcreteCover * 0.001f, false);

            if (LongReinBottom_x_No != "None")
                LongReinBottom_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, int.Parse(LongReinBottom_x_No), float.Parse(LongReinBottom_x_Phi) * 0.001f, pad.Count_Bottom_Bars_y > 0 ? float.Parse(LongReinBottom_y_Phi) * 0.001f :0, ConcreteCover * 0.001f, false);

            if (LongReinBottom_y_No != "None")
                LongReinBottom_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, int.Parse(LongReinBottom_y_No), float.Parse(LongReinBottom_y_Phi) * 0.001f, pad.Count_Bottom_Bars_x > 0 ? float.Parse(LongReinBottom_x_Phi) * 0.001f :0, ConcreteCover * 0.001f, false);

            LongReinTop_x_ColorIndex = CComboBoxHelper.GetColorIndex(Colors.CadetBlue);
            LongReinTop_y_ColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            LongReinBottom_x_ColorIndex = CComboBoxHelper.GetColorIndex(Colors.YellowGreen);
            LongReinBottom_y_ColorIndex = CComboBoxHelper.GetColorIndex(Colors.Purple);
        }

        //tato metoda ma sluzit na nastavenie vlastnosti oznaceneho Footing pad podla hodnot z GUI
        //volat by sa mala po zmene v GUI
        private void UpdateSelectedFootingPadsValuesFromGUI()
        {
            IsSetFromCode = true;

            // Default reinforcement
            int iLongReinTop_x_No = LongReinTop_x_No == "None" ? 0 : Convert.ToInt32(LongReinTop_x_No);
            int iLongReinTop_y_No = LongReinTop_y_No == "None" ? 0 : Convert.ToInt32(LongReinTop_y_No);
            int iLongReinBottom_x_No = LongReinBottom_x_No == "None" ? 0 : Convert.ToInt32(LongReinBottom_x_No);
            int iLongReinBottom_y_No = LongReinBottom_y_No == "None" ? 0 : Convert.ToInt32(LongReinBottom_y_No);

            LongReinTop_x_distance_s_y = 0;
            LongReinTop_y_distance_s_x = 0;
            LongReinBottom_x_distance_s_y = 0;
            LongReinBottom_y_distance_s_x = 0;

            float fConcreteCover = ConcreteCover / 1000f; // Hodnota v metroch

            if (iLongReinTop_x_No > 0)
                LongReinTop_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, iLongReinTop_x_No, (float)Convert.ToDouble(LongReinTop_x_Phi) * 0.001f, (float)Convert.ToDouble(LongReinTop_y_Phi) * 0.001f, fConcreteCover, false); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)
            if (iLongReinTop_y_No > 0)
                LongReinTop_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, iLongReinTop_y_No, (float)Convert.ToDouble(LongReinTop_y_Phi) * 0.001f, (float)Convert.ToDouble(LongReinTop_x_Phi) * 0.001f, fConcreteCover, false); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)
            if (iLongReinBottom_x_No > 0)
                LongReinBottom_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, iLongReinBottom_x_No, (float)Convert.ToDouble(LongReinBottom_x_Phi) * 0.001f, (float)Convert.ToDouble(LongReinBottom_y_Phi) * 0.001f, fConcreteCover, false); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)
            if (iLongReinBottom_y_No > 0)
                LongReinBottom_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, iLongReinBottom_y_No, (float)Convert.ToDouble(LongReinBottom_y_Phi) * 0.001f, (float)Convert.ToDouble(LongReinBottom_x_Phi) * 0.001f, fConcreteCover, false); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            // Update reference bars control points
            // Meni sa vtedy ak sa zmeni cover alebo priemer tyce
            // Prevezmeme hodnoty z GUI a previeme zo stringu na cislo v metroch
            float fDiameterTop_Bar_x = float.Parse(LongReinTop_x_Phi) / 1000f;
            float fDiameterTop_Bar_y = float.Parse(LongReinTop_y_Phi) / 1000f;
            float fDiameterBottom_Bar_x = float.Parse(LongReinBottom_x_Phi) / 1000f;
            float fDiameterBottom_Bar_y = float.Parse(LongReinBottom_y_Phi) / 1000f;

            // Reference / first bar coordinates
            double cp_Top_x_coordX = UseStraightReinforcementBars ? fConcreteCover : fConcreteCover + 0.5f * fDiameterTop_Bar_x;
            double cp_Top_x_coordY = UseStraightReinforcementBars ? fConcreteCover + 0.5f * fDiameterTop_Bar_x : fConcreteCover + fDiameterTop_Bar_y + 0.5f * fDiameterTop_Bar_x;
            double cp_Top_y_coordX = UseStraightReinforcementBars ? fConcreteCover + 0.5f * fDiameterTop_Bar_y : fConcreteCover + fDiameterTop_Bar_x + 0.5f * fDiameterTop_Bar_y;
            double cp_Top_y_coordY = UseStraightReinforcementBars ? fConcreteCover : fConcreteCover + 0.5f * fDiameterTop_Bar_y;
            double cp_Bottom_x_coordX = UseStraightReinforcementBars ? fConcreteCover : fConcreteCover + 0.5f * fDiameterBottom_Bar_x;
            double cp_Bottom_x_coordY = UseStraightReinforcementBars ? fConcreteCover + 0.5f * fDiameterBottom_Bar_x : fConcreteCover + fDiameterBottom_Bar_y + 0.5f * fDiameterBottom_Bar_x;
            double cp_Bottom_y_coordX = UseStraightReinforcementBars ? fConcreteCover + 0.5f * fDiameterBottom_Bar_y : fConcreteCover + fDiameterBottom_Bar_x + 0.5f * fDiameterBottom_Bar_y;
            double cp_Bottom_y_coordY = UseStraightReinforcementBars ? fConcreteCover : fConcreteCover + 0.5f * fDiameterBottom_Bar_y;

            Point3D cp_Top_x = new Point3D(cp_Top_x_coordX, cp_Top_x_coordY, m_FootingPadSize_z_Or_h - fConcreteCover - fDiameterTop_Bar_y - 0.5f * fDiameterTop_Bar_x);
            Point3D cp_Top_y = new Point3D(cp_Top_y_coordX, cp_Top_y_coordY, m_FootingPadSize_z_Or_h - fConcreteCover - 0.5f * fDiameterTop_Bar_y);
            Point3D cp_Bottom_x = new Point3D(cp_Bottom_x_coordX, cp_Bottom_x_coordY, fConcreteCover + fDiameterBottom_Bar_y + 0.5f * fDiameterBottom_Bar_x);
            Point3D cp_Bottom_y = new Point3D(cp_Bottom_y_coordX, cp_Bottom_y_coordY, fConcreteCover + 0.5f * fDiameterBottom_Bar_y);

            if (!UseStraightReinforcementBars)
            {
                cp_Top_x = new Point3D(cp_Top_x_coordX, cp_Top_x_coordY, fConcreteCover + fDiameterTop_Bar_y);
                cp_Top_y = new Point3D(cp_Top_y_coordX, cp_Top_y_coordY, fConcreteCover);

                // Kedze sa vertikalne casti hornych a spodnych prutov prekryvaju posunieme horne pruty o sucet polovic priemeru
                cp_Top_x.Y = cp_Top_x_coordY + 0.5 * fDiameterTop_Bar_x + 0.5 * fDiameterBottom_Bar_x;
                cp_Top_y.X = cp_Top_y_coordX + 0.5 * fDiameterTop_Bar_y + 0.5 * fDiameterBottom_Bar_y;

                cp_Bottom_x = new Point3D(cp_Bottom_x_coordX, cp_Bottom_x_coordY, m_FootingPadSize_z_Or_h - fConcreteCover - fDiameterBottom_Bar_y);
                cp_Bottom_y = new Point3D(cp_Bottom_y_coordX, cp_Bottom_y_coordY, m_FootingPadSize_z_Or_h - fConcreteCover);
            }

            // Regenerate reinforcement bars
            foreach (CFoundation pad in listOfSelectedTypePads)
            {
                // For each pad recalculate lengths of reference bars
                pad.Reference_Top_Bar_x.ProjectionLength = pad.Reference_Top_Bar_x is CReinforcementBarStraight ? m_FootingPadSize_x_Or_a - 2 * fConcreteCover : m_FootingPadSize_x_Or_a - 2 * fConcreteCover - pad.Reference_Top_Bar_x.Diameter;
                pad.Reference_Bottom_Bar_x.ProjectionLength = pad.Reference_Bottom_Bar_x is CReinforcementBarStraight ? m_FootingPadSize_x_Or_a - 2 * fConcreteCover : m_FootingPadSize_x_Or_a - 2 * fConcreteCover - pad.Reference_Bottom_Bar_x.Diameter;

                pad.Reference_Top_Bar_y.ProjectionLength = pad.Reference_Top_Bar_y is CReinforcementBarStraight ? m_FootingPadSize_y_Or_b - 2 * fConcreteCover : m_FootingPadSize_y_Or_b - 2 * fConcreteCover - pad.Reference_Top_Bar_y.Diameter;
                pad.Reference_Bottom_Bar_y.ProjectionLength = pad.Reference_Bottom_Bar_y is CReinforcementBarStraight? m_FootingPadSize_y_Or_b - 2 * fConcreteCover : m_FootingPadSize_y_Or_b - 2 * fConcreteCover - pad.Reference_Bottom_Bar_y.Diameter;

                // For each pad set for all reference bars current control point
                pad.Reference_Top_Bar_x.m_pControlPoint = cp_Top_x;
                pad.Reference_Top_Bar_y.m_pControlPoint = cp_Top_y;
                pad.Reference_Bottom_Bar_x.m_pControlPoint = cp_Bottom_x;
                pad.Reference_Bottom_Bar_y.m_pControlPoint = cp_Bottom_y;

                // Create sets of reinforcement bars
                pad.CreateReinforcementBars();
            }
            
            //Floor slab by som riesil v inej metode
            // Floor Slab
            float fFloorSlab_aX = _pfdVM.Model.m_arrSlabs.First().m_fDim1; // _pfdVM.GableWidth; // Tu by sme potrebovali dostat rozmery floor slab
            float fFloorSlab_bY = _pfdVM.Model.m_arrSlabs.First().m_fDim2; // _pfdVM.Length;

            // Predpoklada sa, ze posledny saw cut je rovnako vzdialeny od konca ako prvy od zaciatku
            if (m_NumberOfSawCutsInDirectionX >= 1)
                SawCutsSpacingInDirectionX = (fFloorSlab_aX - 2 * m_FirstSawCutPositionInDirectionX) / (m_NumberOfSawCutsInDirectionX - 1);
            if (m_NumberOfSawCutsInDirectionY >= 1)
                SawCutsSpacingInDirectionY = (fFloorSlab_bY - 2 * m_FirstSawCutPositionInDirectionY) / (m_NumberOfSawCutsInDirectionY - 1);

            // Predpoklada sa, ze posledny control joint je rovnako vzdialeny od konca ako prvy od zaciatku
            if (m_NumberOfControlJointsInDirectionX >= 1)
                ControlJointsSpacingInDirectionX = (fFloorSlab_aX - 2 * m_FirstControlJointPositionInDirectionX) / (m_NumberOfControlJointsInDirectionX - 1);
            if (m_NumberOfControlJointsInDirectionY >= 1)
                ControlJointsSpacingInDirectionY = (fFloorSlab_bY - 2 * m_FirstControlJointPositionInDirectionY) / (m_NumberOfControlJointsInDirectionY - 1);

            IsSetFromCode = false;
        }

        private float GetDistanceBetweenReinforcementBars(float footingPadWidth, int iNumberOfBarsPerSection, float fBarDiameter, float fPerpendicularBarDiameter, float fConcreteCover, bool bIsPerpendicularStraightBar)
        {
            // Odpocitavam 2 priemery kolmych prutov, kedze sa ocakavaju aj zvisle casti prutov, ak je vystuz len horizontalna ma sa odpocitat len jeden priemer
            int iNumberOfDiameters = bIsPerpendicularStraightBar ? 0 : 2;
            return (footingPadWidth - 2 * fConcreteCover - iNumberOfDiameters * fPerpendicularBarDiameter - fBarDiameter) / (iNumberOfBarsPerSection - 1);
        }

        //private void GetDefaultFootingPadSize(out float faX, out float fbY, out float fhZ)
        //{
        //    if (FootingPadMemberTypeIndex <= 1)
        //    {
        //        // Main or edge frame column (0 and 1)
        //        faX = (float)Math.Round(MathF.Max(0.6f, Math.Min(_pfdVM.GableWidth * 0.08f, _pfdVM.fL1 * 0.40f)), 1);
        //        fbY = (float)Math.Round(MathF.Max(0.6f, Math.Min(_pfdVM.GableWidth * 0.07f, _pfdVM.fL1 * 0.35f)), 1);
        //        fhZ = 0.4f;
        //    }
        //    else // Front a back side wind posts (2 and 3)
        //    {
        //        float fDist_Column;

        //        // Pripravene pre rozne rozostupy wind post na prednej a zadnej strane budovy
        //        if (FootingPadMemberTypeIndex == 2) // Front Side
        //            fDist_Column = _pfdVM.ColumnDistance;
        //        else // Back Side
        //            fDist_Column = _pfdVM.ColumnDistance;

        //        // Front or back side - wind posts
        //        faX = (float)Math.Round(MathF.Max(0.5f, fDist_Column * 0.40f), 1);
        //        fbY = (float)Math.Round(MathF.Max(0.5f, fDist_Column * 0.40f), 1);
        //        fhZ = 0.4f;
        //    }
        //}

        //private int GetDefaultNumberOfReinforcingBars(float footingPadWidth, float fBarDiameter, float fConcreteCover)
        //{
        //    // Pre priblizne urcenie poctu vyztuznych prutov pouzijeme ich defaultnu vzdialenost 150 mm medzi stredmi tyci
        //    float fDefaultDistanceBetweenReinforcementBars = 0.15f; // 150 mm

        //    // Number of spacings + 1
        //    return (int)((footingPadWidth - 2 * fConcreteCover - 3 * fBarDiameter) / fDefaultDistanceBetweenReinforcementBars) + 1;
        //}

        public CalculationSettingsFoundation GetCalcSettings()
        {
            CalculationSettingsFoundation calc = new CalculationSettingsFoundation();
            calc.ConcreteDensity = ConcreteDensity;
            calc.ConcreteGrade = ConcreteGrade;
            calc.ReinforcementGrade = ReinforcementGrade;

            calc.SoilReductionFactor_Phi = SoilReductionFactor_Phi;
            calc.SoilReductionFactorEQ_Phi = SoilReductionFactorEQ_Phi;
            calc.SoilBearingCapacity = SoilBearingCapacity * 1000;  // kPa to Pa
            calc.FloorSlabThickness = FloorSlabThickness / 1000f; // mm to meters

            calc.AggregateSize = float.Parse(AggregateSize) / 1000f; // Float value in meters

            return calc;
        }
    }
}