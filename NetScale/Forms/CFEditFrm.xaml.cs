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

        private CF boCF;

        private CFEntity boCFE;
        private bool DoNotFireChangeText;

        public CFEditFrm(int pk, bool New, string Kfz1)
        {
            DoNotFireChangeText = true;
            // boCF = new CF();
            boCF = (CF) this.RegisterPrimaryBizObj(new CF());

            this.InitializeComponent();
            tb_einheit1.Text = goApp.MengenEinheit;
            tb_einheit2.Text = goApp.MengenEinheit;
            tb_einheit3.Text = goApp.MengenEinheit;

            if (New == true)
            {
                this.Title = "Neues Kfz anlegen";
                boCFE = boCF.NewEntity();
                boCFE.Kfz1 = Kfz1;
                boCFE.KfzID = GetNextFreeKfzId();
                uRet = boCFE.KfzID;
            }
            else
            {
                this.Title = "Kfz bearbeiten";
                boCFE = boCF.GetCFByPK(pk);
                uRet = null;
            }
            DataContext = boCFE;
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            DoNotFireChangeText = false;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                uRet = null;

                this.CancelEntity(boCF, boCFE);

                Hide();
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_CatchFocus.Focus();
            // Focus auf eine nicht sichtbare Textbox setzen. Sonst werden die Änderungen an der letzen TB nicht übernommen. Trick!
         //   boCF.SaveEntity(boCFE);

            var result = this.SaveEntity(boCF, boCFE);
            if (result != mmSaveDataResult.RulesPassed)
                return;
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boCF.DeleteEntity();
                this.Close();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boCF.CancelEntity();
            boCF.Cancel();
            Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)

        {
            boCF.CancelEntity();
            boCF.Cancel();
            Close();
        }

        private void cmdAbruf_Click(object sender, RoutedEventArgs e)
        {
            AbruflisteFrm oABFrm = new AbruflisteFrm();
            oABFrm.ShowDialog();
            int uRet = oABFrm.URet;
            if (uRet == 0)
            {
                oABFrm.Close();
                return;
            }


            Abruf boAB = new Abruf();
            AbrufEntity oABE = boAB.GetAbrufById(uRet);
            if (oABE != null)
            {
                tb_abrufnr.Text = oABE.Abrufnr;
                boCFE.abruf_PK = oABE.PK;
            }
            oABFrm.Close();
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
                var oAp = new AP();
                APEntity oApe = oAp.GetAPByNr(tb_NrFU.Text, "FU");
                if (oApe != null)
                    FillApFu(oApe);
                else
                {
                    ClearApFu();
                }
            }
        }


        // SP Methoden
        private void LookUpAndFillSp()
        {
            var oApFrm = new APFrm(tb_FirmaSP.Text, "SP");
            oApFrm.ShowDialog();
            int uRet = oApFrm.uRet;
            oApFrm.Close();

            if (uRet == 0)
                return;

            var boAp = new AP();
            APEntity boApe = boAp.GetAPById(uRet);
            tb_FirmaSP.Text = boApe.Firma;
            tb_NrSP.Text = boApe.Nr;
        }

        private void LookUpAndFillFu()
        {
            var oApFrm = new APFrm(tb_FirmaFU.Text, "FU");
            oApFrm.ShowDialog();
            int uRet = oApFrm.uRet;
            oApFrm.Close();

            if (uRet == 0)
                return;

            var boAp = new AP();
            APEntity boApe = boAp.GetAPById(uRet);
            boCFE.ap_PKFU = boApe.PK;
            tb_FirmaFU.Text = boApe.Firma;
            tb_NrFU.Text = boApe.Nr;
        }
        private void FillApFu(APEntity boApe)
        {
            tb_NrFU.Text = boApe.Nr;
            tb_FirmaFU.Text = boApe.Firma;
        }
        private void FillApSp(APEntity boApe)
        {
            tb_NrSP.Text = boApe.Nr;
            tb_FirmaSP.Text = boApe.Firma;
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
            bool loopReady = false;
            int ii = 0;
            do
            {
                ii = ii + 1;
                CFEntity boCFE = boCF.GetCFByKfzId(ii.ToString());
                if (boCFE == null)
                {
                    loopReady = true;
                }
            } while (loopReady == false);
            return ii.ToString();
        }
    }
}