using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.KindOfGoodsImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


// Imports Kind of Goods nach Warenarten

namespace NetScalePolosIO.Import.KindOfGoodsImport
{
    public class ImportKindsOfGoods
    {
        private Warenarten _boW;
        private WarenartenEntity _boWe;
        private ImportExportPolos _oIO;
        public ImportKindsOfGoods(ImportExportPolos oIO)
        {
            _oIO = oIO;
        }
        public bool Import(string baseUrl, string location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());
                client.Timeout = 15000;
             //   var request = new RestRequest("/rest/data/kindofgoods") {Method = Method.GET};
                var request = new RestRequest(url) { Method = Method.GET };
                request.AddHeader("X-location-Id", location.ToString());
                request.AddHeader("Accept-Language", "de");

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Warenarten-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return false;
                }
            

                var oK = JsonConvert.DeserializeObject<KindOfGoodsImportRootObject>(response.Content);


              
                _boW = new Warenarten();
                int recordCounter = 0;
                foreach (KindOfGood obj in oK.kindOfGoods)
                {
                    recordCounter = recordCounter + 1;
                    if (oK.kindOfGoods.Count > 0)
                    {
                        _oIO.ProzentStammdaten =(int) (recordCounter/(float)(oK.kindOfGoods.Count/100F));
                    }
                    if (obj.id != null)
                    {
                        _boWe = _boW.GetById(obj.id);
                    }
                    if (_boWe == null)
                    {
                        _boWe = _boW.NewEntity();
                    }
                    _boWe.id = obj.id;
                    _boWe.description = obj.description;
                    _boWe.baseunit_shortdescription = obj.baseUnit.shortDescription;
                    _boW.SaveEntity(_boWe);
                }
            }

            catch (Exception e)
            {
                new WriteErrorLog().WriteToErrorLog(e,null);
            }


            return true;
        }
    }
}