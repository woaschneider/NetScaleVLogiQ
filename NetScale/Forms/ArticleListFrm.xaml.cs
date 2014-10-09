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
using HWB.NETSCALE.FRONTEND.WPF.Import.ArticleImport;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ArticleListFrm Class
    /// </summary>
    public partial class ArticleListFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private int _uRet;

        public int uRet
        {
            get { return _uRet; }
          
        }
        

        private Artikel  boA = new Artikel();
       
        public ArticleListFrm()
        {
            this.InitializeComponent();
            DataContext = boA.GetAllArticle();
            dataGrid.SelectedValuePath = "PK";
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            _uRet = Convert.ToInt32(dataGrid.SelectedValue);
            this.Hide();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}