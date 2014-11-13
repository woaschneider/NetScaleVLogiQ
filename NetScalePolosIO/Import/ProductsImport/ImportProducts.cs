using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.ProductsImport
{
    public class ImportProducts
    {
        private Produkte boP;
        private ProdukteEntity boPE;

        public bool Import(string baseUrl)
        {
            try
            {

                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest("/rest/data/products");
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", "16");


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                var x = response.Content; // Nur für Testzwecke

                var oP = JsonConvert.DeserializeObject<ProduktRootObject > (response.Content);


                
                boP = new Produkte();

                foreach (Product obj in oP.products)
                {
                    if (obj.id != null)
                    {
                        boPE = boP.GetById(obj.id);
                    }
                    if (boPE == null)
                    {
                        boPE = boP.NewEntity();
                    }
                    boPE.id = obj.id;
                    boPE.description = obj.description;
                    boPE.shortdescirption = obj.description;
                    boP.SaveEntity(boPE);
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