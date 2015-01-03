﻿using System;
using System.Collections.Generic;


using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO;

namespace NetScalePolosIO.Export
{
    public class ExportWaegung
    {
        public void ExportLs(string exportPath,WaegeEntity boWe)
        {
          //   Einstellungen boE = new Einstellungen();
         //    boE.GetEinstellungen();

            #region JSON-Polos Struktur aufbauen
            NetScalePolosIO.Export.RootObject oWEx = new RootObject();
            oWEx.orderItems = new List<OrderItem>();
            oWEx.orderItems.Add(new OrderItem());
            oWEx.orderItems[0].product = new Product();
            oWEx.waegung = new Waegung();


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

            oWEx.id = boWe.id;
            oWEx.locationId = boWe.locationId;
            oWEx.number = boWe.number;

            #region  Adressen Customer / Invoicereceiver
            #region Customer

            oWEx.customer = new Customer();
            oWEx.customer.businessIdentifier = boWe.customerBusinessIdentifier;
            oWEx.customer.id = boWe.customerId;
            oWEx.customer.name = boWe.customerName;
            oWEx.customer.subName = boWe.customerSubName2;
            oWEx.customer.owningLocationId = boWe.customerOwningLocationId;
           

            oWEx.customer.address = new Address2();
            oWEx.customer.address.zipCode = boWe.customerZipCode;
            oWEx.customer.address.city = boWe.customerCity;
            oWEx.customer.address.street = boWe.customerStreet;
            
            oWEx.customer.address.country = new Country2();
            oWEx.customer.address.country.id = boWe.customerIdCountry;
            oWEx.customer.address.country.isoCode = boWe.customerIsocodeCountry;
            #endregion
            #region InvoiceReceiver

            oWEx.invoiceReceiver = new InvoiceReceiver();
            oWEx.invoiceReceiver.businessIdentifier = boWe.invoiceReceicerBusinessIdentifier;
            oWEx.invoiceReceiver.id = boWe.invoiceReceiverId;
            oWEx.invoiceReceiver.name = boWe.invoiceReceiverName;
            oWEx.invoiceReceiver.subName = boWe.invoiceReceiverSubName2;
            oWEx.invoiceReceiver.owningLocationId = boWe.invoiceReceiverOwningLocationId;

            oWEx.invoiceReceiver.address = new Address3();
            oWEx.invoiceReceiver.address.zipCode = boWe.invoiceReceiverZipCode;
            oWEx.invoiceReceiver.address.city = boWe.invoiceReceiverCity;
            oWEx.invoiceReceiver.address.street = boWe.invoiceReceiverStreet;

            oWEx.invoiceReceiver.address.country = new Country3();
            oWEx.invoiceReceiver.address.country.id = boWe.invoiceReceiverIdCountry;
            oWEx.invoiceReceiver.address.country.isoCode = boWe.invoiceReceiverIsocodeCountry;

            #endregion
            #region SupplierOrConsignee

            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.businessIdentifier = boWe.supplierOrConsigneeBusinessIdentifier;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.id = boWe.supplierOrConsigneeId;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.name = boWe.supplierOrConsigneeName;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.subName = boWe.invoiceReceiverSubName2;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.owningLocationId =
                boWe.supplierOrConsigneeOwningLocationId;

            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.zipCode =
                boWe.supplierOrConsigneeZipCode;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.city = boWe.supplierOrConsigneeCity;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.street = boWe.supplierOrConsigneeStreet;

            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.country.id =
                boWe.supplierOrConsigneeIdCountry;
            oWEx.orderItems[0].orderItemServices[0].supplierOrConsignee.address.country.isoCode =
                boWe.supplierOrConsigneeIsocodeCountry;

            #endregion

            #endregion


            #region Details


            // Orderitems
            oWEx.orderItems[0].orderItemServices[0].identifier = "N/A";
            oWEx.orderItems[0].orderItemServices[0].remark = boWe.remark;
            oWEx.orderItems[0].orderItemServices[0].state = null;
            oWEx.orderItems[0].orderItemServices[0].plannedBeginDate = null;
            oWEx.orderItems[0].orderItemServices[0].plannedEndDate = null;
            oWEx.orderItems[0].orderItemServices[0].actualBeginDate = null;
            oWEx.orderItems[0].orderItemServices[0].actualEndDate = null;
            oWEx.orderItems[0].orderItemServices[0].targetAmount = 0;
            oWEx.orderItems[0].orderItemServices[0].actualAmount = 0;
            oWEx.orderItems[0].orderItemServices[0].deliveryType = boWe.deliveryType;


            oWEx.orderItems[0].orderItemServices[0].sequence = boWe.sequence;
            
            
            // Produkt
            oWEx.orderItems[0].product.id = boWe.product;
            oWEx.orderItems[0].product.description = boWe.productdescription;

            #endregion 

            #region Waegung

            oWEx.waegung.weight_nr_1 = Convert.ToInt32( boWe.LN1);
            oWEx.waegung.scale_1 = "1";
            oWEx.waegung.weight_nr_2 = Convert.ToInt32(boWe.LN2);
            oWEx.waegung.delivery_note = boWe.LieferscheinNr;
            oWEx.waegung.weight_note = boWe.LieferscheinNr;
            // TODO A / E Maybe
            oWEx.waegung.vehicle_in = boWe.Fahrzeug;
            oWEx.waegung.vehicle_out = boWe.Fahrzeug;
            oWEx.waegung.freight_carrier_in = boWe.ffBusinessIdentifier;
            oWEx.waegung.freight_carrier_out = boWe.ffBusinessIdentifier;
            oWEx.waegung.origin_store_area = boWe.IstQuellLagerPlatz;
            oWEx.waegung.destination_storage_area = boWe.IstZielLagerPlatz;

            oWEx.waegung.tara_weight = boWe.Erstgewicht;
            oWEx.waegung.gros_weight = boWe.Zweitgewicht;
            oWEx.waegung.net_weight = boWe.Nettogewicht;

            // TODO: was muss da hin
            oWEx.waegung.amount_aqu1 = boWe.targedAmount;
            oWEx.waegung.working_start = boWe.ErstDatetime;
            oWEx.waegung.working_end = boWe.zweitDateTime;
            oWEx.waegung.delete_flag = false; // <-----------------

            

            #endregion

            #region Write To Json File

            string FileName = exportPath+"\\W"+boWe.LieferscheinNr.Trim()+".json";
            JsonHelpers.WriteToJsonFile(oWEx,FileName);

            #endregion
        }

    }
}