using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using combit.ListLabel17;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.User;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.WPF;
using MessageBox = System.Windows.MessageBox;
using ObjectDataProvider = combit.ListLabel17.DataProviders.ObjectDataProvider;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ProgrammeinstellungenFrm Class
    /// 
    public partial class ProgrammeinstellungenFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private NETSCALE.BOEF.Lokaleeinstellungen oLE;

        private Einstellungen boE;
        private EinstellungenEntity boEE;
        private Mandant boM;
        private Wiegeart boWA;
        private WiegeartEntity boWAE;
        private Incoterm boI;
        private IncotermEntity boIE;


        public ProgrammeinstellungenFrm()
        {
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            this.InitializeComponent();

            FillPrinterDataGrid();

            //Installierte Drucker auf dem Rechner einlesen und in beide Comboboxen schreiben
            PopulateInstalledPrintersCombo();

            lblEinheit.Content = goApp.MengenEinheit;
            oLE = new Lokaleeinstellungen();
            oLE = oLE.Load();
            FillFrm();
            boE = new Einstellungen();
            boEE = boE.GetEinstellungen();
            if (boEE == null)
            {
                MessageBox.Show("Die Tabelle Einstellungen auf dem SQL-Server ist leer!", "Warnung",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                MessageBox.Show("Es wird nun ein neuer Datensatz in der Tabelle Einstellungen angelegt", "Hinweis",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                boEE = boE.NewEntity();
                boEE.LSNRGlobal = 0;
                boE.SaveEntity(boEE);
            }

            DataContext = boEE;
            cbPdfaktiv.IsChecked = boEE.LsAsPdf;
            // Nur weil man das schnell vergißt:
            // Val. MaxGewicht u. Ein/Aus der selben ist nicht lokal sondern in der Servertabelle "Einstellungen" abgelegt.
            // Aus diesem Grund sind die beiden Properties gebunden

            // Wiegeart und Incotermdefault
            boWA = new Wiegeart();
            boWAE = boWA.GetDefaultWiegeart();
            if (boWAE != null)
            {
                tb_wiegeart.Text = boWAE.Kennung;
                tb_wiegearttext.Text = boWAE.Bezeichnung;
            }

            boI = new Incoterm();
            boIE = boI.GetDefaultIncoterm();
            if (boIE != null)
            {
                tb_incoterm.Text = boIE.Kennung;
                tb_incotermtext.Text = boIE.Bezeichnung;
            }
        }

        private void FillFrm()
        {
            combo_LISTE_InstalledPrinters.Text = oLE.LISTENDRUCKER;


            if (oLE.AUTOKFZ == "J")
                cb_autokfz.IsChecked = true;
            if (oLE.AUTOABRUF == "J")
                cb_autoabruf.IsChecked = true;
            if (oLE.SAVE_ABR2CF == "J")
                cb_save_abr2cf.IsChecked = true;
            if (oLE.TAKE_LAST_ABR == "J")
                cb_take_last_abr.IsChecked = true;
            if (oLE.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN == "J")
                cb_onnew_setfocus_to_kfzkennzeichen.IsChecked = true;
            if (oLE.SAVE_ERST2CFTARA == "J")
                cb_save_erst2cftara.IsChecked = true;
            if (oLE.ZWEIT_OHNE_ERST == "J")
                cb_zweit_ohne_erst.IsChecked = true;
            if (oLE.VALDIERUNG_ERST == "J")
                cb_keine_Val_bei_Erst.IsChecked = true;
            if (oLE.FIRMAKU_VAL == "J")
                cb_firmaku_Val.IsChecked = true;
            if (oLE.PDFEXPORT == "J")
                cb_pdfexport.IsChecked = true;


            if (oLE.WaageAn == "J")
                cb_WaageAn.IsChecked = true;

            if (oLE.MISCHEREXPORT == "J")
                cb_MischerExport.IsChecked = true;


            tb_kartenlesercomport.Text = oLE.KARTENLESERCOMPORT;
            tb_funkmodulcomport.Text = oLE.FUNKMODULCOMPORT;
            tb_fernanzeigecomport.Text = oLE.FERNANZEIGECOMPORT;
            tb_pdf_path.Text = oLE.PDF_PATH;
            tb_importpath.Text = oLE.IMPORT_PATH;
            tb_exportpath.Text = oLE.EXPORT_PATH;
            tb_mischerexportpfad.Text = oLE.MISCHEREXPORT_PATH;
            tb_listenfile.Text = oLE.WIEGELISTFILE;
            tb_listenKundenfile.Text = oLE.WIEGELIST_KU_FILE;
            tb_listenKfzfile.Text = oLE.WIEGELIST_KFZ_FILE;
            tb_erstw.Text = oLE.ERSTWAEGUNGSWAAGE;
            tb_zweitw.Text = oLE.ZWEITWAEGUNGSWAAGE;
        }

        private void FillPrinterDataGrid()
        {
            boM = new Mandant();
            dataGrid1.SelectedValuePath = "PK";

            dataGrid1.ItemsSource = boM.GetAllMandant();
        }

        private void PopulateInstalledPrintersCombo()
        {
            // Add list of installed printers found to the combo box.
            // The pkInstalledPrinters string will be used to provide the display string.
            String pkInstalledPrinters;
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                pkInstalledPrinters = PrinterSettings.InstalledPrinters[i];

                combo_LISTE_InstalledPrinters.Items.Add(pkInstalledPrinters);
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            FillFrm();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)

        {
            tb_incotermtext.Focus(); // Dummy Ersatz
            oLE.LISTENDRUCKER = combo_LISTE_InstalledPrinters.Text;

            if (cb_autokfz.IsChecked == true)
                oLE.AUTOKFZ = "J";
            else
            {
                oLE.AUTOKFZ = "N";
            }

            if (cb_autoabruf.IsChecked == true)
                oLE.AUTOABRUF = "J";
            else
            {
                oLE.AUTOABRUF = "N";
            }

            if (cb_save_abr2cf.IsChecked == true)
                oLE.SAVE_ABR2CF = "J";
            else
            {
                oLE.SAVE_ABR2CF = "N";
            }

            if (cb_take_last_abr.IsChecked == true)
                oLE.TAKE_LAST_ABR = "J";
            else
            {
                oLE.TAKE_LAST_ABR = "N";
            }

            if (cb_onnew_setfocus_to_kfzkennzeichen.IsChecked == true)
                oLE.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN = "J";
            else
            {
                oLE.ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN = "N";
            }

            if (cb_save_erst2cftara.IsChecked == true)
                oLE.SAVE_ERST2CFTARA = "J";
            else
            {
                oLE.SAVE_ERST2CFTARA = "N";
            }

            if (cb_zweit_ohne_erst.IsChecked == true)
                oLE.ZWEIT_OHNE_ERST = "J";
            else
            {
                oLE.ZWEIT_OHNE_ERST = "N";
            }

            if (cb_keine_Val_bei_Erst.IsChecked == true)
                oLE.VALDIERUNG_ERST = "J";
            else
            {
                oLE.VALDIERUNG_ERST = "N";
            }

            if (cb_firmaku_Val.IsChecked == true)
                oLE.FIRMAKU_VAL = "J";
            else
            {
                oLE.FIRMAKU_VAL = "N";
            }


            if (cb_pdfexport.IsChecked == true)
                oLE.PDFEXPORT = "J";
            else
            {
                oLE.PDFEXPORT = "N";
            }


            if (cb_WaageAn.IsChecked == true)
            {
                oLE.WaageAn = "J";
                goApp.WaageAn = true;
            }
            else
            {
                oLE.WaageAn = "N";
                goApp.WaageAn = false;
            }

            if (cb_MischerExport.IsChecked == true)
            {
                oLE.MISCHEREXPORT = "J";
            }
            else
            {
                oLE.MISCHEREXPORT = "N";
            }

            oLE.KARTENLESERCOMPORT = tb_kartenlesercomport.Text;
            oLE.FUNKMODULCOMPORT = tb_funkmodulcomport.Text;
            oLE.FERNANZEIGECOMPORT = tb_fernanzeigecomport.Text;
            oLE.PDF_PATH = tb_pdf_path.Text;
            oLE.IMPORT_PATH = tb_importpath.Text;
            oLE.EXPORT_PATH = tb_exportpath.Text;
            oLE.MISCHEREXPORT_PATH = tb_mischerexportpfad.Text;
            oLE.WIEGELISTFILE = tb_listenfile.Text;
            oLE.WIEGELIST_KU_FILE = tb_listenKundenfile.Text;
            oLE.WIEGELIST_KFZ_FILE = tb_listenKfzfile.Text;
            oLE.ERSTWAEGUNGSWAAGE = tb_erstw.Text;
            oLE.ZWEITWAEGUNGSWAAGE = tb_zweitw.Text;

            boEE.LsAsPdf = cbPdfaktiv.IsChecked;

            // TODO Die folgende Zeile tut es nicht #
            // Wochenende nun!!!
            lblEinheit.Focus(); // Damit das Datagrid verlassen und richtig gesichert wird
            oLE.Save(oLE);

            boE.SaveEntity(boEE);
            boM.SaveEntityList();

            // ****
            boWA.SetDefaultWiegeart(tb_wiegeart.Text);
            boI.SetDefaultIncoterm(tb_incoterm.Text);

            // ****
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }


        private void cmdEditLS_Click(object sender, RoutedEventArgs e)
        {
            ListLabel LL = new ListLabel();
            LL.LicensingInfo = "tHrTEQ";
            Waege boW = new Waege();
            combit.ListLabel17.DataProviders.ObjectDataProvider oDP = boW.GetLSByLSNRGlobal("13");

            LL.Variables.Add("Scheinbezeichnung", "Wiegenote");
            LL.Variables.Add("Original_Kopie", "Original");

            // Unterschriftendatei
            User boU = new User();
            int Wpk = boW.GetLastWaegung();
            UserEntity boUE;
            WaegeEntity boWE = null;

            if (Wpk != null)
                boWE = boW.GetWaegungByID(Wpk);

            if (boWE.UserPK != null)
            {
                boUE = boU.GetUserById(boWE.UserPK);

                if (boUE != null)
                {
                    LL.Variables.AddFromObject(boUE);
                }
            }
            // Unterschrift Ende


            LL.DataSource = oDP;


            LL.AutoProjectType = LlProject.Label;
            try
            {
                LL.Design();
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }


            //     ExportConfiguration exportConfig = new ExportConfiguration(LlExportTarget.Pdf, , "<Projektdateiname mit Pfad>");
        }


        private void cmdEditAPListe_Click(object sender, RoutedEventArgs e)
        {
            ListLabel LL = new ListLabel();
            LL.LicensingInfo = "tHrTEQ";
            AP boAP = new AP();
            mmBindingList<APEntity> boAPE = boAP.GetAllAP();
            LL.DataSource = boAPE;
            try
            {
                LL.Design();
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CmdGetComPortsKartenleserClick(object sender, RoutedEventArgs e)
        {
            tb_kartenlesercomport.Text = LookUpComPorts();
        }

        private void CmdGetComPortsFunkmodulClick(object sender, RoutedEventArgs e)
        {
            tb_funkmodulcomport.Text = LookUpComPorts();
        }

        private void cmdGetComPortsFernanzeige_Click(object sender, RoutedEventArgs e)
        {
            tb_fernanzeigecomport.Text = LookUpComPorts();
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


        private void cmdGetDirPdf_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                tb_pdf_path.Text = folderDialog.SelectedPath;
        }

        private void cmdGetDirImport_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                tb_importpath.Text = folderDialog.SelectedPath;
        }

        private void cmdGetDirExport_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                tb_exportpath.Text = folderDialog.SelectedPath;
        }

        private void cmdGetMischerExportPfad_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                tb_mischerexportpfad.Text = folderDialog.SelectedPath;
        }

        private void CmdGetDirLsClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


// Set filter for file extension and default file extension
            dlg.DefaultExt = ".lbl";

            dlg.Filter = "Text documents (.lbl)|*.lbl";


// Display OpenFileDialog by calling ShowDialog method

            Nullable<bool> result = dlg.ShowDialog();


// Get the selected file name and display in a TextBox

            if (result == true)

            {
                // Open document

                string filename = dlg.FileName;


                int uRet = Convert.ToInt32(dataGrid1.SelectedValue);
                // boM.Entity.LSReport = filename;
                MandantEntity boME = boM.GetMandantByPK(uRet);
                boME.LSReport = filename;
            }
        }

        private void CmdGetDirWiegeListFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".lst";

            dlg.Filter = "Text documents (.lst)|*.lst";


            // Display OpenFileDialog by calling ShowDialog method

            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox

            if (result == true)
            {
                // Open document

                string filename = dlg.FileName;


                tb_listenfile.Text = filename;
            }
        }

        private void cmdWiegelistKunden_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".lst";

            dlg.Filter = "Text documents (.lst)|*.lst";


            // Display OpenFileDialog by calling ShowDialog method

            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox

            if (result == true)
            {
                // Open document

                string filename = dlg.FileName;


                tb_listenKundenfile.Text = filename;
            }
        }

        private void cmdWiegelistKfz_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".lst";

            dlg.Filter = "Text documents (.lst)|*.lst";


            // Display OpenFileDialog by calling ShowDialog method

            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox

            if (result == true)
            {
                // Open document

                string filename = dlg.FileName;


                tb_listenKfzfile.Text = filename;
            }
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }

        private void cmdKeyBoard_Click(object sender, RoutedEventArgs e)
        {
            FnnFrm oFnnFrm = new FnnFrm();
            oFnnFrm.ShowDialog();
        }


        private void tb_wiegeart_LostFocus(object sender, RoutedEventArgs e)
        {
            Wiegeart boWA = new Wiegeart();
            var boWAE = boWA.GetWiegeartByKz(tb_wiegeart.Text);
            if (boWAE != null)
            {
                tb_wiegeart.Text = boWAE.Kennung;
                tb_wiegearttext.Text = boWAE.Bezeichnung;
            }
            else
            {
                tb_wiegeart.Text = "";
                tb_wiegearttext.Text = "";
            }
        }

        private void tb_incoterm_LostFocus(object sender, RoutedEventArgs e)
        {
            Incoterm boI = new Incoterm();
            var boIE = boI.GetIncotermByKz(tb_incoterm.Text);
            if (boIE != null)
            {
                tb_incoterm.Text = boIE.Kennung;
                tb_incotermtext.Text = boIE.Bezeichnung;
            }
            else
            {
                tb_incoterm.Text = "";
                tb_incotermtext.Text = "";
            }
        }

        private void tb_erstw_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!HWB.MyFunctions.InList(tb_erstw.Text, "1", "2"))
                tb_erstw.Text = "1";

            if (!HWB.MyFunctions.InList(tb_zweitw.Text, "1", "2"))
                tb_zweitw.Text = "1";
        }

        private void tb_ust_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_ust.Text = WindowExtensions.TextBoxOnlyDecimal(e, tb_ust.Text);
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}