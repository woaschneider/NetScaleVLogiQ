using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
// using HWB.NETSCALE.FRONTEND.WPF.Import.ArticleImport;
using HWB.NETSCALE.BOEF.JoinClasses;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// AuftragsListeV2 Class
    /// </summary>
   
   
    
    public partial class AuftragsListeV2 : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private int _uRet;

        public int uRet
        {
            get { return _uRet; }

        }

        private Orderitem boOI = new Orderitem();
        private OrderItemservice boOIES = new OrderItemservice();

   

        public AuftragsListeV2(string mc)
        {
            this.InitializeComponent();
            DataContext = boOI.GetAll();
            dataGridOrderItems.SelectedValuePath = "PK";
            dataGridOrderItemService.SelectedValuePath = "PK";
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            _uRet = Convert.ToInt32(dataGridOrderItemService.SelectedValue);
            this.Hide();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilldataGridOrderItemService(int pk)
        {
            dataGridOrderItemService.ItemsSource = boOIES.GetByParentPK(pk);
        }

       

        private void dataGridOrderItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            FilldataGridOrderItemService(Convert.ToInt32(dataGridOrderItems.SelectedValue));
        }

       
      

        private void txtAU_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGridOrderItems.SelectedValuePath = "PK";
          dataGridOrderItems.ItemsSource= boOI.GetByAU_RE_KR_MatchCode(txtAU.Text,
                                                                              txtRE.Text,txtKundenReferenz.Text);
        
        }

        private void txtKundenReferenz_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGridOrderItems.SelectedValuePath = "PK";
            dataGridOrderItems.ItemsSource = boOI.GetByAU_RE_KR_MatchCode(txtAU.Text,
                                                                                txtRE.Text, txtKundenReferenz.Text);
        }

        private void txtRE_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGridOrderItems.SelectedValuePath = "PK";
            dataGridOrderItems.ItemsSource = boOI.GetByAU_RE_KR_MatchCode(txtAU.Text,
                                                                                txtRE.Text, txtKundenReferenz.Text);
        }
    }
}