using BaseClasses;
using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

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

        private List<string> m_ConcreteGradesList;
        private List<string> m_AggregateSizesList;
        private List<string> m_ReinforcementGradesList;
        private List<string> m_ReinforcementBarsList;
        private List<string> m_ReinforcementBarsCountList;

        private int m_FootingPadMemberTypeIndex;
        private string m_ConcreteGrade;
        private string m_AggregateSize;
        private float m_ConcreteDensity;
        private string m_ReinforcementGrade;

        private string m_LongReinTop_x_No;
        private string m_LongReinTop_x_Phi;
        private float m_LongReinTop_x_distance_s_y;

        private string m_LongReinTop_y_No;
        private string m_LongReinTop_y_Phi;
        private float m_LongReinTop_y_distance_s_x;

        private string m_LongReinBottom_x_No;
        private string m_LongReinBottom_x_Phi;
        private float m_LongReinBottom_x_distance_s_y;

        private string m_LongReinBottom_y_No;
        private string m_LongReinBottom_y_Phi;
        private float m_LongReinBottom_y_distance_s_x;

        private float m_FootingPadSize_x_Or_a;
        private float m_FootingPadSize_y_Or_b;
        private float m_FootingPadSize_z_Or_h;

        private float m_Eccentricity_ex;
        private float m_Eccentricity_ey;

        private float m_SoilReductionFactor_Phi;
        private float m_SoilReductionFactorEQ_Phi;

        private float m_SoilBearingCapacity;
        private float m_ConcreteCover;
        private float m_FloorSlabThickness;

        private List<CFoundation> listOfSelectedTypePads;
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
        public int FootingPadMemberTypeIndex
        {
            get
            {
                return m_FootingPadMemberTypeIndex;
            }

            set
            {
                m_FootingPadMemberTypeIndex = value;
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
                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                  pad.Reference_Top_Bar_x.m_fDim1 = 0.5f * float.Parse(LongReinTop_x_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateValuesInGUI();

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

                if (IsSetFromCode == false) UpdateValuesInGUI(); // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika

                NotifyPropertyChanged("LongReinTop_x_distance_s_y");
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

                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                    pad.Reference_Top_Bar_y.m_fDim1 = 0.5f * float.Parse(LongReinTop_y_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                if (IsSetFromCode == false) UpdateValuesInGUI(); // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika
                NotifyPropertyChanged("LongReinTop_y_distance_s_x");
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

                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                    pad.Reference_Bottom_Bar_x.m_fDim1 = 0.5f * float.Parse(LongReinBottom_x_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                if (IsSetFromCode == false) UpdateValuesInGUI();  // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika
                NotifyPropertyChanged("LongReinBottom_x_distance_s_y");
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

                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                    pad.Reference_Bottom_Bar_y.m_fDim1 = 0.5f * float.Parse(LongReinBottom_y_Phi) / 1000f;
                }
                if (IsSetFromCode == false) UpdateValuesInGUI();

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
                if (IsSetFromCode == false) UpdateValuesInGUI();  // Toto som tu dal asi zbytocne, ked sa zmeni pocet tyci zmeni sa automaticky aj vzdialenost medzi nimi a updatuje sa grafika
                NotifyPropertyChanged("LongReinBottom_y_distance_s_x");
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
                }

                if (IsSetFromCode == false) UpdateValuesInGUI();
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
                }
                if (IsSetFromCode == false) UpdateValuesInGUI();
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
                if (value < 0.1f || value > 2f)
                    throw new ArgumentException("Footing pad size must be between 0.1 and 2 [m]");

                m_FootingPadSize_z_Or_h = value;
                foreach (CFoundation pad in listOfSelectedTypePads)
                {
                    pad.m_fDim3 = FootingPadSize_z_Or_h;
                }
                if (IsSetFromCode == false) UpdateValuesInGUI();
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
                                                                          "and x/2 = " + string.Format("{0:0.000}",  0.5f * FootingPadSize_x_Or_a) + " [m]");

                m_Eccentricity_ex = value;
                if (IsSetFromCode == false) UpdateValuesInGUI();
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
                    throw new ArgumentException("Eccentricity must be between -y/2= " + string.Format("{0:0.000}", -0.5f * FootingPadSize_y_Or_b)+
                                                                          "and y/2= " + string.Format("{0:0.000}",  0.5f * FootingPadSize_y_Or_b) + "[m]");

                m_Eccentricity_ey = value;
                if (IsSetFromCode == false) UpdateValuesInGUI();
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
                if (IsSetFromCode == false) UpdateValuesInGUI();

                NotifyPropertyChanged("ConcreteCover");
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
                _model.m_arrGOVolumes.First().m_fDim3 = m_FloorSlabThickness / 1000;
                NotifyPropertyChanged("FloorSlabThickness");
            }
        }

        CPFDViewModel _pfdVM;
        CModel_PFD_01_GR _model;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CFootingInputVM(CPFDViewModel pfdVM)
        {
            IsSetFromCode = true;
            _pfdVM = pfdVM;
            _model = pfdVM.Model as CModel_PFD_01_GR;
            // Fill dictionaries
            ConcreteGrades = CMaterialManager.LoadMaterialPropertiesRC();
            ReinforcementGrades = CMaterialManager.LoadMaterialPropertiesRF();
            ReinforcementBars = CReinforcementBarManager.LoadReiforcementBarProperties();

            // To Ondrej - asi by som mal urobit zoznamy objektov vlastnosti/properties priamo v Database Manager
            // v Database Manager mame niekde dictionary, niekde list of properties, neviem ci nam to fakt oboje treba a ci by to nemalo byt jednotne vsade jedno alebo druhe (dictionary alebo list of properties objects)
            // Convert dictionary keys to list of strings - used for combobox items
            ConcreteGradesList = ConcreteGrades.Keys.ToList();
            ReinforcementGradesList = ReinforcementGrades.Keys.ToList();
            List<int> rcBarsDiameters = ReinforcementBars.Keys.ToList();
            ReinforcementBarsList = rcBarsDiameters.ConvertAll<string>(x => x.ToString());

            // Zoznam poctov vyztuznych tyci pre jeden smer (None alebo 2 - 30)
            ReinforcementBarsCountList = GetReinforcementBarsCountList();

            // Zoznam priemerov kameniva
            AggregateSizesList = new List<string>() {"2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "24", "28", "32", "64", "128", "256" };

            // Set default GUI
            FootingPadMemberTypeIndex = 1;

            ConcreteGrade = "30"; // MPa
            AggregateSize = "20"; // mm

            ConcreteDensity = 2300f; // kg / m^3
            ReinforcementGrade = "500E"; // 500 MPa

            SoilReductionFactor_Phi = 0.5f;
            SoilReductionFactorEQ_Phi = 0.8f;

            SoilBearingCapacity = 200f; // kPa (konverovat kPa na Pa)

            // ---------------------------------------------------------------------------------------------------
            // To Ondrej - tieto hodnoty by sa mali prevziat z vygenerovaneho CModel_PFD_01_GR
            // Alebo sa tu nastavia a podla toho sa vyrobi model
            
            //FloorSlabThickness = 125; // mm 0.125f; m
            FloorSlabThickness = _model.m_arrGOVolumes.First().m_fDim3 * 1000;

            CFoundation pad = GetSelectedFootingPad();
            FootingPadSize_x_Or_a = pad.m_fDim1;
            FootingPadSize_y_Or_b = pad.m_fDim2;
            FootingPadSize_z_Or_h = pad.m_fDim3;

            // TO ONDREJ S tymito excentricitami je trosku problem
            // Pre rovnaky typ patiek sa im pri vyslednom zobrazeni meni sa im znamienko podla toho ako je otocena patka
            // podla toho ci sme na lavej alebo pravej strane budovy

            //Eccentricity_ex = pad.Eccentricity_x;  //toto nenastavujem lebo bolo zaporne a hned sa to zrube na validacii
            //Eccentricity_ey = pad.Eccentricity_y;
            Eccentricity_ex = 0; // m
            Eccentricity_ey = 0; // m

            // To Ondrej - doriesit zadavacie a vypoctove jednotky mm a m
            ConcreteCover = pad.ConcreteCover * 1000; // mm  0.075f; m

            LongReinTop_x_No = pad.Count_Top_Bars_x.ToString();
            LongReinTop_x_Phi = (pad.Reference_Top_Bar_x.m_fDim1 * 2f * 1000f).ToString();
            LongReinTop_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, int.Parse(LongReinTop_x_No), float.Parse(LongReinTop_x_Phi), ConcreteCover);

            LongReinTop_y_No = pad.Count_Top_Bars_y.ToString();
            LongReinTop_y_Phi = (pad.Reference_Top_Bar_y.m_fDim1 * 2f * 1000f).ToString();
            LongReinTop_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, int.Parse(LongReinTop_y_No), float.Parse(LongReinTop_y_Phi), ConcreteCover);

            LongReinBottom_x_No = pad.Count_Bottom_Bars_x.ToString();
            LongReinBottom_x_Phi=(pad.Reference_Bottom_Bar_x.m_fDim1 * 2f * 1000f).ToString();
            LongReinBottom_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, int.Parse(LongReinBottom_x_No), float.Parse(LongReinBottom_x_Phi), ConcreteCover);

            LongReinBottom_y_No = pad.Count_Bottom_Bars_y.ToString();
            LongReinBottom_y_Phi=(pad.Reference_Bottom_Bar_y.m_fDim1 * 2f * 1000f).ToString();
            LongReinBottom_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, int.Parse(LongReinBottom_y_No), float.Parse(LongReinBottom_y_Phi), ConcreteCover);
            //----------------------------------------------------------------------------------------------------

            IsSetFromCode = false;
        }

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

        //private void SetDefaultFootingPadSize()
        //{
        //    // Dimensions of footing pad in meters
        //    //FootingPadSize_x_Or_a = 1.0f;
        //    //FootingPadSize_y_Or_b = 1.0f;
        //    //FootingPadSize_z_Or_h = 0.4f;

        //    // Default size of footing pad
        //    float faX, fbY, fhZ;
        //    GetDefaultFootingPadSize(out faX, out fbY, out fhZ);

        //    FootingPadSize_x_Or_a = faX;
        //    FootingPadSize_y_Or_b = fbY;
        //    FootingPadSize_z_Or_h = fhZ;

        //    UpdateValuesInGUI();
        //}

        private void UpdateModelFootingPads()
        {
            foreach (CFoundation pad in listOfSelectedTypePads)
            {
                pad.m_fDim1 = FootingPadSize_x_Or_a;
                pad.m_fDim2 = FootingPadSize_y_Or_b;
                pad.m_fDim3 = FootingPadSize_z_Or_h;
            }
        }

        private void UpdateValuesInGUI()
        {
            IsSetFromCode = true;

            // Default reinforcement

            //LongReinTop_x_No = "5";
            //LongReinTop_x_No = GetDefaultNumberOfReinforcingBars(FootingPadSize_y_Or_b, (float)Convert.ToDouble(LongReinTop_x_Phi) * 0.001f, ConcreteCover * 0.001f).ToString();
            //LongReinTop_x_Phi = "12";
            //LongReinTop_x_Phi = 0.012f; // bar diameter mm to m
            //LongReinTop_x_distance_s_y = 0.2035f; // m
            LongReinTop_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, Convert.ToInt32(LongReinTop_x_No), (float)Convert.ToDouble(LongReinTop_x_Phi) * 0.001f, ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            //LongReinTop_y_No = "5";
            //LongReinTop_y_No = GetDefaultNumberOfReinforcingBars(FootingPadSize_x_Or_a, (float)Convert.ToDouble(LongReinTop_y_Phi) * 0.001f, ConcreteCover * 0.001f).ToString();
            //LongReinTop_y_Phi = "12";
            //LongReinTop_y_Phi = 0.012f; // bar diameter mm to m
            //LongReinTop_y_distance_s_x = 0.2035f; // m
            LongReinTop_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, Convert.ToInt32(LongReinTop_y_No), (float)Convert.ToDouble(LongReinTop_y_Phi) * 0.001f, ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            //LongReinBottom_x_No = "5";
            //LongReinBottom_x_No = GetDefaultNumberOfReinforcingBars(FootingPadSize_y_Or_b, (float)Convert.ToDouble(LongReinBottom_x_Phi) * 0.001f, ConcreteCover * 0.001f).ToString();
            //LongReinBottom_x_Phi = "12";
            //LongReinBottom_x_Phi = 0.012f; // bar diameter mm to m
            //LongReinBottom_x_distance_s_y = 0.2035f; // m
            LongReinBottom_x_distance_s_y = GetDistanceBetweenReinforcementBars(FootingPadSize_y_Or_b, Convert.ToInt32(LongReinBottom_x_No), (float)Convert.ToDouble(LongReinBottom_x_Phi) * 0.001f, ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            //LongReinBottom_y_No = "5";
            //LongReinBottom_y_No = GetDefaultNumberOfReinforcingBars(FootingPadSize_x_Or_a, (float)Convert.ToDouble(LongReinBottom_y_Phi) * 0.001f, ConcreteCover * 0.001f).ToString();
            //LongReinBottom_y_Phi = "12";
            //LongReinBottom_y_Phi = 0.012f; // bar diameter mm to m
            //LongReinBottom_y_distance_s_x = 0.2035f; // m
            LongReinBottom_y_distance_s_x = GetDistanceBetweenReinforcementBars(FootingPadSize_x_Or_a, Convert.ToInt32(LongReinBottom_y_No), (float)Convert.ToDouble(LongReinBottom_y_Phi) * 0.001f, ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            // Update reference bars control points
            // Meni sa vtedy ak sa zmeni cover alebo priemer tyce
            // Prevezmeme hodnoty z GUI a previeme zo stringu na cislo v metroch
            float fConcreteCover = ConcreteCover / 1000f;
            float fDiameterTop_Bar_x = float.Parse(LongReinTop_x_Phi) / 1000f;
            float fDiameterTop_Bar_y = float.Parse(LongReinTop_y_Phi) / 1000f;
            float fDiameterBottom_Bar_x = float.Parse(LongReinBottom_x_Phi) / 1000f;
            float fDiameterBottom_Bar_y = float.Parse(LongReinBottom_y_Phi) / 1000f;

            CPoint cp_Top_x = new CPoint(1, fConcreteCover, fConcreteCover + 0.5f * fDiameterTop_Bar_x, m_FootingPadSize_z_Or_h - fConcreteCover - fDiameterTop_Bar_y - 0.5f * fDiameterTop_Bar_x, 0);
            CPoint cp_Top_y = new CPoint(2, fConcreteCover + 0.5f * fDiameterTop_Bar_y, fConcreteCover, m_FootingPadSize_z_Or_h - fConcreteCover - 0.5f * fDiameterTop_Bar_y, 0);
            CPoint cp_Bottom_x = new CPoint(3, fConcreteCover, fConcreteCover + 0.5f * fDiameterBottom_Bar_x, fConcreteCover + fDiameterBottom_Bar_y + 0.5f * fDiameterBottom_Bar_x, 0);
            CPoint cp_Bottom_y = new CPoint(4, fConcreteCover + 0.5f * fDiameterBottom_Bar_y, fConcreteCover, fConcreteCover + 0.5f * fDiameterBottom_Bar_y, 0);

            // Regenerate reinforcement bars
            foreach (CFoundation pad in listOfSelectedTypePads)
            {
                // For each pad recalculate lengths of reference bars
                float fLength_Bar_x = m_FootingPadSize_x_Or_a - 2 * ConcreteCover / 1000f;
                pad.Reference_Top_Bar_x.m_fDim2 = fLength_Bar_x;
                pad.Reference_Bottom_Bar_x.m_fDim2 = fLength_Bar_x;

                float fLength_Bar_y = m_FootingPadSize_y_Or_b - 2 * ConcreteCover / 1000f;
                pad.Reference_Top_Bar_y.m_fDim2 = fLength_Bar_y;
                pad.Reference_Bottom_Bar_y.m_fDim2 = fLength_Bar_y;

                // For each pad set for all reference bars current control point
                pad.Reference_Top_Bar_x.m_pControlPoint = cp_Top_x;
                pad.Reference_Top_Bar_y.m_pControlPoint = cp_Top_y;
                pad.Reference_Bottom_Bar_x.m_pControlPoint = cp_Bottom_x;
                pad.Reference_Bottom_Bar_y.m_pControlPoint = cp_Bottom_y;

                // Create sets of reinforcement bars
                pad.CreateReinforcementBars();
            }

            IsSetFromCode = false;
        }

        private float GetDistanceBetweenReinforcementBars(float footingPadWidth, int iNumberOfBarsPerSection, float fBarDiameter, float fConcreteCover)
        {
            // Odpocitavam 3 priemery, kedze sa ocakavaju aj zvisle casti prutov, ak je vystuz len horizontalna ma sa odpocitat len jeden priemer
            return (footingPadWidth - 2 * fConcreteCover - /*3 **/ fBarDiameter) / (iNumberOfBarsPerSection - 1);
        }

        private void GetDefaultFootingPadSize(out float faX, out float fbY, out float fhZ)
        {
            if (FootingPadMemberTypeIndex <= 1)
            {
                // Main or edge frame column (0 and 1)
                faX = (float)Math.Round(MathF.Max(0.6f, Math.Min(_pfdVM.GableWidth * 0.08f, _pfdVM.fL1 * 0.40f)), 1);
                fbY = (float)Math.Round(MathF.Max(0.6f, Math.Min(_pfdVM.GableWidth * 0.07f, _pfdVM.fL1 * 0.35f)), 1);
                fhZ = 0.4f;
            }
            else // Front a back side wind posts (2 and 3)
            {
                float fDist_Column;

                // Pripravene pre rozne rozostupy wind post na prednej a zadnej strane budovy
                if (FootingPadMemberTypeIndex == 2) // Front Side
                    fDist_Column = _pfdVM.ColumnDistance;
                else // Back Side
                    fDist_Column = _pfdVM.ColumnDistance;

                // Front or back side - wind posts
                faX = (float)Math.Round(MathF.Max(0.5f, fDist_Column * 0.40f), 1);
                fbY = (float)Math.Round(MathF.Max(0.5f, fDist_Column * 0.40f), 1);
                fhZ = 0.4f;
            }
        }

        private int GetDefaultNumberOfReinforcingBars(float footingPadWidth, float fBarDiameter, float fConcreteCover)
        {
            // Pre priblizne urcenie poctu vyztuznych prutov pouzijeme ich defaultnu vzdialenost 150 mm medzi stredmi tyci
            float fDefaultDistanceBetweenReinforcementBars = 0.15f; // 150 mm

            // Number of spacings + 1
            return (int)((footingPadWidth - 2 * fConcreteCover - 3 * fBarDiameter) / fDefaultDistanceBetweenReinforcementBars) + 1;
        }

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