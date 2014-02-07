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
using NetScaleGlobal;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// APEditFrm Class
    /// </summary>
    public partial class APEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private AP boAP;

        private APEntity boAPE;

        public APEditFrm(int PK, bool New, string _rolle)
        {
            boAP = new AP();
            this.InitializeComponent();
            if (New == true)
            {
                this.Title = Partnerrollen.GetRollenBezeichnung(_rolle) + " - Neue Adresse anlegen";
                boAPE = boAP.NewEntity();
                if (_rolle == "SP")
                    boAPE.Rolle_SP = true;
                if (_rolle == "FU")
                    boAPE.Rolle_FU = true;

            }
            else
            {
                this.Title = Partnerrollen.GetRollenBezeichnung(_rolle) + " - Adresse bearbeiten";
                //   this.Title = "Empfänger bearbeiten!";
                boAPE = boAP.GetAPById(PK);
            }
            this.DataContext = boAPE;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_CatchFocus.Focus();
            // Focus auf eine nicht sichtbare Textbox setzen. Sonst werden die Änderungen an der letzen TB nicht übernommen. Trick!
            boAP.SaveEntity();
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boAP.DeleteEntity();
                this.Hide();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boAP.Cancel();
            this.Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }
    }
}