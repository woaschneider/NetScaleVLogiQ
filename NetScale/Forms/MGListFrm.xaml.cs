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
    /// MGListFrm Class
    /// </summary>
    public partial class MGListFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        public int uRet;

        private MG boMG;
        private string MatchCode;
        public MGListFrm()
        {
            boMG = new MG();
            RegisterBizObj(boMG);
            this.InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        public MGListFrm(string mc)
        {
            MatchCode = mc;
            boMG = new MG();
            RegisterBizObj(boMG);
            this.InitializeComponent();
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

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
            MGEditFrm oMGEditFrm = new MGEditFrm(0, true);
            oMGEditFrm.ShowDialog();
            oMGEditFrm.Close();
            Fillgrid();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)

                return;

            MGEditFrm oMGEditFrm = new MGEditFrm(uRet, false);
            oMGEditFrm.ShowDialog();
            oMGEditFrm.Close();
            Fillgrid();
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

        private new void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedValuePath = "PK";
          //  dataGrid1.ItemsSource = boMG.GetAllMG();
            dataGrid1.ItemsSource = boMG.GetAllMGByMatchCode(MatchCode);
            if (dataGrid1.Items[0] != null)
            {
                dataGrid1.SelectedItem = dataGrid1.Items[0];
                dataGrid1.Focus();

                KBDown();
                KBDown();
            }
            WindowExtensions.HideCloseButton(this);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boMG.GetAllMGByMatchCode(txtSearch.Text);
        }

        private void Fillgrid()
        {
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boMG.GetAllMGByMatchCode(txtSearch.Text);
        }
    }
}