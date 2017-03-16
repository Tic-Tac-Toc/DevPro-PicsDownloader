using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicsDownloader
{
    static class Program
    {
        public static Download_frm Frm { get; private set; }
        public static Downloader Downloader { get; private set; }

        public static string AppPath = Application.StartupPath;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Downloader = new Downloader();
            Frm = new Download_frm();
            Application.Run(Frm);
        }
    }
}
