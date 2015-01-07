using System;

using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        public string Mc { get; private set; }

        public int URet { get; private set; }

        public OrderItemservice BoOies
        {
            get { return _boOies; }
        }

        private readonly Orderitem _boOi = new Orderitem();
        private readonly OrderItemservice _boOies = new OrderItemservice();


        public AuftragsListeV2(string mc)
        {
            Mc = mc;
            InitializeComponent();
          //  DataContext = _boOi.GetAll();
            dataGridOrderItems.SelectedValuePath = "PK";
            dataGridOrderItemService.SelectedValuePath = "PK";
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            URet = Convert.ToInt32(dataGridOrderItemService.SelectedValue);
            Hide();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FilldataGridOrderItemService(int pk)
        {
            dataGridOrderItemService.ItemsSource = _boOies.GetByParentPK(pk);
        }


        private void dataGridOrderItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilldataGridOrderItemService(Convert.ToInt32(dataGridOrderItems.SelectedValue));
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
            dataGridOrderItems.SelectedValuePath = "PK";
            dataGridOrderItems.ItemsSource = _boOi.GetByAU_RE_KR_MatchCode(txtAU.Text,
                txtRE.Text, txtKundenReferenz.Text, txtFreistellung.Text).Distinct();

            if (dataGridOrderItems.Items.Count > 0)
            {
                object item = dataGridOrderItems.Items[0]; // = Erste Zeile
                dataGridOrderItems.SelectedItem = item;
                FilldataGridOrderItemService(Convert.ToInt32(dataGridOrderItems.SelectedValue));

                DataGridRow row = dataGridOrderItems.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                if (row == null)
                {
                    /* bring the data item (Product object) into view
             * in case it has been virtualized away */
                    dataGridOrderItems.ScrollIntoView(item);

                }
            }
        }
    }
}