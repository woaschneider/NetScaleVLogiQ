using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

// Imports Kind of Goods nach Warenarten

namespace HWB.NETSCALE.POLOSIO.KindOfGoodsImport
{
    public class ImportKindsOfGoods
    {
        private Warenarten boW;
        private WarenartenEntity boWE;

        public bool Import(string baseUrl)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest("/rest/data/kindofgoods");
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", "16");


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                var x = response.Content; // Nur für Testzwecke

                var oK = JsonConvert.DeserializeObject<KindOfGoodsImportRootObject>(response.Content);


              
                boW = new Warenarten();

                foreach (KindOfGood obj in oK.kindOfGoods)
                {
                    if (obj.id != null)
                    {
                        boWE = boW.GetById(obj.id);
                    }
                    if (boWE == null)
                    {
                        boWE = boW.NewEntity();
                    }
                    boWE.id = obj.id;
                    boWE.description = obj.description;
                    boWE.baseunit_shortdescription = obj.baseUnit.shortDescription;
                    boW.SaveEntity(boWE);
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