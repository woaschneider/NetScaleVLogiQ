using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using NetScalePolosIO.Import.AddressImport;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;
// http://www.codeproject.com/Tips/668625/Simple-Usages-of-HttpWebRequest-and-RestSharp-with

namespace HWB.NETSCALE.POLOSIO
{
    public class ImportAddress
    {
        private bool StartReading;
        private Adressen boA;
        private AdressenEntity boAE;

        public bool Import_Obsolete(string FullQualifiedFileName)
        {
            try
            {
                var oR = FullQualifiedFileName.CreateFromJsonFile<AddressRootObject>();


                boA = new Adressen();

                foreach (AddressableEntity obj in oR.addressableEntities)
                {
                    if (obj.id != null)
                    {
                        boAE = boA.GetById(obj.id);

                        if (boAE == null)
                        {
                            boAE = boA.NewEntity();
                        }
                        boAE.id = obj.id;
                        boAE.businessIdentifier = obj.businessIdentifier;
                        boAE.name = obj.name;
                        boAE.owningLocationId = obj.owningLocationId;
                        boAE.subName2 = obj.subName;

                        boAE.street = obj.address.street;
                        boAE.zipCode = obj.address.zipCode;
                        boAE.city = obj.address.city;

                        boAE.idCountry = obj.address.country.id;
                        boAE.isocodeCountry = obj.address.country.isoCode;
                        boA.SaveEntity(boAE);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            return true;
        }

        public bool Import(string baseUrl)
        {
            try
            {
              
               
                var client = new RestClient( baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer ());
              
                var request = new RestRequest("/rest/address/all");
                request.Method = Method.GET;
                request.AddHeader("X-location-Id", "16");
            
               
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                var x = response.Content; // Nur für Testzwecke
               
                var oR = JsonConvert.DeserializeObject<AddressRootObject>(response.Content);

                boA = new Adressen();

                foreach (AddressableEntity obj in oR.addressableEntities)
                {
                    if (obj.id != null)
                    {
                        boAE = boA.GetById(obj.id);

                        if (boAE == null)
                        {
                            boAE = boA.NewEntity();
                        }
                        boAE.id = obj.id;
                        boAE.businessIdentifier = obj.businessIdentifier;
                        boAE.name = obj.name;
                        boAE.owningLocationId = obj.owningLocationId;
                        boAE.subName2 = obj.subName;

                        boAE.street = obj.address.street;
                        boAE.zipCode = obj.address.zipCode;
                        boAE.city = obj.address.city;

                        boAE.idCountry = obj.address.country.id;
                        boAE.isocodeCountry = obj.address.country.isoCode;

                 
                       
                         request = new RestRequest("/rest/address/details/{ID}");

                        request.AddParameter("ID", boAE.id.ToString(), ParameterType.UrlSegment);
                        request.Method = Method.GET;
                        request.AddHeader("X-location-Id", "16");
               
                        response = client.Execute(request);
                        // Nur für Testzwecke
                        x = response.Content;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            boAE.roleClient = false;
                            boAE.roleInvoiceReceiver = false;
                            boAE.roleCarrier = false;
                            boAE.roleReceiver = false;
                            boAE.roleShipOwner = false;
                            boAE.roleStorageClient = false;
                            boAE.rolleSupplier = false;
                            boAE.roleTrainOperator = false;

                            var oRoles = JsonConvert.DeserializeObject<AddressRolesRootObject>(response.Content);
                            for (int i = 0; i < oRoles.roles.Count; i++)
                            {

                            

                                // Client
                                if (oRoles.roles[i] == "CLIENT")
                                {
                                    boAE.roleClient = true;
                                }
                               
                                // INVOICE Receiver
                                if (oRoles.roles[i] == "BILLING_RECEIVER")
                                {
                                    boAE.roleInvoiceReceiver = true;
                                }

                                // Storage_Client
                                if (oRoles.roles[i] == "STORAGE_CLIENT")
                                {
                                    boAE.roleStorageClient = true;
                                }

                                // Supplier
                                if (oRoles.roles[i] == "SUPPLIER")
                                {
                                    boAE.rolleSupplier = true;
                                }

                                //  Receiver
                                if (oRoles.roles[i] == "RECEIVER")
                                {
                                    boAE.roleReceiver = true;
                                }

                                // Carrier
                                if (oRoles.roles[i] == "CARRIER")
                                {
                                    boAE.roleCarrier = true;
                                }

                                // Shipowner
                                if (oRoles.roles[i] == "SHIPOWNER")
                                {
                                    boAE.roleShipOwner = true;
                                }

                                // TRAIN_OPERATOR
                                if (oRoles.roles[i] == "TRAIN_OPERATOR")
                                {
                                    boAE.roleTrainOperator = true;
                                }

                            }
                        }
                        boA.SaveEntity(boAE);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            return true;
        }

        private void ReadJsonObject(JsonToken jt)
        {
        }

        private void WriteToEntity()
        {
        }
    }
}