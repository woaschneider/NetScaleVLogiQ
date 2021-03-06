using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.AuftragsImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using OakLeaf.MM.Main.Business;
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

        private Lagerplaetze _boL;
        private LagerplaetzeEntity _boLe;


        private int _totalresult;
        private ImportExportPolos _oIO;
        public ImportAuftraege(ImportExportPolos oIO)
        {
            _oIO = oIO;
        }
        public bool Import(string baseUrl, string location, string url, bool onlyReadyToDispatch)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());
                //  var request = new RestRequest("/rest/order/query/200/1?status=NEW");
               client.Timeout = 300000;
                var request = new RestRequest(url + "200/1") {Method = Method.GET};
                request.AddHeader("X-location-Id", location);
                request.AddHeader("Accept-Language", "de");
                if (onlyReadyToDispatch)
                {
                    request.AddQueryParameter("status", "READY_TO_DISPATCH");
                }
                _boL = new Lagerplaetze();
                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                    boEe.ConsumerSecret.Trim(),
                    string.Empty, string.Empty);

                #region // Anzahl der Order und Seiten ermittlen

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Auftrag-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return false;
                }


                var oOi = JsonConvert.DeserializeObject<RootObject>(response.Content);

                _totalresult = oOi.totalResults;
                int nPage;

                if (_totalresult % 200 == 0)
                {
                    nPage = _totalresult / 200;
                }
                else
                {
                    nPage = _totalresult / 200 + 1;
                }

                #endregion // Anzahl der Order und Seiten ermittlen

                int recordCounter = 0;
                for (int ii = 1; ii <= nPage; ii++)
                {
                    request = new RestRequest("/rest/order/query/200/" + ii) {Method = Method.GET};
                    request.AddHeader("X-location-Id", location);
                    request.AddHeader("Accept-Language", "de");
                    if (onlyReadyToDispatch)
                    {
                        //  request.AddQueryParameter("status", "READY_TO_DISPATCH");
                        request.AddQueryParameter("status",
                            "state = READY_TO_DISPATCH & scaleRelevant = true & scalingProcessed = false");
                    }

                    response = client.Execute(request);
                    if (response.StatusCode != HttpStatusCode.OK)
                        return false;


                    oOi = JsonConvert.DeserializeObject<RootObject>(response.Content);

                    //***********************************************************************
                    foreach (OrderEntity obj in oOi.orders)
                    {
                        try
                        {
                            recordCounter = recordCounter + 1;
                            try
                            {
                                _oIO.ProzentAuftraege = (int) (recordCounter / (_totalresult / 100F));
                            }
                            catch (Exception)
                            {
                                _oIO.ProzentAuftraege = 0;
                            }

                            //#region Fertige Aufträge falls noch vorhanden löschen
                            if ((obj.orderState == "CANCELLED") || (obj.orderState == "CLOSED") ||
                                (obj.orderState == "COMPLETELY_CLOSED") || (obj.orderState == "READY_FOR_BILLING") || (obj.orderState=="COMMITTED" ))
                            {
                                _boOe = _boO.GetById(obj.id);
                                if (_boOe != null)
                                {
                                    _boO.DeleteEntity(_boOe);
                                }
                            }

                            #region Eigentlicher Import

                            if (obj.orderState == "READY_TO_DISPATCH" || obj.orderState == "NEW")
                            {
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
                                        //_boAe = _boA.GetById(obj.customer.id);
                                        _boAe = _boA.GetByBusinenessIdentifier(_boOe.customerBusinessIdentifier);
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
                                        _boOe.invoiceReceicerBusinessIdentifier =
                                            obj.invoiceReceiver.businessIdentifier ?? "";
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

                                    _boOe.touch = true;

                                    mmSaveDataResult mmSaveDataResult;
                                    mmSaveDataResult = _boO.SaveEntity(_boOe);
                                    _boOe = _boO.GetById(_boOe.id); // Damit ich jetzt den OK habe

                                    // *******************************************************************************************************
                                    // OrderItemservice
                                    //********************************************************************************************************

                                    #region orderItemService

                                    request = new RestRequest("/rest/order/" + _boOe.id) {Method = Method.GET};
                                    request.AddHeader("X-location-Id", location);
                                    request.AddHeader("Accept-Language", "de");


                                    response = client.Execute(request);
                                    if (response.StatusCode == HttpStatusCode.OK)
                                    {
                                        OrderEntity oOEntity =
                                            JsonConvert.DeserializeObject<OrderEntity>(response.Content);

                                        foreach (var orderItem in oOEntity.orderItems)
                                        {
                                            // Fertige Orderitem löschen


                                            //#region Fertige Orderitems löschen
                                            if ((orderItem.orderItemState == "CANCELLED") ||
                                                (orderItem.orderItemState == "CLOSED") ||
                                                (orderItem.orderItemState == " COMPLETELY_CLOSED") ||
                                                (obj.orderState == "READY_FOR_BILLING") || (obj.orderState == "COMMITTED"))
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
                                                foreach (
                                                    OrderItemService orderItemService in orderItem.orderItemServices)
                                                {
                                                    // OrderItemsServices die nicht Ready_To_Dispatch sind und die sich trotzdem im Waagenbestand befinden, werden gelöscht
                                                    if (orderItemService.service.scaleRelevant &&
                                                        orderItemService.state != "READY_TO_DISPATCH")
                                                    {
                                                        _boOis = new OrderItemservice();
                                                        _boOise = _boOis.GetByIdAndPKOrderItem(_boOe.PK,
                                                            orderItemService.identifier);
                                                        if (_boOise != null)
                                                        {
                                                            _boOis.DeleteEntity(_boOise);
                                                        }
                                                    }

                                                    if (orderItemService.service.scaleRelevant &&
                                                        orderItemService.state == "READY_TO_DISPATCH")
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


                                                        _boOise.orderstate = orderItem.orderItemState;
                                                        _boOise.sequenceOItem = orderItem.sequence;
                                                        _boOise.sequenceOItemService = orderItemService.sequence;
                                                        _boOise.serviceId = orderItemService.service.id;
                                                        _boOise.serviceDescription =
                                                            orderItemService.service.description;
                                                        _boOise.identifierOItem = orderItem.identifier;
                                                        _boOise.identifierOItemService = orderItemService.identifier;
                                                        _boOise.remark = orderItemService.remark;

                                                        if (orderItemService.actualStorageAreaId != null)
                                                        {
                                                            _boOise.actualStorageAreaId =
                                                                orderItemService.actualStorageAreaId;
                                                            _boOise.actualStorageAreaName =
                                                                _boL.GetById(_boOise.actualStorageAreaId).fullname;
                                                        }
                                                        _boOise.targetStorageAreaId =
                                                            orderItemService.targetStorageAreaId;
                                                        //  _boOise.IstQuellLagerPlatzId 
                                                        _boOise.product = orderItem.product.id;
                                                        if (orderItem.product.description != null)
                                                        {
                                                            _boOise.productdescription = orderItem.product.description.Trim();
                                                        }
                                                        _boOise.serviceId = orderItemService.service.id;


                                                        // 14.11.2014 Das entspricht der Schnittstellenbeschreibung
                                                        //_boOise.productdescription =
                                                        //    oOEntity.orderItems[0].product.description;
                                                        // 14.11.2014 Das entspricht der Schnittstellenbeschreibung

                                                        _boOise.remark = orderItemService.remark;

                                                        _boOise.deliveryType = orderItemService.deliveryType;
                                                        _boOise.vehicle = orderItemService.vehicle;
                                                        _boOise.vehicle2 = orderItemService.vehicle2;

                                                        #region PlanningDivision

                                                        if (orderItemService.planningDivision != null)
                                                        {
                                                            _boOise.PlanningDivisionId =
                                                                orderItemService.planningDivision.id;
                                                            _boOise.PlanningDivisionDescription =
                                                                orderItemService.planningDivision.description;
                                                            _boOise.PlanningDivisionActive =
                                                                orderItemService.planningDivision.active;
                                                        }

                                                        #endregion

                                                        #region ArticelInstance

                                                        if (orderItemService.articleInstance != null)
                                                        {
                                                            _boOise.articleId =
                                                                orderItemService.articleInstance.article.id;
                                                            _boOise.articleDescription =
                                                                orderItemService.articleInstance.article.description ??
                                                                "";
                                                            _boOise.number =
                                                                orderItemService.articleInstance.article.number ??
                                                                "";
                                                            // Ganz schräg Inhalt zB Testbleche
                                                            _boOise.ownerId =
                                                                orderItemService.articleInstance.article.ownerId;
                                                            _boOise.kindOfGoodId =
                                                                orderItemService.articleInstance.article.kindOfGoodId;
                                                            _boOise.kindOfGoodDescription =
                                                                orderItemService.articleInstance.article
                                                                    .kindOfGoodDescription;
                                                            if (oOEntity.orderItems[0].plannedDate != null)
                                                            {
                                                                _boOise.plannedDate =
                                                                    PolosUtitlities.ConvertPolosDateTime2DateTime(
                                                                        oOEntity.orderItems[0].plannedDate);
                                                            }

                                                            AdressenEntity boAe =
                                                                _boA.GetById(
                                                                    orderItemService.articleInstance.article.ownerId);
                                                            if (boAe != null)
                                                                _boOise.ownerBusinessIdentifier =
                                                                    boAe.businessIdentifier;

                                                            #region // service

                                                            _boOise.targedAmount =
                                                                (decimal) orderItemService.targetAmount;

                                                            #region Conversionunit

                                                            if (orderItemService.articleInstance.article.conversionUnit !=
                                                                null)
                                                            {
                                                                _boOise.conversionUnitId =
                                                                    orderItemService.articleInstance.article.conversionUnit
                                                                        .id;
                                                                _boOise.conversionUnitDescription =
                                                                    orderItemService.articleInstance.article.conversionUnit
                                                                        .description;
                                                                _boOise.conversionUnitShortDescription =
                                                                    orderItemService.articleInstance.article.conversionUnit
                                                                        .shortDescription;
                                                            }

                                                            #endregion

                                                            #endregion
                                                        }



                                                        #endregion

                                                        #region Supplier Receiver  Freight Carrier

                                                        if (orderItemService.supplier != null)
                                                        {
                                                            _boOise.supplierId =
                                                                orderItemService.supplier.id;
                                                            _boOise.supplierBusinessIdentifier =
                                                                orderItemService.supplier.businessIdentifier;
                                                            _boOise.supplierName =
                                                                orderItemService.supplier.name;
                                                            _boOise.supplierSubName2 =
                                                                orderItemService.supplier.subName;
                                                            _boOise.supplierCity =
                                                                orderItemService.supplier.address.city;
                                                            _boOise.supplierStreet =
                                                                orderItemService.supplier.address.street;
                                                            _boOise.supplierZipCode =
                                                                orderItemService.supplier.address.zipCode;
                                                            _boOise.supplierIdCountry =
                                                                orderItemService.supplier.address.country.id;
                                                            _boOise.supplierIsocodeCountry =
                                                                orderItemService.supplier.address.country.isoCode;
                                                        }

                                                        // Falls doch beides kommen kann müssen wir hier noch nachbessern
                                                        if (orderItemService.receiver != null)
                                                        {
                                                            _boOise.receiverId =
                                                                orderItemService.receiver.id;
                                                            _boOise.receiverBusinessIdentifier =
                                                                orderItemService.receiver.businessIdentifier;
                                                            _boOise.receiverName =
                                                                orderItemService.receiver.name;
                                                            _boOise.receiverSubName2 =
                                                                orderItemService.receiver.subName;
                                                            _boOise.receiverCity =
                                                                orderItemService.receiver.address.city;
                                                            _boOise.receiverStreet =
                                                                orderItemService.receiver.address.street;
                                                            _boOise.receiverZipCode =
                                                                orderItemService.receiver.address.zipCode;
                                                            _boOise.receiverIdCountry =
                                                                orderItemService.receiver.address.country.id;
                                                            _boOise.receiverIsocodeCountry =
                                                                orderItemService.receiver.address.country.isoCode;
                                                        }

                                                        // Frachtführer / Frachtführer
                                                        if (orderItemService.freightCarrier != null)
                                                        {
                                                           
                                                            _boOise.ffId = orderItemService.freightCarrier.id;
                                                            _boOise.ffBusinessIdentifier =
                                                                orderItemService.freightCarrier.businessIdentifier;
                                                            _boOise.ffName =
                                                                orderItemService.freightCarrier.name;
                                                            _boOise.ffSubName2 =
                                                                orderItemService.freightCarrier.subName;
                                                            _boOise.ffCity =
                                                                orderItemService.freightCarrier.address.city;
                                                            _boOise.ffStreet =
                                                                orderItemService.freightCarrier.address.street;
                                                            _boOise.ffZipCode =
                                                                orderItemService.freightCarrier.address.zipCode;
                                                            _boOise.ffIdCountry =
                                                                orderItemService.freightCarrier.address.country.id;
                                                            _boOise.ffIsocodeCountry =
                                                                orderItemService.freightCarrier.address.country.isoCode;
                                                        }

                                                        #endregion

                                                        #region Clearance

                                                        // boOISE.clearanceQuantity   = Nicht vorhanden
                                                        _boOise.clearanceReferenz = "";
                                                        // Damit die abfrage in den Aufträgen funktiniert darf kein NULL in der Freistellungen sein
                                                        if (orderItemService.clearance != null)
                                                        {
                                                            _boOise.clearanceReferenz =
                                                                orderItemService.clearance.reference;
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
                                                                _boOise.clearanceUnitId =
                                                                    orderItemService.clearance.unit.id;
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
                                                                        orderItemService.articleInstance.attributes
                                                                            .BATCH;
                                                                    _boOise.orign =
                                                                        orderItemService.articleInstance.attributes
                                                                            .ORIGIN;
                                                                    _boOise.grade =
                                                                        orderItemService.articleInstance.attributes
                                                                            .GRADE;
                                                                    _boOise.originalNumber =
                                                                        orderItemService.articleInstance.attributes
                                                                            .ORIGINAL_NUMBER;
                                                                    _boOise.length =
                                                                        orderItemService.articleInstance.attributes
                                                                            .LENGTH;
                                                                    _boOise.width =
                                                                        orderItemService.articleInstance.attributes
                                                                            .WIDTH;
                                                                    _boOise.height =
                                                                        orderItemService.articleInstance.attributes
                                                                            .HEIGHT;

                                                                    _boOise.storageAreaReference =
                                                                        orderItemService.articleInstance.attributes.
                                                                            STORAGE_AREA_REFERENCE;
                                                                    _boOise.diameter =
                                                                        orderItemService.articleInstance.attributes
                                                                            .DIAMETER;
                                                                    _boOise.orignalMarking =
                                                                        orderItemService.articleInstance.attributes.
                                                                            ORIGINAL_MARKING;
                                                                    _boOise.storageAreaReferenceNumber =
                                                                        orderItemService.articleInstance.attributes.
                                                                            STORAGE_AREA_REFERENCE_NUMBER;
                                                                    _boOise.dimension =
                                                                        orderItemService.articleInstance.attributes
                                                                            .DIMENSION;
                                                                }
                                                            }
                                                        }

                                                        #endregion

                                                        try
                                                        {
                                                            _boOise.PKOrderItem = _boOe.PK;
                                                            _boOis.SaveEntity(_boOise);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Instance.Info("Nummer:" + _boOe.number + " /  " + _boOise.sequenceOItemService);
                                                            Log.Instance.Error("Fehler im Auftrags-Import: " + e.Message +" " + e.InnerException + " " + e.Source +" " +e.StackTrace);
                                                        }
                                                        
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                        catch (Exception ee)
                        {  Log.Instance.Info("Nummer:" + _boOe.number +" /  " +_boOise.sequenceOItemService);
                            Log.Instance.Error("Fehler im Auftrags-Import: " + ee.Message+ " "+ ee.InnerException + " " + ee.Source + " " + ee.StackTrace);
                            return false;
                        }
                    }

                    #endregion
                }
                _boO.DeleteNotTouch();
                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error("Fehler im Auftrags-Import: " + e.Message +" "+e.InnerException+" "+e.Source + " " + e.StackTrace);
                return false;
            }
        }
    }
}