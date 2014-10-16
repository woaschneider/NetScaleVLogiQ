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

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// WarenartListFrm Class
    /// </summary>
    public partial class WarenartListFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private int _uRet;
        // Deklariere das primäre BO 
        private Warenarten boWA = new Warenarten();
        //  private CFEditFrm EditFrm = CFEditFrm();

        public int uRet
        {
            get { return _uRet; }

        }




        public WarenartListFrm(string matchcode)
        {
            this.InitializeComponent();
            // PrimaryBizObj = 
            dataGrid.ItemsSource = boWA.GetByMatchCode(matchcode);
            dataGrid.SelectedValuePath = "PK";
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
            txtSearch.Text = matchcode;
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

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            _uRet = Convert.ToInt32(dataGrid.SelectedValue);
            this.Hide();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGrid.ItemsSource = boWA.GetByMatchCode(txtSearch.Text.Trim());
        }

        private void FillGrid()
        {
        }

        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _uRet = Convert.ToInt32(dataGrid.SelectedValue);
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.SelectedItem = dataGrid.Items[0];
            dataGrid.Focus();

            KBDown();
            KBDown();
            WindowExtensions.HideCloseButton(this);
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}