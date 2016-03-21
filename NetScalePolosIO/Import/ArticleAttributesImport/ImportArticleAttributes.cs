using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.ArticleAttributes;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;

namespace NetScalePolosIO.Import.ArticleAttributesImport
{
    internal class ImportArticleAttributes
    {
        private Artikelattribute _boA;
        private ArtikelattributeEntity _boAe;

        public void Import(string baseUrl, string location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());
                client.Timeout = 15000;
                var request = new RestRequest(url) {Method = Method.GET};
                request.AddHeader("X-location-Id", location.ToString());
                request.AddHeader("Accept-Language", "de");
                request.AddHeader("Accept-Encoding", "gzip");

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Artikelattribute-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return;
                }


                var oA = JsonConvert.DeserializeObject<ArticleAttributesRootObject>(response.Content);
              

                _boA = new Artikelattribute();
                int recordCounter = 0;
                foreach (string t in oA.articleAttributes)
                {
                    recordCounter = recordCounter + 1;
                    goApp.ProzentStammdaten = recordCounter / (oA.articleAttributes.Count / 100);

                    _boAe = _boA.GetArtikelAttributByBezeichnung(t) ?? _boA.NewEntity();
                    if (_boAe != null)
                    {
                        _boAe.AttributName = t;
                        _boA.SaveEntity(_boAe);
                    }
                }
            }

            catch (Exception e)
            {
               new WriteErrorLog().WriteToErrorLog(e,null);
            }


        }
    }
}