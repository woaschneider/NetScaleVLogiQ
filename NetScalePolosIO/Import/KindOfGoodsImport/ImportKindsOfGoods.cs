using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.KindOfGoodsImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

// Imports Kind of Goods nach Warenarten

namespace NetScalePolosIO.Import.KindOfGoodsImport
{
    public class ImportKindsOfGoods
    {
        private Warenarten _boW;
        private WarenartenEntity _boWe;

        public bool Import(string baseUrl)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest("/rest/data/kindofgoods") {Method = Method.GET};
                request.AddHeader("X-location-Id", "16");


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
            

                var oK = JsonConvert.DeserializeObject<KindOfGoodsImportRootObject>(response.Content);


              
                _boW = new Warenarten();

                foreach (KindOfGood obj in oK.kindOfGoods)
                {
                    if (obj.id != null)
                    {
                        _boWe = _boW.GetById(obj.id);
                    }
                    if (_boWe == null)
                    {
                        _boWe = _boW.NewEntity();
                    }
                    _boWe.id = obj.id;
                    _boWe.description = obj.description;
                    _boWe.baseunit_shortdescription = obj.baseUnit.shortDescription;
                    _boW.SaveEntity(_boWe);
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