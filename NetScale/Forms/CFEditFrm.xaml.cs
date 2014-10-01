using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// CFEditFrm Class
    /// </summary>
    public partial class CFEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public string uRet;

        // KfzID direkt in die Wägemaske zurückgeben, wenn Fahrzeug aus der Wägemaske neu angelegt wird.

        private bool DoNotFireChangeText;

        public CFEditFrm(int pk, bool New, string Kfz1)
        {
            DoNotFireChangeText = true;
            // boCF = new CF();
          

            this.InitializeComponent();
            tb_einheit1.Text = goApp.MengenEinheit;
            tb_einheit2.Text = goApp.MengenEinheit;
            tb_einheit3.Text = goApp.MengenEinheit;

            if (New == true)
            {
                this.Title = "Neues Kfz anlegen";
            
            }
            else
            {
                this.Title = "Kfz bearbeiten";
            
                uRet = null;
            }
         
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            DoNotFireChangeText = false;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                uRet = null;


                Hide();
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_CatchFocus.Focus();
            // Focus auf eine nicht sichtbare Textbox setzen. Sonst werden die Änderungen an der letzen TB nicht übernommen. Trick!
        
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
             
                this.Close();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
       
            Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)

        {
        
            Close();
        }

        private void cmdAbruf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tb_NumDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DoNotFireChangeText)
                tb_tara.Text = WindowExtensions.TextBoxOnlyDecimal(e, tb_tara.Text);
        }

        private void ClearApFu()
        {
            tb_NrFU.Text = "";
            tb_FirmaFU.Text = "";
        }

        private void TbNrFuKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return | e.Key == Key.Tab)
            {
            
            }
        }


        // SP Methoden
        private void LookUpAndFillSp()
        {
            var oApFrm = new AddressFrm(tb_FirmaSP.Text, "SP");
            oApFrm.ShowDialog();
            int uRet = oApFrm.uRet;
            oApFrm.Close();

            if (uRet == 0)
                return;

            var boAp = new Address();
            AddressEntity boApe = boAp.GetAPById(uRet);
           
        }

        private void LookUpAndFillFu()
        {
            var oApFrm = new AddressFrm(tb_FirmaFU.Text, "FU");
            oApFrm.ShowDialog();
            int uRet = oApFrm.uRet;
            oApFrm.Close();

            if (uRet == 0)
                return;

            var boAp = new Address();
            AddressEntity boApe = boAp.GetAPById(uRet);
          
        }
        private void FillApFu(AddressEntity boApe)
        {
         
        }
        private void FillApSp(AddressEntity boApe)
        {
        }

        private void ClearApSp()
        {
            tb_NrSP.Text = "";
            tb_FirmaSP.Text = "";
        }
        private void TbFirmaSpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F4)
                LookUpAndFillSp();
        }

        private void TbFirmaFuKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "F4")
                LookUpAndFillFu();
        }
        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }


        public string GetNextFreeKfzId()
        {
            return "0";

        }
    }
}