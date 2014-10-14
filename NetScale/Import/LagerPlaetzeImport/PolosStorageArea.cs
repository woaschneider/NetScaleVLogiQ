using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.LagerPlaetzeImport
{
   public class PolosStorageArea
    {
        public string id { get; set; }
        public string name { get; set; }
        public string locationId { get; set; }
        public string fullName { get; set; }
    }

   public class LagerPlaetzeRootObject
   {
       public List<PolosStorageArea> storageAreas { get; set; }
   }
}
