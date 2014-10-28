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
    /// CFListFrm Class
    /// </summary>
    public partial class CFListFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
  

        private string MatchCode;
        public int uRet;

        public CFListFrm(bool edit, String mc )
        {
            MatchCode = mc;
     
            this.InitializeComponent();
            dataGrid1.SelectedValuePath = "PK";
            if (edit == false)
                cmdSelect.IsEnabled = false;
            Fillgrid();
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

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
            CFEditFrm oCFEditFrm = new CFEditFrm(0, true, "");
            oCFEditFrm.ShowDialog();
            oCFEditFrm.Close();
            Fillgrid();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)
                return;

            CFEditFrm oCFEditFrm = new CFEditFrm(uRet, false, "");
            oCFEditFrm.ShowDialog();
            oCFEditFrm.Close();
            Fillgrid();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }

        private void Fillgrid()
        {
            Fahrzeuge _boF = new Fahrzeuge();
          dataGrid1.ItemsSource =  _boF.GetByKennzeichen(MatchCode);
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.Items.Count > 0)
            {
                dataGrid1.SelectedItem = dataGrid1.Items[0];
                dataGrid1.Focus();

                KBDown();
                KBDown();
            }
                WindowExtensions.HideCloseButton(this);
          
        }
    }
}