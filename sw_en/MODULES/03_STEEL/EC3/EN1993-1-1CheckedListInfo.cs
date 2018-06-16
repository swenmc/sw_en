using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CENEX
{
    [Serializable]
    public class CheckedListInfo
    {
        private List<string> zoznamPremennych;

        public List<string> ZoznamPremennych
        {
            get { return zoznamPremennych; }
            set { zoznamPremennych = value; }
        }
        private List<string> zoznamZobrazPremennych;

        public List<string> ZoznamZobrazPremennych
        {
            get { return zoznamZobrazPremennych; }
            set { zoznamZobrazPremennych = value; }
        }
        private List<string> zoznamJednotiek;

        public List<string> ZoznamJednotiek
        {
            get { return zoznamJednotiek; }
            set { zoznamJednotiek = value; }
        }
        private List<bool> zoznamZasktPrem;

        public List<bool> ZoznamZasktPrem
        {
            get { return zoznamZasktPrem; }
            set { zoznamZasktPrem = value; }
        }

        public CheckedListInfo()
        {
            this.zoznamPremennych = new List<string>(10);
            this.zoznamZasktPrem = new List<bool>(10);
            this.zoznamZobrazPremennych = new List<string>(10);
            this.zoznamJednotiek = new List<string>(10);
        }
    }
}
