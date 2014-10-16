using System;
using HWB.NETSCALE.BOEF;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO
{
    public class ImportAddress
    {
        private bool StartReading;
        private Adressen boA;
        private AdressenEntity boAE;

        public bool Import(string FullQualifiedFileName)
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


        private void ReadJsonObject(JsonToken jt)
        {
        }

        private void WriteToEntity()
        {
        }
    }
}