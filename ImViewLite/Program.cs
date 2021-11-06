using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImViewLite.Controls;
using ImViewLite.Helpers;
using ImViewLite.Settings;

namespace ImViewLite
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

            InternalSettings.EnableWebPIfPossible();
            SettingsLoader.Load();

            Application.Run(new MainForm());
            SettingsLoader.Save();

            if (InternalSettings.Delete_Temp_Directory)
                PathHelper.DeleteFileOrPath(InternalSettings.Temp_Image_Folder);
        }
    }
}
