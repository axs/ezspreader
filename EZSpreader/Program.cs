using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EZSpreader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string cfg = "Default.xml";
            if(args.Length>0){
                cfg = args[0];
            }

            Application.Run(new EZSpreader(cfg));
        }
    }
}