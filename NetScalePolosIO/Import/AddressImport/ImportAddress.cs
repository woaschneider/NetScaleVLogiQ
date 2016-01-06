using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO;
using NetScalePolosIO.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


// http://www.codeproject.com/Tips/668625/Simple-Usages-of-HttpWebRequest-and-RestSharp-with

namespace NetScalePolosIO.Import.AddressImport
{
    public class ImportAddress
    {
        private Adressen _boA;
        private AdressenEntity _boAe;
       


        public void Import(string baseUrl, string location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());


                var request = new RestRequest(url) {Method = Method.GET};
                request.AddHeader("X-location-Id", location);

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                    boEe.ConsumerSecret.Trim(),
                    string.Empty, string.Empty);

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return;


                var oR = JsonConvert.DeserializeObject<AddressRootObject>(response.Content);

                _boA = new Adressen();

                foreach (AddressableEntity obj in oR.addressableEntities)
                {
                    if (true)
                    {
                    //TODO: auf BI prüfen statt auf Id

                      //  _boAe = _boA.GetById(obj.id) ?? _boA.NewEntity();
                        _boAe = _boA.GetByBusinenessIdentifier(obj.businessIdentifier) ?? _boA.NewEntity();

                        _boAe.id = obj.id;
                        _boAe.businessIdentifier = obj.businessIdentifier;

                        _boAe.name = obj.name;


                        _boAe.owningLocationId = obj.owningLocationId;
                        _boAe.subName2 = obj.subName2;

                        _boAe.street = obj.address.street;
                        _boAe.zipCode = obj.address.zipCode;
                        _boAe.city = obj.address.city;

                     //   _boAe.idCountry = obj.address.country.id;
                        _boAe.isocodeCountry = obj.address.country.isoCode;


                        request = new RestRequest("/rest/address/details/{ID}");

                        request.AddParameter("ID", _boAe.id.ToString(), ParameterType.UrlSegment);
                        request.Method = Method.GET;
                        request.AddHeader("X-location-Id", location);

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
                            {// Änderung am 04.01.2016
                                // Auftraggeber
                                if (t == "CUSTOMER") // vorher Client
                                {
                                    _boAe.roleClient = true;
                                }

                                // INVOICE Receiver
                                if (t == "BILLING_RECEIVER")
                                {
                                    _boAe.roleInvoiceReceiver = true;
                                }

                                // Storage_Client
                                if (t == "CLIENT") // Vor her Storage Client
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
                Log.Instance.Error("Fehler im AP Import: "+ e.Message);
                
            }


   
        }
    }
}