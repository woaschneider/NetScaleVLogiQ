using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScalePolosIO.Export
{



    #region Auftragsanlage
    

    public class Article
    {
        public int? id { get; set; }
    }

    public class Attributes
    {
    }

    public class ArticleInstance
    {
        public Article article { get; set; }
        public Attributes attributes { get; set; }
    }

    public class SupplierOrConsignee
    {
        public int? id { get; set; }
    }

    public class Service
    {
        public int? id { get; set; }
    }
    public class Unit
    {
        public int id { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
    }
    public class Clearance
    {

        public string validFrom { get; set; }
        public string validTo { get; set; }
        public int? authorizerId { get; set; } // Auftraggeber
        public int? granteeId { get; set; }   // Wem die Freistellung gewährt
 
        public string reference { get; set; }
    }

    public class OrderItemService
    {
        public int identifier { get; set; }
        public string remark { get; set; }
        public ArticleInstance articleInstance { get; set; }
        public string state { get; set; }
        public SupplierOrConsignee supplierOrConsignee { get; set; }
        public string plannedBeginDate { get; set; }
        public string plannedEndDate { get; set; }
        public Service service { get; set; }
        public int targetAmount { get; set; }
        public string deliveryType { get; set; }
    }

    public class Product
    {
        public int? id { get; set; }
    }

    public class OrderItem
    {   public Clearance clearance { get; set; }
        public string orderItemState { get; set; }
        public IList<OrderItemService> orderItemServices { get; set; }
        public string plannedDate { get; set; }
        public Product product { get; set; }
    }

    public class Customer
    {
        public int? id { get; set; }
    }

    public class InvoiceReceiver
    {
        public int? id { get; set; }
    }

    public class RootObject
    {
        public IList<OrderItem> orderItems { get; set; }
        public string reference { get; set; }
        public string orderState { get; set; }
        public Customer customer { get; set; }
        public InvoiceReceiver invoiceReceiver { get; set; }
        public string locationId { get; set; }
        public string date { get; set; }
    }


    #endregion


    #region Neue abgespeckte Struktur
    public class RootObject2
    {
        public ScalePhaseData scalePhaseData { get; set; }
        public string orderItemServiceId { get; set; }
        public string carrierName { get; set; }
        public int? carrierId { get; set; }
        public string carrierVehicle { get; set; }
        public string storageAreaId { get; set; }
        public string scaleNoteNumber { get; set; }
        public double netAmount { get; set; }

    }


    public class FIRST
    {
        public string scaleId { get; set; }
        public string scaleNumber { get; set; }
        public double amount { get; set; }
        public int additionalAmount { get; set; }
        public string date { get; set; }
    }

    public class SECOND
    {
        public string scaleId { get; set; }
        public string scaleNumber { get; set; }
        public double amount { get; set; }
        public int additionalAmount { get; set; }
        public string date { get; set; }
    }
    public class ScalePhaseData
    {
        public FIRST FIRST { get; set; }
        public SECOND SECOND { get; set; }
    }


  

   
#endregion



    // 

    public class RestServerError
    {   
        public string statusCode { get; set; }
        public string message { get; set; }
        public string additionalInformation { get; set; }
    }
}