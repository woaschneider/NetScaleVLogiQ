using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.FRONTEND.WPF.Forms;
using MessageBox = Microsoft.Windows.Controls.MessageBox;


// Hier muss ich noch ganz viel Kommentar schreiben!!!

namespace HWB.NETSCALE.FRONTEND.WPF
{
    public class ImportISVNew : IImportInterface 
    {
        private string AktFileName;


        private AP oAP = new AP();
        private APEntity oAPE;

        private MG oMG = new MG();
        private MGEntity oMGE;
        private string Path = "";

        private BackgroundWorker worker;

        private WiegeFrm oWF;

        public ImportISVNew()
        {
           
        }

        public void import(WiegeFrm owf)
        {
            oWF = owf;
            worker = new BackgroundWorker {WorkerReportsProgress = true};
            worker.DoWork += delegate
                                 {
                                     oAP.SetAllTouch2False();
                                     ImportAp();
                                     oAP.DeleteAllNotTouch();

                                     oMG.SetAllTouch2ToFalse();
                                     ImportMg();
                                     oMG.DeleteAllNotTouch();
                                 };
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
                                              else
                                              {
                                              }
                                          };


            Lokaleeinstellungen oBE = new Lokaleeinstellungen();
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

            Path = oBE.IMPORT_PATH;

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Counts the lines of a File.
        /// </summary>
        /// <param name="fileToCount">The file to count.</param>
        /// <returns>lines of a File</returns>
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

        private void ImportAp()
        {
            AktFileName = "Adressen";
            TextDatei c_TextDatei = new TextDatei();

            string c_file = Path + "\\" + "VAP.txt";

            float OneProzNlines = OneProzAreNLines(c_file);


            var lineCount = 0;
            var reader = File.OpenText(c_file);
            while (reader.ReadLine() != null)
            {
                worker.ReportProgress((int) (lineCount/OneProzNlines));

                lineCount++;
                string cLine = c_TextDatei.ReadLine(c_file, lineCount);
                string AeKz = cLine.Substring(299, 1);
                string Mandant = cLine.Substring(0, 3);
                string apnr = cLine.Substring(11, 10);

                if (AeKz == "C" || AeKz == "N")
                {
                    oAPE = oAP.GetAPByNr(apnr) ?? oAP.NewEntity();

                    // Füllen
                    FillAp(cLine);
                    // Ortsteil lassen wir erst mal weg
                    oAPE.touch = true;
                    var uRet = oAP.SaveEntity(oAPE);
                }
                // Schauen ob es den Partner gibt
            }
            reader.Close();
        }

        private void FillAp(string cLine)
        {
            oAPE.Mandant = cLine.Substring(0, 3);
            oAPE.Nr = cLine.Substring(11, 10);
            oAPE.Firma = cLine.Substring(56, 35);
            oAPE.Name1 = cLine.Substring(91, 35);
            oAPE.Anschrift = cLine.Substring(126, 35);
            oAPE.Plz = cLine.Substring(161, 10);
            oAPE.Ort = cLine.Substring(216, 35);
            oAPE.Land = cLine.Substring(251, 3);
            oAPE.bonitaet = cLine.Substring(274, 2);
            oAPE.Seitenzahl = Convert.ToInt32(cLine.Substring(276, 1));

            oAPE.Rolle_AU = cLine.Substring(277, 1) == "X";

            oAPE.Rolle_WE = cLine.Substring(278, 1) == "X";

            oAPE.Rolle_RE = cLine.Substring(279, 1) == "X";

            oAPE.Rolle_REG = cLine.Substring(280, 1) == "X";

            oAPE.Rolle_WERK = cLine.Substring(281, 1) == "X";

            oAPE.Rolle_BF = cLine.Substring(282, 1) == "X";

            oAPE.Rolle_BLT = cLine.Substring(283, 1) == "X";

            oAPE.RolleERZ = cLine.Substring(284, 1) == "X";

            oAPE.Role_ABFS = cLine.Substring(285, 1) == "X";

            oAPE.Rolle_ENT = cLine.Substring(286, 1) == "X";

            oAPE.Rolle_AESA = cLine.Substring(287, 1) == "X";

            oAPE.Rolle_BEHOER = cLine.Substring(288, 1) == "X";

            oAPE.Rolle_BEFOER = cLine.Substring(289, 1) == "X";

            oAPE.Rolle_LI = cLine.Substring(290, 1) == "X";

            oAPE.Rolle_SP = cLine.Substring(291, 1) == "X";

            oAPE.Rolle_FU = cLine.Substring(292, 1) == "X";

            oAPE.Rolle_BUK = cLine.Substring(293, 1) == "X";

            oAPE.Rolle_VKORG = cLine.Substring(294, 1) == "X";

            oAPE.Rolle_VKB = cLine.Substring(295, 1) == "X";

            oAPE.Rolle_EKORG = cLine.Substring(296, 1) == "X";

            oAPE.Rolle_ZL = cLine.Substring(297, 1) == "X";

            oAPE.Rolle_ZW = cLine.Substring(298, 1) == "X";

            oAPE.AEKZ = cLine.Substring(299, 1);

            oAPE.Rolle_RQ = cLine.Substring(300, 1) == "X";
        }

        private void ImportMg()
        {
            AktFileName = "Material/Sorten";
            TextDatei c_TextDatei = new TextDatei();

            string c_file = Path + "\\" + "VMG.txt";

            float OneProzNlines = OneProzAreNLines(c_file);


            var lineCount = 0;
            var reader = File.OpenText(c_file);

            while (reader.ReadLine() != null)
            {
                worker.ReportProgress((int) (lineCount/OneProzNlines));

                lineCount++;
                string cLine = c_TextDatei.ReadLine(c_file, lineCount);
                string AeKz = cLine.Substring(218, 1);
                string Mandant = cLine.Substring(0, 3);
                string mgnr = cLine.Substring(17, 18);

                if (AeKz == "C" || AeKz == "N")
                {
                    oMGE = oMG.GetMGByNr(mgnr) ?? oMG.NewEntity();

                    // Füllen
                    FillMg(cLine);
                    // Ortsteil lassen wir erst mal weg
                    oMGE.touch = true;
                    var uRet = oMG.SaveEntity(oMGE);
                }
            }
            reader.Close();
        }

        private void FillMg(string cLine)
        {
            oMGE.Mandant = cLine.Substring(0, 3);
            oMGE.Werksnr = cLine.Substring(9, 4);
            oMGE.SortenNr = cLine.Substring(17, 18);
            oMGE.Sortenbezeichnung1 = cLine.Substring(35, 40);
            oMGE.Sortenbezeichnung2 = cLine.Substring(75, 40);
        }
    }
}