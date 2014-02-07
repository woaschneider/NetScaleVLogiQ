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
using HWB.NETSCALE.BOEF.User;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// MyLoginWindow Class
    /// </summary>
    public partial class MyLoginWindow : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MyLoginWindow()
        {
            this.InitializeComponent();
             // KeyHandler
                // KeyDown += OnButtonKeyDown;
                PreviewKeyDown += MyPreviewKeyDownHandler;
            
        }

        private void MyPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(textBox1.IsFocused==true)
                {textBox2.Focus();
                    return;
                }

                if (textBox2.IsFocused == true)
                {
                    DoLogin();
                }
            }
        }
        private void cmdLogin_Click(object sender, RoutedEventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            User boU = new User();
            UserEntity boUE = boU.CheckLogin(textBox1.Text, textBox2.Password);

            if (boUE != null)
            {
                goApp.acessLevel = boUE.AccessLevel;
                goApp.userid = boUE.UserID;
                goApp.username = boUE.FirstName + " " + boUE.LastName;
                DialogResult = true;
                Close();
            }
            else
            {
                DialogResult = false;
            }
        }


        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
            textBox1.Focus();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                Close();
            }
        }
    }
}