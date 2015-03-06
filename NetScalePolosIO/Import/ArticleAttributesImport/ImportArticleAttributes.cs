using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleAttributes;
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

        public void Import(string baseUrl, int location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest(url) {Method = Method.GET};
                request.AddHeader("X-location-Id", location.ToString());

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return;


                var oA = JsonConvert.DeserializeObject<ArticleAttributesRootObject>(response.Content);
              

                _boA = new Artikelattribute();
                foreach (string t in oA.articleAttributes)
                {
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