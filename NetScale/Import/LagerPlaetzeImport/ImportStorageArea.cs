using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HWB.NETSCALE.BOEF;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.LagerPlaetzeImport
{
 public   class ImportStorageArea
 {

     private Lagerplaetze boL;
     private LagerplaetzeEntity boLE;


     public bool Import(string FullQualifiedFileName)
     {

         try
         {
             boL = new Lagerplaetze();
             LagerPlaetzeRootObject oL = FullQualifiedFileName.CreateFromJsonFile<LagerPlaetzeRootObject>();
             foreach (PolosStorageArea obj in oL.storageAreas)
             {
                 if(obj.id!=null)
                 {
                     boLE = boL.GetById(obj.id);
                     if( boLE==null)
                     {
                         boLE = boL.NewEntity();
                     }
                     if (boLE !=null)
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
             
              MessageBox.Show(e.Message.ToString());
         }
         return true;
     }
 }
}
