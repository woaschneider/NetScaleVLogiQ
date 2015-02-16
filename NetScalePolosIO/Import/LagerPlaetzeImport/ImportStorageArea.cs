using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.LagerPlaetzeImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace NetScalePolosIO.Import.LagerPlaetzeImport
{
    public class ImportStorageArea
    {
        private Lagerplaetze _boL;
        private LagerplaetzeEntity _boLe;


        public bool Import(string baseUrl, int location, string url)
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
            

                var oL = JsonConvert.DeserializeObject<LagerPlaetzeRootObject>(response.Content);
         
               
                _boL = new Lagerplaetze();
                foreach (PolosStorageArea obj in oL.storageAreas)
                {
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
                MessageBox.Show(e.Message);
            }
            return true;
        }
    }
}