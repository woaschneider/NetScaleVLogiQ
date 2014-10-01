using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using combit.ListLabel17;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.User;
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

            // Unterschriftendatei
            // Unterschriftendatei
            User boU = new User();
       
            UserEntity boUE;
           

         

      


            Waege boW = new Waege();

            LL.LicensingInfo = "wOGzEQ";
            // combit.ListLabel17.DataProviders.ObjectDataProvider oDP = boW.GetLSByLSNR(boWE.LSNR); // Wieso LSNR ???
       

        
   

         

         

            // TODO: Diesen Abschnitt vornehmen: Export Pfad prüfen
            //***************************************************************************
            //  Filename und Pfad (hier: PDF)
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEE = boE.GetEinstellungen();
            if (boEE.LsAsPdf == true)
            {
                int? pdf = boEE.PdfCreator;

                switch (pdf)
                {
                    case 1: // List&Label
                        CreateLsAsPdf(boWE, LL, oLE);
                        LL.Dispose();
                        break;
                    case 2:
                        if (Kopie == false)
                        {
                            CreateLsAsPdfwithStepOver(LL, false, 1, "StepOver PDF Converter");
                        }
                        break;
                }
            }
        }

        private void PrintPaperLs(ListLabel LL, bool Kopie, int copies, bool? isLSDruck, string DruckerName)
        {
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
        }


        private void CreateLsAsPdf(WaegeEntity boWE, ListLabel LL, Lokaleeinstellungen oLE)
        {
            if (goApp.PDFEXPORT == true)
            {
                try
                {
              
                }
                catch (ListLabelException ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Exportpfad für PDF's eingerichtet?");
                }
            }
        }

        private void CreateLsAsPdfwithStepOver(ListLabel LL, bool Kopie, int copies,string DruckerName)
        {
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

                   
                        LL.Print(DruckerName);
                }
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}