using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScalePolosIO.Export
{
    //public class BaseUnit
    //{
    //    public int id { get; set; }
    //    public string shortDescription { get; set; }
    //    public string description { get; set; }
    //}

    //public class Article
    //{
    //    public int id { get; set; }
    //    public int ownerId { get; set; }
    //    public string locationId { get; set; }
    //    public string number { get; set; }
    //    public string kindOfGoodDescription { get; set; }
    //    public int kindOfGoodId { get; set; }
    //    public string description { get; set; }
    //    public BaseUnit baseUnit { get; set; }
    //}

    //public class Attributes
    //{
    //}

    //public class ArticleInstance
    //{
    //    public Article article { get; set; }
    //    public Attributes attributes { get; set; }
    //}

    //public class Country
    //{
    //    public int? id { get; set; }
    //    public string isoCode { get; set; }
    //}

    //public class Address
    //{
    //    public string name { get; set; }
    //    public string street { get; set; }
    //    public string zipCode { get; set; }
    //    public string city { get; set; }
    //    public Country country { get; set; }
    //}

    //public class SupplierOrConsignee
    //{
    //    public int id { get; set; }
    //    public string businessIdentifier { get; set; }
    //    public string name { get; set; }
    //    public string owningLocationId { get; set; }
    //    public string subName { get; set; }
    //    public Address address { get; set; }
    //}

    //public class Service
    //{
    //    public int id { get; set; }
    //}

    //public class OrderItemService
    //{
    //    public OrderItemService()
    //    {
    //        articleInstance = new ArticleInstance();
    //        supplierOrConsignee = new SupplierOrConsignee();
    //        service = new Service();
    //    }

    //    public string identifier { get; set; }
    //    public string remark { get; set; }
    //    public int sequence { get; set; }
    //    public ArticleInstance articleInstance { get; set; }
    //    public string state { get; set; }
    //    public SupplierOrConsignee supplierOrConsignee { get; set; }
    //    public string plannedBeginDate { get; set; }
    //    public string plannedEndDate { get; set; }
    //    public string actualBeginDate { get; set; }
    //    public string actualEndDate { get; set; }
    //    public Service service { get; set; }
    //    public int targetAmount { get; set; }
    //    public int actualAmount { get; set; }
    //    public string deliveryType { get; set; }
    //}

    //public class Product
    //{
    //    public int id { get; set; }
    //    public string description { get; set; }
    //}

    //public class OrderItem
    //{
    //    public OrderItem()
    //    {
    //      //orderItemServices = new List<OrderItemService>();
    //        orderItemServices = new OrderItemService();
    //        product = new Product();
    //    }

    //    public string identifier { get; set; }
    //    public int sequence { get; set; }
    //    public string orderItemState { get; set; }
    //  //  public List<OrderItemService> orderItemServices { get; set; }
    //    public OrderItemService orderItemServices { get; set; }

    //    public string plannedDate { get; set; }
    //    public Product product { get; set; }
    //}

    //public class Country2
    //{
    //    public int? id { get; set; }
    //    public string isoCode { get; set; }
    //}

    //public class Address2
    //{
    //    public Address2()
    //    {
    //        country = new Country2();
    //    }

    //    public string name { get; set; }
    //    public string street { get; set; }
    //    public string zipCode { get; set; }
    //    public string city { get; set; }
    //    public Country2 country { get; set; }
    //}

    //public class Customer
    //{
    //    public Customer()
    //    {
    //        address = new Address2();
    //    }

    //    public int? id { get; set; }
    //    public string businessIdentifier { get; set; }
    //    public string name { get; set; }
    //    public string owningLocationId { get; set; }
    //    public string subName { get; set; }
    //    public Address2 address { get; set; }
    //}

    //public class Country3
    //{
    //    public int? id { get; set; }
    //    public string isoCode { get; set; }
    //}

    //public class Address3
    //{
    //    public Address3()
    //    {
    //        country = new Country3();
    //    }

    //    public string name { get; set; }
    //    public string street { get; set; }
    //    public string zipCode { get; set; }
    //    public string city { get; set; }
    //    public Country3 country { get; set; }
    //}

    //public class InvoiceReceiver
    //{
    //    public InvoiceReceiver()
    //    {
    //        address = new Address3();
    //    }

    //    public int? id { get; set; }
    //    public string businessIdentifier { get; set; }
    //    public string name { get; set; }
    //    public string owningLocationId { get; set; }
    //    public string subName { get; set; }
    //    public Address3 address { get; set; }
    //}


    //public class Waegung
    //{
    //    public int weight_nr_1 { get; set; }
    //    public string scale_1 { get; set; }
    //    public int weight_nr_2 { get; set; }
    //    public string scale_2 { get; set; }
    //    public string delivery_note { get; set; }
    //    public string weight_note { get; set; }
    //    public string vehicle_in { get; set; }
    //    public string freight_carrier_in { get; set; }
    //    public string vehicle_out { get; set; }
    //    public string freight_carrier_out { get; set; }
    //    public string origin_store_area { get; set; }
    //    public string destination_storage_area { get; set; }

    //    //Erstgewicht
    //    public decimal tara_weight { get; set; }
    //    //Zweitgewicht
    //    public decimal gros_weight { get; set; }
    //    // Nettogewicht 
    //    public decimal net_weight { get; set; }
    //    // Istmenge ZMEI
    //    public decimal amount_aqu1 { get; set; }
    //    // Arbeitsbeginn
    //    public DateTime working_start { get; set; }
    //    // Arbeitsende
    //    public DateTime working_end { get; set; }
    //    // Stornokennzeichen
    //    public bool delete_flag { get; set; }
    //}

    //public class RootObject
    //{
    //    public RootObject()
    //    {
    //    //    orderItems = new List<OrderItem>();
    //        orderItems = new OrderItem();
    //        customer = new Customer();
    //        invoiceReceiver = new InvoiceReceiver();

    //        waegung = new Waegung();
    //            }

    //      public OrderItem orderItems { get; set; }
    //  //  public List<OrderItem> orderItems { get; set; }
    //    public string id { get; set; }
    //    public string number { get; set; }
    //    public string reference { get; set; }
    //    public string orderState { get; set; }
    //    public Customer customer { get; set; }
    //    public InvoiceReceiver invoiceReceiver { get; set; }
    //    public string locationId { get; set; }
    //    public string date { get; set; }
    //    public Waegung waegung { get; set; }
    //}

    //******************************************************************************************************
    //*******************************************************************************************************
    //********************************************************************************************************
    public class BaseUnit
    {
        public int id { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
    }

    public class Article
    {
        public int id { get; set; }
        public int ownerId { get; set; }
        public string locationId { get; set; }
        public string number { get; set; }
        public string kindOfGoodDescription { get; set; }
        public int kindOfGoodId { get; set; }
        public string description { get; set; }
        public BaseUnit baseUnit { get; set; }
    }

    public class Attributes
    {
    }

    public class ArticleInstance
    {
        public Article article { get; set; }
        public Attributes attributes { get; set; }
    }

    public class Country
    {
        public int id { get; set; }
        public string isoCode { get; set; }
    }

    public class Address
    {
        public string name { get; set; }
        public string street { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public Country country { get; set; }
    }

    public class SupplierOrConsignee
    {
        public int id { get; set; }
        public string businessIdentifier { get; set; }
        public string name { get; set; }
        public string owningLocationId { get; set; }
        public string subName { get; set; }
        public Address address { get; set; }
    }

    public class Service
    {
        public int id { get; set; }
    }

    public class OrderItemService
    {
        public string identifier { get; set; }
        public string remark { get; set; }
        public int sequence { get; set; }
        public ArticleInstance articleInstance { get; set; }
        public string state { get; set; }
        public SupplierOrConsignee supplierOrConsignee { get; set; }
        public string plannedBeginDate { get; set; }
        public string plannedEndDate { get; set; }
        public string actualBeginDate { get; set; }
        public string actualEndDate { get; set; }
        public Service service { get; set; }
        public int targetAmount { get; set; }
        public int actualAmount { get; set; }
        public string deliveryType { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public class OrderItem
    {
        public string identifier { get; set; }
        public int sequence { get; set; }
        public string orderItemState { get; set; }
        public List<OrderItemService> orderItemServices { get; set; }
        public string plannedDate { get; set; }
        public Product product { get; set; }
    }

    public class Country2
    {
        public int? id { get; set; }
        public string isoCode { get; set; }
    }

    public class Address2
    {
        public string name { get; set; }
        public string street { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public Country2 country { get; set; }
    }

    public class Customer
    {
        public int? id { get; set; }
        public string businessIdentifier { get; set; }
        public string name { get; set; }
        public string owningLocationId { get; set; }
        public string subName { get; set; }
        public Address2 address { get; set; }
    }

    public class Country3
    {
        public int? id { get; set; }
        public string isoCode { get; set; }
    }

    public class Address3
    {
        public string name { get; set; }
        public string street { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public Country3 country { get; set; }
    }

    public class InvoiceReceiver
    {
        public int? id { get; set; }
        public string businessIdentifier { get; set; }
        public string name { get; set; }
        public string owningLocationId { get; set; }
        public string subName { get; set; }
        public Address3 address { get; set; }
    }

    public class RootObject
    {
        public List<OrderItem> orderItems { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public string reference { get; set; }
        public string orderState { get; set; }
        public Customer customer { get; set; }
        public InvoiceReceiver invoiceReceiver { get; set; }
        public string locationId { get; set; }
        public string date { get; set; }
    }
}