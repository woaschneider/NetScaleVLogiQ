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
    /// AuftragsListeFrm Class
    /// </summary>
    public partial class AuftragsListeFrm : mmBusinessWindow
    {
        private KK boKK;
        public int uRet = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuftragsListeFrm()
        {
            this.InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
            FillDataGrid("", "");
        }

        public AuftragsListeFrm(string matchcode)
        {
            this.InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
            tb_firma.Text = matchcode;
            FillDataGrid(matchcode, "");
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
            if (e.Key == Key.Return)
            {
                dataGrid1.Focus();
                KBDown();
                KBDown();
                e.Handled = true;
                //   dataGrid1.SelectedItem = dataGrid1.Items[0];

                e.Handled = true;
                uRet = Convert.ToInt32(dataGrid1.SelectedValue);
                AuftragEditFrm oKMFrm = new AuftragEditFrm(uRet);
                oKMFrm.ShowDialog();
                uRet = oKMFrm.uRet;// uRet kmpk
                
                this.Hide();
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


        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            DataContext = dataGrid1.ItemsSource;
        }

        private void FillDataGrid(string matchcode, string baustelle)
        {
            boKK = new KK();
            boKK = new KK();

            dataGrid1.SelectedValuePath = "kkpk";

            dataGrid1.ItemsSource = boKK.GetAuftragsListe(matchcode, baustelle);
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            uRet = 0;
            Close();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedValuePath = "kkpk";
            if (dataGrid1.Items.Count > 0)
            {
                dataGrid1.SelectedItem = dataGrid1.Items[0];
                dataGrid1.Focus();
                KBDown();

                KBDown();
            }

            WindowExtensions.HideCloseButton(this);
        }

        private void tb_baustelle_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillDataGrid(tb_firma.Text, tb_baustelle.Text);
        }

        private void tb_firma_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillDataGrid(tb_firma.Text, tb_baustelle.Text);
        }
    }
}