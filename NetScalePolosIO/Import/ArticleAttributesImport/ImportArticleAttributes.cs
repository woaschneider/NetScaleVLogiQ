using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleAttributes;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace NetScalePolosIO.Import.ArticleAttributesImport
{
    internal class ImportArticleAttributes
    {
        private Artikelattribute _boA;
        private ArtikelattributeEntity _boAe;

        public bool Import(string baseUrl, int location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest(url);
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", location.ToString());


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
            

                var oA = JsonConvert.DeserializeObject<ArticleAttributesRootObject>(response.Content);
              

                _boA = new Artikelattribute();
                for (int i = 0; i < oA.articleAttributes.Count; i++)
                {
                    _boAe = _boA.GetArtikelAttributByBezeichnung(oA.articleAttributes[i]);
                    if (_boAe == null)
                    {
                        _boAe = _boA.NewEntity();
                    }
                    if (_boAe != null)
                    {
                        _boAe.AttributName = oA.articleAttributes[i];
                        _boA.SaveEntity(_boAe);
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