using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.LagerPlaetzeImport
{
    public class PolosStorageArea
    {
        public string id { get; set; }
        public string name { get; set; }
        public string locationId { get; set; }
        public string fullName { get; set; }
        public string parentId { get; set; }
        public string storageAreaType { get; set; }
        public string revisionId { get; set; }
    }

    public class LagerPlaetzeRootObject
    {
        public List<PolosStorageArea> storageAreas { get; set; }
    }
}