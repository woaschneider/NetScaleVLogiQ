using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ProductsImport;
using Newtonsoft.Json;
using RestSharp;
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