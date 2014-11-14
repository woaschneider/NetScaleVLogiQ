using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleAttributes;
using HWB.NETSCALE.POLOSIO.AuftragsImport;
using HWB.NETSCALE.POLOSIO.KindOfGoodsImport;
using HWB.NETSCALE.POLOSIO.LagerPlaetzeImport;
using HWB.NETSCALE.POLOSIO.ProductsImport;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO
{
    public class ImportPolos : IImportInterface
    {
        private string AktFileName;
        private string Path = "";

        // Constructor

        public void Import()
        {


            string uri = GetImportServerIp() + ":" + GetImportPort();
          
            if (uri == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Keine Import-URI in den Einstellungen");                                           
                return;
            }
            else
            {
                
            }

      
       //    new ImportAddress().Import(uri); // OK
      //     new ImportKindsOfGoods().Import(uri);// OK
       //    new ImportArticle().Import(uri);// OK
        //   new ImportProducts().Import(uri); // OK
       //    new ImportArticleAttributes().Import(uri);
       //    new ImportStorageArea().Import(uri);

            new ImportAuftraege().Import(uri);
            Xceed.Wpf.Toolkit.MessageBox.Show("Import fertig!");
        }

      

        private string GetImportServerIp()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEE = boE.GetEinstellungen();
            if(boEE!=null)
            {
                string RestIp = boEE.RestServerAdresse;
              if (RestIp!=null)
              {
                  return RestIp;
              }
              else
              {
                  return "";
              }
            }
            else
            {
                return "";
            }

           

        }
        private string GetImportPort()
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEE = boE.GetEinstellungen();
            if (boEE != null)
            {
                string port= boEE.RestServerPort;
                if (port != null)
                {
                    return port;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }



        }

        private string GetImportPath()
        {
            var oBE = new Lokaleeinstellungen();
            oBE = oBE.Load();
            if (oBE.IMPORT_PATH == null)
            {
                MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Importpfades!",
                                "Warnung: Import nicht möglich!");

                return "";
            }
            oBE.Load();
            if (oBE.IMPORT_PATH == "")
            {
                MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Importpfades!",
                                "Warnung: Import nicht möglich!");

                return "";
            }

            return oBE.IMPORT_PATH;
        }
    }
}