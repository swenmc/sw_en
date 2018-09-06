using System;
using System.Collections.Generic;

namespace BaseClasses
{
    public class CSystemComponent<T>
    {
        private string MName;
        private List<T> MSeries;

        public string Name
        {
            get
            {
                return MName;
            }

            set
            {
                MName = value;
            }
        }

        public List<T> Series
        {
            get
            {
                if (MSeries == null) MSeries = new List<T>();
                return MSeries;
            }

            set
            {
                MSeries = value;
            }
        }

        public CSystemComponent(string name)
        {
            MName = name;            
        }

        public CSystemComponent(string name, List<T> series)
        {
            MName = name;
            MSeries = series;        
        }

        
    }
}
