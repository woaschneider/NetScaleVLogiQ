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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using HWB.Framework;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ArticleEditFrm Class
    /// </summary>
    public partial class ArticleEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ArticleEditFrm()
        {
            this.InitializeComponent();
            CloseButtonHide = true;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        public ArticleEditFrm(string title, int PK, bool New)
        {
            this.InitializeComponent();
            this.Title = title;
            CloseButtonHide = true;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private bool _closeButtonHide;

        public bool CloseButtonHide
        {
            get { return _closeButtonHide; }
            set
            {
                _closeButtonHide = value;
                WindowsExtension.CloseButtonHide = _closeButtonHide;
            }
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }






    }
}