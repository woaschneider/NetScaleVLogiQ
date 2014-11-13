using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.ArticleAttributes
{
    internal class ImportArticleAttributes
    {
        private Artikelattribute boA;
        private ArtikelattributeEntity boAE;

        public bool Import(string baseUrl)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest("/rest/article/attributes");
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", "16");


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                var x = response.Content; // Nur für Testzwecke

                var oA = JsonConvert.DeserializeObject<ArticleAttributesRootObject>(response.Content);
              

                boA = new Artikelattribute();
                for (int i = 0; i < oA.articleAttributes.Count; i++)
                {
                    boAE = boA.GetArtikelAttributByBezeichnung(oA.articleAttributes[i]);
                    if (boAE == null)
                    {
                        boAE = boA.NewEntity();
                    }
                    if (boAE != null)
                    {
                        boAE.AttributName = oA.articleAttributes[i];
                        boA.SaveEntity(boAE);
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