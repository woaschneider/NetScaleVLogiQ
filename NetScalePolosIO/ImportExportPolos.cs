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
using PropertyChanged;


namespace NetScalePolosIO
{
   // [AddINotifyPropertyChangedInterfaceAttribute]
    public class ImportExportPolos : INotifyPropertyChanged, IDisposable
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        // Properties
        public  bool ExportIsRunning { get; set; }
        public  bool ImportStammdatenIsRunning { get; set; }
        public bool ImportAuftrageIsRunning { get; set; }
        public  string ImportMessageStammdaten { get; set; }
        public  int ProzentStammdaten { get; set; }

        public  string ImportMessageAuftraege { get; set; }
        public  int ProzentAuftraege { get; set; }

        public string ExportMessageWaegungen { get; set; }
        public int ProzentWaegung { get; set; }

        public EventHandler IOStatusHasChanged;

        //
        private string ImportServerIp;

        private string ImportPort;
        private string ExportServerIp;
        private string ExportPort;
        private string LocationId;

        public ImportExportPolos()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
            ImportServerIp =  boEe.ImpRESTServerIp;
            ImportPort = boEe.ImpRESTServerPort;
            ExportServerIp = boEe.ExportRESTIp;
            ExportPort = boEe.ExportRESTServerport;
            LocationId = boEe.RestLocation;
    }
        }
        //*****************************************************************

        #region Einstellung

        private string xGetImportServerIp()
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

        private string xGetImportPort()
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

        private string xGetExportServerIp()
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

        private string xGetExportPort()
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

        private string xGetLocationId()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                if (boEe.RestLocation != null)
                {
                    string locationId = boEe.RestLocation;
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
            string uri = ImportServerIp + ":" + ImportPort;

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
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            bw.DoWork += BwDoWorkImport;

            bw.RunWorkerAsync(uri);
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          

            BackgroundWorker worker = sender as BackgroundWorker;
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork -= new DoWorkEventHandler(BwDoWorkImport);
            worker.Dispose();
            ImportMessageStammdaten = "";
            ProzentStammdaten = 0;
            ImportStammdatenIsRunning = false;
            Log.Instance.Info("Stammdatenimport wurde beendet!");
            IOStatusHasChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BwDoWorkImport(object sender, DoWorkEventArgs e)
        {
            ImportMessageStammdaten = "Start Stammdatenimport!";
            ImportStammdatenIsRunning = true;
            IOStatusHasChanged?.Invoke(this, EventArgs.Empty);
            Log.Instance.Info("Stammdatenimport wurde gestartet!");
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            // Adressen
            ImportMessageStammdaten = "Adressen";
            ProzentStammdaten = 1;
            new ImportAddress(this).Import(e.Argument.ToString(), LocationId, boEe.ImpRESTServerAdressesUrl);

            // Warenarten
            ProzentStammdaten = 1;
            ImportMessageStammdaten = "Warenarten";
            new ImportKindsOfGoods(this).Import(e.Argument.ToString(), LocationId, boEe.ImpRESTServerKindofGoodsUrl);

            // Artikel
            ProzentStammdaten = 01;
            ImportMessageStammdaten = "Artikel";
            new ImportArticle(this).Import(e.Argument.ToString(), LocationId, boEe.ImpRESTServerArticleUrl);

            // Produkte
            ProzentStammdaten = 1;
            ImportMessageStammdaten = "Produkte";
            new ImportProducts(this).Import(e.Argument.ToString(), LocationId, boEe.ImpRESTServerProductsUrl);

            // Artikelattribute
            ProzentStammdaten = 1;
            ImportMessageStammdaten = "Artikelattribute";
            new ImportArticleAttributes(this).Import(e.Argument.ToString(), LocationId,
                boEe.ImpRESTServertArticleAttributesUrl);

            // 25.01.2017
            // Nach dem die Stammdaten Artikel und Artikelattribute(alle möglichen) eingelesen worden sind, werden die  
            // Attribute des jeweiligen Artikel in eine eigen Tabelle geschrieben. Allerding stellt sich die Frage, ob wir
            // ArticleAttribute als eigenständige Stammdaten noch brauchen. Sie kommen ja eh mit den Artikel...
            Artikelattribute2TableAttribute();

            // Lagerplätze
            ProzentStammdaten = 1;
            ImportMessageStammdaten = "Lagerplätze";
            new ImportStorageArea(this).Import(e.Argument.ToString(), LocationId, boEe.ImpRESTServerStorageAreaUrl);

            //  PlanningDevision
            ProzentStammdaten = 1;
            ImportMessageStammdaten = "Dispobereiche";
            new ImportPlanningDivison().Import(e.Argument.ToString(), LocationId,
                boEe.ImpRESTServerPlanningDivision);


           
           
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
            string uri = ImportServerIp + ":" + ImportPort;

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
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwDoWorkImportAuftraegeCompleted);
            bw.DoWork += BwDoWorkImportAuftraege;

            bw.RunWorkerAsync(uri);
        }

        void BwDoWorkImportAuftraegeCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
       
          
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(BwDoWorkImportAuftraegeCompleted);
            worker.DoWork -= new DoWorkEventHandler(BwDoWorkImport);
            worker.Dispose();
            Log.Instance.Info("Auftragsimport wurde beendet! ");
            ImportMessageAuftraege = "";
            ImportAuftrageIsRunning = false;
            ProzentAuftraege = 0;
            IOStatusHasChanged?.Invoke(this, EventArgs.Empty);
        }


        private void BwDoWorkImportAuftraege(object sender, DoWorkEventArgs e)
        {
            ImportMessageAuftraege = "Auftragsimport!";
            ImportAuftrageIsRunning = true;
            IOStatusHasChanged?.Invoke(this, EventArgs.Empty);
         //   var info = onlyReadyToDispatch ? "Nur Ready For Dispatch" : "Alle Aufträge";
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            Log.Instance.Info("Auftragsimport wurde gestartet! " );
            new ImportAuftraege(this).Import(e.Argument.ToString(), LocationId, boEe.ImportRESTServerAuftraegeUrl, false);
            Log.Instance.Info("Auftragsimport wurde beendet! " );
          
          
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
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwDoWorkExportCompleted);


            bw.RunWorkerAsync();
        }

        void BwDoWorkExportCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(BwDoWorkExportCompleted);
            worker.DoWork -= new DoWorkEventHandler(BwDoWorkExport);
            worker.Dispose();

            ExportMessageWaegungen = "";
            ExportIsRunning = false;
            IOStatusHasChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BwDoWorkExport(object sender, DoWorkEventArgs e)
        {
            if (ExportIsRunning == false)
            {
                try
                {
                    

                    ExportIsRunning = true;
                  
                    ExportMessageWaegungen = "Datenexport läuft...";
                    IOStatusHasChanged?.Invoke(this, EventArgs.Empty);

                    Waege boW = new Waege();

                    mmBindingList<WaegeEntity> ol = boW.PendingListToPolos();
                    foreach (var w in ol)
                    {
                        ExportToRestServer(w);
                    }
                  
                }
                catch (Exception exception)
                {
                    ExportMessageWaegungen = "";
                    ExportIsRunning = false;
                    IOStatusHasChanged?.Invoke(this, EventArgs.Empty);
                }
             
               
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

        // Wird von beiden Methoden (single / all benutzt)
        private void ExportToRestServer(WaegeEntity boWe)
        {
        
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            string baseUrl = ExportServerIp + ":" + ExportPort;

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
                var array = (JArray) JsonConvert.DeserializeObject(json);
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
                                boAttributEntity.AttributName = boAaE.AttributName;

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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}