namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
    public class Clearance
    {
        public Unit unit { get; set; }
        public string validFrom { get; set; }
        public string validTo { get; set; }
        public int authorizerId { get; set; }
        public int granteeId { get; set; }
        public string reference { get; set; }
    }
}