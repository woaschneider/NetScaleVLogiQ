using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ErrorLogFrm Class
    /// </summary>
    public partial class ErrorLogFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorLogFrm()
        {
            this.InitializeComponent();
            this.FillDataGrid();
        }

        private void FillDataGrid()
        {
            dataGrid1.ItemsSource = new ErrorLog().GetAll();
        }

        private void MenuItemClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.Close();
        }
    }
}