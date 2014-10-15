using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using HWB.NETSCALE.BOEF;
using Microsoft.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HWB.NETSCALE.BOEF;

namespace HWB.NETSCALE.FRONTEND.WPF.Import
{
    public class ImportAddress
    {
       
        private HWB.NETSCALE.BOEF.Adressen boA;
        private AdressenEntity boAE;
        private bool StartReading;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {


                AddressRootObject oR = FullQualifiedFileName.CreateFromJsonFile<AddressRootObject>();


                boA = new BOEF.Adressen();

                foreach (AddressableEntity obj in oR.addressableEntities)
                {
                    if (obj.id != null)
                    {
                        boAE = boA.GetById(obj.id);

                        if (boAE == null)
                        {
                          boAE=  boA.NewEntity();
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
                MessageBox.Show(e.Message.ToString());
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