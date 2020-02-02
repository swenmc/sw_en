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
    public class CComponentInfo : INotifyPropertyChanged
    {
        //todo dokodit IsSetFromCode a filtrovat eventy ktore sa posielaju hore, napr. ked sa zmeni generate aj ine checkboxy...tak GUI sa musi updatovat ale eventy hore nemaju chodit,
        //lebo potom sa UpdateAll() vola vela krat
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool MIsSetFromCode;

        private string MPrefix;
        private Helpers.CComboColor MColor;
        private string MComponentName;
        private EMemberType_FS_Position MMemberTypePosition;
        private string MSection;
        private string MILS;
        private string MSectionColor;
        private List<string> MSections;
        private List<string> MILS_Items;
        private List<Helpers.CComboColor> MColors;
        private string MMaterial;
        private bool? MGenerate;
        private bool MGenerateIsReadonly;
        private bool MGenerateIsEnabled;
        private bool MGenerateIsThreeState;
        private bool MDisplay;
        private bool MCalculate;
        private bool MIsCalculateEnabled;
        private bool MDesign;
        private bool MIsDesignEnabled;
        private bool MMaterialList;

        public string Prefix
        {
            get
            {
                return MPrefix;
            }

            set
            {
                MPrefix = value;
            }
        }

        public string ComponentName
        {
            get
            {
                return MComponentName;
            }

            set
            {
                MComponentName = value;
            }
        }

        public string Section
        {
            get
            {
                return MSection;
            }

            set
            {
                if (MSection != value)
                {
                    MSection = value;
                    IsSetFromCode = true;
                    SetComponentSectionColor();
                    IsSetFromCode = false;
                    NotifyPropertyChanged("Section");
                }
            }
        }

        public string Material
        {
            get
            {
                return MMaterial;
            }

            set
            {
                MMaterial = value;
                NotifyPropertyChanged("Material");
            }
        }

        public bool? Generate
        {
            get
            {
                return MGenerate;
            }

            set
            {
                if (value == false && !ValidateGenerateCouldBeChanged()) return;
                bool propertyWasChanged = MGenerate != value;

                MGenerate = value;
                if (MGenerate == null) return;

                bool isChangedFromCode = IsSetFromCode;
                
                if (propertyWasChanged)
                {
                    if(!isChangedFromCode) IsSetFromCode = true;
                    Display = MGenerate.Value;
                    Calculate = MGenerate.Value;
                    Design = MGenerate.Value;
                    MaterialList = MGenerate.Value;
                    if (!isChangedFromCode) IsSetFromCode = false;
                }
                NotifyPropertyChanged("Generate");
            }
        }
        public bool GenerateIsReadonly
        {
            get
            {
                return MGenerateIsReadonly;
            }

            set
            {
                MGenerateIsReadonly = value;
            }
        }
        public bool GenerateIsEnabled
        {
            get
            {
                return MGenerateIsEnabled;
            }

            set
            {
                MGenerateIsEnabled = value;
                NotifyPropertyChanged("GenerateIsEnabled");
            }
        }
        public bool GenerateIsThreeState
        {
            get
            {
                return MGenerateIsThreeState;
            }

            set
            {
                MGenerateIsThreeState = value;
            }
        }

        public bool Display
        {
            get
            {
                return MDisplay;
            }

            set
            {
                MDisplay = value;
                if (MGenerate.HasValue && !MGenerate.Value && MDisplay)
                    MDisplay = false;

                NotifyPropertyChanged("Display");
            }
        }

        public bool Calculate
        {
            get
            {
                return MCalculate;
            }

            set
            {
                MCalculate = value;
                if (MGenerate.HasValue && !MGenerate.Value && MCalculate)
                    MCalculate = false;

                if (!MCalculate)
                {
                    Design = false;
                }

                NotifyPropertyChanged("Calculate");
            }
        }
        public bool IsCalculateEnabled
        {
            get
            {
                return MIsCalculateEnabled;
            }

            set
            {
                MIsCalculateEnabled = value;
            }
        }

        public bool Design
        {
            get
            {
                return MDesign;
            }

            set
            {
                MDesign = value;
                if (MGenerate.HasValue && !MGenerate.Value && MDesign)
                    MDesign = false;

                if (!MCalculate && MDesign)
                    MDesign = false;

                NotifyPropertyChanged("Design");
            }
        }
        public bool IsDesignEnabled
        {
            get
            {
                return MIsDesignEnabled;
            }

            set
            {
                MIsDesignEnabled = value;
            }
        }

        public bool MaterialList
        {
            get
            {
                return MMaterialList;
            }

            set
            {
                MMaterialList = value;
                if (MGenerate.HasValue && !MGenerate.Value && MMaterialList)
                    MMaterialList = false;

                NotifyPropertyChanged("MaterialList");
            }
        }

        public List<string> Sections
        {
            get
            {
                return MSections;
            }

            set
            {
                MSections = value;
            }
        }
        public List<Helpers.CComboColor> Colors
        {
            get
            {
                return MColors;
            }

            set
            {
                MColors = value;
            }
        }

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

        public EMemberType_FS_Position MemberTypePosition
        {
            get
            {
                return MMemberTypePosition;
            }

            set
            {
                MMemberTypePosition = value;
            }
        }

        public Helpers.CComboColor Color
        {
            get
            {
                return MColor;
            }

            set
            {
                MColor = value;
                NotifyPropertyChanged("Color");
            }
        }

        public string SectionColor
        {
            get
            {
                return MSectionColor;
            }

            set
            {
                MSectionColor = value;
                NotifyPropertyChanged("SectionColor");
            }
        }

        public string ILS
        {
            get
            {
                return MILS;
            }

            set
            {
                MILS = value;
                NotifyPropertyChanged("ILS");
            }
        }

        public List<string> ILS_Items
        {
            get
            {
                return MILS_Items;
            }

            set
            {
                MILS_Items = value;
                NotifyPropertyChanged("ILS_Items");
            }
        }

        public CComponentInfo(string prefix, Helpers.CComboColor color, string componentName, string section, string sectionColor, string material, string ils, bool? generate, bool display, 
            bool calculate, bool design, bool materialList, List<string> sections, List<string> ilsItems, List<BaseClasses.Helpers.CComboColor> colors, EMemberType_FS_Position memberType)
        {
            MIsSetFromCode = false;
            MPrefix = prefix;
            MColor = color;
            MComponentName = componentName;
            MSection = section;
            MSectionColor = sectionColor;
            MMaterial = material;
            MILS = ils;
            MGenerate = generate;
            MDisplay = display;
            MCalculate = calculate;
            MDesign = design;
            MMaterialList = materialList;
            MSections = sections;
            MILS_Items = ilsItems;
            MColors = colors;
            MMemberTypePosition = memberType;

            SetGenerateIsReadonly();
            SetCalculateIsEnabled();
            SetDesignIsEnabled();
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ValidateGenerateCouldBeChanged()
        {
            //Main Columns, Main Rafters , Edge Columns, Edge Rafters a Eave Purlins by sa mali generovat vzdy, 
            //takze nebude mozne checkbox Generate vypnut (pre MC, MR, EC, ER a EP disablovat editaciu checboxu generate, vzdy musi byt true)
            if (MPrefix == "MC" || MPrefix == "MR" || MPrefix == "EC" || MPrefix == "ER" || MPrefix == "EP" || MPrefix == "DF" || MPrefix == "WF" || MPrefix == "DT" || MPrefix == "DL") return false;
            else return true;
        }

        private void SetGenerateIsReadonly()
        {
            if (MPrefix == "MC" || MPrefix == "MR" || MPrefix == "EC" || MPrefix == "ER" || MPrefix == "EP" || MPrefix == "DF" || MPrefix == "WF" || MPrefix == "DT" || MPrefix == "DL") GenerateIsReadonly = true;
            else GenerateIsReadonly = false;

            GenerateIsEnabled = !GenerateIsReadonly;

            if (MPrefix == "DF" || MPrefix == "WF" || MPrefix == "DT" || MPrefix == "DL") GenerateIsThreeState = true;
            else GenerateIsThreeState = false;
        }
        private void SetCalculateIsEnabled()
        {
            if (MPrefix == "DF" || MPrefix == "WF" || MPrefix == "DT" || MPrefix == "DL" || MPrefix == "GB" || MPrefix == "PB") IsCalculateEnabled = false;
            else IsCalculateEnabled = true;
        }
        private void SetDesignIsEnabled()
        {
            if (MPrefix == "DF" || MPrefix == "WF" || MPrefix == "DT" || MPrefix == "DL" || MPrefix == "GB" || MPrefix == "PB") IsDesignEnabled = false;
            else IsDesignEnabled = true;
        }

        private void SetComponentSectionColor()
        {
            CrScProperties prop = CSectionManager.GetSectionProperties(Section);
            SectionColor = prop.colorName;
        }

        public bool IsFrameMember()
        {
            if (MemberTypePosition == EMemberType_FS_Position.MainColumn || MemberTypePosition == EMemberType_FS_Position.MainRafter ||
                MemberTypePosition == EMemberType_FS_Position.EdgeColumn || MemberTypePosition == EMemberType_FS_Position.EdgeRafter)
            {
                return true;
            }
            else return false;
        }
    }
}
