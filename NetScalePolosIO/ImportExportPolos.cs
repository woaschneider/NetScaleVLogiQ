using System.ComponentModel;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace NetScalePolosIO
{
    public class ImportExportPolos : IImportInterface
    {
        // Constructor

        public void Import()
        {


            string uri = GetImportServerIp() + ":" + GetImportPort();
          
            if (uri == "")
            {
                MessageBox.Show("Keine Import-URI in den Einstellungen");                                           
                return;
            }


            ExceImportThread(uri);
         //   Xceed.Wpf.Toolkit.MessageBox.Show("Import fertig!");
        }

        private void ExceImportThread(string uri)
        {   BackgroundWorker bw = new BackgroundWorker();
         bw.DoWork += BwDoWork;
         
   //     bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
   //     bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
     //   bw.WorkerReportsProgress = true;
             bw.RunWorkerAsync(uri);
         
        }
        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
        Einstellungen boE = new Einstellungen();
        EinstellungenEntity boEe = boE.GetEinstellungen();
            new ImportAddress().Import(e.Argument.ToString(),GetLocationId(),boEe.ImpRESTServerAdressesUrl); // OK
            new ImportKindsOfGoods().Import(e.Argument.ToString(),GetLocationId(),boEe.ImpRESTServerKindofGoodsUrl);// OK
            new ImportArticle().Import(e.Argument.ToString(),GetLocationId(),boEe.ImpRESTServerArticleUrl);// OK
            new ImportProducts().Import(e.Argument.ToString(),GetLocationId(),boEe.ImpRESTServerProductsUrl); // OK
            new ImportArticleAttributes().Import(e.Argument.ToString(),GetLocationId(),boEe.ImpRESTServertArticleAttributesUrl);
            new ImportStorageArea().Import(e.Argument.ToString(),GetLocationId(),boEe.ImpRESTServerStorageAreaUrl);

            new ImportAuftraege().Import(e.Argument.ToString(), GetLocationId(),boEe.ImportRESTServerAuftraegeUrl);
        }


        private string GetImportServerIp()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if(boEe!=null)
            {
                string restIp = boEe.ImpRESTServerIp;
              if (restIp!=null)
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
                string port= boEe.ImpRESTServerPort;
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


        private int  GetLocationId()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            if (boEe != null)
            {
                if (boEe.RestLocation != null)
                {
                    int locationId = (int) boEe.RestLocation;
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


        public void Export(WaegeEntity boWe)
        {
            string exportPath = GetExportPath();
            new ExportWaegung().ExportLs(exportPath,boWe);
            ExportTestToRest(boWe);
            

        }

        public void ExportTestToRest(WaegeEntity boWe )
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

    }
}