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
using HWB.NETSCALE.BOEF.JoinClasses;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{


    /// <summary>
    /// AuftragEditFrm Class
    /// </summary>
    public partial class AuftragEditFrm : mmBusinessWindow
    {
        public int uRet;
        KK boKK;
        private Auftragsdetailliste boADLE;
        /// <summary>
        /// Constructor
        /// </summary>
        public AuftragEditFrm(int kkpk)
        {
            this.InitializeComponent();
            boKK = new KK();
            var dk = boKK.GetAuftragsDetailliste(kkpk);

            // 
            // Nichts in der Liste. Liste gleich schließen.
            if(dk.Count==0)
            {
                uRet = 0;
                this.Hide();
            }

            // Einen Record in der Liste. Liste gleich schließen
            if (dk.Count == 1)
            {
                uRet = dk[0].kmpk;
            
            }
            //
            DataContext = dk;
            dataGrid1.SelectedValuePath = "kmpk";
            dataGrid1.ItemsSource = dk;
           
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);


        }
        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
                e.Handled = true;
            }
            if (e.Key == Key.Return)
            {

                cmdSelect_Click(cmdSelect, e);
                Hide();
                e.Handled = true;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();

        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}