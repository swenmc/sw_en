using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BaseClasses.Helpers
{
    public class ModelValidationError
    {
        private int m_ID;
        private string m_Msg;
        private string m_Description;

        public int ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
            }
        }

        public string Msg
        {
            get
            {
                return m_Msg;
            }

            set
            {
                m_Msg = value;
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }

            set
            {
                m_Description = value;
            }
        }

        public ModelValidationError() { }
        public ModelValidationError(int id, string msg, string desc)
        {
            m_ID = id;
            m_Msg = msg;
            m_Description = desc;
        }

    }
}
