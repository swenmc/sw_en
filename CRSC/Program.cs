using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CRSC
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new CRSC_Form());
            Application.Run(new CSForm()); // Run main window / form
        }
    }
}
