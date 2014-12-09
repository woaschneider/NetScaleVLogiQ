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
              
                var request = new RestRequest("/rest/address/all");
                request.Method = Method.GET;
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
                        _boAe = _boA.GetById(obj.id);

                        if (_boAe == null)
                        {
                            _boAe = _boA.NewEntity();
                        }
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
                            for (int i = 0; i < oRoles.roles.Count; i++)
                            {

                            

                                // Client
                                if (oRoles.roles[i] == "CLIENT")
                                {
                                    _boAe.roleClient = true;
                                }
                               
                                // INVOICE Receiver
                                if (oRoles.roles[i] == "BILLING_RECEIVER")
                                {
                                    _boAe.roleInvoiceReceiver = true;
                                }

                                // Storage_Client
                                if (oRoles.roles[i] == "STORAGE_CLIENT")
                                {
                                    _boAe.roleStorageClient = true;
                                }

                                // Supplier
                                if (oRoles.roles[i] == "SUPPLIER")
                                {
                                    _boAe.rolleSupplier = true;
                                }

                                //  Receiver
                                if (oRoles.roles[i] == "RECEIVER")
                                {
                                    _boAe.roleReceiver = true;
                                }

                                // Carrier
                                if (oRoles.roles[i] == "CARRIER")
                                {
                                    _boAe.roleCarrier = true;
                                }

                                // Shipowner
                                if (oRoles.roles[i] == "SHIPOWNER")
                                {
                                    _boAe.roleShipOwner = true;
                                }

                                // TRAIN_OPERATOR
                                if (oRoles.roles[i] == "TRAIN_OPERATOR")
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