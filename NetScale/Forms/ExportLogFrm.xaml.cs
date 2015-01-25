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
    /// ExportLogFrm Class
    /// </summary>
    public partial class ExportLogFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExportLogFrm()
        {
            this.InitializeComponent();
           DatePickerExport.SelectedDate = System.DateTime.Today;
            FillGrid(DatePickerExport.SelectedDate.Value);
        }

        private void FillGrid(DateTime? dt)
        {
        dataGrid.ItemsSource=  new ExportLog().GetAll();
        }


        private void MenuItemClose_OnClick(object sender, RoutedEventArgs e)
        {
        this.Hide();
        }
    }
}