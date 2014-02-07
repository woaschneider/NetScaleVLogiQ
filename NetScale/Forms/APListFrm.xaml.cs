using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using NetScaleGlobal;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WPFBusinessWindow1 Class
    /// </summary>
    public partial class APFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private AP boAP;

        private string _rolle;

        public int uRet;

        public APFrm()
        {
            boAP = new AP();
            //   RegisterBizObj(boAP);
            this.InitializeComponent();
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boAP.GetAllAP();
            this.MouseWheel += new MouseWheelEventHandler(this_mouseWheel);
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        // Methode überladen
        // 1. alle Adressen
        // 2. mit Partnerrolle
        public APFrm(string matchcode)
        {
            boAP = new AP();
            //  RegisterBizObj(boAP);
            this.InitializeComponent();
            dataGrid1.SelectedValuePath = "PK";
            txtSearch.Text = matchcode;
            dataGrid1.ItemsSource = boAP.GetAllAPByMatchCode(txtSearch.Text.Trim());
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        public APFrm(string matchcode, string Rolle)
        {
            _rolle = Rolle;
            boAP = new AP();
            //  RegisterBizObj(boAP);
            this.InitializeComponent();
            Title = Partnerrollen.GetRollenBezeichnung(Rolle);
            dataGrid1.SelectedValuePath = "PK";
            txtSearch.Text = matchcode;
            dataGrid1.ItemsSource = boAP.GetAllAPByMatchCode(txtSearch.Text.Trim(), _rolle);
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
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

        private new void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedItem = dataGrid1.Items[0];
            dataGrid1.Focus();

            KBDown();
            KBDown();
            WindowExtensions.HideCloseButton(this);
            ////
        }

        private void this_mouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                dataGrid1.FontSize = dataGrid1.FontSize + 1;
            }
            else
            {
                dataGrid1.FontSize = dataGrid1.FontSize - 1;
            }
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boAP.GetAllAPByMatchCode(txtSearch.Text.Trim(),_rolle);
        }

        private void txtSearch_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            dataGrid1.FontSize = 12;
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {  if(Partnerrollen.IsRolleEdit(_rolle)==false)
            return;

            APEditFrm oAPEditFrm = new APEditFrm(0, true,_rolle);
            oAPEditFrm.ShowDialog();
            oAPEditFrm.Close();
            Fillgrid();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            if (Partnerrollen.IsRolleEdit(_rolle) == false)
                return;

            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)
                return;

            APEditFrm oAPEditFrm = new APEditFrm(uRet, false,_rolle);
            oAPEditFrm.ShowDialog();
            oAPEditFrm.Close();
            Fillgrid();
        }

        private void Fillgrid()
        {
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boAP.GetAllAPByMatchCode(txtSearch.Text.Trim(),_rolle);
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }


        private void dataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }
    }
}