using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OakLeaf.MM.Main.Collections;

namespace HWB.NETSCALE.BOEF.JoinClasses
{
    public class AuftragsListe : ABusinessEntity
    {
        public int kkpk { get; set; }
        public int appk { get; set; }
        public string nr { get; set; }

        public string mandant { get; set; }
        public string werksnr { get; set; }

        public string Firma { get; set; }
        public string Name1 { get; set; }
        public string Ort { get; set; }
        public string Plz { get; set; }
        public string Anschrift { get; set; }

        public string auftragsart { get; set; }
        public string kontraktart { get; set; }
        public string KontraktNr { get; set; }

        public string wefirma { get; set; }
        public string wename1 { get; set; }
    }

    
}