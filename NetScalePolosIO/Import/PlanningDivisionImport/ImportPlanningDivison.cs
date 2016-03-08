using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HWB.NETSCALE.BOEF;
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

           //     var request = new RestRequest("/rest/data/storageareas") {Method = Method.GET};
                var request = new RestRequest(url) { Method = Method.GET };
                request.AddHeader("X-location-Id", location.ToString());

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
            

                var oP = JsonConvert.DeserializeObject<PlanningDivisonRootObject>(response.Content);
         
               
                _boP = new Planningdivision();
                foreach (PolosPlanningdivison obj in oP.planningdivisons)
                {
                    if (obj.id != null)
                    {
                        _boPe = _boP.GetById(obj.id) ?? _boP.NewEntity();
                        if (_boPe != null)
                        {
                            _boPe.id = obj.id;
                            _boPe.description = obj.description;
                            _boPe.active = obj.active;
                            

                            _boP.SaveEntity(_boPe);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error("Fehler im Storage-Area-Import: " + e.Message);
            }
            return true;
        }
    }


    }

