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
        public string Sortennr { get; set; }
        public string Sortenbezeichnung1 { get; set; }
        public string Sortenbezeichnung2 { get; set; }
    }

    public class Auftragsdetail : ABusinessEntity
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
        public string Sortennr { get; set; }
        public string Sortenbezeichnung1 { get; set; }
        public string Sortenbezeichnung2 { get; set; }
    }

    public class OrderParentAndChild : ABusinessEntity
    {
        // OrderItem
        public string customerBusinessIdentifier { get; set; }
        public string invoiceReceiverBusinessIdentifier { get; set; }

        // OrderItemService
       
        public string productdescription { get; set; }
        public string ownerBusinessIdentifier { get; set; }
        public string remark { get; set; }
        public string supplierOrConsigneeBusinessIdentifiert { get; set; }
        public string deliveryType { get; set; }
        public string kindOfGoodDescription { get; set; }
        public int? product { get; set; }
        public int? articleId  { get; set; }
        public string articleDescription { get; set; }
 
        public DateTime? plannedDate { get; set; }
    }
}