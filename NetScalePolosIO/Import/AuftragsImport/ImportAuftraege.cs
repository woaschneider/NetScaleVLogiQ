using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.AuftragsImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
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

        public bool Import(string baseUrl, int location, string url)
        {
            var client = new RestClient(baseUrl);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
            //  var request = new RestRequest("/rest/order/query/200/1?status=NEW");
            //  
            var request = new RestRequest(url + "200/1") {Method = Method.GET};
            request.AddHeader("X-location-Id", location.ToString());

            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                boEe.ConsumerSecret.Trim(),
                string.Empty, string.Empty);

            #region// Anzahl der Order ermittlen

            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return false;


            var oOi = JsonConvert.DeserializeObject<RootObject>(response.Content);

            _totalresult = oOi.totalResults;
            int nPage;

            if (_totalresult%200 == 0)
            {
                nPage = _totalresult/200;
            }
            else
            {
                nPage = _totalresult/200 + 1;
            }

            #endregion

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
                    //#region Fertige Aufträge löschen
                    if (obj.orderState == "CANCELLED" | obj.orderState == "CLOSED" |
                        obj.orderState == " COMPLETELY_CLOSED")
                    {
                        _boOe = _boO.GetById(obj.id);
                        if (_boOe != null)
                        {
                            _boO.DeleteEntity(_boOe);
                        }
                    }


                    if (obj.orderState == "READY_TO_DISPATCH")
                    {
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
                            _boOe.reference = obj.reference ?? "";

                            // Adressen

                            #region Customer and Invoice

                            // Eigentich brauchen wir nur den BusinessIdentfier 
                            // Später vielleicht zusätzlich (redundant) nehmen wir noch PK und ID

                            //customer
                            _boOe.customerBusinessIdentifier = obj.customer.businessIdentifier ?? "";
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
                            if (obj.invoiceReceiver != null)
                            {
                                _boOe.invoiceReceicerBusinessIdentifier = obj.invoiceReceiver.businessIdentifier != null
                                    ? obj.invoiceReceiver.businessIdentifier
                                    : "";
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
                            }

                            #endregion

                            _boO.SaveEntity(_boOe);
                            _boOe = _boO.GetById(_boOe.id); // Damit ich jetzt den OK habe

                            // *******************************************************************************************************
                            // OrderItemservice
                            //********************************************************************************************************

                            #region orderItemService

                            request = new RestRequest("/rest/order/" + _boOe.id) {Method = Method.GET};
                            request.AddHeader("X-location-Id", location.ToString());


                            response = client.Execute(request);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                OrderEntity oOEntity = JsonConvert.DeserializeObject<OrderEntity>(response.Content);

                                foreach (var orderItem in oOEntity.orderItems)
                                {
                                    // TODO: Fertige Orderitem löschen


                                    //#region Fertige Orderitems löschen
                                    if (orderItem.orderItemState == "CANCELLED" | orderItem.orderItemState == "CLOSED" |
                                        orderItem.orderItemState == " COMPLETELY_CLOSED")
                                    {
                                        _boOis = new OrderItemservice();
                                        _boOise = _boOis.GetByIdentitifier(
                                            orderItem.identifier);
                                        if (_boOise != null)
                                        {
                                            _boOis.DeleteEntity(_boOise);
                                        }
                                    }

                                    if (orderItem.orderItemState == "READY_TO_DISPATCH")
                                    {
                                        foreach (OrderItemService orderItemService in orderItem.orderItemServices)
                                        {
// Order Item Service: Verladen LKW / Leistungs-ID: 2003 / Info: Diese Leistung wird im Produkt WA LKW (8005) übermittelt
//Order Item Service: Abladen LKW / Leistungs ID: 2001/ Info: Diese Leistung wird im Produkt WE LKW (8002) übermittelt 
//Order Item Service: Abladen Schiff / Leistungs ID: 1001/ Info: Diese Leistung wird im Produkt WE Schiff (8000) übermittelt

//Order Item Service: Verladen LKW / Leistungs-ID: 2003 / Info: Diese Leistung wird im Produkt UMS Schiff LKW (3406) übermittelt
//Order Item Service: Abladen LKW / Leistungs-ID: 2001 / Info: Diese Leistung wird im Produkt UMS LKW Schiff (3402) übermittelt

//Order Item Service: Umlagern / Leistungs-ID: 8103 / Info: Diese Leistung wird im Produkt Umlagern (8103; Leistungs-ID = Produkt-ID) übermittelt
//Order Item Service: Fremdverwiegung / Leistungs-ID: 10301/ Info: Diese Leistung wird im Produkt Fremdverwiegung (10301; Leistungs-ID = Produkt-ID) übermittelt
//Order Item Service: Radladerverwiegung / Leistungs-ID: 10300/ Info: Diese Leistung wird im Produkt Radladerverwiegung 10300; Leistungs-ID = Produkt-ID) übermittelt 

// Neu: 24.07.2015
// Filter um 1003 erweitern
// Produkt 8003 Warenausgang Schiff / Leistungsid 1003


// Order Item Service: 

                                            // Filter auf die relevanten Leistungen
                                            if (VFP.InList(orderItemService.service.id, 1003,2003, 2001, 1001, 8103, 10301,
                                                10300))
                                            {
                                                _boOis = new OrderItemservice();

                                                // Gibt es diesen DS schon? Wenn nein, dann anlegen
                                                _boOise = _boOis.GetByIdAndPKOrderItem(_boOe.PK,
                                                    orderItemService.identifier);

                                                if (_boOise == null)
                                                {
                                                    _boOise = _boOis.NewEntity();
                                                    _boOise.HasBinSended = false;
                                                    _boOise.InvisibleSendedOrderItems = false;
                                                    _boOise.HasBinUsed = false;
                                                }

                                                // _boOis.NewEntity();


                                                _boOise.orderstate = orderItem.orderItemState;
                                                _boOise.sequence = orderItem.sequence;

                                                _boOise.serviceId = orderItemService.service.id;
                                                _boOise.serviceDescription = orderItemService.service.description;
                                                _boOise.identifierOItem = orderItem.identifier;
                                                _boOise.identifierOItemService = orderItemService.identifier;
                                                _boOise.remark = orderItemService.remark;

                                                _boOise.actualStorageAreaId = orderItemService.actualStorageAreaId;
                                                _boOise.targetStorageAreaId = orderItemService.targetStorageAreaId;
                                                //  _boOise.IstQuellLagerPlatzId 
                                                _boOise.product = orderItem.product.id;
                                                _boOise.productdescription = orderItem.product.description;
                                                _boOise.serviceId = orderItemService.service.id;


                                                // 14.11.2014 Das entspricht der Schnittstellenbeschreibung
                                                _boOise.productdescription = oOEntity.orderItems[0].product.description;
                                                // 14.11.2014 Das entspricht der Schnittstellenbeschreibung

                                                _boOise.remark = orderItemService.remark;

                                                _boOise.deliveryType = orderItemService.deliveryType;

                                                #region ArticelInstance

                                                if (orderItemService.articleInstance != null)
                                                {
                                                    _boOise.articleId = orderItemService.articleInstance.article.id;
                                                    _boOise.articleDescription =
                                                        orderItemService.articleInstance.article.description ?? "";
                                                    _boOise.number = orderItemService.articleInstance.article.number ??
                                                                     "";
                                                    // Ganz schräg Inhalt zB Testbleche
                                                    _boOise.ownerId = orderItemService.articleInstance.article.ownerId;
                                                    _boOise.kindOfGoodId =
                                                        orderItemService.articleInstance.article.kindOfGoodId;
                                                    _boOise.kindOfGoodDescription =
                                                        orderItemService.articleInstance.article.kindOfGoodDescription;
                                                    if (oOEntity.orderItems[0].plannedDate != null)
                                                    {
                                                        _boOise.plannedDate =
                                                            PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                                oOEntity.orderItems[0].plannedDate);
                                                    }

                                                    AdressenEntity boAe =
                                                        _boA.GetById(orderItemService.articleInstance.article.ownerId);
                                                    if (boAe != null)
                                                        _boOise.ownerBusinessIdentifier = boAe.businessIdentifier;

                                                    #region // service

                                                    _boOise.targedAmount =
                                                        (decimal) orderItemService.service.targedAmount;

                                                    #endregion
                                                }

                                                #endregion

                                                #region Supplier Consignee

                                                if (orderItemService.supplierOrConsignee != null)
                                                {
                                                    _boOise.supplierOrConsigneeId =
                                                        orderItemService.supplierOrConsignee.id;
                                                    _boOise.supplierOrConsigneeBusinessIdentifier =
                                                        orderItemService.supplierOrConsignee.businessIdentifier;
                                                    _boOise.supplierOrConsigneeName =
                                                        orderItemService.supplierOrConsignee.name;
                                                    _boOise.supplierOrConsigneeSubName2 =
                                                        orderItemService.supplierOrConsignee.subName;
                                                    _boOise.supplierOrConsigneeCity =
                                                        orderItemService.supplierOrConsignee.address.city;
                                                    _boOise.supplierOrConsigneeStreet =
                                                        orderItemService.supplierOrConsignee.address.street;
                                                    _boOise.supplierOrConsigneeZipCode =
                                                        orderItemService.supplierOrConsignee.address.zipCode;
                                                    _boOise.supplierOrConsigneedIdCountry =
                                                        orderItemService.supplierOrConsignee.address.country.id;

                                                    _boOise.supplierOrConsigneeIsocodeCountry =
                                                        orderItemService.supplierOrConsignee.address.country.isoCode;
                                                }

                                                #endregion

                                                #region Clearance

                                                // boOISE.clearanceQuantity   = Nicht vorhanden
                                                _boOise.clearanceReferenz = "";
                                                // Damit die abfrage in den Aufträgen funktiniert darf kein NULL in der Freistellungen sein
                                                if (orderItemService.clearance != null)
                                                {
                                                    _boOise.clearanceReferenz = orderItemService.clearance.reference;
                                                    if (orderItemService.clearance.validFrom != null)
                                                    {
                                                        _boOise.clearanceValidFrom =
                                                            PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                                orderItemService.clearance.validFrom);
                                                    }

                                                    if (orderItemService.clearance.validTo != null)
                                                    {
                                                        _boOise.clearanceValidTo =
                                                            PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                                orderItemService.clearance.validTo);
                                                    }
                                                    if (orderItemService.clearance.unit != null)
                                                    {
                                                        _boOise.clearanceUnitId = orderItemService.clearance.unit.id;
                                                        _boOise.clearanceUnitShortDescription =
                                                            orderItemService.clearance.unit.shortDescription;
                                                        _boOise.clearanceDescription =
                                                            orderItemService.clearance.unit.description;
                                                    }

                                                    #endregion

                                                    #region Attribute

                                                    if (orderItemService.articleInstance != null)
                                                    {
                                                        if (orderItemService.articleInstance.attributes != null)
                                                        {
                                                            _boOise.SerialNumber =
                                                                orderItemService.articleInstance.attributes
                                                                    .SERIAL_NUMBER;
                                                            _boOise.batch =
                                                                orderItemService.articleInstance.attributes.BATCH;
                                                            _boOise.orign =
                                                                orderItemService.articleInstance.attributes.ORIGIN;
                                                            _boOise.grade =
                                                                orderItemService.articleInstance.attributes.GRADE;
                                                            _boOise.originalNumber =
                                                                orderItemService.articleInstance.attributes
                                                                    .ORIGINAL_NUMBER;
                                                            _boOise.length =
                                                                orderItemService.articleInstance.attributes.LENGTH;
                                                            _boOise.width =
                                                                orderItemService.articleInstance.attributes.WIDTH;
                                                            _boOise.height =
                                                                orderItemService.articleInstance.attributes.HEIGHT;

                                                            _boOise.storageAreaReference =
                                                                orderItemService.articleInstance.attributes.
                                                                    STORAGE_AREA_REFERENCE;
                                                            _boOise.diameter =
                                                                orderItemService.articleInstance.attributes.DIAMETER;
                                                            _boOise.orignalMarking =
                                                                orderItemService.articleInstance.attributes.
                                                                    ORIGINAL_MARKING;
                                                            _boOise.storageAreaReferenceNumber =
                                                                orderItemService.articleInstance.attributes.
                                                                    STORAGE_AREA_REFERENCE_NUMBER;
                                                            _boOise.dimension =
                                                                orderItemService.articleInstance.attributes.DIMENSION;
                                                        }
                                                    }
                                                }

                                                #endregion

                                                _boOise.PKOrderItem = _boOe.PK;
                                                _boOis.SaveEntity(_boOise);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
            }

            return true;
        }
    }
}