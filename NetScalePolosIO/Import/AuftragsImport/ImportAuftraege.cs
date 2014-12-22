﻿using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.Adressen;
using HWB.NETSCALE.POLOSIO.AuftragsImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace NetScalePolosIO.Import.AuftragsImport
{
    public class ImportAuftraege
    {
        private Adressen _boA;
        private AdressenEntity _boAe;

        private readonly Orderitem _boO = new Orderitem();
        private OrderitemEntity _boOe;

        private OrderItemservice _boOis;
        private OrderItemserviceEntity _boOise;

  

        private int _totalresult;
        public bool Import(string baseUrl, int location)
        {
            var client = new RestClient(baseUrl);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
            //  var request = new RestRequest("/rest/order/query/200/1?status=NEW");
            var request = new RestRequest("/rest/order/query/200/1") {Method = Method.GET};
            request.AddHeader("X-location-Id", location.ToString());


            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return false;
         

            var oOi = JsonConvert.DeserializeObject<RootObject>(response.Content);

            _totalresult = oOi.totalResults;
            int nPage;
        
            if( _totalresult % 200==0)
            {
                nPage = _totalresult/ 200; 
            }
            else
            {
                nPage = _totalresult/200+1;
            }

            for (int ii = 1; ii <= nPage; ii++)
            {

                request = new RestRequest("/rest/order/query/200/" + ii) {Method = Method.GET};
                request.AddHeader("X-location-Id", location.ToString());


                 response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
             

                oOi = JsonConvert.DeserializeObject<RootObject>(response.Content);

                //***********************************************************************
            foreach (OrderEntity obj in oOi.orders)
            {
                #region Fertige Aufträge löschen
                if (obj.orderState == "CANCELLED" | obj.orderState == "CLOSED" | obj.orderState == " COMPLETELY_CLOSED")
                {

                    _boOe = _boO.GetById(obj.id);
                    if(_boOe!=null)
                    {
                    _boO.DeleteEntity(_boOe);
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
                        _boOe = _boO.GetById(obj.id);
                    }
                    if (_boOe == null)
                    {
                        _boOe = _boO.NewEntity();
                    }
                    if (_boOe != null)
                    {
                        _boOe.id = obj.id;
                        _boOe.locationId = obj.locationId;
                        _boOe.number = obj.number;
                        if (obj.date != null)
                        {
                            _boOe.date = PolosUtitlities.ConvertPolosDateTime2DateTime(obj.date);
                        }
                        _boOe.orderstate = obj.orderState;
                        _boOe.reference = obj.reference;

                        // Adressen

                        #region Customer and Invoice

                        // Eigentich brauchen wir nur den BusinessIdentfier 
                        // Später vielleicht zusätzlich (redundant) nehmen wir noch PK und ID

                        //customer
                        _boOe.customerBusinessIdentifier = obj.customer.businessIdentifier;
                        _boOe.customerId = obj.customer.id;
                        _boA = new Adressen();
                        if (_boOe.customerId != null)
                        {
                            _boAe = _boA.GetById(obj.customer.id);
                            if (_boAe != null)
                            {
                                _boOe.customerName = _boAe.name;
                                _boOe.customerSubName2 = _boAe.subName2;
                                _boOe.customerOwningLocationId = _boAe.owningLocationId;
                                _boOe.customerZipCode = _boAe.zipCode;
                                _boOe.customerCity = _boAe.city;
                                _boOe.customerStreet = _boAe.street;
                                _boOe.customerIdCountry = _boAe.idCountry;
                                _boOe.customerIsocodeCountry = _boAe.isocodeCountry;
                            }
                        }
                        //invoice
                        _boOe.invoiceReceicerBusinessIdentifier = obj.invoiceReceiver.businessIdentifier;
                        _boOe.invoiceReceiverId = obj.invoiceReceiver.id;
                        if (_boOe.invoiceReceiverId != null)
                        {
                            _boAe = _boA.GetById(obj.customer.id);
                            if (_boAe != null)
                            {
                                _boOe.invoiceReceiverName = _boAe.name;
                                _boOe.InvoiceReceiverSubName2 = _boAe.subName2;
                                _boOe.invoiceReceiverOwningLocationId = _boAe.owningLocationId;
                                _boOe.InvoiceReceiverZipCode = _boAe.zipCode;
                                _boOe.InvoiceReceiverCity = _boAe.city;
                                _boOe.invoiceReceiverStreet = _boAe.street;
                                _boOe.invoiceReceiverIdCountry = _boAe.idCountry;
                                _boOe.invoiceReceiverIsocodeCountry = _boAe.isocodeCountry;
                            }
                        }

                        #endregion

                        _boO.SaveEntity(_boOe);
                        _boOe = _boO.GetById(_boOe.id); // Damit ich jetzt den OK habe

                        // *******************************************************************************************************
                        // OrderItemservice
                        //********************************************************************************************************

                        #region orderItemService

                        request = new RestRequest("/rest/order/" + _boOe.id) {Method = Method.GET};
                        request.AddHeader("X-location-Id", "16");


                        response = client.Execute(request);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                

                            OrderEntity oOEntity = JsonConvert.DeserializeObject<OrderEntity>(response.Content);

                            foreach (var obj2 in oOEntity.orderItems)
                                foreach (OrderItemService t in obj2.orderItemServices)
                                {
                                    _boOis = new OrderItemservice();
                                    _boOise = _boOis.GetByIdAndPKOrderItem(_boOe.PK, t.identifier) ?? _boOis.NewEntity();

                                    _boOise.identifier = t.identifier;
                                    _boOise.remark = t.remark;
                                    _boOise.sequence = t.sequence;
                                    _boOise.product = oOEntity.orderItems[0].product.id;
                                    // 14.11.2014 Das entspricht der Schnittstellenbeschreibung
                                    _boOise.productdescription = oOEntity.orderItems[0].product.description;
                                    // 14.11.2014 Das entspricht der Schnittstellenbeschreibung

                                    _boOise.remark = t.remark;

                                    _boOise.deliveryType = t.deliveryType;

                                    #region ArticelInstance

                                    if (t.articleInstance != null)
                                    {
                                        _boOise.articleId = t.articleInstance.article.id;
                                        _boOise.articleDescription =
                                            t.articleInstance.article.number;
                                        // Ganz schräg Inhalt zB Testbleche
                                        _boOise.ownerId = t.articleInstance.article.ownerId;
                                        _boOise.kindOfGoodId =
                                            t.articleInstance.article.kindOfGoodId;
                                        _boOise.kindOfGoodDescription =
                                            t.articleInstance.article.kindOfGoodDescription;
                                        if (oOEntity.orderItems[0].plannedDate != null)
                                        {
                                            _boOise.plannedDate =
                                                PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                    oOEntity.orderItems[0].plannedDate);
                                        }

                                        AdressenEntity boAe =
                                            _boA.GetById(t.articleInstance.article.ownerId);
                                        if (boAe != null)
                                            _boOise.ownerBusinessIdentifier = boAe.businessIdentifier;

                                        #region // service

                                        _boOise.targedAmount = (decimal) t.service.targedAmount;

                                        #endregion
                                    }

                                    #endregion

                                    #region Supplier Consignee

                                    if (t.supplierOrConsignee != null)
                                    {
                                        _boOise.supplierOrConsigneeId = t.supplierOrConsignee.id;
                                        _boOise.supplierOrConsigneeBusinessIdentifier =
                                            t.supplierOrConsignee.businessIdentifier;
                                        _boOise.supplierOrConsigneeName =
                                            t.supplierOrConsignee.name;
                                        _boOise.supplierOrConsigneeSubName2 =
                                            t.supplierOrConsignee.subName;
                                        _boOise.supplierOrConsigneeCity =
                                            t.supplierOrConsignee.address.city;
                                        _boOise.supplierOrConsigneeStreet =
                                            t.supplierOrConsignee.address.street;
                                        _boOise.supplierOrConsigneeZipCode =
                                            t.supplierOrConsignee.address.zipCode;
                                        _boOise.supplierOrConsigneedIdCountry =
                                            t.supplierOrConsignee.address.country.id;

                                        _boOise.supplierOrConsigneeIsocodeCountry =
                                            t.supplierOrConsignee.address.country.isoCode;
                                    }

                                    #endregion

                                    #region Clearance

                                    // boOISE.clearanceQuantity   = Nicht vorhanden
                                    if (t.clearance != null)
                                    {
                                        _boOise.clearanceReferenz = t.clearance.reference;
                                        if (t.clearance.validFrom != null)
                                        {
                                            _boOise.clearanceValidFrom =
                                                PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                    t.clearance.validFrom);
                                        }

                                        if (t.clearance.validTo != null)
                                        {
                                            _boOise.clearanceValidTo =
                                                PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                    t.clearance.validTo);
                                        }
                                        _boOise.clearanceUnitId = t.clearance.unit.id;
                                        _boOise.clearanceUnitShortDescription =
                                            t.clearance.unit.shortDescription;
                                        _boOise.clearanceDescription =
                                            t.clearance.unit.description;

                                        #endregion

                                        #region Attribute

                                        if (t.articleInstance != null)
                                        {
                                            if (t.articleInstance.attributes != null)
                                            {
                                                _boOise.SerialNumber =
                                                    t.articleInstance.attributes.SERIAL_NUMBER;
                                                _boOise.batch =
                                                    t.articleInstance.attributes.BATCH;
                                                _boOise.orign =
                                                    t.articleInstance.attributes.ORIGIN;
                                                _boOise.grade =
                                                    t.articleInstance.attributes.GRADE;
                                                _boOise.originalNumber =
                                                    t.articleInstance.attributes.ORIGINAL_NUMBER;
                                                _boOise.length =
                                                    t.articleInstance.attributes.LENGTH;
                                                _boOise.width =
                                                    t.articleInstance.attributes.WIDTH;
                                                _boOise.height =
                                                    t.articleInstance.attributes.HEIGHT;

                                                _boOise.storageAreaReference =
                                                    t.articleInstance.attributes.
                                                        STORAGE_AREA_REFERENCE;
                                                _boOise.diameter =
                                                    t.articleInstance.attributes.DIAMETER;
                                                _boOise.orignalMarking =
                                                    t.articleInstance.attributes.
                                                        ORIGINAL_MARKING;
                                                _boOise.storageAreaReferenceNumber =
                                                    t.articleInstance.attributes.
                                                        STORAGE_AREA_REFERENCE_NUMBER;
                                                _boOise.dimension =
                                                    t.articleInstance.attributes.DIMENSION;
                                            }
                                        }
                                    }

                                    #endregion

                                    _boOise.PKOrderItem = _boOe.PK;
                                    _boOis.SaveEntity(_boOise);
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