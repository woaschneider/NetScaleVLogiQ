using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            DataContext = dk;
            dataGrid1.ItemsSource = dk;

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
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}