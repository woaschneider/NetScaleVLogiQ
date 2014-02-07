using System;
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
using OakLeaf.MM.Main.Business;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// Maintenance Form
    /// </summary>
    public partial class BrokenRulesWindow : mmBrokenRulesBaseWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BrokenRulesWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="businessRule">Business rules object</param>
        public BrokenRulesWindow(mmBusinessRule businessRule)
            : base(businessRule)
        {
            this.InitializeComponent();

            if (!this.DisplayBrokenRules)
            {
                this.lblBrokenRules.Visibility = Visibility.Collapsed;
                this.txtBrokenRules.Visibility = Visibility.Collapsed;
            }

            if (!this.DisplayWarnings)
            {
                this.lblWarnings.Visibility = Visibility.Collapsed;
                this.txtWarnings.Visibility = Visibility.Collapsed;
            }

            btnOK.Focus();
        }

        /// <summary>
        /// btnOK.Click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}