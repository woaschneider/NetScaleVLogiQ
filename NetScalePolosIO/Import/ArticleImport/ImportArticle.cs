using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO
{
    internal class ImportArticle
    {
        private Artikel boA;
        private ArtikelEntity boAE;

        public bool Import(string baseUrl)
        {
            try
            {

                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest("/rest/article/all");
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", "16");


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                var x = response.Content; // Nur für Testzwecke

                var oA = JsonConvert.DeserializeObject<ArticleRootObject>(response.Content);


             

                boA = new Artikel();

                foreach (ArticleInformation obj in oA.articleInformation)
                {
                    if (obj.article.id != null)
                    {
                        boAE = boA.GetById(obj.article.id);
                        if (boAE == null)
                        {
                            boAE = boA.NewEntity();
                        }
                        boAE.id = obj.article.id;
                        boAE.number = obj.article.number;
                        boAE.ownerId = obj.article.ownerId.ToString();
                        boAE.kindOfGoodId = obj.article.kindOfGoodId;
                        boAE.kindOfGoodDescription = obj.article.kindOfGoodDescription;
                        boAE.locationId = obj.article.ownerId.ToString();


                        boAE.baseUnitId = obj.article.baseUnit.id;
                        boAE.baseUnitShortDescription = obj.article.baseUnit.shortDescription;
                        boAE.baseUnitDescription = obj.article.baseUnit.description;

                        if (obj.article.conversionUnit != null)
                        {
                            boAE.conversionUnitId = obj.article.conversionUnit.id;
                            boAE.conversionUnitDescription = obj.article.conversionUnit.description;
                            boAE.conversionUnitShortDescription = obj.article.conversionUnit.shortDescription;
                            boA.SaveEntity(boAE);
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