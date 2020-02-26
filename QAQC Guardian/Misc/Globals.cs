using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace QAQC_Guardian.Misc
{
    public class Globals
    {
        public static string PathExe = AppDomain.CurrentDomain.BaseDirectory;
        //public static string PathExe = "S:\\Maint\\QAQC\\QAQC Guardian";
        public static string PathFiles = Path.GetFullPath(Path.Combine(PathExe, "..", "Output Files\\"));
        public static string PathConfig = Path.GetFullPath(Path.Combine(PathExe, "..", "Configuration\\"));
        public static string PathConfigBackup = Path.GetFullPath(Path.Combine(PathExe, "..", "Configuration\\", "Backups\\"));
        public static string SettingsFileName = "gold";
        public static string SettingsFile = SettingsFileName + ".json";
        public static string LockFile = SettingsFileName + ".json.lock";
        public static bool Locked = false;
        public static bool ChangesMade = false;

        public static void ShowMsg(string text, string caption, MessageBoxButton mbbutton, MessageBoxImage mbimage)
        {
            MessageBox.Show(text, caption, mbbutton, mbimage);

            try
            {
                // Check that the folder exists
                if (!System.IO.Directory.Exists(Globals.PathFiles))
                    System.IO.Directory.CreateDirectory(Globals.PathFiles);

                // Assemble output filename and path
                string targetFile = $"log_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.txt";
                string targetPath = PathFiles + targetFile;

                // Create new log file and add message
                if (!File.Exists(targetPath))
                {
                    using (StreamWriter sw = File.CreateText(targetPath))
                        sw.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"While trying to write the error to a logfile, another error was experienced.\n\nError:\n{e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public static void CheckPaths()
        {
            if (PathExe.ToLower().Contains("desktop"))
            {
                MessageBox.Show("It appears that you copied the application to the Desktop. This will not work. Please make a shortcut" +
                    " to the application instead of copying and pasting the application to your Desktop.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Shut down... somehow...
                System.Windows.Application.Current.Shutdown(); // doesn't work? idk
                System.Environment.Exit(1);
            }
        }
    }
}
