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
using HardwareDevices;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WiegeFrm Class
    /// </summary>
    public partial class WiegeFrm : mmBusinessWindow
    {
        #region Private Fields

        private Waege _boW;
        private WaegeEntity _boWe;
        private int _wiegeStatus;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public WiegeFrm()
        {
            this.InitializeComponent();
            _boW = new Waege();
            DataContext = _boWe;
            // Achtung - Hier wird der Observer abonniert
            netScaleView1.OnWeightChanged += new WeightChangedHandler(ShowEventGewichtHasChanged);
            DisplayErrorDialog = true;
            DisplayErrorProvider = true;
            RegisterPrimaryBizObj(_boW);

            _wiegeStatus = 99;
            _wiegeStatus = 0;
            tb_me1.Text = goApp.MengenEinheit;
            tb_me2.Text = goApp.MengenEinheit;
            tb_me3.Text = goApp.MengenEinheit;

            netScaleView1.Einheit = goApp.MengenEinheit;

            #region Waagen Setup

            var oWe = new Waageneinstellungen();
            oWe = oWe.Load();

            if (!goApp.WaageModulAktiv)
            {
                MessageBox.Show("Das Programm befindet sich im DEMO-Modus", "INFO", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                goApp.WaageModulAktiv = false;
            }

            if (goApp.WaageModulAktiv && goApp.WaageAn) // Wenn true
            {
                netScaleView1.SetUp(false); // false = Keine Demo
                if (oWe.MESSKREISE == "2")
                {
                    netScaleView1.ActiveScale = 3; // Erst mal hardgecodet. Später Default waage
                }


                netScaleView1.SliderInvisible = true;
                goApp.MengenEinheit = oWe.Einheit;

                if (oWe.SCALES == "1" && oWe.MESSKREISE == "1")
                {
                    netScaleView1.ActiveScale = 1; //
                }
                if (oWe.SCALES == "2" || oWe.MESSKREISE == "2")
                {
                    ribbonSelectScale.IsEnabled = true;
                }
            }
            else
            {
                netScaleView1.SetUp(true); // True = Demo
                netScaleView1.ActiveScale = 1;
                goApp.MengenEinheit = oWe.Einheit;
            }
            if (goApp.WaageAn == false)
            {
                netScaleView1.Width = 0;
                netScaleView1.Height = 0;
            }

            #endregion
        }

        // Das Ereignis, welches ausgelöst wird, wenn die Gewichtsänderung in NetScale in der Wägemaske gemeldet wird
        public void ShowEventGewichtHasChanged()
        {
            //  MessageBox.Show("Event");
            if (_wiegeStatus == 2)
            {
                _boWe.Zweitgewicht = netScaleView1.Gewicht;
            }
        }

        #region Ribbon Schaltflächen Methoden

        private void ribbonNeu_Click(object sender, RoutedEventArgs e)
        {
            _boWe = _boW.NewEntity();
            if (_boWe != null)

                DataContext = _boWe;
        }

        private void ribbonSave_Click(object sender, RoutedEventArgs e)
        {
            if (_boWe != null)
                _boW.SaveEntity(_boWe);
        }

        private void ribbonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_boWe != null)
                _boW.DeleteEntity(_boWe);
        }

        private void ribbonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_boWe != null)
                _boW.CancelEntity(_boWe);
        }


        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }


        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            new ImportPolos().Import();
        }

        private void cmdArtikel_Click(object sender, RoutedEventArgs e)
        {
            ArtikelListFrm oAListFrm = new ArtikelListFrm("");
            oAListFrm.ShowDialog();
            oAListFrm.Close();
        }

        private void cmdWarenart_Click(object sender, RoutedEventArgs e)
        {
            WarenartListFrm oWarenartFrm = new WarenartListFrm("");
            oWarenartFrm.ShowDialog();
            oWarenartFrm.Close();
        }

        private void cmdProdukte_Click(object sender, RoutedEventArgs e)
        {
            ProdukteListFrm oPFrm = new ProdukteListFrm("");
            oPFrm.ShowDialog();
            oPFrm.Close();
        }

        private void cmdArtikelAttribute_Click(object sender, RoutedEventArgs e)
        {
            ArtikelAttributeListFrm oA = new ArtikelAttributeListFrm("");
            oA.ShowDialog();
            oA.Close();
        }

        private void cmdLagerPlaetze_Click(object sender, RoutedEventArgs e)
        {
            LagerplaetzeListeFrm oLFrm = new LagerplaetzeListeFrm("");
            oLFrm.ShowDialog();
            oLFrm.Close();
        }


        private void cmdAdressen_Click(object sender, RoutedEventArgs e)
        {
            AdressenListeFrm oA = new AdressenListeFrm("");
            oA.ShowDialog();
            if (oA != null)
            {
                oA.Close();
            }
        }
        private void cmdKFZ_Click(object sender, RoutedEventArgs e)
        {
            CFListFrm oCFFrm = new CFListFrm(true,"");
            oCFFrm.ShowDialog();
            int uRet = oCFFrm.uRet;
            oCFFrm.Close();
        }

        private void cmdFrachtmittel_Click(object sender, RoutedEventArgs e)
        {
            FrachtmittelListFrm oFFrm = new FrachtmittelListFrm();
            oFFrm.ShowDialog();
            int uRet = oFFrm.uRet;
            oFFrm.Close();
        }

        private void ribbonAuftrag_Click(object sender, RoutedEventArgs e)
        {
            AuftragsListeFrm oAFrm = new AuftragsListeFrm("");
            oAFrm.ShowDialog();
            oAFrm.Close();
        }

        #endregion

        private void buttonLookUpKfz_Click(object sender, RoutedEventArgs e)
        {
            CFListFrm oKfzListeFrm = new CFListFrm(true, txtKfzKennzeichen.Text);
            oKfzListeFrm.ShowDialog();
            var uRet = oKfzListeFrm.uRet;
            if (_boWe != null)
            {
                _boW.FillKfz(uRet, _boWe);
            }

            oKfzListeFrm.Close();
        }

        private void txtKfzKennzeichen_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

      

      
    }
}