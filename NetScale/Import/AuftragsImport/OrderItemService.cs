namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
    public class OrderItemService
    {
        public string identifier { get; set; }
        public string remark { get; set; }
        public int sequence { get; set; }
        public ArticleInstance articleInstance { get; set; }
        public string state { get; set; }
        public SupplierOrConsignee supplierOrConsignee { get; set; }
        public string plannedBeginDate { get; set; }
        public string plannedEndDate { get; set; }
        public Service service { get; set; }
        public Clearance clearance { get; set; }
        public double targetAmount { get; set; }
        public string deliveryType { get; set; }
    }
}