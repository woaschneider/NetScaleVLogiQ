using System;
using System.Net;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
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
        private ImportExportPolos _oIO;
        public ImportAddress(ImportExportPolos oIO)
        {
            _oIO = oIO;
        }

        public void Import(string baseUrl, string location, string url)
        {
            try
            {
                var client = new RestClient(baseUrl);
                client.ClearHandlers();
                client.AddHandler("application/json", new JsonDeserializer());

                client.Timeout = 15000;
                var request = new RestRequest(url) {Method = Method.GET};
                request.AddHeader("X-location-Id", location);
                request.AddHeader("Accept-Language", "de");

                Einstellungen boE = new Einstellungen();
                EinstellungenEntity boEe = boE.GetEinstellungen();
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(boEe.ConsumerKey.Trim(),
                    boEe.ConsumerSecret.Trim(),
                    string.Empty, string.Empty);

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Instance.Error("Adressen-Import:Request HttpStatusCode " + response.StatusCode);
                    if (response.StatusCode == 0)
                    {
                        Log.Instance.Error("Wahrscheinlich keine Verbindung zum REST-Server / Rest-Service!");
                    }
                    return;
                }



                var oR = JsonConvert.DeserializeObject<AddressRootObject>(response.Content);

                _boA = new Adressen();


                float recordCounter = 0;
                foreach (AddressableEntity obj in oR.addressableEntities)
                {
                    recordCounter = recordCounter + 1;
                    int totalNumberOfRecords = oR.addressableEntities.Count;
              
                    if (oR.addressableEntities.Count > 0)
                    {
                        try
                        {
                          _oIO.ProzentStammdaten = (int) (recordCounter / (oR.addressableEntities.Count / 100F));
                        }
                        catch (Exception e)
                        {
                            
                           
                        }
                       
                    }
                    
                    if (true)
                    {
                    //TODO: auf BI prüfen statt auf Id

                      //  _boAe = _boA.GetId(obj.id) ?? _boA.NewEntity();
                        _boAe = _boA.GetByBusinenessIdentifier(obj.businessIdentifier.Trim());

                        if (_boAe == null)
                        {
                          _boAe =  _boA.NewEntity();
                        }
                    
                       
                        _boAe.id = obj.id;
                        try
                        {
                            _boAe.businessIdentifier = obj.businessIdentifier.Trim();

                            _boAe.name = obj.name.Trim();
                        }
                        catch (Exception ee)
                        {

                            Log.Instance.Error("Fehler im AP Import: Bussines Identifier oder Name = Null ? " + ee.Message);
                        }
                        


                        _boAe.owningLocationId = obj.owningLocationId;
                        _boAe.subName2 = obj.subName2;

                        _boAe.street = obj.address.street;
                        _boAe.zipCode = obj.address.zipCode;
                        _boAe.city = obj.address.city;

                     //   _boAe.idCountry = obj.address.country.id;
                        _boAe.isocodeCountry = obj.address.country.isoCode;


                        request = new RestRequest("/rest/address/details/{ID}");

                        request.AddParameter("ID", _boAe.id.ToString(), ParameterType.UrlSegment);
                    //  request.AddParameter("ID", _boAe.businessIdentifier.ToString(), ParameterType.UrlSegment);
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