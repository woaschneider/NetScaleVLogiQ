using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.Waege;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;
using HWB.NETSCALE.FRONTEND.WPF;
using HWB.NETSCALE.GLOBAL;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// HoflisteFrm Class
    /// </summary>
    public partial class HoflisteFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public int uRet;

        public string Einheit = "Gewicht / " + goApp.MengenEinheit;

        private Waege boW;

        public HoflisteFrm()
        {
            this.InitializeComponent();
            FillGrid();
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
            if (e.Key == Key.Return)
            {
                cmdSelect_Click(cmdSelect, e);
            }
        }


        private void FillGrid()
        {
            boW = new Waege();
            dataGrid1.ItemsSource = boW.GetHofListe();
            dataGrid1.SelectedValuePath = "PK";
           
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedItem = dataGrid1.Items[0];
            dataGrid1.Focus();

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

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}