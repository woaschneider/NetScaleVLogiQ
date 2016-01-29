﻿using System.ComponentModel;
using System.Threading;
using System.Windows;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO;
using NetScalePolosIO.Export;
using NetScalePolosIO.Import.AddressImport;
using NetScalePolosIO.Import.ArticleAttributesImport;
using NetScalePolosIO.Import.ArticleImport;
using NetScalePolosIO.Import.AuftragsImport;
using NetScalePolosIO.Import.KindOfGoodsImport;
using NetScalePolosIO.Import.LagerPlaetzeImport;
using NetScalePolosIO.Import.ProductsImport;
using NetScalePolosIO.Logging;
using OakLeaf.MM.Main.Collections;



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

        private string GetLocationId()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                if (boEe.RestLocation != null)
                {
                    string locationId =  boEe.RestLocation;
                    return locationId;
                }
            }
            return "";
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
            bw.RunWorkerAsync(uri);
        }

        private void BwDoWorkImport(object sender, DoWorkEventArgs e)
        {
            Log.Instance.Info("Stammdatenimport wurde gestartet!");
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            // Adressen
           new ImportAddress().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerAdressesUrl);
            // Warenarten
           new ImportKindsOfGoods().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerKindofGoodsUrl);
            // Artikel
            new ImportArticle().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerArticleUrl);
            // Produkte
            new ImportProducts().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerProductsUrl);
            // Artikelattribute
            new ImportArticleAttributes().Import(e.Argument.ToString(), GetLocationId()  ,   boEe.ImpRESTServertArticleAttributesUrl);
            // Lagerplätze
            new ImportStorageArea().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerStorageAreaUrl);
            Log.Instance.Info("Stammdatenimport wurde beendet!");
        }

        #endregion

        //*****************************************************************

        #region Import Auftrage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="only_Ready_To_Dispatch"></param>
        public void ImportAuftraege(bool only_Ready_To_Dispatch)
        {
            string uri = GetImportServerIp() + ":" + GetImportPort();

            if (uri == "")
            {
                MessageBox.Show("Keine Import-URI in den Einstellungen");
                return;
            }

            ExceImportAuftraegeThread(uri, only_Ready_To_Dispatch);
        }

        private void ExceImportAuftraegeThread(string uri, bool only_Ready_To_Dispatch)
        {
            BackgroundWorker bw = new BackgroundWorker();
            //  bw.DoWork += BwDoWorkImportAuftraege;
            bw.DoWork += (obj, e) => BwDoWorkImportAuftraege(uri, only_Ready_To_Dispatch);

            bw.RunWorkerAsync();
            // bw.RunWorkerAsync(uri, only_Ready_To_Dispatch);
        }

        private void BwDoWorkImportAuftraege(string uri, bool only_Ready_To_Dispatch)
        {
            string info = "";

            if (only_Ready_To_Dispatch)
            {
                info = "Nur Ready For Dispatch";
            }
            else
            {
                info = "Alle Aufträge";
            }
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            Log.Instance.Info("Auftragsimport wurde gestartet! " + info);
            new ImportAuftraege().Import(uri, GetLocationId(), boEe.ImportRESTServerAuftraegeUrl, only_Ready_To_Dispatch);
            Log.Instance.Info("Auftragsimport wurde beendet! " + info);
        }
        
        #endregion

        //*****************************************************************

        #region Export

        #region Alles Exportieren // Asynchron

        // Alle noch offenen Wägungen werden exportiert
        public void ExportAll()
        {
            ExceExportThread();
        }

        private void ExceExportThread()
        {
           
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BwDoWorkExport;

         
            //     bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            //     bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            //   bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
     
        }

        private void BwDoWorkExport(object sender, DoWorkEventArgs e)
        {
            if (goApp.ExportIsRunning == false)
            {
                goApp.ExportIsRunning = true;
                Waege boW = new Waege();
                mmBindingList<WaegeEntity> ol = boW.PendingListToPolos();
                foreach (var w in ol)
                {
                    ExportToRestServer(w);
                }
                goApp.ExportIsRunning = false;
            }
        }

        #endregion

        #region Einzel-Export // Wird nach einer Zweitwägung aufgerufen (sychron)

        public void ExportSingle(WaegeEntity boWe)
        {
            if (boWe.taab == false)
            {
                ExportToRestServer(boWe);
            }
            
        }

        #endregion

        // Wird von beiden Mehtoden (single / all benutzt)
        private void ExportToRestServer(WaegeEntity boWe)
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

            oEx.ExportLs2Rest(baseUrl, boEe.RestLocation, boWe);
        }

        #endregion

        //*****************************************************************  
    }
}