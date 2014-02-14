using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWB.NETSCALE.BOEF.JoinClasses
{
    public class Auftragsdetailliste : ABusinessEntity
    {
        public int kkpk { get; set; }
        public int appk { get; set; }
        public string nrKU { get; set; }

        public string mandant { get; set; }
        public string werksnr { get; set; }

        public string FirmaKU { get; set; }
        public string Name1KU { get; set; }
        public string OrtKU { get; set; }
        public string PlzKU { get; set; }
        public string AnschriftKU { get; set; }

        public string auftragsart { get; set; }
        public string kontraktart { get; set; }
        public string KontraktNr { get; set; }

        public string wefirma { get; set; }
        public string wename1 { get; set; }
        // KM

        public int kmpk { get; set; }
        public string posnr { get; set; }
        // MG
        public string Sortennr { get; set;}
        public string Sortenbezeichnung1 { get; set; }
        public string Sortenbezeichnung2 { get; set; } 


    }
}
