namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
    public class Customer
    {
        public int id { get; set; }
        public string businessIdentifier { get; set; }
        public string name { get; set; }
        public string owningLocationId { get; set; }
        public string subName { get; set; }
        public Address2 address { get; set; }
    }
}