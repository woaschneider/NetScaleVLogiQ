using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ProductsImport;
using NetScalePolosIO;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.AuftragsImport
{
    public class ImportAuftraege
    {
        private Adressen boA;
        private AdressenEntity boAE;

        private Orderitem boO = new Orderitem();
        private OrderitemEntity boOE;

        private HWB.NETSCALE.BOEF.OrderItemservice boOIS;
        private HWB.NETSCALE.BOEF.OrderItemserviceEntity boOISE;

        private HWB.NETSCALE.BOEF.OrderArticleAttribute boOAA;
        private HWB.NETSCALE.BOEF.OrderArticleAttributeEntity boOAAE;

        private int totalresult;
        public bool Import(string baseUrl, int location)
        {
            var client = new RestClient(baseUrl);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
            TODO:
          //  var request = new RestRequest("/rest/order/query/200/1?status=NEW");
            var request = new RestRequest("/rest/order/query/200/1");
            request.Method = Method.GET;
            request.AddHeader("X-location-Id", location.ToString());


            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return false;
            var x = response.Content; // Nur für Testzwecke

            var oOI = JsonConvert.DeserializeObject<RootObject>(response.Content);

            totalresult = oOI.totalResults;
            int nPage = 0;
        
            if( totalresult % 200==0)
            {
                nPage = totalresult/ 200; 
            }
            else
            {
                nPage = (int) (totalresult/200)+1;
            }

            for (int ii = 1; ii <= nPage; ii++)
            {

                request = new RestRequest("/rest/order/query/200/" + ii.ToString());
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", location.ToString());


                 response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                x = response.Content; // Nur für Testzwecke

                oOI = JsonConvert.DeserializeObject<RootObject>(response.Content);

                //***********************************************************************
            foreach (OrderEntity obj in oOI.orders)
            {
                #region Fertige Aufträge löschen
                if (obj.orderState == "CANCELLED" | obj.orderState == "CLOSED" | obj.orderState == " COMPLETELY_CLOSED")
                {

                    boOE = boO.GetById(obj.id);
                    if(boOE!=null)
                    {
                    boO.DeleteEntity(boOE);
                    }
                    
                }

                #endregion

                // Das müssen wir noch klären
                //if (obj.orderState == "NEW")
               // {

                    #region Eigentlicher Import

                    // Order Status prüfen


                    if (obj.id != null)
                    {
                        boOE = boO.GetById(obj.id);
                    }
                    if (boOE == null)
                    {
                        boOE = boO.NewEntity();
                    }
                    if (boOE != null)
                    {
                        boOE.id = obj.id;
                        boOE.locationId = obj.locationId;
                        boOE.number = obj.number;
                        if (obj.date != null)
                        {
                            boOE.date = PolosUtitlities.ConvertPolosDateTime2DateTime(obj.date);
                        }
                        boOE.orderstate = obj.orderState;
                        boOE.reference = obj.reference;

                        // Adressen

                        #region Customer and Invoice

                        // Eigentich brauchen wir nur den BusinessIdentfier 
                        // Später vielleicht zusätzlich (redundant) nehmen wir noch PK und ID

                        //customer
                        boOE.customerBusinessIdentifier = obj.customer.businessIdentifier;
                        boOE.customerId = obj.customer.id;
                        boA = new Adressen();
                        if (boOE.customerId != null)
                        {
                            boAE = boA.GetById(obj.customer.id);
                            if (boAE != null)
                            {
                                boOE.customerName = boAE.name;
                                boOE.customerSubName2 = boAE.subName2;
                                boOE.customerOwningLocationId = boAE.owningLocationId;
                                boOE.customerZipCode = boAE.zipCode;
                                boOE.customerCity = boAE.city;
                                boOE.customerStreet = boAE.street;
                                boOE.customerIdCountry = boAE.idCountry;
                                boOE.customerIsocodeCountry = boAE.isocodeCountry;
                            }
                        }
                        //invoice
                        boOE.invoiceReceicerBusinessIdentifier = obj.invoiceReceiver.businessIdentifier;
                        boOE.invoiceReceiverId = obj.invoiceReceiver.id;
                        if (boOE.invoiceReceiverId != null)
                        {
                            boAE = boA.GetById(obj.customer.id);
                            if (boAE != null)
                            {
                                boOE.invoiceReceiverName = boAE.name;
                                boOE.InvoiceReceiverSubName2 = boAE.subName2;
                                boOE.invoiceReceiverOwningLocationId = boAE.owningLocationId;
                                boOE.InvoiceReceiverZipCode = boAE.zipCode;
                                boOE.InvoiceReceiverCity = boAE.city;
                                boOE.invoiceReceiverStreet = boAE.street;
                                boOE.invoiceReceiverIdCountry = boAE.idCountry;
                                boOE.invoiceReceiverIsocodeCountry = boAE.isocodeCountry;
                            }
                        }

                        #endregion

                        boO.SaveEntity(boOE);
                        boOE = boO.GetById(boOE.id); // Damit ich jetzt den OK habe

                        // *******************************************************************************************************
                        // OrderItemservice
                        //********************************************************************************************************

                        #region orderItemService

                        request = new RestRequest("/rest/order/" + boOE.id.ToString());
                        request.Method = Method.GET;
                        request.AddHeader("X-location-Id", "16");


                        response = client.Execute(request);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            x = response.Content; // Nur für Testzwecke

                            OrderEntity oOEntity = JsonConvert.DeserializeObject<OrderEntity>(response.Content);

                            foreach (var obj2 in oOEntity.orderItems)
                                for (int i = 0; i < obj2.orderItemServices.Count; i++)
                                {
                                    boOIS = new OrderItemservice();
                                    boOISE = boOIS.GetByIdAndPKOrderItem(boOE.PK, obj2.orderItemServices[i].identifier);

                                    if (boOISE == null)
                                    {
                                        boOISE = boOIS.NewEntity();
                                    }
                                    boOISE.identifier = obj2.orderItemServices[i].identifier;
                                    boOISE.remark = obj2.orderItemServices[i].remark;
                                    boOISE.sequence = obj2.orderItemServices[i].sequence;
                                    boOISE.product = oOEntity.orderItems[0].product.id;
                                    // 14.11.2014 Das entspricht der Schnittstellenbeschreibung
                                    boOISE.productdescription = oOEntity.orderItems[0].product.description;
                                    // 14.11.2014 Das entspricht der Schnittstellenbeschreibung

                                    boOISE.remark = obj2.orderItemServices[i].remark;

                                    boOISE.deliveryType = obj2.orderItemServices[i].deliveryType;

                                    #region ArticelInstance

                                    if (obj2.orderItemServices[i].articleInstance != null)
                                    {
                                        boOISE.articleId = obj2.orderItemServices[i].articleInstance.article.id;
                                        boOISE.articleDescription =
                                            obj2.orderItemServices[i].articleInstance.article.number;
                                        // Ganz schräg Inhalt zB Testbleche
                                        boOISE.ownerId = obj2.orderItemServices[i].articleInstance.article.ownerId;
                                        boOISE.kindOfGoodId =
                                            obj2.orderItemServices[i].articleInstance.article.kindOfGoodId;
                                        boOISE.kindOfGoodDescription =
                                            obj2.orderItemServices[i].articleInstance.article.kindOfGoodDescription;
                                        if (oOEntity.orderItems[0].plannedDate != null)
                                        {
                                            boOISE.plannedDate =
                                                PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                    oOEntity.orderItems[0].plannedDate);
                                        }

                                        AdressenEntity boAE =
                                            boA.GetById(obj2.orderItemServices[i].articleInstance.article.ownerId);
                                        if (boAE != null)
                                            boOISE.ownerBusinessIdentifier = boAE.businessIdentifier;

                                        #region // service

                                        boOISE.targedAmount = (decimal) obj2.orderItemServices[i].service.targedAmount;

                                        #endregion
                                    }

                                    #endregion

                                    #region Supplier Consignee

                                    if (obj2.orderItemServices[i].supplierOrConsignee != null)
                                    {
                                        boOISE.supplierOrConsigneeId = obj2.orderItemServices[i].supplierOrConsignee.id;
                                        boOISE.supplierOrConsigneeBusinessIdentifier =
                                            obj2.orderItemServices[i].supplierOrConsignee.businessIdentifier;
                                        boOISE.supplierOrConsigneeName =
                                            obj2.orderItemServices[i].supplierOrConsignee.name;
                                        boOISE.supplierOrConsigneeSubName2 =
                                            obj2.orderItemServices[i].supplierOrConsignee.subName;
                                        boOISE.supplierOrConsigneeCity =
                                            obj2.orderItemServices[i].supplierOrConsignee.address.city;
                                        boOISE.supplierOrConsigneeStreet =
                                            obj2.orderItemServices[i].supplierOrConsignee.address.street;
                                        boOISE.supplierOrConsigneeZipCode =
                                            obj2.orderItemServices[i].supplierOrConsignee.address.zipCode;
                                        boOISE.supplierOrConsigneedIdCountry =
                                            obj2.orderItemServices[i].supplierOrConsignee.address.country.id;

                                        boOISE.supplierOrConsigneeIsocodeCountry =
                                            obj2.orderItemServices[i].supplierOrConsignee.address.country.isoCode;
                                    }

                                    #endregion

                                    #region Clearance

                                    // boOISE.clearanceQuantity   = Nicht vorhanden
                                    if (obj2.orderItemServices[i].clearance != null)
                                    {
                                        boOISE.clearanceReferenz = obj2.orderItemServices[i].clearance.reference;
                                        if (obj2.orderItemServices[i].clearance.validFrom != null)
                                        {
                                            boOISE.clearanceValidFrom =
                                                PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                    obj2.orderItemServices[i].clearance.validFrom);
                                        }

                                        if (obj2.orderItemServices[i].clearance.validTo != null)
                                        {
                                            boOISE.clearanceValidTo =
                                                PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                    obj2.orderItemServices[i].clearance.validTo);
                                        }
                                        boOISE.clearanceUnitId = obj2.orderItemServices[i].clearance.unit.id;
                                        boOISE.clearanceUnitShortDescription =
                                            obj2.orderItemServices[i].clearance.unit.shortDescription;
                                        boOISE.clearanceDescription =
                                            obj2.orderItemServices[i].clearance.unit.description;

                                        #endregion

                                        #region Attribute

                                        if (obj2.orderItemServices[i].articleInstance != null)
                                        {
                                            if (obj2.orderItemServices[i].articleInstance.attributes != null)
                                            {
                                                boOISE.SerialNumber =
                                                    obj2.orderItemServices[i].articleInstance.attributes.SERIAL_NUMBER;
                                                boOISE.batch =
                                                    obj2.orderItemServices[i].articleInstance.attributes.BATCH;
                                                boOISE.orign =
                                                    obj2.orderItemServices[i].articleInstance.attributes.ORIGIN;
                                                boOISE.grade =
                                                    obj2.orderItemServices[i].articleInstance.attributes.GRADE;
                                                boOISE.originalNumber =
                                                    obj2.orderItemServices[i].articleInstance.attributes.ORIGINAL_NUMBER;
                                                boOISE.length =
                                                    obj2.orderItemServices[i].articleInstance.attributes.LENGTH;
                                                boOISE.width =
                                                    obj2.orderItemServices[i].articleInstance.attributes.WIDTH;
                                                boOISE.height =
                                                    obj2.orderItemServices[i].articleInstance.attributes.HEIGHT;

                                                boOISE.storageAreaReference =
                                                    obj2.orderItemServices[i].articleInstance.attributes.
                                                        STORAGE_AREA_REFERENCE;
                                                boOISE.diameter =
                                                    obj2.orderItemServices[i].articleInstance.attributes.DIAMETER;
                                                boOISE.orignalMarking =
                                                    obj2.orderItemServices[i].articleInstance.attributes.
                                                        ORIGINAL_MARKING;
                                                boOISE.storageAreaReferenceNumber =
                                                    obj2.orderItemServices[i].articleInstance.attributes.
                                                        STORAGE_AREA_REFERENCE_NUMBER;
                                                boOISE.dimension =
                                                    obj2.orderItemServices[i].articleInstance.attributes.DIMENSION;
                                            }
                                        }
                                    }

                                    #endregion

                                    boOISE.PKOrderItem = boOE.PK;
                                    boOIS.SaveEntity(boOISE);
                                }


                        }

                        #endregion

                    }

                    #endregion
               // }
            }

            }


            return true;
        }
    }
}