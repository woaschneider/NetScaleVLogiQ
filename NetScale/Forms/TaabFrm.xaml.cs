using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// TaabFrm Class
    /// </summary>
    public partial class TaabFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TaabFrm()
        {
            this.InitializeComponent();
            TaabDatePicker.SelectedDate = System.DateTime.Now;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }
        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }
        private void cmdExport_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(DateTime.Today.ToString().Substring(0, 10)))
            {
              if(  MessageBox.Show(
                    "Es wurde heute bereits ein Tagesabschluss durchgeführt. Sind Sie sicher, dass sie diesen Vorgang wiederholen wollen?",
                    "Meldung", MessageBoxButton.YesNo)==MessageBoxResult.No)
                return;
            }
            VFP.StrToFile("taab",DateTime.Today.ToString().Substring(0,10));
            Waege _boW = new Waege();
            var uRet =  _boW.TaabOAM(TaabDatePicker.SelectedDate.Value);
          MessageBox.Show("Export fertig. Es wurden "+ uRet.ToString()+ " Datensätze exportiert","Meldung",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            Hide();
        }

       new  private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }
    }
}