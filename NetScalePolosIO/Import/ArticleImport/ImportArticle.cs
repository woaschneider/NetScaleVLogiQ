using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


namespace NetScalePolosIO.Import.ArticleImport
{
    internal class ImportArticle
    {
        private Artikel _boA;
        private ArtikelEntity _boAe;

        public void Import(string baseUrl, int location, string url)
        {
            try
            {

                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

               // var request = new RestRequest("/rest/article/all");
                var request = new RestRequest(url) {Method = Method.GET};
                request.AddHeader("X-location-Id", location.ToString());

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return;


                var oA = JsonConvert.DeserializeObject<ArticleRootObject>(response.Content);


             

                _boA = new Artikel();

                foreach (ArticleInformation obj in oA.articleInformation)
                {
                    {
                        _boAe = _boA.GetById(obj.article.id) ?? _boA.NewEntity();
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
                           
                        }
                        _boA.SaveEntity(_boAe);
                    }
                }
            }

            catch (Exception e)
            {
               new WriteErrorLog().WriteToErrorLog(e,null);
             //   MessageBox.Show(e.Message);
            }


        }
    }
}