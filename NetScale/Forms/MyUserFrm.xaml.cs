using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.User;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.WPF;
using MessageBox = System.Windows.MessageBox;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// MyUserFrm Class
    /// </summary>
    public partial class MyUserFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private User boU;

        private UserEntity boUE;

        public MyUserFrm(int PK, bool New)
        {
            boU = new User();
            boU.AccessLevel = goApp.acessLevel; // <---- Dirty Harry was here
            this.InitializeComponent();
            DisplayErrorDialog = true;
            DisplayErrorProvider = true;
            RegisterPrimaryBizObj(boU);

            if (New == true)
            {
                this.Title = "Neuen Benutzer anlegen";
                boUE = boU.NewEntity();
                this.DataContext = boUE;
                boUE.AccessLevel = goApp.acessLevel;

                tb_accesslevel.Focus();
                if (Convert.ToInt32(goApp.acessLevel) >= 3)
                {
                    cmdDelete.IsEnabled = false;
                    tb_accesslevel.IsEnabled = false;
                    UserProfilLookupButton.IsEnabled = false;
                }
            }
            else
            {
                this.Title = "Benutzer bearbeiten";
                boUE = boU.GetUserById(PK);
                this.DataContext = boUE;
            }


            var pw = boUE.Password;
            txtPassword.Password = pw;
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_catchfocus.Focus();
            mmSaveDataResult result;
            boUE.Password = txtPassword.Password;
            result = this.SaveEntity(boU, boUE);
            if (result != mmSaveDataResult.RulesPassed)
                return;
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boU.DeleteEntity();
                this.Hide();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boU.Cancel();
            Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
            boU.SaveEntity(boUE);
        }

        private void UserProfilLookupButton_Click(object sender, RoutedEventArgs e)
        {
            UserRollenFrm oURFrm = new UserRollenFrm();
            oURFrm.ShowDialog();


            tb_accesslevel.Text = oURFrm.uRet;
            tb_accesslevel.Focus();
            oURFrm.Close();
        }
    }
}