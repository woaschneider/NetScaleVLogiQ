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
using HWB.NETSCALE.BOEF.User;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// MyUserListeFrm Class
    /// </summary>
    public partial class MyUserListeFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private User boU;

        public MyUserListeFrm()
        {
            this.InitializeComponent();
            boU = new User();
            FillGrid();
            if (Convert.ToInt32(goApp.acessLevel) >= 3)
            {
                cmdEdit.IsEnabled = false;
            }
        }

        private void FillGrid()
        {
            dataGrid1.SelectedValuePath = "UserPK";
            dataGrid1.ItemsSource = boU.GetAllUser();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.Close();
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
            MyUserFrm oUFrm = new MyUserFrm(0, true);
            oUFrm.ShowDialog();
            oUFrm.Close();
            FillGrid();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            if (uRet == 0)
                return;

            MyUserFrm oUFrm = new MyUserFrm(uRet, false);
            oUFrm.ShowDialog();
            oUFrm.Close();
            FillGrid();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.Close();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }
    }
}