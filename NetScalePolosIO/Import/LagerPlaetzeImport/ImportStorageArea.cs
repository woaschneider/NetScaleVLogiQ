using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.LagerPlaetzeImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


namespace NetScalePolosIO.Import.LagerPlaetzeImport
{
    public class ImportStorageArea
    {
        private Lagerplaetze _boL;
        private LagerplaetzeEntity _boLe;


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
                    Log.Instance.Error("Storage-Area-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return false;

                }
            

                var oL = JsonConvert.DeserializeObject<LagerPlaetzeRootObject>(response.Content);
         
               
                _boL = new Lagerplaetze();
                int recordCounter = 0;
                foreach (PolosStorageArea obj in oL.storageAreas)
                {
                    recordCounter = recordCounter + 1;
                    goApp.ProzentStammdaten = recordCounter / (oL.storageAreas.Count / 100);
                    if (obj.id != null)
                    {
                        _boLe = _boL.GetById(obj.id) ?? _boL.NewEntity();
                        if (_boLe != null)
                        {
                            _boLe.id = obj.id;
                            _boLe.locationid = obj.locationId;
                            _boLe.name = obj.name;
                            _boLe.fullname = obj.fullName;

                            _boL.SaveEntity(_boLe);
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