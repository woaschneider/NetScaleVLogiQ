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
using HWB.NETSCALE.FRONTEND.WPF.Import.ArticleImport;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// AdressenListeFrm Class
    /// </summary>
    public partial class AdressenListeFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private int _uRet;
        // Deklariere das primäre BO 
        private Adressen boA = new Adressen();
        

        public int uRet
        {
            get { return _uRet; }

        }




        public AdressenListeFrm(string matchcode)
        {
            this.InitializeComponent();

            dataGrid.ItemsSource = boA.GetByMatchCode(matchcode);
            dataGrid.SelectedValuePath = "PK";
          
            txtSearch.Text = matchcode;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
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
            dataGrid.ItemsSource = boA.GetByMatchCode(txtSearch.Text.ToString());
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