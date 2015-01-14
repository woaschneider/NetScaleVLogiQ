using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace NetScalePolosIO.Import.ArticleImport
{
    internal class ImportArticle
    {
        private Artikel _boA;
        private ArtikelEntity _boAe;

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
         

                var oA = JsonConvert.DeserializeObject<ArticleRootObject>(response.Content);


             

                _boA = new Artikel();

                foreach (ArticleInformation obj in oA.articleInformation)
                {
                    {
                        _boAe = _boA.GetById(obj.article.id);
                        if (_boAe == null)
                        {
                            _boAe = _boA.NewEntity();
                        }
                        _boAe.id = obj.article.id;
                        _boAe.number = obj.article.number;
                        _boAe.description = obj.article.description;
                        _boAe.ownerId = obj.article.ownerId.ToString();
                        _boAe.kindOfGoodId = obj.article.kindOfGoodId;
                        _boAe.kindOfGoodDescription = obj.article.kindOfGoodDescription;
                        _boAe.locationId = obj.article.ownerId.ToString();


                        _boAe.baseUnitId = obj.article.baseUnit.id;
                        _boAe.baseUnitShortDescription = obj.article.baseUnit.shortDescription;
                        _boAe.baseUnitDescription = obj.article.baseUnit.description;

                        if (obj.article.conversionUnit != null)
                        {
                            _boAe.conversionUnitId = obj.article.conversionUnit.id;
                            _boAe.conversionUnitDescription = obj.article.conversionUnit.description;
                            _boAe.conversionUnitShortDescription = obj.article.conversionUnit.shortDescription;
                            _boA.SaveEntity(_boAe);
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