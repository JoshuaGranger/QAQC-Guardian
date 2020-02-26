using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QAQC_Guardian.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Misc.Globals.Locked)
            {
                // Changes made
                if (Misc.Globals.ChangesMade)
                {
                    var result = MessageBox.Show("Unsaved changes detected! Are you sure you wish to exit?\n\nBe sure to select Save All Changes on the bottom right of the screen after making changes.",
                        "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    // User selected "no" -> cancel exit
                    if (result == MessageBoxResult.No)
                        e.Cancel = true;

                    // User selected "yes" -> delete lock file
                    else
                        System.IO.File.Delete(Misc.Globals.PathConfig + Misc.Globals.LockFile);
                }

                // Changes not made, don't prompt
                // Delete lock file
                else
                    System.IO.File.Delete(Misc.Globals.PathConfig + Misc.Globals.LockFile);
            }
        }
    }
}
