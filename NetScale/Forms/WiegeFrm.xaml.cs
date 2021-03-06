using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using HardwareDevices;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using NetScalePolosIO;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.WPF;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WiegeFrm Class
    /// </summary>
    public sealed partial class WiegeFrm
    {
        #region Private Fields
        
        private readonly Waege _boW;
        private WaegeEntity _boWe;
        private int _wiegeStatus;
        private mmSaveDataResult _result;
        private ImportExportPolos _oIO;
        private Adressen _oAp;

       
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public WiegeFrm()
        {   _oIO = new ImportExportPolos();
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            _boW = new Waege();
            DataContext = _boWe;

            // Mandant
            goApp.Mandant_PK = new Mandant().GetDefaultMandant().PK;


            // Achtung - Hier wird der Observer abonniert
            netScaleView1.OnWeightChanged += ShowEventGewichtHasChanged;
            DisplayErrorDialog = true;
            DisplayErrorProvider = true;
       


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
      
            // Test Auto Compleed
            _oAp= new Adressen();
          
            //   new ImportExportPolos().ImportStammdaten();
            //   new ImportExportPolos().ImportAuftraege(false);
        }

     

        // Das Ereignis, welches ausgelöst wird, wenn die Gewichtsänderung in NetScale in der Wägemaske gemeldet wird
        private void ShowEventGewichtHasChanged()
        {
          
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
            txtKfzKennzeichen.Focus();
            var dataContext = DataContext;
            DataContext = null;
            DataContext = dataContext;
            if (!netScaleView1.Stillstand)
            {
                MessageBox.Show("Waage hat keinen Stillstand!!!", "ACHTUNG");
                return;
            }
            Wiegen();
        }

        private void ribbonHofliste_Click(object sender, RoutedEventArgs e)
        {
            var oHlFrm = new HoflisteFrm();
            oHlFrm.ShowDialog();
            int uRet = oHlFrm.uRet;
         
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
            var oLsListe = new WiegelisteFrm();
            oLsListe.ShowDialog();
            int uRet = oLsListe.uRet;
            if (uRet == 0)
            {
                oLsListe.Close();
            }
            else
            {
                _boWe = _boW.GetWaegungByPk(uRet);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    oLsListe.Close();
                    Wiegestatus = 5;
              
                }
            }
        }

     



        private void ribbonDelete_Click(object sender, RoutedEventArgs e)
        {
       

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


        private void cmdArtikel_Click(object sender, RoutedEventArgs e)
        {
            ArtikelListFrm oAListFrm = new ArtikelListFrm("","");
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

            oA.Close();
        }

        private void cmdKFZ_Click(object sender, RoutedEventArgs e)
        {
            CFListFrm oCfFrm = new CFListFrm(true, "");
            oCfFrm.ShowDialog();
            oCfFrm.Close();
        }

        private void cmdFrachtmittel_Click(object sender, RoutedEventArgs e)
        {
            FrachtmittelListFrm oFFrm = new FrachtmittelListFrm();
            oFFrm.ShowDialog();
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
            _boW.Auftrag2Waege(uRet, _boWe);

            oAFrm.Close();
        }

        #region Import / ExportAll

        private void CmdExport_OnClick(object sender, RoutedEventArgs e)
        {
            ExportWaegungen();
        }

        private void ExportWaegungen()
        {
            
            if (_oIO != null)
            {
                _oIO.ExportAll();
                progressBarExportWaegung.DataContext = _oIO; ///????
                lblExportMessageWaegungen.DataContext = _oIO;
            }
        }

        private void CmdExportLog_OnClick(object sender, RoutedEventArgs e)
        {
            ExportLogFrm oEFrm = new ExportLogFrm();
            oEFrm.ShowDialog();
            oEFrm.Close();
        }

        private void cmdImportAuftraege_OnClick(object sender, RoutedEventArgs e)
        {
            ImportAuftrage();
        }

        private void ImportAuftrage()
        {
        
            if (_oIO != null)
            {
                _oIO.ImportAuftraege(false);
                lblImportMessageAuftraege.DataContext = _oIO;
                progressBarAuftraege.DataContext = _oIO;
            }
        }

        private void CmdImporStammdaten_OnClick(object sender, RoutedEventArgs e)
        {
            ImportStammdaten();
        }

        private void ImportStammdaten()
        {
            var oIO = new ImportExportPolos();
            if (oIO != null)
            {
                oIO.ImportStammdaten();
                lblImportMessageStammdaten.DataContext = oIO;
                progressBarStammdaten.DataContext = oIO;
            }
        }

        private void cmdExportToYeoman_Click(object sender, RoutedEventArgs e)
        {
            ExportYeoman2XlsFrm oyXlsFrmFrm = new ExportYeoman2XlsFrm();
            oyXlsFrmFrm.ShowDialog();
            oyXlsFrmFrm.Close();
        }

        #endregion

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

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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
                var textBox = ctrl as TextBox;
                if (textBox != null)
                    textBox.IsEnabled = false;

                Button ctrl1 = ctrl as Button;
                if (ctrl1 != null)
                    ctrl1.IsEnabled = false;


                Expander expander = ctrl as Expander;
                if (expander != null)
                    expander.IsEnabled = false;

                SplitButton button = ctrl as SplitButton;
                if (button != null)
                    button.IsEnabled = false;
            }


         
        }

        private void EnableFrmTb()
        {
            foreach (object ctrl in LayoutRoot.Children)
            {
                TextBox box = ctrl as TextBox;
                if (box != null)
                    box.IsEnabled = true;

                Button ctrl1 = ctrl as Button;
                if (ctrl1 != null)
                    ctrl1.IsEnabled = true;

                Expander expander = ctrl as Expander;
                if (expander != null)
                    expander.IsEnabled = true;

                SplitButton button = ctrl as SplitButton;
                if (button != null)
                    button.IsEnabled = true;
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
            get { return _wiegeStatus; }
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
            //if (_boWe != null)
            //    _boW.CancelEntity(_boWe);
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
         //   ribbonAbrufListe.IsEnabled = false;
            DisableFrmTb();
            ribbonNeu.IsEnabled = true;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = true;
      //      ribbonAbrufListe.IsEnabled = true;
            ribbonAuftrag.IsEnabled = false;
            ribbonCancel.IsEnabled = false;
            ribbonDelete.IsEnabled = false;

            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = false;
            //   txtAbrufDate.Visibility = System.Windows.Visibility.Collapsed;
            // cb_Abruffest.Visibility = System.Windows.Visibility.Collapsed;
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

       //     ribbonAbrufListe.IsEnabled = true;
            radioErst.IsEnabled = true;
            radioZweit.IsEnabled = true;
            radioEinmal.IsEnabled = true;
            radioErstEdit.IsEnabled = false;
            radioLSEdit.IsEnabled = false;
            radioAbrufNeu.IsEnabled = true;
            radioAbrufEdit.IsEnabled = false;
            radioLSNew.IsEnabled = true;
            ribbonAuftrag.IsEnabled = true;
            EnableFrmTb();
            ribbonNeu.IsEnabled = false;
            ribbonHofliste.IsEnabled = true;
            ribbonLsListe.IsEnabled = false;
      //      ribbonAbrufListe.IsEnabled = true;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = false;

            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = true;
            txtAbrufDate.Visibility = Visibility.Collapsed;
            CbAbruffest.Visibility = Visibility.Collapsed;
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
            ribbonAuftrag.IsEnabled = true;
            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = true;
            txtAbrufDate.Visibility = Visibility.Collapsed;
            CbAbruffest.Visibility = Visibility.Collapsed;
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
            ribbonAuftrag.IsEnabled = true;
            ribbonSave.IsEnabled = false;
            ribbonWiegen.IsEnabled = true;
            txtAbrufDate.Visibility = Visibility.Collapsed;
            CbAbruffest.Visibility = Visibility.Collapsed;
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
            ribbonAuftrag.IsEnabled = true;
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
            ribbonAuftrag.IsEnabled = true;
            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;

            CbAbruffest.Visibility = Visibility.Collapsed;
            txtAbrufDate.Visibility = Visibility.Collapsed;
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
      //      ribbonAbrufListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;
     //       ribbonAbrufListe.IsEnabled = false;
            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;

            txtKfzKennzeichen.IsEnabled = false;
            CbAbruffest.Visibility = Visibility.Visible;
            txtAbrufDate.Visibility = Visibility.Visible;
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
      //      ribbonAbrufListe.IsEnabled = false;
            ribbonCancel.IsEnabled = true;
            ribbonDelete.IsEnabled = true;

            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;

            txtKfzKennzeichen.IsEnabled = false;
            CbAbruffest.Visibility = Visibility.Visible;
            txtAbrufDate.Visibility = Visibility.Visible;
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
            ribbonAuftrag.IsEnabled = true;
            ribbonSave.IsEnabled = true;
            ribbonWiegen.IsEnabled = false;
        }

        #endregion

        #region  Neu / Wiegen / Save / Cancel / ExportAll

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

                    PrintLz();
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
                    if (_wiegeStatus != 6 & _wiegeStatus != 7)
                    {
                        if (_boWe.Erstgewicht < _boWe.Zweitgewicht)
                        {
                            _boWe.Nettogewicht = _boWe.Zweitgewicht - _boWe.Erstgewicht;
                        }
                        else
                        {
                            _boWe.Nettogewicht = _boWe.Erstgewicht - _boWe.Zweitgewicht;
                        }
                    }
                    // TODO: Welches Feld
                    _boWe.zweitDateTime = oRw.Time;
                    _boWe.LSDatum = oRw.Date;
                    _boWe.Waegung = 2;
                    //  _boWe.wnr2 = netScaleView1.ActiveScale.ToString();

                    try
                    {
                        _result = SaveEntity(_boW, _boWe);
                        if (_result == mmSaveDataResult.RulesPassed)
                        {
                            _boW.SetOrderItemsServiceInvisible(_boWe);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    if (_result != mmSaveDataResult.RulesPassed)
                    {
                        return;
                    }


                    PrintLs();

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


                    PrintLs();
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

                    _boWe.ErstDatetime = DateTime.Today;
                    _boWe.zweitDateTime = DateTime.Now;
                    _boWe.LSDatum = DateTime.Today;

                    _boWe.Waegung = 2;
                    if (_boWe.Erstgewicht != null && _boWe.Zweitgewicht != null)
                    {
                        if (_boWe.Erstgewicht < _boWe.Zweitgewicht)
                        {
                            _boWe.Nettogewicht = _boWe.Zweitgewicht - _boWe.Erstgewicht;
                        }
                        else
                        {
                            _boWe.Nettogewicht = _boWe.Erstgewicht - _boWe.Zweitgewicht;
                        }
                    }
                }

                try
                {
                    _result = SaveEntity(_boW, _boWe);
                    if (_result == mmSaveDataResult.RulesPassed)
                    {
                       
                            _boW.SetOrderItemsServiceInvisible(_boWe);
                       
                        if (_wiegeStatus != 4) // Erstwägung bearbeiten
                            if (
                                MessageBox.Show("Lieferschein drucken?", "Frage", MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                Export2Json(_boWe);
                                //do no stuff
                            }
                            else
                            {
                                PrintLs();
                                Export2Json(_boWe);
                            }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (_wiegeStatus == 6)
            {
                Abruf boA = new Abruf();
                AbrufEntity oAe = boA.CreateAbruf(_boWe);
                _boWe.abruf_PK = oAe.PK;
                _boWe.AbrufNr = oAe.AbrufNr;
            }

            if (_wiegeStatus == 7)
            {
                // Ein wenig tricky : Die Abrufentity wird zurück gegeben, damit die  Restmenge bei Änderung gleich nach
                // dem Speichern angezeigt wird.
                Abruf boA = new Abruf();
                boA.SaveAbruf(_boWe);

                //_boWe.Abrufid = oAE.PK;
                //_boWe.Abrufnr = oAE.Abrufnr;
                //ShowAbrufMengen(oAE);
            }

            //var boA = new Abruf();

            Wiegestatus = 0;
        }

        private void PrintLs()
        {
            var oLe = new Lokaleeinstellungen();
            oLe = oLe.Load();
            var oPls = new PrinterLs();
            oPls.DoPrintLs(oLe, _boWe, false);
        }

        private void PrintLz()
        {
            var oPls = new PrinterLs();
            oPls.PrintLz(_boWe);
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
            new ImportExportPolos().ExportSingle(we);
        }

        #endregion

        #region LookUps

        //private void txtKfzKennzeichen_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.F4)
        //    {
        //        lookupKfz();
        //    }
        //}

        private void cmdLookUpKfz_Click(object sender, RoutedEventArgs e)
        {
            LookupKfz();
        }

        private void LookupKfz()
        {
            CFListFrm oKfzListeFrm = new CFListFrm(true, txtKfzKennzeichen.Text);
            oKfzListeFrm.ShowDialog();
            var uRet = oKfzListeFrm.uRet;
            oKfzListeFrm.Close();
            if (_boWe != null)
            {
                _boW.FillKfz(uRet, _boWe);
                bool lRet = IsKfzErstVerwogenDialog();
                if (lRet) // Dann mach nichts
                {
                }
                else // Schau mal ob das Kfz einen Abruf hat, den man bei der Erstwägung in die Maske schießen kann
                {
                    var oCfe = new Fahrzeuge().GetByPk(uRet);

                    Abruf2Wage(oCfe);
                }
            }
        }

        private bool IsKfzErstVerwogenDialog()
        {
            int uRet = _boW.IsKfzErstVerwogen(txtKfzKennzeichen.Text);
            if (uRet != 0 && Wiegestatus == 1) // Ungleich 0 bedeutet ist Erstgewogen
            {
                _boWe = _boW.GetWaegungByPk(uRet);
                if (_boWe != null)
                {
                    DataContext = _boWe;
                    Wiegestatus = 2;
                    MessageBox.Show(
                        "Dieses Fahrzeug ist schon erstgewogen. Das Programm schaltet aus diesem Grund nun automatisch auf Zweitwägung um!",
                        "Achtung - Wichtiger Hinweis", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }


            if (uRet != 0 && Wiegestatus == 2)
            {
                _boWe = _boW.GetWaegungByPk(uRet);
                DataContext = _boWe;
            }
            if (uRet != 0) // Erstgewogen
            {
                return true;
            }
            return false;
        }

        private void Abruf2Wage(FahrzeugeEntity oCfe)
        {
            if (oCfe == null)
                return;

            int? uRet = oCfe.abruf_PK;
            var boAbruf = new Abruf();
            if (uRet != null)
            {
                WaegeEntity dc = boAbruf.CopyAbrufToWaege(uRet, _boWe);
                if (dc != null)
                {
                    DataContext = dc;
                    //  _wiegestatus = 1;
                }
            }
        }

        private void luFrachtmittel_Click(object sender, RoutedEventArgs e)
        {
            FrachtmittelListFrm oFFrm = new FrachtmittelListFrm();
            oFFrm.ShowDialog();
            int uRet = oFFrm.uRet;
            if (_boWe != null)
            {
                _boW.FillFrachtmittel(uRet, _boWe);
            }
            oFFrm.Close();
        }

        #region lookup Adressen

        private void luFrachtfuehrer_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtFrachtführer.Text, "FF");
            if (uRet != null)
            {
                _boW.FrachtFuehrer2Waege((int) uRet, _boWe);
            }
            else
            {
                _boW.ClearFrachtFuehrerInWaege(_boWe);
            }
        }

        private void luCustomer_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtAuftraggeber.Text, "AG");


            if (uRet != null)
            {
                _boW.Customer2Waege((int) uRet, _boWe);
            }
            else
            {
                _boW.ClearCustomerInWaege(_boWe);
            }
        }

        private void luInvoiceReceiver_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtRechnungsEmpfaenger.Text, "RE");
            if (uRet != null)
            {
                _boW.InvoiceReceiver2Waege((int) uRet, _boWe);
            }
            else
            {
                _boW.ClearinvoiceReceiverInWaege(_boWe);
            }
        }

        private void luLagermandant_Click(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtLagerMandant.Text, "LM");
            if (uRet != null)
            {
                _boW.Owner2Waege((int) uRet, _boWe);
            }
            else
            {
                _boW.ClearOwnerInWaege(_boWe);
            }
        }


    

        private void LuReceiver_OnClick(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtEmpfaenger.Text, "EM");
            if (uRet != null)
            {
                _boW.Receiver2Waege((int)uRet, _boWe);
            }
            else
            {
                _boW.ClearReceiverInWaege();
            }
        }
        private void LuSupplier_OnClick(object sender, RoutedEventArgs e)
        {
            int? uRet = lookUpAdresse(txtSupplier.Text, "LI");
            if (uRet != null)
            {
                _boW.Supplier2Waege((int)uRet, _boWe);
            }
            else
            {
                _boW.ClearSupplierInWaege();
            }
        }

        private int? lookUpAdresse(string mc, string rolle)
        {
            AdressenListeFrm oAFrm = new AdressenListeFrm(mc, rolle);
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
            _boW.Product2Waege(uRet, _boWe);
            oPFrm.Close();
        }

        private void luWarenArt_Click(object sender, RoutedEventArgs e)
        {
            WarenartListFrm oWFrm = new WarenartListFrm(txtWarenArt.Text ?? "");
            oWFrm.ShowDialog();
            int uRet = oWFrm.uRet;
            _boW.WarenArt2Waege(uRet, _boWe);
            oWFrm.Close();
        }

        private void luArticle_Click(object sender, RoutedEventArgs e)
        {
            string oid = null;
            if (_boWe != null)
            {
                oid = _boWe.ownerId;
            }

            ArtikelListFrm oAFrm = new ArtikelListFrm(txtArtikelNr.Text ?? "",oid);
            oAFrm.ShowDialog();
            int uRet = oAFrm.uRet;
            _boW.Article2Waege(uRet, _boWe);
            oAFrm.Close();
        }

        #endregion

        private void FillIncoterms()
        {
            // Füllt die Incotermcombobox -Später aus Tabelle


            Incoterm boI = new Incoterm();

            cbIncoterms.DisplayMemberPath = ("Kennung");
            cbIncoterms.ItemsSource = boI.GetAllIncoterm();
        }


        private void SearchCustomer()
        {
            if (!string.IsNullOrEmpty(txtAuftraggeber.Text))
            {
                Adressen boA = new Adressen();


                var boAe = boA.GetByBusinenessIdentifier(txtAuftraggeber.Text, "AU");
                if (boAe != null)
                {
                    _boW.Customer2Waege(boAe.PK, _boWe);
                }
                else
                {
                    _boW.ClearCustomerInWaege(_boWe);
                }
            }
        }


        private void SearchInvocieReceiver()
        {
            if (!string.IsNullOrEmpty(txtRechnungsEmpfaenger.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtRechnungsEmpfaenger.Text, "RE");
                if (boAe != null)
                {
                    _boW.InvoiceReceiver2Waege(boAe.PK, _boWe);
                }
                else
                {
                    _boW.ClearinvoiceReceiverInWaege(_boWe);
                }
            }
        }


        private void SearchFrachtführer()
        {
            if (!string.IsNullOrEmpty(txtFrachtführer.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtFrachtführer.Text, "FF");
                if (boAe != null)
                {
                    _boW.FrachtFuehrer2Waege(boAe.PK, _boWe);
                }
                else
                {
                    _boW.ClearFrachtFuehrerInWaege(_boWe);
                }
            }
            else
            {
                _boW.ClearFrachtFuehrerInWaege(_boWe);
            }
        }


        private void SearchLagerMandant()
        {
            if (!string.IsNullOrEmpty(txtLagerMandant.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtLagerMandant.Text, "LM");
                if (boAe != null)
                {
                    _boW.Owner2Waege(boAe.PK, _boWe);
                }
                else
                {
                    _boW.ClearOwnerInWaege(_boWe);
                }
            }
            else
            {
                _boW.ClearOwnerInWaege(_boWe);
            }
        }

        private void SearchEmpfänger()
        {
            if (!string.IsNullOrEmpty(txtEmpfaenger.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtEmpfaenger.Text, "EM");
                if (boAe != null)
                {
                    _boW.Receiver2Waege(boAe.PK, _boWe);
                }
                else
                {
                    _boW.ClearReceiverInWaege();
                }
            }
        }

        private void SearchSupplier()
        {
            if (!string.IsNullOrEmpty(txtSupplier.Text))
            {
                Adressen boA = new Adressen();
                AdressenEntity boAe = boA.GetByBusinenessIdentifier(txtSupplier.Text, "LI");
                if (boAe != null)
                {
                    _boW.Supplier2Waege(boAe.PK, _boWe);
                }
                else
                {
                    _boW.ClearSupplierInWaege();
                }
            }
        }


        private void SearchKfz()
        {
            Fahrzeuge boF = new Fahrzeuge();
            FahrzeugeEntity boFe = boF.GetByExactKennzeichen(txtKfzKennzeichen.Text);
            if (boFe != null)
            {
                _boW.FillKfz(boFe.PK, _boWe);
                int uRet = _boW.IsKfzErstVerwogen(_boWe.Fahrzeug);
                if (uRet != 0)
                {
                    _boWe = _boW.GetWaegungByPk(uRet);

                    if (_boWe != null)
                    {
                        DataContext = _boWe;

                        Wiegestatus = 2;
                    }
                }
            }

            if (_boWe != null)
            {
                bool lRet = IsKfzErstVerwogenDialog();
                if (lRet) // Dann mach nichts
                {
                }
                else // Schau mal ob das Kfz einen Abruf hat, den man bei der Erstwägung in die Maske schießen kann
                {
                    Abruf2Wage(boFe);
                }
            }
        }

        //private void SearchProduct()
        //{
  

        //    if (!string.IsNullOrEmpty(txtProductId.Text))
        //    {
        //        _boW.Product2Waege(txtProductId.Text, _boWe);
        //    }
        //    else
        //    {
        //    }
        //}

//***************************************************************************************************
        private void TxtAuftraggeber_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                SearchCustomer();
            }
            if (e.Key == Key.F4)
            {
            }
        }
        private void TxtAuftraggeber_OnLostFocus(object sender, RoutedEventArgs e)
        {
           SearchCustomer();
        }

        private void TxtRechnungsEmpfaenger_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                SearchInvocieReceiver();
            }
            if (e.Key == Key.F4)
            {
            }
        }
        private void TxtRechnungsEmpfaenger_OnLostFocus(object sender, RoutedEventArgs e)
        {
            SearchInvocieReceiver();
        }

        private void TxtFrachtführer_OnKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Tab)
            {
                SearchFrachtführer();
            }
            if (e.Key == Key.F4)
            {
                luFrachtfuehrer.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void TxtFrachtführer_OnLostFocus(object sender, RoutedEventArgs e)
        {
           SearchFrachtführer();
        }

        private void TxtLagerMandant_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                SearchLagerMandant();
            }
            if (e.Key == Key.F4)
            {
            }
        }
        private void TxtLagerMandant_OnLostFocus(object sender, RoutedEventArgs e)
        {
            SearchLagerMandant();
        }

        private void TxtEmpfaenger_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                SearchEmpfänger();
            }
            if (e.Key == Key.F4)
            {
            }
        }
        private void TxtSupplier_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                SearchSupplier();
            }
            if (e.Key == Key.F4)
            {
            }
        }


        private void TxtEmpfaenger_OnLostFocus(object sender, RoutedEventArgs e)
        {
            SearchEmpfänger();
        }
        private void TxtSupplier_OnLostFocus(object sender, RoutedEventArgs e)
        {
           SearchSupplier();
        }

        private void TxtWarenArt_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
            }
            if (e.Key == Key.F4)
            {
            }
        }

        private void TxtArtikelNr_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
            }
            if (e.Key == Key.F4)
            {
            }
        }

        //private void TxtProductId_OnKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Tab)
        //    {
        //        SearchProduct();
        //    }
        //    if (e.Key == Key.F4)
        //    {
        //    }
        //}

        private void TxtKfzKennzeichen_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                SearchKfz();
            }
            if (e.Key == Key.F4)
            {
                cmdLookUpKfz.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void LuQuellLagerplatz_OnClick(object sender, RoutedEventArgs e)
        {
            LagerplaetzeListeFrm oFrm = new LagerplaetzeListeFrm("");
            oFrm.ShowDialog();
            int uRet = oFrm.uRet;
            //if (uRet != null)
            //{
            Lagerplaetze boL = new Lagerplaetze();
            LagerplaetzeEntity boLe = boL.GetByPk(uRet);
            if (boLe != null)
            {
                _boWe.actualStorageAreaId = boLe.id;
                _boWe.IstQuellLagerPlatzId = boLe.id;
                _boWe.IstQuellLagerPlatz = boLe.name;
            }
            else
            {
                _boWe.actualStorageAreaId = null;
                _boWe.IstQuellLagerPlatzId = null;
                _boWe.IstQuellLagerPlatz = null;
            }
            //}
            oFrm.Close();
        }

       

        private void RibbonWiegen_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void RibbonSave_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Tb_ErstGewicht_OnGotFocus(object sender, RoutedEventArgs e)
        {
          //  tempErstgewicht = _boWe.Erstgewicht;
            if (_boWe.LN1 != null)
            {
                tb_ErstGewicht.IsEnabled = false;
                
            }
        }

        private void Tb_ErstGewicht_OnLostFocus(object sender, RoutedEventArgs e)
        {
       
        }

        private void Tb_ZweitGewicht_OnGotFocus(object sender, RoutedEventArgs e)
        {
           // tempZweitgewicht = _boWe.Zweitgewicht;
            if (_boWe.LN2 != null)
            {
                tb_ZweitGewicht.IsEnabled = false;
                // _boWe.Zweitgewicht = tempZweitgewicht;
            }
        }

        private void Tb_ZweitGewicht_OnLostFocus(object sender, RoutedEventArgs e)
        {

            if (_boWe.LN2 != null)
            {
               // _boWe.Zweitgewicht = tempZweitgewicht;
            }
        }

        private void buttonArtickeAttr_Click(object sender, RoutedEventArgs e)
        {
            AttributeForArticleFrm oA = new AttributeForArticleFrm( _boWe.articleId,_boWe);
            oA.ShowDialog();
            oA.Close();


        }

        private new  void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
            ImportStammdaten();
            ImportAuftrage();
            ExportWaegungen();
        }

        private void ffautoFillTxt_SearchStringChanged(object sender, OakLeaf.MM.Main.WPF.SearchStringChangedEventArgs e)
        {
            mmAutoCompleteTextBox ffautoFillTx = (mmAutoCompleteTextBox)sender;

            // Call a method on the Customer controller object and store it in the AutoComplete TextBox's BindingList
           ffautoFillTx.BindingList = this._oAp.GetByMatchCodeAndRole(e.SearchString,"FF");
        

           

        }

        private void ffautoFillTxt_SearchStringSelected(object sender, SearchStringSelectedEventArgs e)
        {

        }
    }
}