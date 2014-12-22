using System;
using System.Windows;
using combit.ListLabel20;
using combit.ListLabel20.DataProviders;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.Waege;
using HWB.NETSCALE.GLOBAL;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    public class PrinterLs
    {
        public void DoPrintLs(Lokaleeinstellungen oLe, WaegeEntity boWe, bool kopie)
        {
            if (boWe == null)
            {
                return;
            }


            var boM = new Mandant();
            MandantEntity boMe = boM.GetMandantByPK(Convert.ToInt32(boWe.Mandant_PK));
            if (boMe == null)
                return;
            string druckerName = boMe.LSDrucker;
            string lsReport = boMe.LSReport;
            int? anzahlausdrucke = boMe.AnzahlLS;
            bool? isLsDruck = boMe.LSDruck;


            var ll = new ListLabel();
            ll.Variables.Add("Original_Kopie", "...");
            ll.Variables.Add("Scheinbezeichnung", "Wiegenote");
            if (kopie) // Wenn Kopie, dann wird die Einstellung aus den Mandanten überschrieben.
            {
                isLsDruck = true;
                ll.Variables.Add("Original_Kopie", "Kopie");
            }


            var boW = new Waege();

            ll.LicensingInfo = "9yJKEQ";


            ObjectDataProvider oDp = boW.GetWaegungOdpbyPk(boWe.PK);

            ll.DataSource = oDp;
            ll.AutoProjectType = LlProject.Label;

            ll.AutoProjectFile = lsReport;
            ll.AutoShowSelectFile = false;
            ll.AutoShowPrintOptions = false;


            // Kopienanzahl  
            int copies = Convert.ToInt32(anzahlausdrucke);

            if (kopie)
                copies = 1;

            // Drucken
            PrintPaperLs(ll, kopie, copies, isLsDruck, druckerName);


            // TODO: Diesen Abschnitt vornehmen: Export Pfad prüfen
            //***************************************************************************
            //  Filename und Pfad (hier: PDF)
            var boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe.LsAsPdf == true)
            {
                int? pdf = boEe.PdfCreator;

                switch (pdf)
                {
                    case 1: // List&Label
                        CreateLsAsPdf(ll);
                        ll.Dispose();
                        break;
                    case 2:
                        if (kopie == false)
                        {
                            CreateLsAsPdfwithStepOver(ll, false, 1, "StepOver PDF Converter");
                        }
                        break;
                }
            }
        }

        private static void PrintPaperLs(ListLabel ll, bool kopie, int copies, bool? isLsDruck, string druckerName)
        {
            try
            {
                for (int nCopy = 0; nCopy < copies; ++nCopy)
                {
                    if (nCopy == 0 & kopie == false)
                    {
                        ll.Variables.Add("Original_Kopie", "Original");
                    }
                    else
                    {
                        ll.Variables.Add("Original_Kopie", "Kopie");
                    }

                    if (isLsDruck == true)
                        ll.Print(druckerName);
                }
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


/*
        private static void CreateLsAsPdf(Lokaleeinstellungen oLe)
        {
            CreateLsAsPdf((IDisposable) null);
        }
*/

        private static void CreateLsAsPdf(IDisposable ll)
        {
            if (ll == null) throw new ArgumentNullException("ll");
            if (goApp.PDFEXPORT)
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

        private static void CreateLsAsPdfwithStepOver(ListLabel ll, bool kopie, int copies, string druckerName)
        {
            try
            {
                for (int nCopy = 0; nCopy < copies; ++nCopy)
                {
                    if (nCopy == 0 & kopie == false)
                    {
                        ll.Variables.Add("Original_Kopie", "Original");
                    }
                    else
                    {
                        ll.Variables.Add("Original_Kopie", "Kopie");
                    }


                    ll.Print(druckerName);
                }
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}