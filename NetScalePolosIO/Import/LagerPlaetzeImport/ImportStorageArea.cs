using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.LagerPlaetzeImport
{
    public class ImportStorageArea
    {
        private Lagerplaetze boL;
        private LagerplaetzeEntity boLE;


        public bool Import(string baseUrl)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                var request = new RestRequest("/rest/data/storageareas");
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", "16");


                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                var x = response.Content; // Nur für Testzwecke

                var oL = JsonConvert.DeserializeObject<LagerPlaetzeRootObject>(response.Content);
         
               
                boL = new Lagerplaetze();
                foreach (PolosStorageArea obj in oL.storageAreas)
                {
                    if (obj.id != null)
                    {
                        boLE = boL.GetById(obj.id);
                        if (boLE == null)
                        {
                            boLE = boL.NewEntity();
                        }
                        if (boLE != null)
                        {
                            boLE.id = obj.id;
                            boLE.locationid = obj.locationId;
                            boLE.name = obj.name;
                            boLE.fullname = obj.fullName;

                            boL.SaveEntity(boLE);
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