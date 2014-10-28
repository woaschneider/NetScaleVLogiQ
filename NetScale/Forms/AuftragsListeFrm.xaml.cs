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
using HWB.NETSCALE.BOEF.JoinClasses;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// AuftragsListeFrm Class
    /// </summary>
    public partial class AuftragsListeFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private int _uRet;

        // Deklariere das primäre BO 
        private Orderitem boO;
        private OrderItemservice boOIS;
        private Adressen boA;
        private AdressenEntity boAE;
        private OrderParentAndChild boOPC;
      
         
        //  private CFEditFrm EditFrm = CFEditFrm();

        public int uRet
        {
            get { return _uRet; }
        }


        public AuftragsListeFrm(string matchcode)
        {
            // Instantiate and register business objects
            this.boO = (Orderitem) this.RegisterPrimaryBizObj(new Orderitem());
            this.boOIS = (OrderItemservice) this.RegisterBizObj(new OrderItemservice());
        
            boA = new Adressen();

            this.InitializeComponent();

         

           // dataGridOrderItem.SelectedValuePath = "PK";
           // dataGridOrderItemService.SelectedValuePath = "PK";

            //  dataGridOrderItem.ItemsSource = boO.GetAll();
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
                //cmdSelect_Click(cmdSelect, e);
                //Hide();
                //e.Handled = true;
            }
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdNeu_Click(object sender, RoutedEventArgs e)
        {
        }

       

        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }

        private void KBDown()
        {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                                                 Key.Down);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
        }

     

        #region Aufträge Suchen

        #region Customer

        private void tb_CustomerSearch_SearchStringChanged(object sender, SearchStringChangedEventArgs e)
        {
 
            tb_CustomerSearch = (mmAutoCompleteTextBox) sender;
            tb_CustomerSearch.BindingList = boA.GetByMatchCode(e.SearchString);
        }

        private void tb_CustomerSearch_SearchStringSelected(object sender, SearchStringSelectedEventArgs e)
        {
                                                                        

            this.dataGrid.ItemsSource = boO.GetOrderWithAllDetailsByMatchCode(tb_InvoiceReceiverSearch.TextBoxContent,
                                                                           tb_InvoiceReceiverSearch.TextBoxContent,
                                                                           tb_OwnerSearch.TextBoxContent, "");
        }

        #endregion

        #region Invoice Receiver

        private void tb_InvoiceReceiverSearch_SearchStringChanged(object sender, SearchStringChangedEventArgs e)
        {
        
            tb_InvoiceReceiverSearch = (mmAutoCompleteTextBox) sender;
            tb_InvoiceReceiverSearch.BindingList = boA.GetByMatchCode(e.SearchString);
        }


        private void tb_InvoiceReceiverSearch_SearchStringSelected(object sender, SearchStringSelectedEventArgs e)
        {
            this.dataGrid.ItemsSource = boO.GetOrderWithAllDetailsByMatchCode(tb_InvoiceReceiverSearch.TextBoxContent,
                                                                              tb_InvoiceReceiverSearch.TextBoxContent,
                                                                              tb_OwnerSearch.TextBoxContent, "");

        }

        #endregion

        #region Lagermandant / Owner
        private void tb_OwnerSearch_SearchStringChanged(object sender, SearchStringChangedEventArgs e)
        {
            tb_OwnerSearch = (mmAutoCompleteTextBox)sender;
            tb_OwnerSearch.BindingList = boA.GetByMatchCode(e.SearchString);
        }



       

        private void tb_OwnerSearch_SearchStringSelected(object sender, SearchStringSelectedEventArgs e)
        {

        }

        #endregion

        private void tb_OwnerSearch_SearchStringChanged_1(object sender, SearchStringChangedEventArgs e)
        {

        }

        #endregion
    }
}