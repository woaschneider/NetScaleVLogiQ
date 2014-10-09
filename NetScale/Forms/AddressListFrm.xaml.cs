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
    public partial class AddressFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Adressen boAddress;

        private string _rolle;

        public int uRet;

        public AddressFrm()
        {
            this.MouseWheel += new MouseWheelEventHandler(this_mouseWheel);
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        // Methode überladen
        // 1. alle Adressen
        // 2. mit Partnerrolle
        public AddressFrm(string matchcode)
        {
            boAddress = new Adressen();
            //  RegisterBizObj(boAddress);
            this.InitializeComponent();
            dataGrid1.SelectedValuePath = "PK";
            txtSearch.Text = matchcode;

            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        public AddressFrm(string matchcode, string Rolle)
        {
            _rolle = Rolle;
            boAddress = new Adressen();
            //  RegisterBizObj(boAddress);
            this.InitializeComponent();
            Title = Partnerrollen.GetRollenBezeichnung(Rolle);
            dataGrid1.SelectedValuePath = "PK";
            txtSearch.Text = matchcode;

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
        }

        private void txtSearch_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            dataGrid1.FontSize = 12;
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
            // if(Partnerrollen.IsRolleEdit(_rolle)==false)
            //    return;

            AddressEditFrm oAddressEditFrm = new AddressEditFrm(0, true, _rolle);
            oAddressEditFrm.ShowDialog();
            oAddressEditFrm.Close();
            Fillgrid();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)
                return;

            AddressEditFrm oAddressEditFrm = new AddressEditFrm(uRet, false, _rolle);
            oAddressEditFrm.ShowDialog();
            oAddressEditFrm.Close();
            Fillgrid();
        }

        private void Fillgrid()
        {
            dataGrid1.SelectedValuePath = "PK";
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