using System;
using System.ComponentModel;
using System.IO;
using System.Net;


namespace HWB.NETSCALE.BOEF
{
    public class Waageneinstellungen
    {
        private const string XML_FILE_NAME = "WaageneinstellungenObject.xml";

        // Anzahl der Waagen
        public string SCALES; // = Anzahl Auswertegeräte
        public string MESSKREISE;
        public string Einheit;


        public string W1_WAAGENID;
        public string W1_WAAGENBESCHREIBUNG;
        public string W1_WAAGENNAME;
        public string W1_IP_NUMMER;
        public string W1_COM;
        public string W1_BAUD;
        public string W1_DATA_BIT;
        public string W1_PARITY_BIT;
        public string W1_STOP_BIT;
        public string W1_e;
        public string W1_min;
        public string W1_max;


        public string W2_WAAGENID;
        public string W2_WAAGENBESCHREIBUNG;
        public string W2_WAAGENNAME;
        public string W2_IP_NUMMER;
        public string W2_COM;
        public string W2_BAUD;
        public string W2_DATA_BIT;
        public string W2_PARITY_BIT;
        public string W2_STOP_BIT;
        public string W2_e;
        public string W2_min;
        public string W2_max;


        public Waageneinstellungen()
        {
        }

        public Waageneinstellungen Load()
        {
            Waageneinstellungen oWaagenEinstell = ObjectXMLSerializer<Waageneinstellungen>.Load(XML_FILE_NAME);
            return oWaagenEinstell;
        }

        public void Save(Waageneinstellungen oWE)
        {
            ObjectXMLSerializer<Waageneinstellungen>.Save(oWE, XML_FILE_NAME);
        }
    }
}