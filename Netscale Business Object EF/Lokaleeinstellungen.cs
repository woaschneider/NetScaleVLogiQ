using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWB.NETSCALE.BOEF
{
    public class Lokaleeinstellungen
    {
        private const string XML_FILE_NAME = "LokaleeinstellungenObject.xml";


        public string IMPORT_PATH;
        public string EXPORT_PATH;
        public string PDF_PATH;
        public string MISCHEREXPORT_PATH;
        public string WIEGELISTFILE;
        public string WIEGELIST_KU_FILE;
        public string WIEGELIST_KFZ_FILE;

        public string LISTENDRUCKER;

        public string VALDIERUNG_ERST;

        public string MEHRWERTSTEUER;

        public string AUTOKFZ;
        public string AUTOABRUF;

        public string SAVE_ABR2CF;
        public string TAKE_LAST_ABR;
        public string ONNEW_SETFOCUS_TO_KFZ_KENNZEICHEN;
        public string SAVE_ERST2CFTARA;
        public string ZWEIT_OHNE_ERST;
        public string MAXGEWICHT_VAL;
        public string FIRMAKU_VAL;

        public string PDFEXPORT;
        public string MISCHEREXPORT;


        public string LI_WAAGE;
        public string LI_FUNK;
        public string LI_KARTEN;
        public string LI_FERNANZEIGE;

        public string KARTENLESERCOMPORT;
        public string FUNKMODULCOMPORT;
        public string FERNANZEIGECOMPORT;
        public string ERSTWAEGUNGSWAAGE;
        public string ZWEITWAEGUNGSWAAGE;
        public string WaageAn;


        public Lokaleeinstellungen()
        {
        }

        public Lokaleeinstellungen Load()
        {
            Lokaleeinstellungen oLocalEinstellungen = ObjectXMLSerializer<Lokaleeinstellungen>.Load(XML_FILE_NAME);
            return oLocalEinstellungen;
        }

        public void Save(Lokaleeinstellungen oLocalEinstellungen)
        {
            ObjectXMLSerializer<Lokaleeinstellungen>.Save(oLocalEinstellungen, XML_FILE_NAME);
        }
    }
}