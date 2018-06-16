using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EC5
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            eCH_EC5 num = eCH_EC5.eCH_EC5_1101;

            if (num == eCH_EC5.eCH_EC5_1101) 
            {
                CH_EC5_1101 obj = new CH_EC5_1101();
                Console.WriteLine(obj.ToString());
                Console.ReadLine();
            }
            
        }
    }
}
