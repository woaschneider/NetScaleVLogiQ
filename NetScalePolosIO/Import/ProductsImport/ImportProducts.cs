using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using HWB.NETSCALE.POLOSIO.ProductsImport;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


namespace NetScalePolosIO.Import.ProductsImport
{
    public class ImportProducts
    {
        private Produkte _boP;
        private ProdukteEntity _boPe;

        public bool Import(string baseUrl, string location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());
                client.Timeout = 15000;
                //     var request = new RestRequest("/rest/data/products") {Method = Method.GET};
                var request = new RestRequest("/rest/data/products") {Method = Method.GET};
                request.AddHeader("X-location-Id", location.ToString());
                request.AddHeader("Accept-Language", "de");

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                    boEe.ConsumerSecret.Trim(),
                    string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Produkte-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return false;
                }

                var oP = JsonConvert.DeserializeObject<ProduktRootObject>(response.Content);


                _boP = new Produkte();
                int recordCounter = 0;
                foreach (Product obj in oP.products)
                {
                    recordCounter = recordCounter + 1;
                    if(oP.products.Count > 0)
                    goApp.ProzentStammdaten = recordCounter /(float) (oP.products.Count / 100);
                    foreach (Service s in obj.services)
                    {

                        if (s.scaleRelevant==true)
                        {
                            _boPe = _boP.GetById(obj.id);

                            if (_boPe == null)
                            {
                                _boPe = _boP.NewEntity();
                            }
                            _boPe.id = obj.id;
                            _boPe.description = obj.description;
                            _boPe.shortdescirption = obj.description;

                            _boPe.serviceId = s.id;
                            _boPe.serviceDescription = s.description;
                            _boPe.scaleRelevant = s.scaleRelevant; // Nur zum schauen

                            _boP.SaveEntity(_boPe); 
                        }
                    }

                   // if (obj.services[0].scaleRelevant == true)
                  //  {
                        
                 //   }
                    //var oService = JsonConvert.DeserializeObject<Service>(response.Content);
                    //foreach (Service S in obj.services)
                    //{
                      
                    //    string description = S.description;
                    //    // Prüfe ob das Produkt schon diese Leistung in seiner Tabelle hat
                    //    _boPe = _boP.GetById(_boPe.id);
                    //    if (_boPe != null)
                    //    {
                    //        Serv _boServ = new Serv();

                    //        ServEntity boSerE = _boServ.GetById_Fk(S.id, _boPe.PK);
                    //        if (boSerE == null)
                    //        {
                    //            boSerE = new ServEntity();
                    //        }
                    //        boSerE.FK = _boPe.PK;
                    //        boSerE.id =_boPe.id;
                    //        boSerE.description = description;

                    //        _boServ.SaveEntity(boSerE);
                    //    }
                    //}
                }
            }

            catch (Exception e)
            {
                Log.Instance.Error("Fehler im Produkt-Import: " + e.Message);
            }


            return true;
        }
    }
}