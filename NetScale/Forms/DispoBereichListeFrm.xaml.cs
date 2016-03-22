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

using HWB.NETSCALE.FRONTEND.WPF.Forms;
using NetScaleGlobal;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;



namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// DispoBereichListeFrm Class
    /// </summary>
    public partial class DispoBereichListeFrm
    {
        public int URet;

        /// <summary>
        /// Constructor
        /// </summary>
        public DispoBereichListeFrm()
        {
            InitializeComponent();
            // PrimaryBizObj = 
            var boP = new Planningdivision();
            dataGrid.ItemsSource = boP.GetAll();
            dataGrid.SelectedValuePath = "PK";
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }
        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
                e.Handled = true;
            }
            if (e.Key == Key.Return)
            {
                CmdSelect_OnClick(cmdSelect, e);
                Hide();
                e.Handled = true;
            }
            if (e.Key == Key.Tab)
            {
                
            }

        }
        private void CmdSelect_OnClick(object sender, RoutedEventArgs e)
        {
            URet = Convert.ToInt32(dataGrid.SelectedValue);
            Hide();
        }

        private void MenuItemClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void CmdNeu_OnClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void CmdEdit_OnClick(object sender, RoutedEventArgs e)
        {
       
        }

        private new  void  Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.HideCloseButton();
        }
    }
}