using System;
using System.Windows;
using combit.ListLabel21;
using combit.ListLabel21.DataProviders;
using HWB.NETSCALE.BOEF;

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
            MandantEntity boMe = boM.GetMandantByPK(Convert.ToInt32(goApp.Mandant_PK));
            if (boMe == null)
                return;
            string druckerName = boMe.LSDrucker;
            string lsReport = boMe.LSReport;
           // Neu 14.01.2014 Auftraggeber abhängiger Druck

            Adressen boA = new Adressen();
            AdressenEntity boAe = boA.GetByBusinenessIdentifier(boWe.customerBusinessIdentifier);
            if (boAe != null)
            {
                if (!string.IsNullOrEmpty( boAe.Lieferscheinvorlage))
                {
                    lsReport = boAe.Lieferscheinvorlage;
                }

                if (!string.IsNullOrEmpty(boAe.Drucker))
                {
                    druckerName = boAe.Drucker;
                }
            }

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

            ll.LicensingInfo = "pWFZEQ";


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
            PrintPaperLs(ll, kopie, copies, isLsDruck, druckerName,boWe.attributes_as_json);


            // TODO: Diesen Abschnitt vornehmen: ExportAll Pfad prüfen
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
            else
            {
                ll.Dispose();
            }
          
        }
        public void PrintLz(WaegeEntity boWe)
        {
            if (boWe == null)
            {
                return;
            }


            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe == null)
            {
                return;
            }

            if (boEe.PrintLaufzettel != true)
            {
                return;
            }

            var boM = new Mandant();
            MandantEntity boMe = boM.GetMandantByPK(Convert.ToInt32(goApp.Mandant_PK));
            if (boMe == null)
                return;
            string druckerName = boMe.LSDrucker;
            
            string lsReport = boEe.ReportLaufzettel;
            // Neu 14.01.2014 Auftraggeber abhängiger Druck

            int? copies = boEe.AnzahlLz;

    
           


            var ll = new ListLabel();
          
       


            var boW = new Waege();

            ll.LicensingInfo = "pWFZEQ";


            ObjectDataProvider oDp = boW.GetWaegungOdpbyPk(boWe.PK);

            ll.DataSource = oDp;
            ll.AutoProjectType = LlProject.Label;

            ll.AutoProjectFile = lsReport;
            ll.AutoShowSelectFile = false;
            ll.AutoShowPrintOptions = false;
            for (int nCopy = 0; nCopy < copies; ++nCopy)
            {

                ll.Print(druckerName);

            }
        }

        private static void PrintPaperLs(ListLabel ll, bool kopie, int copies, bool? isLsDruck, string druckerName,string attribute_as_json)
        {
            try
            {
                for (int nCopy = 0; nCopy < copies; ++nCopy)
                {
                    ll.Variables.Add("Attribute",attribute_as_json);

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