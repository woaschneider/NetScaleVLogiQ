using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.AuftragsImport
{



public class BaseUnit
{
    public int id { get; set; }
    public string shortDescription { get; set; }
    public string description { get; set; }
}

public class ConversionUnit
{
    public int id { get; set; }
    public string shortDescription { get; set; }
    public string description { get; set; }
}

public class Article
{
    public string id { get; set; }
    public string ownerId { get; set; }
    public string locationId { get; set; }
    public string number { get; set; }
    public string kindOfGoodDescription { get; set; }
    public string kindOfGoodId { get; set; }
    public string description { get; set; }
    public BaseUnit baseUnit { get; set; }
    public ConversionUnit conversionUnit { get; set; }
}

public class Attributes
{
    public string SERIAL_NUMBER { get; set; }
    public string BATCH { get; set; }
    public string ORIGIN { get; set; }
    public string GRADE { get; set; }
    public string ORIGINAL_NUMBER { get; set; }
    public string LENGTH { get; set; }
    public string WIDTH { get; set; }
    public string HEIGHT { get; set; }
    public string STORAGE_AREA_REFERENCE { get; set; }
    public string DIAMETER { get; set; }
    public string ORIGINAL_MARKING { get; set; }
    public string STORAGE_AREA_REFERENCE_NUMBER { get; set; }
    public string DIMENSION { get; set; }
}

public class ArticleInstance
{
    public Article article { get; set; }
    public Attributes attributes { get; set; }
    public Service service { get; set; }
}

public class Country
{
    public string id { get; set; }
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
    public string id { get; set; }
    public string businessIdentifier { get; set; }
    public string name { get; set; }
    public string owningLocationId { get; set; }
    public string subName { get; set; }
    public Address address { get; set; }
}

public class Service
{
    public string id { get; set; }
    public string description { get; set; }
    
    public double targedAmount { get; set; }
}

public class Unit
{
    public string id { get; set; }
    public string shortDescription { get; set; }
    public string description { get; set; }
}

public class Clearance
{
    public Unit unit { get; set; }
    public string validFrom { get; set; }
    public string validTo { get; set; }
    public int authorizerId { get; set; }
    public int granteeId { get; set; }
    public string reference { get; set; }
}

public class OrderItemService
{
    public string identifier { get; set; }
    public string remark { get; set; }
    public string sequence { get; set; }
    public ArticleInstance articleInstance { get; set; }
    public string state { get; set; }
    public SupplierOrConsignee supplierOrConsignee { get; set; }
    public string plannedBeginDate { get; set; }
    public string plannedEndDate { get; set; }
    public Service service { get; set; }
    public Clearance clearance { get; set; }
 //   public double targetAmount { get; set; }
    public string actualStorageAreaId { get; set; }
    public string targetStorageAreaId { get; set; }
    public string deliveryType { get; set; }
}

public class Product
{
    public string id { get; set; }
    public string description { get; set; }
}

public class OrderItem
{
    public string identifier { get; set; }
    public string sequence { get; set; }
    public string orderItemState { get; set; }
    public List<OrderItemService> orderItemServices { get; set; }
    public string plannedDate { get; set; }
    public Product product { get; set; }
}

public class Country2
{
    public string id { get; set; }
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
    public string id { get; set; }
    public string businessIdentifier { get; set; }
    public string name { get; set; }
    public string owningLocationId { get; set; }
    public string subName { get; set; }
    public Address2 address { get; set; }
}

public class Country3
{
    public string id { get; set; }
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
    public string id { get; set; }
    public string businessIdentifier { get; set; }
    public string name { get; set; }
    public string owningLocationId { get; set; }
    public string subName { get; set; }
    public Address3 address { get; set; }
}

public class OrderEntity
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

public class RootObject
{
    public List<OrderEntity> orders { get; set; }
    public int totalResults  { get; set; }
  

  
}



    
  } 