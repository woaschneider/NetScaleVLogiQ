using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;


namespace NetScalePolosIO.Export
{
    public class ExportWaegungVersion2Rest
    {
        public void ExportLs2Rest(string baseUrl, string location, WaegeEntity _boWe)
        {
            // Neu 30.8.2015 die Waegeentitaet trennen

            int waegePk = _boWe.PK;
            Waege boW = new Waege();

            WaegeEntity boWe = boW.GetWaegungByPk(waegePk);
            if (boWe == null)
                return;


            Export2Rest(baseUrl, boWe);
        }


        private void Export2Rest(string baseUrl, WaegeEntity boWe)
        {
            Einstellungen boE = new Einstellungen();
            EinstellungenEntity boEe = boE.GetEinstellungen();

            #region JSON-Polos Struktur aufbauen

            var oWEx2 = new RootObject2
            {
                orderItemServiceId = boWe.identifierOItemService,
                carrierBusinessIdentifier = boWe.ffBusinessIdentifier,
                carrierVehicle = boWe.Fahrzeug
            };


            if (!string.IsNullOrEmpty(boWe.IstQuellLagerPlatzId))
            {
                oWEx2.storageAreaId = boWe.IstQuellLagerPlatzId; // Panko 04.03.2015;
            }

            oWEx2.scaleNoteNumber = boWe.LieferscheinNr;
            oWEx2.netAmount = boWe.Nettogewicht;
            oWEx2.additionalAmount = boWe.conversionUnitAmount;

            oWEx2.customerBusinessIdentifier = boWe.customerBusinessIdentifier;


            // Artikel       
            oWEx2.articleInstance = new ArticleInstance {article = new Article {id = boWe.articleId}};

            #region Artikelattribute

            JObject attObj;
            if (boWe.attributes_as_json != null )
            {
                try
                {
                    attObj = JObject.Parse(boWe.attributes_as_json);
                }
                catch (Exception e)
                {
                   attObj = null;
                }
              

                if (attObj != null) 
                {
                    oWEx2.articleInstance.attributes = new ArticleAttribute();

                    int counter = attObj.Count;

                    string[] att = new string[counter];
                    counter = 0;
                    foreach (var pair in attObj)
                    {
                        string propName = pair.Key;
                        string propValue = pair.Value.ToString();
                        string propDataTyp = "string"; // Default


                        switch (propName)
                        {
                            case "SERIAL_NUMBER":
                                oWEx2.articleInstance.attributes.SERIAL_NUMBER = propValue;
                                break;
                            case "BARCODE":
                                oWEx2.articleInstance.attributes.BARCODE = propValue;
                                break;
                            case "BATCH":
                                oWEx2.articleInstance.attributes.BATCH = propValue;
                                break;
                            case "ORIGIN":
                                oWEx2.articleInstance.attributes.ORIGN = propValue;
                                break;
                            case "GRADE":
                                oWEx2.articleInstance.attributes.GRADE = propValue;
                                break;
                            case "ORIGINAL_NUMBER":
                                oWEx2.articleInstance.attributes.ORIGINAL_NUMBER = propValue;
                                break;
                            case "ORIGINAL_MARKING":
                                oWEx2.articleInstance.attributes.ORIGINAL_MARKING = propValue;
                                break;
                            case "LENGTH":
                                oWEx2.articleInstance.attributes.LENGTH = propValue;
                                break;
                            case "WIDTH":
                                oWEx2.articleInstance.attributes.WIDTH = propValue;
                                break;
                            case "HEIGHT":
                                oWEx2.articleInstance.attributes.HEIGHT = propValue;
                                break;
                            case "DIMENSION":
                                oWEx2.articleInstance.attributes.DIMENSION = propValue;
                                break;
                            case "STORAGE_AREA_REFERENCE":
                                oWEx2.articleInstance.attributes.STORAGE_AREA_REFERENCE = propValue;
                                break;
                            case "STORAGE_AREA_REFERENCE_NUMBER":
                                oWEx2.articleInstance.attributes.STORAGE_AREA_REFERENCE_NUMBER = propValue;
                                break;
                            case "DIAMETER":
                                oWEx2.articleInstance.attributes.DIAMETER = propValue;
                                break;
                        }


                        counter = counter + 1;
                    }
                }
            }

            #endregion

            if (boWe.Erstgewicht > 0 | boWe.Zweitgewicht > 0)
            {
                #region Scale Data 1

                oWEx2.scalePhaseData = new ScalePhaseData();
                if (boWe.Erstgewicht > 0)
                {
                    oWEx2.scalePhaseData.FIRST = new FIRST {scaleId = "1"};
                    if (boWe.LN1 != null)
                        oWEx2.scalePhaseData.FIRST.scaleNumber = boWe.LN1.Trim();

                    if (boWe.Erstgewicht != null)
                        oWEx2.scalePhaseData.FIRST.amount = boWe.Erstgewicht;

                    if (boWe.ErstDatetime != null)
                    {
                        oWEx2.scalePhaseData.FIRST.date = string.Format("{0:yyyyMMddHHmmss}", boWe.ErstDatetime) + "000";
                    }
                    else
                    {
                        oWEx2.scalePhaseData.FIRST.date = string.Format("{0:yyyyMMddHHmmss}", boWe.LSDatum) + "000";
                    }
                }

                #endregion

                #region Scale Data 2

                if (boWe.Zweitgewicht > 0)
                {
                    oWEx2.scalePhaseData.SECOND = new SECOND {scaleId = "1"};
                    if (boWe.LN2 != null)
                        oWEx2.scalePhaseData.SECOND.scaleNumber = boWe.LN2.Trim();

                    if (boWe.Zweitgewicht != null)
                        oWEx2.scalePhaseData.SECOND.amount = boWe.Zweitgewicht;

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

                #endregion
            }
            if (!string.IsNullOrEmpty(boWe.freightCarrierFreeText))
                oWEx2.freightCarrierFreeText = boWe.freightCarrierFreeText;

            if (!string.IsNullOrEmpty(boWe.recipientFreeText))
                oWEx2.recipientFreeText = boWe.recipientFreeText;

            if (!string.IsNullOrEmpty(boWe.supplierFreeText))
                oWEx2.supplierFreeText = boWe.supplierFreeText;

            if (!string.IsNullOrEmpty(boWe.supplierFreeText))
                oWEx2.releaseFreeText = boWe.releaseFreeText;

            // Neu 30.8.2015
            Log.Instance.Info("Export Wiegedaten: LS-NR: " + boWe.LieferscheinNr + "Erstgewicht/lfd Nr : " + boWe.LN1 +
                              " " + boWe.Erstgewicht.ToString() + " Zeitgewicht/lfd Nr: "
                              + boWe.LN2 + " " + boWe.Zweitgewicht.ToString() + " Nettogewicht :" +
                              boWe.Nettogewicht.ToString());
            /////////////////////////////////////////////////////

            #endregion
            
            #region REST ExportAll

            try
            {
                var client = new RestClient(baseUrl);

                client.ClearHandlers();
                //    client.Timeout = 15000;

                var request = new RestRequest("/rest/scale/set") {Method = Method.POST};
                request.Timeout = 60000;
                request.AddHeader("X-location-Id", boEe.RestLocation);
                request.AddHeader("Accept-Language", "de");
                request.RequestFormat = DataFormat.Json;


                var obj = JsonConvert.SerializeObject(oWEx2, Formatting.Indented,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});


                request.AddParameter("application/json; charset=utf-8", obj, ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;

                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                    boEe.ConsumerSecret.Trim(),
                    string.Empty, string.Empty);

                var response = client.Execute(request);

                //TODO:ExportAll Fehlschläge loggen - Erfolgreiche unvisible setzen
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //  WriteToExportLog(response, boWe);

                    //  Log.Instance.Error("Export: Request HttpStatusCode " + response.StatusCode);

                    // Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    Log.Instance.Error("Exportfehler - Antwort vom Restserver: " + response.StatusCode + ", Message: " +
                                       response.Content);


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

                        w.SaveEntity(we);
                        SetOrderItemServiceAsSend(we);
                        WriteToExportLog(response, boWe);
                    }
                }


                WriteToExportLog(response, boWe);
            }
            catch (Exception ee)
            {
                new WriteErrorLog().WriteToErrorLog(ee, boWe);
            }
        }

        private static void SetOrderItemServiceAsSend(WaegeEntity we)
        {
            // Änderung Bug
            OrderItemservice oIe = new OrderItemservice();
            OrderItemserviceEntity oIes = oIe.GetByOrderIdentifierItemService(we.identifierOItemService);
            if (oIes != null)
            {
                oIes.HasBinSended = true;
                oIes.HasBinUsed = true;
                var uRet = oIe.SaveEntity(oIes);
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
                    boEe.Message3 = we.LieferscheinNr;
                }
                else
                {
                    boEe.Message3 = we.LieferscheinNr;
                    boEe.Message2 = "ResponseStatus:" + response.ResponseStatus;
                    boEe.Message1 = "Response Error Exception: " + response.ErrorException.Message;
                }
                boEe.OrderItemNumber = we.number;
                boEe.OrderItemServiceIdentifier = we.identifierOItemService;
                boE.SaveEntity(boEe);
            }
            catch (Exception e)
            {
                new WriteErrorLog().WriteToErrorLog(e, we);
            }
        }
    }

    #endregion
}