using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO;

namespace NetScalePolosIO.Export
{
    public class ExportWaegung
    {
        public void ExportLS(string exportPath,WaegeEntity boWE)
        {
             Einstellungen boE = new Einstellungen();
             EinstellungenEntity boEE = boE.GetEinstellungen();

            #region JSON-Polos Struktur aufbauen
            NetScalePolosIO.Export.RootObject oWEx = new RootObject();
            oWEx.orderItems = new List<OrderItem>();
            oWEx.orderItems.Add(new OrderItem());
            oWEx.orderItems[0].product = new Product();

            oWEx.orderItems[0].orderItemServices = new List<OrderItemService>();
            oWEx.orderItems[0].orderItemServices.Add(new OrderItemService());
            oWEx.orderItems[0].orderItemServices[0].articleInstance = new ArticleInstance();
            oWEx.orderItems[0].orderItemServices[0].articleInstance.article = new Article();
            oWEx.orderItems[0].orderItemServices[0].articleInstance.attributes = new Attributes();
            oWEx.orderItems[0].orderItemServices[0].service = new Service();
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee = new SupplierOrConsignee();
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address = new Address();
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.country = new Country();
            #endregion 

            oWEx.id = boWE.id;
            oWEx.locationId = boWE.locationId;
            oWEx.number = boWE.number;

            #region  Adressen Customer / Invoicereceiver
            #region Customer

            oWEx.customer = new Customer();
            oWEx.customer.businessIdentifier = boWE.customerBusinessIdentifier;
            oWEx.customer.id = boWE.customerId;
            oWEx.customer.name = boWE.customerName;
            oWEx.customer.subName = boWE.customerSubName2;
            oWEx.customer.owningLocationId = boWE.customerOwningLocationId;
           

            oWEx.customer.address = new Address2();
            oWEx.customer.address.zipCode = boWE.customerZipCode;
            oWEx.customer.address.city = boWE.customerCity;
            oWEx.customer.address.street = boWE.customerStreet;
            
            oWEx.customer.address.country = new Country2();
            oWEx.customer.address.country.id = boWE.customerIdCountry;
            oWEx.customer.address.country.isoCode = boWE.customerIsocodeCountry;
            #endregion
            #region InvoiceReceiver

            oWEx.invoiceReceiver = new InvoiceReceiver();
            oWEx.invoiceReceiver.businessIdentifier = boWE.invoiceReceicerBusinessIdentifier;
            oWEx.invoiceReceiver.id = boWE.invoiceReceiverId;
            oWEx.invoiceReceiver.name = boWE.invoiceReceiverName;
            oWEx.invoiceReceiver.subName = boWE.invoiceReceiverSubName2;
            oWEx.invoiceReceiver.owningLocationId = boWE.invoiceReceiverOwningLocationId;

            oWEx.invoiceReceiver.address = new Address3();
            oWEx.invoiceReceiver.address.zipCode = boWE.invoiceReceiverZipCode;
            oWEx.invoiceReceiver.address.city = boWE.invoiceReceiverCity;
            oWEx.invoiceReceiver.address.street = boWE.invoiceReceiverStreet;

            oWEx.invoiceReceiver.address.country = new Country3();
            oWEx.invoiceReceiver.address.country.id = boWE.invoiceReceiverIdCountry;
            oWEx.invoiceReceiver.address.country.isoCode = boWE.invoiceReceiverIsocodeCountry;

            #endregion
            #region SupplierOrConsignee

            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.businessIdentifier = boWE.supplierOrConsigneeBusinessIdentifier;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.id = boWE.supplierOrConsigneeId;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.name = boWE.supplierOrConsigneeName;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.subName = boWE.invoiceReceiverSubName2;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.owningLocationId =
                boWE.supplierOrConsigneeOwningLocationId;

            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.zipCode =
                boWE.supplierOrConsigneeZipCode;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.city = boWE.supplierOrConsigneeCity;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.street = boWE.supplierOrConsigneeStreet;

            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.country.id =
                boWE.supplierOrConsigneeIdCountry;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.country.isoCode =
                boWE.supplierOrConsigneeIsocodeCountry;

            #endregion

            #endregion


            #region Details


            // Orderitems
            oWEx.orderItems[0].orderItemServices[0].identifier = "N/A";
            oWEx.orderItems[0].orderItemServices[0].remark = boWE.remark;
            oWEx.orderItems[0].orderItemServices[0].state = null;
            oWEx.orderItems[0].orderItemServices[0].plannedBeginDate = null;
            oWEx.orderItems[0].orderItemServices[0].plannedEndDate = null;
            oWEx.orderItems[0].orderItemServices[0].actualBeginDate = null;
            oWEx.orderItems[0].orderItemServices[0].actualEndDate = null;
            oWEx.orderItems[0].orderItemServices[0].targetAmount = 0;
            oWEx.orderItems[0].orderItemServices[0].actualAmount = 0;
            oWEx.orderItems[0].orderItemServices[0].deliveryType = boWE.deliveryType;


            oWEx.orderItems[0].orderItemServices[0].sequence = boWE.sequence;
            
            
            // Produkt
            oWEx.orderItems[0].product.id = boWE.product;
            oWEx.orderItems[0].product.description = boWE.productdescription;

            #endregion

            #region Write To Json File

            string FileName = exportPath+"\\W"+boWE.LieferscheinNr.Trim()+".json";
            JsonHelpers.WriteToJsonFile(oWEx,FileName);

            #endregion
        }

    }
}
