using System;
using System.Net;
using HWB.NETSCALE.BOEF;

using HWB.NETSCALE.POLOSIO;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using Xceed.Wpf.Toolkit;

// http://www.codeproject.com/Tips/668625/Simple-Usages-of-HttpWebRequest-and-RestSharp-with

namespace NetScalePolosIO.Import.AddressImport
{
    public class ImportAddress
    {
    
        private Adressen _boA;
        private AdressenEntity _boAe;

   

        public bool Import(string baseUrl)
        {
            try
            {
              
               
                var client = new RestClient( baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer ());
              
                var request = new RestRequest("/rest/address/all") {Method = Method.GET};
                request.AddHeader("X-location-Id", "16");
            
               
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
            
               
                var oR = JsonConvert.DeserializeObject<AddressRootObject>(response.Content);

                _boA = new Adressen();

                foreach (AddressableEntity obj in oR.addressableEntities)
                {
                    if (true)
                    {
                        _boAe = _boA.GetById(obj.id) ?? _boA.NewEntity();
                     
                        _boAe.id = obj.id;
                        _boAe.businessIdentifier = obj.businessIdentifier;
                        _boAe.name = obj.name;
                        _boAe.owningLocationId = obj.owningLocationId;
                        _boAe.subName2 = obj.subName;

                        _boAe.street = obj.address.street;
                        _boAe.zipCode = obj.address.zipCode;
                        _boAe.city = obj.address.city;

                        _boAe.idCountry = obj.address.country.id;
                        _boAe.isocodeCountry = obj.address.country.isoCode;

                 
                       
                         request = new RestRequest("/rest/address/details/{ID}");

                        request.AddParameter("ID", _boAe.id.ToString(), ParameterType.UrlSegment);
                        request.Method = Method.GET;
                        request.AddHeader("X-location-Id", "16");
               
                        response = client.Execute(request);
                    
                    
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            _boAe.roleClient = false;
                            _boAe.roleInvoiceReceiver = false;
                            _boAe.roleCarrier = false;
                            _boAe.roleReceiver = false;
                            _boAe.roleShipOwner = false;
                            _boAe.roleStorageClient = false;
                            _boAe.rolleSupplier = false;
                            _boAe.roleTrainOperator = false;

                            var oRoles = JsonConvert.DeserializeObject<AddressRolesRootObject>(response.Content);
                            foreach (string t in oRoles.roles)
                            {
// Client
                                if (t == "CLIENT")
                                {
                                    _boAe.roleClient = true;
                                }
                               
                                // INVOICE Receiver
                                if (t == "BILLING_RECEIVER")
                                {
                                    _boAe.roleInvoiceReceiver = true;
                                }

                                // Storage_Client
                                if (t == "STORAGE_CLIENT")
                                {
                                    _boAe.roleStorageClient = true;
                                }

                                // Supplier
                                if (t == "SUPPLIER")
                                {
                                    _boAe.rolleSupplier = true;
                                }

                                //  Receiver
                                if (t == "RECEIVER")
                                {
                                    _boAe.roleReceiver = true;
                                }

                                // Carrier
                                if (t == "CARRIER")
                                {
                                    _boAe.roleCarrier = true;
                                }

                                // Shipowner
                                if (t == "SHIPOWNER")
                                {
                                    _boAe.roleShipOwner = true;
                                }

                                // TRAIN_OPERATOR
                                if (t == "TRAIN_OPERATOR")
                                {
                                    _boAe.roleTrainOperator = true;
                                }
                            }
                        }
                        _boA.SaveEntity(_boAe);
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