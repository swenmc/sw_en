using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    abstract public class CObject
    {
        int m_ID; // Unique object ID

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        float m_fTime;

        public float FTime
        {
            get { return m_fTime; }
            set { m_fTime = value; }
        }

        bool m_bIsGenerated;

        public bool BIsGenerated
        {
            get { return m_bIsGenerated; }
            set { m_bIsGenerated = value; }
        }

        bool m_bIsDisplayed;

        public bool BIsDisplayed
        {
            get { return m_bIsDisplayed; }
            set { m_bIsDisplayed = value; }
        }

        bool m_bIsSelectedForIFCalculation;

        public bool BIsDSelectedForIFCalculation
        {
            get { return m_bIsSelectedForIFCalculation; }
            set { m_bIsSelectedForIFCalculation = value; }
        }

        bool m_bIsSelectedForDesign;

        public bool BIsDSelectedForDesign
        {
            get { return m_bIsSelectedForDesign; }
            set { m_bIsSelectedForDesign = value; }
        }

        bool m_bIsSelectedForMaterialList;

        public bool BIsSelectedForMaterialList
        {
            get { return m_bIsSelectedForMaterialList; }
            set { m_bIsSelectedForMaterialList = value; }
        }

        private bool m_bIsDebugging;

        public bool BIsDebugging
        {
            get { return m_bIsDebugging; }
            set { m_bIsDebugging = value; }
        }

        private string m_Name;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_Prefix;

        public string Prefix
        {
            get { return m_Prefix; }
            set { m_Prefix = value; }
        }

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CObject() { }

        //----------------------------------------------------------------------------------------------------------------
        public void Delete()
        { }

        public void Edit()
        { }

        public void Draw()
        { }

        // Refresh, modify, update, redraw, copy, changeID .....
    }
}
