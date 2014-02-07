﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using combit.ListLabel17;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    public class PrinterLS
    {
        public void DoPrintLS(Lokaleeinstellungen oLE, WaegeEntity boWE, bool Kopie)
        {
            bool? isLSDruck;


            if (boWE == null)
            {
                return;
            }
            string DruckerName;
            string LSReport; // Vorlage
            int? Anzahlausdrucke;


            ListLabel LL = new ListLabel();
            LL.Variables.Add("Original_Kopie", "...");
            LL.Variables.Add("Scheinbezeichnung", "Wiegenote");

            Mandant boM = new Mandant();
            MandantEntity boME = boM.GetMandantByPK(Convert.ToInt32(boWE.PK_Mandant));
            if (boME == null)
                return;
            DruckerName = boME.LSDrucker;
            LSReport = boME.LSReport;
            Anzahlausdrucke = boME.AnzahlLS;
            isLSDruck = boME.LSDruck;
            if (Kopie == true) // Wenn Kopie, dann wird die Einstellung aus den Mandanten überschrieben.
            {
                isLSDruck = true;
                LL.Variables.Add("Original_Kopie", "Kopie");
            }


            Waege boW = new Waege();

            LL.LicensingInfo = "wOGzEQ";
            // combit.ListLabel17.DataProviders.ObjectDataProvider oDP = boW.GetLSByLSNR(boWE.LSNR); // Wieso LSNR ???
            combit.ListLabel17.DataProviders.ObjectDataProvider oDP = boW.GetWaegungODPByID(boWE.PK);

            LL.DataSource = oDP;
            LL.AutoProjectType = LlProject.Label;

            LL.AutoProjectFile = LSReport;
            LL.AutoShowSelectFile = false;
            LL.AutoShowPrintOptions = false;

            // Kopienanzahl  
            int copies = Convert.ToInt32(Anzahlausdrucke);
            // Schiewe
            if (boWE.BonitaetKz == "7")
                copies = 2;
            //
            if (Kopie == true)
            {
                copies = 1;
            }

            try
            {
                for (int nCopy = 0; nCopy < copies; ++nCopy)
                {
                    if (nCopy == 0 & Kopie == false)
                    {
                        LL.Variables.Add("Original_Kopie", "Original");
                    }
                    else
                    {
                        LL.Variables.Add("Original_Kopie", "Kopie");
                    }

                    if (isLSDruck == true)
                        LL.Print(DruckerName);
                }
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }

            // TODO: Diesen Abschnitt vornehmen: Export Pfad prüfen
            //***************************************************************************
            //  Filename und Pfad (hier: PDF)
            if (goApp.PDFEXPORT == true)
            {
                try
                {
                    LL.ExportOptions.Add(LlExportOption.ExportTarget, "PDF");
                    LL.ExportOptions.Add(LlExportOption.ExportFile,
                                         boWE.LSNRGlobal + "_" + DateTime.Now.ToString().Replace(":", "") + ".pdf");
                    LL.ExportOptions.Add(LlExportOption.ExportPath, oLE.PDF_PATH);
                    // Dateiauswahldialog unterdrücken
                    LL.ExportOptions.Add(LlExportOption.ExportQuiet, "1");
                    // Export starten
                    LL.Print();
                }
                catch (ListLabelException ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Exportpfad für PDF's eingerichtet?");
                }
            }

            LL.Dispose();
        }
    }
}