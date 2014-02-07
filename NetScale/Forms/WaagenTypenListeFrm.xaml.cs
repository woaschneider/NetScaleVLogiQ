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
    /// WaagenTypenListeFrm Class
    /// </summary>
    public partial class WaagenTypenListeFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public int uRet;

        private NETSCALE.BOEF.Waagentypen boWT;


        public WaagenTypenListeFrm()
        {
            this.InitializeComponent();
            boWT = new Waagentypen();
        }

        private new void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boWT.GetAllWT();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)
                return;
            this.Hide();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            uRet = 0;
            this.Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            uRet = 0;
            this.Hide();
        }
    }
}