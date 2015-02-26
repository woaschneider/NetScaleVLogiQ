using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ProductsImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace NetScalePolosIO.Import.ProductsImport
{
    public class ImportProducts
    {
        private Produkte _boP;
        private ProdukteEntity _boPe;

        public bool Import(string baseUrl, int location, string url)
        {
            try
            {

                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

           //     var request = new RestRequest("/rest/data/products") {Method = Method.GET};
                var request = new RestRequest("/rest/data/products") { Method = Method.GET };
                request.AddHeader("X-location-Id", location.ToString());

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(), boEe.ConsumerSecret.Trim(),
                   string.Empty, string.Empty);
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
            

                var oP = JsonConvert.DeserializeObject<ProduktRootObject > (response.Content);


                
                _boP = new Produkte();

                foreach (Product obj in oP.products)
                {
                    {
                        _boPe = _boP.GetById(obj.id);
                    }
                    if (_boPe == null)
                    {
                        _boPe = _boP.NewEntity();
                    }
                    _boPe.id = obj.id;
                    _boPe.description = obj.description;
                    _boPe.shortdescirption = obj.description;
                    _boP.SaveEntity(_boPe);

                    var oService = JsonConvert.DeserializeObject<Service>(response.Content);
                    foreach (Service S in obj.services)
                    {
                        int id = S.id;
                        string description = S.description;
                        // Prüfe ob das Produkt schon diese LEistung in seiner Tabelle hat
                        _boPe = _boP.GetById(_boPe.id);
                        if (_boPe!=null)
                        {   Serv _boServ = new Serv();

                            ServEntity boSerE = _boServ.GetById_Fk(S.id, _boPe.PK);
                            if (boSerE == null)
                            {
                                boSerE= new ServEntity();
                            }
                            boSerE.FK = _boPe.PK;
                            boSerE.id = id;
                            boSerE.description = description;

                            _boServ.SaveEntity(boSerE);

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