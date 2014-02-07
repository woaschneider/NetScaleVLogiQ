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
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;
using HWB.NETSCALE.BOEF;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// FnnFrm Class
    /// </summary>
    public partial class FnnFrm : mmBusinessWindow
    {
        private KbFnn boFnn;

        /// <summary>
        /// Constructor
        /// </summary>
        public FnnFrm()
        {
            this.InitializeComponent();
            boFnn = new KbFnn();
            dataGrid1.SelectedValuePath = "pk";
            Fillgrid();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            boFnn.SaveEntityList();
            this.Hide();
            Close();
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
                Close();
            }
        }

        private void Fillgrid()
        {
            var Liste = boFnn.GetAllKbFn();
            dataGrid1.ItemsSource = Liste;
        }
    }
}