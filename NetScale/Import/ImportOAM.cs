using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.FRONTEND.WPF.Forms;

// Schiewe Import. Ehemalig ISV. Jetzt mit Änderungen bei den Längen der Sortenbezeichnungen.

namespace HWB.NETSCALE.FRONTEND.WPF
{
    internal class ImportOAM : IImportInterface
    {
        private string localDir;
        private string AktFileName;
        private int lineCount;
        private float OneProzNlines;
        private string c_file;
        private string sPath = "";

        private AP oAP = new AP();
        private APEntity oAPE;
        private MG oMG = new MG();
        private MGEntity oMGE;
        private KK oKK = new KK();
        private KKEntity oKKE;
        private KM oKM = new KM();
        private KMEntity oKME;

        private Lokaleeinstellungen oBE;

        private StreamReader reader;
        private BackgroundWorker worker;

        private WiegeFrm oWF;

        public ImportOAM()
        {
            oBE = new Lokaleeinstellungen();
            oBE = oBE.Load();
            if (oBE.IMPORT_PATH == null)
            {
                MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Importpfades!",
                                "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            oBE.Load();
            if (oBE.IMPORT_PATH == "")
            {
                MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Importpfades!",
                                "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
        }


        public void import(WiegeFrm owf)
        {
            oWF = owf;
            sPath = oBE.IMPORT_PATH;

            // Kapselt den eigentlichen Import und aktualisiert die ProgressBar.
            ProgressBarThread();
        }

        private void ProgressBarThread()
        {
            worker = new BackgroundWorker {WorkerReportsProgress = true};
            worker.DoWork += delegate { ExecuteImport(); }; // <------------------- Execute Import
            worker.RunWorkerCompleted += delegate
                                             {
                                                 //  object result = args.Result;
                                                 oWF.progressBar1.Width = 0;
                                                 oWF.ProgressLabel.Width = 0;
                                                 MessageBox.Show("Import ist fertig");
                                             };

            worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
                                          {
                                              oWF.progressBar1.Width = 719;
                                              oWF.ProgressLabel.Width = 134;
                                              oWF.progressBar1.Value = args.ProgressPercentage;
                                              if (args.ProgressPercentage < 101)
                                              {
                                                  oWF.ProgressLabel.Content = AktFileName + " " +
                                                                              args.ProgressPercentage + " %";
                                              }
                                          };


            worker.RunWorkerAsync();
        }

        private void ExecuteImport()
        {
            CopyAllImportFilesToLocalDrive();

            oAP.SetAllTouch2False();
            ImportAp("OAM_Export_ADRESSEN.TXT");
            // ImportFuhrunternehmer("Fuhrunternehmer");
            oAP.DeleteAllNotTouch();
            oMG.SetAllTouch2ToFalse();
            ImportMg("OAM_Export_SORTEN.TXT");
            oMG.DeleteAllNotTouch();
            oKK.SetAllTouch2False();
            ImportKK("OAM_Export_KONTRAKTKOPF.TXT");
            oKK.DeleteAllNotTouch();

         //   ImportKm("OAM_Export_KONTRAKTDETAIL.TXT");
            oKM.DeleteAllNotTouch();

        }

        private void CopyAllImportFilesToLocalDrive()
        {
            localDir = Environment.CurrentDirectory + "\\Temp\\";
            if (!Directory.Exists(localDir + "\\Temp"))
            {
                Directory.CreateDirectory(localDir);
            }

            
            DirectoryInfo dest = new DirectoryInfo(localDir);
            DirectoryInfo scr = new DirectoryInfo(sPath);

            // 9.1.2014 Vorher alle Files im Zielverzeichnis löschen
            string[] filePaths = Directory.GetFiles(localDir);
            foreach (string filePath in filePaths)
                File.Delete(filePath);
            //
            CopyDirectory(scr, dest);
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName,
                                         file.Name), true);
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }

        private void ImportAp(string FileName)
        {
            TextDatei c_TextDatei = GetCTextDatei(FileName);


            while (reader.ReadLine() != null)
            {
                worker.ReportProgress((int) (lineCount/OneProzNlines));

                lineCount++;
                string cLine = c_TextDatei.ReadLine(c_file, lineCount);
             

                string apnr = cLine.Substring(2, 10);
                if (apnr == "          ") // Leere Zeile
                    return;
             
                    oAPE = oAP.GetAPByNr(apnr);
                    if (oAPE == null)
                           oAPE = oAP.NewEntity();
                

                // Füllen
                    FillAp(cLine);


                    var uRet = oAP.SaveEntity(oAPE);
              
                // Schauen ob es den Partner gibt
            }
            reader.Close();
        }
        
        // TODO:
        private void FillAp(string cLine)
        {
            // oAPE.Mandant = cLine.Substring(0, 2);

            oAPE.Nr = cLine.Substring(2, 10);
           
            oAPE.Firma = cLine.Substring(12, 50);
            oAPE.Name1 = cLine.Substring(62, 50);
            oAPE.Name2 = cLine.Substring(112, 50);
           oAPE.Anschrift = cLine.Substring(162, 40);
          oAPE.Land = cLine.Substring(202, 3);
           oAPE.Plz = cLine.Substring(205, 6);
           oAPE.Ort = cLine.Substring(211, 50);
            oAPE.Rolle_AU = true;
            oAPE.Rolle_LI = true;
            oAPE.Rolle_SP = false;
            oAPE.Rolle_FU = false;
            oAPE.bonitaet = cLine.Substring(167, 1);


            oAPE.touch = true;
        }

        private void ImportFuhrunternehmer(string FileName)
        {
            TextDatei c_TextDatei = GetCTextDatei(FileName);
            if (c_TextDatei == null)
                return;

            while (reader.ReadLine() != null)
            {
                worker.ReportProgress((int) (lineCount/OneProzNlines));

                lineCount++;
                string cLine = c_TextDatei.ReadLine(c_file, lineCount);
                if (cLine.Length < 5)
                    return;

                string AeKz = cLine.Substring(7, 1);

                string apnr = cLine.Substring(2, 5);

                if ((AeKz == "C" || AeKz == "N" || AeKz == " ") && cLine.Substring(0, 3) != "$$$")
                    // $$$ Prüfen auf Kopfzeile 
                {
                    oAPE = oAP.GetAPByNr(apnr) ?? oAP.NewEntity();

                    // Füllen
                    FillFuhrunternehmer(cLine);
                    // Ortsteil lassen wir erst mal weg

                    var uRet = oAP.SaveEntity(oAPE);
                }
                // Schauen ob es den Partner gibt
            }
            reader.Close();
        }

        private void FillFuhrunternehmer(string cLine)
        {
            // TODO SPEDI 

            oAPE.Nr = cLine.Substring(2, 5);
            oAPE.Firma = cLine.Substring(8, 30);
            oAPE.Name1 = cLine.Substring(38, 30);
            oAPE.Name2 = cLine.Substring(68, 30);
            oAPE.Anschrift = cLine.Substring(98, 30);
            oAPE.Land = cLine.Substring(128, 3);
            oAPE.Plz = cLine.Substring(131, 5);
            int RestLaenge = cLine.Length - 137;
            oAPE.Ort = cLine.Substring(137, RestLaenge);

            oAPE.Rolle_AU = false;
            oAPE.Rolle_LI = false;
            oAPE.Rolle_SP = false;
            oAPE.Rolle_FU = true;

            oAPE.touch = true;
        }

        private void ImportMg(string FileName)
        {
            TextDatei c_TextDatei = GetCTextDatei(FileName);

            while (reader.ReadLine() != null)
            {
                worker.ReportProgress((int) (lineCount/OneProzNlines));

                lineCount++;
                string cLine = c_TextDatei.ReadLine(c_file, lineCount);
              
                string mgnr = cLine.Substring(0, 8);

              
                    oMGE = oMG.GetMGByNr(mgnr) ?? oMG.NewEntity();

                    // Füllen
                    FillMg(cLine);
                    // Ortsteil lassen wir erst mal weg
                    oMGE.touch = true;
                    var uRet = oMG.SaveEntity(oMGE);
            }
            reader.Close();
        }

        private void FillMg(string cLine)
        {
          //  oMGE.Mandant = cLine.Substring(0, 2);
          //  oMGE.Werksnr = cLine.Substring(5, 3);
            oMGE.SortenNr = cLine.Substring(0, 8);
            oMGE.Sortenbezeichnung1 = cLine.Substring(8, 50); 
            oMGE.Sortenbezeichnung2 = cLine.Substring(58, 50);
            oMGE.Sortenbezeichnung2 = cLine.Substring(108, 50);

    //        oMGE.preisvk = Convert.ToDecimal(cLine.Substring(158, 16))/1000; // // 98 auf 118
        //    oMGE.ph = cLine.Substring(266, 18);    // 246 auf 266 
        //    oMGE.Siegel1 = cLine.Substring(286, 1) == "1" ? true : false; // Und hier den ganzen Rest um 20 Stellen verschoben.
         //   oMGE.Siegel2 = cLine.Substring(287, 1) == "1" ? true : false;
        //    oMGE.Siegel3 = cLine.Substring(288, 1) == "1" ? true : false;
       //     oMGE.Siegel4 = cLine.Substring(289, 1) == "1" ? true : false;
            // 
            if (oMGE.me == null)
                oMGE.me = "t";
        }

        private void ImportKK(string FileName)
        {
            TextDatei c_TextDatei = GetCTextDatei(FileName);

            if (c_TextDatei == null)
                return;

            while (reader.ReadLine() != null)
            {
                worker.ReportProgress((int) (lineCount/OneProzNlines));

                lineCount++;
                string cLine = c_TextDatei.ReadLine(c_file, lineCount);
                string AeKz = cLine.Substring(29, 1);

                string auftragnr = cLine.Substring(2, 8);

                if ((AeKz == "C" || AeKz == "N" || AeKz == " ") & cLine.Substring(0, 3) != "$$$")
                    // $$$ Prüfen auf Kopfzeile 
                {
                    oKKE = oKK.GetKKByAuftragsNr(auftragnr) ?? oKK.NewEntity();
                    oKME = oKM.GetKMByAuftragsNr(auftragnr) ?? oKM.NewEntity();
                    // Füllen
                    FillKK(cLine);


                    var uRet = oKK.SaveEntity(oKKE);
                }
                // Schauen ob es den Partner gibt
            }

            reader.Close();
        }
        private void FillKK(string cLine)
        {
            oKKE.mandant = cLine.Substring(0, 2);
            oKKE.kontraktnr = cLine.Substring(2, 8);
            oKKE.werksnr = cLine.Substring(13, 3);
            oKKE.auftragsart = cLine.Substring(30, 2);
            oKKE.wefirma = cLine.Substring(32, 30);
            oKKE.wename1 = cLine.Substring(62, 30);
            oKKE.partnernrAU = cLine.Substring(93, 5);
            var APFK = oAP.GetAPByNr(oKKE.partnernrAU);
            if (APFK != null)
                oKKE.APFK = APFK.PK;
            // TODO ? Produktgruppe
            //
            oKKE.touch = true;
        }


        private void ImportKM(string FileName)
        {
        }
        private void FillKM(string cLine)
        {
        }

        private TextDatei GetCTextDatei(string FileName)
        {
            AktFileName = FileName;
            TextDatei c_TextDatei = new TextDatei();
            // sPath = localDir + "\\";


            string[] oFiles = Directory.GetFiles(localDir,"*"+ FileName + "*");
            if (oFiles.Length != 0) // (oFiles[0] != null)
            {
                c_file = oFiles[0].ToString();
                OneProzNlines = OneProzAreNLines(c_file);
                lineCount = 0;
                reader = new StreamReader(c_file, Encoding.GetEncoding(65001));
                // var x = reader.CurrentEncoding.CodePage;//'Autodetected encoding
                reader = File.OpenText(c_file);
            }

            else
            {
                c_TextDatei = null;
                return c_TextDatei;
            }
            return c_TextDatei;
        }

        private static float OneProzAreNLines(string fileToCount)
        {
            int counter = 0;
            using (StreamReader countReader = new StreamReader(fileToCount))
            {
                while (countReader.ReadLine() != null)
                    counter++;
            }
            // ReSharper disable PossibleLossOfFraction
            return counter/100;
            // ReSharper restore PossibleLossOfFraction
        }
    }
}