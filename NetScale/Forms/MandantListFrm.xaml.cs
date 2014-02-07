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
    /// MandantListFrm Class
    /// </summary>
    public partial class MandantListFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        private Mandant boMandant;

        public int uRet;

        public MandantListFrm()
        {
            boMandant = new Mandant();
            this.InitializeComponent();
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boMandant.GetAllMandant();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
            if (e.Key == Key.Return)
            {
                cmdSelect_Click(cmdSelect, e);
                Hide();
            }
        }

        private void KBDown()
        {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                                                 Key.Down);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
        }


        private void dataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.Focus();

            KBDown();
            KBDown();
            WindowExtensions.HideCloseButton(this);
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
            MandantEditFrm oMEFrm = new MandantEditFrm(0, true);
            oMEFrm.ShowDialog();
            oMEFrm.Close();
            //  APEditFrm oAPEditFrm = new APEditFrm(0, true);
            // oAPEditFrm.ShowDialog();
            //  oAPEditFrm.Close();
            Fillgrid();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)
                return;

            MandantEditFrm oMEFrm = new MandantEditFrm(uRet, false);
            oMEFrm.ShowDialog();
            oMEFrm.Close();
            Fillgrid();
        }

        private void Fillgrid()
        {
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boMandant.GetAllMandant();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }
    }
}