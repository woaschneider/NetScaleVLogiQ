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

using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// PasswortFrm2 Class
    /// </summary>
    public partial class PasswortFrm2 : mmBusinessWindow
    {
        public bool PWOk = false;
        /// <summary>
        /// Constructor
        /// </summary>
        public PasswortFrm2()
        {     
            this.InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password == "235711")
            {
                PWOk = true;
                this.Hide();
            }
            else
            {
                PWOk = false;
                this.Hide();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            PWOk = false;
            this.Hide();
        }
    }
}