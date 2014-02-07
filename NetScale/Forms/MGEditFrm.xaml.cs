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
    /// MGEditFrm Class
    /// </summary>
    public partial class MGEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        private MG boMG;

        private MGEntity boMGE;

        public MGEditFrm(int PK, bool New)
        {
            boMG = new MG();

            this.InitializeComponent();
            if (New == true)
            {
                this.Title = "Neue Sorte anlegen";
                boMGE = boMG.NewEntity();
            }
            else
            {
                this.Title = "Sorte bearbeiten";
                boMGE = boMG.GetMGById(PK);
            }
            this.DataContext = boMGE;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);

            cb_ME.Items.Add("t");
            cb_ME.Items.Add("Stk");
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
            boMG.SaveEntity();
        }


        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boMG.DeleteEntity();
                this.Hide();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boMG.Cancel();
            this.Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }
    }
}