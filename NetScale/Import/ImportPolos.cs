using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.FRONTEND.WPF.Forms;
using HWB.NETSCALE.FRONTEND.WPF.Import.ArticleAttributes;
using HWB.NETSCALE.FRONTEND.WPF.Import.KindOfGoodsImport;
using HWB.NETSCALE.FRONTEND.WPF.Import.ProductsImport;

namespace HWB.NETSCALE.FRONTEND.WPF.Import
{
  public  class ImportPolos : IImportInterface 
    {
      private string Path = "";
      private string AktFileName;

      // Constructor
      public ImportPolos()
      {

      }

      public void Import()
      {
          Path = GetImportPath();
          if (Path == "")
          {
              MessageBox.Show("Importpfad in den Programmeinstellungen prüfen!",
                              "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);
              return;
          }

       //   new ImportAddress().Import(Path + "\\Polos_Adressen.json");
       //   new ImportKindsOfGoods().Import(Path + "\\Polos_Warenarten.json");
        //  new ImportArticle().Import(Path + "\\Polos_Artikel.json");
      //    new ImportProducts().Import(Path + "\\Polos_Produkte.json");
          new ImportArticleAttributes().Import(Path + "\\Polos_Artikelattribute.json");
      }
    public  void Import(WiegeFrm owf)
    {
        Path = GetImportPath();
        if (Path == "")
        {
            MessageBox.Show("Importpfad in den Programmeinstellungen prüfen!",
                            "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
        }

      //    new ImportAddress().Import(Path + "\\Polos_Adressen.json");
    //    new ImportKindsOfGoods().Import(Path + "\\Polos_Warenarten.json");
        //  new ImportArticle().Import(Path + "\\Polos_Artikel.json");
        // new ImportProducts().Import(Path + "\\Polos_Produkte.json");
        new ImportArticleAttributes().Import(Path + "\\Polos_Artikelattribute.json");
    }


    private string GetImportPath()
    {
        Lokaleeinstellungen oBE = new Lokaleeinstellungen();
        oBE = oBE.Load();
        if (oBE.IMPORT_PATH == null)
        {
            MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Importpfades!",
                            "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);

            return "";
        }
        oBE.Load();
        if (oBE.IMPORT_PATH == "")
        {
            MessageBox.Show("Möglicherweise fehlt in den Programmeinstellungen die Angabe des Importpfades!",
                            "Warnung: Import nicht möglich!", MessageBoxButton.OK, MessageBoxImage.Error);

            return "";
        }

        return oBE.IMPORT_PATH;
    }

  }
 
      

   

    

}
