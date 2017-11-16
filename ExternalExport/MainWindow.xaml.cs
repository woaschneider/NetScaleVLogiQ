using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using OakLeaf.MM.Main.WPF;

namespace HWB.EXTERNALEXPORT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : mmMainAppWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set security setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecuritySetupMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.SecuritySetupMode = true;
        }

        /// <summary>
        /// Clear security setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecuritySetupMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.SecuritySetupMode = false;
        }

        /// <summary>
        /// Set localize setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizeSetupMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.LocalizeSetupMode = true;
        }

        /// <summary>
        /// Clear localize setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizeSetupMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.LocalizeSetupMode = false;
        }

        /// <summary>
        /// Display the Users window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mmAppWPF.WindowManager.Show(new UserWindow(), this);
        }

        /// <summary>
        /// Close the main application window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileExitItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
