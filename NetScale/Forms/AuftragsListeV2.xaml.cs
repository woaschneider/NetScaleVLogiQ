using System;

using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

using HWB.NETSCALE.BOEF;
// using HWB.NETSCALE.FRONTEND.WPF.Import.ArticleImport;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// AuftragsListeV2 Class
    /// </summary>

    // Not Implemented:   
    public partial class AuftragsListeV2
    {
        private string _mc;


        public int URet
        {
            get { return _uRet; }
            private set { _uRet = value; }
        }

     

        private readonly Orderitem _boOi = new Orderitem();
        private readonly OrderItemservice _boOies = new OrderItemservice();
        private int _uRet;



        public AuftragsListeV2(string mc)
        {
            _mc = mc;
            InitializeComponent();
           
            DataGridOrderItems.SelectedValuePath = "PK";
            DataGridOrderItemService.SelectedValuePath = "PK";
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            GetValueOrderPos();
        }

        private void GetValueOrderPos()
        {
            URet = Convert.ToInt32(DataGridOrderItemService.SelectedValue);
            Hide();
        }


        private void FilldataGridOrderItemService(int pk)
        {
            DataGridOrderItemService.ItemsSource = _boOies.GetByParentPK(pk);
        }


        private void dataGridOrderItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilldataGridOrderItemService(Convert.ToInt32(DataGridOrderItems.SelectedValue));
        }


        private void txtAU_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetOrderByMc();
        }

        private void txtKundenReferenz_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetOrderByMc();
        }

        private void txtRE_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetOrderByMc();
        }




        private void TxtFreistellung_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            GetOrderByMc();
        }

        private void GetOrderByMc()
        {
            DataGridOrderItems.SelectedValuePath = "PK";
            DataGridOrderItems.ItemsSource = _boOi.GetByAU_RE_KR_MatchCode(TxtAu.Text,
           TxtRe.Text, TxtKundenReferenz.Text, TxtArticleBeschreibung.Text, TxtFreistellung.Text).Distinct();
          

            if (DataGridOrderItems.Items.Count > 0)
            {
                object item = DataGridOrderItems.Items[0]; // = Erste Zeile
                DataGridOrderItems.SelectedItem = item;
                FilldataGridOrderItemService(Convert.ToInt32(DataGridOrderItems.SelectedValue));

                DataGridRow row = DataGridOrderItems.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                if (row == null)
                {


                    DataGridOrderItems.ScrollIntoView(item);


                }
               }
            }

            private
            void TxtArticleBeschreibung_OnTextChanged 
            (object sender, TextChangedEventArgs e)
            {
                GetOrderByMc();
            }

        private
            void DataGridOrderItemService_OnMouseDoubleClick 
            (object sender, MouseButtonEventArgs e)
            {
                GetValueOrderPos();
            }
        }
    }
