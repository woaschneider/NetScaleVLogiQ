using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// Interaction logic for UserLoginWindow.xaml
    /// </summary>
    public partial class UserLoginWindow : mmUserLoginWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserLoginWindow()
        {
            InitializeComponent();

            this.UserIDConnect = "sa";
            this.PasswordConnect = "";

            this.FocusOnLoadElement = this.txtUserId;
        }

        /// <summary>
        /// Cancel the login and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Canceled = true;
            this.Close();
        }

        /// <summary>
        /// Move the window when the user holds down the left mouse button and drags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mmUserLoginWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (this.AuthenticateUser(this.txtUserId.Text, this.txtPassword.Password, this.DatabaseKeyConnect,
                                      this.UserIDConnect, this.PasswordConnect))
            {
                this.DialogResult = true;


                this.Close();
            }
        }
    }
}