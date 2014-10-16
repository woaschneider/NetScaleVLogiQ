using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.LagerPlaetzeImport
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