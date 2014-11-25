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

            NetScalePolosIO.Export.RootObject oWEx = new RootObject();

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

            #endregion


            #region Details

            oWEx.orderItems = new List<OrderItem>();
            oWEx.orderItems.Add(new OrderItem());
            
            oWEx.orderItems[0].orderItemServices = new List<OrderItemService>();
            oWEx.orderItems[0].orderItemServices.Add(new OrderItemService());

            oWEx.orderItems[0].orderItemServices.s = boWE.sequence;
            //oWEx.orderItems.orderItemServices.
            #endregion

            #region Write To Json File

            string FileName = exportPath+"\\W"+boWE.LieferscheinNr.Trim()+".json";
            JsonHelpers.WriteToJsonFile(oWEx,FileName);

            #endregion
        }

    }
}
