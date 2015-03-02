using System;
using System.ComponentModel;
using System.Net.Mail;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO;
using HWB.NETSCALE.POLOSIO.ArticleAttributes;
using HWB.NETSCALE.POLOSIO.AuftragsImport;
using HWB.NETSCALE.POLOSIO.KindOfGoodsImport;
using HWB.NETSCALE.POLOSIO.LagerPlaetzeImport;
using HWB.NETSCALE.POLOSIO.ProductsImport;
using NetScalePolosIO.Export;
using NetScalePolosIO.Import.AddressImport;
using NetScalePolosIO.Import.ArticleAttributesImport;
using NetScalePolosIO.Import.ArticleImport;
using NetScalePolosIO.Import.AuftragsImport;
using NetScalePolosIO.Import.KindOfGoodsImport;
using NetScalePolosIO.Import.LagerPlaetzeImport;
using NetScalePolosIO.Import.ProductsImport;
using OakLeaf.MM.Main.Collections;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace NetScalePolosIO
{
    public class ImportExportPolos : IImportInterface
    {
        //*****************************************************************
        #region Einstellung
        private string GetImportServerIp()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                string restIp = boEe.ImpRESTServerIp;
                if (restIp != null)
                {
                    return restIp;
                }
                return "";
            }
            return "";
        }

        private string GetImportPort()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                string port = boEe.ImpRESTServerPort;
                if (port != null)
                {
                    return port;
                }
                return "";
            }
            return "";
        }

        private string GetExportServerIp()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                string restIp = boEe.ExportRESTIp;
                if (restIp != null)
                {
                    return restIp;
                }
                return "";
            }
            return "";
        }

        private string GetExportPort()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                string port = boEe.ExportRESTServerport;
                if (port != null)
                {
                    return port;
                }
                return "";
            }
            return "";
        }

        private int GetLocationId()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                if (boEe.RestLocation != null)
                {
                    int locationId = (int)boEe.RestLocation;
                    return locationId;
                }
            }
            return 0;
        }

        private string GetExportPath()
        {
            var oBe = new Lokaleeinstellungen();
            oBe = oBe.Load();
            if (oBe.EXPORT_PATH == null)
            {
                MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Exportpfades!",
                    "Warnung: Import nicht möglich!");

                return "";
            }
            oBe.Load();
            if (oBe.EXPORT_PATH == "")
            {
                MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Exportpfades!",
                    "Warnung: Import nicht möglich!");

                return "";
            }
            return oBe.EXPORT_PATH;
        }
        #endregion
        //*****************************************************************
        #region Import Stammdaten

        public void ImportStammdaten()
        {
            string uri = GetImportServerIp() + ":" + GetImportPort();

            if (uri == "")
            {
                MessageBox.Show("Keine Import-URI in den Einstellungen");
                return;
            }


            ExceImportStammdatenThread(uri);
           
        }

        private void ExceImportStammdatenThread(string uri)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BwDoWorkImport;

            //     bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            //     bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            //   bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync(uri);
        }

        private void BwDoWorkImport(object sender, DoWorkEventArgs e)
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            new ImportAddress().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerAdressesUrl); // OK
            new ImportKindsOfGoods().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerKindofGoodsUrl);
            // OK
            new ImportArticle().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerArticleUrl); // OK
            new ImportProducts().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerProductsUrl); // OK
            new ImportArticleAttributes().Import(e.Argument.ToString(), GetLocationId(),
                boEe.ImpRESTServertArticleAttributesUrl);
            new ImportStorageArea().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerStorageAreaUrl);

       //     new ImportAuftraege().Import(e.Argument.ToString(), GetLocationId(), boEe.ImportRESTServerAuftraegeUrl);
        }
        #endregion
        //*****************************************************************
        #region Import Auftrage

        public void ImportAuftraege()
        {
            string uri = GetImportServerIp() + ":" + GetImportPort();

            if (uri == "")
            {
                MessageBox.Show("Keine Import-URI in den Einstellungen");
                return;
            }


            ExceImportAuftraegeThread(uri);

        }

        private void ExceImportAuftraegeThread(string uri)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BwDoWorkImportAuftraege;

            //     bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            //     bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            //   bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync(uri);
        }

        private void BwDoWorkImportAuftraege(object sender, DoWorkEventArgs e)
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
        new ImportAuftraege().Import(e.Argument.ToString(), GetLocationId(), boEe.ImportRESTServerAuftraegeUrl);
        }
        #endregion
        //*****************************************************************
        #region Export
        public void ExportAll()
        {
           
                ExceExportThread();
           


            
        }

        // Export wir entweder aus der GUI angestoßen (synchron) oder im Rahmen des ExceExportThread innerhalb des Background Worker
        public void ExportSingle(WaegeEntity boWe)
        {
            ExportToRESTServer(boWe);
        }

        private void ExceExportThread()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork +=BwDoWorkExport;

            //     bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            //     bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            //   bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }

        private void BwDoWorkExport(object sender, DoWorkEventArgs e)
        {
             Waege boW = new Waege();
            mmBindingList<WaegeEntity> ol = boW.PendingListToPolos();
            foreach (var w in ol)
            {


                ExportToRESTServer(w);
            }
        }

        private void ExportToRESTServer(WaegeEntity boWe)
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            string baseUrl = GetExportServerIp() + ":" + GetExportPort();

            if (baseUrl == "")
            {
                MessageBox.Show("Keine Import-URI in den Einstellungen");
                return;
            }
            var oEx = new ExportWaegungVersion2Rest();
            int? l = ((boEe.RestLocation != null) ? boEe.RestLocation : 0);
            oEx.ExportLs2Rest(baseUrl, boEe.ExportRESTServerUrl, l, boWe);
        }

     
     
          


        #endregion
        //*****************************************************************
      

    
    }
}