using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.AuftragsImport;
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
            Path = GetImportPath();
            if (Path == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Keine Importpfadangabe");
               
                
                //MessageBox.Show("Importpfad in den Programmeinstellungen prüfen!",
                //                "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);)
                return;
            }

            //   new ImportAddress().Import(Path + "\\Polos_Adressen.json");
            //   new ImportKindsOfGoods().Import(Path + "\\Polos_Warenarten.json");
          //  new ImportArticle().Import(Path + "\\Polos_Artikel.json");
              new ImportProducts().Import(Path + "\\Polos_Produkte.json");
            //   new ImportArticleAttributes().Import(Path + "\\Polos_Artikelattribute.json");
            //  new ImportStorageArea().Import(Path + "\\Polos_Lagerplätze.json");

            new ImportAuftraege().Import(Path + "\\Auftrag_587_WA_LKW2.json");
            Xceed.Wpf.Toolkit.MessageBox.Show("Import fertig!");
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