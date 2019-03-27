using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CComponentInfo : INotifyPropertyChanged
    {
        //todo dokodit IsSetFromCode a filtrovat eventy ktore sa posielaju hore, napr. ked sa zmeni generate aj ine checkboxy...tak GUI sa musi updatovat ale eventy hore nemaju chodit,
        //lebo potom sa UpdateAll() vola vela krat

        public event PropertyChangedEventHandler PropertyChanged;

        private bool MIsSetFromCode;

        private string MPrefix;
        private string MComponentName;
        private string MSection;
        private List<string> MSections;
        private string MMaterial;
        private bool MGenerate;
        private bool MGenerateIsReadonly;
        private bool MDisplay;
        private bool MCalculate;
        private bool MDesign;
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
                MSection = value;
                NotifyPropertyChanged("Section");
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

        public bool Generate
        {
            get
            {
                return MGenerate;
            }

            set
            {
                if (value == false && !ValidateGenerateCouldBeChanged()) return;
                
                MGenerate = value;
                if (!MGenerate)
                {
                    IsSetFromCode = true;
                    Display = false;
                    Calculate = false;
                    Design = false;
                    MaterialList = false;
                    IsSetFromCode = false;
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

        public bool Display
        {
            get
            {
                return MDisplay;
            }

            set
            {
                MDisplay = value;
                if (!MGenerate && MDisplay)                                    
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
                if (!MGenerate && MCalculate)
                    MCalculate = false;

                if (!MCalculate)
                {
                    Design = false;
                }

                NotifyPropertyChanged("Calculate");
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
                if (!MGenerate && MDesign)
                    MDesign = false;

                if (!MCalculate && MDesign)
                    MDesign = false;

                NotifyPropertyChanged("Design");
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
                if (!MGenerate && MMaterialList)
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

        public bool IsSetFromCode
        {
            get
            {
                return MIsSetFromCode;
            }

            set
            {
                MIsSetFromCode = value;
            }
        }

        

        public CComponentInfo(string prefix, string componentName, string section, string material, bool generate, bool display, bool calculate, bool design, bool materialList, List<string> sections)
        {
            MIsSetFromCode = false;
            MPrefix = prefix;
            MComponentName = componentName;
            MSection = section;
            MMaterial = material;
            MGenerate = generate;
            MDisplay = display;
            MCalculate = calculate;
            MDesign = design;
            MMaterialList = materialList;
            MSections = sections;

            SetGenerateIsReadonly();
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool ValidateGenerateCouldBeChanged()
        {
            //Main Columns, Main Rafters , Edge Columns, Edge Rafters a Eave Purlins by sa mali generovat vzdy, 
            //takze nebude mozne checkbox Generate vypnut (pre MC, MR, EC, ER a EP disablovat editaciu checboxu generate, vzdy musi byt true)
            if (MPrefix == "MC" || MPrefix == "MR" || MPrefix == "EC" || MPrefix == "ER" || MPrefix == "EP") return false;
            else return true;
        }

        private void SetGenerateIsReadonly()
        {
            if (MPrefix == "MC" || MPrefix == "MR" || MPrefix == "EC" || MPrefix == "ER" || MPrefix == "EP") GenerateIsReadonly = true;
            else GenerateIsReadonly = false;            
        }
    }
}
