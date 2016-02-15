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


            // Diese Prüfung reicht nicht! Das muss angepaßt werden. 
            if (boWe.identifierOItem != null)
            {
                Export2Rest(baseUrl, boWe);
            }
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
            //if (oWEx2.orderItemServiceId == null)
            //{
            oWEx2.customerBusinessIdentifier = boWe.customerBusinessIdentifier;
            //}

            // Artikel       
            oWEx2.articleInstance = new ArticleInstance {article = new Article {id = boWe.articleId}};


           
            #region Artikelattribute
            if (boWe.attributes_as_json != null)
            {
                JObject attObj = JObject.Parse(boWe.attributes_as_json);

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

                        // Datentyp für Attribut erfragen
                        // string, numeric, float und int
                        Artikelattribute boAa = new Artikelattribute();
                        ArtikelattributeEntity boAae = boAa.GetArtikelAttributByBezeichnung(propName);
                        if (boAae != null)
                        {
                            if (boAae.Datatyp != null)
                            {
                                propDataTyp = boAae.Datatyp;
                            }

                            string dt = propDataTyp;
                            switch (dt)
                            {
                                case "string":
                                    att[counter] = propName + ": " + propValue;
                                    break;
                                case "numeric":
                                    att[counter] = propName + ": " + propValue.Replace(",", ".");
                                    break;
                                case "float":
                                    att[counter] = propName + ": " + propValue.Replace(",", ".");
                                    break;
                                case "int":
                                    att[counter] = propName + ": " + propValue.Replace(",", "");
                                    break;
                                default:
                                    att[counter] = propName + ": " + '\u0022' + propValue + '\u0022';
                                    break;
                            }
                        }


                 oWEx2.articleInstance.attributes.BATCH = '\u0022' + propValue + '\u0022';
                  //      oWEx2.articleInstance.attributes.BATCH =  propValue;
                        counter = counter + 1;
                    }
                }
            }
            #endregion


            if (boWe.Erstgewicht > 0 | boWe.Zweitgewicht > 0)
            {
                oWEx2.scalePhaseData = new ScalePhaseData();
                if (boWe.Erstgewicht > 0)
                {
                    oWEx2.scalePhaseData.FIRST = new FIRST {scaleId = "3"};
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


                if (boWe.Zweitgewicht > 0)
                {
                    oWEx2.scalePhaseData.SECOND = new SECOND {scaleId = "3"};
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
            }
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


                var request = new RestRequest("/rest/scale/set") {Method = Method.POST};

                request.AddHeader("X-location-Id", boEe.RestLocation);
                request.AddHeader("Accept-Language", "de");
                request.RequestFormat = DataFormat.Json;

                //  var obj = request.JsonSerializer.Serialize(oWEx2);
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
                new WriteErrorLog().WriteToErrorLog(ee, boWe);
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