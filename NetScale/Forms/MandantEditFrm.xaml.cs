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
using MessageBox = System.Windows.MessageBox;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// MandantEditFrm Class
    /// </summary>
    public partial class MandantEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        private Mandant boM;

        private MandantEntity boME;

        public MandantEditFrm(int PK, bool New)
        {
            boM = new Mandant();
            this.InitializeComponent();
            if (New == true)
            {
                this.Title = "Neuen Mandanten anlegen";
                boME = boM.NewEntity();
            }
            else
            {
                this.Title = "Mandant bearbeiten";
                //   this.Title = "Empfänger bearbeiten!";
                boME = boM.GetMandantByPK(PK);
            }

            this.DataContext = boME;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_CatchFocus.Focus();
            // Focus auf eine nicht sichtbare Textbox setzen. Sonst werden die Änderungen an der letzen TB nicht übernommen. Trick!
            boM.SaveEntity();
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boM.DeleteEntity();
                Hide();
            }
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boM.Cancel();
            this.Hide();
        }
    }
}