using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using OakLeaf.MM.Main.WPF;
using NetScalePolosIO;

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

        #region Fields 
        private ImportExportPolos _oIO;

        #endregion
        public MainWindow()
        {
            InitializeComponent();
            _oIO =  new ImportExportPolos();
            DataContext = _oIO;
            _oIO.IOStatusHasChanged += IOStatusHasChanged;
        }

        private void IOStatusHasChanged(object sender, EventArgs e)
        {
            
          
            if (!_oIO.ExportIsRunning && !_oIO.ImportAuftrageIsRunning && !_oIO.ImportStammdatenIsRunning)
            {
                _oIO.IOStatusHasChanged -= IOStatusHasChanged;
                _oIO = null;
                Thread.Sleep(2000);
                Application.Current.Dispatcher.Invoke(new Action(() => { Close();}));
            
      
            }
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

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            _oIO.ExportAll();
            _oIO.ImportStammdaten();
            _oIO.ImportAuftraege(false);
        }
    }
}
