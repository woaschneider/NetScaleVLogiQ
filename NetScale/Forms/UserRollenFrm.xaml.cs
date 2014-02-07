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
    /// UserRollenFrm Class
    /// </summary>
    public partial class UserRollenFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public string uRet;

        public UserRollen boUR;

        public UserRollenFrm()
        {
            boUR = new UserRollen();
            this.InitializeComponent();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = dataGrid1.SelectedValue.ToString();
            Hide();
        }

        private void Fillgrid()
        {
            dataGrid1.SelectedValuePath = "AccessLevel";
            dataGrid1.ItemsSource = boUR.GetAllEntities();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedValuePath = "AccessLevel";
            dataGrid1.ItemsSource = boUR.GetAllEntities();
            WindowExtensions.HideCloseButton(this);
        }
    }
}