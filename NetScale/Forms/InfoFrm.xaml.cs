using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
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
    /// InfoFrm Class
    /// </summary>
    public partial class InfoFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InfoFrm()
        {
            this.InitializeComponent();
            tb_version.Text = "Version : " +
                              System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AssemblyCopyrightAttribute copyright =
                Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false)[0] as
                AssemblyCopyrightAttribute;
            tb_copyright.Text = copyright.Copyright;
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }
    }
}