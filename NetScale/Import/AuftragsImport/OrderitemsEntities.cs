using System.Collections.Generic;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
    public class OrderitemsEntities
    {
        public List<OrderItem> orderItems { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public string reference { get; set; }
        public string orderState { get; set; }
        public Customer customer { get; set; }
        public InvoiceReceiver invoiceReceiver { get; set; }
        public string locationId { get; set; }
        public string date { get; set; }
    }
}