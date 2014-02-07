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
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// Waageneinstellungen Class
    /// </summary>
    public partial class WaageneinstellungenFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private NETSCALE.BOEF.Waageneinstellungen oWE;

        public WaageneinstellungenFrm()
        {
            this.InitializeComponent();
            // Ein wenig tricky: Instanz erzeugen;Methode aufrufen die wiederum eine Instanz des gleichen Typs zurückgibt 
            // Egal funktioniert aber.
            oWE = new Waageneinstellungen();
            oWE = oWE.Load();
            FillFrm();
        }

        private void FillFrm()
        {
            tb_anzahlauswertegeraete.Text = oWE.SCALES;
            tb_anzahlmesskreise.Text = oWE.MESSKREISE;

            tb_w1WaagenID.Text = oWE.W1_WAAGENID;
            tb_w1waagentypbeschreibung.Text = oWE.W1_WAAGENBESCHREIBUNG;
            tb_w1ipnummer.Text = oWE.W1_IP_NUMMER;
            tb_w1comport.Text = oWE.W1_COM;
            tb_W1Baud.Text = oWE.W1_BAUD;
            tb_W1Databit.Text = oWE.W1_DATA_BIT;
            tb_W1Parity.Text = oWE.W1_PARITY_BIT;
            tb_W1Stop.Text = oWE.W1_STOP_BIT;
            tb_eW1.Text = oWE.W1_e;
            tb_minW1.Text = oWE.W1_min;
            tb_maxW1.Text = oWE.W1_max;
            tb_waagennameW1.Text = oWE.W1_WAAGENNAME;
            //TODO Validierung t,Kg etc
            cb_ME.Items.Add("t");
            cb_ME.Items.Add("kg");
            cb_ME.Text = oWE.Einheit;


            tb_w2WaagenID.Text = oWE.W2_WAAGENID;
            tb_w2waagentypbeschreibung.Text = oWE.W2_WAAGENBESCHREIBUNG;
            tb_w2ipnummer.Text = oWE.W2_IP_NUMMER;
            tb_w2comport.Text = oWE.W2_COM;
            tb_W2Baud.Text = oWE.W2_BAUD;
            tb_W2Databit.Text = oWE.W2_DATA_BIT;
            tb_W2Parity.Text = oWE.W2_PARITY_BIT;
            tb_W2Stop.Text = oWE.W2_STOP_BIT;
            tb_eW2.Text = oWE.W2_e;
            tb_minW2.Text = oWE.W2_min;
            tb_maxW2.Text = oWE.W2_max;
            tb_waagennameW2.Text = oWE.W2_WAAGENNAME;
            if (tb_anzahlauswertegeraete.Text == "1")
            {
                Window.Width = 460;
            }
            else
            {
                Window.Width = 879;
            }
            if (tb_anzahlmesskreise.Text == "2")
            {
                buttonSpinner1.IsEnabled = false;
                tb_anzahlauswertegeraete.IsEnabled = false;
            }
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            oWE.SCALES = tb_anzahlauswertegeraete.Text;
            oWE.MESSKREISE = tb_anzahlmesskreise.Text;
            oWE.Einheit = cb_ME.Text;

            oWE.W1_WAAGENID = tb_w1WaagenID.Text;
            oWE.W1_WAAGENBESCHREIBUNG = tb_w1waagentypbeschreibung.Text;
            oWE.W1_IP_NUMMER = tb_w1ipnummer.Text;
            oWE.W1_COM = tb_w1comport.Text;
            oWE.W1_BAUD = tb_W1Baud.Text;
            oWE.W1_DATA_BIT = tb_W1Databit.Text;
            oWE.W1_PARITY_BIT = tb_W1Parity.Text;
            oWE.W1_STOP_BIT = tb_W1Stop.Text;
            oWE.W1_e = tb_eW1.Text;
            oWE.W1_min = tb_minW1.Text;
            oWE.W1_max = tb_maxW1.Text;
            oWE.W1_WAAGENNAME = tb_waagennameW1.Text;

            oWE.W2_WAAGENID = tb_w2WaagenID.Text;
            oWE.W2_WAAGENBESCHREIBUNG = tb_w2waagentypbeschreibung.Text;
            oWE.W2_IP_NUMMER = tb_w2ipnummer.Text;
            oWE.W2_COM = tb_w2comport.Text;
            oWE.W2_BAUD = tb_W2Baud.Text;
            oWE.W2_DATA_BIT = tb_W2Databit.Text;
            oWE.W2_PARITY_BIT = tb_W2Parity.Text;
            oWE.W2_STOP_BIT = tb_W2Stop.Text;
            oWE.W2_e = tb_eW2.Text;
            oWE.W2_min = tb_minW2.Text;
            oWE.W2_max = tb_maxW2.Text;
            oWE.W2_WAAGENNAME = tb_waagennameW2.Text;

            oWE.Save(oWE);
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            FillFrm();
        }

        private void cmdSelectScaleType_ClickW1(object sender, RoutedEventArgs e)
        {
            WaagenTypenListeFrm oWTListeFrm = new WaagenTypenListeFrm();
            oWTListeFrm.ShowDialog();
            int uRet = oWTListeFrm.uRet;
            if (uRet != 0)
            {
                Waagentypen boWT = new Waagentypen();
                WaagentypenEntity boWTE = boWT.GetWTByPK(uRet);
                tb_w1WaagenID.Text = boWTE.WaagenID.ToString();
                tb_w1waagentypbeschreibung.Text = boWTE.Waagenbezeichnung;
            }
            oWTListeFrm.Close();
        }

        private void cmdSelectScaleTypeW2_Click(object sender, RoutedEventArgs e)
        {
            WaagenTypenListeFrm oWTListeFrm = new WaagenTypenListeFrm();
            oWTListeFrm.ShowDialog();
            int uRet = oWTListeFrm.uRet;
            if (uRet != 0)
            {
                Waagentypen boWT = new Waagentypen();
                WaagentypenEntity boWTE = boWT.GetWTByPK(uRet);
                tb_w2WaagenID.Text = boWTE.WaagenID.ToString();
                tb_w2waagentypbeschreibung.Text = boWTE.Waagenbezeichnung;
            }
            oWTListeFrm.Close();
        }

        private void buttonSpinner1_Spin(object sender, Microsoft.Windows.Controls.SpinEventArgs e)
        {
            int value = String.IsNullOrEmpty(tb_anzahlauswertegeraete.Text)
                            ? 0
                            : Convert.ToInt32(tb_anzahlauswertegeraete.Text);
            if (e.Direction == Microsoft.Windows.Controls.SpinDirection.Increase)
                if (value == 2)
                {
                    value = 1;
                }
                else
                {
                    value++;
                }

            else if (value == 1)
            {
                value = 2;
            }
            else
            {
                value--;
            }

            tb_anzahlauswertegeraete.Text = value.ToString();
            if (tb_anzahlauswertegeraete.Text == "1")
            {
                Window.Width = 460;
            }
            else
            {
                Window.Width = 879;
            }
        }

        private void cmdGetComPorts1_Click(object sender, RoutedEventArgs e)
        {
            tb_w1comport.Text = LookUpComPorts();
        }

        private void cmdGetComPorts2_Click(object sender, RoutedEventArgs e)
        {
            tb_w2comport.Text = LookUpComPorts();
        }

        public string LookUpComPorts()
        {
            PortListFrm oCPFrm = new PortListFrm();
            oCPFrm.ShowDialog();
            string uRet = oCPFrm.uRet;
            oCPFrm.Close();
            if (uRet != "")
                return uRet;
            else
            {
                return "";
            }
        }

        private void buttonSpinner2_Spin(object sender, Microsoft.Windows.Controls.SpinEventArgs e)
        {
            int value = String.IsNullOrEmpty(tb_anzahlmesskreise.Text) ? 0 : Convert.ToInt32(tb_anzahlmesskreise.Text);
            if (e.Direction == Microsoft.Windows.Controls.SpinDirection.Increase)
                if (value == 2)
                {
                    value = 1;
                }
                else
                {
                    value++;
                }

            else if (value == 1)
            {
                value = 2;
            }
            else
            {
                value--;
            }

            tb_anzahlmesskreise.Text = value.ToString();
            if (tb_anzahlmesskreise.Text == "1")
            {
                buttonSpinner1.IsEnabled = true;
                tb_anzahlauswertegeraete.IsEnabled = true;
            }
            if (tb_anzahlmesskreise.Text == "2")
            {
                tb_anzahlauswertegeraete.Text = "1";

                buttonSpinner1.IsEnabled = false;
                tb_anzahlauswertegeraete.IsEnabled = false;
            }
        }

       
    }
}