using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Net;
using System.Windows.Controls;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO;
using Newtonsoft.Json;
using OakLeaf.MM.Main.Collections;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace NetScalePolosIO.Export
{
    




    public class ExportWaegungVersion2Rest
    {
        public void ExportLs2Rest(string baseUrl, string url, int? location, WaegeEntity boWe)
        {
            // Diese Prüfung reicht nicht! Das muss angepaßt werden. 
            if (boWe.identifierOItem != null)
            {
                ExportExistingOrder2Rest(baseUrl, url, location, boWe);
            }
            else // Kein Auftrag vorhanden
            {
                // Lege OrderItemObject an
                RootObject oOI = CreateOrderItem(boWe, location);
                // 3. Die Id in die WaegeEntiy füllen (boWe.identifierOItemService)
                boWe.identifierOItemService = GetIdentifierOItemService(oOI, boWe, baseUrl).ToString();
                if (boWe.identifierOItemService != "0") // = bedeutet die Auftragsanlage schlug fehl
                {   Waege boW = new Waege();
                    boW.SaveEntity(boWe); // Mal schauen ob das  so klappt.
                    ExportExistingOrder2Rest(baseUrl, url, location, boWe);
                }
            }
        }

        private RootObject CreateOrderItem(WaegeEntity w, int? location)
        {
            NetScalePolosIO.Export.RootObject oOI = new RootObject();

            // RootObject *********************************************************************
            //TODO: Frankatur / Incoterm
            oOI.reference = "FREE";
            oOI.orderState = "NEW";
         
            oOI.locationId = location.ToString();
            oOI.date = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";

            // Customer
            oOI.customer = new Customer();
            oOI.customer.id = w.customerId;
            //Invocie Receiver
            oOI.invoiceReceiver = new InvoiceReceiver();
            oOI.invoiceReceiver.id = w.invoiceReceiverId;


            // OrderItems**********************************************************************

            // Ein OrderItem Object erzeugen
            oOI.orderItems = new List<OrderItem>();
            oOI.orderItems.Add(new OrderItem());


            oOI.orderItems[0].orderItemState = "NEW";
            oOI.orderItems[0].plannedDate = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";

            oOI.orderItems[0].product = new Product();
            oOI.orderItems[0].product.id = w.productid;


            // Wieviele Leistungen hat das Produkt 
            Serv boS = new Serv();
            mmBindingList<ServEntity> boSe = boS.GetAllByProduktId(w.productid);
            if (boSe.Count > 0)
            {
                oOI.orderItems[0].orderItemServices = new List<OrderItemService>();
                for (int i = 0; i <= boSe.Count - 1; i++)
                {
                    

                    oOI.orderItems[0].orderItemServices.Add(new OrderItemService());
                    oOI.orderItems[0].orderItemServices[i].remark = "NO ORDER";
                    oOI.orderItems[0].orderItemServices[i].state = "NEW";
                    oOI.orderItems[0].orderItemServices[i].targetAmount = 40000; // Muss das sein ?
                    oOI.orderItems[0].orderItemServices[i].plannedBeginDate = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";
                    oOI.orderItems[0].orderItemServices[i].plannedEndDate = string.Format("{0:yyyyMMddHHmmss}", w.zweitDateTime) + "000";
                    oOI.orderItems[0].orderItemServices[i].deliveryType = "FREE"; // TODO auf Frankatur / Incoterm umstellen

                    // ArtikelInstance
                    oOI.orderItems[0].orderItemServices[i].articleInstance= new ArticleInstance();
                    oOI.orderItems[0].orderItemServices[i].articleInstance.article = new Article();
                    oOI.orderItems[0].orderItemServices[i].articleInstance.article.id = w.articleId;
                    oOI.orderItems[0].orderItemServices[i].articleInstance.attributes = new Attributes(); // Warum auch immer muss hier ein leeres Objekt erzeugt werden ?
                    oOI.orderItems[0].orderItemServices[i].supplierOrConsignee= new SupplierOrConsignee();
                    oOI.orderItems[0].orderItemServices[i].supplierOrConsignee.id = w.supplierOrConsigneeId;


                    // Service
                    oOI.orderItems[0].orderItemServices[i].service = new Service();
                    oOI.orderItems[0].orderItemServices[i].service.id = boSe[i].id;
                }
            }


            //
            return oOI;
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

            var obj = JsonConvert.SerializeObject(oOi);
            request.AddParameter("application/json; charset=utf-8", obj, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                boEe.ConsumerSecret.Trim(),
                string.Empty, string.Empty);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteToExportLog(response, boWe);

                return 0;
            }


            int OrdItemServiceId = 0;
            return OrdItemServiceId;
        }


        // Wenn Auftrag vorliegt
        private bool ExportExistingOrder2Rest(string baseUrl, string url, int? location, WaegeEntity boWe)
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            #region JSON-Polos Struktur aufbauen

            var oWEx2 = new RootObject2();
            oWEx2.scalePhaseData = new ScalePhaseData();
            oWEx2.scalePhaseData.FIRST = new FIRST();
            oWEx2.scalePhaseData.SECOND = new SECOND();

            oWEx2.orderItemServiceId = boWe.identifierOItemService;
            // oWEx2.carrierName = boWe.ffBusinessIdentifier;
            oWEx2.carrierName = boWe.ffSubName2;
            oWEx2.carrierVehicle = boWe.Fahrzeug;
            oWEx2.storageAreaId = "0"; // boWe.storageAreaReferenceNumber;
            oWEx2.scaleNoteNumber = boWe.LieferscheinNr;

            oWEx2.scalePhaseData.FIRST.scaleId = "1";
            if (boWe.LN1 != null)
                oWEx2.scalePhaseData.FIRST.scaleNumber = boWe.LN1.Trim();

            if (boWe.Erstgewicht != null)
                oWEx2.scalePhaseData.FIRST.amount = ((int) (boWe.Erstgewicht));

            oWEx2.scalePhaseData.FIRST.date = string.Format("{0:yyyyMMddHHmmss}", boWe.ErstDatetime) + "000";
            oWEx2.scalePhaseData.SECOND.scaleId = "1";

            if (boWe.LN2 != null)
                oWEx2.scalePhaseData.SECOND.scaleNumber = boWe.LN2.Trim();

            if (boWe.Zweitgewicht != null)
                oWEx2.scalePhaseData.SECOND.amount = ((int) (boWe.Zweitgewicht));

            oWEx2.scalePhaseData.SECOND.date = string.Format("{0:yyyyMMddHHmmss}", boWe.zweitDateTime) + "000";

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

                    return false;
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
                new WriteErrorLog().WriteToErrorLog(ee);
                return false;
            }


            return false;
        }

       

        private static void SetOrderItemServiceAsSend(WaegeEntity we)
        {
            OrderItemservice oIE = new OrderItemservice();
            OrderItemserviceEntity oIES = oIE.GetByPK(we.PK);
            if (oIES != null)
            {
                oIES.HasBinSended = true;
                oIE.SaveEntity(oIES);
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
                boEe.Message2 = oR.message;
            }
            else
            {
                boEe.Message1 = response.ErrorException.Message;
            }
            boEe.OrderItemNumber = we.number;
            boEe.OrderItemServiceIdentifier = we.identifierOItemService;
            boE.SaveEntity(boEe);
          

            }
            catch (Exception e)
            {
                
                new WriteErrorLog().WriteToErrorLog(e);
            }
        }
    }

    #endregion
}