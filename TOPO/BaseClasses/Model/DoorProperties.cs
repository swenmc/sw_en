using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    [Serializable]
    public class DoorProperties : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_sBuildingSide;
        private int m_iBayNumber;
        private List<int> m_Bays;
        private string m_sDoorType;
        private float m_fDoorsHeight;
        private float m_fDoorsWidth;
        private float m_fDoorCoordinateXinBlock;

        private List<CoatingColour> m_CoatingColors;
        private CoatingColour m_coatingColor;

        private List<string> m_Series;
        private string m_Serie;
        private bool m_SerieEnabled;

        private RebateProperties m_RebateProp;
        float fRollerDoorTrimmerWidth; // Sirka cross-section typu roller door trimmer

        //validationValues
        private float m_WallHeight = float.NaN;
        private float m_L1 = float.NaN;
        private float m_distFrontColumns = float.NaN;
        private float m_distBackColumns = float.NaN;

        public bool IsSetFromCode = false;

        public string sBuildingSide
        {
            get
            {
                return m_sBuildingSide;
            }

            set
            {
                string temp = m_sBuildingSide;
                m_sBuildingSide = value;
                
                if (!ValidateDoorInsideBay())
                {
                    m_sBuildingSide = temp;
                    MessageBox.Show("Doors outside of bay width.");
                }
                else NotifyPropertyChanged("sBuildingSide");
            }
        }

        public int iBayNumber
        {
            get
            {
                return m_iBayNumber;
            }

            set
            {
                m_iBayNumber = value;
                NotifyPropertyChanged("iBayNumber");
            }
        }

        public string sDoorType
        {
            get
            {
                return m_sDoorType;
            }

            set
            {
                m_sDoorType = value;
                IsSetFromCode = true;

                if (m_sDoorType == "Roller Door")
                {

                    Series = new List<string>() { "Domestic", "Roller Shutter" };
                    Serie = "Domestic";
                    SerieEnabled = true;

                }
                else
                {
                    Series = new List<string>();
                    SerieEnabled = false;
                }

                IsSetFromCode = false;

                NotifyPropertyChanged("sDoorType");
            }
        }

        public float fDoorsHeight
        {
            get
            {
                return m_fDoorsHeight;
            }

            set
            {
                if (!float.IsNaN(m_WallHeight) && (value < 1 || value > m_WallHeight))
                {
                    MessageBox.Show($"Doors Height must be between 1 and {m_WallHeight} [m]");
                }
                else
                {
                    m_fDoorsHeight = value;
                    NotifyPropertyChanged("fDoorsHeight");
                }
                
            }
        }

        public float fDoorsWidth
        {
            get
            {
                return m_fDoorsWidth;
            }

            set
            {
                float temp = m_fDoorsWidth;
                m_fDoorsWidth = value;
                if (!ValidateDoorInsideBay()) { 
                    m_fDoorsWidth = temp;
                    MessageBox.Show("Doors outside of bay width.");
                }
                else NotifyPropertyChanged("fDoorsWidth");
            }
        }

        public float fDoorCoordinateXinBlock
        {
            get
            {
                return m_fDoorCoordinateXinBlock;
            }

            set
            {
                float temp = m_fDoorCoordinateXinBlock;
                m_fDoorCoordinateXinBlock = value;
                if (!ValidateDoorInsideBay()) {
                    m_fDoorCoordinateXinBlock = temp;
                    MessageBox.Show("Doors outside of bay width.");
                }
                else NotifyPropertyChanged("fDoorCoordinateXinBlock");
            }
        }

        public CoatingColour CoatingColor
        {
            get
            {
                return m_coatingColor;
            }

            set
            {
                m_coatingColor = value;
                NotifyPropertyChanged("CoatingColor");
            }
        }
        public List<CoatingColour> CoatingColors
        {
            get
            {
                if (m_CoatingColors == null) m_CoatingColors = CCoatingColorManager.LoadColours("AccessoriesSQLiteDB");
                return m_CoatingColors;
            }

            set
            {
                m_CoatingColors = value;
            }
        }

        public List<int> Bays
        {
            get
            {
                if (m_Bays == null) m_Bays = new List<int>();
                return m_Bays;
            }

            set
            {
                m_Bays = value;
                if(m_Bays != null) NotifyPropertyChanged("Bays");
            }
        }

        public RebateProperties RebateProp
        {
            get
            {
                return m_RebateProp;
            }

            set
            {
                m_RebateProp = value;
            }
        }

        public List<string> Series
        {
            get
            {
                if (m_Series == null)
                {
                    m_Series = new List<string>();
                }
                return m_Series;
            }

            set
            {
                m_Series = value;
                NotifyPropertyChanged("Series");
            }
        }

        public string Serie
        {
            get
            {               
                return m_Serie;
            }

            set
            {
                m_Serie = value;
                NotifyPropertyChanged("Serie");
            }
        }

        public bool SerieEnabled
        {
            get
            {
                return m_SerieEnabled;
            }

            set
            {
                m_SerieEnabled = value;
                NotifyPropertyChanged("SerieEnabled");
            }
        }

        public DoorProperties()
        {            
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Validate()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(m_sBuildingSide)) return false;
            if (string.IsNullOrEmpty(m_sDoorType)) return false;
            if (m_iBayNumber < 0f) return false;
            if (m_fDoorsHeight < 0.1f) return false;
            if (m_fDoorsWidth < 0.1f) return false;
            if (m_fDoorCoordinateXinBlock < 0.05f) return false;

            return isValid;
        }

        public bool ValidateBays()
        {
            if (iBayNumber <= Bays.Count) return true;
            else return false;
        }

        public bool ValidateDoorInsideBay()
        {
            if (sBuildingSide == "Front")
            {
                if (float.IsNaN(m_distFrontColumns)) return true;
                if (m_distFrontColumns < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            else if (sBuildingSide == "Back")
            {
                if (float.IsNaN(m_distBackColumns)) return true;
                if (m_distBackColumns < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            else if (sBuildingSide == "Left") {
                if (float.IsNaN(m_L1)) return true;
                if (m_L1 < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }
            else if (sBuildingSide == "Right")
            {
                if (float.IsNaN(m_L1)) return true;
                if (m_L1 < m_fDoorsWidth + m_fDoorCoordinateXinBlock) return false;
            }

            if(m_WallHeight < m_fDoorsHeight) return false; // Doors are higher than wall (Todo - pre prednu a zadnu stenu by bolo dobre zohladnit presne vysku v danej bay

            return true;
        }

        public void SetValidationValues(float wallHeight, float L1, float distFrontColumns, float distBackColumns)
        {
            m_WallHeight = wallHeight;
            m_L1 = L1;
            m_distFrontColumns = distFrontColumns;
            m_distBackColumns = distBackColumns;
        }

        // TODO Ondrej - duplicita parametrov s funkciou SetValidationValues
        public void SetRebateProperties(float fRollerDoorTrimmerWidth, float fRebateWidth,
                    float L1, // TODO  Ondrej - tento parameter by tu asi nemal byt, hodnoty by sa mohli nastavit skor
                    float distFrontColumns, // TODO  Ondrej - tento parameter by tu asi nemal byt, hodnoty by sa mohli nastavit skor
                    float distBackColumns // TODO  Ondrej - tento parameter by tu asi nemal byt, hodnoty by sa mohli nastavit skor
            )
        {
            m_L1 = L1;
            m_distFrontColumns = distFrontColumns;
            m_distBackColumns = distBackColumns;

            if (m_sDoorType == "Roller Door")
            {
                m_RebateProp = new RebateProperties();
                m_RebateProp.RebateWidth = fRebateWidth;

                if (m_sBuildingSide == "Right" || m_sBuildingSide == "Left")
                    m_RebateProp.RebatePosition = (m_iBayNumber - 1) * m_L1 + 0.5f * fRollerDoorTrimmerWidth + m_fDoorCoordinateXinBlock;
                else if (m_sBuildingSide == "Front")
                    m_RebateProp.RebatePosition = (m_iBayNumber - 1) * m_distFrontColumns + 0.5f * fRollerDoorTrimmerWidth + m_fDoorCoordinateXinBlock;
                else
                    m_RebateProp.RebatePosition = (m_iBayNumber - 1) * m_distBackColumns + 0.5f * fRollerDoorTrimmerWidth + m_fDoorCoordinateXinBlock;

                m_RebateProp.RebateLength = m_fDoorsWidth - fRollerDoorTrimmerWidth;
                m_RebateProp.RebateDepth_Step = 0.01f;
                m_RebateProp.RebateDepth_Edge = 0.02f;
            }
        }
    }
}
