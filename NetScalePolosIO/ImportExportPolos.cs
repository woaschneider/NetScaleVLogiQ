using System;
using System.ComponentModel;
using System.Linq;
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
using NetScalePolosIO.Import.PlanningDivisionImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            goApp.ImportMessageStammdaten = "Start Stammdatenimport!";
            Log.Instance.Info("Stammdatenimport wurde gestartet!");
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            // Adressen
            goApp.ImportMessageStammdaten = "Adressen";
            goApp.ProzentStammdaten = 1;
            new ImportAddress().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerAdressesUrl);
            
            // Warenarten
            goApp.ProzentStammdaten = 1;
            goApp.ImportMessageStammdaten = "Warenarten";
           new ImportKindsOfGoods().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerKindofGoodsUrl);

            // Artikel
            goApp.ProzentStammdaten = 01;
           goApp.ImportMessageStammdaten = "Artikel";
            new ImportArticle().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerArticleUrl);
            
            // Produkte
            goApp.ProzentStammdaten = 1;
            goApp.ImportMessageStammdaten = "Produkte";
            new ImportProducts().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerProductsUrl);
            
            // Artikelattribute
            goApp.ProzentStammdaten = 1;
            goApp.ImportMessageStammdaten = "Artikelattribute";
            new ImportArticleAttributes().Import(e.Argument.ToString(), GetLocationId()  ,   boEe.ImpRESTServertArticleAttributesUrl);

            // 25.01.2017
            // Nach dem die Stammdaten Artikel und Artikelattribute(alle möglichen) eingelesen worden sind, werden die  
            // Attribute des jeweiligen Artikel in eine eigen Tabelle geschrieben. Allerding stellt sich die Frage, ob wir
            // ArticleAttribute als eigenständige Stammdaten noch brauchen. Sie kommen ja eh mit den Artikel...
             Artikelattribute2TableAttribute();

            // Lagerplätze
            goApp.ProzentStammdaten = 1;
            goApp.ImportMessageStammdaten = "Lagerplätze";
            new ImportStorageArea().Import(e.Argument.ToString(), GetLocationId(), boEe.ImpRESTServerStorageAreaUrl);
           
            //  PlanningDevision
            goApp.ProzentStammdaten = 1;
            goApp.ImportMessageStammdaten = "Dispobereiche";
            new ImportPlanningDivison().Import(e.Argument.ToString(), GetLocationId(),
            boEe.ImpRESTServerPlanningDivision);


            goApp.ImportMessageStammdaten = "";
            goApp.ProzentStammdaten = 0;
            Log.Instance.Info("Stammdatenimport wurde beendet!");
          
        }

        #endregion

        //*****************************************************************

        #region Import Auftrage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onlyReadyToDispatch"></param>
        public void ImportAuftraege(bool onlyReadyToDispatch)
        {
            
            string uri = GetImportServerIp() + ":" + GetImportPort();

            if (uri == "")
            {
                MessageBox.Show("Keine Import-URI in den Einstellungen");
                return;
            }

            ExceImportAuftraegeThread(uri, onlyReadyToDispatch);
        }

        private void ExceImportAuftraegeThread(string uri, bool onlyReadyToDispatch)
        {
            BackgroundWorker bw = new BackgroundWorker();
           
            bw.DoWork += (obj, e) => BwDoWorkImportAuftraege(uri, onlyReadyToDispatch);

            bw.RunWorkerAsync();
            
        }

        private void BwDoWorkImportAuftraege(string uri, bool onlyReadyToDispatch)
        {
            goApp.ImportMessageAuftraege = "Auftragsimport!";

            var info = onlyReadyToDispatch ? "Nur Ready For Dispatch" : "Alle Aufträge";
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            Log.Instance.Info("Auftragsimport wurde gestartet! " + info);
            new ImportAuftraege().Import(uri, GetLocationId(), boEe.ImportRESTServerAuftraegeUrl, onlyReadyToDispatch);
            Log.Instance.Info("Auftragsimport wurde beendet! " + info);
            goApp.ImportMessageAuftraege = "";
            goApp.ProzentAuftraege = 0;
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

        // House keeping
        // Da die Attribute durch den Superuser als Pflichtfelder definiert werden sollen
        // packen wir diese Daten in eine eigene Tabelle. Diese enthält eigentlich nur das 
        // zusätzliche Required-Feld. Der Rest sind FK's
        private void Artikelattribute2TableAttribute()
        {
            // Alle Artikel-Entitäten parsen
            Artikel boArtikel = new Artikel();
            Attribut boAttribut = new Attribut();
            Artikelattribute boAa = new Artikelattribute();
            mmBindingList<ArtikelEntity> list = boArtikel.GetAll();
            foreach (var a in list)
            {
                // Gibt es das Attribut in der Stammdatentabelle Artikelattribute
                // Wenn ja, prüfe ob die Tabelle Attibute für die Artikel und dieses Attribut 
                // bereits einen Eintrag besitzt. Wenn ja, dann passiert nix. Wenn nicht wird ein
                // neuer angelegt
                var json = a.attributes_as_json;
                var array = (JArray)JsonConvert.DeserializeObject(json);
                var attributList = array.ToList<object>();
                foreach (var t in attributList)
                {
                  
                    ArtikelattributeEntity boAaE = boAa.GetArtikelAttributByBezeichnung(t.ToString());
                    if (boAaE != null)
                    {
                        boAttribut = new Attribut();
                        bool uRet = boAttribut.IsArtikelAttribut(a.PK, boAaE.PK);
                        if (uRet == false)
                        {
                            try
                            {
                               
                               var boAttributEntity = boAttribut.NewEntity();
                                boAttributEntity.Required = false;
                                boAttributEntity.ArtikelFK = a.PK;
                                boAttributEntity.AttributeFK = boAaE.PK;
                                
                                boAttribut.SaveEntity(boAttributEntity);
                                
                            }
                            catch (Exception e)
                            {
                              Log.Instance.Error(e.ToString());
                                Log.Instance.Error(e.InnerException);
                            }
                          
                        }

                    }
                }
            }
        }
    }
}