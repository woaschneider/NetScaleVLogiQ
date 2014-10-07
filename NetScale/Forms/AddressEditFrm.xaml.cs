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
    /// AddressEditFrm Class
    /// </summary>
    public partial class AddressEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Address boAddress;

        private AddressEntity boAPE;

        public AddressEditFrm(int PK, bool New, string _rolle)
        {
            boAddress = new Address();
            this.InitializeComponent();
            if (New == true)
            {
                this.Title = Partnerrollen.GetRollenBezeichnung(_rolle) + " - Neue Adresse anlegen";
                boAPE = boAddress.NewEntity();


            }
            else
            {
                this.Title = Partnerrollen.GetRollenBezeichnung(_rolle) + " - Adresse bearbeiten";
                //   this.Title = "Empfänger bearbeiten!";
             //   boAPE = boAddress.GetAPById(PK);
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
            boAddress.SaveEntity();
            this.Hide();

        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boAddress.DeleteEntity();
                this.Hide();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boAddress.Cancel();
            this.Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }
    }
}