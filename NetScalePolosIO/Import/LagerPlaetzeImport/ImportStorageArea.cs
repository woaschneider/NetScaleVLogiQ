using System;
using HWB.NETSCALE.BOEF;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.LagerPlaetzeImport
{
    public class ImportStorageArea
    {
        private Lagerplaetze boL;
        private LagerplaetzeEntity boLE;


        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                boL = new Lagerplaetze();
                var oL = FullQualifiedFileName.CreateFromJsonFile<LagerPlaetzeRootObject>();
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