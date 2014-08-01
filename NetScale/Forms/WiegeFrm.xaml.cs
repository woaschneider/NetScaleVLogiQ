using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using HardwareDevices;
using HardwareDevices.Elseco;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using NetScaleGlobal;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Message = HWB.NETSCALE.BOEF.Message;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Net;
//using System.Security.Cryptography;
//using System.Threading;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Navigation;
//using OakLeaf.MM.Main;

//using OakLeaf.MM.Main.WPF;
//using HWB.NETSCALE.FRONTEND.WPF.Forms;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WiegeFrm Class
    /// </summary>
    public sealed partial class WiegeFrm
    {
        // private bool RibbonClicksAuswerten = false;
        private readonly Waege _boW;
        private readonly RFReceiver _oRfReceiver;
        private WaegeEntity _boWe;

        private mmSaveDataResult _result;
        private bool _rfidAktiv;
        private Mandant boMandant;
        private bool _mengenErfassung;
        //  private decimal _gewicht ; Entfernen ?


        private int _wiegestatus;

        public WiegeFrm()
        {
            InitializeComponent();
            // Achtung - Hier wird der Observer abonniert
            netScaleView1.OnWeightChanged += new WeightChangedHandler(ShowEventGewichtHasChanged);

            DisplayErrorDialog = true;
            DisplayErrorProvider = true;
            RegisterPrimaryBizObj(_boW);

            boMandant = new Mandant();
            _boW = new Waege();
            //   Gewicht = netScaleView1.Gewicht;
            lblKfz1.Target = tb_Kfz1;
            lblfirma.Target = tb_FirmaKU;
            lblSortenNr.Target = tb_SortenNr;
            Wiegestatus = 99;
            Wiegestatus = 0;
            tb_me1.Text = goApp.MengenEinheit;
            tb_me2.Text = goApp.MengenEinheit;
            tb_me3.Text = goApp.MengenEinheit;
            netScaleView1.Einheit = goApp.MengenEinheit;


            var oWe = new Waageneinstellungen();
            oWe = oWe.Load();

            //*************************************************************************************


            if (!goApp.WaageModulAktiv)

            {
                MessageBox.Show("Das Programm befindet sich im DEMO-Modus", "INFO", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                goApp.WaageModulAktiv = false;
            }


            if (goApp.FunkModulAktiv)
            {
                _oRfReceiver = new RFReceiver(true);
                if (_oRfReceiver.ErrorMessage != "")
                    MessageBox.Show(_oRfReceiver.ErrorMessage, "ACHTUNG", MessageBoxButton.OK, MessageBoxImage.Error);

                SetTelegrammBinding();
            }
            else
            {
                goApp.FunkModulAktiv = false;
                _oRfReceiver = new RFReceiver(false);
            }


            //**************************************************************************************
            //
            if (goApp.WaageModulAktiv && goApp.WaageAn) // Wenn true
            {
                netScaleView1.SetUp(false); // false = Keine Demo
                netScaleView1.ActiveScale = 1;
                netScaleView1.SliderInvisible = true;
                goApp.MengenEinheit = oWe.Einheit;
                if (oWe.SCALES == "2")
                {
                    cmdSelectScale.IsEnabled = true;
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

            cb_ZweitWaegungPreset.IsEnabled = goApp.ZWEIT_OHNE_ERST;

            // KeyDown += OnButtonKeyDown;
            PreviewKeyDown += MyPreviewKeyDownHandler;
            SetToolTips();

            FillMandantCombobox();
            //  Formatierung zur Laufzeit
            SetWeightBindingFormat();
        }

        // Das Ereignis, welches ausgelöst wird, wenn die Gewichtsänderung in NetScale in der Wägemaske gemeldet wird
        public void ShowEventGewichtHasChanged()
        {
            //  MessageBox.Show("Event");
            if (_wiegestatus == 2)
            {
                _boWe.ZweitGewicht = netScaleView1.Gewicht;
                CalcNetto();
                _boW.CalcRechnung();
            }
        }

        private int Wiegestatus
        {
            set
            {
                if (value != _wiegestatus)
                {
                    _wiegestatus = value;
                    WiegeStatusChange(_wiegestatus);
                }
            }
        }

        public bool RfidAktiv
        {
            get { return _rfidAktiv; }
            set
            {
                _rfidAktiv = value;
                // Diese kompakte Syntax wurde durch Resharper vorgeschlagen. Eigentlich steckt da ein VFP IIF() dahinter ->  return cond ? left : right;
                tb_rfid.Text = RfidAktiv ? "Fernsteuerung aktiv" : "...";
            }
        }

        public bool MengenErfassung
        {
            get { return _mengenErfassung; }
            set
            {
                _mengenErfassung = value;
                if (_mengenErfassung == true)
                {
                    netScaleView1.Visibility = System.Windows.Visibility.Collapsed;
                    tb_ErstGewicht.Visibility = System.Windows.Visibility.Collapsed;
                    tb_ZweitGewicht.Visibility = System.Windows.Visibility.Collapsed;
                    tb_ln1.Visibility = System.Windows.Visibility.Collapsed;
                    tb_ln2.Visibility = System.Windows.Visibility.Collapsed;
                    lblNettogewicht.Content = "_Menge";
                    tb_me1.Text = "";
                    tb_me2.Text = "";
                    tb_me3.Text = "Stk";
                    lblErst.Content = "";
                    lblZweit.Content = "";
                    lbl_Ln.Content = "";
                    if (_wiegestatus != 5)
                        Wiegestatus = 8;
                    tb_Nettogewicht.IsReadOnly = false;
                }
                else
                {
                    netScaleView1.Visibility = System.Windows.Visibility.Visible;
                    tb_ErstGewicht.Visibility = System.Windows.Visibility.Visible;
                    tb_ZweitGewicht.Visibility = System.Windows.Visibility.Visible;
                    tb_ln1.Visibility = System.Windows.Visibility.Visible;
                    tb_ln2.Visibility = System.Windows.Visibility.Visible;
                    lblNettogewicht.Content = "Nettogewicht";
                    tb_me1.Text = goApp.MengenEinheit;
                    tb_me2.Text = goApp.MengenEinheit;
                    tb_me3.Text = goApp.MengenEinheit;
                    lblErst.Content = "Erstgewicht";
                    lblZweit.Content = "Zweitgewicht";
                    lbl_Ln.Content = "Lfd. Wäg.-Nr.";
                    tb_Nettogewicht.IsEnabled = true;
                    //TODO:
                    //  if(_wiegestatus!=5)
                    // Wiegestatus = 1; // Diese Zeile führt zu einem Umschalten auf Erstwägung bei Änderung der Sorte
                    // Deswegen erstaml auskommentiert
                }
            }
        }


        private void SetWeightBindingFormat()
        {
            string cFormat = "";
            if (goApp.MengenEinheit == "t")
                cFormat = "F2";

            if (goApp.MengenEinheit == "kg")
                cFormat = "F0";

            //**************************************************************
            var bNettogewicht = new Binding("Nettogewicht")
                                    {
                                        StringFormat = cFormat,
                                        Mode = BindingMode.TwoWay,
                                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                    };


            tb_Nettogewicht.SetBinding(TextBox.TextProperty, bNettogewicht);

            //*******************************************************************
            var bErstgewicht = new Binding("ErstGewicht")
                                   {
                                       StringFormat = cFormat,
                                       Mode = BindingMode.TwoWay,
                                       UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                   };


            tb_ErstGewicht.SetBinding(TextBox.TextProperty, bErstgewicht);

            //***********************************************************************
            var bZweitgewicht = new Binding("ZweitGewicht")
                                    {
                                        StringFormat = cFormat,
                                        Mode = BindingMode.TwoWay,
                                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                    };


            tb_ZweitGewicht.SetBinding(TextBox.TextProperty, bZweitgewicht);

            //***************************************************************
            var babrufsoll = new Binding("Sollmenge")
                                 {
                                     StringFormat = cFormat,
                                     Mode = BindingMode.TwoWay,
                                     UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                 };


            tb_abruf_soll.SetBinding(TextBox.TextProperty, babrufsoll);

            //***************************************************************
            var babrufist = new Binding("Istmenge")
                                {
                                    StringFormat = cFormat,
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                };
            tb_abruf_ist.SetBinding(TextBox.TextProperty, babrufist);

            //***************************************************************
            var babrufrest = new Binding("Restmenge")
                                 {
                                     StringFormat = cFormat,
                                     Mode = BindingMode.TwoWay,
                                     UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                 };
            tb_abruf_rest.SetBinding(TextBox.TextProperty, babrufrest);
        }

        #region Ribbonbutton-Click 
     
        private void MenuItem1Click(object sender, RoutedEventArgs e)
        {
            if (_oRfReceiver != null)
                _oRfReceiver.Close();


            netScaleView1.Close();
            Hide();
        }

        private void CmdSelectScaleClick(object sender, RoutedEventArgs e)
        {
            netScaleView1.ActiveScale = netScaleView1.ActiveScale == 1 ? 2 : 1;
        }

        private void CmdNeuClick(object sender, RoutedEventArgs e)
        {
     
            NewWaege();
            tb_frachtmittelkz.Text = "0";
            if (goApp.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN)
            {
                tb_Kfz1.Focus();
            }
            else
            {
                tb_kfzid.Focus();
            }
        }

        private void CmdCancelClick(object sender, RoutedEventArgs e)
        {
            MengenErfassung = false;
            _boWe = null;
            Wiegestatus = 0;
        }

        private void CmdDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_wiegestatus == 7) // Abruf bearbeiten
            {
                var oAbruf = new Abruf();
                AbrufEntity oAe = oAbruf.GetAbrufById(_boWe.Abrufid);
                if (oAe != null)
                {
                    oAbruf.DeleteEntity(oAe);
                }
            }

            if (_boWe != null)
            {
                MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (uRet == MessageBoxResult.Yes)
                {
                    if (_boWe.AbrechnungsKZ == "RG")
                    {
                        // boW.DeleteEntity(boWE);
                        _boWe.AbrechnungsKZ = "LO";
                        _boW.SaveEntity(_boWe);
                        Wiegestatus = 0;
                    }
                    else
                    {
                        _boW.DeleteEntity(_boWe);
                        Wiegestatus = 0;
                    }
                }
            }
        }

        private void CmdSaveClick(object sender, RoutedEventArgs e)
        {
            lblKfz1.Focus();
            Save();
        }

        private void CmdHoflisteClick(object sender, RoutedEventArgs e)
        {
            var oHlFrm = new HoflisteFrm();
            oHlFrm.ShowDialog();
            int uRet = oHlFrm.uRet;
            //TODO Verbessern!
            if (uRet == 0)
            {
                oHlFrm.Close();
            }
            else
            {
                _boWe = _boW.GetWaegungByID(uRet);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    oHlFrm.Close();
                    Wiegestatus = 2;

                    ChangeMandantComboboxItem(); // Hofliste
                    goApp.Mandant_PK = (int)_boWe.PK_Mandant;
                    
                }
            }
        }

        private void CmdWiegenClick(object sender, RoutedEventArgs e)
        {
            Wiegen();
        }

        private void CmdLsClick(object sender, RoutedEventArgs e)
        {
            var oWl = new WiegelisteFrm();
            oWl.ShowDialog();
            int uRet = oWl.uRet;
            if (uRet != 0)
            {
                _boWe = _boW.GetWaegungByID(uRet);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    Wiegestatus = 5;
                    ChangeMandantComboboxItem(); // LS Liste
                }
            }
            oWl.Close();
        }

        private void CmdImportClick(object sender, RoutedEventArgs e)
        {
            IImportInterface oI = new ImportOAM();
            oI.import(this);
            progressBar1.Width = 0;
            ProgressLabel.Width = 0;
        }
        #endregion

        private void NewWaege()
        {
            _boWe = null;
            _boWe = _boW.NewEntity();
            DataContext = _boWe;
            Wiegestatus = cb_ZweitWaegungPreset.IsChecked == true ? 2 : 1;

            _boWe.Mengeneinheit = goApp.MengenEinheit;
        }

        private void Wiegen()
        {
            tb_ZweitGewicht.Focus();
            var boE = new Einstellungen();
            Abruf oAbruf = new Abruf();
            RegisterWeight oRw = netScaleView1.RegisterGewicht();
            switch (_wiegestatus)
            {
                case 1:
                    if (oRw.Status != "80")
                    {
                        if (RfidAktiv)
                        {
                            tb_rfid.Text = "Wägung fehlgeschlagen, bitte wieder holen";
                            Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Background,
                                new ThreadStart(delegate { }));
                            _oRfReceiver.LastKfzId = "";

                            if (netScaleView1.oWF != null)
                                netScaleView1.oWF.SetRedLight();

                            return;
                        }
                        else
                        {
                            MessageBox.Show("Wägung fehlgeschlagen");
                            if (netScaleView1.oWF != null)
                                netScaleView1.oWF.SetRedLight();

                            netScaleView1.poll();
                            return;
                        }
                    }


                    _boWe.LN1 = oRw.Ln;
                    _boWe.ErstGewicht = oRw.weight;
                    _boWe.ErstZeit = oRw.Time;
                    _boWe.Waegung = 1;
                    _boWe.wnr1 = netScaleView1.ActiveScale.ToString();

               
                        _result = SaveEntity(_boW, _boWe);
           

                    if (_result != mmSaveDataResult.RulesPassed)
                    {
                        _boWe.LN1 = null;
                        _boWe.ErstGewicht = null;
                        return;
                    }

                    if (netScaleView1.oWF != null)
                        netScaleView1.oWF.SetGreenLight();
                    _boW.WriteToMischer(_boWe);
                    ShowAbrufMengen(oAbruf.GetAbrufById(_boWe.Abrufid));
                    Wiegestatus = 0;
                    break;
                case 2:

                    // Prüfen: Erstwägung ohne Zweitwägung

                    if (_boWe.Waegung == null)
                    {
                        var oCf = new CF();
                        CFEntity oCfe = oCf.GetCFByKennzeichen(_boWe.Kfz1);
                        if (oCfe != null)
                        {
                            if(_boWe.ErstGewicht==null)
                            _boWe.ErstGewicht = oCfe.Tara;
                            if (_boWe.ErstGewicht == 0)
                                _boWe.ErstGewicht = oCfe.Tara;
                        }
                    }

                    if (_boWe.ErstGewicht == 0 | _boWe.ErstGewicht == null)
                    {
                        if (RfidAktiv)
                        {
                            MessageBox.Show("Zweitwägung mit Erstgewicht 0 ist nicht möglich!");
                            return;
                        }
                        else
                        {
                            tb_rfid.Text = "Zweitwägung mit Erstgewicht 0 ist nicht möglich!";
                            _oRfReceiver.Kfzid = "";
                            return;
                        }
                    }

                    if (oRw.Status != "80")
                    {
                        if (RfidAktiv)
                        {
                            if (netScaleView1.oWF != null)
                                netScaleView1.oWF.SetRedLight();

                            tb_rfid.Text = "Wägung fehlgeschlagen, bitte wieder holen";
                            _oRfReceiver.LastKfzId = "";
                            return;
                        }
                        else
                        {
                            if (netScaleView1.oWF != null)
                                netScaleView1.oWF.SetRedLight();

                            MessageBox.Show("Wägung fehlgeschlagen");
                            netScaleView1.poll();
                            return;
                        }
                    }


                    _boWe.LN2 = oRw.Ln;
                    _boWe.ZweitGewicht = oRw.weight;
                    CalcNetto();


                    _boWe.LSNRGlobal = boE.NewLsNrGlobal();
                    _boWe.lsnr = boMandant.GetLsNr(_boWe);


                    _boWe.ZweitZeit = oRw.Time;
                    _boWe.LSDatum = oRw.Date;
                    _boWe.Waegung = 2;
                    _boWe.wnr2 = netScaleView1.ActiveScale.ToString();

                    try
                    {
                        _result = SaveEntity(_boW, _boWe);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    if (_result != mmSaveDataResult.RulesPassed)
                        return;


                    _boW.CalcAbrufMengen(_boWe);


                    ShowAbrufMengen(oAbruf.GetAbrufById(_boWe.Abrufid));

                    PrintLs();

                    if (netScaleView1.oWF != null)
                        netScaleView1.oWF.SetGreenLight();
                    Wiegestatus = 0;
                    ShowAbrufMengen(oAbruf.GetAbrufById(_boWe.Abrufid));
                    tb_Kfz1.Focus(); // Test wegen dem F2 Problem bei der ersten Wägung
                    break;

                case 3:
                    _boWe.LN2 = oRw.Ln;
                    _boWe.ErstGewicht = 0;
                    _boWe.ZweitGewicht = oRw.weight;

                    _boWe.Nettogewicht = _boWe.ZweitGewicht;

                    boE = new Einstellungen();
                    _boWe.LSNRGlobal = boE.NewLsNrGlobal();
                    _boWe.lsnr = boMandant.GetLsNr(_boWe);

                    _boWe.ZweitZeit = oRw.Time;
                    _boWe.LSDatum = oRw.Date;
                    _boWe.Waegung = 2;


                    try
                    {
                        _result = SaveEntity(_boW, _boWe);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    if (_result != mmSaveDataResult.RulesPassed)
                        return;

                    PrintLs();
                    Wiegestatus = 0;
                    break;
            }
        }

        private void Save()
        {
            var boA = new Abruf();
            tb_ZweitGewicht.Focus();
            if (_wiegestatus != 6 && _wiegestatus != 7)
            {
                if (_wiegestatus == 8 | _wiegestatus == 5) // LS Neu
                {
                    _boWe.Waegung = 2;

                    CalcNetto();
                }

                if (_wiegestatus == 8)
                {
                    var boE = new Einstellungen();
                    _boWe.LSNRGlobal = boE.NewLsNrGlobal();
                    _boWe.lsnr = boMandant.GetLsNr(_boWe);

                    
                    _boWe.ZweitZeit = DateTime.Now;

                    if(_boWe.LSDatum==null)
                        _boWe.LSDatum = DateTime.Today;


                                        _boWe.Waegung = 2;
                }

                try
                {
                    _result = SaveEntity(_boW, _boWe);
                    if(_wiegestatus != 4) // Erstwägung bearbeiten
                        if (MessageBox.Show("Lieferschein drucken?", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                        }
                        else
                        {
                            PrintLs();
                        }
                
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (_result != mmSaveDataResult.RulesPassed)
                    return;
            }
            if (_wiegestatus == 6)
            {
                decimal soll;
                decimal ist;
                decimal rest;
                Decimal.TryParse(tb_abruf_soll.Text, NumberStyles.Any, CultureInfo.CurrentCulture,
                                 out soll);
                Decimal.TryParse(tb_abruf_ist.Text, NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat, out ist);
                Decimal.TryParse(tb_abruf_rest.Text, NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat,
                                 out rest);
                AbrufEntity oAE = boA.CreateAbruf(_boWe, ist, soll, rest);
                _boWe.Abrufid = oAE.PK;
                _boWe.Abrufnr = oAE.Abrufnr;
            }

            if (_wiegestatus == 7)
            {
                // Ein wenig tricky : Die Abrufentity wird zurück gegeben, damit die  Restmenge bei Änderung gleich nach
                // dem Speichern angezeigt wird.

                var oAE = boA.SaveAbruf(_boWe);
                _boWe.Abrufid = oAE.PK;
                _boWe.Abrufnr = oAE.Abrufnr;
                ShowAbrufMengen(oAE);
            }

            //var boA = new Abruf();

            Wiegestatus = 0;
        }

        private void ShowAbrufMengen(AbrufEntity boAE)
        {
            if (boAE != null)
            {
                _boWe.Sollmenge = boAE.Sollmenge;
                _boWe.Istmenge = boAE.Istmenge;
                _boWe.Restmenge = boAE.Restmenge;
            }
        }

        private void CalcNetto()
        {
            if (_boWe.ErstGewicht <= _boWe.ZweitGewicht)
            {
                _boWe.Nettogewicht = _boWe.ZweitGewicht - _boWe.ErstGewicht;
            }
            if (_boWe.ErstGewicht >= _boWe.ZweitGewicht)
            {
                _boWe.Nettogewicht = _boWe.ErstGewicht - _boWe.ZweitGewicht;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T) child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
         
        private void DisableFrmTb()
        {
            foreach (object ctrl in LayoutRoot.Children)
            {
                if (ctrl is TextBox)
                    ((TextBox) ctrl).IsEnabled = false;
            }

            tb_mischersollwert.IsEnabled = false;


            foreach (object ctrl in LayoutRoot.Children)
            {
                if (ctrl is Button)
                    ((Button) ctrl).IsEnabled = false;
            }

            // Hier wird gezielt gesucht, zB Exrechnung
            foreach (TextBox tb in FindVisualChildren<TextBox>(this.ExRechnung))
            {
                tb.IsEnabled = false;
            }
        }

        private void EnableFrmTb()
        {
            foreach (object ctrl in LayoutRoot.Children)
            {
                if (ctrl is TextBox)
                    ((TextBox) ctrl).IsEnabled = true;
            }
            tb_mischersollwert.IsEnabled = true;
            // Hier wird gezielt gesucht, zB Exrechnung
            foreach (TextBox tb in FindVisualChildren<TextBox>(this.ExRechnung))
            {
                tb.IsEnabled = true;
            }
            foreach (object ctrl in LayoutRoot.Children)
            {
                if (ctrl is Button)
                    ((Button) ctrl).IsEnabled = true;
            }
        }


        // ************************************************************************************************************
        #region Wiegestatus-Methoden
        private void WiegeStatusChange(int ws)
        {
            switch (ws)
            {
                case 0:
                    Wiegestatus0();
                    break;
                case 1:
                    Wiegestatus1();
                    break;
                case 2:
                    Wiegestatus2();
                    break;
                case 3:
                    Wiegestatus3();
                    break;
                case 4:
                    Wiegestatus4();
                    break;
                case 5:
                    Wiegestatus5();
                    break;
                case 6:
                    Wiegestatus6();
                    break;
                case 7:
                    Wiegestatus7();
                    break;
                case 8:
                    Wiegestatus8();
                    break;
            }
        }

        private void Wiegestatus0()
        {
            if (_boWe != null)
                _boW.CancelEntity(_boWe);
            // 
            DataContext = _boWe;
            tb_wiegestatus.Text = "...";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = false;
            radioAbrufEdit.IsChecked = false;
            radioLSNew.IsChecked = false;
            radioLSNew.IsEnabled = false;

            rb_wiegeart.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = false;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsChecked = false;

            DisableFrmTb();
            cmdNeu.IsEnabled = true;
            cmdHofliste.IsEnabled = true;
            cmdLS.IsEnabled = true;
            cmdAbrufListe.IsEnabled = true;
            cmdCancel.IsEnabled = false;
            cmdDelete.IsEnabled = false;

            cmdSave.IsEnabled = false;
            cmdWiegen.IsEnabled = false;
            RfidAktiv = false;

            cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Wiegestatus1()
        {
            // Waagenumschaltung
            netScaleView1.ActiveScale = Int32.Parse(goApp.ErstW);

            tb_wiegestatus.Text = "Erstwägung";
            rb_wiegeart.IsChecked = true;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = false;
            radioAbrufEdit.IsChecked = false;
            radioLSNew.IsChecked = false;


            rb_wiegeart.IsEnabled = true;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = true;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsEnabled = true;

            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = true;
            cmdLS.IsEnabled = false;
            cmdAbrufListe.IsEnabled = true;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = false;

            cmdSave.IsEnabled = false;
            cmdWiegen.IsEnabled = true;
            cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Wiegestatus2()
        {
            netScaleView1.ActiveScale = Int32.Parse(goApp.ZweitW);
            tb_wiegestatus.Text = "Zweitwägung";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = true;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;

            rb_wiegeart.IsEnabled = false;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = true;
            radioLSEdit.IsEnabled = false;


            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = true;
            cmdLS.IsEnabled = false;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = false;
            cmdWiegen.IsEnabled = true;
            cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Wiegestatus3() // Einmal
        {
            tb_wiegestatus.Text = "Einmal-/Kontrollwägung";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = true;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;

            rb_wiegeart.IsEnabled = true;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;


            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = true;
            cmdLS.IsEnabled = false;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = false;
            cmdWiegen.IsEnabled = true;
            cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Wiegestatus4()
        {
            tb_wiegestatus.Text = "Erstwägung bearbeiten";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = true;
            radioLSEdit.IsChecked = false;

            rb_wiegeart.IsEnabled = false;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = true;
            radioLSEdit.IsEnabled = false;


            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = true;
            cmdLS.IsEnabled = false;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = true;
            cmdWiegen.IsEnabled = false;
            cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
        }

        // Erstwägung bearbeiten

        private void Wiegestatus5() // LS bearbeiten
        {
            tb_wiegestatus.Text = "Lieferschein bearbeiten";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = true;

            rb_wiegeart.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = true;


            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = false;
            cmdLS.IsEnabled = true;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = true;
            cmdWiegen.IsEnabled = false;

            // Damit immer die aktuellen Werte angezeigt werden
            Abruf oAbruf = new Abruf();
            ShowAbrufMengen(oAbruf.GetAbrufById(_boWe.Abrufid));
            cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Wiegestatus6() // Abruf anlegen
        {
            tb_wiegestatus.Text = "Abruf anlegen";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = true;
            radioAbrufEdit.IsChecked = false;

            rb_wiegeart.IsEnabled = true;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = true;
            radioAbrufEdit.IsEnabled = false;


            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = false;
            cmdLS.IsEnabled = false;
            cmdAbrufListe.IsEnabled = false;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = true;
            cmdWiegen.IsEnabled = false;

            tb_kfzid.IsEnabled = false;
            tb_Kfz1.IsEnabled = false;
            tb_Kfz2.IsEnabled = false;
            cb_Abruffest.Visibility = System.Windows.Visibility.Visible;
        }

        private void Wiegestatus7() // Abruf bearbeiten
        {
            tb_wiegestatus.Text = "Abruf bearbeiten";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = false;
            radioAbrufEdit.IsChecked = true;
            radioLSNew.IsChecked = false;

            rb_wiegeart.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = false;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsEnabled = false;

            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = false;
            cmdLS.IsEnabled = false;
            cmdAbrufListe.IsEnabled = false;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = true;
            cmdWiegen.IsEnabled = false;

            tb_kfzid.IsEnabled = false;
            tb_Kfz1.IsEnabled = false;
            tb_Kfz2.IsEnabled = false;
            cb_Abruffest.Visibility = System.Windows.Visibility.Visible;
        }

        private void Wiegestatus8() // LS NEU
        {
            tb_wiegestatus.Text = "Lieferschein händisch anlegen";
            rb_wiegeart.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioLSNew.IsChecked = true;

            rb_wiegeart.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioLSNew.IsEnabled = true;

            EnableFrmTb();
            cmdNeu.IsEnabled = false;
            cmdHofliste.IsEnabled = false;
            cmdLS.IsEnabled = true;
            cmdCancel.IsEnabled = true;
            cmdDelete.IsEnabled = true;

            cmdSave.IsEnabled = true;
            cmdWiegen.IsEnabled = false;
        }
        #endregion 

        private void RbWiegeartClickRadioClick(object sender, RoutedEventArgs e)
        {
            if (rb_wiegeart.IsChecked == true)
                Wiegestatus = 1;
            else if (radioZweit.IsChecked == true)
                Wiegestatus = 2;
            else if (radioEinmal.IsChecked == true)
                Wiegestatus = 3;
            else if (radioErstEdit.IsChecked == true)
                Wiegestatus = 4;
            else if (radioLSEdit.IsChecked == true)
                Wiegestatus = 5;
            else if (radioAbrufNeu.IsChecked == true)
                Wiegestatus = 6;
            else if (radioAbrufEdit.IsChecked == true)
                Wiegestatus = 7;
            else if (radioLSNew.IsChecked == true)
                Wiegestatus = 8;
        }


        private void tb_abrufnr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab | e.Key == Key.Return)
                CheckAbrufNr();

            if (e.Key == Key.F4)
                cmdAbrufListe.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void tb_abrufnr_LostFocus(object sender, RoutedEventArgs e)
        {
            var x = _wiegestatus;
            CheckAbrufNr();
        }

        private void CheckAbrufNr()
        {
            if (tb_abrufnr.Text.Length == 0)
                return;

            var oA = new Abruf();
            AbrufEntity oAe = oA.GetAbrufByNr(tb_abrufnr.Text);

            if (oAe == null)
            {
                if (!VFP.InList(_wiegestatus,6,7))
                {
                    MessageBox.Show("Diese Abruf-Nummer gibt es nicht!", "ACHTUNG", MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                    _boWe.Abrufid = null;
                    _boWe.Abrufnr = null;
                    return;
                }
            }

            if (_wiegestatus == 6 | _wiegestatus == 7)
                return;


            oA.CopyAbrufToWaege(oAe.PK, _boWe);
            DataContext = oA.CopyAbrufToWaege(oAe.PK, _boWe);
            ChangeMandantComboboxItem();
        }

        // Ich denke, das man einen großen Teil der folgenden Methoden ins BO Waege verlegen sollte. BL raus aus der GUI
        // Zunächst also Incoterm,Wiegeart, AP Kunde

        // Mandanten-Methoden
        private void FillMandantCombobox()
        {
            // Zum Verbinden mehrer Spalten in der Combobox habe ich einen entsprechenden View auf
            // den SQL-Server erstellt. Nicht gerade elegant, aber es tut es...
            var oM = new SvMandant();
            SvMandantEntity oMe = oM.GetDefaultMandant();

            mmBindingList<SvMandantEntity> oMel = oM.GetAllMandant();
            goApp.Mandant_PK = oMe.PK;
            goApp.MandantNr = oMe.MandantNr;
            cb_Mandant.ItemsSource = oMel;
            cb_Mandant.SelectedValuePath = "PK";
            cb_Mandant.DisplayMemberPath = "MNrName";
            cb_Mandant.Text = oMe.MNrName;
        }

        // PK wurde aus der Waegetabelle gelesen  und damit die Combobox umschalten. Also Hofliste,LS bearbeiten etc
        private void ChangeMandantComboboxItem()
        {
            int? pkMandant = _boWe.PK_Mandant;
          

               
            var oM = new SvMandant();
          
            if (pkMandant != null)
            {
                SvMandantEntity oMe = oM.GetMandantByPK((int)pkMandant);
                cb_Mandant.SelectedValue = oMe.MandantNr;
               
                if (oMe == null) throw new NotImplementedException();
                cb_Mandant.Text = oMe.MNrName; // <--- Hiermit wird die Anzeige in der Combobox geändert
               
            }
          
        }

        // Umschaltung der Combobox aktualisiert in der Waegeentitität
        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Mandant.SelectedValue!=null)
            goApp.Mandant_PK = Convert.ToInt32(cb_Mandant.SelectedValue.ToString());

          

            if (_boWe != null)
            {
                goApp.Mandant_PK = Convert.ToInt32(cb_Mandant.SelectedValue);
                goApp.MandantNr = boMandant.GetMandantByPK(goApp.Mandant_PK).MandantNr;
          
            }
        }


        // Auftrags Methoden
        private void LooUpAndFillAuftrag(string matchcode)
        {
            var oAlFrm = new AuftragsListeFrm(matchcode);
       

            oAlFrm.ShowDialog();
           

            int uRetKMPK = oAlFrm.uRet;
            _boW.AuftragDetail2Waege(uRetKMPK, _boWe);
            oAlFrm.Close();
        }

        #region AP Methoden

        private void LookUpAndFillKu()
        {
            var oApFrm = new APFrm(tb_FirmaKU.Text, Partnerrollen.GetRollenByWiegeart(_boWe.WiegeartKz));
            oApFrm.ShowDialog();
            int uRet = oApFrm.uRet;
            oApFrm.Close();

            if (uRet == 0)
                return;

            _boW.FillApKu(uRet, _boWe);
        }


        private void TbNrKuKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var oAp = new AP();
                APEntity oApe = oAp.GetAPByNr(tb_NrKU.Text, "AU");
                if (oApe != null)
                    _boW.FillApKu(oApe.PK, _boWe);
            }
        }

      

        private void LookUpAndFillFu()
        {
            var oApFrm = new APFrm(tb_FirmaFU.Text, "FU");
            oApFrm.ShowDialog();
            int uRet = oApFrm.uRet;
            oApFrm.Close();

            if (uRet == 0)
                return;
            _boW.FillApFu(uRet, _boWe);
         
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
                    _boW.FillApFu(oApe.PK,_boWe);
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
        #endregion

#region MG Methoden
        public void LookUpAndFillMg(string mc)
        {
            var oMgFrm = new MGListFrm(mc);
            oMgFrm.ShowDialog();
            int uRet = oMgFrm.uRet;
            oMgFrm.Close();

            if (uRet == 0)
                return;

            var boMg = new MG();
            MGEntity boMge = boMg.GetMGById(uRet);
            FillMg(boMge);
        }

        private void FillMg(MGEntity boMge)
        {
            tb_SortenNr.Text = boMge.SortenNr;
            tb_Sortenbezeichnung1.Text = boMge.Sortenbezeichnung1;
            tb_Sortenbezeichnung2.Text = boMge.Sortenbezeichnung2;
            tb_Sortenbezeichnung3.Text = boMge.Sortenbezeichnung3;
          //  if(_boWe.preisvk == 0) // Neu 15102013
            _boWe.preisvk = boMge.preisvk; // Bis hier ok
            _boWe.me = boMge.me;
            if (_boWe.me == "Stk")
            {
                MengenErfassung = true;
            }

            else
            {
                MengenErfassung = false;
            }
            _boWe.Siegel1 = boMge.Siegel1;
            _boWe.Siegel2 = boMge.Siegel2;
            _boWe.Siegel3 = boMge.Siegel3;
            _boWe.Siegel4 = boMge.Siegel4;
        }

        private void ClearMg()
        {
            tb_SortenNr.Text = "";
            tb_Sortenbezeichnung1.Text = "";
            tb_Sortenbezeichnung2.Text = "";
        }
#endregion 
        // Wiegeart
        private void TbWiegeartLostFocus(object sender, RoutedEventArgs e)
        {
            if (_boWe.WiegeartKz != null)
            {
                if (_boWe.WiegeartKz == "W") // Einmalwägung
                {
                    Wiegestatus = 3;
                }
                _boW.FillWiegeart(_boWe);
            }
        }

        // Das ist ein Workaround um Clicksevent auf den Ribbontabs auszuwerten.
        private void RibbonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string expr = Ribbon.SelectedItem.ToString().ToUpper();
            if (Regex.IsMatch(expr, "STAMMDATEN"))
            {
            }
            if (Regex.IsMatch(expr, "WIEGEBETRIEB"))
            {
            }
        }

        private void PrintLs()
        {
            var oLe = new Lokaleeinstellungen();
            oLe = oLe.Load();
            var oPls = new PrinterLS();
            oPls.DoPrintLS(oLe, _boWe, false);
        }

        private bool IsKfzErstVerwogenDialog()
        {
            int uRet = _boW.IsKfzErstVerwogen(tb_Kfz1.Text);
            if (uRet != 0 && _wiegestatus == 1) // Ungleich 0 bedeutet ist Erstgewogen
            {
                _boWe = _boW.GetWaegungByID(uRet);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    Wiegestatus = 2;
                    MessageBox.Show(
                        "Dieses Fahrzeug ist schon erstgewogen. Das Programm schaltet aus diesem Grund nun automatisch auf Zweitwägung um!",
                        "Achtung - Wichtiger Hinweis", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ChangeMandantComboboxItem();
                }
            }


            if (uRet != 0 && _wiegestatus == 2)
            {
                _boWe = _boW.GetWaegungByID(uRet);
                DataContext = _boWe;
            }
            if (uRet != 0) // Erstgewogen
            {
                return true;
            }
            return false;
        }

        // Für UHF Fernbedienung EFS-12HCS
        public void SetTelegrammBinding()
        {
            // TODO Das Ganze in XAML übersetzen !!! Beim nächsten Projekt :-)
            // Achtung: tb_kfzid Tricky
            // Hier liegt ein Multibinding vor. Zum einem an das Property (im XAML Code) in der WaegeEntity und zum zweiten
            // an kfzid in oReceiver via Code behind
            // Das Auslösen der Fernbedienung füllt ggf. das Property im oReceiver.

            //TODO 3.7.2013 Stattdessen könnte man auf das Oberserver Pattern umstellen! Siehe auch Gewicht in NetScaleView 
            // http://www.code-magazine.com/Article.aspx?quickid=0907101
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////77
            var bTb1 = new Binding
                           {
                               Source = _oRfReceiver,
                               Path = new PropertyPath("Kfzid"),
                               Mode = BindingMode.OneWay,
                               NotifyOnTargetUpdated = true,
                               UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                           };

            tb_kfzid.SetBinding(OriginalTextProperty, bTb1);
            tb_kfzid.TargetUpdated += TbTelegrammTargetUpdated;
        }


        private void TbTelegrammTargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (_oRfReceiver.Kfzid != null && _oRfReceiver.Kfzid != "")
            {
                RfidAktiv = true;
                // DoEvent Ersatz damit in der Wägemmaske der Remotecontrolstatus angezeigt. Siehe auch den 500ms Sleep
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));

                System.Threading.Thread.Sleep(500);
            }

            else
            {
                return;
            }


            // 1.KFZ-Daten abholen

            var oCf = new CF();
            CFEntity oCfe = oCf.GetCFById(_oRfReceiver.Kfzid);
            if (oCfe == null)
                return;

            // 2. Prüfen, ob das Fahrzeug schonerstverwogen ist
            int wPk = _boW.IsKfzErstVerwogen(oCfe.Kfz1);
            if (wPk == 0) // Neu DS anlegen und erstwiegen
            {
                NewWaege();
                _boWe.KfzID = oCfe.KfzID;
                _boWe.Kfz1 = oCfe.Kfz1;

                Abruf2Wage(oCfe);
            }
            else // DS holen und zweitwiegen
            {
                _boWe = _boW.GetWaegungByID(wPk);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    Wiegestatus = 2;
                }
            }

            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new ThreadStart(delegate { }));

            System.Threading.Thread.Sleep(500);
            Wiegen();
            _boW.SaveEntity(_boWe);
        }

        private void Abruf2Wage(CFEntity oCfe)
        {
            int? uRet = oCfe.abruf_PK;
            var boAbruf = new Abruf();
            WaegeEntity dc = boAbruf.CopyAbrufToWaege(uRet, _boWe);
            if (dc != null)
            {
                DataContext = dc;
                //  _wiegestatus = 1;
            }
        }

        private void CmdAbrufListeClick(object sender, RoutedEventArgs e)
        {
            var oAbrufFrm = new AbruflisteFrm();
            oAbrufFrm.ShowDialog();
            int uRet = oAbrufFrm.URet;


            if (uRet == 0)
            {
                _wiegestatus = 0;
                oAbrufFrm.Close();
                return;
            }
            if (_wiegestatus == 0) // Abruf bearbeiten
            {
                _boWe = null;
                _boWe = _boW.NewEntity();
                var boAbruf = new Abruf();
                DataContext = boAbruf.CopyAbrufToWaege(uRet, _boWe);

                Wiegestatus = 7;
            }
            if (_wiegestatus == 1 | _wiegestatus == 2 | _wiegestatus == 3)
            {
                var boAbruf = new Abruf();
                DataContext = boAbruf.CopyAbrufToWaege(uRet, _boWe);
                ChangeMandantComboboxItem(); //  Abrufliste
            }
            ChangeMandantComboboxItem(); // Hofliste
        
            oAbrufFrm.Close();
        }

        #region Kfz Methoden
        private void LookupKfzId()
        {
            var oCf = new CF();
            CFEntity oCfe = oCf.GetCFByKfzId(tb_kfzid.Text);
            if (oCfe != null)
            {
                FillKfz(oCfe.PK);
                bool lRet = IsKfzErstVerwogenDialog();
                if (lRet) // Dann mach nichts
                {
                }
                else // Schau mal ob das Kfz einen Abruf hat, den man bei der Erstwägung in die Maske schießen kann
                {
                    Abruf2Wage(oCfe);
                    ChangeMandantComboboxItem();
                }
            }
            else
            {
                tb_kfzid.Text = "";
                tb_Kfz1.Text = "";
            }
        }

        private void LookupKfzKennzeichen()
        {
            var oCf = new CF();
            CFEntity oCfe = oCf.GetCFByKennzeichen(tb_Kfz1.Text);
            if (oCfe != null)
            {
                tb_kfzid.Text = oCfe.KfzID;
                FillKfz(oCfe.PK);
                bool lRet = IsKfzErstVerwogenDialog();
                if (lRet) // Dann mach nichts
                {
                }
                else // Schau mal ob das Kfz einen Abruf hat, den man bei der Erstwägung in die Maske schießen kann
                {
                    Abruf2Wage(oCfe);
                    ChangeMandantComboboxItem(); 
                }
            }
        }

        private void FillKfz(int pk)
        {
            var oCf = new CF();
            CFEntity oCfe = oCf.GetCFByPK(pk);
            tb_kfzid.Text = oCfe.KfzID;
            tb_Kfz1.Text = oCfe.Kfz1;
            tb_Kfz2.Text = oCfe.Kfz2;
            tb_NrSP.Text = oCfe.NrSP;

            tb_FirmaSP.Text = oCfe.FirmaSP;

            _boW.FillApFu(oCfe.ap_PKFU, _boWe);

            //tb_NrFU.Text = oCfe.NrFU;
            //tb_FirmaFU.Text = oCfe.FirmaFU;

            if (oCfe.MaxLademenge != null)
            {
                if (oCfe.MaxLademenge > 0)
                    tb_mischersollwert.Text = oCfe.MaxLademenge.ToString();
            }
        }
        #endregion 

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.HideCloseButton();
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        private void SetToolTips()
        {
            var oKb = new KbFnn();
            KbFnnEntity oKbe = oKb.GetKbFnnByActionNr("2");
            if (oKbe != null)
                cmdNeu.ToolTip = oKbe.Command + " " + oKbe.ToolTip;
            oKbe = oKb.GetKbFnnByActionNr("3");
            if (oKbe != null)
                cmdHofliste.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("5");
            if (oKbe != null)
                cmdLS.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("6");
            if (oKbe != null)
                cmdAbrufListe.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("7");
            if (oKbe != null)
                cmdDelete.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("8");
            if (oKbe != null)
                cmdSave.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("11");
            if (oKbe != null)
                cmdSelectScale.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("13");
            if (oKbe != null)
                cmdWiegen.ToolTip = oKbe.Command + " " + oKbe.ToolTip;

            oKbe = oKb.GetKbFnnByActionNr("20");
            if (oKbe != null)
                cmdCancel.ToolTip = oKbe.Command + " " + oKbe.ToolTip;
        }


        // TODO: Ordentlich machen
        private void SwitchExRechnung()

        {
            switch (tb_Bonitaetkz.Text.Trim())
            {
                case "7":
                    ExRechnung.IsExpanded = true;
                    ExRechnung.IsEnabled = true;
                    break;
                case "5":
                    ExRechnung.IsExpanded = true;
                    ExRechnung.IsEnabled = true;
                    break;

                case "8":
                    ExRechnung.IsExpanded = false;
                    ExRechnung.IsEnabled = false;
                    break;
                default:
                    ExRechnung.IsExpanded = false;
                    ExRechnung.IsEnabled = false;
                    break;
            }
        }


        #region Tastatur-Handling
        // TODO: Sortieren wie sie in der Wägesmaske erscheinen

        private void tb_KfzidKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return | e.Key == Key.Tab)
            {
                LookupKfzId();
            }
            if (e.Key.ToString() == "F4")
            {
                var oCfFrm = new CFListFrm(true,tb_Kfz1.Text);

                oCfFrm.ShowDialog();
                int pk = oCfFrm.uRet;
                tb_Kfz1.Focus();

                if (pk != 0)
                {
                    FillKfz(pk);
                }

                oCfFrm.Close();
                LookupKfzKennzeichen();
            }
        }
        private void tb_Kfz1KeyDown(object sender, KeyEventArgs e) //
        {
            if (e.Key == Key.Return | e.Key == Key.Tab)
            {
                LookupKfzKennzeichen();
            }

            if (e.Key.ToString() == "F4")
            {
                var oCfFrm = new CFListFrm(true, tb_Kfz1.Text);

                oCfFrm.ShowDialog();
                int pk = oCfFrm.uRet;
                tb_Kfz1.Focus();

                {
                    if (pk != 0)
                    {
                        FillKfz(pk);
                    }

                    oCfFrm.Close();
                    LookupKfzKennzeichen();
                }
            }
        }

        private void tb_NrSpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return | e.Key == Key.Tab)
            {
                var oAp = new AP();
                APEntity oApe = oAp.GetAPByNr(tb_NrSP.Text, "SP");
                if (oApe != null)
                    FillApSp(oApe);
                else
                {
                    ClearApSp();
                }
            }
        }
        private void TbFirmaFuKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "F4")
                LookUpAndFillFu();
        }

        private void TbFirmaSpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F4)
                LookUpAndFillSp();
        }
        

        private void TbSortenNrKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var oMg = new MG();
                MGEntity oMge = oMg.GetMGByNr(tb_SortenNr.Text);
                if (oMge != null)
                {
                    FillMg(oMge);
                }
                else
                {
                    ClearMg();
                }
            }
        }

     

        private void TbFirmaKuKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4:

                    LookUpAndFillKu();

                    break;
                case Key.Return:

                    LooUpAndFillAuftrag(tb_FirmaKU.Text);
                    tb_FirmaKU.Focus();

                    break;
            }
        }

        private void TbSortenbezeichnung1KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "F4")
                LookUpAndFillMg(tb_Sortenbezeichnung1.Text);
        }

        private void TbErstGewichtGotFocus(object sender, RoutedEventArgs e)
        {
            if (_boWe.LN1 != null)
                tb_ErstGewicht.IsEnabled = false;
        }

        private void TbZweitGewichtGotFocus(object sender, RoutedEventArgs e)
        {
            if (_boWe.LN2 != null)
                tb_ZweitGewicht.IsEnabled = false;
        }

        private void tb_Bonitaetkz_TextChanged(object sender, TextChangedEventArgs e)
        {
            Bonitaet oB = new Bonitaet();
            BonitaetEntity oBE = oB.GetBonitaetByKz(tb_Bonitaetkz.Text);
            if (_boWe != null)
            {
                if (oBE != null)
                {
                    _boWe.BonitaetBezeichnung = oBE.Bezeichnung;
                    SwitchExRechnung();
                }
                else
                    _boWe.BonitaetBezeichnung = "";
                SwitchExRechnung(); 
               
            }
            else
            {
                SwitchExRechnung(); 
            }
        }

        private void tb_frachtmittelkz_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_frachtmittelkz.Text.Length == 1)
            {
                Frachtmittel oF = new Frachtmittel();
                FrachtmittelEntity oFE = oF.GetFrachtmittelByKz(tb_frachtmittelkz.Text);
                if (oFE != null)
                    tb_frachtmittelbezeichnung.Text = oFE.Bezeichnung;
                else
                {
                    tb_frachtmittelkz.Text = "";
                    tb_frachtmittelbezeichnung.Text = "";
                }
            }
        }

        private void tb_SortenNr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_SortenNr.Text.Length == 6)
            {
                var oMg = new MG();
                MGEntity oMge = oMg.GetMGByNr(tb_SortenNr.Text);
                if (oMge != null)
                {
                  //  FillMg(oMge); // Änderungen  Am 6.2.2014 auskommentiert
                }
                else
                {
                    ClearMg();
                }
            }
        }

        private void tb_PreisMG_TextChanged(object sender, TextChangedEventArgs e)
        {
            // tb_PreisMG.Text = WindowExtensions.TextBoxOnlyDecimal(e, tb_PreisMG.Text);
            if(_boWe!=null)
            _boW.CalcRechnung();
        }

        private void tb_FrachtPreis_TextChanged(object sender, TextChangedEventArgs e)
        {
          //  tb_FrachtPreis.Text = WindowExtensions.TextBoxOnlyDecimal(e, tb_FrachtPreis.Text);
            if(_boW!=null)
            _boW.CalcRechnung();
        }
      

        private void tb_Skonto_proz_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_Skonto_proz.Text = WindowExtensions.TextBoxOnlyDecimal(e, tb_Skonto_proz.Text);
            _boW.CalcRechnung();
        }

        private void tb_ustbetrag_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        #endregion

        // TODO Eventuell die UP and Down Tasten auswerten
        private void MyPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            string ee = e.Key.ToString();
            var oFnn = new KbFnn();
            KbFnnEntity oFnnE = oFnn.GetKbFnnByKey(ee);
            if (oFnnE != null)
                DoAction(oFnnE.ActionNr, e);
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

        private void KBUp()
        {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                                                 Key.Up);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
        }

        private void KBTab()
        {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                                                 Key.Tab);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
        }

        public void DoAction(string actionNr, KeyEventArgs e)
        {
            switch (actionNr)
            {
                case "2":
                    if (cmdNeu.IsEnabled)
                        CmdNeuClick(cmdNeu, e);
                    break;
                case "3":
                    if (cmdHofliste.IsEnabled)
                    {
                        CmdHoflisteClick(cmdHofliste, e);
                    }
                    break;
                case "5":
                    if (cmdLS.IsEnabled)
                    {
                        CmdLsClick(cmdLS, e);
                    }
                    break;
                case "6":
                    if (cmdAbrufListe.IsEnabled)
                    {
                        CmdAbrufListeClick(cmdAbrufListe, e);
                    }
                    break;
                case "7":
                    if (cmdDelete.IsEnabled)
                    {
                        CmdDeleteClick(cmdDelete, e);
                    }
                    break;
                case "8":
                    if (cmdSave.IsEnabled)
                    {
                        CmdSaveClick(cmdSave, e);
                    }
                    break;
                case "11":
                    if (cmdSelectScale.IsEnabled)
                    {
                        CmdSelectScaleClick(cmdSelectScale, e);
                    }
                    break;
                case "13":
                    if (cmdWiegen.IsEnabled)
                    {
                        CmdWiegenClick(cmdWiegen, e);
                    }
                    break;

                case "20":
                    if (cmdCancel.IsEnabled)
                    {
                        CmdCancelClick(cmdCancel, e);
                    }
                    break;
            }
        }

        private void cmdExport_Click(object sender, RoutedEventArgs e)
        {
            TaabFrm oTFrm = new TaabFrm();
            oTFrm.ShowDialog();
            oTFrm.Close();
        }

        public void ShowPrice()
        {
        }

        private void tb_wiegeart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_boWe != null)
            {
                if (_boWe.WiegeartKz != null)
                {
                    if (_boWe.WiegeartKz == "W") // Einmalwägung
                    {
                        Wiegestatus = 3;
                    }
                    _boW.FillWiegeart(_boWe);
                }
            }
        }


    }
}