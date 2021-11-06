using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImViewLite.Helpers;
using ImViewLite.Settings;

namespace ImViewLite
{
    static class Program
    {
        public static string BaseDirectory;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // because we're changing the cwd every time a new directory is loaded
            // gotta keep the base path of the exe as a directory for save files / tmp files
            BaseDirectory = AppContext.BaseDirectory;
            Directory.SetCurrentDirectory(BaseDirectory);

            InternalSettings.EnableWebPIfPossible();
            SettingsLoader.Load();

            Application.Run(new MainForm());

            // reset the directory
            Directory.SetCurrentDirectory(BaseDirectory);

            SettingsLoader.Save();

            if (InternalSettings.Delete_Temp_Directory)
                PathHelper.DeleteFileOrPath(InternalSettings.Temp_Image_Folder);
        }
    }
}
