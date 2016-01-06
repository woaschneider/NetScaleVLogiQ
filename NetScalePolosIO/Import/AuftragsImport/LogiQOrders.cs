//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NetScalePolosIO.Import.AuftragsImport
//{
//    public class BaseUnit
//    {
//        public string id { get; set; }
//        public string shortDescription { get; set; }
//        public string description { get; set; }
//    }

//    public class Article
//    {
//        public string id { get; set; }
//        public string ownerId { get; set; }
//        public string locationId { get; set; }
//        public string number { get; set; }
//        public string kindOfGoodDescription { get; set; }
//        public string kindOfGoodId { get; set; }
//        public string description { get; set; }
//        public string extendedDescription { get; set; }
//        public BaseUnit baseUnit { get; set; }
//    }

//    public class Attributes
//    {
//    }

//    public class ArticleInstance
//    {
//        public string id { get; set; }
//        public Article article { get; set; }
//        public Attributes attributes { get; set; }
//    }

//    public class Service
//    {
//        public string id { get; set; }
//        public string description { get; set; }
//        public bool scaleRelevant { get; set; }
//        public string stockType { get; set; }
//    }

//    public class PlanningDivision
//    {
//        public string id { get; set; }
//        public string description { get; set; }
//        public bool active { get; set; }
//    }

//    public class OrderItemService
//    {
//        public string identifier { get; set; }
//        public string orderNumber { get; set; }
//        public int orderItemSequence { get; set; }
//        public int sequence { get; set; }
//        public ArticleInstance articleInstance { get; set; }
//        public string state { get; set; }
//        public Service service { get; set; }
//        public int targetAmount { get; set; }
//        public PlanningDivision planningDivision { get; set; }
//    }

//    public class Service2
//    {
//        public string id { get; set; }
//        public string description { get; set; }
//        public bool scaleRelevant { get; set; }
//        public string stockType { get; set; }
//    }

//    public class Product
//    {
//        public string id { get; set; }
//        public string description { get; set; }
//        public List<Service2> services { get; set; }
//    }

//    public class OrderItem
//    {
//        public string identifier { get; set; }
//        public string reference { get; set; }
//        public int sequence { get; set; }
//        public string orderItemState { get; set; }
//        public List<OrderItemService> orderItemServices { get; set; }
//        public string plannedDate { get; set; }
//        public Product product { get; set; }
//    }

//    public class Country
//    {
//        public string id { get; set; }
//        public string isoCode { get; set; }
//    }

//    public class Address
//    {
//        public string name { get; set; }
//        public string street { get; set; }
//        public string zipCode { get; set; }
//        public string city { get; set; }
//        public Country country { get; set; }
//    }

//    public class Customer
//    {
//        public string id { get; set; }
//        public string businessIdentifier { get; set; }
//        public string name { get; set; }
//        public string owningLocationId { get; set; }
//        public Address address { get; set; }
//    }

//    public class Country2
//    {
//        public string id { get; set; }
//        public string isoCode { get; set; }
//    }

//    public class Address2
//    {
//        public string name { get; set; }
//        public string street { get; set; }
//        public string zipCode { get; set; }
//        public string city { get; set; }
//        public Country2 country { get; set; }
//    }

//    public class InvoiceReceiver
//    {
//        public string id { get; set; }
//        public string businessIdentifier { get; set; }
//        public string name { get; set; }
//        public string owningLocationId { get; set; }
//        public Address2 address { get; set; }
//    }

//    public class RootObject
//    {
//        public List<OrderItem> orderItems { get; set; }
//        public string id { get; set; }
//        public string number { get; set; }
//        public string reference { get; set; }
//        public string invoiceReference { get; set; }
//        public string orderState { get; set; }
//        public Customer customer { get; set; }
//        public InvoiceReceiver invoiceReceiver { get; set; }
//        public string locationId { get; set; }
//        public string date { get; set; }
//    }
//}
