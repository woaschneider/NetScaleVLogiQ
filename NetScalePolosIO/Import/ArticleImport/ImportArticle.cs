using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.ArticleImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


namespace NetScalePolosIO.Import.ArticleImport
{
    internal class ImportArticle
    {
        private Artikel _boArtikel;
        private ArtikelEntity _boAe;

        public bool Import(string baseUrl, string location, string url)
        {
            try
            {

                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());
                client.Timeout = 15000;
               // var request = new RestRequest("/rest/article/all");
                var request = new RestRequest(url) {Method = Method.GET};
                request.AddHeader("X-location-Id", location.ToString());
                request.AddHeader("Accept-Language", "de");

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Artikel-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return false;
                }


                var oA = JsonConvert.DeserializeObject<ArticleRootObject>(response.Content);


             

                _boArtikel = new Artikel();
                Adressen boA = new Adressen();
                int recordCounter = 0;
                foreach (ArticleInformation obj in oA.articleInformation)
                {
                    recordCounter = recordCounter + 1;
                    goApp.ProzentStammdaten = recordCounter / (oA.articleInformation.Count / 100);

                    {
                        _boAe = _boArtikel.GetById(obj.article.id) ?? _boArtikel.NewEntity();
                        _boAe.id = obj.article.id;
                        _boAe.number = obj.article.number;
                        _boAe.description = obj.article.description;

                        _boAe.ownerId = obj.article.ownerId;





                        // Neu 29.12.2015
                        
                     _boAe.ownerBI = boA.GetById(_boAe.ownerId).businessIdentifier;

                        _boAe.kindOfGoodId = obj.article.kindOfGoodId;
                        _boAe.kindOfGoodDescription = obj.article.kindOfGoodDescription;
                        
                        // Geändert am 29.12.2015
                        _boAe.locationId = obj.article.locationId;
                       

                        _boAe.baseUnitId = obj.article.baseUnit.id;
                        _boAe.baseUnitShortDescription = obj.article.baseUnit.shortDescription;
                        _boAe.baseUnitDescription = obj.article.baseUnit.description;

                        if (obj.article.conversionUnit != null)
                        {
                            _boAe.conversionUnitId = obj.article.conversionUnit.id;
                            _boAe.conversionUnitDescription = obj.article.conversionUnit.description;
                            _boAe.conversionUnitShortDescription = obj.article.conversionUnit.shortDescription;
                           
                        }
                        
                        // Neu 06.01.2015
                       _boAe.attributes_as_json =  JsonConvert.SerializeObject(obj.attributes);
                       
                        //
                 
                        //
                        _boArtikel.SaveEntity(_boAe);
                    }
                }
            }

            catch (Exception e)
            {
                Log.Instance.Error("Fehler im Artikel-Import: " + e.Message);
                return false;
            }

            return true;
        }
    }
}