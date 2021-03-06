using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HWB.Logging;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.FRONTEND.WPF.Forms;

using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO;
using NetScalePolosIO;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Security;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : mmMainAppWindow
    {
        /// <summary>
        /// Constructor
        private MyUserListeFrm oU;

        public MainWindow()
        {
            try
            {
        
                InitializeComponent();

                // DefaultMandant
                Mandant boMa = new Mandant();
                MandantEntity boMaE = boMa.GetDefaultMandant();
                goApp.Mandant = boMaE.MandantNr;

                // Ein wenig tricky: Instanz erzeugen;Methode aufrufen die wiederum eine Instanz des gleichen Typs zurückgibt 
                // Egal, funktioniert aber.
                Waageneinstellungen oWE = new Waageneinstellungen();
                oWE = oWE.Load();
                goApp.MengenEinheit = oWE.Einheit;

                Einstellungen boE = new Einstellungen();

                goApp.MaxGewicht = boE.GetMaxGewicht();
                goApp.MaxGewichtValidieren = boE.GetMaxGewichtValidieren();
                goApp.planningDivisionId = boE.GetPlanningDivisionId();
             

                Lokaleeinstellungen oLE = new Lokaleeinstellungen();
                oLE = oLE.Load();

                if (oLE.AUTOKFZ == "J")
                    goApp.AutoKfz = true;
                else
                    goApp.AutoKfz = false;

                if (oLE.AUTOABRUF == "J")
                    goApp.AutoAbruf = true;
                else
                    goApp.AutoAbruf = false;

                if (oLE.SAVE_ABR2CF == "J")
                    goApp.SAVE_ABR2CF = true;
                else
                {
                    goApp.SAVE_ABR2CF = false;
                }

                if (oLE.TAKE_LAST_ABR == "J")
                    goApp.TAKE_LAST_ABR = true;
                else
                {
                    goApp.TAKE_LAST_ABR = false;
                }

                if (oLE.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN == "J")
                    goApp.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN = true;
                else
                {
                    goApp.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN = false;
                }

                if (oLE.SAVE_ERST2CFTARA == "J")
                    goApp.SAVE_ERST2CFTARA = true;
                else
                {
                    goApp.SAVE_ERST2CFTARA = false;
                }

                if (oLE.ZWEIT_OHNE_ERST == "J")
                    goApp.ZWEIT_OHNE_ERST = true;
                else
                {
                    goApp.ZWEIT_OHNE_ERST = false;
                }

                if (oLE.VALDIERUNG_ERST == "J")
                    goApp.VALDIERUNG_ERST = true;
                else
                {
                    goApp.VALDIERUNG_ERST = false;
                }
                if (oLE.FIRMAKU_VAL == "J")
                    goApp.FIRMAKU_VAL = true;
                else
                {
                    goApp.FIRMAKU_VAL = false;
                }
                if (oLE.PDFEXPORT == "J")
                    goApp.PDFEXPORT = true;
                else
                {
                    goApp.PDFEXPORT = false;
                }

                goApp.ErstW = oLE.ERSTWAEGUNGSWAAGE;
                goApp.ZweitW = oLE.ZWEITWAEGUNGSWAAGE;


                Title = Title + "  " + " Straßenfahrzeugwaage für LogIQ";
                if (goApp.acessLevel == "1")
                    Title = Title + "   ----> Service " + goApp.username;

                if (goApp.acessLevel == "2")
                    Title = Title + "  Administrator " + goApp.username;

                if (goApp.acessLevel == "3")
                    Title = Title + "  Benutzer: " + goApp.username;

                if (oLE.WaageAn == "J")
                    goApp.WaageAn = true;

                // DEFAULT Mandant PK eintragen
                Mandant boM = new Mandant();
                goApp.Mandant_PK = boM.GetDefaultMandantPK();

              




                // Module

                var oLe = new Lokaleeinstellungen();
                oLe = oLe.Load();
                var oHi = new HardewareInfo();
                string serHd = oHi.GetVolumeSerialNumber2();

                if (Md5Stuff.VerifyMd5Hash(serHd + "W1789A", oLe.LI_WAAGE))
                    goApp.WaageModulAktiv = true;

                if (Md5Stuff.VerifyMd5Hash(serHd + "FERN523D", oLe.LI_FERNANZEIGE))
                    goApp.FernanzeigeAktive = true;

                if (Md5Stuff.VerifyMd5Hash(serHd + "F0687B", oLe.LI_FUNK))
                    goApp.FunkModulAktiv = true;

                if (Md5Stuff.VerifyMd5Hash(serHd + "K9785C", oLe.LI_KARTEN))
                    goApp.KartenleserModulAktiv = true;


                // Temporäre Abruf löschen
                Abruf boAbruf = new Abruf();
                boAbruf.DeleteOldAbrufe();

                // Abrufe-Nr vom Vortag oder älter aus Kfz löschen
                Fahrzeuge ocf = new Fahrzeuge();
                ocf.DeleteOldAbrufe();

              

                // KeyHandler
                // KeyDown += OnButtonKeyDown;
                PreviewKeyDown += MyPreviewKeyDownHandler;
            }
            catch (InvalidCastException e)
            {
                
                MessageBox.Show( e.ToString());
            }
        }


        private void MyPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {



         
            if (e.Key == Key.Return)
                StartWiegeFrm(); 
        }
         

        /// <summary>
        /// Set security setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecuritySetupMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.SecuritySetupMode = true;
        }

        /// <summary>
        /// Clear security setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecuritySetupMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.SecuritySetupMode = false;
        }

        /// <summary>
        /// Set localize setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizeSetupMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.LocalizeSetupMode = true;
        }

        /// <summary>
        /// Clear localize setup mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizeSetupMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            mmAppWPF.LocalizeSetupMode = false;
        }

        /// <summary>
        /// Display the Users window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mmAppWPF.WindowManager.Show(new UserWindow(), this);
        }

        /// <summary>
        /// Close the main application window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileExitItem_Click(object sender, RoutedEventArgs e)
        {



            var path =  System.AppDomain.CurrentDomain.BaseDirectory;




            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = path+ "\\ExternalIO\\HWB.EXTERNALEXPORT.exe";
            //    myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself.
                // Given that is is started without a window so you cannot terminate it
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method.
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            this.Close();
        }

        private void cmdStartWaegebetrieb_Click(object sender, RoutedEventArgs e)
        {
            StartWiegeFrm();
        }

        private void StartWiegeFrm()
        {
            try
            {
                Forms.WiegeFrm oWFFrm = new WiegeFrm();
                oWFFrm.ShowDialog();
                oWFFrm.Close();
            }
            catch (Exception e)
            {

                Log.Instance.Error(e.Message + " "+ e.InnerException + " "+ e.Source);
            }
           
        }

        private void cmdSetUpWaagen_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(goApp.acessLevel) > 1)
            {
                MessageBox.Show("Sie haben für diese Funktion keine Berechtigung!", "ACHTUNG", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }
            WaageneinstellungenFrm oWEFrm = new WaageneinstellungenFrm();
            PasswortFrm2 oPwFrm = new PasswortFrm2();
            oPwFrm.ShowDialog();
            if (oPwFrm.PWOk)
            {
                oPwFrm.Close();
                oWEFrm.ShowDialog();
                oWEFrm.Close();
            }
            else
            {
                oPwFrm.Close();
            }
        }

        private void cmdSetUp_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(goApp.acessLevel) > 2)
            {
                MessageBox.Show("Sie haben für diese Funktion keine Berechtigung!", "ACHTUNG", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }
            PasswortFrm2 oPwFrm = new PasswortFrm2();
            oPwFrm.ShowDialog();
            if (oPwFrm.PWOk)
            {
                ProgrammeinstellungenFrm oSFrm = new ProgrammeinstellungenFrm();
                oPwFrm.Close();
                oSFrm.ShowDialog();
                oSFrm.Close();
            }
            else
            {
                oPwFrm.Close(); 
            }
           
        }

        private void cmdInfo_Click(object sender, RoutedEventArgs e)
        {
            var oInfoFrm = new InfoFrm();
            oInfoFrm.ShowDialog();
            oInfoFrm.Close();
        }

        private void cmdUser_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(goApp.acessLevel) > 2)
            {
                MessageBox.Show("Sie haben für diese Funktion keine Berechtigung!", "ACHTUNG", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }
            oU = new MyUserListeFrm();
            oU.ShowDialog();
            oU.Close();
        }

      


        private void Module_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(goApp.acessLevel) > 1)
            {
                MessageBox.Show("Sie haben für diese Funktion keine Berechtigung!", "ACHTUNG", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }

            ModulVerwaltungFrm oMV = new ModulVerwaltungFrm();
            oMV.ShowDialog();
            oMV.Close();
        }

        //private void ArbeitsleistungsFilte_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ArbeitsleistungFilterFrm oAFrm = new ArbeitsleistungFilterFrm();
        //    oAFrm.ShowDialog();
        //}

        private void CmdErrorLog_OnClick(object sender, RoutedEventArgs e)
        {
            ErrorLogFrm oEF = new ErrorLogFrm();
            oEF.ShowDialog();
        }
    }
}