namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
    public class Article
    {
        public int id { get; set; }
        public int ownerId { get; set; }
        public string locationId { get; set; }
        public string number { get; set; }
        public string kindOfGoodDescription { get; set; }
        public int kindOfGoodId { get; set; }
        public string description { get; set; }
        public BaseUnit baseUnit { get; set; }
        public ConversionUnit conversionUnit { get; set; }
    }
}