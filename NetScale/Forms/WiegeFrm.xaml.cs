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
using HWB.NETSCALE.FRONTEND.WPF.Import;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WiegeFrm Class
    /// </summary>
    public partial class WiegeFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WiegeFrm()
        {
            this.InitializeComponent();
        }




        #region Ribbon Schaltflächen Methoden


        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        #endregion 

        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            new ImportPolos().Import(this);
        }

        private void cmdArtikel_Click(object sender, RoutedEventArgs e)
        {
            ArtikelListFrm oAListFrm = new ArtikelListFrm("");
            oAListFrm.ShowDialog();
            oAListFrm.Close();
        }

        private void cmdWarenart_Click(object sender, RoutedEventArgs e)
        {
            WarenartListFrm oWarenartFrm = new WarenartListFrm("");
            oWarenartFrm.ShowDialog();
            oWarenartFrm.Close();
        }

        private void cmdProdukte_Click(object sender, RoutedEventArgs e)
        {
            ProdukteListFrm oPFrm = new ProdukteListFrm("");
            oPFrm.ShowDialog();
            oPFrm.Close();
        }

        private void cmdArtikelAttribute_Click(object sender, RoutedEventArgs e)
        {
            ArtikelAttributeListFrm oA = new ArtikelAttributeListFrm("");
            oA.ShowDialog();
            oA.Close();
        }

        private void cmdLagerPlaetze_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdAdresseb_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdAdressen_Click(object sender, RoutedEventArgs e)
        {
            AdressenListeFrm oA = new AdressenListeFrm("");
            oA.ShowDialog();
            if (oA != null)
            {
                oA.Close();
            }
        }



    }
}