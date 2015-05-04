using System;
using System.Collections.Generic;

using System.Net;

using HWB.NETSCALE.BOEF;

using Newtonsoft.Json;
using OakLeaf.MM.Main.Collections;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Extensions;


namespace NetScalePolosIO.Export
{
    




    public class ExportWaegungVersion2Rest
    {
        public void ExportLs2Rest(string baseUrl, int? location, WaegeEntity boWe)
        {
            // Diese Prüfung reicht nicht! Das muss angepaßt werden. 
            if (boWe.identifierOItem != null)
            {
                ExportExistingOrder2Rest(baseUrl, boWe);
            }
            else // Kein Auftrag vorhanden
            {
                if (boWe.productid != null)
                {
                    // Lege OrderItemObject an
                    RootObject oOi = CreateOrderItem(boWe, location);
                    // 3. Die Id in die WaegeEntiy füllen (boWe.identifierOItemService)
                    boWe.identifierOItemService = GetIdentifierOItemService(oOi, boWe, baseUrl).ToString();
                    if (boWe.identifierOItemService != "0") // = bedeutet die Auftragsanlage schlug fehl
                    {
                        Waege boW = new Waege();
                        // boW.SaveEntity(boWe); // Mal schauen ob das  so klappt.
                        ExportExistingOrder2Rest(baseUrl, boWe);
                    }
                }
            }
        }

        private RootObject CreateOrderItem(WaegeEntity w, int? location)
        {
            var properties = new Dictionary<string, object>(); // Neu 28.4.15
            RootObject oOi = new RootObject();

            // RootObject *********************************************************************
            //TODO: Frankatur / Incoterm
            oOi.reference = "FREE";
            oOi.orderState = "READY_TO_DISPATCH";
         
            oOi.locationId = location.ToString();
            oOi.date = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";

            // Customer
            oOi.customer = new Customer();
            oOi.customer.id = w.customerId;
            //Invocie Receiver
            oOi.invoiceReceiver = new InvoiceReceiver();
            oOi.invoiceReceiver.id = w.invoiceReceiverId;


            // OrderItems**********************************************************************

            // Ein OrderItem Object erzeugen
            oOi.orderItems = new List<OrderItem>();
            oOi.orderItems.Add(new OrderItem());


            oOi.orderItems[0].orderItemState = "READY_TO_DISPATCH";
            oOi.orderItems[0].plannedDate = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";

            oOi.orderItems[0].product = new Product();
            oOi.orderItems[0].product.id = w.productid;

            oOi.orderItems[0].clearance = new Clearance();
            oOi.orderItems[0].clearance.reference = w.clearanceReferenz;
            oOi.orderItems[0].clearance.authorizerId = w.customerId;
            oOi.orderItems[0].clearance.granteeId = w.supplierOrConsigneeId;
            oOi.orderItems[0].clearance.active = "false";
            oOi.orderItems[0].clearance.validFrom= string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";
            oOi.orderItems[0].clearance.validTo = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";

            //

            // Wieviele Leistungen hat das Produkt 
            Serv boS = new Serv();
            mmBindingList<ServEntity> boSe = boS.GetAllByProduktId(w.productid);
            if (boSe.Count > 0)
            {
                oOi.orderItems[0].orderItemServices = new List<OrderItemService>();
                for (int i = 0; i <= boSe.Count - 1; i++)
                {
                    

                    oOi.orderItems[0].orderItemServices.Add(new OrderItemService());
                    oOi.orderItems[0].orderItemServices[i].remark = "NO ORDER";
                    oOi.orderItems[0].orderItemServices[i].state = "READY_TO_DISPATCH";
                    oOi.orderItems[0].orderItemServices[i].targetAmount = 0; // Muss das sein ?
                    oOi.orderItems[0].orderItemServices[i].plannedBeginDate = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";
                    oOi.orderItems[0].orderItemServices[i].plannedEndDate = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";
                    oOi.orderItems[0].orderItemServices[i].deliveryType = "FREE"; // TODO auf Frankatur / Incoterm umstellen

                    // ArtikelInstance
                    if (w.articleId != null)
                    {
                        oOi.orderItems[0].orderItemServices[i].articleInstance = new ArticleInstance();
                        oOi.orderItems[0].orderItemServices[i].articleInstance.article = new Article();
                        oOi.orderItems[0].orderItemServices[i].articleInstance.article.id = w.articleId;
                        oOi.orderItems[0].orderItemServices[i].articleInstance.attributes = new Attributes(); // Warum auch immer muss hier ein leeres Objekt erzeugt werden ?
                    }
                  
                    oOi.orderItems[0].orderItemServices[i].supplierOrConsignee= new SupplierOrConsignee();
                    oOi.orderItems[0].orderItemServices[i].supplierOrConsignee.id = w.supplierOrConsigneeId;

                    // Neu 28.4.15
                    if (w.productdescription.Trim() == "Fremdverwiegung")
                    {

                        oOi.orderItems[0].remark = "AU: " + (string.IsNullOrEmpty(w.customerName) ? "": w.customerName.Trim())+","+
                                                            (string.IsNullOrEmpty(w.customerSubName2) ? "" : w.customerSubName2.Trim()) + ","+
                                                            (string.IsNullOrEmpty(w.customerStreet) ? "" : w.customerStreet.Trim()) + ","+
                                                            (string.IsNullOrEmpty(w.customerZipCode) ? "" : w.customerZipCode.Trim()) + ","+
                                                            (string.IsNullOrEmpty(w.customerCity) ? "" : w.customerCity.Trim()) +","+


                                                             "RE: " + (string.IsNullOrEmpty(w.invoiceReceiverName) ? "" : w.invoiceReceiverName.Trim()) + "," +
                                                            (string.IsNullOrEmpty(w.invoiceReceiverSubName2) ? "" : w.invoiceReceiverSubName2.Trim()) + "," +
                                                            (string.IsNullOrEmpty(w.invoiceReceiverStreet) ? "" : w.invoiceReceiverStreet.Trim()) + "," +
                                                            (string.IsNullOrEmpty(w.invoiceReceiverZipCode) ? "" : w.invoiceReceiverZipCode.Trim()) + "," +
                                                            (string.IsNullOrEmpty(w.invoiceReceiverCity) ? "" : w.invoiceReceiverCity.Trim())+" "+

                            "FF: " + (string.IsNullOrEmpty(w.ffName) ? "" : w.ffName.Trim()) + "," +
                      (string.IsNullOrEmpty(w.ffSubName2) ? "" : w.ffSubName2.Trim()) + "," +
                      (string.IsNullOrEmpty(w.ffStreet) ? "" : w.ffStreet.Trim()) + "," +
                      (string.IsNullOrEmpty(w.ffZipCode) ? "" : w.ffZipCode.Trim()) + "," +
                      (string.IsNullOrEmpty(w.ffCity) ? "" : w.ffCity.Trim());



                        oOi.orderItems[0].orderItemServices[i].kindOfGoodId = w.kindOfGoodId.ToString();
                    }

                    // Service
                    oOi.orderItems[0].orderItemServices[i].service = new Service();
                    oOi.orderItems[0].orderItemServices[i].service.id = boSe[i].id;
                }
            }


            //
            return oOi;
        }

        private int GetIdentifierOItemService(RootObject oOi, WaegeEntity boWe, string baseUrl)
        {
            // Das ist hier ein wenig kompliziert
            // Nachdem wir die Auftragshülle mit CreateOrderItem erstellt und mit den  entsprechenden Ids gefüllt haben:
            // 1. Senden der Auftragshülle an Polos
            // 2. Ausfiltern der relevanten Arbeitsleistung (service id) an Hand der Filtertabelle
            // 3. Dann die die entsprechende OrdItemServiceId raussuchen und zurückliefern


            // Schritt 1 Senden der Auftragshülle an Polos
            // 
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();
            var client = new RestClient(baseUrl);

            client.ClearHandlers();
            var request = new RestRequest("/rest/order") {Method = Method.PUT};
            request.AddHeader("X-location-Id", boEe.RestLocation.ToString());
            request.RequestFormat = DataFormat.Json;

            var obj = JsonConvert.SerializeObject(oOi, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json; charset=utf-8", obj, ParameterType.RequestBody);

            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer ());
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                boEe.ConsumerSecret.Trim(),
                string.Empty, string.Empty);



            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteToExportLog(response, boWe);

                return 0;
            }

            var oO = JsonConvert.DeserializeObject<RootObject>(response.Content);
            int ordItemServiceId = 0;
             Arbeitsleistungsfilter boA = new Arbeitsleistungsfilter();
            int? Arbeitsleistung = boA.GetServiceByProduct(oO.orderItems[0].product.id);

            if (Arbeitsleistung != null)
            {
                foreach (OrderItemService ois in oO.orderItems[0].orderItemServices)
                {
                    if (ois.service.id == Arbeitsleistung)
                    {
                        ordItemServiceId = ois.identifier;
                    }
                }
            }


           
            return ordItemServiceId;
        }

        // Wenn Auftrag vorliegt
        private void ExportExistingOrder2Rest(string baseUrl, WaegeEntity boWe)
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            #region JSON-Polos Struktur aufbauen

            var oWEx2 = new RootObject2();
           

            oWEx2.orderItemServiceId = boWe.identifierOItemService;
            // oWEx2.carrierName = boWe.ffBusinessIdentifier;
            oWEx2.carrierId = boWe.ffId;
            oWEx2.articleId = boWe.articleId;
            oWEx2.carrierName = boWe.ffSubName2;
            oWEx2.carrierVehicle = boWe.Fahrzeug;
            oWEx2.netAmount = (double)boWe.Nettogewicht;

            if (!string.IsNullOrEmpty(boWe.IstQuellLagerPlatzId))
            {
                oWEx2.storageAreaId = boWe.IstQuellLagerPlatzId; // Panko 04.03.2015;
            }
            
           
            oWEx2.scaleNoteNumber = boWe.LieferscheinNr;


            if (boWe.Erstgewicht > 0 | boWe.Zweitgewicht > 0)
            {
                oWEx2.scalePhaseData = new ScalePhaseData();
                if (boWe.Erstgewicht > 0)
                {
                    oWEx2.scalePhaseData.FIRST = new FIRST();
                    oWEx2.scalePhaseData.FIRST.scaleId = "1";
                    if (boWe.LN1 != null)
                        oWEx2.scalePhaseData.FIRST.scaleNumber = boWe.LN1.Trim();

                    if (boWe.Erstgewicht != null)
                        oWEx2.scalePhaseData.FIRST.amount = (double)boWe.Erstgewicht;

                    if (boWe.ErstDatetime != null)
                    {
                        oWEx2.scalePhaseData.FIRST.date = string.Format("{0:yyyyMMddHHmmss}", boWe.ErstDatetime) + "000";
                    }
                    else
                    {
                        oWEx2.scalePhaseData.FIRST.date = string.Format("{0:yyyyMMddHHmmss}", boWe.LSDatum) + "000";
                    }
                    



                }



                if (boWe.Zweitgewicht > 0)
                {
                    oWEx2.scalePhaseData.SECOND = new SECOND();
                    oWEx2.scalePhaseData.SECOND.scaleId = "1";
                    if (boWe.LN2 != null)
                        oWEx2.scalePhaseData.SECOND.scaleNumber = boWe.LN2.Trim();

                    if (boWe.Zweitgewicht != null)
                        oWEx2.scalePhaseData.SECOND.amount = (double)boWe.Zweitgewicht;

                    if (boWe.zweitDateTime != null)
                    {
                        oWEx2.scalePhaseData.SECOND.date = string.Format("{0:yyyyMMddHHmmss}", boWe.zweitDateTime) +
                                                           "000";
                    }
                    else
                    {
                        oWEx2.scalePhaseData.SECOND.date = string.Format("{0:yyyyMMddHHmmss}", boWe.LSDatum) +
                                                             "000";  
                    }
                }
            }

          

      
          

            #endregion

            #region REST ExportAll

            try
            {
                var client = new RestClient(baseUrl);
             
                client.ClearHandlers();

             


                var request = new RestRequest("/rest/scale/set") {Method = Method.POST};
           
                request.AddHeader("X-location-Id", boEe.RestLocation.ToString());
                request.RequestFormat = DataFormat.Json;

                //  var obj = request.JsonSerializer.Serialize(oWEx2);
                var obj = JsonConvert.SerializeObject(oWEx2);
             

                request.AddParameter("application/json; charset=utf-8", obj, ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;
           
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                    boEe.ConsumerSecret.Trim(),
                    string.Empty, string.Empty);

                var response = client.Execute(request);

                //TODO:ExportAll Fehlschläge loggen - Erfolgreiche unvisible setzen
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteToExportLog(response, boWe);

                    return;
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Waege w = new Waege();
                    WaegeEntity we = w.GetWaegungByPk((boWe.PK));
                    if (we != null)
                    {
                        we.taab = true;
                        we.HasBinSended = true;
                        //   we.HasBinSendedDateTime = DateTime.Today;
                        w.SaveEntity(we);
                        SetOrderItemServiceAsSend(we);
                        WriteToExportLog(response, boWe);
                    }
                }


                WriteToExportLog(response, boWe);
            }
            catch (Exception ee)
            {
                new WriteErrorLog().WriteToErrorLog(ee,boWe);
            }
        }

        private static void SetOrderItemServiceAsSend(WaegeEntity we)
        {
            OrderItemservice oIe = new OrderItemservice();
            OrderItemserviceEntity oIes = oIe.GetByPK(we.PK);
            if (oIes != null)
            {
                oIes.HasBinSended = true;
                oIe.SaveEntity(oIes);
            }
        }

        private void WriteToExportLog(IRestResponse response, WaegeEntity we)
        {
            try
            {
           
            var oR = JsonConvert.DeserializeObject<RestServerError>(response.Content);

            ExportLog boE = new ExportLog();
            ExportLogEntity boEe = boE.NewEntity();
            boEe.dt = DateTime.Now;
            if (oR != null)
            {
                boEe.Message1 = oR.statusCode;
                boEe.Message2 = oR.additionalInformation;
                boEe.Message3= we.LieferscheinNr;
            }
            else
            {
                boEe.Message3 = we.LieferscheinNr;
                boEe.Message2 = "ResponseStatus:" + response.ResponseStatus;
                boEe.Message1 ="Response Error Exception: "+ response.ErrorException.Message;
            }
            boEe.OrderItemNumber = we.number;
            boEe.OrderItemServiceIdentifier = we.identifierOItemService;
            boE.SaveEntity(boEe);
          

            }
            catch (Exception e)
            {
                
                new WriteErrorLog().WriteToErrorLog(e,we);
            }
        }
    }

    #endregion
}