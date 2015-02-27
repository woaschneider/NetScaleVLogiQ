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
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ArbeitsleistungFilterFrm Class
    /// </summary>
    public partial class ArbeitsleistungFilterFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Arbeitsleistungsfilter _boA;
        private mmBindingList<ArbeitsleistungsfilterEntity> oAList; 
        public ArbeitsleistungFilterFrm()
        {
            this.InitializeComponent();
            _boA= new Arbeitsleistungsfilter();
            oAList = _boA.GetAll();
            dataGrid1.ItemsSource = oAList;

        }

        private void CmdSaveRibbonButton_OnClick(object sender, RoutedEventArgs e)
        {
            _boA.SaveEntityList(oAList);
        }

        private void CmdDeleteRibbonButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void CmdCancelRibbonButton_OnClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void RibbonApplicationMenuItem1_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
           this.Close();
        }
    }
}