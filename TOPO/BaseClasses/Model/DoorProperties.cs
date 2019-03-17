using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class DoorProperties
    {
        private string m_sBuildingSide;
        private int m_iBayNumber;
        private string m_sDoorType;
        private float m_fDoorsHeight;
        private float m_fDoorsWidth;
        private float m_fDoorCoordinateXinBlock;

        public string sBuildingSide
        {
            get
            {
                return m_sBuildingSide;
            }

            set
            {
                m_sBuildingSide = value;
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
                m_fDoorsHeight = value;
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
                m_fDoorsWidth = value;
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
                m_fDoorCoordinateXinBlock = value;
            }
        }

        public DoorProperties() { }
    }
}
