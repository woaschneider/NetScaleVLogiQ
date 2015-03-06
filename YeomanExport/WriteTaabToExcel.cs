using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main.Collections;
using HWB.NETSCALE.BOEF;

namespace YeomanExport
{
    public class WriteTaabToExcel
    {
        private string exportpfad = "";
        private string ExcelFileName = "";

        public int Export2Xls(mmBindingList<WaegeEntity> oExport)
        {
            int uRet = 0;
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEE = boE.GetEinstellungen();
            if (boEE == null)
            {
                return 0;
            }

            try
            {
                Lokaleeinstellungen oL = new Lokaleeinstellungen().Load();

                exportpfad = oL.EXPORT_PATH + "\\";

                ExcelFileName = boEE.RestLocation.ToString(); // !!! Da verbirgt sich der Standort der Waaege hinter
                ExcelFileName = ExcelFileName + "_" + DateTime.Today.ToString().Substring(0, 10) + ".xls";


                FileStream stream = new FileStream(exportpfad + ExcelFileName, FileMode.OpenOrCreate);
                ExcelWriter writer = new ExcelWriter(stream);
                writer.BeginWrite();
                // Header
                writer.WriteCell(0, 0, "LS-Nr");
                writer.WriteCell(0, 1, "LS-Datum");
                writer.WriteCell(0, 2, "Material-Nr.");
                writer.WriteCell(0, 3, "Mat. Bezeichnung");
                writer.WriteCell(0, 4, "Kunden Nr.");
                writer.WriteCell(0, 5, "Kunden Name");
                writer.WriteCell(0, 6, "Empfänger");
                writer.WriteCell(0, 7, "Empfänger Name");
                writer.WriteCell(0, 8, "Baustelle");
                writer.WriteCell(0, 9, "Kfz-Kennzeichen");
                writer.WriteCell(0, 10, "Incoterm");
                writer.WriteCell(0, 11, "Spedition Nr.");
                writer.WriteCell(0, 12, "Spedition Name");
                writer.WriteCell(0, 13, "Menge");
                writer.WriteCell(0, 14, "Mengeneinheit");
                // Header Ende

                int lc = 1; // LineCounter
                foreach (var w in oExport)
                {
                    writer.WriteCell(lc, 0, w.LieferscheinNr ?? "");
                    writer.WriteCell(lc, 1, w.LSDatum.ToString() ?? "");
                    writer.WriteCell(lc, 2, w.articleId.ToString() ?? "");
                    writer.WriteCell(lc, 3, w.articleDescription ?? "");
                    writer.WriteCell(lc, 4, w.customerBusinessIdentifier ?? "");
                    writer.WriteCell(lc, 5, w.customerName ?? "");

                    writer.WriteCell(lc, 6, w.supplierOrConsigneeBusinessIdentifier ?? "");
                    writer.WriteCell(lc, 7, w.supplierOrConsigneeName ?? "");
                    writer.WriteCell(lc, 8, w.freitext1 ?? "");
                    writer.WriteCell(lc, 9, w.Fahrzeug ?? "");
                    //   writer.WriteCell(lc, 9, w.deliveryType ?? "");
                    writer.WriteCell(lc, 10, w.incoterm ?? "");

                    writer.WriteCell(lc, 11, w.ffBusinessIdentifier ?? "");
                    writer.WriteCell(lc, 12, w.ffName ?? "");
                    writer.WriteCell(lc, 13, w.Nettogewicht.ToString() ?? "");
                    writer.WriteCell(lc, 14, "t" ?? ""); // TODO: Aus den Daten entnehmen
                    uRet = lc;
                    lc = lc + 1;
                }

                writer.EndWrite();
                stream.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show((ee.Message));
            }


            return uRet;
        }
    }
}