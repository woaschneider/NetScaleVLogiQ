using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.LagerPlaetzeImport;
using HWB.NETSCALE.POLOSIO.PlanningDivisionImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;

namespace NetScalePolosIO.Import.PlanningDivisionImport
{
    class ImportPlanningDivison
    {

          private Planningdivision _boP;
        private PlanningdivisionEntity _boPe;


        public bool Import(string baseUrl, string location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());
                client.Timeout = 15000;
           //     var request = new RestRequest("/rest/data/storageareas") {Method = Method.GET};
                var request = new RestRequest(url) { Method = Method.GET };
                request.AddHeader("X-location-Id", location.ToString());

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Dispobereiche-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return false;
                }
            

                var oP = JsonConvert.DeserializeObject<PlanningDivisonRootObject>(response.Content);
         
               
                _boP = new Planningdivision();
                int recordCounter = 0;
                foreach (PolosPlanningdivison obj in oP.planningDivisions)
                {
                    recordCounter = recordCounter + 1;
                    //if (oP.planningdivisons.Count > 0)
                    //{
                    //    goApp.ProzentStammdaten = recordCounter/(float)(oP.planningdivisons.Count/100);
                    //}
                    if (obj.id != null)
                    {
                        _boPe = _boP.GetById(obj.id);
                        if (_boPe == null)
                        {
                      _boPe =      _boP.NewEntity();
                        }
                        if (_boPe != null)
                        {
                            _boPe.id = obj.id;
                            _boPe.description = obj.description;
                            _boPe.active = obj.active;
                            

                      var uRet =      _boP.SaveEntity(_boPe);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error("Fehler im Planingdivision-Import: " + e.Message);
            }
            return true;
        }
    }


    }

