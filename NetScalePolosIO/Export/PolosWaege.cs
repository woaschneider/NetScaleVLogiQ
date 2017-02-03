using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NetScalePolosIO.Export
{
    public class Article
    {
        public string id { get; set; }
    }

    public class ArticleAttribute
    {
        public string SERIAL_NUMBER { get; set; }
        public string BARCODE { get; set; }
        public string BATCH { get; set; }
        public string ORIGN { get; set; }
        public string GRADE { get; set; }
        public string ORIGINAL_NUMBER { get; set; }
        public string ORIGINAL_MARKING { get; set; }
        public string LENGTH { get; set; }
        public string WIDTH { get; set; }
        public string HEIGHT { get; set; }
        public string DIMENSION { get; set; }
        public string STORAGE_AREA_REFERENCE { get; set; }
        public string STORAGE_AREA_REFERENCE_NUMBER { get; set; }
        public string DIAMETER { get; set; }
    }

    public class ArticleInstance
    {
        public Article article { get; set; }

        public ArticleAttribute attributes { get; set; }
    }

    public class Clearance
    {
        public string validFrom { get; set; }
        public string validTo { get; set; }
        public string authorizerId { get; set; } // Auftraggeber
        public string granteeId { get; set; } // Wem die Freistellung gewährt
        public string active { get; set; }
        public string reference { get; set; }
    }

    #region Neue abgespeckte Struktur

    public class RootObject2
    {
        public ScalePhaseData scalePhaseData { get; set; }
        public string orderItemServiceId { get; set; }
        public string carrierBusinessIdentifier { get; set; }
        public string customerBusinessIdentifier { get; set; }


        public string carrierVehicle { get; set; }
        public string storageAreaId { get; set; }
        public string scaleNoteNumber { get; set; }

        public ArticleInstance articleInstance { get; set; }


        public decimal? netAmount { get; set; }
        public decimal? additionalAmount { get; set; }

        public string freightCarrierFreeText { get; set; }
        public string recipientFreeText { get; set; }
        public string releaseFreeText { get; set; }
    }


    public class FIRST
    {
        public string scaleId { get; set; }
        public string scaleNumber { get; set; }

        public decimal? amount { get; set; }

        public string date { get; set; }
    }

    public class SECOND
    {
        public string scaleId { get; set; }
        public string scaleNumber { get; set; }

        public decimal? amount { get; set; }

        public string date { get; set; }
    }

    public class ScalePhaseData
    {
        public FIRST FIRST { get; set; }
        public SECOND SECOND { get; set; }
    }

    #endregion

    public class RestServerError
    {
        public string statusCode { get; set; }
        public string message { get; set; }
        public string additionalInformation { get; set; }
    }
}