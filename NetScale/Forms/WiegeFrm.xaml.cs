using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HardwareDevices;
using HWB.NETSCALE.BOEF;

using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO;
using NetScalePolosIO;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.WPF;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

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
        private mmSaveDataResult _result;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public WiegeFrm()
        {
            this.InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            _boW = new Waege();
            DataContext = _boWe;

            // Mandant
            goApp.Mandant_PK = new Mandant().GetDefaultMandant().PK;


            // Achtung - Hier wird der Observer abonniert
            netScaleView1.OnWeightChanged += new WeightChangedHandler(ShowEventGewichtHasChanged);
            DisplayErrorDialog = true;
            DisplayErrorProvider = true;
            RegisterPrimaryBizObj(_boW);


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

            Wiegestatus = 99;
            Wiegestatus = 0;

            SetWeightBindingFormat();
            FillIncoterms();
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
            NewWaege();
        }
      

        private void ribbonSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void ribbonWiegen_Click(object sender, RoutedEventArgs e)
        {
            Wiegen();
        }

        private void ribbonHofliste_Click(object sender, RoutedEventArgs e)
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
                _boWe = _boW.GetWaegungByPk(uRet);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    oHlFrm.Close();
                    Wiegestatus = 2;
                }
            }
        }

        private void ribbonLsListe_Click(object sender, RoutedEventArgs e)
        {
            var oLSListe = new WiegelisteFrm();
            oLSListe.ShowDialog();
            int uRet = oLSListe.uRet;
            if(uRet ==0)
            {
                oLSListe.Close();
            }
            else
            {
                _boWe = _boW.GetWaegungByPk(uRet);
                if(_boWe!=null)
                {
                    DataContext = _boWe;
                    oLSListe.Close();
                    Wiegestatus = 5;
                }
            }
        }


        private void ribbonDelete_Click(object sender, RoutedEventArgs e)
        {
            //if (_wiegestatus == 7) // Abruf bearbeiten
            //{
            //    var oAbruf = new Abruf();
            //    AbrufEntity oAe = oAbruf.GetAbrufById(_boWe.Abrufid);
            //    if (oAe != null)
            //    {
            //        oAbruf.DeleteEntity(oAe);
            //    }
            //}

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

        private void ribbonCancel_Click(object sender, RoutedEventArgs e)
        {
            _boW.CancelEntity(_boWe);
            
            Wiegestatus = 0;
        }




        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            netScaleView1.Close();
                 Cancel();
            Close();
        }


        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {
            new ImportExportPolos().Import();
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
            AdressenListeFrm oA = new AdressenListeFrm();
            oA.ShowDialog();
            if (oA != null)
            {
            }
            oA.Close();
        }

        private void cmdKFZ_Click(object sender, RoutedEventArgs e)
        {
            CFListFrm oCFFrm = new CFListFrm(true, "");
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
            if (_boWe == null)
            {
                return;
            }

            AuftragsListeV2 oAFrm = new AuftragsListeV2("");
            oAFrm.ShowDialog();
            int uRet = oAFrm.URet;
            _boW.Auftrag2Waege(uRet);

            oAFrm.Close();
        }

        #endregion

      

        #region GUI Umschaltung

        private void RadioClick(object sender, RoutedEventArgs e)
        {
            if (radioErst.IsChecked == true)
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

                if (ctrl is Button)
                    ((Button) ctrl).IsEnabled = false;


                if (ctrl is Expander)
                    ((Expander) ctrl).IsEnabled = false;

                if (ctrl is SplitButton)
                    ((SplitButton) ctrl).IsEnabled = false;
                
            }


            // Hier wird gezielt gesucht, zB Exrechnung
            //foreach (TextBox tb in FindVisualChildren<TextBox>(this.expanderCustomerEdit))
            //{
            //    this.expanderCustomerEdit.IsExpanded = false;
            //    tb.IsEnabled = false;
            //}
        }

        private void EnableFrmTb()
        {
            foreach (object ctrl in LayoutRoot.Children)
            {
                if (ctrl is TextBox)
                    ((TextBox) ctrl).IsEnabled = true;

                if (ctrl is Button)
                    ((Button) ctrl).IsEnabled = true;

                if (ctrl is Expander)
                    ((Expander) ctrl).IsEnabled = true;

                if (ctrl is SplitButton)
                    ((SplitButton)ctrl).IsEnabled = true;
            }


          

        //    foreach (object ctrl in LayoutRoot.Children)


        //        //foreach (TextBox tb in FindVisualChildren<TextBox>(this.expanderCustomerEdit))
        //        //{
        //        //    this.expanderCustomerEdit.IsExpanded = false;
        //        //    tb.IsEnabled = true;
        //        //}
        //
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
            var bErstgewicht = new Binding("Erstgewicht")
            {
                StringFormat = cFormat,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };


            tb_ErstGewicht.SetBinding(TextBox.TextProperty, bErstgewicht);

            //***********************************************************************
            var bZweitgewicht = new Binding("Zweitgewicht")
            {
                StringFormat = cFormat,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            tb_ZweitGewicht.SetBinding(TextBox.TextProperty, bZweitgewicht);




        }


        // ************************************************************************************************************
        private int Wiegestatus
        {
            set
            {
                if (value != _wiegeStatus)
                {
                    _wiegeStatus = value;
                    WiegeStatusChange(_wiegeStatus);
                }
            }
        }

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
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = false;
            radioAbrufEdit.IsChecked = false;
            radioLSNew.IsChecked = false;
            radioLSNew.IsEnabled = false;

            radioErst.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = false;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsChecked = false;

            DisableFrmTb();
            ribbonNeu.IsEnabled = true;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = true;
            //   ribbonAbrufListe.IsEnabled = true;
            ribbonCancel.IsEnabled = false;
            ribbonDelete.IsEnabled = false;

            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = false;
        }

        private void Wiegestatus1()
        {
            // Waagenumschaltung
            netScaleView1.ActiveScale = Int32.Parse(goApp.ErstW);

            tb_wiegestatus.Text = "Erstwägung";
            radioErst.IsChecked = true;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = false;
            radioAbrufEdit.IsChecked = false;
            radioLSNew.IsChecked = false;


            radioErst.IsEnabled = true;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = true;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsEnabled = true;

            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = false;
            //  cmdAbrufListe.IsEnabled = true;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = false;

            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = true;
        }

        private void Wiegestatus2()
        {
            netScaleView1.ActiveScale = Int32.Parse(goApp.ZweitW);
            tb_wiegestatus.Text = "Zweitwägung";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = true;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;

            radioErst.IsEnabled = false;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = true;
            radioLSEdit.IsEnabled = false;


            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = true;
        }

        private void Wiegestatus3() // Einmal
        {
            tb_wiegestatus.Text = "Einmal-/Kontrollwägung";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = true;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;

            radioErst.IsEnabled = true;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;


            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = true;
        }

        private void Wiegestatus4()
        {
            tb_wiegestatus.Text = "Erstwägung bearbeiten";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = true;
            radioLSEdit.IsChecked = false;

            radioErst.IsEnabled = false;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = true;
            radioLSEdit.IsEnabled = false;


            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;
        }

        // Erstwägung bearbeiten

        private void Wiegestatus5() // LS bearbeiten
        {
            tb_wiegestatus.Text = "Lieferschein bearbeiten";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = true;

            radioErst.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = true;


            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = false;
            ribbonLsListe.IsEnabled = true;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;

            // Damit immer die aktuellen Werte angezeigt werden
            // Abruf oAbruf = new Abruf();
            //  ShowAbrufMengen(oAbruf.GetAbrufById(_boWe.Abrufid));
        }

        private void Wiegestatus6() // Abruf anlegen
        {
            tb_wiegestatus.Text = "Abruf anlegen";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = true;
            radioAbrufEdit.IsChecked = false;

            radioErst.IsEnabled = true;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = true;
            radioAbrufEdit.IsEnabled = false;


            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = false;
            ribbonLsListe.IsEnabled = false;
            ribbonAbrufListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;

            //  tb_kfzid.IsEnabled = false;

            //tb_Kfz1.IsEnabled = false;
            //  tb_Kfz2.IsEnabled = false;
        }

        private void Wiegestatus7() // Abruf bearbeiten
        {
            tb_wiegestatus.Text = "Abruf bearbeiten";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioAbrufNeu.IsChecked = false;
            radioAbrufEdit.IsChecked = true;
            radioLSNew.IsChecked = false;

            radioErst.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = false;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsEnabled = false;

            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = false;
            ribbonLsListe.IsEnabled = false;
            ribbonAbrufListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;

            //tb_kfzid.IsEnabled = false;
            //tb_Kfz1.IsEnabled = false;
            //tb_Kfz2.IsEnabled = false;
        }

        private void Wiegestatus8() // LS NEU
        {
            tb_wiegestatus.Text = "Lieferschein händisch anlegen";
            radioErst.IsChecked = false;
            radioZweit.IsChecked = false;
            radioEinmal.IsChecked = false;
            radioErstEdit.IsChecked = false;
            radioLSEdit.IsChecked = false;
            radioLSNew.IsChecked = true;

            radioErst.IsEnabled = false;
            radioZweit.IsEnabled = false;
            radioEinmal.IsEnabled = false;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioLSNew.IsEnabled = true;

            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = false;
            ribbonLsListe.IsEnabled = true;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;
        }

        #endregion

        #region  Neu / Wiegen / Save / Cancel / Export

        private void NewWaege()
        {
            _boWe = null;
            _boWe = _boW.NewEntity();
            DataContext = _boWe;
            //   Wiegestatus = cb_ZweitWaegungPreset.IsChecked == true ? 2 : 1;
            Wiegestatus = 1;
        }

        private void Wiegen()
        {
            tb_ZweitGewicht.Focus();
            var boE = new Einstellungen();

            RegisterWeight oRw = netScaleView1.RegisterGewicht();
            switch (_wiegeStatus)
            {
                case 1:
                    if (oRw.Status != "80")
                    {
                        MessageBox.Show("Wägung fehlgeschlagen");
                        if (netScaleView1.oWF != null)
                            netScaleView1.oWF.SetRedLight();
                        return;
                    }


                    _boWe.LN1 = oRw.Ln;
                    _boWe.Erstgewicht = oRw.weight;
                    _boWe.ErstDatetime = oRw.Time;
                    _boWe.Waegung = 1;
                    // _boWe.wnr1 = netScaleView1.ActiveScale.ToString();

                    //   try
                    // {
                    _result = SaveEntity(_boW, _boWe);
                    //  }
                    //   catch (Exception ex)
                    //  {
                    //    MessageBox.Show(ex.Message);
                    //  }

                    if (_result != mmSaveDataResult.RulesPassed)
                    {
                        _boWe.LN1 = null;
                        _boWe.Erstgewicht = null;
                        return;
                    }


                    Wiegestatus = 0;
                    break;
                case 2:

                    // Prüfen: Erstwägung ohne Zweitwägung

                    if (_boWe.Waegung == null)
                    {
                        var oCf = new Fahrzeuge();
                        FahrzeugeEntity oCfe = oCf.GetByExactKennzeichen(_boWe.Fahrzeug);
                        if (oCfe != null)
                        {
                            if (_boWe.Erstgewicht == null)
                                _boWe.Erstgewicht = oCfe.Tara;
                            if (_boWe.Erstgewicht == 0)
                                _boWe.Erstgewicht = oCfe.Tara;
                        }
                    }

                    if (_boWe.Erstgewicht == 0 | _boWe.Erstgewicht == null)
                    {
                        MessageBox.Show("Zweitwägung mit Erstgewicht 0 ist nicht möglich!");
                        return;
                    }

                    if (oRw.Status != "80")
                    {
                        if (netScaleView1.oWF != null)
                            netScaleView1.oWF.SetRedLight();

                        MessageBox.Show("Wägung fehlgeschlagen");
                        return;
                    }


                    _boWe.LN2 = oRw.Ln;
                    _boWe.Zweitgewicht = oRw.weight;


                    _boWe.LieferscheinNr = boE.NewLsNrGlobal();

                    if(_boWe.Erstgewicht < _boWe.Zweitgewicht)
                    {
                        _boWe.Nettogewicht = _boWe.Zweitgewicht - _boWe.Erstgewicht;
                    }
                    else
                    {
                        _boWe.Nettogewicht = _boWe.Erstgewicht - _boWe.Zweitgewicht;
                    }

                    // TODO: Welches Feld
                    _boWe.zweitDateTime = oRw.Time;
                     _boWe.LSDatum = oRw.Date;
                    _boWe.Waegung = 2;
                    //  _boWe.wnr2 = netScaleView1.ActiveScale.ToString();

                    try
                    {
                        _result = SaveEntity(_boW, _boWe);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    if (_result != mmSaveDataResult.RulesPassed)
                    {
                        return;
                    }


                    //  PrintLs();
                    Export2Json(_boWe);
                    if (netScaleView1.oWF != null)
                        netScaleView1.oWF.SetGreenLight();
                    Wiegestatus = 0;
                    // ShowAbrufMengen(oAbruf.GetAbrufById(_boWe.Abrufid));
                    //   tb_Kfz1.Focus(); // Test wegen dem F2 Problem bei der ersten Wägung
                    break;

                case 3:
                    _boWe.LN2 = oRw.Ln;
                    _boWe.Erstgewicht = 0;
                    _boWe.Zweitgewicht = oRw.weight;

                    _boWe.Nettogewicht = _boWe.Zweitgewicht;

                    boE = new Einstellungen();
                    _boWe.LieferscheinNr = boE.NewLsNrGlobal();
                    // _boWe.lsnr = boMandant.GetLsNr(_boWe);
                  
                    _boWe.zweitDateTime = oRw.Time;
                    //   _boWe.LSDatum = oRw.Date;
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

                    // PrintLs();
                    Wiegestatus = 0;
                    break;
            }
        }

        private void Save()
        {
            tb_ZweitGewicht.Focus();
            if (_wiegeStatus != 6 && _wiegeStatus != 7)
            {
                if (_wiegeStatus == 8 | _wiegeStatus == 5) // LS Neu
                {
                    _boWe.Waegung = 2;
                }

                if (_wiegeStatus == 8)
                {
                    var boE = new Einstellungen();
                    _boWe.LieferscheinNr = boE.NewLsNrGlobal();


                    _boWe.zweitDateTime = DateTime.Now;
                    //  _boWe.LSDatum = DateTime.Today;
                    _boWe.Waegung = 2;
                    if (_boWe.Erstgewicht < _boWe.Zweitgewicht)
                    {
                        _boWe.Nettogewicht = _boWe.Zweitgewicht - _boWe.Erstgewicht;
                    }
                    else
                    {
                        _boWe.Nettogewicht = _boWe.Erstgewicht - _boWe.Zweitgewicht;
                    }
                }

                try
                {
                    _result = SaveEntity(_boW, _boWe);
                    if (_wiegeStatus != 4) // Erstwägung bearbeiten
                        if (
                            MessageBox.Show("Lieferschein drucken?", "Frage", MessageBoxButton.YesNo,
                                            MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                        }
                        else
                        {
                            //  PrintLs();
                            Export2Json(_boWe);
                        }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (_result != mmSaveDataResult.RulesPassed)
                    return;
            }
            if (_wiegeStatus == 6)
            {
                //decimal soll;
                //decimal ist;
                //decimal rest;
                //Decimal.TryParse(tb_abruf_soll.Text, NumberStyles.Any, CultureInfo.CurrentCulture,
                //                 out soll);
                //Decimal.TryParse(tb_abruf_ist.Text, NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat, out ist);
                //Decimal.TryParse(tb_abruf_rest.Text, NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat,
                //                 out rest);
                //AbrufEntity oAE = boA.CreateAbruf(_boWe, ist, soll, rest);
                //_boWe.Abrufid = oAE.PK;
                //_boWe.Abrufnr = oAE.Abrufnr;
            }

            if (_wiegeStatus == 7)
            {
                // Ein wenig tricky : Die Abrufentity wird zurück gegeben, damit die  Restmenge bei Änderung gleich nach
                // dem Speichern angezeigt wird.

                //var oAE = boA.SaveAbruf(_boWe);
                //_boWe.Abrufid = oAE.PK;
                //_boWe.Abrufnr = oAE.Abrufnr;
                //ShowAbrufMengen(oAE);
            }

            //var boA = new Abruf();

            Wiegestatus = 0;
        }
        private void Cancel()
        {
            if (_boWe != null)
            {
                _boW.CancelEntity(_boWe);
                Wiegestatus = 0;
            }
        }
        private void Export2Json(WaegeEntity we)
        {
            new ImportExportPolos().Export(we);
        }

        #endregion

        #region LookUps


        private void txtKfzKennzeichen_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F4)
            {
                lookupKfz();
            }
        }

        private void cmdLookUpKfz_Click(object sender, RoutedEventArgs e)
        {
            lookupKfz();
        }

        private void lookupKfz()
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

        private void luFrachtmittel_Click(object sender, RoutedEventArgs e)
        {
            FrachtmittelListFrm oFFrm = new FrachtmittelListFrm();
            oFFrm.ShowDialog();
            int uRet = oFFrm.uRet;
            if(_boWe!=null)
            {
              _boW.FillFrachtmittel(uRet);

            }
            oFFrm.Close();
        }

        #region lookup Adressen

        private void luFrachtfuehrer_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtFrachtführer.Text, "FF");
            if (uRet != null)
            {
                _boW.FrachtFuehrer2Waege((int)uRet);
            }
            else
            {
                _boW.ClearFrachtFuehrerInWaege();
            }
        }

        private void luCustomer_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtAuftraggeber.Text,"AG");
            
          
            

            if (uRet != null)
            {
                _boW.Customer2Waege((int) uRet);
            }
            else
            {
                _boW.ClearCustomerInWaege();
            }
        }

        private void luInvoiceReceiver_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtRechnungsEmpfaenger.Text,"RE");
            if (uRet != null)
            {
                _boW.InvoiceReceiver2Waege((int)uRet);
            }
            else
            {
                _boW.ClearinvoiceReceiverInWaege();
            }
        }

        private void luLagermandant_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtLagerMandant.Text,"LM");
            if (uRet != null)
            {
                _boW.Owner2Waege((int)uRet);
            }
            else
            {
                _boW.ClearOwnerInWaege();
            }
        }


        private void luConsigneeSupplier_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse("","");
            if (uRet != null)
            {
                _boW.SupplierOrConsignee2Waege((int)uRet);
            }
            else
            {
                _boW.ClearsupplierOrConsigneeInWaege();
            }
        }

        private int? lookUpAdresse(string mc, string rolle)
        {
            AdressenListeFrm oAFrm = new AdressenListeFrm(mc,rolle);
            oAFrm.ShowDialog();
            int uRet = oAFrm.uRet;
            oAFrm.Close();
            return uRet;
        }

        #endregion

        private void cmdLookUpProduct_Click(object sender, RoutedEventArgs e)
        {
            ProdukteListFrm oPFrm = new ProdukteListFrm("");
            oPFrm.ShowDialog();
            int uRet = oPFrm.uRet;
            _boW.Product2Waege(uRet);
            oPFrm.Close();
        }

        private void luWarenArt_Click(object sender, RoutedEventArgs e)
        {
            WarenartListFrm oWFrm = new WarenartListFrm("");
            oWFrm.ShowDialog();
            int uRet = oWFrm.uRet;
            _boW.WarenArt2Waege(uRet);
            oWFrm.Close();

        }

        private void luArticle_Click(object sender, RoutedEventArgs e)
        {
            ArtikelListFrm oAFrm = new ArtikelListFrm("");
            oAFrm.ShowDialog();
            int uRet = oAFrm.uRet;
            _boW.Article2Waege(uRet);
            oAFrm.Close();
        }
        #endregion

        private void FillIncoterms()
        {
            // Füllt die Incotermcombobox -Später aus Tabelle


            Incoterm boI = new Incoterm();
    
            cbIncoterms.DisplayMemberPath= ("Kennung");
            cbIncoterms.ItemsSource = boI.GetAllIncoterm();
        }

        private void cmdExportToYeoman_Click(object sender, RoutedEventArgs e)
        {
            ExportYeoman2XlsFrm oyXlsFrmFrm = new ExportYeoman2XlsFrm();
            oyXlsFrmFrm.ShowDialog();
            oyXlsFrmFrm.Close();
        }


        private void TxtAuftraggeber_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAuftraggeber.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtAuftraggeber.Text,"AU");
                if (boAe != null)
                {
                    _boW.Customer2Waege(boAe.PK);
                }
                else
                {
                    _boW.ClearCustomerInWaege();
                }
            }
        }

        private void TxtRechnungsEmpfaenger_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRechnungsEmpfaenger.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtRechnungsEmpfaenger.Text,"RE");
                if (boAe != null)
                {
                    _boW.InvoiceReceiver2Waege(boAe.PK);
                }
                else
                {
                    _boW.ClearinvoiceReceiverInWaege();
                }
            }
          
        }

        private void TxtFrachtführer_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFrachtführer.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtFrachtführer.Text,"LI");
                if (boAe != null)
                {
                    _boW.FrachtFuehrer2Waege(boAe.PK);
                }
                else
                {
                    _boW.ClearFrachtFuehrerInWaege();
                }
            }
          
        }

        private void TxtLagerMandant_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLagerMandant.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtLagerMandant.Text,"LM");
                if (boAe != null)
                {
                    _boW.Owner2Waege(boAe.PK);
                }
                else
                {
                    _boW.ClearOwnerInWaege();
                }

            }
        }

        private void TxtLieferantEmpfaenger_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLieferantEmpfaenger.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtLieferantEmpfaenger.Text,"EM");
                if (boAe != null)
                {
                    _boW.SupplierOrConsignee2Waege(boAe.PK);
                }
                else
                {
                    _boW.ClearsupplierOrConsigneeInWaege();
                }
            }
        }

        private void TxtKfzKennzeichen_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Fahrzeuge boF = new Fahrzeuge();
            FahrzeugeEntity boFe = boF.GetByExactKennzeichen(txtKfzKennzeichen.Text);
            if (boFe != null)
            {
                _boW.FillKfz(boFe.PK, _boWe);
                int uRet = _boW.IsKfzErstVerwogen(_boWe.Fahrzeug);
                if(uRet!=0)
                {
                _boWe =   _boW.GetWaegungByPk(uRet);
                  
                if (_boWe != null)
                {
                    DataContext = _boWe;
                  
                    Wiegestatus = 2;
                }
                }
            }
        }
    }
}